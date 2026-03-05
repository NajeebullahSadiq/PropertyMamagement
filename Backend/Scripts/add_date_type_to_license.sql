-- Add DateType column to LicenseDetails table
-- This column stores the calendar type used for date entry (gregorian, hijriShamsi, or hijriQamari)

ALTER TABLE org."LicenseDetails"
ADD COLUMN IF NOT EXISTS "DateType" VARCHAR(20) DEFAULT 'hijriShamsi';

-- Update existing records to have the default value
UPDATE org."LicenseDetails"
SET "DateType" = 'hijriShamsi'
WHERE "DateType" IS NULL;

-- Verify the column was added
SELECT column_name, data_type, column_default
FROM information_schema.columns
WHERE table_schema = 'org' 
  AND table_name = 'LicenseDetails' 
  AND column_name = 'DateType';
