-- =====================================================
-- Module: Securities Tables
-- Schema: org
-- Dependencies: Company (org schema)
-- Execution Order: 6
-- =====================================================

-- SecuritiesDistribution - اسناد بهادار رهنمای معاملات
CREATE TABLE IF NOT EXISTS org."SecuritiesDistribution" (
    "Id" SERIAL PRIMARY KEY,
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "LicenseOwnerName" VARCHAR(200) NOT NULL,
    "LicenseOwnerFatherName" VARCHAR(200) NOT NULL,
    "TransactionGuideName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    "DocumentType" INTEGER,
    "PropertySubType" INTEGER,
    "VehicleSubType" INTEGER,
    "PropertySaleCount" INTEGER,
    "PropertySaleSerialStart" VARCHAR(100),
    "PropertySaleSerialEnd" VARCHAR(100),
    "BayWafaCount" INTEGER,
    "BayWafaSerialStart" VARCHAR(100),
    "BayWafaSerialEnd" VARCHAR(100),
    "RentCount" INTEGER,
    "RentSerialStart" VARCHAR(100),
    "RentSerialEnd" VARCHAR(100),
    "VehicleSaleCount" INTEGER,
    "VehicleSaleSerialStart" VARCHAR(100),
    "VehicleSaleSerialEnd" VARCHAR(100),
    "VehicleExchangeCount" INTEGER,
    "VehicleExchangeSerialStart" VARCHAR(100),
    "VehicleExchangeSerialEnd" VARCHAR(100),
    "RegistrationBookType" INTEGER,
    "RegistrationBookCount" INTEGER,
    "DuplicateBookCount" INTEGER,
    "PricePerDocument" DECIMAL(18,2),
    "TotalDocumentsPrice" DECIMAL(18,2),
    "RegistrationBookPrice" DECIMAL(18,2),
    "TotalSecuritiesPrice" DECIMAL(18,2),
    "BankReceiptNumber" VARCHAR(100),
    "DeliveryDate" DATE,
    "DistributionDate" DATE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- PetitionWriterSecurities - اسناد بهادار عریضه نویسان
CREATE TABLE IF NOT EXISTS org."PetitionWriterSecurities" (
    "Id" SERIAL PRIMARY KEY,
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "PetitionWriterName" VARCHAR(200) NOT NULL,
    "PetitionWriterFatherName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    "PetitionCount" INTEGER NOT NULL,
    "Amount" DECIMAL(18,2) NOT NULL,
    "BankReceiptNumber" VARCHAR(100) NOT NULL,
    "SerialNumberStart" VARCHAR(100) NOT NULL,
    "SerialNumberEnd" VARCHAR(100) NOT NULL,
    "DistributionDate" DATE NOT NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- SecuritiesControl - کنترول اسناد بهادار
CREATE TABLE IF NOT EXISTS org."SecuritiesControl" (
    "Id" SERIAL PRIMARY KEY,
    "SerialNumber" VARCHAR(50) NOT NULL,
    "SecurityDocumentType" INTEGER NOT NULL,
    "ProposalNumber" VARCHAR(100),
    "ProposalDate" DATE,
    "DistributionTicketNumber" VARCHAR(100),
    "DeliveryDate" DATE,
    "SecuritiesType" INTEGER,
    "PropertySaleCount" INTEGER,
    "PropertySaleSerialStart" VARCHAR(100),
    "PropertySaleSerialEnd" VARCHAR(100),
    "BayWafaCount" INTEGER,
    "BayWafaSerialStart" VARCHAR(100),
    "BayWafaSerialEnd" VARCHAR(100),
    "RentCount" INTEGER,
    "RentSerialStart" VARCHAR(100),
    "RentSerialEnd" VARCHAR(100),
    "VehicleSaleCount" INTEGER,
    "VehicleSaleSerialStart" VARCHAR(100),
    "VehicleSaleSerialEnd" VARCHAR(100),
    "ExchangeCount" INTEGER,
    "ExchangeSerialStart" VARCHAR(100),
    "ExchangeSerialEnd" VARCHAR(100),
    "RegistrationBookCount" INTEGER,
    "RegistrationBookSerialStart" VARCHAR(100),
    "RegistrationBookSerialEnd" VARCHAR(100),
    "PrintedPetitionCount" INTEGER,
    "PrintedPetitionSerialStart" VARCHAR(100),
    "PrintedPetitionSerialEnd" VARCHAR(100),
    "DistributionStartNumber" VARCHAR(100),
    "DistributionEndNumber" VARCHAR(100),
    "DistributedPersonsCount" INTEGER,
    "Remarks" VARCHAR(2000),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Create indexes for SecuritiesDistribution
CREATE UNIQUE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_RegistrationNumber" 
    ON org."SecuritiesDistribution" ("RegistrationNumber");
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_LicenseNumber" 
    ON org."SecuritiesDistribution" ("LicenseNumber");
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_BankReceiptNumber" 
    ON org."SecuritiesDistribution" ("BankReceiptNumber");
CREATE INDEX IF NOT EXISTS "IX_SecuritiesDistribution_TransactionGuideName" 
    ON org."SecuritiesDistribution" ("TransactionGuideName");

-- Create indexes for PetitionWriterSecurities
CREATE UNIQUE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_RegistrationNumber" 
    ON org."PetitionWriterSecurities" ("RegistrationNumber");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_LicenseNumber" 
    ON org."PetitionWriterSecurities" ("LicenseNumber");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_BankReceiptNumber" 
    ON org."PetitionWriterSecurities" ("BankReceiptNumber");

-- Create index for SecuritiesControl
CREATE UNIQUE INDEX IF NOT EXISTS "IX_SecuritiesControl_SerialNumber" 
    ON org."SecuritiesControl" ("SerialNumber");

-- =====================================================
-- End of Securities Module
-- =====================================================
