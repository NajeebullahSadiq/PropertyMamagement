-- Verify Owner Data Mapping
-- Check if owner data is correctly mapped from source to database

-- Sample owner records with all fields
SELECT 
    co."Id",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    co."DateofBirth",
    co."ElectronicNationalIdNumber",
    co."PhoneNumber",
    el."Name" as education_level,
    owner_prov."Name" as owner_province,
    owner_dist."Name" as owner_district,
    co."OwnerVillage",
    perm_prov."Name" as permanent_province,
    perm_dist."Name" as permanent_district,
    co."PermanentVillage",
    co."Status",
    cd."Title" as company_name
FROM org."CompanyOwners" co
LEFT JOIN org."CompanyDetails" cd ON cd."Id" = co."CompanyId"
LEFT JOIN look."EducationLevel" el ON el."ID" = co."EducationLevelId"
LEFT JOIN look."Location" owner_prov ON owner_prov."ID" = co."OwnerProvinceId"
LEFT JOIN look."Location" owner_dist ON owner_dist."ID" = co."OwnerDistrictId"
LEFT JOIN look."Location" perm_prov ON perm_prov."ID" = co."PermanentProvinceId"
LEFT JOIN look."Location" perm_dist ON perm_dist."ID" = co."PermanentDistrictId"
ORDER BY co."Id"
LIMIT 20;

-- Check for NULL values in important fields
SELECT 
    'FirstName NULL' as issue,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "FirstName" IS NULL
UNION ALL
SELECT 
    'FatherName NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "FatherName" IS NULL
UNION ALL
SELECT 
    'GrandFatherName NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "GrandFatherName" IS NULL
UNION ALL
SELECT 
    'DateofBirth NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "DateofBirth" IS NULL
UNION ALL
SELECT 
    'ElectronicNationalIdNumber NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "ElectronicNationalIdNumber" IS NULL
UNION ALL
SELECT 
    'PhoneNumber NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "PhoneNumber" IS NULL
UNION ALL
SELECT 
    'OwnerProvinceId NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "OwnerProvinceId" IS NULL
UNION ALL
SELECT 
    'OwnerDistrictId NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "OwnerDistrictId" IS NULL
UNION ALL
SELECT 
    'OwnerVillage NULL',
    COUNT(*)
FROM org."CompanyOwners"
WHERE "OwnerVillage" IS NULL;

-- Find a specific owner with the data you mentioned
SELECT 
    co."Id",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    el."Name" as education_level,
    co."DateofBirth",
    co."ElectronicNationalIdNumber",
    co."PhoneNumber",
    owner_prov."Name" as owner_province,
    owner_dist."Name" as owner_district,
    co."OwnerVillage"
FROM org."CompanyOwners" co
LEFT JOIN look."EducationLevel" el ON el."ID" = co."EducationLevelId"
LEFT JOIN look."Location" owner_prov ON owner_prov."ID" = co."OwnerProvinceId"
LEFT JOIN look."Location" owner_dist ON owner_dist."ID" = co."OwnerDistrictId"
WHERE co."FatherName" = 'محمد غوث'
  AND co."GrandFatherName" = 'محمد قاهر'
LIMIT 5;

-- Check education level mapping
SELECT 
    el."ID",
    el."Name",
    el."Dari",
    COUNT(co."Id") as owner_count
FROM look."EducationLevel" el
LEFT JOIN org."CompanyOwners" co ON co."EducationLevelId" = el."ID"
GROUP BY el."ID", el."Name", el."Dari"
ORDER BY owner_count DESC;
