-- Rename IndentityCardNumber to ElectronicNationalIdNumber in Company module tables
-- This fixes the column naming mismatch causing the 500 error

DO $$
BEGIN
    -- Rename in CompanyOwner table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE org."CompanyOwner" 
        RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";
        
        RAISE NOTICE 'Renamed IndentityCardNumber to ElectronicNationalIdNumber in CompanyOwner';
    ELSE
        RAISE NOTICE 'IndentityCardNumber column does not exist in CompanyOwner (may already be renamed)';
    END IF;

    -- Rename in Guarantor table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE org."Guarantor" 
        RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";
        
        RAISE NOTICE 'Renamed IndentityCardNumber to ElectronicNationalIdNumber in Guarantor';
    ELSE
        RAISE NOTICE 'IndentityCardNumber column does not exist in Guarantor (may already be renamed)';
    END IF;

    -- Drop old paper ID columns from CompanyOwner if they exist
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'IdentityCardTypeId'
    ) THEN
        ALTER TABLE org."CompanyOwner" DROP COLUMN "IdentityCardTypeId";
        RAISE NOTICE 'Dropped IdentityCardTypeId from CompanyOwner';
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'Jild'
    ) THEN
        ALTER TABLE org."CompanyOwner" DROP COLUMN "Jild";
        RAISE NOTICE 'Dropped Jild from CompanyOwner';
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'Safha'
    ) THEN
        ALTER TABLE org."CompanyOwner" DROP COLUMN "Safha";
        RAISE NOTICE 'Dropped Safha from CompanyOwner';
    END IF;

    -- Drop old paper ID columns from Guarantor if they exist
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'IdentityCardTypeId'
    ) THEN
        ALTER TABLE org."Guarantor" DROP COLUMN "IdentityCardTypeId";
        RAISE NOTICE 'Dropped IdentityCardTypeId from Guarantor';
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'Jild'
    ) THEN
        ALTER TABLE org."Guarantor" DROP COLUMN "Jild";
        RAISE NOTICE 'Dropped Jild from Guarantor';
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'Safha'
    ) THEN
        ALTER TABLE org."Guarantor" DROP COLUMN "Safha";
        RAISE NOTICE 'Dropped Safha from Guarantor';
    END IF;

END $$;

-- Verify the changes
SELECT 
    table_name,
    column_name,
    data_type
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name IN ('CompanyOwner', 'Guarantor')
AND column_name IN ('ElectronicNationalIdNumber', 'IndentityCardNumber', 'IdentityCardTypeId', 'Jild', 'Safha')
ORDER BY table_name, column_name;
