-- Simple check for verification code PRO-2026-D3XU8P

-- Check if code exists
SELECT 
    COUNT(*) as verification_count,
    CASE 
        WHEN COUNT(*) = 0 THEN '❌ Verification code PRO-2026-D3XU8P does NOT exist in database'
        ELSE '✓ Verification code EXISTS'
    END as result
FROM org."DocumentVerifications"
WHERE "VerificationCode" = 'PRO-2026-D3XU8P';

-- If it exists, show details
SELECT * FROM org."DocumentVerifications"
WHERE "VerificationCode" = 'PRO-2026-D3XU8P';

-- Show all verification codes (to see what exists)
SELECT 
    "VerificationCode",
    "DocumentType",
    "DocumentId",
    "CreatedAt"
FROM org."DocumentVerifications"
ORDER BY "CreatedAt" DESC
LIMIT 10;
