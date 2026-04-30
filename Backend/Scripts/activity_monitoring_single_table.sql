-- =====================================================
-- Activity Monitoring Module - Single Table Design
-- =====================================================
-- Purpose: Drop all activity monitoring tables and create a single unified table
-- Schema: org (organization)
-- Date: 2026-03-16
-- 
-- WARNING: THIS SCRIPT WILL DELETE ALL ACTIVITY MONITORING DATA!
-- =====================================================

-- =====================================================
-- STEP 1: DROP ALL EXISTING TABLES
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE 'Starting activity monitoring module table cleanup...';
END $$;

-- Drop tables in correct order (children first, then parents)
DROP TABLE IF EXISTS org."ActivityMonitoringInspections" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringPetitionWriterViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRealEstateViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringComplaints" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringDeedItems" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRecords" CASCADE;

DO $$ 
BEGIN
    RAISE NOTICE '✓ All activity monitoring tables dropped successfully';
END $$;

-- =====================================================
-- STEP 2: CREATE SINGLE UNIFIED TABLE
-- =====================================================

CREATE TABLE org."ActivityMonitoringRecords" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Common Fields (Section 1: Annual Report - گزارش سالانه)
    "SerialNumber" VARCHAR(50),
    "LicenseNumber" VARCHAR(50),
    "LicenseHolderName" VARCHAR(200),
    "CompanyTitle" VARCHAR(300),
    "District" VARCHAR(200),
    "ReportRegistrationDate" DATE,
    "SectionType" VARCHAR(50),
    
    -- Deed Counts (for Annual Report)
    "SaleDeedsCount" INTEGER,
    "RentalDeedsCount" INTEGER,
    "BaiUlWafaDeedsCount" INTEGER,
    "VehicleTransactionDeedsCount" INTEGER,
    
    -- Deed Items (JSON array for flexibility)
    "DeedItems" JSONB,
    
    -- Section 2: Complaints (ثبت شکایات)
    "ComplaintSubject" VARCHAR(500),
    "ComplainantName" VARCHAR(200),
    "ComplaintActionsTaken" VARCHAR(1000),
    "ComplaintRemarks" VARCHAR(1000),
    
    -- Section 3: Real Estate Violations (تخلفات دفاتر رهنمای معاملات)
    "ViolationStatus" VARCHAR(100),
    "ViolationType" VARCHAR(500),
    "ClosureReason" VARCHAR(500),
    "ViolationActionsTaken" VARCHAR(1000),
    "ViolationRemarks" VARCHAR(1000),
    
    -- Section 4: Inspections (نظارت و بازرسی)
    "Year" VARCHAR(20),
    "Month" VARCHAR(50),
    "MonitoringCount" INTEGER,
    "MonitoringRemarks" VARCHAR(1000),
    
    -- Audit fields
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ Single unified ActivityMonitoringRecords table created successfully';
END $$;

-- =====================================================
-- STEP 3: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- Common field indexes
CREATE INDEX "IX_ActivityMonitoringRecords_SerialNumber" 
    ON org."ActivityMonitoringRecords"("SerialNumber");
CREATE INDEX "IX_ActivityMonitoringRecords_LicenseNumber" 
    ON org."ActivityMonitoringRecords"("LicenseNumber");
CREATE INDEX "IX_ActivityMonitoringRecords_LicenseHolderName" 
    ON org."ActivityMonitoringRecords"("LicenseHolderName");
CREATE INDEX "IX_ActivityMonitoringRecords_District" 
    ON org."ActivityMonitoringRecords"("District");
CREATE INDEX "IX_ActivityMonitoringRecords_SectionType" 
    ON org."ActivityMonitoringRecords"("SectionType");
CREATE INDEX "IX_ActivityMonitoringRecords_Status" 
    ON org."ActivityMonitoringRecords"("Status");
CREATE INDEX "IX_ActivityMonitoringRecords_CreatedAt" 
    ON org."ActivityMonitoringRecords"("CreatedAt");

-- Date indexes for each section
CREATE INDEX "IX_ActivityMonitoringRecords_ReportRegistrationDate" 
    ON org."ActivityMonitoringRecords"("ReportRegistrationDate");
CREATE INDEX "IX_ActivityMonitoringRecords_ComplaintRegistrationDate" 
    ON org."ActivityMonitoringRecords"("ComplaintRegistrationDate");

-- JSONB index for deed items
CREATE INDEX "IX_ActivityMonitoringRecords_DeedItems" 
    ON org."ActivityMonitoringRecords" USING GIN ("DeedItems");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All performance indexes created successfully';
END $$;

-- =====================================================
-- STEP 4: ADD TABLE COMMENTS (Documentation)
-- =====================================================

COMMENT ON TABLE org."ActivityMonitoringRecords" IS 'نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان - Unified activity monitoring records';

-- Common fields
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SerialNumber" IS 'نمبر مسلسل - Auto-generated serial number';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseNumber" IS 'نمبر جواز - License number';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseHolderName" IS 'شهرت دارنده جواز - License holder name';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."District" IS 'ناحیه - District/Area';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ReportRegistrationDate" IS 'تاریخ ثبت گزارش - Report registration date';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SectionType" IS 'نوعیت ثبت - Section type: complaints, violations, inspection';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."DeedItems" IS 'سته‌های اسناد - JSON array of deed items with serial numbers';

-- Deed counts
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SaleDeedsCount" IS 'سته‌های فروش - Sale deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."RentalDeedsCount" IS 'سته‌های کرایی - Rental deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."BaiUlWafaDeedsCount" IS 'سته‌های بیع الوفا - Bai Ul Wafa deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."VehicleTransactionDeedsCount" IS 'سته‌های وسایط نقلیه - Vehicle transaction deeds count';

-- Complaints fields
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintRegistrationDate" IS 'تاریخ ثبت شکایت - Complaint registration date';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintSubject" IS 'موضوع شکایت - Complaint subject';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplainantName" IS 'شهرت عارض - Complainant name';

-- Violations fields
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationStatus" IS 'وضعیت تخلف - Violation status';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationType" IS 'نوعیت تخلف - Violation type';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ClosureReason" IS 'علت مسدودی - Closure reason';

-- Inspection fields
COMMENT ON COLUMN org."ActivityMonitoringRecords"."MonitoringType" IS 'نوعیت نظارت - Monitoring type';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Month" IS 'ماه - Month';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."MonitoringCount" IS 'تعداد نظارت - Monitoring count';

DO $$ 
BEGIN
    RAISE NOTICE '✓ Table and column comments added successfully';
END $$;

-- =====================================================
-- STEP 5: VERIFICATION
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
    AND table_name = 'ActivityMonitoringRecords';
    
    -- Count indexes
    SELECT COUNT(*) INTO index_count
    FROM pg_indexes 
    WHERE schemaname = 'org'
    AND tablename = 'ActivityMonitoringRecords';
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Activity Monitoring Module Recreation Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Single Unified Table Created:';
    RAISE NOTICE '  - org.ActivityMonitoringRecords';
    RAISE NOTICE '';
    RAISE NOTICE 'Section Types Supported:';
    RAISE NOTICE '  - complaints (ثبت شکایات)';
    RAISE NOTICE '  - violations (تخلفات دفاتر رهنمای معاملات)';
    RAISE NOTICE '  - inspection (نظارت و بررسی فعالیت)';
    RAISE NOTICE '';
    RAISE NOTICE 'Deed Types (in DeedItems JSONB):';
    RAISE NOTICE '  1 = سته‌های وسایط نقلیه (Vehicle Transaction Deeds)';
    RAISE NOTICE '  2 = سته‌های کرایی (Rental Deeds)';
    RAISE NOTICE '  3 = سته‌های فروش (Sale Deeds)';
    RAISE NOTICE '  4 = سته‌های بیع الوفا (Bai Ul Wafa Deeds)';
    RAISE NOTICE '';
    RAISE NOTICE 'Features:';
    RAISE NOTICE '  ✓ Single table design for simpler queries';
    RAISE NOTICE '  ✓ JSONB for flexible deed items storage';
    RAISE NOTICE '  ✓ Section type field for filtering';
    RAISE NOTICE '  ✓ All fields in one place';
    RAISE NOTICE '  ✓ Comprehensive indexing';
    RAISE NOTICE '  ✓ Full audit trail';
    RAISE NOTICE '';
    RAISE NOTICE 'Verification:';
    RAISE NOTICE '  - Table exists: %', CASE WHEN table_count > 0 THEN 'YES' ELSE 'NO' END;
    RAISE NOTICE '  - Indexes created: %', index_count;
    RAISE NOTICE '';
END $$;

-- =====================================================
-- STEP 6: SAMPLE QUERIES
-- =====================================================

-- View table structure
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name = 'ActivityMonitoringRecords'
ORDER BY ordinal_position;

-- View indexes
SELECT 
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'org'
AND tablename = 'ActivityMonitoringRecords'
ORDER BY indexname;

DO $$ 
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '✓ Activity Monitoring module with single table is ready!';
    RAISE NOTICE '';
END $$;
