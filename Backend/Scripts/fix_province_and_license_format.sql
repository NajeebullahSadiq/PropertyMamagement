-- Fix Province IDs and License Number Format
-- Sets all ProvinceId to 1 and formats license numbers as KBL-00000{number}

DO $$
DECLARE
    updated_companies INTEGER;
    updated_licenses INTEGER;
BEGIN
    RAISE NOTICE 'Starting fix for Province IDs and License Numbers...';
    RAISE NOTICE ' ';
    
    -- Update CompanyDetails ProvinceId to 1
    UPDATE org."CompanyDetails"
    SET "ProvinceId" = 1
    WHERE "ProvinceId" IS NULL OR "ProvinceId" != 1;
    
    GET DIAGNOSTICS updated_companies = ROW_COUNT;
    RAISE NOTICE '✓ Updated % companies to ProvinceId = 1', updated_companies;
    
    -- Update LicenseDetails ProvinceId to 1 and format LicenseNumber
    UPDATE org."LicenseDetails"
    SET 
        "ProvinceId" = 1,
        "LicenseNumber" = 'KBL-' || LPAD("LicenseNumber", 5, '0')
    WHERE "ProvinceId" IS NULL OR "ProvinceId" != 1 
       OR "LicenseNumber" NOT LIKE 'KBL-%';
    
    GET DIAGNOSTICS updated_licenses = ROW_COUNT;
    RAISE NOTICE '✓ Updated % licenses to ProvinceId = 1 and formatted license numbers', updated_licenses;
    
    RAISE NOTICE ' ';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Fix Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE ' ';
END $$;

-- Verify the changes
SELECT 
    'CompanyDetails with ProvinceId = 1' as description,
    COUNT(*) as count
FROM org."CompanyDetails"
WHERE "ProvinceId" = 1
UNION ALL
SELECT 
    'LicenseDetails with ProvinceId = 1',
    COUNT(*)
FROM org."LicenseDetails"
WHERE "ProvinceId" = 1
UNION ALL
SELECT 
    'Licenses with KBL- prefix',
    COUNT(*)
FROM org."LicenseDetails"
WHERE "LicenseNumber" LIKE 'KBL-%';

-- Show sample formatted license numbers
SELECT 
    "Id",
    "LicenseNumber",
    "ProvinceId",
    "IssueDate"
FROM org."LicenseDetails"
ORDER BY "Id"
LIMIT 10;
