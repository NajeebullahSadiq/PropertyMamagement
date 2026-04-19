-- Fix Status values in Company module tables
-- License Status is now based on ExpireDate (تاریخ ختم جواز):
--   - If ExpireDate >= today: Status = true (active)
--   - If ExpireDate < today: Status = false (inactive/expired)
--   - If ExpireDate is NULL: Status = true (treat as active)
-- Run this script once to fix existing data

-- Fix LicenseDetails (جواز‌ها) - Status based on ExpireDate
UPDATE org."LicenseDetails" SET "Status" = true WHERE "ExpireDate" IS NULL OR "ExpireDate" >= CURRENT_DATE;
UPDATE org."LicenseDetails" SET "Status" = false WHERE "ExpireDate" IS NOT NULL AND "ExpireDate" < CURRENT_DATE;

-- Fix Guarantors (تضمین‌کنندگان) - always active on creation
UPDATE org."Guarantors" SET "Status" = true WHERE "Status" IS NULL;

-- Fix CompanyOwner (مالک دفتر) - always active on creation
UPDATE org."CompanyOwner" SET "Status" = true WHERE "Status" IS NULL;

-- Fix Gaurantee (تضمین‌ها) - always active on creation  
UPDATE org."Gaurantee" SET "Status" = true WHERE "Status" IS NULL;

-- Fix CompanyOwnerAddress (آدرس‌ها) - always active on creation
UPDATE org."CompanyOwnerAddress" SET "Status" = true WHERE "Status" IS NULL;

-- Fix Haqulemtyaz (حق‌الامتیاز) - always active on creation
UPDATE org."Haqulemtyaz" SET "Status" = true WHERE "Status" IS NULL;

-- Fix PeriodicForm (فارم دوره‌ای) - always active on creation
UPDATE org."PeriodicForm" SET "Status" = true WHERE "Status" IS NULL;

-- Verify the fix
SELECT 'LicenseDetails' AS "Table", COUNT(*) AS "TotalRecords", 
       COUNT(*) FILTER (WHERE "Status" = true) AS "Active",
       COUNT(*) FILTER (WHERE "Status" = false) AS "Expired",
       COUNT(*) FILTER (WHERE "Status" IS NULL) AS "StatusNull"
FROM org."LicenseDetails"
UNION ALL
SELECT 'Guarantors', COUNT(*), 
       COUNT(*) FILTER (WHERE "Status" = true),
       COUNT(*) FILTER (WHERE "Status" = false),
       COUNT(*) FILTER (WHERE "Status" IS NULL)
FROM org."Guarantors"
UNION ALL
SELECT 'CompanyOwner', COUNT(*), 
       COUNT(*) FILTER (WHERE "Status" = true),
       COUNT(*) FILTER (WHERE "Status" = false),
       COUNT(*) FILTER (WHERE "Status" IS NULL)
FROM org."CompanyOwner";
