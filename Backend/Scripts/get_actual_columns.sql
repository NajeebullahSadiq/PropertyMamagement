-- Get actual column names from SellerDetails and BuyerDetails tables

-- SellerDetails columns
SELECT 
    'SellerDetails' as table_name,
    column_name, 
    data_type,
    ordinal_position
FROM information_schema.columns
WHERE table_schema = 'tr' 
  AND table_name = 'SellerDetails'
ORDER BY ordinal_position;

-- BuyerDetails columns
SELECT 
    'BuyerDetails' as table_name,
    column_name, 
    data_type,
    ordinal_position
FROM information_schema.columns
WHERE table_schema = 'tr' 
  AND table_name = 'BuyerDetails'
ORDER BY ordinal_position;
