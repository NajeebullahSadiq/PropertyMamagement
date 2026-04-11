    -- Create sec schema if it doesn't exist
CREATE SCHEMA IF NOT EXISTS sec;

-- Create PetitionWriterSecurities table
CREATE TABLE IF NOT EXISTS sec."PetitionWriterSecurities" (
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
    "DeliveryDate" DATE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    "Status" BOOLEAN DEFAULT TRUE
);

-- Create indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_RegistrationNumber" 
    ON sec."PetitionWriterSecurities" ("RegistrationNumber");

CREATE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_LicenseNumber" 
    ON sec."PetitionWriterSecurities" ("LicenseNumber");

CREATE INDEX IF NOT EXISTS "IX_PetitionWriterSecurities_BankReceiptNumber" 
    ON sec."PetitionWriterSecurities" ("BankReceiptNumber");

-- Add comment
COMMENT ON TABLE sec."PetitionWriterSecurities" IS 'سند بهادار عریضه‌ نویسان - Securities for Petition Writers';
