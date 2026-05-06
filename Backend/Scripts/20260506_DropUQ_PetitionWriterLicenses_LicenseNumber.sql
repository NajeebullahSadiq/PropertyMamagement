-- Migration: Drop additional unique constraint on LicenseNumber
-- Date: 2026-05-06
-- The filtered unique index IX_PetitionWriterLicenses_LicenseNumber (created earlier)
-- already handles uniqueness for active records. This old unfiltered constraint is redundant.

BEGIN;

ALTER TABLE org."PetitionWriterLicenses" DROP CONSTRAINT IF EXISTS "UQ_PetitionWriterLicenses_LicenseNumber";

COMMIT;
