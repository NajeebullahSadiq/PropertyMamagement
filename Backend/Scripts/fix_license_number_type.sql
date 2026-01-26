-- Fix LicenseNumber in LicenseDetails table
-- Convert from double precision to TEXT
-- This is the root cause of the 500 error on /api/CompanyDetails

DO $$
BEGIN
    -- Check if LicenseNumber exists and is double precision
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'LicenseDetails' 
        AND column_name = 'LicenseNumber'
        AND data_type = 'double precision'
    ) THEN
        -- Convert double precision to TEXT
        ALTER TABLE org."LicenseDetails" 
        ALTER COLUMN "LicenseNumber" TYPE TEXT 
        USING CASE 
            WHEN "LicenseNumber" IS NULL THEN NULL
            ELSE "LicenseNumber"::TEXT
        END;
        
        RAISE NOTICE 'Successfully converted LicenseNumber from double precision to TEXT in LicenseDetails';
    ELSE
        RAISE NOTICE 'LicenseNumber in LicenseDetails is already correct type or does not exist';
    END IF;

END $$;

-- Verify the change
SELECT 
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'LicenseDetails'
AND column_name = 'LicenseNumber';

-- Double-check: Show all remaining double precision columns in org schema
SELECT 
    table_name,
    column_name,
    data_type
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND data_type = 'double precision'
ORDER BY table_name, column_name;
