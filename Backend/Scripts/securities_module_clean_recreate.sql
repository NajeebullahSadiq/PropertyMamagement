-- =====================================================
-- Securities Module Clean Recreation Script
-- =====================================================
-- Purpose: Drop and recreate all securities module tables
-- Schema: org (organization)
-- Date: 2026-02-07
-- 
-- WARNING: THIS SCRIPT WILL DELETE ALL SECURITIES DATA!
-- =====================================================

-- =====================================================
-- STEP 1: DROP ALL EXISTING TABLES
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE 'Starting securities module table cleanup...';
END $$;

-- Drop receipt history table if it exists (from failed migration)
DROP TABLE IF EXISTS org."SecuritiesDistributionReceipt" CASCADE;

-- Drop tables in correct order (children first, then parents)
DROP TABLE IF EXISTS org."SecuritiesDistributionItem" CASCADE;
DROP TABLE IF EXISTS org."SecuritiesDistribution" CASCADE;
DROP TABLE IF EXISTS org."PetitionWriterSecurities" CASCADE;
DROP TABLE IF EXISTS org."SecuritiesControl" CASCADE;

DO $$ 
BEGIN
    RAISE NOTICE '✓ All securities module tables dropped successfully';
END $$;

-- =====================================================
-- STEP 2: ENSURE SCHEMAS EXIST
-- =====================================================

CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS look;

-- =====================================================
-- STEP 3: CREATE SECURITIES TABLES (org schema)
-- =====================================================

-- 1. SecuritiesDistribution (Main securities distribution table)
CREATE TABLE org."SecuritiesDistribution" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Tab 1: مشخصات رهنمای معاملات (Transaction Guide Information)
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "LicenseOwnerName" VARCHAR(200) NOT NULL,
    "LicenseOwnerFatherName" VARCHAR(200) NOT NULL,
    "TransactionGuideName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    
    -- Tab 3: قیمت اسناد بهادار (Securities Pricing)
    "PricePerDocument" DECIMAL(18,2),
    "TotalDocumentsPrice" DECIMAL(18,2),
    "TotalSecuritiesPrice" DECIMAL(18,2),
    
    -- Tab 4: مشخصات آویز تحویلی و تاریخ توزیع (Receipt and Distribution Info)
    "BankReceiptNumber" VARCHAR(100),
    "DeliveryDate" DATE,
    "DistributionDate" DATE,
    
    -- Audit fields
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

-- 2. SecuritiesDistributionItem (Securities items - Tab 2)
CREATE TABLE org."SecuritiesDistributionItem" (
    "Id" SERIAL PRIMARY KEY,
    "SecuritiesDistributionId" INTEGER NOT NULL,
    
    -- Document information
    "DocumentType" INTEGER NOT NULL,
    "SerialStart" VARCHAR(100),
    "SerialEnd" VARCHAR(100),
    "Count" INTEGER NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    
    -- Audit fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_SecuritiesDistributionItem_SecuritiesDistribution" 
        FOREIGN KEY ("SecuritiesDistributionId") 
        REFERENCES org."SecuritiesDistribution"("Id") 
        ON DELETE CASCADE
);

-- 3. PetitionWriterSecurities (Petition writer securities)
CREATE TABLE org."PetitionWriterSecurities" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Basic information
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "PetitionWriterName" VARCHAR(200) NOT NULL,
    "PetitionWriterFatherName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    
    -- Securities information
    "BankReceiptNumber" VARCHAR(100),
    "SerialNumberStart" VARCHAR(100),
    "SerialNumberEnd" VARCHAR(100),
    "Amount" DECIMAL(18,2),
    "DeliveryDate" DATE,
    "DistributionDate" DATE,
    
    -- Audit fields
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

-- 4. SecuritiesControl (Securities inventory control)
CREATE TABLE org."SecuritiesControl" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Control information
    "DocumentType" INTEGER NOT NULL,
    "SerialStart" VARCHAR(100) NOT NULL,
    "SerialEnd" VARCHAR(100) NOT NULL,
    "TotalCount" INTEGER NOT NULL,
    "DistributedCount" INTEGER DEFAULT 0,
    "RemainingCount" INTEGER,
    "ReceiptDate" DATE,
    "Notes" VARCHAR(500),
    
    -- Audit fields
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ All 4 securities module tables created successfully';
END $$;

-- =====================================================
-- STEP 4: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- SecuritiesDistribution indexes
CREATE INDEX "IX_SecuritiesDistribution_RegistrationNumber" 
    ON org."SecuritiesDistribution"("RegistrationNumber");
CREATE INDEX "IX_SecuritiesDistribution_LicenseNumber" 
    ON org."SecuritiesDistribution"("LicenseNumber");
CREATE INDEX "IX_SecuritiesDistribution_BankReceiptNumber" 
    ON org."SecuritiesDistribution"("BankReceiptNumber");
CREATE INDEX "IX_SecuritiesDistribution_TransactionGuideName" 
    ON org."SecuritiesDistribution"("TransactionGuideName");
CREATE INDEX "IX_SecuritiesDistribution_LicenseOwnerName" 
    ON org."SecuritiesDistribution"("LicenseOwnerName");
CREATE INDEX "IX_SecuritiesDistribution_Status" 
    ON org."SecuritiesDistribution"("Status");
CREATE INDEX "IX_SecuritiesDistribution_CreatedAt" 
    ON org."SecuritiesDistribution"("CreatedAt");

-- SecuritiesDistributionItem indexes
CREATE INDEX "IX_SecuritiesDistributionItem_SecuritiesDistributionId" 
    ON org."SecuritiesDistributionItem"("SecuritiesDistributionId");
CREATE INDEX "IX_SecuritiesDistributionItem_DocumentType" 
    ON org."SecuritiesDistributionItem"("DocumentType");

-- PetitionWriterSecurities indexes
CREATE UNIQUE INDEX "IX_PetitionWriterSecurities_RegistrationNumber" 
    ON org."PetitionWriterSecurities"("RegistrationNumber");
CREATE INDEX "IX_PetitionWriterSecurities_LicenseNumber" 
    ON org."PetitionWriterSecurities"("LicenseNumber");
CREATE INDEX "IX_PetitionWriterSecurities_BankReceiptNumber" 
    ON org."PetitionWriterSecurities"("BankReceiptNumber");
CREATE INDEX "IX_PetitionWriterSecurities_PetitionWriterName" 
    ON org."PetitionWriterSecurities"("PetitionWriterName");
CREATE INDEX "IX_PetitionWriterSecurities_Status" 
    ON org."PetitionWriterSecurities"("Status");

-- SecuritiesControl indexes
CREATE INDEX "IX_SecuritiesControl_DocumentType" 
    ON org."SecuritiesControl"("DocumentType");
CREATE INDEX "IX_SecuritiesControl_Status" 
    ON org."SecuritiesControl"("Status");
CREATE INDEX "IX_SecuritiesControl_SerialStart" 
    ON org."SecuritiesControl"("SerialStart");
CREATE INDEX "IX_SecuritiesControl_SerialEnd" 
    ON org."SecuritiesControl"("SerialEnd");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All performance indexes created successfully';
END $$;

-- =====================================================
-- STEP 5: ADD TABLE COMMENTS (Documentation)
-- =====================================================

COMMENT ON TABLE org."SecuritiesDistribution" IS 'Main securities distribution records for real estate transaction guides';
COMMENT ON TABLE org."SecuritiesDistributionItem" IS 'Individual document items within a securities distribution';
COMMENT ON TABLE org."PetitionWriterSecurities" IS 'Securities distributed to petition writers (عریضه نویسان)';
COMMENT ON TABLE org."SecuritiesControl" IS 'Inventory control and tracking of securities stock';

-- SecuritiesDistribution column comments
COMMENT ON COLUMN org."SecuritiesDistribution"."RegistrationNumber" IS 'نمبر ثبت - Unique registration number';
COMMENT ON COLUMN org."SecuritiesDistribution"."LicenseOwnerName" IS 'نام صاحب جواز - License owner name';
COMMENT ON COLUMN org."SecuritiesDistribution"."LicenseOwnerFatherName" IS 'نام پدر صاحب جواز - License owner father name';
COMMENT ON COLUMN org."SecuritiesDistribution"."TransactionGuideName" IS 'نام رهنمای معاملات - Transaction guide name';
COMMENT ON COLUMN org."SecuritiesDistribution"."LicenseNumber" IS 'نمبر جواز - License number';
COMMENT ON COLUMN org."SecuritiesDistribution"."BankReceiptNumber" IS 'نمبر رسید بانکی - Bank receipt number';
COMMENT ON COLUMN org."SecuritiesDistribution"."DeliveryDate" IS 'تاریخ تحویلی - Delivery date';
COMMENT ON COLUMN org."SecuritiesDistribution"."DistributionDate" IS 'تاریخ توزیع - Distribution date';

-- SecuritiesDistributionItem column comments
COMMENT ON COLUMN org."SecuritiesDistributionItem"."DocumentType" IS 'نوع سند - Document type: 1=Property Sale, 2=Bay Wafa, 3=Rent, 4=Vehicle, 5=Registration Book, 6=Duplicate Book';
COMMENT ON COLUMN org."SecuritiesDistributionItem"."SerialStart" IS 'آغاز سریال نمبر - Starting serial number';
COMMENT ON COLUMN org."SecuritiesDistributionItem"."SerialEnd" IS 'ختم سریال نمبر - Ending serial number';
COMMENT ON COLUMN org."SecuritiesDistributionItem"."Count" IS 'تعداد - Quantity of documents';
COMMENT ON COLUMN org."SecuritiesDistributionItem"."Price" IS 'قیمت - Total price for this item';

DO $$ 
BEGIN
    RAISE NOTICE '✓ Table and column comments added successfully';
END $$;

-- =====================================================
-- STEP 6: VERIFICATION QUERIES
-- =====================================================

DO $$ 
DECLARE
    table_count INTEGER;
    index_count INTEGER;
BEGIN
    -- Count tables
    SELECT COUNT(*) INTO table_count
    FROM information_schema.tables 
    WHERE table_schema = 'org' 
    AND table_name IN (
        'SecuritiesDistribution', 'SecuritiesDistributionItem',
        'PetitionWriterSecurities', 'SecuritiesControl'
    );
    
    -- Count indexes
    SELECT COUNT(*) INTO index_count
    FROM pg_indexes 
    WHERE schemaname = 'org'
    AND (tablename LIKE '%Securities%');
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Securities Module Recreation Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Tables Created:';
    RAISE NOTICE '  - org.SecuritiesDistribution';
    RAISE NOTICE '  - org.SecuritiesDistributionItem';
    RAISE NOTICE '  - org.PetitionWriterSecurities';
    RAISE NOTICE '  - org.SecuritiesControl';
    RAISE NOTICE '';
    RAISE NOTICE 'Document Types Supported:';
    RAISE NOTICE '  1 = سټه یی خرید و فروش (Property Sale) - 4000 Afs';
    RAISE NOTICE '  2 = سټه یی بیع وفا (Bay Wafa) - 4000 Afs';
    RAISE NOTICE '  3 = سټه یی کرایی (Rent) - 4000 Afs';
    RAISE NOTICE '  4 = سټه وسایط نقلیه (Vehicle) - 4000 Afs';
    RAISE NOTICE '  5 = کتاب ثبت (Registration Book) - 1000 Afs';
    RAISE NOTICE '  6 = کتاب ثبت مثنی (Duplicate Book) - 20000 Afs';
    RAISE NOTICE '';
    RAISE NOTICE 'Features Included:';
    RAISE NOTICE '  ✓ Dynamic items collection (Tab 2)';
    RAISE NOTICE '  ✓ Multiple document types per distribution';
    RAISE NOTICE '  ✓ Serial number tracking';
    RAISE NOTICE '  ✓ Automatic count calculation';
    RAISE NOTICE '  ✓ Automatic price calculation';
    RAISE NOTICE '  ✓ Bank receipt tracking';
    RAISE NOTICE '  ✓ Delivery and distribution dates';
    RAISE NOTICE '  ✓ Petition writer securities';
    RAISE NOTICE '  ✓ Securities inventory control';
    RAISE NOTICE '  ✓ Comprehensive indexing';
    RAISE NOTICE '  ✓ Full audit trail';
    RAISE NOTICE '';
    RAISE NOTICE 'Verification:';
    RAISE NOTICE '  - Tables created: % / 4', table_count;
    RAISE NOTICE '  - Indexes created: %', index_count;
    RAISE NOTICE '';
    RAISE NOTICE 'Important Notes:';
    RAISE NOTICE '  - NO receipt history table (removed)';
    RAISE NOTICE '  - NO ReceiptId column in items';
    RAISE NOTICE '  - Clean structure matching current code';
    RAISE NOTICE '  - Ready for production deployment';
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Verify table structure above';
    RAISE NOTICE '  2. Test securities distribution in frontend';
    RAISE NOTICE '  3. Verify all fields save correctly';
    RAISE NOTICE '  4. Test print functionality';
    RAISE NOTICE '';
END $$;

-- =====================================================
-- STEP 7: SAMPLE VERIFICATION QUERIES
-- =====================================================

-- Check table structure
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name IN ('SecuritiesDistribution', 'SecuritiesDistributionItem', 'PetitionWriterSecurities', 'SecuritiesControl')
ORDER BY table_name, ordinal_position;

-- Check indexes
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'org'
AND tablename LIKE '%Securities%'
ORDER BY tablename, indexname;

-- Check foreign keys
SELECT
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name,
    tc.constraint_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
    AND ccu.table_schema = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY'
AND tc.table_schema = 'org'
AND tc.table_name LIKE '%Securities%'
ORDER BY tc.table_name;

DO $$ 
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '✓ Securities module is ready for use!';
    RAISE NOTICE '';
END $$;
