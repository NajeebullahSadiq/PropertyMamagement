-- Add unique constraint to LicenseDetails.LicenseNumber to prevent duplicates
-- Run this AFTER fixing any existing duplicates

-- Step 1: Check for existing duplicates first
DO $$
DECLARE
    duplicate_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO duplicate_count
    FROM (
        SELECT "LicenseNumber"
        FROM org."LicenseDetails"
        WHERE "LicenseNumber" IS NOT NULL
        GROUP BY "LicenseNumber"
        HAVING COUNT(*) > 1
    ) duplicates;
    
    IF duplicate_count > 0 THEN
        RAISE EXCEPTION 'Found % duplicate license numbers. Please run fix_duplicate_license_numbers.sql first', duplicate_count;
    END IF;
END $$;

-- Step 2: Drop existing non-unique index if it exists
DROP INDEX IF EXISTS org."IX_LicenseDetails_LicenseNumber";

-- Step 3: Create unique index
CREATE UNIQUE INDEX "IX_LicenseDetails_LicenseNumber" 
ON org."LicenseDetails" ("LicenseNumber")
WHERE "LicenseNumber" IS NOT NULL;

COMMENT ON INDEX org."IX_LicenseDetails_LicenseNumber" IS 'Ensures license numbers are unique to prevent race condition duplicates';

-- Step 4: Verify the unique index was created
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'org' 
        AND tablename = 'LicenseDetails' 
        AND indexname = 'IX_LicenseDetails_LicenseNumber'
    ) THEN
        RAISE NOTICE 'SUCCESS: Unique index IX_LicenseDetails_LicenseNumber created successfully';
    ELSE
        RAISE EXCEPTION 'FAILED: Unique index was not created';
    END IF;
END $$;
