-- =============================================================================
-- Migration: Fix ALL LicenseApplications soft-delete unique index issues
-- Date: 2026-07-02
-- Module: ثبت درخواست متقاضیان جواز رهنمای معاملات
--
-- Fixes both DB unique indexes that block re-entry after soft delete:
--   1) IX_LicenseApplications_RequestSerialNumber
--   2) IX_LicenseApplications_ApplicantElectronicNumber
--
-- Other duplicate fields (ProposedGuideName, guarantor/deed numbers, etc.)
-- are validated only in API code and already ignore soft-deleted applications.
-- No additional DB index changes are required for those fields.
--
-- Data safety:
--   - Does NOT delete, update, or truncate any table data.
--   - Only drops and recreates indexes.
--   - Aborts before any change if active records have duplicate values.
--   - Wrapped in a transaction (rolls back on error).
--   - Idempotent: safe even if you already ran the individual scripts.
--
-- How to run (production):
--   psql -h <host> -U <user> -d <database> -f 20260702_UpdateLicenseApplicationSoftDeleteUniqueIndexes.sql
-- =============================================================================

BEGIN;

-- -----------------------------------------------------------------------------
-- Step 1: Pre-flight checks (read-only)
-- -----------------------------------------------------------------------------

DO $$
DECLARE
    duplicate_serial_groups INTEGER;
    duplicate_electronic_groups INTEGER;
BEGIN
    SELECT COUNT(*) INTO duplicate_serial_groups
    FROM (
        SELECT "RequestSerialNumber"
        FROM org."LicenseApplications"
        WHERE "Status" = TRUE
        GROUP BY "RequestSerialNumber"
        HAVING COUNT(*) > 1
    ) d;

    IF duplicate_serial_groups > 0 THEN
        RAISE EXCEPTION
            'Migration aborted: % active duplicate RequestSerialNumber group(s). Fix active data first.',
            duplicate_serial_groups;
    END IF;

    SELECT COUNT(*) INTO duplicate_electronic_groups
    FROM (
        SELECT "ApplicantElectronicNumber"
        FROM org."LicenseApplications"
        WHERE "Status" = TRUE
          AND "ApplicantElectronicNumber" IS NOT NULL
          AND BTRIM("ApplicantElectronicNumber") <> ''
        GROUP BY "ApplicantElectronicNumber"
        HAVING COUNT(*) > 1
    ) d;

    IF duplicate_electronic_groups > 0 THEN
        RAISE EXCEPTION
            'Migration aborted: % active duplicate ApplicantElectronicNumber group(s). Fix active data first.',
            duplicate_electronic_groups;
    END IF;

    RAISE NOTICE 'Pre-flight OK: no duplicate active RequestSerialNumber or ApplicantElectronicNumber.';
END $$;

-- -----------------------------------------------------------------------------
-- Step 2: RequestSerialNumber index
-- -----------------------------------------------------------------------------

DROP INDEX IF EXISTS org."IX_LicenseApplications_RequestSerialNumber";

CREATE UNIQUE INDEX "IX_LicenseApplications_RequestSerialNumber"
    ON org."LicenseApplications" ("RequestSerialNumber")
    WHERE "Status" = TRUE;

COMMENT ON INDEX org."IX_LicenseApplications_RequestSerialNumber" IS
    'Unique among active license applications only (Status=true). Soft-deleted rows excluded.';

-- -----------------------------------------------------------------------------
-- Step 3: ApplicantElectronicNumber index
-- -----------------------------------------------------------------------------

DROP INDEX IF EXISTS org."IX_LicenseApplications_ApplicantElectronicNumber";

CREATE UNIQUE INDEX "IX_LicenseApplications_ApplicantElectronicNumber"
    ON org."LicenseApplications" ("ApplicantElectronicNumber")
    WHERE "Status" = TRUE
      AND "ApplicantElectronicNumber" IS NOT NULL
      AND "ApplicantElectronicNumber" <> '';

COMMENT ON INDEX org."IX_LicenseApplications_ApplicantElectronicNumber" IS
    'Unique among active license applications only (Status=true). Soft-deleted rows excluded.';

-- -----------------------------------------------------------------------------
-- Step 4: Verify indexes
-- -----------------------------------------------------------------------------

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes
        WHERE schemaname = 'org'
          AND tablename = 'LicenseApplications'
          AND indexname = 'IX_LicenseApplications_RequestSerialNumber'
    ) THEN
        RAISE EXCEPTION 'Verification failed: RequestSerialNumber index missing.';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes
        WHERE schemaname = 'org'
          AND tablename = 'LicenseApplications'
          AND indexname = 'IX_LicenseApplications_ApplicantElectronicNumber'
    ) THEN
        RAISE EXCEPTION 'Verification failed: ApplicantElectronicNumber index missing.';
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
          AND pg_get_expr(i.indpred, i.indrelid) ILIKE '%"Status"%true%'
    ) THEN
        RAISE EXCEPTION 'Verification failed: RequestSerialNumber index is not filtered by Status=true.';
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
          AND pg_get_expr(i.indpred, i.indrelid) ILIKE '%"Status"%true%'
    ) THEN
        RAISE EXCEPTION 'Verification failed: ApplicantElectronicNumber index is not filtered by Status=true.';
    END IF;

    RAISE NOTICE 'SUCCESS: both filtered unique indexes are in place.';
END $$;

-- -----------------------------------------------------------------------------
-- Step 5: Record migration (optional)
-- -----------------------------------------------------------------------------

DO $$
BEGIN
    IF to_regclass('public."__EFMigrationsHistory"') IS NOT NULL THEN
        INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        SELECT '20260702_UpdateLicenseApplicationSoftDeleteUniqueIndexes', '7.0.0'
        WHERE NOT EXISTS (
            SELECT 1
            FROM public."__EFMigrationsHistory"
            WHERE "MigrationId" = '20260702_UpdateLicenseApplicationSoftDeleteUniqueIndexes'
        );
    END IF;
END $$;

COMMIT;

-- Post-run verification (read-only)
SELECT indexname, indexdef
FROM pg_indexes
WHERE schemaname = 'org'
  AND tablename = 'LicenseApplications'
  AND indexname IN (
      'IX_LicenseApplications_RequestSerialNumber',
      'IX_LicenseApplications_ApplicantElectronicNumber'
  )
ORDER BY indexname;
