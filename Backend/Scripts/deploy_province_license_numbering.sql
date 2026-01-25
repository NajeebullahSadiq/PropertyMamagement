-- =====================================================
-- Province-Based License Numbering System Deployment
-- =====================================================
-- This script adds province-specific license numbering
-- Format: PROVINCE_CODE-SEQUENTIAL_NUMBER (e.g., KBL-0001)
-- =====================================================

BEGIN;

-- Step 1: Add ProvinceId column to LicenseDetails
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'LicenseDetails' 
        AND column_name = 'ProvinceId'
    ) THEN
        ALTER TABLE org."LicenseDetails" 
        ADD COLUMN "ProvinceId" INTEGER NULL;
        
        RAISE NOTICE 'Added ProvinceId column to LicenseDetails table';
    ELSE
        RAISE NOTICE 'ProvinceId column already exists';
    END IF;
END $$;

-- Step 2: Add foreign key constraint
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_schema = 'org' 
        AND table_name = 'LicenseDetails' 
        AND constraint_name = 'FK_LicenseDetails_Location_ProvinceId'
    ) THEN
        ALTER TABLE org."LicenseDetails"
        ADD CONSTRAINT "FK_LicenseDetails_Location_ProvinceId"
        FOREIGN KEY ("ProvinceId")
        REFERENCES look."Location"("ID")
        ON DELETE RESTRICT;
        
        RAISE NOTICE 'Added foreign key constraint';
    ELSE
        RAISE NOTICE 'Foreign key constraint already exists';
    END IF;
END $$;

-- Step 3: Create indexes for performance
DO $$
BEGIN
    -- Index on ProvinceId
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'org' 
        AND tablename = 'LicenseDetails' 
        AND indexname = 'IX_LicenseDetails_ProvinceId'
    ) THEN
        CREATE INDEX "IX_LicenseDetails_ProvinceId" 
        ON org."LicenseDetails"("ProvinceId");
        
        RAISE NOTICE 'Created index on ProvinceId';
    END IF;

    -- Index on LicenseNumber
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'org' 
        AND tablename = 'LicenseDetails' 
        AND indexname = 'IX_LicenseDetails_LicenseNumber'
    ) THEN
        CREATE INDEX "IX_LicenseDetails_LicenseNumber" 
        ON org."LicenseDetails"("LicenseNumber");
        
        RAISE NOTICE 'Created index on LicenseNumber';
    END IF;

    -- Composite index on ProvinceId and LicenseNumber
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'org' 
        AND tablename = 'LicenseDetails' 
        AND indexname = 'IX_LicenseDetails_ProvinceId_LicenseNumber'
    ) THEN
        CREATE INDEX "IX_LicenseDetails_ProvinceId_LicenseNumber" 
        ON org."LicenseDetails"("ProvinceId", "LicenseNumber");
        
        RAISE NOTICE 'Created composite index on ProvinceId and LicenseNumber';
    END IF;
END $$;

-- Step 4: Optional - Populate ProvinceId for existing licenses
-- This attempts to set ProvinceId based on company owner's permanent province
DO $$
DECLARE
    updated_count INTEGER;
BEGIN
    UPDATE org."LicenseDetails" ld
    SET "ProvinceId" = co."PermanentProvinceId"
    FROM org."CompanyDetails" cd
    JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
    WHERE ld."CompanyId" = cd."Id"
      AND ld."ProvinceId" IS NULL
      AND co."PermanentProvinceId" IS NOT NULL;
    
    GET DIAGNOSTICS updated_count = ROW_COUNT;
    RAISE NOTICE 'Updated % existing licenses with ProvinceId', updated_count;
END $$;

-- Step 5: Verify the changes
DO $$
DECLARE
    total_licenses INTEGER;
    licenses_with_province INTEGER;
    licenses_without_province INTEGER;
BEGIN
    SELECT COUNT(*) INTO total_licenses FROM org."LicenseDetails";
    SELECT COUNT(*) INTO licenses_with_province FROM org."LicenseDetails" WHERE "ProvinceId" IS NOT NULL;
    SELECT COUNT(*) INTO licenses_without_province FROM org."LicenseDetails" WHERE "ProvinceId" IS NULL;
    
    RAISE NOTICE '==============================================';
    RAISE NOTICE 'Deployment Summary:';
    RAISE NOTICE '==============================================';
    RAISE NOTICE 'Total licenses: %', total_licenses;
    RAISE NOTICE 'Licenses with province: %', licenses_with_province;
    RAISE NOTICE 'Licenses without province: %', licenses_without_province;
    RAISE NOTICE '==============================================';
END $$;

-- Step 6: Show sample data
SELECT 
    ld."Id",
    ld."LicenseNumber",
    ld."ProvinceId",
    l."Name" as "ProvinceName",
    l."Dari" as "ProvinceNameDari",
    cd."Title" as "CompanyName"
FROM org."LicenseDetails" ld
LEFT JOIN look."Location" l ON ld."ProvinceId" = l."ID"
LEFT JOIN org."CompanyDetails" cd ON ld."CompanyId" = cd."Id"
ORDER BY ld."Id" DESC
LIMIT 10;

COMMIT;

-- =====================================================
-- Deployment Complete!
-- =====================================================
-- Next Steps:
-- 1. Restart the backend application
-- 2. Test license creation with province selection
-- 3. Verify auto-generated license numbers
-- =====================================================
