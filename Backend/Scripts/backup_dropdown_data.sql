-- BACKUP SCRIPT FOR DROPDOWN DATA
-- Run this script BEFORE applying migrations to backup all lookup table data
-- This ensures you can restore dropdown data if anything goes wrong

-- Create backup tables
CREATE TABLE IF NOT EXISTS tr.backup_PropertyTypes AS SELECT * FROM tr."PropertyTypes";
CREATE TABLE IF NOT EXISTS tr.backup_TransactionTypes AS SELECT * FROM tr."TransactionTypes";
CREATE TABLE IF NOT EXISTS tr.backup_EducationLevels AS SELECT * FROM tr."EducationLevels";
CREATE TABLE IF NOT EXISTS tr.backup_IdentityCardTypes AS SELECT * FROM tr."IdentityCardTypes";
CREATE TABLE IF NOT EXISTS tr.backup_AddressTypes AS SELECT * FROM tr."AddressTypes";
CREATE TABLE IF NOT EXISTS tr.backup_GuaranteeTypes AS SELECT * FROM tr."GuaranteeTypes";
CREATE TABLE IF NOT EXISTS tr.backup_PunitTypes AS SELECT * FROM tr."PunitTypes";
CREATE TABLE IF NOT EXISTS tr.backup_Areas AS SELECT * FROM tr."Areas";
CREATE TABLE IF NOT EXISTS tr.backup_ViolationTypes AS SELECT * FROM tr."ViolationTypes";
CREATE TABLE IF NOT EXISTS tr.backup_LostdocumentsTypes AS SELECT * FROM tr."LostdocumentsTypes";
CREATE TABLE IF NOT EXISTS tr.backup_Locations AS SELECT * FROM tr."Locations";

-- Verify backup counts
SELECT 
    'PropertyTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."PropertyTypes"
UNION ALL
SELECT 
    'TransactionTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."TransactionTypes"
UNION ALL
SELECT 
    'EducationLevels' as table_name, 
    COUNT(*) as record_count 
FROM tr."EducationLevels"
UNION ALL
SELECT 
    'IdentityCardTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."IdentityCardTypes"
UNION ALL
SELECT 
    'AddressTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."AddressTypes"
UNION ALL
SELECT 
    'GuaranteeTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."GuaranteeTypes"
UNION ALL
SELECT 
    'PunitTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."PunitTypes"
UNION ALL
SELECT 
    'Areas' as table_name, 
    COUNT(*) as record_count 
FROM tr."Areas"
UNION ALL
SELECT 
    'ViolationTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."ViolationTypes"
UNION ALL
SELECT 
    'LostdocumentsTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."LostdocumentsTypes"
UNION ALL
SELECT 
    'Locations' as table_name, 
    COUNT(*) as record_count 
FROM tr."Locations";

-- Display message
SELECT 'BACKUP COMPLETED - All dropdown data has been backed up!' as status;
