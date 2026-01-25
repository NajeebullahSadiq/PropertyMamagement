-- =============================================
-- Add Province-Based License Numbering to Petition Writer Licenses
-- Format: PROVINCE_CODE-SEQUENTIAL_NUMBER (e.g., KBL-0001, KHR-0002)
-- Date: 2026-01-25
-- PostgreSQL Version
-- =============================================

DO $$
BEGIN
    RAISE NOTICE 'Starting Province-Based License Numbering Migration for Petition Writer Licenses...';
    
    -- Step 1: Add ProvinceId column if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'PetitionWriterLicenses' 
        AND column_name = 'ProvinceId'
    ) THEN
        RAISE NOTICE 'Adding ProvinceId column to PetitionWriterLicenses table...';
        
        ALTER TABLE org."PetitionWriterLicenses"
        ADD COLUMN "ProvinceId" INTEGER NULL;
        
        RAISE NOTICE 'ProvinceId column added successfully.';
    ELSE
        RAISE NOTICE 'ProvinceId column already exists.';
    END IF;
    
    -- Step 2: Add foreign key constraint
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_schema = 'org'
        AND constraint_name = 'FK_PetitionWriterLicenses_Province'
        AND table_name = 'PetitionWriterLicenses'
    ) THEN
        RAISE NOTICE 'Adding foreign key constraint...';
        
        ALTER TABLE org."PetitionWriterLicenses"
        ADD CONSTRAINT "FK_PetitionWriterLicenses_Province"
        FOREIGN KEY ("ProvinceId") REFERENCES shared."Locations"("Id");
        
        RAISE NOTICE 'Foreign key constraint added successfully.';
    ELSE
        RAISE NOTICE 'Foreign key constraint already exists.';
    END IF;
    
    -- Step 3: Add index on ProvinceId for better query performance
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'org'
        AND tablename = 'PetitionWriterLicenses'
        AND indexname = 'IX_PetitionWriterLicenses_ProvinceId'
    ) THEN
        RAISE NOTICE 'Creating index on ProvinceId...';
        
        CREATE INDEX "IX_PetitionWriterLicenses_ProvinceId"
        ON org."PetitionWriterLicenses"("ProvinceId");
        
        RAISE NOTICE 'Index created successfully.';
    ELSE
        RAISE NOTICE 'Index on ProvinceId already exists.';
    END IF;
    
    -- Step 4: Add index on LicenseNumber for faster lookups
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'org'
        AND tablename = 'PetitionWriterLicenses'
        AND indexname = 'IX_PetitionWriterLicenses_LicenseNumber'
    ) THEN
        RAISE NOTICE 'Creating index on LicenseNumber...';
        
        CREATE INDEX "IX_PetitionWriterLicenses_LicenseNumber"
        ON org."PetitionWriterLicenses"("LicenseNumber");
        
        RAISE NOTICE 'Index created successfully.';
    ELSE
        RAISE NOTICE 'Index on LicenseNumber already exists.';
    END IF;
    
    RAISE NOTICE '';
    RAISE NOTICE 'Province Codes for License Numbering:';
    RAISE NOTICE '======================================';
    RAISE NOTICE 'Kabul (کابل) = KBL';
    RAISE NOTICE 'Herat (هرات) = HRT';
    RAISE NOTICE 'Kandahar (کندهار) = KHR';
    RAISE NOTICE 'Balkh (بلخ) = BLK';
    RAISE NOTICE 'Nangarhar (ننگرهار) = NGR';
    RAISE NOTICE 'Ghazni (غزنی) = GHZ';
    RAISE NOTICE 'Helmand (هلمند) = HLM';
    RAISE NOTICE 'And 27 more provinces...';
    RAISE NOTICE '';
    
    RAISE NOTICE 'Migration completed successfully!';
    RAISE NOTICE '';
    RAISE NOTICE 'IMPORTANT NOTES:';
    RAISE NOTICE '1. Existing licenses will have NULL ProvinceId - update them manually or through the application';
    RAISE NOTICE '2. New licenses will require ProvinceId to generate proper license numbers';
    RAISE NOTICE '3. License number format: PROVINCE_CODE-SEQUENTIAL_NUMBER (e.g., KBL-0001)';
    RAISE NOTICE '4. Each province has its own sequential numbering starting from 0001';
    RAISE NOTICE '';
END $$;

-- Verify the changes
SELECT 
    'PetitionWriterLicenses' AS "TableName",
    COUNT(*) AS "TotalRecords",
    SUM(CASE WHEN "ProvinceId" IS NOT NULL THEN 1 ELSE 0 END) AS "RecordsWithProvince",
    SUM(CASE WHEN "ProvinceId" IS NULL THEN 1 ELSE 0 END) AS "RecordsWithoutProvince"
FROM org."PetitionWriterLicenses"
WHERE "Status" = 1;
