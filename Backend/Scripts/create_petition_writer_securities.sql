-- Create PetitionWriterSecurities table in org schema
-- Run this script to create the table for Petition Writer Securities

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- Create the table
CREATE TABLE IF NOT EXISTS org."PetitionWriterSecurities" (
    "Id" SERIAL PRIMARY KEY,
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "PetitionWriterName" VARCHAR(200) NOT NULL,
    "PetitionWriterFatherName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    "PetitionCount" INTEGER NOT NULL,
    "Amount" DECIMAL(18, 2) NOT NULL,
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

-- Create unique index on RegistrationNumber
CREATE UNIQUE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_RegistrationNumber" 
ON org."PetitionWriterSecurities" ("RegistrationNumber");

-- Create index on LicenseNumber for search
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_LicenseNumber" 
ON org."PetitionWriterSecurities" ("LicenseNumber");

-- Create index on BankReceiptNumber for search
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_BankReceiptNumber" 
ON org."PetitionWriterSecurities" ("BankReceiptNumber");

-- Add constraint name for primary key (optional, matches EF migration)
-- ALTER TABLE org."PetitionWriterSecurities" 
-- DROP CONSTRAINT IF EXISTS "PetitionWriterSecurities_pkey",
-- ADD CONSTRAINT "PetitionWriterSecurities_pkey" PRIMARY KEY ("Id");

SELECT 'PetitionWriterSecurities table created successfully!' as result;
