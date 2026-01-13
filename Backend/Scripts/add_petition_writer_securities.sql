-- Migration: Add PetitionWriterSecurities table
-- سند بهادار عریضه‌ نویسان - Securities for Petition Writers

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
    "Status" BOOLEAN DEFAULT TRUE
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

-- Add comment to table
COMMENT ON TABLE org."PetitionWriterSecurities" IS 'سند بهادار عریضه‌ نویسان - Securities for Petition Writers';
