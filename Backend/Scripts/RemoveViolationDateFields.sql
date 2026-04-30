-- Remove ViolationDate and ClosureDate columns from ActivityMonitoringRecords table
-- These fields are being removed from the violations section (۳. تخلفات دفاتر رهنمای معاملات)
-- Date: 2026-04-28

DO $$ 
BEGIN
    -- Drop ViolationDate column if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'ViolationDate'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        DROP COLUMN "ViolationDate";
        
        RAISE NOTICE 'Column ViolationDate dropped successfully';
    ELSE
        RAISE NOTICE 'Column ViolationDate does not exist';
    END IF;

    -- Drop ClosureDate column if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'ClosureDate'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        DROP COLUMN "ClosureDate";
        
        RAISE NOTICE 'Column ClosureDate dropped successfully';
    ELSE
        RAISE NOTICE 'Column ClosureDate does not exist';
    END IF;

END $$;

-- Verify the columns have been removed
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'ActivityMonitoringRecords'
AND column_name IN ('ViolationDate', 'ClosureDate');

-- If the above query returns no rows, the columns have been successfully removed
