-- Check for paper-based Tazkira fields in Property module tables
-- These fields should be removed, keeping only ElectronicNationalIdNumber

-- Check SellerDetails table
SELECT 
    'tr.SellerDetails' as table_name,
    column_name, 
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_schema = 'tr' 
    AND table_name = 'SellerDetails'
    AND (column_name ILIKE '%tazkira%' OR column_name = 'ElectronicNationalIdNumber')
ORDER BY column_name;

-- Check BuyerDetails table
SELECT 
    'tr.BuyerDetails' as table_name,
    column_name, 
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_schema = 'tr' 
    AND table_name = 'BuyerDetails'
    AND (column_name ILIKE '%tazkira%' OR column_name = 'ElectronicNationalIdNumber')
ORDER BY column_name;

-- Check WitnessDetails table
SELECT 
    'tr.WitnessDetails' as table_name,
    column_name, 
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_schema = 'tr' 
    AND table_name = 'WitnessDetails'
    AND (column_name ILIKE '%tazkira%' OR column_name = 'ElectronicNationalIdNumber')
ORDER BY column_name;
