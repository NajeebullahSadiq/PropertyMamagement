-- Add LicenseCost field to PetitionWriterLicenses table
-- This field stores the cost based on license type: جدید = 168, تجدید/مثنی = 85

-- Add the column
ALTER TABLE org."PetitionWriterLicenses"
ADD COLUMN IF NOT EXISTS "LicenseCost" DECIMAL(18,2);

-- Add comment
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseCost" IS 'قیمت جواز - License cost based on type';

-- Verify the column was added
SELECT 
    column_name,
    data_type,
    numeric_precision,
    numeric_scale,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org'
  AND table_name = 'PetitionWriterLicenses'
  AND column_name = 'LicenseCost';
