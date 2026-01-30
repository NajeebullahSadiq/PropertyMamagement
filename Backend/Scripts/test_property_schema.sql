-- Quick test to verify Property module schema and tables

-- Check if tr schema exists
SELECT schema_name 
FROM information_schema.schemata 
WHERE schema_name = 'tr';

-- List all tables in tr schema
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'tr'
ORDER BY table_name;

-- Check SellerDetails table structure
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'tr' 
  AND table_name = 'SellerDetails'
ORDER BY ordinal_position;

-- Check BuyerDetails table structure
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'tr' 
  AND table_name = 'BuyerDetails'
ORDER BY ordinal_position;

-- Count records with photos
SELECT 
    'SellerDetails' as table_name,
    COUNT(*) as total_records,
    COUNT("Photo") as records_with_photo
FROM tr."SellerDetails"
UNION ALL
SELECT 
    'BuyerDetails' as table_name,
    COUNT(*) as total_records,
    COUNT("Photo") as records_with_photo
FROM tr."BuyerDetails";
