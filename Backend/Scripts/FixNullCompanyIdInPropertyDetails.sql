-- Fix NULL CompanyId values in PropertyDetails
-- Run each section separately if needed

-- Step 1: Count records with NULL CompanyId
SELECT COUNT(*) as NullCompanyIdCount
FROM tr."PropertyDetails"
WHERE "CompanyId" IS NULL;

-- Step 2: Update NULL CompanyId values to match the creator's company
UPDATE tr."PropertyDetails" p
SET "CompanyId" = u."CompanyId"
FROM public."AspNetUsers" u
WHERE p."CreatedBy" = u."Id"
AND p."CompanyId" IS NULL
AND u."CompanyId" IS NOT NULL
AND u."CompanyId" != 0;

-- Step 3: Check remaining NULL values
SELECT COUNT(*) as RemainingNullCount
FROM tr."PropertyDetails"
WHERE "CompanyId" IS NULL;

-- Step 4: List records that still have NULL CompanyId for manual review
SELECT 
    p."Id" as PropertyId,
    p."PNumber" as PropertyNumber,
    p."CreatedBy" as UserId,
    u."UserName" as UserName,
    u."CompanyId" as UserCompanyId,
    p."CreatedAt" as CreatedDate
FROM tr."PropertyDetails" p
LEFT JOIN public."AspNetUsers" u ON p."CreatedBy" = u."Id"
WHERE p."CompanyId" IS NULL
ORDER BY p."CreatedAt" DESC;

-- Step 5: Verify the fix - show distribution of CompanyId values
SELECT 
    CASE 
        WHEN "CompanyId" IS NULL THEN 'NULL (Needs Review)'
        ELSE 'Company ' || "CompanyId"::TEXT
    END as CompanyIdStatus,
    COUNT(*) as RecordCount
FROM tr."PropertyDetails"
GROUP BY "CompanyId"
ORDER BY "CompanyId" NULLS FIRST;
