-- Migration Script: Add Apartment and Deed Fields to PropertyDetails
-- Date: 2026-04-16
-- Description: Adds fields for apartment number, above/below boundaries, and private deed number
-- Database: PostgreSQL
-- Schema: tr

-- Add ApartmentNumber column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr'
        AND table_name = 'PropertyDetails' 
        AND column_name = 'ApartmentNumber'
    ) THEN
        ALTER TABLE tr."PropertyDetails"
        ADD COLUMN "ApartmentNumber" VARCHAR(100) NULL;
        RAISE NOTICE 'Added ApartmentNumber column to PropertyDetails table';
    ELSE
        RAISE NOTICE 'ApartmentNumber column already exists in PropertyDetails table';
    END IF;
END $$;

-- Add Above column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr'
        AND table_name = 'PropertyDetails' 
        AND column_name = 'Above'
    ) THEN
        ALTER TABLE tr."PropertyDetails"
        ADD COLUMN "Above" VARCHAR(500) NULL;
        RAISE NOTICE 'Added Above column to PropertyDetails table';
    ELSE
        RAISE NOTICE 'Above column already exists in PropertyDetails table';
    END IF;
END $$;

-- Add Below column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr'
        AND table_name = 'PropertyDetails' 
        AND column_name = 'Below'
    ) THEN
        ALTER TABLE tr."PropertyDetails"
        ADD COLUMN "Below" VARCHAR(500) NULL;
        RAISE NOTICE 'Added Below column to PropertyDetails table';
    ELSE
        RAISE NOTICE 'Below column already exists in PropertyDetails table';
    END IF;
END $$;

-- Add PrivateDeedNumber column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr'
        AND table_name = 'PropertyDetails' 
        AND column_name = 'PrivateDeedNumber'
    ) THEN
        ALTER TABLE tr."PropertyDetails"
        ADD COLUMN "PrivateDeedNumber" VARCHAR(100) NULL;
        RAISE NOTICE 'Added PrivateDeedNumber column to PropertyDetails table';
    ELSE
        RAISE NOTICE 'PrivateDeedNumber column already exists in PropertyDetails table';
    END IF;
END $$;

-- Final message
DO $$
BEGIN
    RAISE NOTICE 'Migration completed successfully';
END $$;
