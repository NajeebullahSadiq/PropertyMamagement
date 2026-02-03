-- Diagnose Company Owner API Issue
-- Check if owner data exists for CompanyId = 2

-- 1. Check if CompanyId = 2 exists
SELECT 
    'Company 2 exists?' as check_type,
    CASE WHEN EXISTS (SELECT 1 FROM org."CompanyDetails" WHERE "Id" = 2) 
        THEN 'YES' 
        ELSE 'NO' 
    END as result;

-- 2. Check if owner exists for CompanyId = 2
SELECT 
    'Owner for Company 2 exists?' as check_type,
    CASE WHEN EXISTS (SELECT 1 FROM org."CompanyOwners" WHERE "CompanyId" = 2) 
        THEN 'YES' 
        ELSE 'NO' 
    END as result;

-- 3. Show Company 2 details
SELECT 
    "Id",
    "Title",
    "TIN",
    "ProvinceId",
    "Status",
    "CreatedAt"
FROM org."CompanyDetails"
WHERE "Id" = 2;

-- 4. Show all owners for Company 2
SELECT 
    co."Id",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    co."CompanyId",
    co."DateofBirth",
    co."ElectronicNationalIdNumber",
    co."PhoneNumber",
    co."OwnerProvinceId",
    co."OwnerDistrictId",
    co."OwnerVillage",
    owner_prov."Dari" as owner_province_name,
    owner_dist."Dari" as owner_district_name
FROM org."CompanyOwners" co
LEFT JOIN look."Location" owner_prov ON owner_prov."ID" = co."OwnerProvinceId"
LEFT JOIN look."Location" owner_dist ON owner_dist."ID" = co."OwnerDistrictId"
WHERE co."CompanyId" = 2;

-- 5. Check CompanyId data type and values
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "FatherName",
    pg_typeof("CompanyId") as companyid_type
FROM org."CompanyOwners"
WHERE "CompanyId" IN (1, 2, 3, 4, 5)
ORDER BY "CompanyId";

-- 6. Count owners per company (first 10 companies)
SELECT 
    cd."Id" as company_id,
    cd."Title" as company_name,
    COUNT(co."Id") as owner_count
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwners" co ON co."CompanyId" = cd."Id"
WHERE cd."Id" <= 10
GROUP BY cd."Id", cd."Title"
ORDER BY cd."Id";

-- 7. Check for NULL CompanyId values
SELECT 
    'Owners with NULL CompanyId' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CompanyId" IS NULL;

-- 8. Check if there are any owners at all
SELECT 
    'Total Owners' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners";

-- 9. Show first 10 owners with their company IDs
SELECT 
    co."Id",
    co."CompanyId",
    co."FirstName",
    co."FatherName",
    cd."Title" as company_name
FROM org."CompanyOwners" co
LEFT JOIN org."CompanyDetails" cd ON cd."Id" = co."CompanyId"
ORDER BY co."Id"
LIMIT 10;
