-- Fix CompanyOwner tables - Step by Step
-- Run each section separately if needed

-- STEP 1: Rollback any failed transaction
ROLLBACK;

-- STEP 2: Check what tables exist
SELECT 
    schemaname, 
    tablename 
FROM pg_tables 
WHERE schemaname = 'org' 
  AND tablename LIKE '%ompanyOwner%'
ORDER BY tablename;

-- STEP 3: Check record counts
SELECT 'CompanyOwner (singular)' as table_name, COUNT(*) as records 
FROM org."CompanyOwner";

SELECT 'CompanyOwners (plural)' as table_name, COUNT(*) as records 
FROM org."CompanyOwners";

-- STEP 4: Drop the empty singular tables (run this separately)
-- Uncomment and run these one at a time:

-- DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
-- DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
-- DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;

-- STEP 5: Rename plural to singular (run after step 4)
-- Uncomment and run these one at a time:

-- ALTER TABLE org."CompanyOwners" RENAME TO "CompanyOwner";
-- ALTER TABLE org."CompanyOwnerAddresses" RENAME TO "CompanyOwnerAddress";
-- ALTER TABLE org."CompanyOwnerAddressHistories" RENAME TO "CompanyOwnerAddressHistory";

-- STEP 6: Verify the result
-- SELECT 'CompanyOwner' as table_name, COUNT(*) as records FROM org."CompanyOwner";
