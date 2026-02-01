-- =====================================================
-- Script: Add Witness Fields (GrandFatherName, WitnessSide, Des)
-- Date: 2026-01-31
-- Description: Adds three new fields to WitnessDetails and VehiclesWitnessDetails tables
-- =====================================================

-- INSTRUCTIONS:
-- 1. Connect to your PostgreSQL database
-- 2. Run this script using pgAdmin or psql command line
-- 3. Command: psql -h localhost -U postgres -d PRMIS -f add_witness_fields.sql

-- =====================================================
-- Add fields to Property WitnessDetails table
-- =====================================================

-- Add GrandFatherName field
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'WitnessDetails' 
        AND column_name = 'GrandFatherName'
    ) THEN
        ALTER TABLE tr."WitnessDetails" 
        ADD COLUMN "GrandFatherName" VARCHAR(255);
        
        RAISE NOTICE 'Added GrandFatherName column to WitnessDetails';
    ELSE
        RAISE NOTICE 'GrandFatherName column already exists in WitnessDetails';
    END IF;
END $$;

-- Add WitnessSide field (Buyer or Seller)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'WitnessDetails' 
        AND column_name = 'WitnessSide'
    ) THEN
        ALTER TABLE tr."WitnessDetails" 
        ADD COLUMN "WitnessSide" VARCHAR(50);
        
        RAISE NOTICE 'Added WitnessSide column to WitnessDetails';
    ELSE
        RAISE NOTICE 'WitnessSide column already exists in WitnessDetails';
    END IF;
END $$;

-- Add Des field (جزئیات دیگر)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'WitnessDetails' 
        AND column_name = 'Des'
    ) THEN
        ALTER TABLE tr."WitnessDetails" 
        ADD COLUMN "Des" VARCHAR(1000);
        
        RAISE NOTICE 'Added Des column to WitnessDetails';
    ELSE
        RAISE NOTICE 'Des column already exists in WitnessDetails';
    END IF;
END $$;

-- =====================================================
-- Add fields to Vehicle VehiclesWitnessDetails table
-- =====================================================

-- Add GrandFatherName field
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesWitnessDetails' 
        AND column_name = 'GrandFatherName'
    ) THEN
        ALTER TABLE tr."VehiclesWitnessDetails" 
        ADD COLUMN "GrandFatherName" VARCHAR(255);
        
        RAISE NOTICE 'Added GrandFatherName column to VehiclesWitnessDetails';
    ELSE
        RAISE NOTICE 'GrandFatherName column already exists in VehiclesWitnessDetails';
    END IF;
END $$;

-- Add WitnessSide field (Buyer or Seller)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesWitnessDetails' 
        AND column_name = 'WitnessSide'
    ) THEN
        ALTER TABLE tr."VehiclesWitnessDetails" 
        ADD COLUMN "WitnessSide" VARCHAR(50);
        
        RAISE NOTICE 'Added WitnessSide column to VehiclesWitnessDetails';
    ELSE
        RAISE NOTICE 'WitnessSide column already exists in VehiclesWitnessDetails';
    END IF;
END $$;

-- Add Des field (جزئیات دیگر)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'VehiclesWitnessDetails' 
        AND column_name = 'Des'
    ) THEN
        ALTER TABLE tr."VehiclesWitnessDetails" 
        ADD COLUMN "Des" VARCHAR(1000);
        
        RAISE NOTICE 'Added Des column to VehiclesWitnessDetails';
    ELSE
        RAISE NOTICE 'Des column already exists in VehiclesWitnessDetails';
    END IF;
END $$;

-- =====================================================
-- Add comments for documentation
-- =====================================================

COMMENT ON COLUMN tr."WitnessDetails"."GrandFatherName" IS 'نام پدر کلان شاهد';
COMMENT ON COLUMN tr."WitnessDetails"."WitnessSide" IS 'شاهد از طرف (Buyer/Seller)';
COMMENT ON COLUMN tr."WitnessDetails"."Des" IS 'جزئیات دیگر - اختیاری';

COMMENT ON COLUMN tr."VehiclesWitnessDetails"."GrandFatherName" IS 'نام پدر کلان شاهد';
COMMENT ON COLUMN tr."VehiclesWitnessDetails"."WitnessSide" IS 'شاهد از طرف (Buyer/Seller)';
COMMENT ON COLUMN tr."VehiclesWitnessDetails"."Des" IS 'جزئیات دیگر - اختیاری';

-- =====================================================
-- Verification Query
-- =====================================================

-- Verify Property WitnessDetails columns
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'tr' 
  AND table_name = 'WitnessDetails'
  AND column_name IN ('GrandFatherName', 'WitnessSide', 'Des')
ORDER BY column_name;

-- Verify Vehicle VehiclesWitnessDetails columns
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'tr' 
  AND table_name = 'VehiclesWitnessDetails'
  AND column_name IN ('GrandFatherName', 'WitnessSide', 'Des')
ORDER BY column_name;
