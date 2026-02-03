-- Simple rename script - no transaction blocks
-- This will work even if previous transaction failed

-- First, drop the old empty singular tables
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;

-- Now rename the plural tables (with data) to singular
ALTER TABLE org."CompanyOwners" RENAME TO "CompanyOwner";
ALTER TABLE org."CompanyOwnerAddresses" RENAME TO "CompanyOwnerAddress";
ALTER TABLE org."CompanyOwnerAddressHistories" RENAME TO "CompanyOwnerAddressHistory";

-- Verify the result
SELECT 'CompanyOwner' as table_name, COUNT(*) as record_count FROM org."CompanyOwner";
SELECT 'CompanyOwnerAddress' as table_name, COUNT(*) as record_count FROM org."CompanyOwnerAddress";
SELECT 'CompanyOwnerAddressHistory' as table_name, COUNT(*) as record_count FROM org."CompanyOwnerAddressHistory";
