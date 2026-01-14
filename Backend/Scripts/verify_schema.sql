-- =====================================================
-- Schema Verification Script
-- =====================================================
-- Run this script to verify the database schema matches
-- the expected module-based structure.
-- =====================================================

\echo '=========================================='
\echo 'PRMIS Schema Verification'
\echo '=========================================='

-- Check schemas exist
\echo ''
\echo '=== Verifying Schemas ==='
SELECT schema_name, 
       CASE WHEN schema_name IN ('look', 'org', 'tr', 'log', 'public') 
            THEN '✓ Expected' 
            ELSE '? Unknown' 
       END as status
FROM information_schema.schemata
WHERE schema_name NOT IN ('pg_catalog', 'information_schema', 'pg_toast')
ORDER BY schema_name;

-- Verify Shared/Lookup tables (look schema)
\echo ''
\echo '=== Module: Shared/Lookup (look schema) ==='
SELECT table_name,
       CASE WHEN table_name IN ('AddressType', 'Area', 'EducationLevel', 'FormsReference', 
                                 'GuaranteeType', 'IdentityCardType', 'Location', 
                                 'LostdocumentsType', 'PropertyType', 'PUnitType', 
                                 'TransactionType', 'ViolationType')
            THEN '✓ Expected'
            ELSE '? Extra'
       END as status
FROM information_schema.tables
WHERE table_schema = 'look'
ORDER BY table_name;

-- Verify Company tables (org schema)
\echo ''
\echo '=== Module: Company (org schema) ==='
SELECT table_name,
       CASE WHEN table_name IN ('CompanyDetails', 'CompanyOwner', 'CompanyOwnerAddress',
                                 'CompanyOwnerAddressHistory', 'LicenseDetails', 'Gaurantee',
                                 'Guarantors', 'Haqulemtyaz', 'PeriodicForm', 'Seta',
                                 'CompanyAccountInfo', 'CompanyCancellationInfo',
                                 'SecuritiesDistribution', 'PetitionWriterSecurities', 'SecuritiesControl')
            THEN '✓ Expected'
            ELSE '? Extra'
       END as status
FROM information_schema.tables
WHERE table_schema = 'org'
ORDER BY table_name;

-- Verify Transaction tables (tr schema)
\echo ''
\echo '=== Module: Property & Vehicle (tr schema) ==='
SELECT table_name,
       CASE WHEN table_name IN ('PropertyDetails', 'PropertyAddress', 'BuyerDetails',
                                 'SellerDetails', 'WitnessDetails', 'PropertyCancellations',
                                 'PropertyCancellationDocuments', 'VehiclesPropertyDetails',
                                 'VehiclesBuyerDetails', 'VehiclesSellerDetails', 'VehiclesWitnessDetails')
            THEN '✓ Expected'
            ELSE '? Extra'
       END as status
FROM information_schema.tables
WHERE table_schema = 'tr'
ORDER BY table_name;

-- Verify Audit tables (log schema)
\echo ''
\echo '=== Module: Audit (log schema) ==='
SELECT table_name,
       CASE WHEN table_name IN ('Propertyaudit', 'Propertybuyeraudit', 'Propertyselleraudit',
                                 'Vehicleaudit', 'Vehicleselleraudit', 'Vehiclebuyeraudit',
                                 'Licenseaudit', 'Guarantorsaudit', 'Graunteeaudit',
                                 'Companyowneraudit', 'Companydetailsaudit')
            THEN '✓ Expected'
            ELSE '? Extra'
       END as status
FROM information_schema.tables
WHERE table_schema = 'log'
ORDER BY table_name;

-- Verify Identity tables (public schema)
\echo ''
\echo '=== Module: UserManagement (public schema) ==='
SELECT table_name,
       CASE WHEN table_name IN ('AspNetUsers', 'AspNetRoles', 'AspNetUserRoles',
                                 'AspNetUserClaims', 'AspNetRoleClaims', 'AspNetUserLogins',
                                 'AspNetUserTokens', '__EFMigrationsHistory')
            THEN '✓ Expected'
            ELSE '? Extra'
       END as status
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;

-- Summary
\echo ''
\echo '=== Summary ==='
SELECT 
    table_schema as schema,
    COUNT(*) as table_count
FROM information_schema.tables
WHERE table_schema IN ('look', 'org', 'tr', 'log', 'public')
GROUP BY table_schema
ORDER BY table_schema;

\echo ''
\echo '=========================================='
\echo 'Verification Complete'
\echo '=========================================='
