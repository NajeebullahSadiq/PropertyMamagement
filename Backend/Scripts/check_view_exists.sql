-- Check if LicenseView exists
SELECT * FROM information_schema.views 
WHERE table_name = 'LicenseView';

-- Check what columns exist in the view
SELECT column_name, data_type, ordinal_position
FROM information_schema.columns 
WHERE table_name = 'LicenseView'
ORDER BY ordinal_position;

-- Try to select from the view
SELECT * FROM public."LicenseView" LIMIT 1;
