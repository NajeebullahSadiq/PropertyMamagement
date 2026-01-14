-- Create SecuritiesControl table in org schema
-- Run this script if the migration hasn't been applied

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- Create SecuritiesControl table
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

-- Create index on SerialNumber for faster lookups
CREATE INDEX IF NOT EXISTS idx_securities_control_serial ON org."SecuritiesControl" ("SerialNumber");
