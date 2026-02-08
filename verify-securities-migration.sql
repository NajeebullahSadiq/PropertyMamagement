-- Securities Migration Verification Script
-- Run this on production after migration to verify data integrity

\echo '=========================================='
\echo 'Securities Migration Verification'
\echo '=========================================='
\echo ''

-- 1. Check total distributions
\echo '1. Total Securities Distributions:'
SELECT COUNT(*) as total_distributions FROM org."SecuritiesDistribution";
\echo 'Expected: ~6,989'
\echo ''

-- 2. Check total distribution items
\echo '2. Total Distribution Items:'
SELECT COUNT(*) as total_items FROM org."SecuritiesDistributionItem";
\echo 'Expected: ~12,416'
\echo ''

-- 3. Check distribution by document type
\echo '3. Distribution by Document Type:'
SELECT 
    sdt."TypeName",
    COUNT(DISTINCT sdi."DistributionId") as distributions,
    COUNT(*) as items,
    SUM(sdi."Quantity") as total_quantity
FROM org."SecuritiesDistributionItem" sdi
JOIN org."SecuritiesDocumentType" sdt ON sdi."DocumentTypeId" = sdt."Id"
GROUP BY sdt."Id", sdt."TypeName"
ORDER BY sdt."Id";
\echo ''

-- 4. Check for any null or invalid data
\echo '4. Data Quality Checks:'
\echo '   a. Distributions with missing registration numbers:'
SELECT COUNT(*) as missing_reg_numbers 
FROM org."SecuritiesDistribution" 
WHERE "RegistrationNumber" IS NULL OR "RegistrationNumber" = '';
\echo '   Expected: 0'
\echo ''

\echo '   b. Distributions with missing license numbers:'
SELECT COUNT(*) as missing_license_numbers 
FROM org."SecuritiesDistribution" 
WHERE "LicenseNumber" IS NULL OR "LicenseNumber" = '';
\echo '   Expected: 0'
\echo ''

\echo '   c. Distribution items with zero quantity:'
SELECT COUNT(*) as zero_quantity_items 
FROM org."SecuritiesDistributionItem" 
WHERE "Quantity" <= 0;
\echo '   Expected: 0'
\echo ''

-- 5. Sample data from distributions
\echo '5. Sample Securities Distributions (First 10):'
SELECT 
    "Id",
    "RegistrationNumber",
    "LicenseOwnerName",
    "LicenseNumber",
    "DistributionDate"
FROM org."SecuritiesDistribution" 
ORDER BY "Id"
LIMIT 10;
\echo ''

-- 6. Sample distribution items
\echo '6. Sample Distribution Items (First 10):'
SELECT 
    sdi."Id",
    sdi."DistributionId",
    sdt."TypeName",
    sdi."StartSerial",
    sdi."EndSerial",
    sdi."Quantity"
FROM org."SecuritiesDistributionItem" sdi
JOIN org."SecuritiesDocumentType" sdt ON sdi."DocumentTypeId" = sdt."Id"
ORDER BY sdi."Id"
LIMIT 10;
\echo ''

-- 7. Check date ranges
\echo '7. Distribution Date Range:'
SELECT 
    MIN("DistributionDate") as earliest_date,
    MAX("DistributionDate") as latest_date,
    COUNT(*) as total_with_dates
FROM org."SecuritiesDistribution" 
WHERE "DistributionDate" IS NOT NULL;
\echo ''

-- 8. Check for duplicate registration numbers
\echo '8. Duplicate Registration Numbers:'
SELECT 
    "RegistrationNumber",
    COUNT(*) as count
FROM org."SecuritiesDistribution"
GROUP BY "RegistrationNumber"
HAVING COUNT(*) > 1
ORDER BY count DESC
LIMIT 10;
\echo 'Note: "فقط کتاب ثبت" duplicates are expected'
\echo ''

-- 9. Price statistics
\echo '9. Price Statistics:'
SELECT 
    COUNT(*) as distributions_with_prices,
    MIN("TotalSecuritiesPrice") as min_price,
    MAX("TotalSecuritiesPrice") as max_price,
    AVG("TotalSecuritiesPrice") as avg_price,
    SUM("TotalSecuritiesPrice") as total_price
FROM org."SecuritiesDistribution" 
WHERE "TotalSecuritiesPrice" IS NOT NULL;
\echo ''

-- 10. Items per distribution statistics
\echo '10. Items per Distribution:'
SELECT 
    COUNT(DISTINCT "DistributionId") as total_distributions,
    MIN(item_count) as min_items,
    MAX(item_count) as max_items,
    AVG(item_count)::numeric(10,2) as avg_items
FROM (
    SELECT "DistributionId", COUNT(*) as item_count
    FROM org."SecuritiesDistributionItem"
    GROUP BY "DistributionId"
) subquery;
\echo ''

\echo '=========================================='
\echo 'Verification Complete!'
\echo '=========================================='
\echo ''
\echo 'If all checks pass:'
\echo '  ✓ Total distributions: ~6,989'
\echo '  ✓ Total items: ~12,416'
\echo '  ✓ No null/invalid data'
\echo '  ✓ Sample data looks correct'
\echo ''
\echo 'Migration is successful!'
\echo ''
