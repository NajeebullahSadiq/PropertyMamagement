-- Simple verification check that handles case sensitivity
-- Run this to check if verification code exists

-- First, find all tables in org schema
SELECT '=========================================' as info;
SELECT 'Tables in org schema:' as info;
SELECT '=========================================' as info;

SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'org'
AND table_name ILIKE '%verification%'
ORDER BY table_name;

SELECT '' as separator;

-- Check verification code with different case variations
SELECT '=========================================' as info;
SELECT 'Checking for verification code PRO-2026-D3XU8P' as info;
SELECT '=========================================' as info;

-- Try lowercase table name
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'org' 
        AND table_name = 'documentverifications'
    ) THEN
        RAISE NOTICE 'Found table: org.documentverifications (lowercase)';
        
        -- Check if code exists
        PERFORM 1 FROM org.documentverifications 
        WHERE verificationcode = 'PRO-2026-D3XU8P';
        
        IF FOUND THEN
            RAISE NOTICE '✓ Verification code EXISTS in database';
        ELSE
            RAISE NOTICE '❌ Verification code does NOT exist in database';
        END IF;
    END IF;
END $$;

-- Try PascalCase with quotes
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'org' 
        AND table_name = 'DocumentVerifications'
    ) THEN
        RAISE NOTICE 'Found table: org."DocumentVerifications" (PascalCase)';
        
        -- Check if code exists
        EXECUTE 'SELECT 1 FROM org."DocumentVerifications" WHERE "VerificationCode" = $1'
        INTO STRICT FOUND
        USING 'PRO-2026-D3XU8P';
        
        IF FOUND THEN
            RAISE NOTICE '✓ Verification code EXISTS in database';
        ELSE
            RAISE NOTICE '❌ Verification code does NOT exist in database';
        END IF;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RAISE NOTICE '❌ Verification code does NOT exist in database';
    END IF;
END $$;

SELECT '' as separator;
SELECT '=========================================' as info;
SELECT 'Next Steps:' as info;
SELECT '=========================================' as info;

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE 'If verification code does NOT exist:';
    RAISE NOTICE '1. Find the property in your UI';
    RAISE NOTICE '2. Click "Print" button';
    RAISE NOTICE '3. System will generate verification code';
    RAISE NOTICE '';
    RAISE NOTICE 'If verification code EXISTS but API returns invalid:';
    RAISE NOTICE '1. Check property has seller data';
    RAISE NOTICE '2. Check property has buyer data';
    RAISE NOTICE '3. Regenerate verification from print page';
END $$;
