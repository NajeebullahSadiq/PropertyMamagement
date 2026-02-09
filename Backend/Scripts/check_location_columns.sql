-- Check the structure of Location table to find primary key
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'look'
AND table_name = 'Location'
ORDER BY ordinal_position;
