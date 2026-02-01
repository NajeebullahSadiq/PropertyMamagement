-- =====================================================
-- Property Module - Clean Recreate Script
-- Date: 2026-02-01
-- Purpose: Drop and recreate all property/real estate module tables
-- WARNING: This will DELETE ALL DATA in property module tables!
-- =====================================================

-- INSTRUCTIONS:
-- 1. BACKUP YOUR DATABASE FIRST!
-- 2. This script is for TESTING/STAGING environments only
-- 3. Execute: psql -h localhost -U postgres -d PRMIS -f property_module_clean_recreate.sql
-- 4. Verify tables are created correctly

-- =====================================================
-- STEP 1: DROP ALL PROPERTY MODULE TABLES
-- =====================================================

-- Drop tables in reverse dependency order (child tables first)

-- Drop audit tables (log schema)
DROP TABLE IF EXISTS log."Propertyaudit" CASCADE;
DROP TABLE IF EXISTS log."Propertybuyeraudit" CASCADE;
DROP TABLE IF EXISTS log."Propertyselleraudit" CASCADE;

-- Drop property transaction tables (tr schema)
DROP TABLE IF EXISTS tr."PropertyCancellationDocuments" CASCADE;
DROP TABLE IF EXISTS tr."PropertyCancellations" CASCADE;
DROP TABLE IF EXISTS tr."WitnessDetails" CASCADE;
DROP TABLE IF EXISTS tr."SellerDetails" CASCADE;
DROP TABLE IF EXISTS tr."BuyerDetails" CASCADE;
DROP TABLE IF EXISTS tr."PropertyAddress" CASCADE;
DROP TABLE IF EXISTS tr."PropertyDetails" CASCADE;

-- Confirmation message
DO $$
BEGIN
    RAISE NOTICE '✓ All property module tables dropped successfully';
END $$;

-- =====================================================
-- STEP 2: CREATE PROPERTY MODULE TABLES
-- =====================================================

-- Ensure schema exists
CREATE SCHEMA IF NOT EXISTS tr;
CREATE SCHEMA IF NOT EXISTS log;

-- =====================================================
-- PropertyDetails - Main property transaction table
-- =====================================================
CREATE TABLE tr."PropertyDetails" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Property Identification
    "PNumber" VARCHAR(100),
    "Parea" VARCHAR(100),
    "PunitTypeId" INTEGER,
    "NumofFloor" INTEGER,
    "NumofRooms" INTEGER,
    "PropertyTypeId" INTEGER,
    "CustomPropertyType" VARCHAR(255),
    
    -- Financial Information
    "Price" VARCHAR(100),
    "PriceText" TEXT,
    "RoyaltyAmount" VARCHAR(100),
    
    -- Transaction Information
    "TransactionTypeId" INTEGER,
    "Status" VARCHAR(50) DEFAULT 'Draft',
    "VerifiedBy" VARCHAR(50),
    "VerifiedAt" TIMESTAMP WITHOUT TIME ZONE,
    "ApprovedBy" VARCHAR(50),
    "ApprovedAt" TIMESTAMP WITHOUT TIME ZONE,
    
    -- Property Description
    "Des" TEXT,
    
    -- Boundaries
    "West" TEXT,
    "South" TEXT,
    "East" TEXT,
    "North" TEXT,
    
    -- Document Information
    "DocumentType" VARCHAR(100),
    "CustomDocumentType" VARCHAR(255),
    "IssuanceNumber" VARCHAR(100),
    "IssuanceDate" TIMESTAMP WITHOUT TIME ZONE,
    "SerialNumber" VARCHAR(100),
    "TransactionDate" TIMESTAMP WITHOUT TIME ZONE,
    
    -- File Paths
    "FilePath" TEXT,
    "PreviousDocumentsPath" TEXT,
    "ExistingDocumentsPath" TEXT,
    
    -- Status Flags
    "iscomplete" BOOLEAN DEFAULT FALSE,
    "iseditable" BOOLEAN DEFAULT TRUE,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Data Isolation (Province-based access control)
    "CompanyId" INTEGER
);

-- Add comments
COMMENT ON TABLE tr."PropertyDetails" IS 'Main property transaction details table';
COMMENT ON COLUMN tr."PropertyDetails"."CompanyId" IS 'Company ID for data isolation and province-based access control';
COMMENT ON COLUMN tr."PropertyDetails"."CustomPropertyType" IS 'Custom property type when PropertyTypeId is "Other"';
COMMENT ON COLUMN tr."PropertyDetails"."CustomDocumentType" IS 'Custom document type when DocumentType is "Other"';

-- =====================================================
-- PropertyAddress - Property location information
-- =====================================================
CREATE TABLE tr."PropertyAddress" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER,
    "ProvinceId" INTEGER,
    "DistrictId" INTEGER,
    "Village" TEXT,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    CONSTRAINT "PropertyAddress_PropertyDetailsId_fkey" 
        FOREIGN KEY ("PropertyDetailsId") 
        REFERENCES tr."PropertyDetails"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE tr."PropertyAddress" IS 'Property location and address information';

-- =====================================================
-- BuyerDetails - Property buyer information
-- =====================================================
CREATE TABLE tr."BuyerDetails" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Personal Information
    "FirstName" VARCHAR(255) NOT NULL,
    "FatherName" VARCHAR(255) NOT NULL,
    "GrandFather" VARCHAR(255) NOT NULL,
    "ElectronicNationalIdNumber" VARCHAR(50),
    "PhoneNumber" VARCHAR(14),
    
    -- Permanent Address
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    
    -- Temporary Address
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" TEXT,
    
    -- Documents
    "Photo" TEXT,
    "NationalIdCard" TEXT,
    "AuthorizationLetter" TEXT,
    
    -- Role and Type
    "RoleType" VARCHAR(100),
    "TransactionType" VARCHAR(100),
    "TransactionTypeDescription" TEXT,
    
    -- Financial Information
    "SharePercentage" VARCHAR(50),
    "ShareAmount" VARCHAR(100),
    "Price" VARCHAR(100),
    "PriceText" TEXT,
    "RoyaltyAmount" VARCHAR(100),
    "HalfPrice" VARCHAR(100),
    
    -- Rental Information
    "RentStartDate" TIMESTAMP WITHOUT TIME ZONE,
    "RentEndDate" TIMESTAMP WITHOUT TIME ZONE,
    
    -- Additional Information
    "TaxIdentificationNumber" VARCHAR(100),
    "AdditionalDetails" TEXT,
    
    -- Foreign Key
    "PropertyDetailsId" INTEGER,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    CONSTRAINT "BuyerDetails_PropertyDetailsId_fkey" 
        FOREIGN KEY ("PropertyDetailsId") 
        REFERENCES tr."PropertyDetails"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE tr."BuyerDetails" IS 'Property buyer/purchaser information';
COMMENT ON COLUMN tr."BuyerDetails"."ElectronicNationalIdNumber" IS 'الیکټرونیکی تذکره - Electronic National ID';
COMMENT ON COLUMN tr."BuyerDetails"."RoleType" IS 'Buyer or Authorized Agent (Buyer)';

-- =====================================================
-- SellerDetails - Property seller information
-- =====================================================
CREATE TABLE tr."SellerDetails" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Personal Information
    "FirstName" VARCHAR(255) NOT NULL,
    "FatherName" VARCHAR(255) NOT NULL,
    "GrandFather" VARCHAR(255) NOT NULL,
    "ElectronicNationalIdNumber" VARCHAR(50),
    "PhoneNumber" VARCHAR(14),
    
    -- Permanent Address
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    
    -- Temporary Address
    "TaddressProvinceId" INTEGER,
    "TaddressDistrictId" INTEGER,
    "TaddressVillage" TEXT,
    
    -- Documents
    "Photo" TEXT,
    "NationalIdCard" TEXT,
    "AuthorizationLetter" TEXT,
    "HeirsLetter" TEXT,
    
    -- Role
    "RoleType" VARCHAR(100),
    
    -- Financial Information
    "Price" VARCHAR(100),
    "RoyaltyAmount" VARCHAR(100),
    "HalfPrice" VARCHAR(100),
    "SharePercentage" VARCHAR(50),
    "ShareAmount" VARCHAR(100),
    
    -- Additional Information
    "TaxIdentificationNumber" VARCHAR(100),
    "AdditionalDetails" TEXT,
    
    -- Foreign Key
    "PropertyDetailsId" INTEGER,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    CONSTRAINT "SellerDetails_PropertyDetailsId_fkey" 
        FOREIGN KEY ("PropertyDetailsId") 
        REFERENCES tr."PropertyDetails"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE tr."SellerDetails" IS 'Property seller/vendor information';
COMMENT ON COLUMN tr."SellerDetails"."ElectronicNationalIdNumber" IS 'الیکټرونیکی تذکره - Electronic National ID';
COMMENT ON COLUMN tr."SellerDetails"."RoleType" IS 'Seller or Authorized Agent (Seller)';

-- =====================================================
-- WitnessDetails - Property transaction witness information
-- =====================================================
CREATE TABLE tr."WitnessDetails" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Personal Information
    "FirstName" VARCHAR(255) NOT NULL,
    "FatherName" VARCHAR(255) NOT NULL,
    "GrandFatherName" VARCHAR(255),
    "ElectronicNationalIdNumber" VARCHAR(50),
    "PhoneNumber" VARCHAR(20),
    
    -- Address
    "PaddressProvinceId" INTEGER,
    "PaddressDistrictId" INTEGER,
    "PaddressVillage" TEXT,
    
    -- Documents
    "NationalIdCard" TEXT,
    
    -- Witness Information
    "RelationshipToParties" VARCHAR(100),
    "WitnessType" VARCHAR(100),
    "WitnessSide" VARCHAR(50),
    "Des" TEXT,
    
    -- Foreign Key
    "PropertyDetailsId" INTEGER,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    CONSTRAINT "WitnessDetails_PropertyDetailsId_fkey" 
        FOREIGN KEY ("PropertyDetailsId") 
        REFERENCES tr."PropertyDetails"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE tr."WitnessDetails" IS 'Property transaction witness information';
COMMENT ON COLUMN tr."WitnessDetails"."GrandFatherName" IS 'نام پدر کلان - Grandfather name';
COMMENT ON COLUMN tr."WitnessDetails"."WitnessSide" IS 'شاهد از طرف - Witness side (Buyer/Seller)';
COMMENT ON COLUMN tr."WitnessDetails"."Des" IS 'جزئیات دیگر - Additional details';

-- =====================================================
-- PropertyCancellations - Property cancellation records
-- =====================================================
CREATE TABLE tr."PropertyCancellations" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER NOT NULL,
    "CancellationDate" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "CancellationReason" TEXT,
    "CancelledBy" VARCHAR(50),
    "Status" VARCHAR(50) DEFAULT 'Cancelled',
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT "PropertyCancellations_PropertyDetailsId_fkey" 
        FOREIGN KEY ("PropertyDetailsId") 
        REFERENCES tr."PropertyDetails"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE tr."PropertyCancellations" IS 'Property transaction cancellation records';

-- =====================================================
-- PropertyCancellationDocuments - Cancellation supporting documents
-- =====================================================
CREATE TABLE tr."PropertyCancellationDocuments" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyCancellationId" INTEGER NOT NULL,
    "DocumentPath" TEXT,
    "DocumentName" VARCHAR(255),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    CONSTRAINT "PropertyCancellationDocuments_PropertyCancellationId_fkey" 
        FOREIGN KEY ("PropertyCancellationId") 
        REFERENCES tr."PropertyCancellations"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE tr."PropertyCancellationDocuments" IS 'Supporting documents for property cancellations';

-- =====================================================
-- STEP 3: CREATE AUDIT TABLES (log schema)
-- =====================================================

-- Propertyaudit - Property details audit log
CREATE TABLE log."Propertyaudit" (
    "Id" SERIAL PRIMARY KEY,
    "PropertyDetailsId" INTEGER,
    "Action" VARCHAR(50),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" VARCHAR(50),
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE log."Propertyaudit" IS 'Audit log for property details changes';

-- Propertybuyeraudit - Buyer details audit log
CREATE TABLE log."Propertybuyeraudit" (
    "Id" SERIAL PRIMARY KEY,
    "BuyerDetailsId" INTEGER,
    "Action" VARCHAR(50),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" VARCHAR(50),
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE log."Propertybuyeraudit" IS 'Audit log for buyer details changes';

-- Propertyselleraudit - Seller details audit log
CREATE TABLE log."Propertyselleraudit" (
    "Id" SERIAL PRIMARY KEY,
    "SellerDetailsId" INTEGER,
    "Action" VARCHAR(50),
    "OldValue" TEXT,
    "NewValue" TEXT,
    "ChangedBy" VARCHAR(50),
    "ChangedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

COMMENT ON TABLE log."Propertyselleraudit" IS 'Audit log for seller details changes';

-- =====================================================
-- STEP 4: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- PropertyAddress indexes
CREATE INDEX "IX_PropertyAddress_PropertyDetailsId" 
    ON tr."PropertyAddress" ("PropertyDetailsId");
CREATE INDEX "IX_PropertyAddress_ProvinceId" 
    ON tr."PropertyAddress" ("ProvinceId");
CREATE INDEX "IX_PropertyAddress_DistrictId" 
    ON tr."PropertyAddress" ("DistrictId");

-- BuyerDetails indexes
CREATE INDEX "IX_BuyerDetails_PropertyDetailsId" 
    ON tr."BuyerDetails" ("PropertyDetailsId");
CREATE INDEX "IX_BuyerDetails_ElectronicNationalIdNumber" 
    ON tr."BuyerDetails" ("ElectronicNationalIdNumber");
CREATE INDEX "IX_BuyerDetails_PhoneNumber" 
    ON tr."BuyerDetails" ("PhoneNumber");

-- SellerDetails indexes
CREATE INDEX "IX_SellerDetails_PropertyDetailsId" 
    ON tr."SellerDetails" ("PropertyDetailsId");
CREATE INDEX "IX_SellerDetails_ElectronicNationalIdNumber" 
    ON tr."SellerDetails" ("ElectronicNationalIdNumber");
CREATE INDEX "IX_SellerDetails_PhoneNumber" 
    ON tr."SellerDetails" ("PhoneNumber");

-- WitnessDetails indexes
CREATE INDEX "IX_WitnessDetails_PropertyDetailsId" 
    ON tr."WitnessDetails" ("PropertyDetailsId");
CREATE INDEX "IX_WitnessDetails_ElectronicNationalIdNumber" 
    ON tr."WitnessDetails" ("ElectronicNationalIdNumber");

-- PropertyCancellations indexes
CREATE INDEX "IX_PropertyCancellations_PropertyDetailsId" 
    ON tr."PropertyCancellations" ("PropertyDetailsId");
CREATE INDEX "IX_PropertyCancellations_Status" 
    ON tr."PropertyCancellations" ("Status");

-- PropertyCancellationDocuments indexes
CREATE INDEX "IX_PropertyCancellationDocuments_PropertyCancellationId" 
    ON tr."PropertyCancellationDocuments" ("PropertyCancellationId");

-- PropertyDetails indexes
CREATE INDEX "IX_PropertyDetails_CompanyId" 
    ON tr."PropertyDetails" ("CompanyId");
CREATE INDEX "IX_PropertyDetails_Status" 
    ON tr."PropertyDetails" ("Status");
CREATE INDEX "IX_PropertyDetails_TransactionTypeId" 
    ON tr."PropertyDetails" ("TransactionTypeId");
CREATE INDEX "IX_PropertyDetails_PropertyTypeId" 
    ON tr."PropertyDetails" ("PropertyTypeId");
CREATE INDEX "IX_PropertyDetails_CreatedAt" 
    ON tr."PropertyDetails" ("CreatedAt");

-- Audit table indexes
CREATE INDEX "IX_Propertyaudit_PropertyDetailsId" 
    ON log."Propertyaudit" ("PropertyDetailsId");
CREATE INDEX "IX_Propertyaudit_ChangedAt" 
    ON log."Propertyaudit" ("ChangedAt");

CREATE INDEX "IX_Propertybuyeraudit_BuyerDetailsId" 
    ON log."Propertybuyeraudit" ("BuyerDetailsId");
CREATE INDEX "IX_Propertybuyeraudit_ChangedAt" 
    ON log."Propertybuyeraudit" ("ChangedAt");

CREATE INDEX "IX_Propertyselleraudit_SellerDetailsId" 
    ON log."Propertyselleraudit" ("SellerDetailsId");
CREATE INDEX "IX_Propertyselleraudit_ChangedAt" 
    ON log."Propertyselleraudit" ("ChangedAt");

-- =====================================================
-- STEP 5: VERIFICATION QUERIES
-- =====================================================

-- Verify all tables were created
DO $$
DECLARE
    table_count INTEGER;
BEGIN
    -- Count property module tables
    SELECT COUNT(*) INTO table_count
    FROM information_schema.tables
    WHERE table_schema = 'tr'
    AND table_name IN (
        'PropertyDetails',
        'PropertyAddress',
        'BuyerDetails',
        'SellerDetails',
        'WitnessDetails',
        'PropertyCancellations',
        'PropertyCancellationDocuments'
    );
    
    IF table_count = 7 THEN
        RAISE NOTICE '✓ All 7 property module tables created successfully';
    ELSE
        RAISE WARNING '⚠ Expected 7 tables, found %', table_count;
    END IF;
    
    -- Count audit tables
    SELECT COUNT(*) INTO table_count
    FROM information_schema.tables
    WHERE table_schema = 'log'
    AND table_name IN (
        'Propertyaudit',
        'Propertybuyeraudit',
        'Propertyselleraudit'
    );
    
    IF table_count = 3 THEN
        RAISE NOTICE '✓ All 3 property audit tables created successfully';
    ELSE
        RAISE WARNING '⚠ Expected 3 audit tables, found %', table_count;
    END IF;
END $$;

-- List all created tables
SELECT 
    schemaname AS "Schema",
    tablename AS "Table Name",
    pg_size_pretty(pg_total_relation_size('"' || schemaname || '"."' || tablename || '"')) AS "Size"
FROM pg_tables
WHERE schemaname IN ('tr', 'log')
AND (
    tablename LIKE '%roperty%' 
    OR tablename LIKE '%uyer%' 
    OR tablename LIKE '%eller%' 
    OR tablename LIKE '%itness%'
    OR tablename LIKE '%ancellation%'
    OR tablename LIKE '%audit%'
)
ORDER BY schemaname, tablename;

-- =====================================================
-- COMPLETION MESSAGE
-- =====================================================

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Property Module Recreation Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Tables Created:';
    RAISE NOTICE '  - tr.PropertyDetails';
    RAISE NOTICE '  - tr.PropertyAddress';
    RAISE NOTICE '  - tr.BuyerDetails';
    RAISE NOTICE '  - tr.SellerDetails';
    RAISE NOTICE '  - tr.WitnessDetails';
    RAISE NOTICE '  - tr.PropertyCancellations';
    RAISE NOTICE '  - tr.PropertyCancellationDocuments';
    RAISE NOTICE '  - log.Propertyaudit';
    RAISE NOTICE '  - log.Propertybuyeraudit';
    RAISE NOTICE '  - log.Propertyselleraudit';
    RAISE NOTICE '';
    RAISE NOTICE 'Latest Features Included:';
    RAISE NOTICE '  ✓ Province-based access control (CompanyId)';
    RAISE NOTICE '  ✓ Custom property type support';
    RAISE NOTICE '  ✓ Custom document type support';
    RAISE NOTICE '  ✓ Witness grandfather name field';
    RAISE NOTICE '  ✓ Witness side field (Buyer/Seller)';
    RAISE NOTICE '  ✓ Witness additional details field';
    RAISE NOTICE '  ✓ Electronic National ID fields';
    RAISE NOTICE '  ✓ Multiple buyers/sellers support';
    RAISE NOTICE '  ✓ Comprehensive audit logging';
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Verify table structure above';
    RAISE NOTICE '  2. Test property registration in frontend';
    RAISE NOTICE '  3. Verify all fields save correctly';
    RAISE NOTICE '';
END $$;

-- =====================================================
-- END OF SCRIPT
-- =====================================================
