-- Check both CompanyOwner tables

-- Check singular table
SELECT 'CompanyOwner (singular)' as table_name, COUNT(*) as record_count
FROM org."CompanyOwner";

-- Check plural table
SELECT 'CompanyOwners (plural)' as table_name, COUNT(*) as record_count
FROM org."CompanyOwners";

-- Show structure of singular table
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
  AND table_name = 'CompanyOwner'
ORDER BY ordinal_position;

-- Show structure of plural table
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
  AND table_name = 'CompanyOwners'
ORDER BY ordinal_position;
