-- FINAL FIX: Clean up duplicate identity columns in Company module
-- This handles the case where both old and new columns exist

DO $$
BEGIN
    -- ============================================
    -- CompanyOwner Table
    -- ============================================
    
    -- Check if old IndentityCardNumber column exists and has double precision type
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'IndentityCardNumber'
        AND data_type = 'double precision'
    ) THEN
        -- Convert to text first, then drop
        ALTER TABLE org."CompanyOwner" 
        ALTER COLUMN "IndentityCardNumber" TYPE TEXT 
        USING "IndentityCardNumber"::TEXT;
        
        RAISE NOTICE 'Converted IndentityCardNumber to TEXT in CompanyOwner';
    END IF;
    
    -- Drop the old IndentityCardNumber column if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyOwner' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE org."CompanyOwner" DROP COLUMN "IndentityCardNumber";
        RAISE NOTICE 'Dropped old IndentityCardNumber column from CompanyOwner';
    END IF;

    -- Drop old paper ID columns from CompanyOwner
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

    -- ============================================
    -- Guarantor Table
    -- ============================================
    
    -- Check if old IndentityCardNumber column exists and has double precision type
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'IndentityCardNumber'
        AND data_type = 'double precision'
    ) THEN
        -- Convert to text first, then drop
        ALTER TABLE org."Guarantor" 
        ALTER COLUMN "IndentityCardNumber" TYPE TEXT 
        USING "IndentityCardNumber"::TEXT;
        
        RAISE NOTICE 'Converted IndentityCardNumber to TEXT in Guarantor';
    END IF;
    
    -- Drop the old IndentityCardNumber column if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantor' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE org."Guarantor" DROP COLUMN "IndentityCardNumber";
        RAISE NOTICE 'Dropped old IndentityCardNumber column from Guarantor';
    END IF;

    -- Drop old paper ID columns from Guarantor
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

-- Verify the final state
SELECT 
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name IN ('CompanyOwner', 'Guarantor')
AND (column_name LIKE '%Identity%' OR column_name LIKE '%Jild%' OR column_name LIKE '%Safha%' OR column_name = 'ElectronicNationalIdNumber')
ORDER BY table_name, column_name;
