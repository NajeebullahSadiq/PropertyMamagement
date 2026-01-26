-- Diagnose all double precision columns in Company module tables
-- This will help identify which column is causing the type mismatch error

-- Check all columns in CompanyOwner table
SELECT 
    'CompanyOwner' as table_name,
    column_name,
    data_type,
    udt_name,
    character_maximum_length,
    numeric_precision,
    numeric_scale
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'CompanyOwner'
ORDER BY ordinal_position;

-- Check all columns in Guarantor table
SELECT 
    'Guarantor' as table_name,
    column_name,
    data_type,
    udt_name,
    character_maximum_length,
    numeric_precision,
    numeric_scale
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'Guarantor'
ORDER BY ordinal_position;

-- Check all columns in LicenseDetail table
SELECT 
    'LicenseDetail' as table_name,
    column_name,
    data_type,
    udt_name,
    character_maximum_length,
    numeric_precision,
    numeric_scale
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'LicenseDetail'
ORDER BY ordinal_position;

-- Find ALL double precision columns in org schema
SELECT 
    table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND data_type = 'double precision'
ORDER BY table_name, column_name;
