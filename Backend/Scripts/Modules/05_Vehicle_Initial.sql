-- =====================================================
-- Module: Vehicle Transaction Tables
-- Schema: tr
-- Dependencies: Shared (look schema), Company (org schema)
-- Execution Order: 5
-- =====================================================

-- VehiclesPropertyDetails
CREATE TABLE IF NOT EXISTS tr."VehiclesPropertyDetails" (
    "Id" SERIAL PRIMARY KEY,
    "TransactionTypeId" INTEGER,
    "VehicleType" TEXT,
    "VehicleBrand" TEXT,
    "VehicleModel" TEXT,
    "VehicleColor" TEXT,
    "VehicleYear" INTEGER,
    "PlateNumber" TEXT,
    "ChassisNumber" TEXT,
    "EngineNumber" TEXT,
    "LicenseNumber" TEXT,
    "Hand" TEXT,
    "SerialNumber" TEXT,
    "TransactionDate" TIMESTAMP WITH TIME ZONE,
    "Price" DECIMAL,
    "CompanyId" INTEGER,
    "iscomplete" BOOLEAN DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- VehiclesBuyerDetails
CREATE TABLE IF NOT EXISTS tr."VehiclesBuyerDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "GrandFatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" TEXT,
    "Photo" TEXT,
    "VehiclesPropertyDetailsId" INTEGER REFERENCES tr."VehiclesPropertyDetails"("Id"),
    "IdentityCardTypeId" INTEGER,
    "RoleType" TEXT,
    "RentStartDate" DATE,
    "RentEndDate" DATE,
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- VehiclesSellerDetails
CREATE TABLE IF NOT EXISTS tr."VehiclesSellerDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "GrandFatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" TEXT,
    "Photo" TEXT,
    "VehiclesPropertyDetailsId" INTEGER REFERENCES tr."VehiclesPropertyDetails"("Id"),
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
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- VehiclesWitnessDetails
CREATE TABLE IF NOT EXISTS tr."VehiclesWitnessDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "FatherName" TEXT,
    "TazkiraNumber" TEXT,
    "TazkiraPage" TEXT,
    "TazkiraVolume" TEXT,
    "TazkiraRegNumber" TEXT,
    "NationalIdNumber" TEXT,
    "PhoneNumber" TEXT,
    "VehiclesPropertyDetailsId" INTEGER REFERENCES tr."VehiclesPropertyDetails"("Id"),
    "IdentityCardTypeId" INTEGER,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "Status" BOOLEAN
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_VehiclesBuyerDetails_VehiclesPropertyDetailsId" ON tr."VehiclesBuyerDetails" ("VehiclesPropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_VehiclesSellerDetails_VehiclesPropertyDetailsId" ON tr."VehiclesSellerDetails" ("VehiclesPropertyDetailsId");
CREATE INDEX IF NOT EXISTS "IX_VehiclesWitnessDetails_VehiclesPropertyDetailsId" ON tr."VehiclesWitnessDetails" ("VehiclesPropertyDetailsId");

-- =====================================================
-- End of Vehicle Module
-- =====================================================
