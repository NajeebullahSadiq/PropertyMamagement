-- =====================================================
-- Petition Writer Securities Module Clean Recreation Script
-- =====================================================
-- Purpose: Drop and recreate the PetitionWriterSecurities table
-- Schema: sec (securities)
-- Date: 2026-03-14
-- 
-- WARNING: THIS SCRIPT WILL DELETE ALL PETITION WRITER SECURITIES DATA!
-- =====================================================

-- =====================================================
-- STEP 1: DROP EXISTING TABLE
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE 'Starting Petition Writer Securities table cleanup...';
END $$;

-- Drop table (cascade to remove any dependencies)
DROP TABLE IF EXISTS sec."PetitionWriterSecurities" CASCADE;

DO $$ 
BEGIN
    RAISE NOTICE '✓ PetitionWriterSecurities table dropped successfully';
END $$;

-- =====================================================
-- STEP 2: ENSURE SCHEMA EXISTS
-- =====================================================

CREATE SCHEMA IF NOT EXISTS sec;

-- =====================================================
-- STEP 3: CREATE PETITION WRITER SECURITIES TABLE
-- =====================================================

-- PetitionWriterSecurities (سند بهادار عریضه‌ نویسان)
CREATE TABLE sec."PetitionWriterSecurities" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Tab 1: مشخصات عریضه‌نویس (Petition Writer Information)
    "RegistrationNumber" VARCHAR(50) NOT NULL,
    "PetitionWriterName" VARCHAR(200) NOT NULL,
    "PetitionWriterFatherName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50) NOT NULL,
    
    -- Tab 2: مشخصات سند بهادار عریضه (Securities Details)
    "PetitionCount" INTEGER NOT NULL,
    "Amount" DECIMAL(18, 2) NOT NULL,
    "BankReceiptNumber" VARCHAR(100) NOT NULL,
    "SerialNumberStart" VARCHAR(100) NOT NULL,
    "SerialNumberEnd" VARCHAR(100) NOT NULL,
    "DistributionDate" DATE NOT NULL,
    "DeliveryDate" DATE,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    "Status" BOOLEAN DEFAULT true,
    
    -- Constraints
    CONSTRAINT "UQ_PetitionWriterSecurities_RegistrationNumber" UNIQUE ("RegistrationNumber")
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ PetitionWriterSecurities table created successfully';
END $$;

-- =====================================================
-- STEP 4: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

CREATE INDEX "IX_PetitionWriterSecurities_RegistrationNumber" ON sec."PetitionWriterSecurities"("RegistrationNumber");
CREATE INDEX "IX_PetitionWriterSecurities_PetitionWriterName" ON sec."PetitionWriterSecurities"("PetitionWriterName");
CREATE INDEX "IX_PetitionWriterSecurities_LicenseNumber" ON sec."PetitionWriterSecurities"("LicenseNumber");
CREATE INDEX "IX_PetitionWriterSecurities_DistributionDate" ON sec."PetitionWriterSecurities"("DistributionDate");
CREATE INDEX "IX_PetitionWriterSecurities_DeliveryDate" ON sec."PetitionWriterSecurities"("DeliveryDate");
CREATE INDEX "IX_PetitionWriterSecurities_Status" ON sec."PetitionWriterSecurities"("Status");
CREATE INDEX "IX_PetitionWriterSecurities_CreatedAt" ON sec."PetitionWriterSecurities"("CreatedAt");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All performance indexes created successfully';
END $$;

-- =====================================================
-- STEP 5: VERIFICATION QUERIES
-- =====================================================

DO $$ 
DECLARE
    table_exists INTEGER;
    index_count INTEGER;
BEGIN
    -- Check if table exists
    SELECT COUNT(*) INTO table_exists
    FROM information_schema.tables 
    WHERE table_schema = 'sec' 
    AND table_name = 'PetitionWriterSecurities';
    
    -- Count indexes
    SELECT COUNT(*) INTO index_count
    FROM pg_indexes 
    WHERE schemaname = 'sec'
    AND tablename = 'PetitionWriterSecurities';
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Petition Writer Securities Recreation Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Table Created:';
    RAISE NOTICE '  - sec.PetitionWriterSecurities';
    RAISE NOTICE '';
    RAISE NOTICE 'Columns:';
    RAISE NOTICE '  - Id (SERIAL PRIMARY KEY)';
    RAISE NOTICE '  - RegistrationNumber (VARCHAR 50, UNIQUE, NOT NULL)';
    RAISE NOTICE '  - PetitionWriterName (VARCHAR 200, NOT NULL)';
    RAISE NOTICE '  - PetitionWriterFatherName (VARCHAR 200, NOT NULL)';
    RAISE NOTICE '  - LicenseNumber (VARCHAR 50, NOT NULL)';
    RAISE NOTICE '  - PetitionCount (INTEGER, NOT NULL)';
    RAISE NOTICE '  - Amount (DECIMAL 18,2, NOT NULL)';
    RAISE NOTICE '  - BankReceiptNumber (VARCHAR 100, NOT NULL)';
    RAISE NOTICE '  - SerialNumberStart (VARCHAR 100, NOT NULL)';
    RAISE NOTICE '  - SerialNumberEnd (VARCHAR 100, NOT NULL)';
    RAISE NOTICE '  - DistributionDate (DATE, NOT NULL)';
    RAISE NOTICE '  - DeliveryDate (DATE, NULLABLE)';
    RAISE NOTICE '  - CreatedAt (TIMESTAMP)';
    RAISE NOTICE '  - CreatedBy (VARCHAR 50)';
    RAISE NOTICE '  - UpdatedAt (TIMESTAMP)';
    RAISE NOTICE '  - UpdatedBy (VARCHAR 50)';
    RAISE NOTICE '  - Status (BOOLEAN)';
    RAISE NOTICE '';
    RAISE NOTICE 'Verification:';
    RAISE NOTICE '  - Table exists: %', CASE WHEN table_exists = 1 THEN 'YES' ELSE 'NO' END;
    RAISE NOTICE '  - Indexes created: %', index_count;
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Verify table structure above';
    RAISE NOTICE '  2. Test petition writer securities registration in frontend';
    RAISE NOTICE '  3. Verify all fields save correctly';
    RAISE NOTICE '';
END $$;
