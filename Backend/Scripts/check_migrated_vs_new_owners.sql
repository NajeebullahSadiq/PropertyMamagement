-- Compare Migrated vs New Owner Records
-- Find what's different between working and non-working owners

-- Check Status field (might be NULL for migrated data)
SELECT 
    'Migrated owners with NULL Status' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
  AND "Status" IS NULL;

-- Check Status field for new owners
SELECT 
    'New owners with NULL Status' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CreatedBy" != 'MIGRATION_SCRIPT'
  AND "Status" IS NULL;

-- Compare Status values
SELECT 
    "Status",
    "CreatedBy",
    COUNT(*) as count
FROM org."CompanyOwners"
GROUP BY "Status", "CreatedBy"
ORDER BY "CreatedBy", "Status";

-- Check all fields for migrated owner (CompanyId = 2)
SELECT 
    co."Id",
    co."CompanyId",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    co."EducationLevelId",
    co."DateofBirth",
    co."ElectronicNationalIdNumber",
    co."PhoneNumber",
    co."WhatsAppNumber",
    co."OwnerProvinceId",
    co."OwnerDistrictId",
    co."OwnerVillage",
    co."PermanentProvinceId",
    co."PermanentDistrictId",
    co."PermanentVillage",
    co."Status",
    co."CreatedBy",
    co."CreatedAt",
    co."PothoPath"
FROM org."CompanyOwners" co
WHERE co."CompanyId" = 2;

-- Check if Status field is causing the filter
SELECT 
    'Owners with Status = true' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "Status" = true;

SELECT 
    'Owners with Status = false' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "Status" = false;

SELECT 
    'Owners with Status IS NULL' as check_type,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "Status" IS NULL;

-- Show first 5 migrated owners
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "FatherName",
    "Status",
    "CreatedBy"
FROM org."CompanyOwners"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
ORDER BY "Id"
LIMIT 5;

-- Show first 5 new owners (if any)
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "FatherName",
    "Status",
    "CreatedBy"
FROM org."CompanyOwners"
WHERE "CreatedBy" != 'MIGRATION_SCRIPT' OR "CreatedBy" IS NULL
ORDER BY "Id"
LIMIT 5;
