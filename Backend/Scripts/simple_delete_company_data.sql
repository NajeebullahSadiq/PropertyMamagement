-- Simple delete - run each command separately if needed

-- First rollback any failed transaction
ROLLBACK;

-- Delete CompanyDetails (CASCADE will delete related records)
DELETE FROM org."CompanyDetails";

-- Verify deletion
SELECT 'CompanyDetails' as table_name, COUNT(*) as records FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwner', COUNT(*) FROM org."CompanyOwner"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails";
