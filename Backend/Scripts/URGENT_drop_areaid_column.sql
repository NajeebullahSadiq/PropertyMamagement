-- URGENT: Drop AreaId column from LicenseDetails table
-- Run this immediately to fix the backend errors

-- Drop the AreaId column and its constraints
ALTER TABLE org."LicenseDetails" 
DROP CONSTRAINT IF EXISTS "FK_LicenseDetails_Area";

ALTER TABLE org."LicenseDetails" 
DROP CONSTRAINT IF EXISTS "LicenseDetails_AreaId_fkey";

ALTER TABLE org."LicenseDetails" 
DROP COLUMN IF EXISTS "AreaId";

-- Add TransferLocation column if it doesn't exist
ALTER TABLE org."LicenseDetails" 
ADD COLUMN IF NOT EXISTS "TransferLocation" VARCHAR(500);

-- Verify the change
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'LicenseDetails' 
  AND column_name IN ('AreaId', 'TransferLocation');
