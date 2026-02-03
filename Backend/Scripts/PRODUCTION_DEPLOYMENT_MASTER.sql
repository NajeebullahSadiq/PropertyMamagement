-- =====================================================
-- PRODUCTION DEPLOYMENT MASTER SCRIPT
-- =====================================================
-- This script should be run AFTER data migration
-- It ensures sequences are properly set for new records
-- =====================================================

\echo ''
\echo '========================================='
\echo 'PRODUCTION DEPLOYMENT - SEQUENCE FIX'
\echo '========================================='
\echo ''
\echo 'This script fixes sequences after data migration'
\echo 'Run this AFTER migrating company data'
\echo ''

-- Fix CompanyDetails sequence
\echo 'Fixing CompanyDetails sequence...'
SELECT setval(
    pg_get_serial_sequence('org."CompanyDetails"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."CompanyDetails"), 1),
    true
);

-- Fix CompanyOwner sequence
\echo 'Fixing CompanyOwner sequence...'
SELECT setval(
    pg_get_serial_sequence('org."CompanyOwner"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."CompanyOwner"), 1),
    true
);

-- Fix LicenseDetails sequence
\echo 'Fixing LicenseDetails sequence...'
SELECT setval(
    pg_get_serial_sequence('org."LicenseDetails"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."LicenseDetails"), 1),
    true
);

-- Fix Guarantors sequence (only if table has data)
\echo 'Fixing Guarantors sequence...'
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM org."Guarantors") > 0 THEN
        PERFORM setval('org."Guarantors_Id_seq"', (SELECT MAX("Id") FROM org."Guarantors"), true);
    ELSE
        PERFORM setval('org."Guarantors_Id_seq"', 1, false);
    END IF;
END $$;

-- Fix CompanyCancellationInfo sequence (only if table has data)
\echo 'Fixing CompanyCancellationInfo sequence...'
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM org."CompanyCancellationInfo") > 0 THEN
        PERFORM setval('org."CompanyCancellationInfo_Id_seq"', (SELECT MAX("Id") FROM org."CompanyCancellationInfo"), true);
    ELSE
        PERFORM setval('org."CompanyCancellationInfo_Id_seq"', 1, false);
    END IF;
END $$;

-- Fix CompanyAccountInfo sequence (only if table has data)
\echo 'Fixing CompanyAccountInfo sequence...'
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM org."CompanyAccountInfo") > 0 THEN
        PERFORM setval('org."CompanyAccountInfo_Id_seq"', (SELECT MAX("Id") FROM org."CompanyAccountInfo"), true);
    ELSE
        PERFORM setval('org."CompanyAccountInfo_Id_seq"', 1, false);
    END IF;
END $$;

\echo ''
\echo 'Verifying sequences...'
\echo ''

-- Verify sequences
SELECT 
    'CompanyDetails' as table_name,
    last_value as next_id,
    is_called
FROM org."CompanyDetails_Id_seq"
UNION ALL
SELECT 
    'CompanyOwner',
    last_value,
    is_called
FROM org."CompanyOwner_Id_seq"
UNION ALL
SELECT 
    'LicenseDetails',
    last_value,
    is_called
FROM org."LicenseDetails_Id_seq"
UNION ALL
SELECT 
    'Guarantors',
    last_value,
    is_called
FROM org."Guarantors_Id_seq"
UNION ALL
SELECT 
    'CompanyCancellationInfo',
    last_value,
    is_called
FROM org."CompanyCancellationInfo_Id_seq"
UNION ALL
SELECT 
    'CompanyAccountInfo',
    last_value,
    is_called
FROM org."CompanyAccountInfo_Id_seq";

\echo ''
\echo '========================================='
\echo 'SEQUENCE FIX COMPLETE!'
\echo '========================================='
\echo ''
\echo 'Next ID for each table will be MAX(Id) + 1'
\echo 'New records will not conflict with migrated data'
\echo ''
