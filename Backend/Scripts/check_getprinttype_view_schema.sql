-- Check the schema of GetPrintType view to identify column types
SELECT 
    column_name,
    data_type,
    udt_name,
    character_maximum_length,
    numeric_precision
FROM information_schema.columns
WHERE table_name = 'getprinttype'
ORDER BY ordinal_position;
