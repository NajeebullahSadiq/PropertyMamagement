-- Comprehensive diagnostic script for verification code: PRO-2026-D3XU8P
-- Run this to understand why verification is failing

-- =========================================
-- VERIFICATION CODE DIAGNOSTIC
-- Code: PRO-2026-D3XU8P
-- =========================================

-- Step 1: Check if verification code exists
SELECT '=========================================' as info;
SELECT 'Step 1: Checking if verification code exists...' as info;
SELECT '=========================================' as info;

SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ PROBLEM: Verification code does not exist in database'
        WHEN COUNT(*) > 0 THEN '✓ Verification code found'
    END as status
FROM org.DocumentVerifications
WHERE VerificationCode = 'PRO-2026-D3XU8P';

-- Show verification details if exists
SELECT 
    Id as verification_id,
    VerificationCode as code,
    DocumentId as property_id,
    DocumentType as doc_type,
    IsRevoked as is_revoked,
    RevokedReason as revoked_reason,
    CreatedAt as created_at,
    CreatedBy as created_by,
    LENGTH(DigitalSignature) as signature_length
FROM org.DocumentVerifications
WHERE VerificationCode = 'PRO-2026-D3XU8P';

SELECT '' as info;
SELECT '=========================================' as info;
SELECT 'Step 2: Checking property details...' as info;
SELECT '=========================================' as info;

-- Check if property exists
SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ PROBLEM: Property does not exist'
        WHEN COUNT(*) > 0 THEN '✓ Property found'
    END as status
FROM org.PropertyDetails pd
WHERE pd.Id = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

-- Show property details
SELECT 
    pd.Id as property_id,
    pd.PNumber as property_number,
    pd.Parea as area,
    pd.Price as price,
    pd.iscomplete as is_complete,
    pd.iseditable as is_editable,
    pd.CreatedAt as created_at,
    pd.CreatedBy as created_by,
    pd.CompanyId as company_id,
    pd.IssuanceNumber as issuance_number,
    pd.IssuanceDate as issuance_date,
    pt.Name as property_type,
    pd.CustomPropertyType as custom_property_type,
    tt.Name as transaction_type
FROM org.PropertyDetails pd
LEFT JOIN org.PropertyTypes pt ON pt.Id = pd.PropertyTypeId
LEFT JOIN org.TransactionTypes tt ON tt.Id = pd.TransactionTypeId
WHERE pd.Id = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

SELECT '' as info;
SELECT '=========================================' as info;
SELECT 'Step 3: Checking seller details...' as info;
SELECT '=========================================' as info;

-- Check seller count
SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ PROBLEM: No seller data found'
        WHEN COUNT(*) > 0 THEN '✓ Seller data found (' || COUNT(*) || ' seller(s))'
    END as status
FROM org.SellerDetails sd
WHERE sd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

-- Show seller details
SELECT 
    sd.Id as seller_id,
    sd.FirstName as first_name,
    sd.FatherName as father_name,
    sd.GrandFather as grandfather_name,
    sd.ElectronicNationalIdNumber as national_id,
    sd.PhoneNumber as phone,
    sd.Photo as photo_path,
    sd.PaddressProvinceId as province_id,
    lp.Name as province_name,
    lp.Dari as province_dari,
    sd.PaddressDistrictId as district_id,
    ld.Name as district_name,
    ld.Dari as district_dari,
    sd.PaddressVillage as village,
    CASE 
        WHEN sd.PaddressProvinceId IS NOT NULL AND lp.Id IS NULL THEN '❌ Invalid Province ID'
        WHEN sd.PaddressDistrictId IS NOT NULL AND ld.Id IS NULL THEN '❌ Invalid District ID'
        ELSE '✓ Location data valid'
    END as location_status
FROM org.SellerDetails sd
LEFT JOIN org.Locations lp ON lp.Id = sd.PaddressProvinceId
LEFT JOIN org.Locations ld ON ld.Id = sd.PaddressDistrictId
WHERE sd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

SELECT '' as info;
SELECT '=========================================' as info;
SELECT 'Step 4: Checking buyer details...' as info;
SELECT '=========================================' as info;

-- Check buyer count
SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ PROBLEM: No buyer data found'
        WHEN COUNT(*) > 0 THEN '✓ Buyer data found (' || COUNT(*) || ' buyer(s))'
    END as status
FROM org.BuyerDetails bd
WHERE bd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

-- Show buyer details
SELECT 
    bd.Id as buyer_id,
    bd.FirstName as first_name,
    bd.FatherName as father_name,
    bd.GrandFather as grandfather_name,
    bd.ElectronicNationalIdNumber as national_id,
    bd.PhoneNumber as phone,
    bd.Photo as photo_path,
    bd.PaddressProvinceId as province_id,
    lp.Name as province_name,
    lp.Dari as province_dari,
    bd.PaddressDistrictId as district_id,
    ld.Name as district_name,
    ld.Dari as district_dari,
    bd.PaddressVillage as village,
    bd.Price as price,
    bd.PriceText as price_text,
    bd.RoyaltyAmount as royalty_amount,
    CASE 
        WHEN bd.PaddressProvinceId IS NOT NULL AND lp.Id IS NULL THEN '❌ Invalid Province ID'
        WHEN bd.PaddressDistrictId IS NOT NULL AND ld.Id IS NULL THEN '❌ Invalid District ID'
        ELSE '✓ Location data valid'
    END as location_status
FROM org.BuyerDetails bd
LEFT JOIN org.Locations lp ON lp.Id = bd.PaddressProvinceId
LEFT JOIN org.Locations ld ON ld.Id = bd.PaddressDistrictId
WHERE bd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

SELECT '' as info;
SELECT '=========================================' as info;
SELECT 'Step 5: Checking property address...' as info;
SELECT '=========================================' as info;

-- Check property address
SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '⚠ WARNING: No property address found'
        WHEN COUNT(*) > 0 THEN '✓ Property address found'
    END as status
FROM org.PropertyAddresses pa
WHERE pa.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

-- Show property address
SELECT 
    pa.Id as address_id,
    pa.ProvinceId as province_id,
    lp.Name as province_name,
    lp.Dari as province_dari,
    pa.DistrictId as district_id,
    ld.Name as district_name,
    ld.Dari as district_dari,
    pa.Village as village,
    CASE 
        WHEN pa.ProvinceId IS NOT NULL AND lp.Id IS NULL THEN '❌ Invalid Province ID'
        WHEN pa.DistrictId IS NOT NULL AND ld.Id IS NULL THEN '❌ Invalid District ID'
        ELSE '✓ Location data valid'
    END as location_status
FROM org.PropertyAddresses pa
LEFT JOIN org.Locations lp ON lp.Id = pa.ProvinceId
LEFT JOIN org.Locations ld ON ld.Id = pa.DistrictId
WHERE pa.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

SELECT '' as info;
SELECT '=========================================' as info;
SELECT 'Step 6: Checking witness details...' as info;
SELECT '=========================================' as info;

-- Check witness count
SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '⚠ INFO: No witness data found (optional)'
        WHEN COUNT(*) > 0 THEN '✓ Witness data found (' || COUNT(*) || ' witness(es))'
    END as status
FROM org.WitnessDetails wd
WHERE wd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

-- Show witness details
SELECT 
    wd.Id as witness_id,
    wd.FirstName as first_name,
    wd.FatherName as father_name,
    wd.PhoneNumber as phone,
    wd.ElectronicNationalIdNumber as national_id,
    wd.NationalIdCard as national_id_card_path
FROM org.WitnessDetails wd
WHERE wd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
    LIMIT 1
);

SELECT '' as info;
SELECT '=========================================' as info;
SELECT 'SUMMARY AND RECOMMENDATIONS' as info;
SELECT '=========================================' as info;

-- Generate summary
DO $$
DECLARE
    v_verification_exists BOOLEAN;
    v_property_exists BOOLEAN;
    v_seller_exists BOOLEAN;
    v_buyer_exists BOOLEAN;
    v_is_revoked BOOLEAN;
    v_property_id INT;
    v_problem_count INT := 0;
BEGIN
    -- Check verification
    SELECT EXISTS(
        SELECT 1 FROM org.DocumentVerifications 
        WHERE VerificationCode = 'PRO-2026-D3XU8P'
    ) INTO v_verification_exists;
    
    IF NOT v_verification_exists THEN
        RAISE NOTICE '❌ CRITICAL: Verification code does not exist in database';
        RAISE NOTICE '   → Solution: Regenerate verification code from print page';
        RAISE NOTICE '   → Or use API: POST /api/verification/generate';
        v_problem_count := v_problem_count + 1;
    ELSE
        RAISE NOTICE '✓ Verification code exists';
        
        -- Check if revoked
        SELECT IsRevoked, DocumentId INTO v_is_revoked, v_property_id
        FROM org.DocumentVerifications 
        WHERE VerificationCode = 'PRO-2026-D3XU8P';
        
        IF v_is_revoked THEN
            RAISE NOTICE '❌ PROBLEM: Verification code is revoked';
            RAISE NOTICE '   → Solution: Generate new verification code';
            v_problem_count := v_problem_count + 1;
        ELSE
            -- Check property
            SELECT EXISTS(
                SELECT 1 FROM org.PropertyDetails WHERE Id = v_property_id
            ) INTO v_property_exists;
            
            IF NOT v_property_exists THEN
                RAISE NOTICE '❌ CRITICAL: Property does not exist (ID: %)', v_property_id;
                RAISE NOTICE '   → Solution: Property was deleted, verification is invalid';
                v_problem_count := v_problem_count + 1;
            ELSE
                RAISE NOTICE '✓ Property exists (ID: %)', v_property_id;
                
                -- Check seller
                SELECT EXISTS(
                    SELECT 1 FROM org.SellerDetails WHERE PropertyDetailsId = v_property_id
                ) INTO v_seller_exists;
                
                IF NOT v_seller_exists THEN
                    RAISE NOTICE '❌ PROBLEM: No seller data found';
                    RAISE NOTICE '   → Solution: Add seller details to property';
                    v_problem_count := v_problem_count + 1;
                ELSE
                    RAISE NOTICE '✓ Seller data exists';
                END IF;
                
                -- Check buyer
                SELECT EXISTS(
                    SELECT 1 FROM org.BuyerDetails WHERE PropertyDetailsId = v_property_id
                ) INTO v_buyer_exists;
                
                IF NOT v_buyer_exists THEN
                    RAISE NOTICE '❌ PROBLEM: No buyer data found';
                    RAISE NOTICE '   → Solution: Add buyer details to property';
                    v_problem_count := v_problem_count + 1;
                ELSE
                    RAISE NOTICE '✓ Buyer data exists';
                END IF;
            END IF;
        END IF;
    END IF;
    
    RAISE NOTICE '';
    IF v_problem_count = 0 THEN
        RAISE NOTICE '========================================';
        RAISE NOTICE '✓ ALL CHECKS PASSED';
        RAISE NOTICE '========================================';
        RAISE NOTICE 'If verification still fails, the issue might be:';
        RAISE NOTICE '1. Signature mismatch (property data changed after verification)';
        RAISE NOTICE '2. Backend service error';
        RAISE NOTICE '3. Database connection issue';
        RAISE NOTICE '';
        RAISE NOTICE 'Try regenerating the verification code.';
    ELSE
        RAISE NOTICE '========================================';
        RAISE NOTICE '❌ FOUND % PROBLEM(S)', v_problem_count;
        RAISE NOTICE '========================================';
        RAISE NOTICE 'Fix the problems listed above and try again.';
    END IF;
END $$;

SELECT '' as info;
SELECT 'Diagnostic complete.' as info;
