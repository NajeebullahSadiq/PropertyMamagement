-- Add ActivityLocation field to LicenseDetails table
-- محل فعالیت (Activity Location)

ALTER TABLE org."LicenseDetails"
ADD COLUMN IF NOT EXISTS "ActivityLocation" TEXT;

COMMENT ON COLUMN org."LicenseDetails"."ActivityLocation" IS 'محل فعالیت - Activity Location';
