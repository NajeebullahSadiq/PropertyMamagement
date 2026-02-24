-- Change AreaId to TransferLocation in LicenseDetails table
-- This will drop the AreaId column and area data

BEGIN;

-- Add new TransferLocation column
ALTER TABLE org."LicenseDetails" 
ADD COLUMN IF NOT EXISTS "TransferLocation" VARCHAR(500);

-- Drop the AreaId column and its foreign key
ALTER TABLE org."LicenseDetails" 
DROP CONSTRAINT IF EXISTS "FK_LicenseDetails_Area";

ALTER TABLE org."LicenseDetails" 
DROP CONSTRAINT IF EXISTS "LicenseDetails_AreaId_fkey";

ALTER TABLE org."LicenseDetails" 
DROP COLUMN IF EXISTS "AreaId";

COMMIT;

-- Verify the change
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'LicenseDetails' 
  AND column_name IN ('AreaId', 'TransferLocation');
