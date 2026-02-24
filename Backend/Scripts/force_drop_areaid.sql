-- Force drop AreaId column - run this if the previous script didn't work

-- First, drop any views that might depend on this column
DROP VIEW IF EXISTS public."LicenseView" CASCADE;

-- Drop all constraints that reference AreaId
DO $$ 
DECLARE
    constraint_name text;
BEGIN
    FOR constraint_name IN 
        SELECT conname 
        FROM pg_constraint 
        WHERE conrelid = 'org."LicenseDetails"'::regclass 
        AND conname LIKE '%Area%'
    LOOP
        EXECUTE 'ALTER TABLE org."LicenseDetails" DROP CONSTRAINT IF EXISTS "' || constraint_name || '" CASCADE';
    END LOOP;
END $$;

-- Now drop the column
ALTER TABLE org."LicenseDetails" DROP COLUMN IF EXISTS "AreaId" CASCADE;

-- Verify it's gone
SELECT column_name 
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'LicenseDetails'
  AND column_name = 'AreaId';
-- Should return 0 rows
