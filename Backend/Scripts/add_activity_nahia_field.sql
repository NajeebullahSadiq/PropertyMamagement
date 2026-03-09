-- Add ActivityNahia field to PetitionWriterLicenses table
-- This field stores the district/nahia information for the activity location

-- Add the column
ALTER TABLE org."PetitionWriterLicenses"
ADD COLUMN IF NOT EXISTS "ActivityNahia" VARCHAR(200);

-- Add comment
COMMENT ON COLUMN org."PetitionWriterLicenses"."ActivityNahia" IS 'ناحیه محل فعالیت - District/Nahia of activity location';

-- Verify the column was added
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org'
  AND table_name = 'PetitionWriterLicenses'
  AND column_name = 'ActivityNahia';

-- Show sample data
SELECT 
    "Id",
    "LicenseNumber",
    "ApplicantName",
    "DetailedAddress",
    "ActivityNahia"
FROM org."PetitionWriterLicenses"
WHERE "Status" = true
LIMIT 10;
