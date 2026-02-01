-- =============================================
-- Migration: Add CompanyId to Property and Vehicle tables
-- Date: 2026-01-30
-- Description: Adds CompanyId column for company-based data isolation
-- Database: PostgreSQL
-- Note: Tables already have CompanyId in initial schema, this is a safety check
-- =============================================

-- Add CompanyId to PropertyDetails table (if not exists)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr'
        AND table_name = 'PropertyDetails' 
        AND column_name = 'CompanyId'
    ) THEN
        ALTER TABLE tr."PropertyDetails"
        ADD COLUMN "CompanyId" INTEGER NULL;
        
        RAISE NOTICE 'Added CompanyId column to tr.PropertyDetails table';
    ELSE
        RAISE NOTICE 'CompanyId column already exists in tr.PropertyDetails table';
    END IF;
END $$;

-- Create index for PropertyDetails.CompanyId
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'tr'
        AND tablename = 'PropertyDetails' 
        AND indexname = 'IX_PropertyDetails_CompanyId'
    ) THEN
        CREATE INDEX "IX_PropertyDetails_CompanyId"
        ON tr."PropertyDetails" ("CompanyId");
        
        RAISE NOTICE 'Created index IX_PropertyDetails_CompanyId';
    ELSE
        RAISE NOTICE 'Index IX_PropertyDetails_CompanyId already exists';
    END IF;
END $$;

-- Add CompanyId to VehiclesPropertyDetails table (if not exists)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr'
        AND table_name = 'VehiclesPropertyDetails' 
        AND column_name = 'CompanyId'
    ) THEN
        ALTER TABLE tr."VehiclesPropertyDetails"
        ADD COLUMN "CompanyId" INTEGER NULL;
        
        RAISE NOTICE 'Added CompanyId column to tr.VehiclesPropertyDetails table';
    ELSE
        RAISE NOTICE 'CompanyId column already exists in tr.VehiclesPropertyDetails table';
    END IF;
END $$;

-- Create index for VehiclesPropertyDetails.CompanyId
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'tr'
        AND tablename = 'VehiclesPropertyDetails' 
        AND indexname = 'IX_VehiclesPropertyDetails_CompanyId'
    ) THEN
        CREATE INDEX "IX_VehiclesPropertyDetails_CompanyId"
        ON tr."VehiclesPropertyDetails" ("CompanyId");
        
        RAISE NOTICE 'Created index IX_VehiclesPropertyDetails_CompanyId';
    ELSE
        RAISE NOTICE 'Index IX_VehiclesPropertyDetails_CompanyId already exists';
    END IF;
END $$;

-- Final message
DO $$
BEGIN
    RAISE NOTICE 'Migration completed: CompanyId columns verified/added to Property and Vehicle tables';
END $$;
