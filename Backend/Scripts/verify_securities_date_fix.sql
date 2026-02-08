-- =====================================================
-- Verify Securities Date Column Fix
-- =====================================================

-- 1. Check column types (should be 'date' not 'timestamp')
SELECT 
    column_name, 
    data_type, 
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
    AND table_name = 'SecuritiesDistribution'
    AND column_name IN ('DeliveryDate', 'DistributionDate');

-- Expected Result:
-- column_name       | data_type | is_nullable
-- ------------------+-----------+------------
-- DeliveryDate      | date      | YES
-- DistributionDate  | date      | YES

-- 2. Check existing data (should show dates without timestamps)
SELECT 
    "Id",
    "RegistrationNumber",
    "DeliveryDate",
    "DistributionDate",
    "CreatedAt"
FROM org."SecuritiesDistribution"
ORDER BY "Id" DESC
LIMIT 10;

-- Expected Result:
-- Dates should be in format: 2026-02-03 (no time component)

-- 3. Check if any records exist
SELECT COUNT(*) as total_records
FROM org."SecuritiesDistribution";
