-- Corrected verification diagnostic for PRO-2026-D3XU8P
-- Uses correct schema names: tr for transactions, org for organization

SELECT '=========================================' as info;
SELECT 'Verification Code Diagnostic' as info;
SELECT 'Code: PRO-2026-D3XU8P' as info;
SELECT '=========================================' as info;

-- Step 1: Check if verification code exists
SELECT '' as separator;
SELECT 'Step 1: Checking verification code...' as info;

SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ Verification code does NOT exist'
        ELSE '✓ Verification code EXISTS'
    END as status,
    COUNT(*) as count
FROM org."DocumentVerifications"
WHERE "VerificationCode" = 'PRO-2026-D3XU8P';

-- Show verification details
SELECT 
    "Id" as verification_id,
    "VerificationCode" as code,
    "DocumentId" as property_id,
    "DocumentType" as doc_type,
    "IsRevoked" as is_revoked,
    "RevokedReason" as revoked_reason,
    "CreatedAt" as created_at,
    "CreatedBy" as created_by
FROM org."DocumentVerifications"
WHERE "VerificationCode" = 'PRO-2026-D3XU8P';

-- Step 2: Check property details (tr schema, not org!)
SELECT '' as separator;
SELECT 'Step 2: Checking property details...' as info;

SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ Property does NOT exist'
        ELSE '✓ Property EXISTS'
    END as status,
    COUNT(*) as count
FROM tr."PropertyDetails" pd
WHERE pd."Id" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Show property details
SELECT 
    pd."Id" as property_id,
    pd."PNumber" as property_number,
    pd."Parea" as area,
    pd."Price" as price,
    pd."iscomplete" as is_complete,
    pd."iseditable" as is_editable,
    pd."CreatedAt" as created_at,
    pd."CreatedBy" as created_by,
    pd."CompanyId" as company_id
FROM tr."PropertyDetails" pd
WHERE pd."Id" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Step 3: Check seller details (tr schema)
SELECT '' as separator;
SELECT 'Step 3: Checking seller details...' as info;

SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ NO seller data found'
        ELSE '✓ Seller data EXISTS (' || COUNT(*) || ' seller(s))'
    END as status,
    COUNT(*) as seller_count
FROM tr."SellerDetails" sd
WHERE sd."PropertyDetailsId" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Show seller details
SELECT 
    sd."Id" as seller_id,
    sd."FirstName" as first_name,
    sd."FatherName" as father_name,
    sd."GrandFather" as grandfather_name,
    sd."ElectronicNationalIdNumber" as national_id,
    sd."PhoneNumber" as phone,
    sd."Photo" as photo_path,
    sd."PaddressProvinceId" as province_id,
    sd."PaddressDistrictId" as district_id,
    sd."PaddressVillage" as village
FROM tr."SellerDetails" sd
WHERE sd."PropertyDetailsId" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Step 4: Check buyer details (tr schema)
SELECT '' as separator;
SELECT 'Step 4: Checking buyer details...' as info;

SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ NO buyer data found'
        ELSE '✓ Buyer data EXISTS (' || COUNT(*) || ' buyer(s))'
    END as status,
    COUNT(*) as buyer_count
FROM tr."BuyerDetails" bd
WHERE bd."PropertyDetailsId" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Show buyer details
SELECT 
    bd."Id" as buyer_id,
    bd."FirstName" as first_name,
    bd."FatherName" as father_name,
    bd."GrandFather" as grandfather_name,
    bd."ElectronicNationalIdNumber" as national_id,
    bd."PhoneNumber" as phone,
    bd."Photo" as photo_path,
    bd."PaddressProvinceId" as province_id,
    bd."PaddressDistrictId" as district_id,
    bd."PaddressVillage" as village,
    bd."Price" as price,
    bd."PriceText" as price_text
FROM tr."BuyerDetails" bd
WHERE bd."PropertyDetailsId" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Step 5: Summary and recommendations
SELECT '' as separator;
SELECT '=========================================' as info;
SELECT 'SUMMARY' as info;
SELECT '=========================================' as info;

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
        SELECT 1 FROM org."DocumentVerifications" 
        WHERE "VerificationCode" = 'PRO-2026-D3XU8P'
    ) INTO v_verification_exists;
    
    IF NOT v_verification_exists THEN
        RAISE NOTICE '';
        RAISE NOTICE '❌ PROBLEM: Verification code does NOT exist in database';
        RAISE NOTICE '';
        RAISE NOTICE 'SOLUTION:';
        RAISE NOTICE '1. Find the property in your UI';
        RAISE NOTICE '2. Click "Print" button';
        RAISE NOTICE '3. System will automatically generate verification code';
        RAISE NOTICE '';
        v_problem_count := v_problem_count + 1;
    ELSE
        SELECT "IsRevoked", "DocumentId" INTO v_is_revoked, v_property_id
        FROM org."DocumentVerifications"
        WHERE "VerificationCode" = 'PRO-2026-D3XU8P';
        
        RAISE NOTICE '';
        RAISE NOTICE '✓ Verification code exists for Property ID: %', v_property_id;
        
        IF v_is_revoked THEN
            RAISE NOTICE '❌ PROBLEM: Verification is REVOKED';
            RAISE NOTICE '   Solution: Generate new verification code';
            v_problem_count := v_problem_count + 1;
        ELSE
            -- Check property
            SELECT EXISTS(
                SELECT 1 FROM tr."PropertyDetails" WHERE "Id" = v_property_id
            ) INTO v_property_exists;
            
            IF NOT v_property_exists THEN
                RAISE NOTICE '❌ PROBLEM: Property does NOT exist (deleted?)';
                v_problem_count := v_problem_count + 1;
            ELSE
                RAISE NOTICE '✓ Property exists';
                
                -- Check seller
                SELECT EXISTS(
                    SELECT 1 FROM tr."SellerDetails" WHERE "PropertyDetailsId" = v_property_id
                ) INTO v_seller_exists;
                
                IF NOT v_seller_exists THEN
                    RAISE NOTICE '❌ PROBLEM: NO seller data';
                    RAISE NOTICE '   Solution: Add seller details to property';
                    v_problem_count := v_problem_count + 1;
                ELSE
                    RAISE NOTICE '✓ Seller data exists';
                END IF;
                
                -- Check buyer
                SELECT EXISTS(
                    SELECT 1 FROM tr."BuyerDetails" WHERE "PropertyDetailsId" = v_property_id
                ) INTO v_buyer_exists;
                
                IF NOT v_buyer_exists THEN
                    RAISE NOTICE '❌ PROBLEM: NO buyer data';
                    RAISE NOTICE '   Solution: Add buyer details to property';
                    v_problem_count := v_problem_count + 1;
                ELSE
                    RAISE NOTICE '✓ Buyer data exists';
                END IF;
            END IF;
        END IF;
    END IF;
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    IF v_problem_count = 0 THEN
        RAISE NOTICE '✓ ALL CHECKS PASSED';
        RAISE NOTICE '========================================';
        RAISE NOTICE '';
        RAISE NOTICE 'If verification still returns invalid:';
        RAISE NOTICE '1. Property data may have changed after verification';
        RAISE NOTICE '2. Regenerate verification from print page';
        RAISE NOTICE '3. Check backend logs for errors';
    ELSE
        RAISE NOTICE '❌ FOUND % PROBLEM(S)', v_problem_count;
        RAISE NOTICE '========================================';
        RAISE NOTICE '';
        RAISE NOTICE 'Fix the problems above, then:';
        RAISE NOTICE '1. Open property in UI';
        RAISE NOTICE '2. Click "Print"';
        RAISE NOTICE '3. New verification code will be generated';
    END IF;
    RAISE NOTICE '';
END $$;

SELECT 'Diagnostic complete.' as status;
