-- Add CompanyTitle column to ActivityMonitoringRecords table (if not exists)
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'ActivityMonitoringRecords' 
        AND column_name = 'CompanyTitle'
    ) THEN
        ALTER TABLE org."ActivityMonitoringRecords"
        ADD COLUMN "CompanyTitle" character varying(300) NULL;
        
        COMMENT ON COLUMN org."ActivityMonitoringRecords"."CompanyTitle" IS 'عنوان رهنمایی معاملات - Transaction Guidance Title';
    END IF;
END $$;
