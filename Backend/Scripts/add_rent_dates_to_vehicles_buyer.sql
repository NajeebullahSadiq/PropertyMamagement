-- Add RentStartDate and RentEndDate columns to VehiclesBuyerDetails table
-- Run this script if the migration doesn't apply automatically

DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'RentStartDate'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "RentStartDate" timestamp without time zone NULL;
        RAISE NOTICE 'Added RentStartDate column to VehiclesBuyerDetails';
    ELSE
        RAISE NOTICE 'RentStartDate column already exists in VehiclesBuyerDetails';
    END IF;
END $$;

DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'RentEndDate'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "RentEndDate" timestamp without time zone NULL;
        RAISE NOTICE 'Added RentEndDate column to VehiclesBuyerDetails';
    ELSE
        RAISE NOTICE 'RentEndDate column already exists in VehiclesBuyerDetails';
    END IF;
END $$;
