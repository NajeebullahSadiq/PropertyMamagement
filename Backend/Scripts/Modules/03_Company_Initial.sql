-- =====================================================
-- Module: Company/Organization Tables
-- Schema: org
-- Dependencies: Shared (look schema)
-- Execution Order: 3
-- =====================================================

-- Create schema if not exists
CREATE SCHEMA IF NOT EXISTS org;

-- CompanyDetails
CREATE TABLE IF NOT EXISTS org."CompanyDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "PhoneNumber" VARCHAR(13),
    "LicenseNumber" TEXT,
    "PetitionDate" DATE,
    "PetitionNumber" VARCHAR(12),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN,
    "DocPath" TEXT,
    "TIN" DOUBLE PRECISION
);

-- CompanyOwner
CREATE TABLE IF NOT EXISTS org."CompanyOwner" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "GrandFatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" VARCHAR(20),
    "WhatsAppNumber" VARCHAR(20),
    "Photo" TEXT,
    "CompanyId" INTEGER REFERENCES org."CompanyDetails"("Id"),
    "EducationLevelId" SMALLINT REFERENCES look."EducationLevel"("ID"),
    "IdentityCardTypeId" INTEGER REFERENCES look."IdentityCardType"("Id"),
    "OwnerProvinceId" INTEGER,
    "OwnerDistrictId" INTEGER,
    "OwnerVillage" TEXT,
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" TEXT,
    "TemporaryProvinceId" INTEGER,
    "TemporaryDistrictId" INTEGER,
    "TemporaryVillage" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- CompanyOwnerAddress
CREATE TABLE IF NOT EXISTS org."CompanyOwnerAddress" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER REFERENCES org."CompanyOwner"("Id"),
    "AddressTypeId" INTEGER REFERENCES look."AddressType"("Id"),
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- CompanyOwnerAddressHistory
CREATE TABLE IF NOT EXISTS org."CompanyOwnerAddressHistory" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER NOT NULL REFERENCES org."CompanyOwner"("Id"),
    "AddressType" VARCHAR(20) NOT NULL,
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" TEXT,
    "EffectiveFrom" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "EffectiveTo" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50)
);

-- LicenseDetails
CREATE TABLE IF NOT EXISTS org."LicenseDetails" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseNumber" TEXT,
    "LicenseType" TEXT,
    "LicenseCategory" TEXT,
    "LicenseDate" DATE,
    "LicenseExpireDate" DATE,
    "CompanyId" INTEGER REFERENCES org."CompanyDetails"("Id"),
    "AreaId" INTEGER REFERENCES look."Area"("Id"),
    "TaxPaymentAmount" DECIMAL(18,2),
    "CompanyCommission" DECIMAL(18,2),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Gaurantee
CREATE TABLE IF NOT EXISTS org."Gaurantee" (
    "Id" SERIAL PRIMARY KEY,
    "GuaranteeTypeId" INTEGER REFERENCES look."GuaranteeType"("Id"),
    "GuaranteeNumber" BIGINT,
    "GuaranteeDate" DATE,
    "CompanyId" INTEGER REFERENCES org."CompanyDetails"("Id"),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Guarantors
CREATE TABLE IF NOT EXISTS org."Guarantors" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "GrandFatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" VARCHAR(13),
    "Photo" TEXT,
    "CompanyId" INTEGER REFERENCES org."CompanyDetails"("Id"),
    "IdentityCardTypeId" INTEGER REFERENCES look."IdentityCardType"("Id"),
    "GuaranteeTypeId" INTEGER REFERENCES look."GuaranteeType"("Id"),
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" TEXT,
    "GuaranteeDistrictId" INTEGER,
    "CourtName" VARCHAR(255),
    "CollateralNumber" VARCHAR(100),
    "SetSerialNumber" VARCHAR(100),
    "BankName" VARCHAR(255),
    "DepositNumber" VARCHAR(100),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Haqulemtyaz
CREATE TABLE IF NOT EXISTS org."Haqulemtyaz" (
    "Id" SERIAL PRIMARY KEY,
    "HaqulemtyazNumber" INTEGER,
    "HaqulemtyazDate" DATE,
    "CompanyId" INTEGER REFERENCES org."CompanyDetails"("Id"),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- PeriodicForm
CREATE TABLE IF NOT EXISTS org."PeriodicForm" (
    "Id" SERIAL PRIMARY KEY,
    "ReferenceId" INTEGER REFERENCES look."FormsReference"("Id"),
    "FormNumber" INTEGER,
    "FormDate" DATE,
    "CompanyId" INTEGER REFERENCES org."CompanyDetails"("Id"),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Seta
CREATE TABLE IF NOT EXISTS org."Seta" (
    "Id" SERIAL PRIMARY KEY,
    "TransactionTypeId" INTEGER,
    "InquiryNumber" INTEGER,
    "InquiryDate" DATE,
    "SetaSerialNumber" INTEGER,
    "SetaStampedDate" DATE,
    "CompanyId" INTEGER,
    "DocPath" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- CompanyAccountInfo
CREATE TABLE IF NOT EXISTS org."CompanyAccountInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL REFERENCES org."CompanyDetails"("Id"),
    "SettlementInfo" VARCHAR(500),
    "TaxPaymentAmount" DECIMAL(18,2),
    "CompanyCommission" DECIMAL(18,2),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- CompanyCancellationInfo
CREATE TABLE IF NOT EXISTS org."CompanyCancellationInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL REFERENCES org."CompanyDetails"("Id"),
    "LicenseCancellationLetterNumber" VARCHAR(100),
    "LicenseCancellationDate" DATE,
    "RevenueCancellationLetterNumber" VARCHAR(100),
    "RevenueCancellationDate" DATE,
    "Remarks" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_CompanyOwner_CompanyId" ON org."CompanyOwner" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_CompanyOwner_EducationLevelId" ON org."CompanyOwner" ("EducationLevelId");
CREATE INDEX IF NOT EXISTS "IX_CompanyOwnerAddress_CompanyOwnerId" ON org."CompanyOwnerAddress" ("CompanyOwnerId");
CREATE INDEX IF NOT EXISTS "IX_LicenseDetails_CompanyId" ON org."LicenseDetails" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_LicenseDetails_AreaId" ON org."LicenseDetails" ("AreaId");
CREATE INDEX IF NOT EXISTS "IX_Gaurantee_CompanyId" ON org."Gaurantee" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_Guarantors_CompanyId" ON org."Guarantors" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_Haqulemtyaz_CompanyId" ON org."Haqulemtyaz" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_PeriodicForm_CompanyId" ON org."PeriodicForm" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_CompanyAccountInfo_CompanyId" ON org."CompanyAccountInfo" ("CompanyId");
CREATE INDEX IF NOT EXISTS "IX_CompanyCancellationInfo_CompanyId" ON org."CompanyCancellationInfo" ("CompanyId");

-- =====================================================
-- End of Company Module
-- =====================================================
