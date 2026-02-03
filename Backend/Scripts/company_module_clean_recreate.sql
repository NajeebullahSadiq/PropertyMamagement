-- =====================================================
-- Company Module Clean Recreation Script
-- =====================================================
-- Purpose: Drop and recreate all company/license module tables
-- Schema: org (organization), log (audit)
-- Date: 2026-02-01
-- 
-- WARNING: THIS SCRIPT WILL DELETE ALL COMPANY DATA!
-- =====================================================

-- =====================================================
-- STEP 1: DROP ALL EXISTING TABLES
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE 'Starting company module table cleanup...';
END $$;

-- Drop tables in correct order (children first, then parents)
DROP TABLE IF EXISTS log."Licenseaudit" CASCADE;
DROP TABLE IF EXISTS log."Graunteeaudit" CASCADE;
DROP TABLE IF EXISTS log."Guarantorsaudit" CASCADE;
DROP TABLE IF EXISTS log."Companyowneraudit" CASCADE;
DROP TABLE IF EXISTS log."Companydetailsaudit" CASCADE;

DROP TABLE IF EXISTS org."PeriodicForms" CASCADE;
DROP TABLE IF EXISTS org."Haqulemtyaz" CASCADE;
DROP TABLE IF EXISTS org."CompanyCancellationInfo" CASCADE;
DROP TABLE IF EXISTS org."CompanyAccountInfo" CASCADE;
DROP TABLE IF EXISTS org."LicenseDetails" CASCADE;
DROP TABLE IF EXISTS org."Gaurantees" CASCADE;
DROP TABLE IF EXISTS org."Guarantors" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;
DROP TABLE IF EXISTS org."CompanyDetails" CASCADE;

DO $$ 
BEGIN
    RAISE NOTICE '✓ All company module tables dropped successfully';
END $$;

-- =====================================================
-- STEP 2: ENSURE SCHEMAS EXIST
-- =====================================================

CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS log;
CREATE SCHEMA IF NOT EXISTS look;

-- =====================================================
-- STEP 3: CREATE TRANSACTION TABLES (org schema)
-- =====================================================

-- 1. CompanyDetails (Main company/license holder table)
CREATE TABLE org."CompanyDetails" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(500) NOT NULL,
    "TIN" VARCHAR(50),
    "ProvinceId" INTEGER,
    "DocPath" VARCHAR(500),
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_CompanyDetails_Province" 
        FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL
);

-- 2. CompanyOwner (Company owner/partner information)
CREATE TABLE org."CompanyOwner" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(200) NOT NULL,
    "FatherName" VARCHAR(200) NOT NULL,
    "GrandFatherName" VARCHAR(200),
    "EducationLevelId" SMALLINT,
    "DateofBirth" DATE,
    "ElectronicNationalIdNumber" VARCHAR(50),
    "PhoneNumber" VARCHAR(20),
    "WhatsAppNumber" VARCHAR(20),
    "CompanyId" INTEGER,
    "PothoPath" VARCHAR(500),
    
    -- Owner's Own Address (آدرس اصلی مالک)
    "OwnerProvinceId" INTEGER,
    "OwnerDistrictId" INTEGER,
    "OwnerVillage" VARCHAR(500),
    
    -- Permanent Address (آدرس دایمی)
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" VARCHAR(500),
    
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_CompanyOwner_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyOwner_EducationLevel" 
        FOREIGN KEY ("EducationLevelId") 
        REFERENCES look."EducationLevel"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwner_OwnerProvince" 
        FOREIGN KEY ("OwnerProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwner_OwnerDistrict" 
        FOREIGN KEY ("OwnerDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwner_PermanentProvince" 
        FOREIGN KEY ("PermanentProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwner_PermanentDistrict" 
        FOREIGN KEY ("PermanentDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL
);

-- 3. CompanyOwnerAddress (Legacy address table - kept for compatibility)
CREATE TABLE org."CompanyOwnerAddress" (
    "Id" SERIAL PRIMARY KEY,
    "AddressTypeId" INTEGER,
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" VARCHAR(500),
    "CompanyOwnerId" INTEGER,
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_CompanyOwnerAddress_CompanyOwner" 
        FOREIGN KEY ("CompanyOwnerId") 
        REFERENCES org."CompanyOwner"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyOwnerAddress_AddressType" 
        FOREIGN KEY ("AddressTypeId") 
        REFERENCES look."AddressType"("Id") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwnerAddress_Province" 
        FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwnerAddress_District" 
        FOREIGN KEY ("DistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL
);

-- 4. CompanyOwnerAddressHistory (Address change history)
CREATE TABLE org."CompanyOwnerAddressHistory" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyOwnerId" INTEGER NOT NULL,
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" VARCHAR(500),
    "AddressType" VARCHAR(50) DEFAULT 'Permanent',
    "EffectiveFrom" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "EffectiveTo" TIMESTAMP WITHOUT TIME ZONE,
    "IsActive" BOOLEAN DEFAULT false,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_CompanyOwnerAddressHistory_CompanyOwner" 
        FOREIGN KEY ("CompanyOwnerId") 
        REFERENCES org."CompanyOwner"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyOwnerAddressHistory_Province" 
        FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_CompanyOwnerAddressHistory_District" 
        FOREIGN KEY ("DistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL
);

-- 5. Guarantors (Guarantor/Collateral provider information)
CREATE TABLE org."Guarantors" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(200) NOT NULL,
    "FatherName" VARCHAR(200) NOT NULL,
    "GrandFatherName" VARCHAR(200),
    "CompanyId" INTEGER,
    "ElectronicNationalIdNumber" VARCHAR(50),
    "PhoneNumber" VARCHAR(20),
    
    -- Permanent Address
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" VARCHAR(500),
    
    -- Temporary Address
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" VARCHAR(500),
    
    -- Guarantee Information
    "GuaranteeTypeId" INTEGER,
    "PropertyDocumentNumber" BIGINT,
    "PropertyDocumentDate" DATE,
    "SenderMaktobNumber" VARCHAR(100),
    "SenderMaktobDate" DATE,
    "AnswerdMaktobNumber" BIGINT,
    "AnswerdMaktobDate" DATE,
    "DateofGuarantee" DATE,
    "GuaranteeDocNumber" BIGINT,
    "GuaranteeDate" DATE,
    "GuaranteeDocPath" VARCHAR(500),
    
    -- Conditional fields for Sharia Deed (قباله شرعی)
    "CourtName" VARCHAR(200),
    "CollateralNumber" VARCHAR(100),
    
    -- Conditional fields for Customary Deed (قباله عرفی)
    "SetSerialNumber" VARCHAR(100),
    "GuaranteeDistrictId" INTEGER,
    
    -- Conditional fields for Cash (پول نقد)
    "BankName" VARCHAR(200),
    "DepositNumber" VARCHAR(100),
    "DepositDate" DATE,
    
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_Guarantors_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_Guarantors_GuaranteeType" 
        FOREIGN KEY ("GuaranteeTypeId") 
        REFERENCES look."GuaranteeType"("Id") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_Guarantors_PaddressProvince" 
        FOREIGN KEY ("PaddressProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_Guarantors_PaddressDistrict" 
        FOREIGN KEY ("PaddressDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_Guarantors_TaddressProvince" 
        FOREIGN KEY ("TaddressProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_Guarantors_TaddressDistrict" 
        FOREIGN KEY ("TaddressDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_Guarantors_GuaranteeDistrict" 
        FOREIGN KEY ("GuaranteeDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL
);

-- 6. Gaurantees (Legacy guarantee table - kept for compatibility)
CREATE TABLE org."Gaurantees" (
    "Id" SERIAL PRIMARY KEY,
    "GuaranteeTypeId" INTEGER,
    "PropertyDocumentNumber" BIGINT,
    "PropertyDocumentDate" DATE,
    "SenderMaktobNumber" VARCHAR(100),
    "SenderMaktobDate" DATE,
    "AnswerdMaktobNumber" BIGINT,
    "AnswerdMaktobDate" DATE,
    "DateofGuarantee" DATE,
    "GuaranteeDocNumber" BIGINT,
    "GuaranteeDate" DATE,
    "CompanyId" INTEGER,
    "DocPath" VARCHAR(500),
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_Gaurantees_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_Gaurantees_GuaranteeType" 
        FOREIGN KEY ("GuaranteeTypeId") 
        REFERENCES look."GuaranteeType"("Id") 
        ON DELETE SET NULL
);

-- 7. LicenseDetails (License/permit information)
CREATE TABLE org."LicenseDetails" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseNumber" VARCHAR(50),
    "ProvinceId" INTEGER,
    "IssueDate" DATE,
    "ExpireDate" DATE,
    "AreaId" INTEGER,
    "OfficeAddress" VARCHAR(500),
    "CompanyId" INTEGER,
    "DocPath" VARCHAR(500),
    "LicenseType" VARCHAR(100),
    "LicenseCategory" VARCHAR(50),
    "RenewalRound" INTEGER,
    "RoyaltyAmount" DECIMAL(18, 2),
    "RoyaltyDate" DATE,
    "TariffNumber" VARCHAR(100),
    "PenaltyAmount" DECIMAL(18, 2),
    "PenaltyDate" DATE,
    "HrLetter" VARCHAR(100),
    "HrLetterDate" DATE,
    "IsComplete" BOOLEAN DEFAULT false,
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_LicenseDetails_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_LicenseDetails_Province" 
        FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_LicenseDetails_Area" 
        FOREIGN KEY ("AreaId") 
        REFERENCES look."Area"("Id") 
        ON DELETE SET NULL
);

-- 8. CompanyAccountInfo (Financial/tax information)
CREATE TABLE org."CompanyAccountInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL,
    "SettlementInfo" VARCHAR(500),
    "TaxPaymentAmount" DECIMAL(18, 2) NOT NULL DEFAULT 0,
    "SettlementYear" INTEGER,
    "TaxPaymentDate" DATE,
    "TransactionCount" INTEGER,
    "CompanyCommission" DECIMAL(18, 2),
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_CompanyAccountInfo_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE
);

-- 9. CompanyCancellationInfo (License cancellation information)
CREATE TABLE org."CompanyCancellationInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL,
    "LicenseCancellationLetterNumber" VARCHAR(100),
    "RevenueCancellationLetterNumber" VARCHAR(100),
    "LicenseCancellationLetterDate" DATE,
    "Remarks" VARCHAR(1000),
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_CompanyCancellationInfo_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE
);

-- 10. Haqulemtyaz (Royalty/privilege fee information)
CREATE TABLE org."Haqulemtyaz" (
    "Id" SERIAL PRIMARY KEY,
    "FormNumber" INTEGER,
    "FormDate" DATE,
    "SubmissionFormNumber" INTEGER,
    "SubmissionFormDate" DATE,
    "CompanyId" INTEGER,
    "DocPath" VARCHAR(500),
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_Haqulemtyaz_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE
);

-- 11. PeriodicForms (Periodic reporting forms)
CREATE TABLE org."PeriodicForms" (
    "Id" SERIAL PRIMARY KEY,
    "ReferenceId" INTEGER,
    "FormNumber" INTEGER,
    "FormDate" DATE,
    "SubmissionDate" DATE,
    "MaktobNumber" VARCHAR(100),
    "MaktobDate" DATE,
    "DiagnosisNumber" INTEGER,
    "Details" TEXT,
    "DocPath" VARCHAR(500),
    "CompanyId" INTEGER,
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_PeriodicForms_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_PeriodicForms_Reference" 
        FOREIGN KEY ("ReferenceId") 
        REFERENCES look."FormsReference"("Id") 
        ON DELETE SET NULL
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ All 11 company module tables created successfully';
END $$;

-- =====================================================
-- STEP 4: CREATE AUDIT TABLES (log schema)
-- =====================================================

-- 1. Companydetailsaudit
CREATE TABLE log."Companydetailsaudit" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL,
    "PropertyName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Companydetailsaudit_Company" 
        FOREIGN KEY ("CompanyId") 
        REFERENCES org."CompanyDetails"("Id") 
        ON DELETE CASCADE
);

-- 2. Companyowneraudit
CREATE TABLE log."Companyowneraudit" (
    "Id" SERIAL PRIMARY KEY,
    "OwnerId" INTEGER NOT NULL,
    "PropertyName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Companyowneraudit_Owner" 
        FOREIGN KEY ("OwnerId") 
        REFERENCES org."CompanyOwner"("Id") 
        ON DELETE CASCADE
);

-- 3. Guarantorsaudit
CREATE TABLE log."Guarantorsaudit" (
    "Id" SERIAL PRIMARY KEY,
    "GuarantorsId" INTEGER NOT NULL,
    "PropertyName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Guarantorsaudit_Guarantor" 
        FOREIGN KEY ("GuarantorsId") 
        REFERENCES org."Guarantors"("Id") 
        ON DELETE CASCADE
);

-- 4. Graunteeaudit
CREATE TABLE log."Graunteeaudit" (
    "Id" SERIAL PRIMARY KEY,
    "GauranteeId" INTEGER NOT NULL,
    "PropertyName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Graunteeaudit_Gaurantee" 
        FOREIGN KEY ("GauranteeId") 
        REFERENCES org."Gaurantees"("Id") 
        ON DELETE CASCADE
);

-- 5. Licenseaudit
CREATE TABLE log."Licenseaudit" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseId" INTEGER NOT NULL,
    "PropertyName" VARCHAR(100),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "UpdatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Licenseaudit_License" 
        FOREIGN KEY ("LicenseId") 
        REFERENCES org."LicenseDetails"("Id") 
        ON DELETE CASCADE
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ All 5 company audit tables created successfully';
END $$;

-- =====================================================
-- STEP 5: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- CompanyDetails indexes
CREATE INDEX "IX_CompanyDetails_ProvinceId" ON org."CompanyDetails"("ProvinceId");
CREATE INDEX "IX_CompanyDetails_Title" ON org."CompanyDetails"("Title");
CREATE INDEX "IX_CompanyDetails_TIN" ON org."CompanyDetails"("TIN");
CREATE INDEX "IX_CompanyDetails_Status" ON org."CompanyDetails"("Status");

-- CompanyOwner indexes
CREATE INDEX "IX_CompanyOwner_CompanyId" ON org."CompanyOwner"("CompanyId");
CREATE INDEX "IX_CompanyOwner_ElectronicNationalIdNumber" ON org."CompanyOwner"("ElectronicNationalIdNumber");
CREATE INDEX "IX_CompanyOwner_FirstName" ON org."CompanyOwner"("FirstName");

-- CompanyOwnerAddress indexes
CREATE INDEX "IX_CompanyOwnerAddress_CompanyOwnerId" ON org."CompanyOwnerAddress"("CompanyOwnerId");
CREATE INDEX "IX_CompanyOwnerAddress_ProvinceId" ON org."CompanyOwnerAddress"("ProvinceId");

-- CompanyOwnerAddressHistory indexes
CREATE INDEX "IX_CompanyOwnerAddressHistory_CompanyOwnerId" ON org."CompanyOwnerAddressHistory"("CompanyOwnerId");
CREATE INDEX "IX_CompanyOwnerAddressHistory_IsActive" ON org."CompanyOwnerAddressHistory"("IsActive");

-- Guarantors indexes
CREATE INDEX "IX_Guarantors_CompanyId" ON org."Guarantors"("CompanyId");
CREATE INDEX "IX_Guarantors_ElectronicNationalIdNumber" ON org."Guarantors"("ElectronicNationalIdNumber");
CREATE INDEX "IX_Guarantors_GuaranteeTypeId" ON org."Guarantors"("GuaranteeTypeId");

-- Gaurantees indexes
CREATE INDEX "IX_Gaurantees_CompanyId" ON org."Gaurantees"("CompanyId");
CREATE INDEX "IX_Gaurantees_GuaranteeTypeId" ON org."Gaurantees"("GuaranteeTypeId");

-- LicenseDetails indexes
CREATE INDEX "IX_LicenseDetails_CompanyId" ON org."LicenseDetails"("CompanyId");
CREATE INDEX "IX_LicenseDetails_LicenseNumber" ON org."LicenseDetails"("LicenseNumber");
CREATE INDEX "IX_LicenseDetails_ProvinceId" ON org."LicenseDetails"("ProvinceId");
CREATE INDEX "IX_LicenseDetails_LicenseType" ON org."LicenseDetails"("LicenseType");
CREATE INDEX "IX_LicenseDetails_LicenseCategory" ON org."LicenseDetails"("LicenseCategory");
CREATE INDEX "IX_LicenseDetails_IssueDate" ON org."LicenseDetails"("IssueDate");
CREATE INDEX "IX_LicenseDetails_ExpireDate" ON org."LicenseDetails"("ExpireDate");

-- CompanyAccountInfo indexes
CREATE INDEX "IX_CompanyAccountInfo_CompanyId" ON org."CompanyAccountInfo"("CompanyId");
CREATE INDEX "IX_CompanyAccountInfo_SettlementYear" ON org."CompanyAccountInfo"("SettlementYear");

-- CompanyCancellationInfo indexes
CREATE INDEX "IX_CompanyCancellationInfo_CompanyId" ON org."CompanyCancellationInfo"("CompanyId");

-- Haqulemtyaz indexes
CREATE INDEX "IX_Haqulemtyaz_CompanyId" ON org."Haqulemtyaz"("CompanyId");

-- PeriodicForms indexes
CREATE INDEX "IX_PeriodicForms_CompanyId" ON org."PeriodicForms"("CompanyId");
CREATE INDEX "IX_PeriodicForms_ReferenceId" ON org."PeriodicForms"("ReferenceId");

-- Audit table indexes
CREATE INDEX "IX_Companydetailsaudit_CompanyId" ON log."Companydetailsaudit"("CompanyId");
CREATE INDEX "IX_Companydetailsaudit_UpdatedAt" ON log."Companydetailsaudit"("UpdatedAt");
CREATE INDEX "IX_Companyowneraudit_OwnerId" ON log."Companyowneraudit"("OwnerId");
CREATE INDEX "IX_Companyowneraudit_UpdatedAt" ON log."Companyowneraudit"("UpdatedAt");
CREATE INDEX "IX_Guarantorsaudit_GuarantorsId" ON log."Guarantorsaudit"("GuarantorsId");
CREATE INDEX "IX_Guarantorsaudit_UpdatedAt" ON log."Guarantorsaudit"("UpdatedAt");
CREATE INDEX "IX_Graunteeaudit_GauranteeId" ON log."Graunteeaudit"("GauranteeId");
CREATE INDEX "IX_Graunteeaudit_UpdatedAt" ON log."Graunteeaudit"("UpdatedAt");
CREATE INDEX "IX_Licenseaudit_LicenseId" ON log."Licenseaudit"("LicenseId");
CREATE INDEX "IX_Licenseaudit_UpdatedAt" ON log."Licenseaudit"("UpdatedAt");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All performance indexes created successfully';
END $$;

-- =====================================================
-- STEP 6: VERIFICATION QUERIES
-- =====================================================

DO $$ 
DECLARE
    transaction_table_count INTEGER;
    audit_table_count INTEGER;
    index_count INTEGER;
BEGIN
    -- Count transaction tables
    SELECT COUNT(*) INTO transaction_table_count
    FROM information_schema.tables 
    WHERE table_schema = 'org' 
    AND table_name IN (
        'CompanyDetails', 'CompanyOwner', 'CompanyOwnerAddress', 
        'CompanyOwnerAddressHistory', 'Guarantors', 'Gaurantees', 
        'LicenseDetails', 'CompanyAccountInfo', 'CompanyCancellationInfo',
        'Haqulemtyaz', 'PeriodicForms'
    );
    
    -- Count audit tables
    SELECT COUNT(*) INTO audit_table_count
    FROM information_schema.tables 
    WHERE table_schema = 'log' 
    AND table_name IN (
        'Companydetailsaudit', 'Companyowneraudit', 'Guarantorsaudit',
        'Graunteeaudit', 'Licenseaudit'
    );
    
    -- Count indexes
    SELECT COUNT(*) INTO index_count
    FROM pg_indexes 
    WHERE schemaname IN ('org', 'log')
    AND (tablename LIKE '%Company%' OR tablename LIKE '%License%' 
         OR tablename LIKE '%Guarantor%' OR tablename LIKE '%Gaurantee%'
         OR tablename LIKE '%Haqulemtyaz%' OR tablename LIKE '%Periodic%');
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Company Module Recreation Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Tables Created:';
    RAISE NOTICE '  - org.CompanyDetails';
    RAISE NOTICE '  - org.CompanyOwner';
    RAISE NOTICE '  - org.CompanyOwnerAddress';
    RAISE NOTICE '  - org.CompanyOwnerAddressHistory';
    RAISE NOTICE '  - org.Guarantors';
    RAISE NOTICE '  - org.Gaurantees';
    RAISE NOTICE '  - org.LicenseDetails';
    RAISE NOTICE '  - org.CompanyAccountInfo';
    RAISE NOTICE '  - org.CompanyCancellationInfo';
    RAISE NOTICE '  - org.Haqulemtyaz';
    RAISE NOTICE '  - org.PeriodicForms';
    RAISE NOTICE '  - log.Companydetailsaudit';
    RAISE NOTICE '  - log.Companyowneraudit';
    RAISE NOTICE '  - log.Guarantorsaudit';
    RAISE NOTICE '  - log.Graunteeaudit';
    RAISE NOTICE '  - log.Licenseaudit';
    RAISE NOTICE '';
    RAISE NOTICE 'Latest Features Included:';
    RAISE NOTICE '  ✓ Province-based access control (ProvinceId in CompanyDetails)';
    RAISE NOTICE '  ✓ Province-specific license numbering (ProvinceId in LicenseDetails)';
    RAISE NOTICE '  ✓ Electronic National ID support';
    RAISE NOTICE '  ✓ License categories (جدید, تجدید, مثنی)';
    RAISE NOTICE '  ✓ Renewal round tracking';
    RAISE NOTICE '  ✓ Royalty and penalty tracking';
    RAISE NOTICE '  ✓ HR letter tracking';
    RAISE NOTICE '  ✓ Tariff number support';
    RAISE NOTICE '  ✓ License completion status tracking (IsComplete)';
    RAISE NOTICE '  ✓ Print restriction based on completion status';
    RAISE NOTICE '  ✓ Company financial information (CompanyAccountInfo)';
    RAISE NOTICE '  ✓ License cancellation tracking (CompanyCancellationInfo)';
    RAISE NOTICE '  ✓ Address history tracking';
    RAISE NOTICE '  ✓ Comprehensive audit logging';
    RAISE NOTICE '  ✓ Performance indexes (% total)', index_count;
    RAISE NOTICE '';
    RAISE NOTICE 'Verification:';
    RAISE NOTICE '  - Transaction tables: % / 11', transaction_table_count;
    RAISE NOTICE '  - Audit tables: % / 5', audit_table_count;
    RAISE NOTICE '  - Indexes created: %', index_count;
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Verify table structure above';
    RAISE NOTICE '  2. Test company registration in frontend';
    RAISE NOTICE '  3. Verify all fields save correctly';
    RAISE NOTICE '';
END $$;
