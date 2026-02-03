-- Delete all company-related data to allow fresh migration
-- Run this before re-running the migration

-- Delete audit tables
TRUNCATE TABLE log."Companyowneraudit" CASCADE;
TRUNCATE TABLE log."Guarantorsaudit" CASCADE;
TRUNCATE TABLE log."Graunteeaudit" CASCADE;
TRUNCATE TABLE log."Licenseaudit" CASCADE;
TRUNCATE TABLE log."Companydetailsaudit" CASCADE;

-- Delete company owner tables
TRUNCATE TABLE org."CompanyOwnerAddressHistory" CASCADE;
TRUNCATE TABLE org."CompanyOwnerAddress" CASCADE;
TRUNCATE TABLE org."CompanyOwner" CASCADE;

-- Delete other company tables
TRUNCATE TABLE org."CompanyCancellationInfo" CASCADE;
TRUNCATE TABLE org."CompanyAccountInfo" CASCADE;
TRUNCATE TABLE org."Guarantors" CASCADE;
TRUNCATE TABLE org."Gaurantees" CASCADE;
TRUNCATE TABLE org."LicenseDetails" CASCADE;
TRUNCATE TABLE org."CompanyDetails" CASCADE;

-- Verify deletion
SELECT 'CompanyDetails' as table_name, COUNT(*) as records FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwner', COUNT(*) FROM org."CompanyOwner"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails";
