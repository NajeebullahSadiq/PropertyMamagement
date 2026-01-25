-- =====================================================
-- Module: Audit/Log Tables
-- Schema: log
-- Dependencies: All other modules
-- Execution Order: 7
-- =====================================================

-- Create schema if not exists
CREATE SCHEMA IF NOT EXISTS log;

-- Propertyaudit
CREATE TABLE IF NOT EXISTS log."Propertyaudit" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Propertybuyeraudit
CREATE TABLE IF NOT EXISTS log."Propertybuyeraudit" (
    "Id" SERIAL PRIMARY KEY,
    "BuyerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Propertyselleraudit
CREATE TABLE IF NOT EXISTS log."Propertyselleraudit" (
    "Id" SERIAL PRIMARY KEY,
    "SellerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Vehicleaudit
CREATE TABLE IF NOT EXISTS log."Vehicleaudit" (
    "Id" SERIAL PRIMARY KEY,
    "VehiclesPropertyDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Vehicleselleraudit
CREATE TABLE IF NOT EXISTS log."Vehicleselleraudit" (
    "Id" SERIAL PRIMARY KEY,
    "VehiclesSellerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Vehiclebuyeraudit
CREATE TABLE IF NOT EXISTS log."Vehiclebuyeraudit" (
    "Id" SERIAL PRIMARY KEY,
    "VehiclesBuyerDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Licenseaudit
CREATE TABLE IF NOT EXISTS log."licenseaudit" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseId" INTEGER NOT NULL,
    "PropertyName" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" TEXT,
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    CONSTRAINT "FK_licenseaudit_LicenseDetails" FOREIGN KEY ("LicenseId") 
        REFERENCES company."LicenseDetails"("Id") ON DELETE CASCADE
);

-- Guarantorsaudit
CREATE TABLE IF NOT EXISTS log."Guarantorsaudit" (
    "Id" SERIAL PRIMARY KEY,
    "GuarantorsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Graunteeaudit
CREATE TABLE IF NOT EXISTS log."Graunteeaudit" (
    "Id" SERIAL PRIMARY KEY,
    "GauranteeId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Companyowneraudit
CREATE TABLE IF NOT EXISTS log."Companyowneraudit" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- Companydetailsaudit
CREATE TABLE IF NOT EXISTS log."Companydetailsaudit" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyDetailsId" INTEGER,
    "Action" TEXT,
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" TEXT,
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE
);

-- =====================================================
-- End of Audit Module
-- =====================================================
