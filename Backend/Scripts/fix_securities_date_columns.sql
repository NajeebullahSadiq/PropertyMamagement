-- =====================================================
-- Fix Securities Date Columns
-- Convert timestamp columns to DATE type
-- =====================================================

-- Check current column types
SELECT 
    column_name, 
    data_type, 
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
    AND table_name = 'SecuritiesDistribution'
    AND column_name IN ('DeliveryDate', 'DistributionDate');

-- Backup existing data (optional but recommended)
-- CREATE TABLE org."SecuritiesDistribution_backup" AS 
-- SELECT * FROM org."SecuritiesDistribution";

-- Convert DeliveryDate from timestamp to date
-- First, alter the column to allow the conversion
ALTER TABLE org."SecuritiesDistribution" 
    ALTER COLUMN "DeliveryDate" TYPE DATE USING "DeliveryDate"::DATE;

-- Convert DistributionDate from timestamp to date
ALTER TABLE org."SecuritiesDistribution" 
    ALTER COLUMN "DistributionDate" TYPE DATE USING "DistributionDate"::DATE;

-- Verify the changes
SELECT 
    column_name, 
    data_type, 
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
    AND table_name = 'SecuritiesDistribution'
    AND column_name IN ('DeliveryDate', 'DistributionDate');

-- Check the data after conversion
SELECT 
    "Id",
    "RegistrationNumber",
    "DeliveryDate",
    "DistributionDate"
FROM org."SecuritiesDistribution"
ORDER BY "Id" DESC
LIMIT 10;

COMMIT;
