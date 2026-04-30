-- Fix ActivityMonitoringRecords table schema to match the model
-- Adds missing columns and removes obsolete ones
-- Date: 2026-04-28

DO $$ 
BEGIN
    RAISE NOTICE 'Fixing ActivityMonitoringRecords table schema...';
    
    -- Add CompanyTitle if missing
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'CompanyTitle'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        ADD COLUMN "CompanyTitle" VARCHAR(300);
        RAISE NOTICE '✓ Added CompanyTitle column';
    ELSE
        RAISE NOTICE '  CompanyTitle already exists';
    END IF;

    -- Add Year if missing
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'Year'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        ADD COLUMN "Year" VARCHAR(20);
        RAISE NOTICE '✓ Added Year column';
    ELSE
        RAISE NOTICE '  Year already exists';
    END IF;

    -- Add MonitoringRemarks if missing
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'MonitoringRemarks'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        ADD COLUMN "MonitoringRemarks" VARCHAR(1000);
        RAISE NOTICE '✓ Added MonitoringRemarks column';
    ELSE
        RAISE NOTICE '  MonitoringRemarks already exists';
    END IF;

    -- Add UpdatedAt if missing
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'UpdatedAt'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        ADD COLUMN "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE;
        RAISE NOTICE '✓ Added UpdatedAt column';
    ELSE
        RAISE NOTICE '  UpdatedAt already exists';
    END IF;

    -- Add UpdatedBy if missing
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'UpdatedBy'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        ADD COLUMN "UpdatedBy" VARCHAR(50);
        RAISE NOTICE '✓ Added UpdatedBy column';
    ELSE
        RAISE NOTICE '  UpdatedBy already exists';
    END IF;

    -- Drop obsolete columns if they exist
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'MonitoringType'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        DROP COLUMN "MonitoringType";
        RAISE NOTICE '✓ Dropped obsolete MonitoringType column';
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'AnnualReportRemarks'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        DROP COLUMN "AnnualReportRemarks";
        RAISE NOTICE '✓ Dropped obsolete AnnualReportRemarks column';
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'ComplaintRegistrationDate'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        DROP COLUMN "ComplaintRegistrationDate";
        RAISE NOTICE '✓ Dropped obsolete ComplaintRegistrationDate column';
    END IF;

    RAISE NOTICE '';
    RAISE NOTICE '✓ ActivityMonitoringRecords table schema fixed successfully';
    
END $$;

-- Verify the schema
SELECT column_name, data_type, character_maximum_length, is_nullable
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name = 'ActivityMonitoringRecords'
ORDER BY ordinal_position;
