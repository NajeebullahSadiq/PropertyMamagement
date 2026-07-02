-- =============================================================================
-- Migration: Update LicenseApplications ApplicantElectronicNumber unique index
-- Date: 2026-07-02
-- Module: ثبت درخواست متقاضیان جواز رهنمای معاملات
--
-- Problem:
--   Delete is soft (Status = false), but the unique index on ApplicantElectronicNumber
--   only filtered NULL/empty values — not soft-deleted rows.
--   Re-entering the same electronic number after delete fails with:
--   23505 duplicate key value violates unique constraint
--   "IX_LicenseApplications_ApplicantElectronicNumber"
--
-- Fix:
--   Replace the index with a filtered unique index:
--   unique only when Status = true AND value is not null/empty.
--
-- Data safety:
--   - Does NOT delete, update, or truncate any table data.
--   - Only drops and recreates an index.
--   - Aborts before any change if active records already have duplicate electronic numbers.
--   - Wrapped in a transaction (rolls back on error).
--
-- Prerequisite:
--   If you have not yet run the RequestSerialNumber fix, run that script first:
--   20260702_UpdateLicenseApplicationRequestSerialUniqueIndex.sql
--
-- How to run (production):
--   psql -h <host> -U <user> -d <database> -f 20260702_UpdateLicenseApplicationApplicantElectronicNumberUniqueIndex.sql
-- =============================================================================

BEGIN;

-- -----------------------------------------------------------------------------
-- Step 1: Pre-flight checks (read-only)
-- -----------------------------------------------------------------------------

DO $$
DECLARE
    total_rows INTEGER;
    active_rows INTEGER;
    inactive_rows INTEGER;
    active_duplicate_groups INTEGER;
BEGIN
    SELECT COUNT(*) INTO total_rows
    FROM org."LicenseApplications";

    SELECT COUNT(*) INTO active_rows
    FROM org."LicenseApplications"
    WHERE "Status" = TRUE;

    SELECT COUNT(*) INTO inactive_rows
    FROM org."LicenseApplications"
    WHERE "Status" = FALSE;

    RAISE NOTICE 'LicenseApplications row counts: total=%, active=%, inactive=%',
        total_rows, active_rows, inactive_rows;

    SELECT COUNT(*) INTO active_duplicate_groups
    FROM (
        SELECT "ApplicantElectronicNumber"
        FROM org."LicenseApplications"
        WHERE "Status" = TRUE
          AND "ApplicantElectronicNumber" IS NOT NULL
          AND BTRIM("ApplicantElectronicNumber") <> ''
        GROUP BY "ApplicantElectronicNumber"
        HAVING COUNT(*) > 1
    ) duplicates;

    IF active_duplicate_groups > 0 THEN
        RAISE EXCEPTION
            'Migration aborted: % active duplicate ApplicantElectronicNumber group(s) found. Resolve duplicates among Status=true rows before running this script.',
            active_duplicate_groups;
    END IF;

    RAISE NOTICE 'Pre-flight OK: no duplicate ApplicantElectronicNumber among active records.';
END $$;

-- Optional: show soft-deleted electronic numbers that currently block reuse (informational only)
DO $$
DECLARE
    blocked_numbers INTEGER;
BEGIN
    SELECT COUNT(DISTINCT inactive."ApplicantElectronicNumber") INTO blocked_numbers
    FROM org."LicenseApplications" inactive
    WHERE inactive."Status" = FALSE
      AND inactive."ApplicantElectronicNumber" IS NOT NULL
      AND BTRIM(inactive."ApplicantElectronicNumber") <> ''
      AND NOT EXISTS (
          SELECT 1
          FROM org."LicenseApplications" active
          WHERE active."Status" = TRUE
            AND active."ApplicantElectronicNumber" = inactive."ApplicantElectronicNumber"
      );

    RAISE NOTICE 'Soft-deleted electronic numbers that will become reusable after migration: %',
        blocked_numbers;
END $$;

-- -----------------------------------------------------------------------------
-- Step 2: Replace index (no data changes)
-- -----------------------------------------------------------------------------

DROP INDEX IF EXISTS org."IX_LicenseApplications_ApplicantElectronicNumber";

CREATE UNIQUE INDEX "IX_LicenseApplications_ApplicantElectronicNumber"
    ON org."LicenseApplications" ("ApplicantElectronicNumber")
    WHERE "Status" = TRUE
      AND "ApplicantElectronicNumber" IS NOT NULL
      AND "ApplicantElectronicNumber" <> '';

COMMENT ON INDEX org."IX_LicenseApplications_ApplicantElectronicNumber" IS
    'Ensures ApplicantElectronicNumber is unique among active license applications (Status=true). Soft-deleted rows are excluded.';

-- -----------------------------------------------------------------------------
-- Step 3: Verify index
-- -----------------------------------------------------------------------------

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_indexes
        WHERE schemaname = 'org'
          AND tablename = 'LicenseApplications'
          AND indexname = 'IX_LicenseApplications_ApplicantElectronicNumber'
    ) THEN
        RAISE EXCEPTION 'Verification failed: filtered unique index was not created.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_index i
        JOIN pg_class c ON c.oid = i.indexrelid
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname = 'org'
          AND c.relname = 'IX_LicenseApplications_ApplicantElectronicNumber'
          AND i.indisunique = TRUE
          AND i.indpred IS NOT NULL
    ) THEN
        RAISE EXCEPTION 'Verification failed: index exists but is not a filtered unique index.';
    END IF;

    RAISE NOTICE 'SUCCESS: filtered unique index IX_LicenseApplications_ApplicantElectronicNumber is in place.';
END $$;

-- -----------------------------------------------------------------------------
-- Step 4: Record migration (optional, for deployment tracking)
-- -----------------------------------------------------------------------------

DO $$
BEGIN
    IF to_regclass('public."__EFMigrationsHistory"') IS NOT NULL THEN
        INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        SELECT '20260702_UpdateLicenseApplicationApplicantElectronicNumberUniqueIndex', '7.0.0'
        WHERE NOT EXISTS (
            SELECT 1
            FROM public."__EFMigrationsHistory"
            WHERE "MigrationId" = '20260702_UpdateLicenseApplicationApplicantElectronicNumberUniqueIndex'
        );

        RAISE NOTICE 'Migration recorded in __EFMigrationsHistory.';
    ELSE
        RAISE NOTICE 'Skipped __EFMigrationsHistory insert (table not found). Index change still applied.';
    END IF;
END $$;

COMMIT;

-- -----------------------------------------------------------------------------
-- Post-run verification (read-only; safe to run again)
-- -----------------------------------------------------------------------------

SELECT
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'org'
  AND tablename = 'LicenseApplications'
  AND indexname IN (
      'IX_LicenseApplications_RequestSerialNumber',
      'IX_LicenseApplications_ApplicantElectronicNumber'
  )
ORDER BY indexname;
