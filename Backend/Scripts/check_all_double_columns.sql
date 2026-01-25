-- Check for ALL remaining double precision columns in the database
SELECT 
    table_schema,
    table_name, 
    column_name, 
    data_type
FROM information_schema.columns
WHERE data_type = 'double precision'
    AND table_schema IN ('tr', 'org', 'look', 'sec', 'audit')
ORDER BY table_schema, table_name, column_name;
