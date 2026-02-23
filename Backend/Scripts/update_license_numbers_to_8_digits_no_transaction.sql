-- =====================================================
-- Update License Numbers to 8-Digit Format (No Transaction)
-- =====================================================
-- This script updates all existing license numbers to use 8-digit format
-- Example: KBL-7418 becomes KBL-00007418
-- This version runs without a transaction for easier testing
-- =====================================================

-- Update Company License Numbers (LicenseDetails table)
UPDATE org."LicenseDetails"
SET "LicenseNumber" = 
    CASE 
        WHEN "LicenseNumber" IS NOT NULL AND "LicenseNumber" LIKE '%-%' THEN
            SPLIT_PART("LicenseNumber", '-', 1) || '-' || 
            LPAD(SPLIT_PART("LicenseNumber", '-', 2), 8, '0')
        ELSE 
            "LicenseNumber"
    END
WHERE "LicenseNumber" IS NOT NULL 
  AND "LicenseNumber" LIKE '%-%'
  AND LENGTH(SPLIT_PART("LicenseNumber", '-', 2)) < 8;

-- Update Petition Writer License Numbers
UPDATE org."PetitionWriterLicenses"
SET "LicenseNumber" = 
    CASE 
        WHEN "LicenseNumber" IS NOT NULL AND "LicenseNumber" LIKE '%-%' THEN
            SPLIT_PART("LicenseNumber", '-', 1) || '-' || 
            LPAD(SPLIT_PART("LicenseNumber", '-', 2), 8, '0')
        ELSE 
            "LicenseNumber"
    END
WHERE "LicenseNumber" IS NOT NULL 
  AND "LicenseNumber" LIKE '%-%'
  AND LENGTH(SPLIT_PART("LicenseNumber", '-', 2)) < 8;

-- Verify the updates
SELECT 'Company Licenses Updated:' as info, COUNT(*) as count
FROM org."LicenseDetails"
WHERE "LicenseNumber" IS NOT NULL 
  AND "LicenseNumber" LIKE '%-%'
  AND LENGTH(SPLIT_PART("LicenseNumber", '-', 2)) = 8;

SELECT 'Petition Writer Licenses Updated:' as info, COUNT(*) as count
FROM org."PetitionWriterLicenses"
WHERE "LicenseNumber" IS NOT NULL 
  AND "LicenseNumber" LIKE '%-%'
  AND LENGTH(SPLIT_PART("LicenseNumber", '-', 2)) = 8;

-- Show sample of updated license numbers
SELECT 'Sample Company License Numbers:' as info;
SELECT "Id", "LicenseNumber", "CompanyId"
FROM org."LicenseDetails"
WHERE "LicenseNumber" IS NOT NULL
ORDER BY "Id" DESC
LIMIT 10;

SELECT 'Sample Petition Writer License Numbers:' as info;
SELECT "Id", "LicenseNumber"
FROM org."PetitionWriterLicenses"
WHERE "LicenseNumber" IS NOT NULL
ORDER BY "Id" DESC
LIMIT 10;

-- =====================================================
-- INSTRUCTIONS:
-- =====================================================
-- 1. Backup your database before running this script
-- 2. Run this script in your PostgreSQL database
-- 3. Verify the results using the SELECT statements
-- 4. All license numbers will be updated to 8-digit format
-- =====================================================
