-- Add TIN column to CompanyDetails table if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'CompanyDetails' 
        AND column_name = 'TIN'
    ) THEN
        ALTER TABLE org."CompanyDetails" 
        ADD COLUMN "TIN" double precision NULL;
    END IF;
END $$;
