-- Quick check for CompanyId issue

-- 1. Check owner with Id=5 (CompanyId should be 2)
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "Status"
FROM org."CompanyOwners"
WHERE "Id" = 5;

-- 2. Try to find using CompanyId = 2
SELECT 
    "Id",
    "CompanyId",
    "FirstName"
FROM org."CompanyOwners"
WHERE "CompanyId" = 2;

-- 3. Check Status field for all owners
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "Status",
    CASE 
        WHEN "Status" IS NULL THEN 'NULL'
        WHEN "Status" = true THEN 'TRUE'
        WHEN "Status" = false THEN 'FALSE'
    END as status_value
FROM org."CompanyOwners"
ORDER BY "Id"
LIMIT 10;
