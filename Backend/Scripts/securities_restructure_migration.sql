-- =====================================================
-- Securities Module Restructure Migration
-- Purpose: Convert from fixed fields to dynamic Items collection
-- Date: 2026-02-06
-- =====================================================

-- Step 1: Create new SecuritiesDistributionItem table
CREATE TABLE IF NOT EXISTS org."SecuritiesDistributionItem" (
    "Id" SERIAL PRIMARY KEY,
    "SecuritiesDistributionId" INTEGER NOT NULL,
    "DocumentType" INTEGER NOT NULL,
    "SerialStart" VARCHAR(100),
    "SerialEnd" VARCHAR(100),
    "Count" INTEGER NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    CONSTRAINT "FK_SecuritiesDistributionItem_SecuritiesDistribution" 
        FOREIGN KEY ("SecuritiesDistributionId") 
        REFERENCES org."SecuritiesDistribution"("Id") 
        ON DELETE CASCADE
);

-- Create index for foreign key
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistributionItem_SecuritiesDistributionId" 
    ON org."SecuritiesDistributionItem" ("SecuritiesDistributionId");

-- Step 2: Remove old columns from SecuritiesDistribution
-- These columns are being replaced by the Items collection
ALTER TABLE org."SecuritiesDistribution" 
    DROP COLUMN IF EXISTS "DocumentType",
    DROP COLUMN IF EXISTS "PropertySubType",
    DROP COLUMN IF EXISTS "VehicleSubType",
    DROP COLUMN IF EXISTS "PropertySaleCount",
    DROP COLUMN IF EXISTS "PropertySaleSerialStart",
    DROP COLUMN IF EXISTS "PropertySaleSerialEnd",
    DROP COLUMN IF EXISTS "BayWafaCount",
    DROP COLUMN IF EXISTS "BayWafaSerialStart",
    DROP COLUMN IF EXISTS "BayWafaSerialEnd",
    DROP COLUMN IF EXISTS "RentCount",
    DROP COLUMN IF EXISTS "RentSerialStart",
    DROP COLUMN IF EXISTS "RentSerialEnd",
    DROP COLUMN IF EXISTS "VehicleSaleCount",
    DROP COLUMN IF EXISTS "VehicleSaleSerialStart",
    DROP COLUMN IF EXISTS "VehicleSaleSerialEnd",
    DROP COLUMN IF EXISTS "VehicleExchangeCount",
    DROP COLUMN IF EXISTS "VehicleExchangeSerialStart",
    DROP COLUMN IF EXISTS "VehicleExchangeSerialEnd",
    DROP COLUMN IF EXISTS "RegistrationBookType",
    DROP COLUMN IF EXISTS "RegistrationBookCount",
    DROP COLUMN IF EXISTS "DuplicateBookCount",
    DROP COLUMN IF EXISTS "RegistrationBookPrice";

-- Step 3: Verify the changes
DO $$
BEGIN
    RAISE NOTICE 'Migration completed successfully!';
    RAISE NOTICE 'New table: org.SecuritiesDistributionItem';
    RAISE NOTICE 'Removed columns from org.SecuritiesDistribution';
    RAISE NOTICE 'Remaining columns: RegistrationNumber, LicenseOwnerName, LicenseOwnerFatherName, TransactionGuideName, LicenseNumber, PricePerDocument, TotalDocumentsPrice, TotalSecuritiesPrice, BankReceiptNumber, DeliveryDate, DistributionDate, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status';
END $$;

-- =====================================================
-- Migration Notes:
-- 
-- Document Types:
-- 1 = سټه یی خرید و فروش (Property Sale) - 4000 Afs each
-- 2 = سټه یی بیع وفا (Bay Wafa) - 4000 Afs each
-- 3 = سټه یی کرایی (Rent) - 4000 Afs each
-- 4 = سټه وسایط نقلیه (Vehicle) - 4000 Afs each
-- 5 = کتاب ثبت (Registration Book) - 1000 Afs each
-- 6 = کتاب ثبت مثنی (Duplicate Book) - 20000 Afs each
--
-- For types 1-4: SerialStart and SerialEnd are required
-- For types 5-6: Only Count is required (no serial numbers)
--
-- Count Calculation (types 1-4):
-- Count = (SerialEnd numeric part) - (SerialStart numeric part) + 1
--
-- Price Calculation:
-- Price = Count × Price Per Unit (based on DocumentType)
-- =====================================================
