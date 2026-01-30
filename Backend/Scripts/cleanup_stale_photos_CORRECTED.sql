-- Clean up stale photo references in Property module
-- CORRECTED VERSION with actual column names from database

-- First, check what we're dealing with
SELECT 
    'SellerDetails' as table_name,
    COUNT(*) as total_records,
    COUNT("Photo") as records_with_photo,
    SUM(CASE WHEN "Photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_2025_references,
    SUM(CASE WHEN "Photo" LIKE '%2026%' THEN 1 ELSE 0 END) as current_2026_references
FROM tr."SellerDetails";

SELECT 
    'BuyerDetails' as table_name,
    COUNT(*) as total_records,
    COUNT("photo") as records_with_photo,
    SUM(CASE WHEN "photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_2025_references,
    SUM(CASE WHEN "photo" LIKE '%2026%' THEN 1 ELSE 0 END) as current_2026_references
FROM tr."BuyerDetails";

-- Show the stale references (December 2025)
SELECT 
    'Seller' as type,
    "Id",
    "FirstName",
    "FatherName",
    "Photo",
    "PropertyDetailsId"
FROM tr."SellerDetails"
WHERE "Photo" LIKE '%2025%'
ORDER BY "Id";

SELECT 
    'Buyer' as type,
    "Id",
    "FirstName",
    "FatherName",
    "photo",
    "PropertyDetailsId"
FROM tr."BuyerDetails"
WHERE "photo" LIKE '%2025%'
ORDER BY "Id";

-- Show current references (January 2026) - these should work
SELECT 
    'Seller (2026)' as type,
    "Id",
    "FirstName",
    "Photo"
FROM tr."SellerDetails"
WHERE "Photo" LIKE '%2026%'
ORDER BY "Id";

SELECT 
    'Buyer (2026)' as type,
    "Id",
    "FirstName",
    "photo"
FROM tr."BuyerDetails"
WHERE "photo" LIKE '%2026%'
ORDER BY "Id";

-- UNCOMMENT THE FOLLOWING LINES TO ACTUALLY CLEAN UP THE DATA
-- WARNING: This will set Photo/photo to NULL for all records with 2025 dates

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
--     'After Cleanup - Seller' as status,
--     COUNT(*) as total_records,
--     COUNT("Photo") as records_with_photo,
--     SUM(CASE WHEN "Photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_references
-- FROM tr."SellerDetails";

-- SELECT 
--     'After Cleanup - Buyer' as status,
--     COUNT(*) as total_records,
--     COUNT("photo") as records_with_photo,
--     SUM(CASE WHEN "photo" LIKE '%2025%' THEN 1 ELSE 0 END) as stale_references
-- FROM tr."BuyerDetails";
