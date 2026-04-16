-- Add foreign key relationship between PropertyDetails and CompanyDetails
-- This script ensures data integrity for the CompanyId field in PropertyDetails

-- Step 1: Clean up invalid CompanyId values (set 0 to NULL)
UPDATE tr."PropertyDetails"
SET "CompanyId" = NULL
WHERE "CompanyId" = 0;

-- Step 2: Add foreign key constraint if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM pg_constraint 
        WHERE conname = 'fk_propertydetails_companydetails_companyid'
    ) THEN
        ALTER TABLE tr."PropertyDetails"
        ADD CONSTRAINT fk_propertydetails_companydetails_companyid
        FOREIGN KEY ("CompanyId") REFERENCES org."CompanyDetails"("Id")
        ON DELETE SET NULL;
        
        RAISE NOTICE 'Foreign key fk_propertydetails_companydetails_companyid added successfully';
    ELSE
        RAISE NOTICE 'Foreign key fk_propertydetails_companydetails_companyid already exists';
    END IF;
END $$;

-- Step 3: Create index on CompanyId for better query performance
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM pg_indexes 
        WHERE indexname = 'ix_propertydetails_companyid'
    ) THEN
        CREATE INDEX ix_propertydetails_companyid 
        ON tr."PropertyDetails"("CompanyId");
        
        RAISE NOTICE 'Index ix_propertydetails_companyid created successfully';
    ELSE
        RAISE NOTICE 'Index ix_propertydetails_companyid already exists';
    END IF;
END $$;
