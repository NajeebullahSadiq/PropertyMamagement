-- Find the correct table names for Property module

-- List all tables in the property schema
SELECT 
    table_schema,
    table_name,
    table_type
FROM information_schema.tables
WHERE table_schema = 'property'
ORDER BY table_name;

-- Also check if tables might be in public schema
SELECT 
    table_schema,
    table_name,
    table_type
FROM information_schema.tables
WHERE table_name ILIKE '%seller%' 
   OR table_name ILIKE '%buyer%' 
   OR table_name ILIKE '%property%'
ORDER BY table_schema, table_name;
