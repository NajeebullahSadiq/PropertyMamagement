-- Simple check for photos in Property module tables
-- This uses SELECT * to avoid column name issues

-- Check SellerDetails
SELECT 'SellerDetails' as table_name, COUNT(*) as total_records
FROM tr."SellerDetails";

-- Check BuyerDetails  
SELECT 'BuyerDetails' as table_name, COUNT(*) as total_records
FROM tr."BuyerDetails";

-- Show first few records from SellerDetails to see actual column names
SELECT * FROM tr."SellerDetails" LIMIT 3;

-- Show first few records from BuyerDetails to see actual column names
SELECT * FROM tr."BuyerDetails" LIMIT 3;
