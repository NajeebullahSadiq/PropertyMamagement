-- Fix SecuritiesDistribution table - Create if not exists or add missing columns
-- Run this script on your PostgreSQL database

-- First, ensure the org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- Create the table if it doesn't exist (with all correct columns)
CREATE TABLE IF NOT EXISTS org."SecuritiesDistribution" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Tab 1: مشخصات رهنمای معاملات
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "LicenseOwnerName" VARCHAR(200) NOT NULL,
    "LicenseOwnerFatherName" VARCHAR(200) NOT NULL,
    "TransactionGuideName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    
    -- Tab 2: مشخصات اسناد توزیعی
    "DocumentType" INTEGER,
    "PropertySubType" INTEGER,
    "VehicleSubType" INTEGER,
    
    -- Property Document Fields (with start/end serial numbers)
    "PropertySaleCount" INTEGER,
    "PropertySaleSerialStart" VARCHAR(100),
    "PropertySaleSerialEnd" VARCHAR(100),
    "BayWafaCount" INTEGER,
    "BayWafaSerialStart" VARCHAR(100),
    "BayWafaSerialEnd" VARCHAR(100),
    "RentCount" INTEGER,
    "RentSerialStart" VARCHAR(100),
    "RentSerialEnd" VARCHAR(100),
    
    -- Vehicle Document Fields (with start/end serial numbers)
    "VehicleSaleCount" INTEGER,
    "VehicleSaleSerialStart" VARCHAR(100),
    "VehicleSaleSerialEnd" VARCHAR(100),
    "VehicleExchangeCount" INTEGER,
    "VehicleExchangeSerialStart" VARCHAR(100),
    "VehicleExchangeSerialEnd" VARCHAR(100),
    
    -- Registration Book Fields
    "RegistrationBookType" INTEGER,
    "RegistrationBookCount" INTEGER,
    "DuplicateBookCount" INTEGER,
    
    -- Tab 3: قیمت اسناد بهادار
    "PricePerDocument" DECIMAL(18, 2),
    "TotalDocumentsPrice" DECIMAL(18, 2),
    "RegistrationBookPrice" DECIMAL(18, 2),
    "TotalSecuritiesPrice" DECIMAL(18, 2),
    
    -- Tab 4: مشخصات آویز تحویلی و تاریخ توزیع
    "BankReceiptNumber" VARCHAR(100),
    "DeliveryDate" DATE,
    "DistributionDate" DATE,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    "Status" BOOLEAN DEFAULT TRUE
);

-- If table already exists, add missing columns
ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "PropertySaleSerialStart" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "PropertySaleSerialEnd" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "BayWafaSerialStart" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "BayWafaSerialEnd" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "RentSerialStart" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "RentSerialEnd" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "VehicleSaleSerialStart" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "VehicleSaleSerialEnd" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "VehicleExchangeSerialStart" VARCHAR(100);

ALTER TABLE org."SecuritiesDistribution" 
ADD COLUMN IF NOT EXISTS "VehicleExchangeSerialEnd" VARCHAR(100);

-- Migrate existing data if old columns exist
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_schema = 'org' 
               AND table_name = 'SecuritiesDistribution' 
               AND column_name = 'PropertySaleSerialNumber') THEN
        UPDATE org."SecuritiesDistribution"
        SET "PropertySaleSerialStart" = "PropertySaleSerialNumber"
        WHERE "PropertySaleSerialNumber" IS NOT NULL AND "PropertySaleSerialStart" IS NULL;
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_schema = 'org' 
               AND table_name = 'SecuritiesDistribution' 
               AND column_name = 'BayWafaSerialNumber') THEN
        UPDATE org."SecuritiesDistribution"
        SET "BayWafaSerialStart" = "BayWafaSerialNumber"
        WHERE "BayWafaSerialNumber" IS NOT NULL AND "BayWafaSerialStart" IS NULL;
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_schema = 'org' 
               AND table_name = 'SecuritiesDistribution' 
               AND column_name = 'RentSerialNumber') THEN
        UPDATE org."SecuritiesDistribution"
        SET "RentSerialStart" = "RentSerialNumber"
        WHERE "RentSerialNumber" IS NOT NULL AND "RentSerialStart" IS NULL;
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_schema = 'org' 
               AND table_name = 'SecuritiesDistribution' 
               AND column_name = 'VehicleSaleSerialNumber') THEN
        UPDATE org."SecuritiesDistribution"
        SET "VehicleSaleSerialStart" = "VehicleSaleSerialNumber"
        WHERE "VehicleSaleSerialNumber" IS NOT NULL AND "VehicleSaleSerialStart" IS NULL;
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_schema = 'org' 
               AND table_name = 'SecuritiesDistribution' 
               AND column_name = 'VehicleExchangeSerialNumber') THEN
        UPDATE org."SecuritiesDistribution"
        SET "VehicleExchangeSerialStart" = "VehicleExchangeSerialNumber"
        WHERE "VehicleExchangeSerialNumber" IS NOT NULL AND "VehicleExchangeSerialStart" IS NULL;
    END IF;
END $$;

-- Create indexes if they don't exist
CREATE UNIQUE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_RegistrationNumber" 
ON org."SecuritiesDistribution" ("RegistrationNumber");

CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_LicenseNumber" 
ON org."SecuritiesDistribution" ("LicenseNumber");

CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_BankReceiptNumber" 
ON org."SecuritiesDistribution" ("BankReceiptNumber");

CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_TransactionGuideName" 
ON org."SecuritiesDistribution" ("TransactionGuideName");

-- Verify the table structure
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_schema = 'org' AND table_name = 'SecuritiesDistribution'
ORDER BY ordinal_position;
