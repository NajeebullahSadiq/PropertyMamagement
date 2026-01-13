-- Migration: Split serial number fields into start/end pairs for SecuritiesDistributions
-- Date: 2026-01-13

-- Add new start/end columns for Property Sale
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "PropertySaleSerialStart" VARCHAR(100);
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "PropertySaleSerialEnd" VARCHAR(100);

-- Add new start/end columns for Bay Wafa
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "BayWafaSerialStart" VARCHAR(100);
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "BayWafaSerialEnd" VARCHAR(100);

-- Add new start/end columns for Rent
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "RentSerialStart" VARCHAR(100);
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "RentSerialEnd" VARCHAR(100);

-- Add new start/end columns for Vehicle Sale
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "VehicleSaleSerialStart" VARCHAR(100);
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "VehicleSaleSerialEnd" VARCHAR(100);

-- Add new start/end columns for Vehicle Exchange
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "VehicleExchangeSerialStart" VARCHAR(100);
ALTER TABLE "SecuritiesDistributions" ADD COLUMN IF NOT EXISTS "VehicleExchangeSerialEnd" VARCHAR(100);

-- Migrate existing data: copy old serial numbers to start fields
UPDATE "SecuritiesDistributions"
SET "PropertySaleSerialStart" = "PropertySaleSerialNumber"
WHERE "PropertySaleSerialNumber" IS NOT NULL
  AND "PropertySaleSerialStart" IS NULL;

UPDATE "SecuritiesDistributions"
SET "BayWafaSerialStart" = "BayWafaSerialNumber"
WHERE "BayWafaSerialNumber" IS NOT NULL
  AND "BayWafaSerialStart" IS NULL;

UPDATE "SecuritiesDistributions"
SET "RentSerialStart" = "RentSerialNumber"
WHERE "RentSerialNumber" IS NOT NULL
  AND "RentSerialStart" IS NULL;

UPDATE "SecuritiesDistributions"
SET "VehicleSaleSerialStart" = "VehicleSaleSerialNumber"
WHERE "VehicleSaleSerialNumber" IS NOT NULL
  AND "VehicleSaleSerialStart" IS NULL;

UPDATE "SecuritiesDistributions"
SET "VehicleExchangeSerialStart" = "VehicleExchangeSerialNumber"
WHERE "VehicleExchangeSerialNumber" IS NOT NULL
  AND "VehicleExchangeSerialStart" IS NULL;

-- Drop old columns (only if they exist)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'SecuritiesDistributions' AND column_name = 'PropertySaleSerialNumber') THEN
        ALTER TABLE "SecuritiesDistributions" DROP COLUMN "PropertySaleSerialNumber";
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'SecuritiesDistributions' AND column_name = 'BayWafaSerialNumber') THEN
        ALTER TABLE "SecuritiesDistributions" DROP COLUMN "BayWafaSerialNumber";
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'SecuritiesDistributions' AND column_name = 'RentSerialNumber') THEN
        ALTER TABLE "SecuritiesDistributions" DROP COLUMN "RentSerialNumber";
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'SecuritiesDistributions' AND column_name = 'VehicleSaleSerialNumber') THEN
        ALTER TABLE "SecuritiesDistributions" DROP COLUMN "VehicleSaleSerialNumber";
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'SecuritiesDistributions' AND column_name = 'VehicleExchangeSerialNumber') THEN
        ALTER TABLE "SecuritiesDistributions" DROP COLUMN "VehicleExchangeSerialNumber";
    END IF;
END $$;
