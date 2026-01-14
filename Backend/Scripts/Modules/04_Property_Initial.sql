-- =====================================================
-- Module: Property Transaction Tables
-- Schema: tr
-- Dependencies: Shared (look schema), Company (org schema)
-- Execution Order: 4
-- =====================================================

-- Create schema if not exists
CREATE SCHEMA IF NOT EXISTS tr;

-- PropertyDetails
CREATE TABLE IF NOT EXISTS tr."PropertyDetails" (
    "Id" SERIAL PRIMARY KEY,
    "TransactionTypeId" INTEGER,
    "PropertyTypeId" INTEGER,
    "DocumentType" TEXT,
    "IssuanceNumber" TEXT,
    "IssuanceDate" TIMESTAMP WITH TIME ZONE,
    "SerialNumber" TEXT,
    "TransactionDate" TIMESTAMP WITH TIME ZONE,
    "DeedDate" DATE,
    "PrivateNumber" TEXT,
    "east" TEXT,
    "West" TEXT,
    "North" TEXT,
    "South" TEXT,
    "Area" DECIMAL,
    "PunitTypeId" INTEGER,
    "Price" DECIMAL,
    "CompanyId" INTEGER,
    "iscomplete" BOOLEAN DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- PropertyAddress
CREATE TABLE IF NOT EXISTS tr."PropertyAddress" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER REFERENCES tr."PropertyDetails"("Id"),
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- BuyerDetails
CREATE TABLE IF NOT EXISTS tr."BuyerDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "GrandFatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" VARCHAR(14),
    "photo" TEXT,
    "PropertyDetailsId" INTEGER REFERENCES tr."PropertyDetails"("Id"),
    "IdentityCardTypeId" INTEGER,
    "RoleType" TEXT,
    "TransactionType" TEXT,
    "RentStartDate" DATE,
    "RentEndDate" DATE,
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" TEXT,
    "TaxIdentificationNumber" TEXT,
    "AdditionalDetails" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- SellerDetails
CREATE TABLE IF NOT EXISTS tr."SellerDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "GrandFatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" VARCHAR(14),
    "Photo" TEXT,
    "PropertyDetailsId" INTEGER REFERENCES tr."PropertyDetails"("Id"),
    "IdentityCardTypeId" INTEGER,
    "RoleType" TEXT,
    "AuthorizationLetterNumber" TEXT,
    "AuthorizationLetterDate" DATE,
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" TEXT,
    "TaxIdentificationNumber" TEXT,
    "AdditionalDetails" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- WitnessDetails
CREATE TABLE IF NOT EXISTS tr."WitnessDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" TEXT,
    "PropertyDetailsId" INTEGER REFERENCES tr."PropertyDetails"("Id"),
    "IdentityCardTypeId" INTEGER,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- PropertyCancellations
CREATE TABLE IF NOT EXISTS tr."PropertyCancellations" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER NOT NULL REFERENCES tr."PropertyDetails"("Id"),
    "CancellationDate" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "CancellationReason" TEXT,
    "CancelledBy" VARCHAR(50),
    "Status" VARCHAR(50),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- PropertyCancellationDocuments
CREATE TABLE IF NOT EXISTS tr."PropertyCancellationDocuments" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyCancellationId" INTEGER NOT NULL REFERENCES tr."PropertyCancellations"("Id"),
    "DocumentPath" TEXT,
    "DocumentName" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50)
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_PropertyAddress_PropertyDetailsId" ON tr."PropertyAddress" ("PropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_BuyerDetails_PropertyDetailsId" ON tr."BuyerDetails" ("PropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_SellerDetails_PropertyDetailsId" ON tr."SellerDetails" ("PropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_WitnessDetails_PropertyDetailsId" ON tr."WitnessDetails" ("PropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_PropertyCancellations_PropertyDetailsId" ON tr."PropertyCancellations" ("PropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_PropertyCancellationDocuments_PropertyCancellationId" ON tr."PropertyCancellationDocuments" ("PropertyCancellationId");

-- =====================================================
-- End of Property Module
-- =====================================================
