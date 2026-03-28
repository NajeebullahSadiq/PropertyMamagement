-- =====================================================
-- Activity Monitoring Module - Single Table Migration
-- =====================================================
-- Purpose: Migrate from multi-table to single-table design
-- Schema: org (organization)
-- Date: 2026-03-28
-- 
-- This script will:
--   - Drop existing multi-table structure
--   - Create single table with all sections
--   - Create indexes for performance
--   - Add comments for documentation
-- =====================================================

-- =====================================================
-- STEP 1: DROP EXISTING TABLES
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE 'Starting Activity Monitoring single table migration...';
END $$;

-- Drop all existing tables (cascade will also drop dependent objects)
DROP TABLE IF EXISTS org."ActivityMonitoringInspections" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringDeedItems" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringPetitionWriterViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRealEstateViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringComplaints" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRecords" CASCADE;

DO $$ 
BEGIN
    RAISE NOTICE '✓ Existing tables dropped (if any)';
END $$;

-- =====================================================
-- STEP 2: ENSURE SCHEMA EXISTS
-- =====================================================

CREATE SCHEMA IF NOT EXISTS org;

DO $$ 
BEGIN
    RAISE NOTICE '✓ Schema org verified';
END $$;

-- =====================================================
-- STEP 3: CREATE SINGLE TABLE
-- =====================================================

-- ActivityMonitoringRecords (Single table design - all sections in one table)
CREATE TABLE org."ActivityMonitoringRecords" (
    "Id" SERIAL PRIMARY KEY,
    
    -- ============ Common Fields ============
    "SerialNumber" VARCHAR(50),
    "LicenseNumber" VARCHAR(50),
    "LicenseHolderName" VARCHAR(200),
    "District" VARCHAR(200),
    "SectionType" VARCHAR(50) NOT NULL,  -- 'annualReport', 'complaints', 'violations', 'inspection'
    "ReportRegistrationDate" DATE,
    
    -- ============ Section 1: Annual Report (گزارش سالانه) ============
    "SaleDeedsCount" INTEGER,
    "RentalDeedsCount" INTEGER,
    "BaiUlWafaDeedsCount" INTEGER,
    "VehicleTransactionDeedsCount" INTEGER,
    "AnnualReportRemarks" VARCHAR(1000),
    
    -- Deed Items as JSONB (flexible array storage)
    "DeedItems" JSONB,
    
    -- ============ Section 2: Complaints (ثبت شکایات) ============
    "ComplaintRegistrationDate" DATE,
    "ComplaintSubject" VARCHAR(500),
    "ComplainantName" VARCHAR(200),
    "ComplaintActionsTaken" VARCHAR(1000),
    "ComplaintRemarks" VARCHAR(1000),
    
    -- ============ Section 3: Violations (تخلفات دفاتر رهنمای معاملات) ============
    "ViolationStatus" VARCHAR(100),
    "ViolationType" VARCHAR(500),
    "ViolationDate" DATE,
    "ClosureDate" DATE,
    "ClosureReason" VARCHAR(500),
    "ViolationActionsTaken" VARCHAR(1000),
    "ViolationRemarks" VARCHAR(1000),
    
    -- ============ Section 4: Inspections (نظارت و بازرسی) ============
    "MonitoringType" VARCHAR(100),
    "Year" VARCHAR(20),
    "Month" VARCHAR(50),
    "MonitoringCount" INTEGER,
    
    -- ============ Audit Fields ============
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ Table ActivityMonitoringRecords created';
END $$;

-- =====================================================
-- STEP 4: CREATE INDEXES
-- =====================================================

-- Common fields indexes
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
CREATE INDEX "IX_ActivityMonitoringRecords_ReportRegistrationDate" 
    ON org."ActivityMonitoringRecords"("ReportRegistrationDate");
CREATE INDEX "IX_ActivityMonitoringRecords_Status" 
    ON org."ActivityMonitoringRecords"("Status");
CREATE INDEX "IX_ActivityMonitoringRecords_CreatedAt" 
    ON org."ActivityMonitoringRecords"("CreatedAt");

-- Violations section indexes
CREATE INDEX "IX_ActivityMonitoringRecords_ViolationStatus" 
    ON org."ActivityMonitoringRecords"("ViolationStatus");
CREATE INDEX "IX_ActivityMonitoringRecords_ViolationDate" 
    ON org."ActivityMonitoringRecords"("ViolationDate");

-- Inspection section indexes
CREATE INDEX "IX_ActivityMonitoringRecords_MonitoringType" 
    ON org."ActivityMonitoringRecords"("MonitoringType");
CREATE INDEX "IX_ActivityMonitoringRecords_Year" 
    ON org."ActivityMonitoringRecords"("Year");
CREATE INDEX "IX_ActivityMonitoringRecords_Month" 
    ON org."ActivityMonitoringRecords"("Month");

-- JSONB index for DeedItems
CREATE INDEX "IX_ActivityMonitoringRecords_DeedItems" 
    ON org."ActivityMonitoringRecords" USING GIN ("DeedItems");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All indexes created';
END $$;

-- =====================================================
-- STEP 5: ADD TABLE AND COLUMN COMMENTS
-- =====================================================

-- Table comment
COMMENT ON TABLE org."ActivityMonitoringRecords" IS 'نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان - Activity Monitoring Single Table Design';

-- Common fields
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Id" IS 'شناسه - Primary key';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SerialNumber" IS 'نمبر مسلسل - Auto-generated serial number';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseNumber" IS 'نمبر جواز - License number';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseHolderName" IS 'شهرت دارنده جواز - License holder name';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."District" IS 'ناحیه - District';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SectionType" IS 'نوعیت ثبت - Section type: annualReport (گزارش سالانه), complaints (ثبت شکایات), violations (تخلفات), inspection (نظارت)';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ReportRegistrationDate" IS 'تاریخ ثبت - Report registration date';

-- Annual Report section
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SaleDeedsCount" IS 'تعداد سته‌های فروش - Sale deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."RentalDeedsCount" IS 'تعداد سته‌های کرایی - Rental deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."BaiUlWafaDeedsCount" IS 'تعداد سته‌های بیع الوفا - Bai Ul Wafa deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."VehicleTransactionDeedsCount" IS 'تعداد سته‌های وسایط نقلیه - Vehicle transaction deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."AnnualReportRemarks" IS 'ملاحظات گزارش سالانه - Annual report remarks';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."DeedItems" IS 'سته‌های اسناد - Deed items with serial numbers (JSONB array)';

-- Complaints section
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintRegistrationDate" IS 'تاریخ ثبت شکایت - Complaint registration date';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintSubject" IS 'موضوع شکایت - Complaint subject';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplainantName" IS 'شهرت عارض - Complainant name';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintRemarks" IS 'ملاحظات - Remarks';

-- Violations section
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationStatus" IS 'وضعیت تخلف - Violation status: منجر به مسدودی, عادی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationType" IS 'نوعیت تخلف - Violation type';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationDate" IS 'تاریخ ثبت تخلف - Violation date';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ClosureDate" IS 'تاریخ مسدودی - Closure date';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ClosureReason" IS 'علت مسدودی - Closure reason';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationRemarks" IS 'ملاحظات - Remarks';

-- Inspection section
COMMENT ON COLUMN org."ActivityMonitoringRecords"."MonitoringType" IS 'نوعیت نظارت - Monitoring type';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Year" IS 'سال - Year';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Month" IS 'ماه - Month (Afghan calendar: حمل، ثور، جوزا، سرطان، اسد، سنبله، میزان، عقرب، قوس، جدی، دلو، حوت)';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."MonitoringCount" IS 'تعداد نظارت - Monitoring count';

-- Audit fields
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Status" IS 'وضعیت - Active/Inactive status (soft delete)';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."CreatedAt" IS 'تاریخ ایجاد - Creation timestamp';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."CreatedBy" IS 'ایجاد کننده - Creator user ID';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."UpdatedAt" IS 'تاریخ تغییر - Update timestamp';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."UpdatedBy" IS 'تغییر دهنده - Updater user ID';

DO $$ 
BEGIN
    RAISE NOTICE '✓ Table and column comments added';
END $$;

-- =====================================================
-- STEP 6: VERIFICATION QUERIES
-- =====================================================

-- Verify table structure
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name = 'ActivityMonitoringRecords'
ORDER BY ordinal_position;

-- Verify indexes
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'org'
AND tablename = 'ActivityMonitoringRecords'
ORDER BY indexname;

-- =====================================================
-- STEP 7: COMPLETION NOTICE
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Activity Monitoring Single Table Migration Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Table Created:';
    RAISE NOTICE '  - org."ActivityMonitoringRecords" (single table design)';
    RAISE NOTICE '';
    RAISE NOTICE 'Indexes Created: 14';
    RAISE NOTICE '  - SerialNumber, LicenseNumber, LicenseHolderName, District';
    RAISE NOTICE '  - SectionType, ReportRegistrationDate, Status, CreatedAt';
    RAISE NOTICE '  - ViolationStatus, ViolationDate';
    RAISE NOTICE '  - MonitoringType, Year, Month';
    RAISE NOTICE '  - DeedItems (GIN index for JSONB)';
    RAISE NOTICE '';
    RAISE NOTICE 'Sections:';
    RAISE NOTICE '  1. گزارش سالانه (Annual Report)';
    RAISE NOTICE '  2. ثبت شکایات (Complaints)';
    RAISE NOTICE '  3. تخلفات دفاتر رهنمای معاملات (Violations)';
    RAISE NOTICE '  4. نظارت و بازرسی (Inspection)';
    RAISE NOTICE '';
    RAISE NOTICE 'Features:';
    RAISE NOTICE '  ✓ Single table design - simplified structure';
    RAISE NOTICE '  ✓ JSONB for flexible deed items storage';
    RAISE NOTICE '  ✓ All sections in one table';
    RAISE NOTICE '  ✓ Comprehensive indexing';
    RAISE NOTICE '  ✓ Full audit trail';
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Restart backend application';
    RAISE NOTICE '  2. Test activity monitoring form';
    RAISE NOTICE '  3. Verify all sections work correctly';
    RAISE NOTICE '';
END $$;
