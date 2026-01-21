-- =====================================================
-- PRMIS Database - Full Module Deployment Script
-- =====================================================
-- This script deploys all modules in the correct order
-- Safe for both empty databases and existing production
-- All statements are idempotent (IF NOT EXISTS)
-- =====================================================

-- Execution Order:
-- 1. Shared/Lookup (look schema)
-- 2. UserManagement (public schema)
-- 3. Company (org schema)
-- 4. Property (tr schema)
-- 5. Vehicle (tr schema)
-- 6. Securities (org schema)
-- 7. Audit (log schema)
-- 8. LicenseApplication (org schema)
-- 9. PetitionWriterLicense (org schema)
-- 10. Verification (org schema)
-- 11. ActivityMonitoring (org schema)

\echo '=========================================='
\echo 'Starting PRMIS Module Deployment'
\echo '=========================================='

-- Module 1: Shared/Lookup
\echo 'Deploying Module 1: Shared/Lookup...'
\i 01_Shared_Initial.sql

-- Module 2: UserManagement
\echo 'Deploying Module 2: UserManagement...'
\i 02_UserManagement_Initial.sql

-- Module 3: Company
\echo 'Deploying Module 3: Company...'
\i 03_Company_Initial.sql

-- Module 4: Property
\echo 'Deploying Module 4: Property...'
\i 04_Property_Initial.sql

-- Module 5: Vehicle
\echo 'Deploying Module 5: Vehicle...'
\i 05_Vehicle_Initial.sql

-- Module 6: Securities
\echo 'Deploying Module 6: Securities...'
\i 06_Securities_Initial.sql

-- Module 7: Audit
\echo 'Deploying Module 7: Audit...'
\i 07_Audit_Initial.sql

-- Module 8: LicenseApplication
\echo 'Deploying Module 8: LicenseApplication...'
\i 08_LicenseApplication_Initial.sql

-- Module 9: PetitionWriterLicense
\echo 'Deploying Module 9: PetitionWriterLicense...'
\i 09_PetitionWriterLicense_Initial.sql

-- Module 10: Verification
\echo 'Deploying Module 10: Verification...'
\i 10_Verification_Initial.sql

-- Module 11: ActivityMonitoring
\echo 'Deploying Module 11: ActivityMonitoring...'
\i 11_ActivityMonitoring_Initial.sql

\echo '=========================================='
\echo 'PRMIS Module Deployment Complete!'
\echo '=========================================='
