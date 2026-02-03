-- =============================================
-- Setup Verification System and Check Code
-- =============================================

-- Step 1: Create verification tables if they don't exist
SELECT 'Setting up verification tables...' as status;

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- Create DocumentVerifications table
CREATE TABLE IF NOT EXISTS org."DocumentVerifications" (
    "Id" SERIAL PRIMARY KEY,
    "VerificationCode" VARCHAR(20) NOT NULL,
    "DocumentId" INTEGER NOT NULL,
    "DocumentType" VARCHAR(50) NOT NULL,
    "DigitalSignature" VARCHAR(128) NOT NULL,
    "DocumentSnapshot" JSONB,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "IsRevoked" BOOLEAN DEFAULT FALSE,
    "RevokedAt" TIMESTAMP WITH TIME ZONE,
    "RevokedBy" VARCHAR(50),
    "RevokedReason" VARCHAR(500)
);

-- Create indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_DocumentVerifications_VerificationCode" 
ON org."DocumentVerifications"("VerificationCode");

CREATE INDEX IF NOT EXISTS "IX_DocumentVerifications_DocumentId_DocumentType" 
ON org."DocumentVerifications"("DocumentId", "DocumentType");

CREATE INDEX IF NOT EXISTS "IX_DocumentVerifications_IsRevoked" 
ON org."DocumentVerifications"("IsRevoked") WHERE "IsRevoked" = FALSE;

-- Create VerificationLogs table
CREATE TABLE IF NOT EXISTS org."VerificationLogs" (
    "Id" SERIAL PRIMARY KEY,
    "VerificationCode" VARCHAR(20) NOT NULL,
    "AttemptedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "IpAddress" VARCHAR(45),
    "WasSuccessful" BOOLEAN NOT NULL,
    "FailureReason" VARCHAR(50)
);

-- Create indexes for logs
CREATE INDEX IF NOT EXISTS "IX_VerificationLogs_VerificationCode_AttemptedAt" 
ON org."VerificationLogs"("VerificationCode", "AttemptedAt" DESC);

CREATE INDEX IF NOT EXISTS "IX_VerificationLogs_WasSuccessful" 
ON org."VerificationLogs"("WasSuccessful") WHERE "WasSuccessful" = FALSE;

SELECT 'Verification tables created successfully!' as status;

-- Step 2: Check if verification code exists
SELECT '' as separator;
SELECT '=========================================' as info;
SELECT 'Checking verification code: PRO-2026-D3XU8P' as info;
SELECT '=========================================' as info;

SELECT 
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ Verification code does NOT exist in database'
        ELSE '✓ Verification code EXISTS'
    END as result
FROM org."DocumentVerifications"
WHERE "VerificationCode" = 'PRO-2026-D3XU8P';

-- Show verification details if exists
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

-- Step 3: If verification exists, check the property
SELECT '' as separator;
SELECT 'Checking property data...' as info;

SELECT 
    pd."Id" as property_id,
    pd."PNumber" as property_number,
    pd."Parea" as area,
    pd."Price" as price,
    pd."iscomplete" as is_complete,
    pd."CreatedAt" as created_at
FROM org."PropertyDetails" pd
WHERE pd."Id" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Step 4: Check seller data
SELECT '' as separator;
SELECT 'Checking seller data...' as info;

SELECT 
    COUNT(*) as seller_count,
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ No seller data'
        ELSE '✓ Seller data exists'
    END as status
FROM org."SellerDetails" sd
WHERE sd."PropertyDetailsId" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Step 5: Check buyer data
SELECT '' as separator;
SELECT 'Checking buyer data...' as info;

SELECT 
    COUNT(*) as buyer_count,
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ No buyer data'
        ELSE '✓ Buyer data exists'
    END as status
FROM org."BuyerDetails" bd
WHERE bd."PropertyDetailsId" = (
    SELECT "DocumentId" 
    FROM org."DocumentVerifications" 
    WHERE "VerificationCode" = 'PRO-2026-D3XU8P' 
    AND "DocumentType" = 'PropertyDocument'
    LIMIT 1
);

-- Step 6: Summary
SELECT '' as separator;
SELECT '=========================================' as info;
SELECT 'SUMMARY' as info;
SELECT '=========================================' as info;

DO $$
DECLARE
    v_verification_exists BOOLEAN;
    v_property_id INT;
BEGIN
    -- Check if verification exists
    SELECT EXISTS(
        SELECT 1 FROM org."DocumentVerifications" 
        WHERE "VerificationCode" = 'PRO-2026-D3XU8P'
    ) INTO v_verification_exists;
    
    IF NOT v_verification_exists THEN
        RAISE NOTICE '';
        RAISE NOTICE '❌ PROBLEM: Verification code PRO-2026-D3XU8P does NOT exist';
        RAISE NOTICE '';
        RAISE NOTICE 'SOLUTION:';
        RAISE NOTICE '1. Open the property in the UI';
        RAISE NOTICE '2. Click "Print" button';
        RAISE NOTICE '3. The system will automatically generate a verification code';
        RAISE NOTICE '';
        RAISE NOTICE 'OR use the API:';
        RAISE NOTICE 'POST /api/verification/generate';
        RAISE NOTICE '{"documentId": PROPERTY_ID, "documentType": "PropertyDocument"}';
    ELSE
        SELECT "DocumentId" INTO v_property_id
        FROM org."DocumentVerifications"
        WHERE "VerificationCode" = 'PRO-2026-D3XU8P';
        
        RAISE NOTICE '';
        RAISE NOTICE '✓ Verification code exists for Property ID: %', v_property_id;
        RAISE NOTICE '';
        RAISE NOTICE 'If verification still returns invalid, check:';
        RAISE NOTICE '1. Property has seller data';
        RAISE NOTICE '2. Property has buyer data';
        RAISE NOTICE '3. Property data was not modified after verification generation';
        RAISE NOTICE '';
        RAISE NOTICE 'To fix: Regenerate verification code from print page';
    END IF;
END $$;

SELECT 'Diagnostic complete.' as status;
