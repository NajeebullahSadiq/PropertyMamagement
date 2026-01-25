-- Verify LicenseView structure
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'LicenseView' 
  AND table_schema = 'public'
ORDER BY ordinal_position;

-- Test the view with a sample query
SELECT * FROM public."LicenseView" LIMIT 1;
