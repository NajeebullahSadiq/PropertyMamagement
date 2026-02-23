-- Add new fields to LicenseApplications table
-- Split شهرت متقاضی into Name, Father Name, Grandfather Name and add Electronic Number

-- Add ApplicantFatherName column
ALTER TABLE org."LicenseApplications"
ADD COLUMN IF NOT EXISTS "ApplicantFatherName" VARCHAR(200);

-- Add ApplicantGrandfatherName column
ALTER TABLE org."LicenseApplications"
ADD COLUMN IF NOT EXISTS "ApplicantGrandfatherName" VARCHAR(200);

-- Add ApplicantElectronicNumber column
ALTER TABLE org."LicenseApplications"
ADD COLUMN IF NOT EXISTS "ApplicantElectronicNumber" VARCHAR(50);

-- Add comments for clarity
COMMENT ON COLUMN org."LicenseApplications"."ApplicantName" IS 'نام متقاضی';
COMMENT ON COLUMN org."LicenseApplications"."ApplicantFatherName" IS 'نام پدر متقاضی';
COMMENT ON COLUMN org."LicenseApplications"."ApplicantGrandfatherName" IS 'نام پدرکلان متقاضی';
COMMENT ON COLUMN org."LicenseApplications"."ApplicantElectronicNumber" IS 'نمبر الکترونیکی متقاضی';

-- Verify the changes
SELECT column_name, data_type, character_maximum_length, is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
  AND table_name = 'LicenseApplications'
  AND column_name IN ('ApplicantName', 'ApplicantFatherName', 'ApplicantGrandfatherName', 'ApplicantElectronicNumber')
ORDER BY ordinal_position;
