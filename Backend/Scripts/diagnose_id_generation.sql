-- Check if ID columns have proper SERIAL/IDENTITY configuration

-- Check CompanyDetails table structure
SELECT 
    column_name,
    data_type,
    column_default,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
    AND table_name = 'CompanyDetails'
    AND column_name = 'Id';

-- Check if sequence exists and is linked
SELECT 
    pg_get_serial_sequence('org."CompanyDetails"', 'Id') as sequence_name;

-- Check current sequence value
SELECT 
    sequencename,
    last_value,
    is_called
FROM pg_sequences
WHERE schemaname = 'org' 
    AND sequencename LIKE '%CompanyDetails%';

-- Check the last few records
SELECT "Id", "Title", "CreatedAt"
FROM org."CompanyDetails"
ORDER BY "Id" DESC
LIMIT 5;
