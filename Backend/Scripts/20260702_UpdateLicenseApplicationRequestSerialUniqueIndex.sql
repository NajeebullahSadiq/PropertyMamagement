-- =============================================================================
-- Migration: Update LicenseApplications RequestSerialNumber unique index
-- Date: 2026-07-02
-- Module: ثبت درخواست متقاضیان جواز رهنمای معاملات
--
-- Problem:
--   Delete is soft (Status = false), but the old unique index applied to ALL rows.
--   Re-entering the same request serial number after delete fails with:
--   23505 duplicate key value violates unique constraint
--   "IX_LicenseApplications_RequestSerialNumber"
--
-- Fix:
--   Replace the unfiltered unique index with a filtered one:
--   unique only when Status = true (active records).
--
-- Data safety:
--   - Does NOT delete, update, or truncate any table data.
--   - Only drops and recreates an index.
--   - Aborts before any change if active records already have duplicate serials.
--   - Wrapped in a transaction (rolls back on error).
--
-- How to run (production):
--   psql -h <host> -U <user> -d <database> -f 20260702_UpdateLicenseApplicationRequestSerialUniqueIndex.sql
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

    -- Only active records must be unique under the new index.
    SELECT COUNT(*) INTO active_duplicate_groups
    FROM (
        SELECT "RequestSerialNumber"
        FROM org."LicenseApplications"
        WHERE "Status" = TRUE
        GROUP BY "RequestSerialNumber"
        HAVING COUNT(*) > 1
    ) duplicates;

    IF active_duplicate_groups > 0 THEN
        RAISE EXCEPTION
            'Migration aborted: % active duplicate RequestSerialNumber group(s) found. Resolve duplicates among Status=true rows before running this script.',
            active_duplicate_groups;
    END IF;

    RAISE NOTICE 'Pre-flight OK: no duplicate RequestSerialNumber among active records.';
END $$;

-- Optional: show soft-deleted serials that currently block reuse (informational only)
DO $$
DECLARE
    blocked_serials INTEGER;
BEGIN
    SELECT COUNT(DISTINCT inactive."RequestSerialNumber") INTO blocked_serials
    FROM org."LicenseApplications" inactive
    WHERE inactive."Status" = FALSE
      AND NOT EXISTS (
          SELECT 1
          FROM org."LicenseApplications" active
          WHERE active."Status" = TRUE
            AND active."RequestSerialNumber" = inactive."RequestSerialNumber"
      );

    RAISE NOTICE 'Soft-deleted serial numbers that will become reusable after migration: %',
        blocked_serials;
END $$;

-- -----------------------------------------------------------------------------
-- Step 2: Replace index (no data changes)
-- -----------------------------------------------------------------------------

DROP INDEX IF EXISTS org."IX_LicenseApplications_RequestSerialNumber";

CREATE UNIQUE INDEX "IX_LicenseApplications_RequestSerialNumber"
    ON org."LicenseApplications" ("RequestSerialNumber")
    WHERE "Status" = TRUE;

COMMENT ON INDEX org."IX_LicenseApplications_RequestSerialNumber" IS
    'Ensures RequestSerialNumber is unique among active license applications (Status=true). Soft-deleted rows are excluded.';

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
          AND indexname = 'IX_LicenseApplications_RequestSerialNumber'
    ) THEN
        RAISE EXCEPTION 'Verification failed: filtered unique index was not created.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_index i
        JOIN pg_class c ON c.oid = i.indexrelid
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname = 'org'
          AND c.relname = 'IX_LicenseApplications_RequestSerialNumber'
          AND i.indisunique = TRUE
          AND i.indpred IS NOT NULL
    ) THEN
        RAISE EXCEPTION 'Verification failed: index exists but is not a filtered unique index.';
    END IF;

    RAISE NOTICE 'SUCCESS: filtered unique index IX_LicenseApplications_RequestSerialNumber is in place.';
END $$;

-- -----------------------------------------------------------------------------
-- Step 4: Record migration (optional, for deployment tracking)
-- -----------------------------------------------------------------------------

DO $$
BEGIN
    IF to_regclass('public."__EFMigrationsHistory"') IS NOT NULL THEN
        INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        SELECT '20260702_UpdateLicenseApplicationRequestSerialUniqueIndex', '7.0.0'
        WHERE NOT EXISTS (
            SELECT 1
            FROM public."__EFMigrationsHistory"
            WHERE "MigrationId" = '20260702_UpdateLicenseApplicationRequestSerialUniqueIndex'
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
  AND indexname = 'IX_LicenseApplications_RequestSerialNumber';
