-- Check CompanyId Type Mismatch Issue
-- The API can't find owners even though they exist in the database

-- 1. Check the exact CompanyId value and type for owner Id=5
SELECT 
    "Id",
    "CompanyId",
    pg_typeof("CompanyId") as companyid_type,
    "FirstName",
    "FatherName",
    "CreatedBy"
FROM org."CompanyOwners"
WHERE "Id" = 5;

-- 2. Check all CompanyId values for first 10 owners
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "FatherName"
FROM org."CompanyOwners"
ORDER BY "Id"
LIMIT 10;

-- 3. Try to find owner with CompanyId = 2 using different comparisons
SELECT 
    'Using = operator' as method,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CompanyId" = 2;

SELECT 
    'Using CAST to int' as method,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE CAST("CompanyId" AS INTEGER) = 2;

SELECT 
    'Using string comparison' as method,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CompanyId"::text = '2';

-- 4. Check if CompanyId is NULL
SELECT 
    'CompanyId IS NULL' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CompanyId" IS NULL;

-- 5. Show the actual CompanyId value as different types
SELECT 
    "Id",
    "CompanyId",
    "CompanyId"::text as companyid_as_text,
    "CompanyId"::integer as companyid_as_int,
    LENGTH("CompanyId"::text) as length,
    "FirstName"
FROM org."CompanyOwners"
WHERE "Id" = 5;

-- 6. Check CompanyDetails table for Company 2
SELECT 
    "Id",
    pg_typeof("Id") as id_type,
    "Title"
FROM org."CompanyDetails"
WHERE "Id" = 2;

-- 7. Check if there's a foreign key constraint
SELECT
    tc.constraint_name,
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.table_schema = 'org'
    AND tc.table_name = 'CompanyOwners'
    AND tc.constraint_type = 'FOREIGN KEY';
