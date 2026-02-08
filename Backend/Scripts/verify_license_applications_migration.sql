-- Verification Script for License Applications Migration
-- Run this after migration to verify data integrity

-- ============================================================
-- 1. BASIC COUNTS
-- ============================================================

-- Total migrated applications
SELECT 
    'Total Migrated Applications' as metric,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT';

-- Approved vs Withdrawn
SELECT 
    CASE 
        WHEN "IsWithdrawn" = true THEN 'Withdrawn'
        ELSE 'Active'
    END as status,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
GROUP BY "IsWithdrawn";

-- ============================================================
-- 2. SAMPLE RECORDS
-- ============================================================

-- First 10 migrated applications
SELECT 
    "Id",
    "RequestSerialNumber",
    "ApplicantName",
    "ProposedGuideName",
    "RequestDate",
    "Status",
    "IsWithdrawn"
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
ORDER BY "Id"
LIMIT 10;

-- ============================================================
-- 3. DATA QUALITY CHECKS
-- ============================================================

-- Check for missing required fields
SELECT 
    'Missing RequestSerialNumber' as issue,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND ("RequestSerialNumber" IS NULL OR "RequestSerialNumber" = '');

SELECT 
    'Missing ApplicantName' as issue,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND ("ApplicantName" IS NULL OR "ApplicantName" = '');

SELECT 
    'Missing ProposedGuideName' as issue,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND ("ProposedGuideName" IS NULL OR "ProposedGuideName" = '');

-- ============================================================
-- 4. SERIAL NUMBER FORMAT CHECK
-- ============================================================

-- Verify all serial numbers follow KBL-XXXXX format
SELECT 
    'Valid Serial Number Format' as check_type,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND "RequestSerialNumber" ~ '^KBL-[0-9]{5}$';

-- Find any invalid formats
SELECT 
    "Id",
    "RequestSerialNumber",
    "ApplicantName"
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND NOT ("RequestSerialNumber" ~ '^KBL-[0-9]{5}$')
LIMIT 10;

-- ============================================================
-- 5. DATE VALIDATION
-- ============================================================

-- Check for NULL request dates
SELECT 
    'NULL Request Dates' as issue,
    COUNT(*) as count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND "RequestDate" IS NULL;

-- Check date range (should be reasonable historical dates)
SELECT 
    MIN("RequestDate") as earliest_date,
    MAX("RequestDate") as latest_date,
    COUNT(*) as total_with_dates
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND "RequestDate" IS NOT NULL;

-- ============================================================
-- 6. PROVINCE/DISTRICT VALIDATION
-- ============================================================

-- Check province distribution
SELECT 
    COALESCE(p."Name", 'NULL') as province,
    COUNT(*) as count
FROM org."LicenseApplications" la
LEFT JOIN look."Location" p ON la."PermanentProvinceId" = p."ID"
WHERE la."CreatedBy" = 'MIGRATION_SCRIPT'
GROUP BY p."Name"
ORDER BY count DESC;

-- ============================================================
-- 7. WITHDRAWN APPLICATIONS DETAIL
-- ============================================================

-- List all withdrawn applications
SELECT 
    "RequestSerialNumber",
    "ApplicantName",
    "ProposedGuideName",
    "RequestDate"
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
AND "IsWithdrawn" = true
ORDER BY "RequestSerialNumber"
LIMIT 20;

-- ============================================================
-- 8. DUPLICATE CHECK
-- ============================================================

-- Check for duplicate serial numbers
SELECT 
    "RequestSerialNumber",
    COUNT(*) as duplicate_count
FROM org."LicenseApplications"
WHERE "CreatedBy" = 'MIGRATION_SCRIPT'
GROUP BY "RequestSerialNumber"
HAVING COUNT(*) > 1;

-- ============================================================
-- 9. COMPARISON WITH EXISTING DATA
-- ============================================================

-- Compare migrated vs manually entered applications
SELECT 
    CASE 
        WHEN "CreatedBy" = 'MIGRATION_SCRIPT' THEN 'Migrated'
        ELSE 'Manual'
    END as source,
    COUNT(*) as count
FROM org."LicenseApplications"
GROUP BY CASE WHEN "CreatedBy" = 'MIGRATION_SCRIPT' THEN 'Migrated' ELSE 'Manual' END;

-- ============================================================
-- 10. FINAL SUMMARY
-- ============================================================

SELECT 
    'MIGRATION SUMMARY' as report,
    (SELECT COUNT(*) FROM org."LicenseApplications" WHERE "CreatedBy" = 'MIGRATION_SCRIPT') as total_migrated,
    (SELECT COUNT(*) FROM org."LicenseApplications" WHERE "CreatedBy" = 'MIGRATION_SCRIPT' AND "IsWithdrawn" = false) as active_applications,
    (SELECT COUNT(*) FROM org."LicenseApplications" WHERE "CreatedBy" = 'MIGRATION_SCRIPT' AND "IsWithdrawn" = true) as withdrawn_applications,
    (SELECT COUNT(*) FROM org."LicenseApplications" WHERE "CreatedBy" = 'MIGRATION_SCRIPT' AND "RequestDate" IS NULL) as missing_dates,
    (SELECT COUNT(*) FROM org."LicenseApplications" WHERE "CreatedBy" = 'MIGRATION_SCRIPT' AND ("ApplicantName" IS NULL OR "ApplicantName" = '')) as missing_names;
