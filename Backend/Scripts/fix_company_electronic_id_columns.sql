-- Fix ElectronicNationalIdNumber columns in Company module tables
-- This script converts the type from double precision to VARCHAR(50)

DO $$
BEGIN
    -- Fix CompanyOwner table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'ElectronicNationalIdNumber'
        AND data_type = 'double precision'
    ) THEN
        ALTER TABLE org."CompanyOwner" 
        ALTER COLUMN "ElectronicNationalIdNumber" TYPE VARCHAR(50) 
        USING CASE 
            WHEN "ElectronicNationalIdNumber" IS NULL THEN NULL
            ELSE "ElectronicNationalIdNumber"::VARCHAR(50)
        END;
        
        RAISE NOTICE 'Converted ElectronicNationalIdNumber to VARCHAR(50) in CompanyOwner';
    ELSE
        RAISE NOTICE 'ElectronicNationalIdNumber in CompanyOwner is already correct type or does not exist';
    END IF;

    -- Fix Guarantor table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'ElectronicNationalIdNumber'
        AND data_type = 'double precision'
    ) THEN
        ALTER TABLE org."Guarantor" 
        ALTER COLUMN "ElectronicNationalIdNumber" TYPE VARCHAR(50) 
        USING CASE 
            WHEN "ElectronicNationalIdNumber" IS NULL THEN NULL
            ELSE "ElectronicNationalIdNumber"::VARCHAR(50)
        END;
        
        RAISE NOTICE 'Converted ElectronicNationalIdNumber to VARCHAR(50) in Guarantor';
    ELSE
        RAISE NOTICE 'ElectronicNationalIdNumber in Guarantor is already correct type or does not exist';
    END IF;

END $$;

-- Verify the changes
SELECT 
    table_schema,
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name IN ('CompanyOwner', 'Guarantor')
AND column_name = 'ElectronicNationalIdNumber';
