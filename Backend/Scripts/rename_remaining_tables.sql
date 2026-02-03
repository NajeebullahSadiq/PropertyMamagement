-- Rename only the tables that still exist (after CASCADE drop)

-- Rename CompanyOwners to CompanyOwner
ALTER TABLE IF EXISTS org."CompanyOwners" RENAME TO "CompanyOwner";

-- Rename CompanyOwnerAddresses to CompanyOwnerAddress  
ALTER TABLE IF EXISTS org."CompanyOwnerAddresses" RENAME TO "CompanyOwnerAddress";

-- Rename CompanyOwnerAddressHistories to CompanyOwnerAddressHistory
ALTER TABLE IF EXISTS org."CompanyOwnerAddressHistories" RENAME TO "CompanyOwnerAddressHistory";

-- Verify the result
SELECT 'CompanyOwner' as table_name, COUNT(*) as record_count FROM org."CompanyOwner";
