-- Comprehensive fix for VehiclesBuyerDetails table
-- Run this script directly on your PostgreSQL database to add all missing columns

-- Add NationalIdCardPath column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'NationalIdCardPath'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "NationalIdCardPath" text NULL;
        RAISE NOTICE 'Added NationalIdCardPath column';
    ELSE
        RAISE NOTICE 'NationalIdCardPath column already exists';
    END IF;
END $$;

-- Add RoleType column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'RoleType'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "RoleType" text NULL;
        RAISE NOTICE 'Added RoleType column';
    ELSE
        RAISE NOTICE 'RoleType column already exists';
    END IF;
END $$;

-- Add AuthorizationLetter column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'AuthorizationLetter'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "AuthorizationLetter" text NULL;
        RAISE NOTICE 'Added AuthorizationLetter column';
    ELSE
        RAISE NOTICE 'AuthorizationLetter column already exists';
    END IF;
END $$;

-- Add RentStartDate column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'RentStartDate'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "RentStartDate" timestamp without time zone NULL;
        RAISE NOTICE 'Added RentStartDate column';
    ELSE
        RAISE NOTICE 'RentStartDate column already exists';
    END IF;
END $$;

-- Add RentEndDate column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'RentEndDate'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "RentEndDate" timestamp without time zone NULL;
        RAISE NOTICE 'Added RentEndDate column';
    ELSE
        RAISE NOTICE 'RentEndDate column already exists';
    END IF;
END $$;

-- Add TazkiraType column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'TazkiraType'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "TazkiraType" text NULL;
        RAISE NOTICE 'Added TazkiraType column';
    ELSE
        RAISE NOTICE 'TazkiraType column already exists';
    END IF;
END $$;

-- Add TazkiraVolume column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'TazkiraVolume'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "TazkiraVolume" text NULL;
        RAISE NOTICE 'Added TazkiraVolume column';
    ELSE
        RAISE NOTICE 'TazkiraVolume column already exists';
    END IF;
END $$;

-- Add TazkiraPage column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'TazkiraPage'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "TazkiraPage" text NULL;
        RAISE NOTICE 'Added TazkiraPage column';
    ELSE
        RAISE NOTICE 'TazkiraPage column already exists';
    END IF;
END $$;

-- Add TazkiraNumber column
DO $$ 
BEGIN 
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesBuyerDetails' 
        AND column_name = 'TazkiraNumber'
    ) THEN
        ALTER TABLE tr."VehiclesBuyerDetails" ADD COLUMN "TazkiraNumber" text NULL;
        RAISE NOTICE 'Added TazkiraNumber column';
    ELSE
        RAISE NOTICE 'TazkiraNumber column already exists';
    END IF;
END $$;

-- Verify the table structure
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_schema = 'tr' 
AND table_name = 'VehiclesBuyerDetails'
ORDER BY ordinal_position;
