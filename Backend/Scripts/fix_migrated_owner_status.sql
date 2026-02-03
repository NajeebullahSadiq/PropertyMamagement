-- Fix Migrated Owner Status Field
-- Set Status to true for all migrated owners if it's NULL or false

-- First, check current status distribution
SELECT 
    'Current Status Distribution' as info,
    "Status",
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
GROUP BY "Status";

-- Update all migrated owners to have Status = true
UPDATE org."CompanyOwners"
SET "Status" = true
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
  AND ("Status" IS NULL OR "Status" = false);

-- Show how many were updated
SELECT 
    'Updated migrated owners to Status = true' as result,
    COUNT(*) as count
FROM org."CompanyOwners"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
  AND "Status" = true;

-- Verify the fix for CompanyId = 2
SELECT 
    "Id",
    "CompanyId",
    "FirstName",
    "FatherName",
    "Status",
    "CreatedBy"
FROM org."CompanyOwners"
WHERE "CompanyId" = 2;
