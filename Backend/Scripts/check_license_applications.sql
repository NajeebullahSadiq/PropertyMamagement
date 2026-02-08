-- Check License Applications Migration Status
-- Run this to see if any license applications exist

-- 1. Count total license applications
SELECT 'Total License Applications' as check_name, COUNT(*) as count
FROM org."LicenseApplications";

-- 2. Count migrated license applications
SELECT 'Migrated License Applications' as check_name, COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT';

-- 3. Check if any exist with KBL- prefix (migration format)
SELECT 'Applications with KBL- prefix' as check_name, COUNT(*) as count
FROM org."LicenseApplications"
WHERE "RequestSerialNumber" LIKE 'KBL-%';

-- 4. Sample of existing applications (if any)
SELECT "Id", "RequestSerialNumber", "ApplicantName", "ProposedGuideName", 
       "Status", "IsWithdrawn", "CreatedBy", "CreatedAt"
FROM org."LicenseApplications"
LIMIT 10;

-- 5. Count companies that should have applications
SELECT 'Companies with LicenseNo' as check_name, COUNT(*) as count
FROM org."CompanyDetails" cd
JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
WHERE cd."CreatedBy" = 'MIGRATION_SCRIPT';
