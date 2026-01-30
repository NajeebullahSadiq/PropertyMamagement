-- Clean up stale photo references in Property module
-- This script removes references to photos that no longer exist on disk

-- First, let's see what we're dealing with
SELECT 
    'Seller' as record_type,
    COUNT(*) as total_records,
    COUNT("Photo") as records_with_photo,
    SUM(CASE WHEN "Photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_references
FROM tr."SellerDetails";

SELECT 
    'Buyer' as record_type,
    COUNT(*) as total_records,
    COUNT("photo") as records_with_photo,
    SUM(CASE WHEN "photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_references
FROM tr."BuyerDetails";

-- Show the stale references
SELECT 
    'Seller' as type,
    "Id",
    "Name",
    "FatherName",
    "Photo",
    "PropertyDetailsId"
FROM tr."SellerDetails"
WHERE "Photo" LIKE '%2025%'
ORDER BY "Id";

SELECT 
    'Buyer' as type,
    "Id",
    "Name",
    "FatherName",
    "photo",
    "PropertyDetailsId"
FROM tr."BuyerDetails"
WHERE "photo" LIKE '%2025%'
ORDER BY "Id";

-- UNCOMMENT THE FOLLOWING LINES TO ACTUALLY CLEAN UP THE DATA
-- WARNING: This will set Photo to NULL for all records with 2025 dates

-- Clean up sellers
-- UPDATE tr."SellerDetails"
-- SET "Photo" = NULL
-- WHERE "Photo" LIKE '%2025%';

-- Clean up buyers
-- UPDATE tr."BuyerDetails"
-- SET "photo" = NULL
-- WHERE "photo" LIKE '%2025%';

-- Verify cleanup (run after uncommenting above)
-- SELECT 
--     'After Cleanup - Seller' as record_type,
--     COUNT(*) as total_records,
--     COUNT("Photo") as records_with_photo,
--     SUM(CASE WHEN "Photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_references
-- FROM tr."SellerDetails";

-- SELECT 
--     'After Cleanup - Buyer' as record_type,
--     COUNT(*) as total_records,
--     COUNT("photo") as records_with_photo,
--     SUM(CASE WHEN "photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_references
-- FROM tr."BuyerDetails";
