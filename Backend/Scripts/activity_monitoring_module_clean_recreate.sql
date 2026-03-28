-- =====================================================
-- Activity Monitoring Module Clean Recreation Script
-- =====================================================
-- Purpose: Drop and recreate all activity monitoring module tables
-- Schema: org (organization)
-- Date: 2026-03-15
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
DROP TABLE IF EXISTS org."ActivityMonitoringDeedItems" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringPetitionWriterViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRealEstateViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringComplaints" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRecords" CASCADE;

DO $$ 
BEGIN
    RAISE NOTICE '✓ All activity monitoring module tables dropped successfully';
END $$;

-- =====================================================
-- STEP 2: ENSURE SCHEMAS EXIST
-- =====================================================

CREATE SCHEMA IF NOT EXISTS org;

-- =====================================================
-- STEP 3: CREATE ACTIVITY MONITORING TABLES (org schema)
-- =====================================================

-- 1. ActivityMonitoringRecords (Main table)
CREATE TABLE org."ActivityMonitoringRecords" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Section 1: Annual Report (گزارش سالانه)
    "SerialNumber" VARCHAR(50),
    "LicenseNumber" VARCHAR(50) NOT NULL,
    "LicenseHolderName" VARCHAR(200) NOT NULL,
    "District" VARCHAR(200),
    "ReportRegistrationDate" DATE,
    "SaleDeedsCount" INTEGER DEFAULT 0,
    "RentalDeedsCount" INTEGER DEFAULT 0,
    "BaiUlWafaDeedsCount" INTEGER DEFAULT 0,
    "VehicleTransactionDeedsCount" INTEGER DEFAULT 0,
    "AnnualReportRemarks" VARCHAR(1000),
    
    -- Section 5: Inspection & Supervision Summary
    "InspectionDate" DATE,
    "InspectedRealEstateOfficesCount" INTEGER,
    "SealedOfficesCount" INTEGER,
    "InspectedPetitionWritersCount" INTEGER,
    "ViolatingPetitionWritersCount" INTEGER,
    
    -- Audit fields
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

-- 2. ActivityMonitoringDeedItems (Deed items with serial numbers)
CREATE TABLE org."ActivityMonitoringDeedItems" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    
    -- Deed information
    "DeedType" INTEGER NOT NULL,
    "SerialStart" VARCHAR(100),
    "SerialEnd" VARCHAR(100),
    "Count" INTEGER NOT NULL DEFAULT 0,
    
    -- Audit fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_ActivityMonitoringDeedItems_ActivityMonitoringRecord" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- 3. ActivityMonitoringComplaints (Section 2: Complaints Registration)
CREATE TABLE org."ActivityMonitoringComplaints" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    
    -- Complaint information
    "ComplaintSerialNumber" VARCHAR(50),
    "ComplainantName" VARCHAR(200) NOT NULL,
    "ComplaintSubject" VARCHAR(500) NOT NULL,
    "ComplaintRegistrationDate" DATE,
    "AccusedPartyName" VARCHAR(200) NOT NULL,
    "ActionsTaken" VARCHAR(1000),
    "Remarks" VARCHAR(1000),
    
    -- Audit fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_ActivityMonitoringComplaints_ActivityMonitoringRecord" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- 4. ActivityMonitoringRealEstateViolations (Section 3: Real Estate Violations)
CREATE TABLE org."ActivityMonitoringRealEstateViolations" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    
    -- Violation information
    "ViolationSerialNumber" VARCHAR(50),
    "LicenseHolderName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50),
    "District" VARCHAR(200),
    "ViolationStatus" VARCHAR(100),
    "ViolationType" VARCHAR(500),
    "ViolationDate" DATE,
    "ClosureDate" DATE,
    "ClosureReason" VARCHAR(500),
    "ActionsTaken" VARCHAR(1000),
    "Remarks" VARCHAR(1000),
    
    -- Audit fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecord" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- 5. ActivityMonitoringPetitionWriterViolations (Section 4: Petition Writer Violations)
CREATE TABLE org."ActivityMonitoringPetitionWriterViolations" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    
    -- Violation information
    "ViolationSerialNumber" VARCHAR(50),
    "PetitionWriterName" VARCHAR(200) NOT NULL,
    "LicenseNumber" VARCHAR(50),
    "District" VARCHAR(200),
    "ViolationType" VARCHAR(500) NOT NULL,
    "ViolationDate" DATE,
    "ActionsTaken" VARCHAR(1000),
    "Remarks" VARCHAR(1000),
    
    -- Audit fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecord" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- 6. ActivityMonitoringInspections (Section 5: Inspections)
CREATE TABLE org."ActivityMonitoringInspections" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    
    -- Inspection information
    "MonitoringType" VARCHAR(100),
    "Year" VARCHAR(20),
    "Month" VARCHAR(50),
    "MonitoringCount" INTEGER,
    "SealedCount" INTEGER,
    "ViolatingCount" INTEGER,
    "Remarks" VARCHAR(1000),
    
    -- Audit fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_ActivityMonitoringInspections_ActivityMonitoringRecord" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ All 6 activity monitoring module tables created successfully';
END $$;

-- =====================================================
-- STEP 4: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- ActivityMonitoringRecords indexes
CREATE INDEX "IX_ActivityMonitoringRecords_SerialNumber" 
    ON org."ActivityMonitoringRecords"("SerialNumber");
CREATE INDEX "IX_ActivityMonitoringRecords_LicenseNumber" 
    ON org."ActivityMonitoringRecords"("LicenseNumber");
CREATE INDEX "IX_ActivityMonitoringRecords_LicenseHolderName" 
    ON org."ActivityMonitoringRecords"("LicenseHolderName");
CREATE INDEX "IX_ActivityMonitoringRecords_District" 
    ON org."ActivityMonitoringRecords"("District");
CREATE INDEX "IX_ActivityMonitoringRecords_ReportRegistrationDate" 
    ON org."ActivityMonitoringRecords"("ReportRegistrationDate");
CREATE INDEX "IX_ActivityMonitoringRecords_Status" 
    ON org."ActivityMonitoringRecords"("Status");
CREATE INDEX "IX_ActivityMonitoringRecords_CreatedAt" 
    ON org."ActivityMonitoringRecords"("CreatedAt");

-- ActivityMonitoringDeedItems indexes
CREATE INDEX "IX_ActivityMonitoringDeedItems_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringDeedItems"("ActivityMonitoringRecordId");
CREATE INDEX "IX_ActivityMonitoringDeedItems_DeedType" 
    ON org."ActivityMonitoringDeedItems"("DeedType");

-- ActivityMonitoringComplaints indexes
CREATE INDEX "IX_ActivityMonitoringComplaints_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringComplaints"("ActivityMonitoringRecordId");
CREATE INDEX "IX_ActivityMonitoringComplaints_ComplaintSerialNumber" 
    ON org."ActivityMonitoringComplaints"("ComplaintSerialNumber");
CREATE INDEX "IX_ActivityMonitoringComplaints_ComplainantName" 
    ON org."ActivityMonitoringComplaints"("ComplainantName");
CREATE INDEX "IX_ActivityMonitoringComplaints_ComplaintRegistrationDate" 
    ON org."ActivityMonitoringComplaints"("ComplaintRegistrationDate");

-- ActivityMonitoringRealEstateViolations indexes
CREATE INDEX "IX_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringRealEstateViolations"("ActivityMonitoringRecordId");
CREATE INDEX "IX_ActivityMonitoringRealEstateViolations_ViolationSerialNumber" 
    ON org."ActivityMonitoringRealEstateViolations"("ViolationSerialNumber");
CREATE INDEX "IX_ActivityMonitoringRealEstateViolations_LicenseHolderName" 
    ON org."ActivityMonitoringRealEstateViolations"("LicenseHolderName");
CREATE INDEX "IX_ActivityMonitoringRealEstateViolations_LicenseNumber" 
    ON org."ActivityMonitoringRealEstateViolations"("LicenseNumber");
CREATE INDEX "IX_ActivityMonitoringRealEstateViolations_ViolationDate" 
    ON org."ActivityMonitoringRealEstateViolations"("ViolationDate");

-- ActivityMonitoringPetitionWriterViolations indexes
CREATE INDEX "IX_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringPetitionWriterViolations"("ActivityMonitoringRecordId");
CREATE INDEX "IX_ActivityMonitoringPetitionWriterViolations_ViolationSerialNumber" 
    ON org."ActivityMonitoringPetitionWriterViolations"("ViolationSerialNumber");
CREATE INDEX "IX_ActivityMonitoringPetitionWriterViolations_PetitionWriterName" 
    ON org."ActivityMonitoringPetitionWriterViolations"("PetitionWriterName");
CREATE INDEX "IX_ActivityMonitoringPetitionWriterViolations_ViolationDate" 
    ON org."ActivityMonitoringPetitionWriterViolations"("ViolationDate");

-- ActivityMonitoringInspections indexes
CREATE INDEX "IX_ActivityMonitoringInspections_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringInspections"("ActivityMonitoringRecordId");
CREATE INDEX "IX_ActivityMonitoringInspections_MonitoringType" 
    ON org."ActivityMonitoringInspections"("MonitoringType");
CREATE INDEX "IX_ActivityMonitoringInspections_Year" 
    ON org."ActivityMonitoringInspections"("Year");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All performance indexes created successfully';
END $$;

-- =====================================================
-- STEP 5: ADD TABLE COMMENTS (Documentation)
-- =====================================================

COMMENT ON TABLE org."ActivityMonitoringRecords" IS 'نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان - Main activity monitoring records';
COMMENT ON TABLE org."ActivityMonitoringDeedItems" IS 'سته‌های اسناد - Deed items with serial numbers for annual report';
COMMENT ON TABLE org."ActivityMonitoringComplaints" IS 'ثبت شکایات - Complaints Registration';
COMMENT ON TABLE org."ActivityMonitoringRealEstateViolations" IS 'تخلفات دفاتر رهنمای معاملات - Real Estate Office Violations';
COMMENT ON TABLE org."ActivityMonitoringPetitionWriterViolations" IS 'تخلفات عریضه نویسان - Petition Writer Violations';
COMMENT ON TABLE org."ActivityMonitoringInspections" IS 'نظارت و بازرسی - Inspections';

-- ActivityMonitoringRecords column comments
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SerialNumber" IS 'نمبر مسلسل - Auto-generated serial number';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseNumber" IS 'نمبر جواز - License number (used for company search)';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseHolderName" IS 'شهرت دارنده جواز - License holder name';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."District" IS 'ناحیه - District/Area';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ReportRegistrationDate" IS 'تاریخ ثبت گزارش - Report registration date';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SaleDeedsCount" IS 'سته‌های فروش - Sale deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."RentalDeedsCount" IS 'سته‌های کرایی - Rental deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."BaiUlWafaDeedsCount" IS 'سته‌های بیع الوفا - Bai Ul Wafa deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."VehicleTransactionDeedsCount" IS 'سته‌های وسایط نقلیه - Vehicle transaction deeds count';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."AnnualReportRemarks" IS 'ملاحظات - Remarks';

-- ActivityMonitoringDeedItems column comments
COMMENT ON COLUMN org."ActivityMonitoringDeedItems"."DeedType" IS 'نوعیت سته - Deed type: 1=Vehicle, 2=Rental, 3=Sale, 4=Bai Ul Wafa';
COMMENT ON COLUMN org."ActivityMonitoringDeedItems"."SerialStart" IS 'آغاز سریال نمبر - Starting serial number';
COMMENT ON COLUMN org."ActivityMonitoringDeedItems"."SerialEnd" IS 'ختم سریال نمبر - Ending serial number';
COMMENT ON COLUMN org."ActivityMonitoringDeedItems"."Count" IS 'تعداد - Quantity of deeds';

-- ActivityMonitoringComplaints column comments
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplaintSerialNumber" IS 'نمبر مسلسل شکایت - Complaint serial number';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplainantName" IS 'شهرت عارض - Complainant name';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplaintSubject" IS 'موضوع شکایت - Complaint subject';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplaintRegistrationDate" IS 'تاریخ ثبت شکایت - Complaint registration date';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."AccusedPartyName" IS 'شهرت معروض علیه - Accused party name';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."Remarks" IS 'ملاحظات - Remarks';

-- ActivityMonitoringRealEstateViolations column comments
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationSerialNumber" IS 'نمبر مسلسل تخلف - Violation serial number';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."LicenseHolderName" IS 'شهرت دارنده جواز - License holder name';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."LicenseNumber" IS 'نمبر جواز - License number';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."District" IS 'ناحیه - District';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationStatus" IS 'وضعیت تخلف - Violation status';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationType" IS 'نوعیت تخلف - Violation type';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationDate" IS 'تاریخ ثبت تخلف - Violation date';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ClosureDate" IS 'تاریخ توقف - Closure date';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ClosureReason" IS 'دلیل توقف - Closure reason';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."Remarks" IS 'ملاحظات - Remarks';

-- ActivityMonitoringPetitionWriterViolations column comments
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ViolationSerialNumber" IS 'نمبر مسلسل تخلف - Violation serial number';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."PetitionWriterName" IS 'شهرت عریضه نویس - Petition writer name';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."LicenseNumber" IS 'نمبر جواز - License number';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."District" IS 'ناحیه - District';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ViolationType" IS 'نوعیت تخلف - Violation type';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ViolationDate" IS 'تاریخ ثبت تخلف - Violation date';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."Remarks" IS 'ملاحظات - Remarks';

-- ActivityMonitoringInspections column comments
COMMENT ON COLUMN org."ActivityMonitoringInspections"."MonitoringType" IS 'نوعیت نظارت - Monitoring type';
COMMENT ON COLUMN org."ActivityMonitoringInspections"."Year" IS 'سال - Year';
COMMENT ON COLUMN org."ActivityMonitoringInspections"."Month" IS 'ماه - Month';
COMMENT ON COLUMN org."ActivityMonitoringInspections"."MonitoringCount" IS 'تعداد نظارت - Monitoring count';
COMMENT ON COLUMN org."ActivityMonitoringInspections"."SealedCount" IS 'تعداد مهرولاک شده - Sealed count';
COMMENT ON COLUMN org."ActivityMonitoringInspections"."ViolatingCount" IS 'تعداد متخلفین - Violating count';
COMMENT ON COLUMN org."ActivityMonitoringInspections"."Remarks" IS 'ملاحظات - Remarks';

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
        'ActivityMonitoringRecords', 'ActivityMonitoringDeedItems',
        'ActivityMonitoringComplaints', 'ActivityMonitoringRealEstateViolations',
        'ActivityMonitoringPetitionWriterViolations', 'ActivityMonitoringInspections'
    );
    
    -- Count indexes
    SELECT COUNT(*) INTO index_count
    FROM pg_indexes 
    WHERE schemaname = 'org'
    AND (tablename LIKE '%ActivityMonitoring%');
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Activity Monitoring Module Recreation Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Tables Created:';
    RAISE NOTICE '  - org.ActivityMonitoringRecords (main)';
    RAISE NOTICE '  - org.ActivityMonitoringDeedItems (deed items with serials)';
    RAISE NOTICE '  - org.ActivityMonitoringComplaints';
    RAISE NOTICE '  - org.ActivityMonitoringRealEstateViolations';
    RAISE NOTICE '  - org.ActivityMonitoringPetitionWriterViolations';
    RAISE NOTICE '  - org.ActivityMonitoringInspections';
    RAISE NOTICE '';
    RAISE NOTICE 'Deed Types Supported:';
    RAISE NOTICE '  1 = سته‌های وسایط نقلیه (Vehicle Transaction Deeds)';
    RAISE NOTICE '  2 = سته‌های کرایی (Rental Deeds)';
    RAISE NOTICE '  3 = سته‌های فروش (Sale Deeds)';
    RAISE NOTICE '  4 = سته‌های بیع الوفا (Bai Ul Wafa Deeds)';
    RAISE NOTICE '';
    RAISE NOTICE 'Features Included:';
    RAISE NOTICE '  ✓ Company search by license number';
    RAISE NOTICE '  ✓ Auto-populate license holder name';
    RAISE NOTICE '  ✓ Auto-populate district (ناحیه)';
    RAISE NOTICE '  ✓ Dynamic deed items collection';
    RAISE NOTICE '  ✓ Serial number tracking for each deed type';
    RAISE NOTICE '  ✓ Automatic count calculation';
    RAISE NOTICE '  ✓ Complaints registration';
    RAISE NOTICE '  ✓ Real estate violations tracking';
    RAISE NOTICE '  ✓ Petition writer violations tracking';
    RAISE NOTICE '  ✓ Inspections tracking';
    RAISE NOTICE '  ✓ Comprehensive indexing';
    RAISE NOTICE '  ✓ Full audit trail';
    RAISE NOTICE '';
    RAISE NOTICE 'Verification:';
    RAISE NOTICE '  - Tables created: % / 6', table_count;
    RAISE NOTICE '  - Indexes created: %', index_count;
    RAISE NOTICE '';
    RAISE NOTICE 'Important Notes:';
    RAISE NOTICE '  - LicenseNumber field added for company search';
    RAISE NOTICE '  - District field added for area tracking';
    RAISE NOTICE '  - DeedItems table for serial number tracking';
    RAISE NOTICE '  - Clean structure matching current code';
    RAISE NOTICE '  - Ready for production deployment';
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Verify table structure above';
    RAISE NOTICE '  2. Test activity monitoring form in frontend';
    RAISE NOTICE '  3. Test company search functionality';
    RAISE NOTICE '  4. Verify all fields save correctly';
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
AND table_name LIKE '%ActivityMonitoring%'
ORDER BY table_name, ordinal_position;

-- Check indexes
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'org'
AND tablename LIKE '%ActivityMonitoring%'
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
AND tc.table_name LIKE '%ActivityMonitoring%'
ORDER BY tc.table_name;

DO $$ 
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '✓ Activity Monitoring module is ready for use!';
    RAISE NOTICE '';
END $$;
