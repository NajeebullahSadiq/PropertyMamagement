-- Check which table actually exists in the database

-- Check for singular table name
SELECT 'CompanyOwner (singular)' as table_name, COUNT(*) as record_count
FROM org."CompanyOwner"
WHERE true;

-- Check for plural table name  
SELECT 'CompanyOwners (plural)' as table_name, COUNT(*) as record_count
FROM org."CompanyOwners"
WHERE true;
