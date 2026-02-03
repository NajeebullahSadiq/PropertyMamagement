-- Find the actual table names in the database
-- PostgreSQL is case-sensitive with quoted identifiers

SELECT 
    table_schema,
    table_name,
    CASE 
        WHEN table_name LIKE '%Property%' THEN '✓ Property table'
        WHEN table_name LIKE '%Seller%' THEN '✓ Seller table'
        WHEN table_name LIKE '%Buyer%' THEN '✓ Buyer table'
        WHEN table_name LIKE '%Verification%' THEN '✓ Verification table'
        ELSE ''
    END as note
FROM information_schema.tables
WHERE table_schema = 'org'
ORDER BY table_name;
