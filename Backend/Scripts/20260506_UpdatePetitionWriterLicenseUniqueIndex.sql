-- Migration: Update PetitionWriterLicenses LicenseNumber unique index to filtered (only active records)
-- Date: 2026-05-06
-- This fixes the issue where soft-deleted records with the same LicenseNumber
-- cause unique constraint violations during updates.
-- Also fixes FK constraints to use ON DELETE SET NULL for nullable FKs.

BEGIN;

-- 1. Drop the old unfiltered unique index on LicenseNumber
DROP INDEX IF EXISTS org."IX_PetitionWriterLicenses_LicenseNumber";

-- Create new filtered unique index (only where Status = true)
-- This allows soft-deleted records to have duplicate LicenseNumbers
CREATE UNIQUE INDEX "IX_PetitionWriterLicenses_LicenseNumber"
    ON org."PetitionWriterLicenses" ("LicenseNumber")
    WHERE "Status" = true;

-- 2. Fix ProvinceId FK: drop old constraint and recreate with ON DELETE SET NULL
-- The old FK name from FluentMigrator was "FK_PetitionWriterLicenses_Province"
-- The EF Core migration used "FK_PetitionWriterLicenses_Location_ProvinceId"
-- Drop whichever exists
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "FK_PetitionWriterLicenses_Province";
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "FK_PetitionWriterLicenses_Location_ProvinceId";
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "PetitionWriterLicenses_ProvinceId_fkey";

ALTER TABLE org."PetitionWriterLicenses"
    ADD CONSTRAINT "PetitionWriterLicenses_ProvinceId_fkey"
    FOREIGN KEY ("ProvinceId") REFERENCES look."Location"("ID")
    ON DELETE SET NULL;

-- 3. Fix PermanentProvinceId FK
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "PetitionWriterLicenses_PermanentProvinceId_fkey";

ALTER TABLE org."PetitionWriterLicenses"
    ADD CONSTRAINT "PetitionWriterLicenses_PermanentProvinceId_fkey"
    FOREIGN KEY ("PermanentProvinceId") REFERENCES look."Location"("ID")
    ON DELETE SET NULL;

-- 4. Fix PermanentDistrictId FK
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "PetitionWriterLicenses_PermanentDistrictId_fkey";

ALTER TABLE org."PetitionWriterLicenses"
    ADD CONSTRAINT "PetitionWriterLicenses_PermanentDistrictId_fkey"
    FOREIGN KEY ("PermanentDistrictId") REFERENCES look."Location"("ID")
    ON DELETE SET NULL;

-- 5. Fix CurrentProvinceId FK
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "PetitionWriterLicenses_CurrentProvinceId_fkey";

ALTER TABLE org."PetitionWriterLicenses"
    ADD CONSTRAINT "PetitionWriterLicenses_CurrentProvinceId_fkey"
    FOREIGN KEY ("CurrentProvinceId") REFERENCES look."Location"("ID")
    ON DELETE SET NULL;

-- 6. Fix CurrentDistrictId FK
ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "PetitionWriterLicenses_CurrentDistrictId_fkey";

ALTER TABLE org."PetitionWriterLicenses"
    ADD CONSTRAINT "PetitionWriterLicenses_CurrentDistrictId_fkey"
    FOREIGN KEY ("CurrentDistrictId") REFERENCES look."Location"("ID")
    ON DELETE SET NULL;

COMMIT;
