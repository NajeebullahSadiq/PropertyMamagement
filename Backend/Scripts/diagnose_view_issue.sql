-- DIAGNOSTIC: Check what's actually in the database

-- 1. Check which database you're connected to
SELECT current_database();

-- 2. List ALL views named LicenseView in ANY schema
SELECT schemaname, viewname, definition
FROM pg_views
WHERE viewname = 'LicenseView';

-- 3. Check columns in public.LicenseView
SELECT column_name, data_type, ordinal_position
FROM information_schema.columns 
WHERE table_schema = 'public' 
  AND table_name = 'LicenseView'
ORDER BY ordinal_position;

-- 4. Try to select from the view directly
SELECT * FROM public."LicenseView" LIMIT 1;

-- 5. Check if there's a view without quotes (lowercase)
SELECT column_name 
FROM information_schema.columns 
WHERE table_schema = 'public' 
  AND table_name = 'licenseview'  -- lowercase
ORDER BY ordinal_position;
