-- Clean recreation of CompanyOwner tables with SINGULAR names
-- This will be used for production migration as well

-- Step 1: Drop all existing CompanyOwner related tables
DROP TABLE IF EXISTS log."Companyowneraudit" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistories" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddresses" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwners" CASCADE;

-- Step 2: Create CompanyOwner table (SINGULAR)
CREATE TABLE org."CompanyOwner" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(200) NOT NULL,
    "FatherName" VARCHAR(200) NOT NULL,
    "GrandFatherName" VARCHAR(200),
    "EducationLevelId" SMALLINT,
    "DateofBirth" DATE,
    "ElectronicNationalIdNumber" VARCHAR(20),
    "PhoneNumber" VARCHAR(20),
    "WhatsAppNumber" VARCHAR(20),
    "CompanyId" INTEGER,
    "PothoPath" VARCHAR(500),
    "OwnerProvinceId" INTEGER,
    "OwnerDistrictId" INTEGER,
    "OwnerVillage" VARCHAR(500),
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" VARCHAR(500),
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "FK_CompanyOwner_Company" FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id"),
    CONSTRAINT "FK_CompanyOwner_EducationLevel" FOREIGN KEY ("EducationLevelId") 
        REFERENCES look."EducationLevel"("ID"),
    CONSTRAINT "FK_CompanyOwner_OwnerProvince" FOREIGN KEY ("OwnerProvinceId") 
        REFERENCES look."Location"("ID"),
    CONSTRAINT "FK_CompanyOwner_OwnerDistrict" FOREIGN KEY ("OwnerDistrictId") 
        REFERENCES look."Location"("ID"),
    CONSTRAINT "FK_CompanyOwner_PermanentProvince" FOREIGN KEY ("PermanentProvinceId") 
        REFERENCES look."Location"("ID"),
    CONSTRAINT "FK_CompanyOwner_PermanentDistrict" FOREIGN KEY ("PermanentDistrictId") 
        REFERENCES look."Location"("ID")
);

-- Step 3: Create CompanyOwnerAddress table (SINGULAR)
CREATE TABLE org."CompanyOwnerAddress" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER NOT NULL,
    "AddressTypeId" INTEGER,
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" VARCHAR(500),
    "CreatedAt" TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "FK_CompanyOwnerAddress_CompanyOwner" FOREIGN KEY ("CompanyOwnerId") 
        REFERENCES org."CompanyOwner"("Id"),
    CONSTRAINT "FK_CompanyOwnerAddress_AddressType" FOREIGN KEY ("AddressTypeId") 
        REFERENCES look."AddressType"("Id"),
    CONSTRAINT "FK_CompanyOwnerAddress_Province" FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID"),
    CONSTRAINT "FK_CompanyOwnerAddress_District" FOREIGN KEY ("DistrictId") 
        REFERENCES look."Location"("ID")
);

-- Step 4: Create CompanyOwnerAddressHistory table (SINGULAR)
CREATE TABLE org."CompanyOwnerAddressHistory" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER NOT NULL,
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" VARCHAR(500),
    "AddressType" VARCHAR(20),
    "EffectiveFrom" TIMESTAMP NOT NULL,
    "EffectiveTo" TIMESTAMP,
    "IsActive" BOOLEAN DEFAULT false,
    "CreatedAt" TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "FK_CompanyOwnerAddressHistory_CompanyOwner" FOREIGN KEY ("CompanyOwnerId") 
        REFERENCES org."CompanyOwner"("Id"),
    CONSTRAINT "FK_CompanyOwnerAddressHistory_Province" FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID"),
    CONSTRAINT "FK_CompanyOwnerAddressHistory_District" FOREIGN KEY ("DistrictId") 
        REFERENCES look."Location"("ID")
);

-- Step 5: Create audit table
CREATE TABLE log."Companyowneraudit" (
    "Id" SERIAL PRIMARY KEY,
    "OwnerId" INTEGER NOT NULL,
    "PropertyName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedAt" TIMESTAMP,
    "UpdatedBy" VARCHAR(50),
    CONSTRAINT "FK_Companyowneraudit_CompanyOwner" FOREIGN KEY ("OwnerId") 
        REFERENCES org."CompanyOwner"("Id")
);

-- Step 6: Create indexes
CREATE INDEX "IX_CompanyOwner_CompanyId" ON org."CompanyOwner"("CompanyId");
CREATE INDEX "IX_CompanyOwnerAddress_CompanyOwnerId" ON org."CompanyOwnerAddress"("CompanyOwnerId");
CREATE INDEX "IX_CompanyOwnerAddressHistory_CompanyOwnerId" ON org."CompanyOwnerAddressHistory"("CompanyOwnerId");

-- Verify tables created
SELECT 'Tables created successfully' as status;
SELECT tablename FROM pg_tables WHERE schemaname = 'org' AND tablename LIKE '%ompanyOwner%' ORDER BY tablename;
