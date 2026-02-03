-- Rename CompanyOwner tables from plural to singular
-- This matches the business logic: one owner per company

BEGIN;

-- First, drop the old empty singular tables if they exist
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;

-- Now rename the plural tables (with data) to singular
-- 1. Rename CompanyOwners to CompanyOwner
ALTER TABLE org."CompanyOwners" RENAME TO "CompanyOwner";

-- 2. Rename CompanyOwnerAddresses to CompanyOwnerAddress
ALTER TABLE org."CompanyOwnerAddresses" RENAME TO "CompanyOwnerAddress";

-- 3. Rename CompanyOwnerAddressHistories to CompanyOwnerAddressHistory
ALTER TABLE org."CompanyOwnerAddressHistories" RENAME TO "CompanyOwnerAddressHistory";

-- Verify the rename worked
SELECT 'CompanyOwner' as table_name, COUNT(*) as record_count FROM org."CompanyOwner";
SELECT 'CompanyOwnerAddress' as table_name, COUNT(*) as record_count FROM org."CompanyOwnerAddress";
SELECT 'CompanyOwnerAddressHistory' as table_name, COUNT(*) as record_count FROM org."CompanyOwnerAddressHistory";

COMMIT;
