-- Cleanup Partial Migration Data
-- Run this before re-running the migration to start fresh

DO $$
BEGIN
    RAISE NOTICE 'Starting cleanup of partial migration data...';
    
    -- Delete in reverse order of dependencies
    DELETE FROM org."CompanyCancellationInfo";
    RAISE NOTICE '✓ Deleted CompanyCancellationInfo records';
    
    DELETE FROM org."LicenseDetails";
    RAISE NOTICE '✓ Deleted LicenseDetails records';
    
    DELETE FROM org."CompanyOwners";
    RAISE NOTICE '✓ Deleted CompanyOwners records';
    
    DELETE FROM org."CompanyDetails";
    RAISE NOTICE '✓ Deleted CompanyDetails records';
    
    RAISE NOTICE ' ';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Cleanup Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE ' ';
    RAISE NOTICE 'Ready to run migration again.';
    RAISE NOTICE ' ';
END $$;

-- Verify cleanup
SELECT 
    'CompanyDetails' as table_name, 
    COUNT(*) as remaining_records 
FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwners', COUNT(*) FROM org."CompanyOwners"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails"
UNION ALL
SELECT 'CancellationInfo', COUNT(*) FROM org."CompanyCancellationInfo";
