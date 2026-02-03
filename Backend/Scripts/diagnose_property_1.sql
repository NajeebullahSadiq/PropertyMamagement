-- Detailed diagnostic for Property ID 1 (PRO-2026-D3XU8P)

SELECT '=========================================' as info;
SELECT 'Property ID 1 Diagnostic' as info;
SELECT 'Verification Code: PRO-2026-D3XU8P' as info;
SELECT '=========================================' as info;

-- Step 1: Verification record
SELECT '' as separator;
SELECT 'Step 1: Verification Record' as info;
SELECT * FROM org."DocumentVerifications"
WHERE "VerificationCode" = 'PRO-2026-D3XU8P';

-- Step 2: Property details
SELECT '' as separator;
SELECT 'Step 2: Property Details' as info;
SELECT * FROM tr."PropertyDetails"
WHERE "Id" = 1;

-- Step 3: Seller details
SELECT '' as separator;
SELECT 'Step 3: Seller Details' as info;
SELECT 
    "Id",
    "FirstName",
    "FatherName",
    "GrandFather",
    "ElectronicNationalIdNumber",
    "PhoneNumber",
    "Photo",
    "PaddressProvinceId",
    "PaddressDistrictId",
    "PaddressVillage",
    "PropertyDetailsId"
FROM tr."SellerDetails"
WHERE "PropertyDetailsId" = 1;

-- Step 4: Buyer details
SELECT '' as separator;
SELECT 'Step 4: Buyer Details' as info;
SELECT 
    "Id",
    "FirstName",
    "FatherName",
    "GrandFather",
    "ElectronicNationalIdNumber",
    "PhoneNumber",
    "Photo",
    "PaddressProvinceId",
    "PaddressDistrictId",
    "PaddressVillage",
    "PropertyDetailsId"
FROM tr."BuyerDetails"
WHERE "PropertyDetailsId" = 1;

-- Step 5: Property address
SELECT '' as separator;
SELECT 'Step 5: Property Address' as info;
SELECT * FROM tr."PropertyAddress"
WHERE "PropertyDetailsId" = 1;

-- Step 6: Check location data for seller
SELECT '' as separator;
SELECT 'Step 6: Seller Location Data' as info;
SELECT 
    l."ID",
    l."Name",
    l."Dari",
    l."ParentID",
    'Province or District' as type
FROM look."Location" l
WHERE l."ID" IN (
    SELECT "PaddressProvinceId" FROM tr."SellerDetails" WHERE "PropertyDetailsId" = 1
    UNION
    SELECT "PaddressDistrictId" FROM tr."SellerDetails" WHERE "PropertyDetailsId" = 1
);

-- Step 7: Check location data for buyer
SELECT '' as separator;
SELECT 'Step 7: Buyer Location Data' as info;
SELECT 
    l."ID",
    l."Name",
    l."Dari",
    l."ParentID",
    'Province or District' as type
FROM look."Location" l
WHERE l."ID" IN (
    SELECT "PaddressProvinceId" FROM tr."BuyerDetails" WHERE "PropertyDetailsId" = 1
    UNION
    SELECT "PaddressDistrictId" FROM tr."BuyerDetails" WHERE "PropertyDetailsId" = 1
);

-- Step 8: Summary
SELECT '' as separator;
SELECT '=========================================' as info;
SELECT 'SUMMARY' as info;
SELECT '=========================================' as info;

SELECT 
    CASE 
        WHEN (SELECT COUNT(*) FROM tr."PropertyDetails" WHERE "Id" = 1) = 0 
        THEN '❌ Property does NOT exist'
        ELSE '✓ Property exists'
    END as property_status,
    
    CASE 
        WHEN (SELECT COUNT(*) FROM tr."SellerDetails" WHERE "PropertyDetailsId" = 1) = 0 
        THEN '❌ NO seller data'
        ELSE '✓ Seller data exists (' || (SELECT COUNT(*) FROM tr."SellerDetails" WHERE "PropertyDetailsId" = 1) || ')'
    END as seller_status,
    
    CASE 
        WHEN (SELECT COUNT(*) FROM tr."BuyerDetails" WHERE "PropertyDetailsId" = 1) = 0 
        THEN '❌ NO buyer data'
        ELSE '✓ Buyer data exists (' || (SELECT COUNT(*) FROM tr."BuyerDetails" WHERE "PropertyDetailsId" = 1) || ')'
    END as buyer_status,
    
    CASE 
        WHEN (SELECT "IsRevoked" FROM org."DocumentVerifications" WHERE "VerificationCode" = 'PRO-2026-D3XU8P') = true
        THEN '❌ Verification is REVOKED'
        ELSE '✓ Verification is active'
    END as verification_status;
