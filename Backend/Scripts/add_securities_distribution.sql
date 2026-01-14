-- Migration script for SecuritiesDistribution table
-- اسناد بهادار رهنمای معاملات

-- Create the table
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

-- Create unique index on RegistrationNumber
CREATE UNIQUE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_RegistrationNumber" 
ON org."SecuritiesDistribution" ("RegistrationNumber");

-- Create index for search on LicenseNumber
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_LicenseNumber" 
ON org."SecuritiesDistribution" ("LicenseNumber");

-- Create index for search on BankReceiptNumber
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_BankReceiptNumber" 
ON org."SecuritiesDistribution" ("BankReceiptNumber");

-- Create index for search on TransactionGuideName
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_TransactionGuideName" 
ON org."SecuritiesDistribution" ("TransactionGuideName");

-- Add comment to table
COMMENT ON TABLE org."SecuritiesDistribution" IS 'اسناد بهادار رهنمای معاملات - Securities Distribution for Transaction Guides';
