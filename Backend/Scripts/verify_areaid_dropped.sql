-- Verify that AreaId column has been dropped

-- Check all columns in LicenseDetails table
SELECT column_name, data_type, is_nullable
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'LicenseDetails'
ORDER BY ordinal_position;

-- Check if AreaId still exists (should return 0 rows)
SELECT COUNT(*) as areaid_exists
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'LicenseDetails'
  AND column_name = 'AreaId';
