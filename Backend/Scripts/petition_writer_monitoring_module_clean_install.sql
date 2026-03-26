-- =====================================================
-- Petition Writer Monitoring Module - Clean Installation Script
-- =====================================================
-- Purpose: Complete setup for production deployment
-- Schema: org (organization)
-- Date: 2026-03-26
-- 
-- Module: ثبت نظارت بر فعالیت عریضه نویسان
-- Sections:
--   1. ثبت شکایات (Complaints Registration)
--   2. تخلفات عریضه نویسان (Violations)
--   3. نظارت فعالیت عریضه نویسان (Monitoring Activities)
-- 
-- This script will:
--   - Drop existing tables if they exist
--   - Create tables with proper structure
--   - Create indexes for performance
--   - Add comments for documentation
--   - Add permissions to roles
-- =====================================================

-- =====================================================
-- STEP 1: DROP EXISTING TABLES
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE 'Starting Petition Writer Monitoring module setup...';
END $$;

-- Drop table if exists (cascade will also drop dependent objects)
DROP TABLE IF EXISTS org."PetitionWriterMonitoringRecords" CASCADE;

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
-- STEP 3: CREATE TABLE
-- =====================================================

-- PetitionWriterMonitoringRecords (Single table design - all sections in one table)
CREATE TABLE org."PetitionWriterMonitoringRecords" (
    "Id" SERIAL PRIMARY KEY,
    
    -- ============ Common Fields ============
    "SerialNumber" VARCHAR(50),
    "SectionType" VARCHAR(50) NOT NULL,  -- 'complaints', 'violations', 'monitoring'
    "RegistrationDate" DATE,
    
    -- ============ Section 1: Complaints Registration (ثبت شکایات) ============
    "ComplainantName" VARCHAR(200),
    "ComplaintSubject" VARCHAR(500),
    "ComplaintActionsTaken" VARCHAR(1000),
    "ComplaintRemarks" VARCHAR(1000),
    
    -- ============ Section 2: Violations (تخلفات عریضه نویسان) ============
    "PetitionWriterName" VARCHAR(200),
    "PetitionWriterLicenseNumber" VARCHAR(50),
    "PetitionWriterDistrict" VARCHAR(200),
    "ViolationType" VARCHAR(500),
    "ViolationActionsTaken" VARCHAR(1000),
    "ViolationRemarks" VARCHAR(1000),
    
    -- ============ Section 3: Monitoring Activities (نظارت فعالیت عریضه نویسان) ============
    "MonitoringYear" VARCHAR(20),
    "MonitoringMonth" VARCHAR(50),  -- Afghan calendar months: حمل، ثور، جوزا، etc.
    "MonitoringCount" INTEGER,
    "MonitoringRemarks" VARCHAR(1000),
    
    -- ============ Audit Fields ============
    "Status" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

DO $$ 
BEGIN
    RAISE NOTICE '✓ Table PetitionWriterMonitoringRecords created';
END $$;

-- =====================================================
-- STEP 4: CREATE INDEXES
-- =====================================================

-- Common fields indexes
CREATE INDEX "IX_PetitionWriterMonitoringRecords_SerialNumber" 
    ON org."PetitionWriterMonitoringRecords"("SerialNumber");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_SectionType" 
    ON org."PetitionWriterMonitoringRecords"("SectionType");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_RegistrationDate" 
    ON org."PetitionWriterMonitoringRecords"("RegistrationDate");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_Status" 
    ON org."PetitionWriterMonitoringRecords"("Status");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_CreatedAt" 
    ON org."PetitionWriterMonitoringRecords"("CreatedAt");

-- Violations section indexes
CREATE INDEX "IX_PetitionWriterMonitoringRecords_PetitionWriterName" 
    ON org."PetitionWriterMonitoringRecords"("PetitionWriterName");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_PetitionWriterLicenseNumber" 
    ON org."PetitionWriterMonitoringRecords"("PetitionWriterLicenseNumber");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_PetitionWriterDistrict" 
    ON org."PetitionWriterMonitoringRecords"("PetitionWriterDistrict");

-- Monitoring section indexes
CREATE INDEX "IX_PetitionWriterMonitoringRecords_MonitoringYear" 
    ON org."PetitionWriterMonitoringRecords"("MonitoringYear");
CREATE INDEX "IX_PetitionWriterMonitoringRecords_MonitoringMonth" 
    ON org."PetitionWriterMonitoringRecords"("MonitoringMonth");

DO $$ 
BEGIN
    RAISE NOTICE '✓ All indexes created';
END $$;

-- =====================================================
-- STEP 5: ADD TABLE AND COLUMN COMMENTS
-- =====================================================

-- Table comment
COMMENT ON TABLE org."PetitionWriterMonitoringRecords" IS 'نظارت بر فعالیت عریضه نویسان - Petition Writer Activity Monitoring';

-- Common fields
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."Id" IS 'شناسه - Primary key';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."SerialNumber" IS 'نمبر مسلسل - Auto-generated serial number';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."SectionType" IS 'نوعیت ثبت - Section type: complaints (ثبت شکایات), violations (تخلفات), monitoring (نظارت)';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."RegistrationDate" IS 'تاریخ ثبت - Registration date';

-- Complaints section
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ComplainantName" IS 'شهرت عارض - Complainant name';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ComplaintSubject" IS 'موضوع شکایت - Complaint subject';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ComplaintActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ComplaintRemarks" IS 'ملاحظات - Remarks';

-- Violations section
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."PetitionWriterName" IS 'شهرت عریضه نویس - Petition writer name';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."PetitionWriterLicenseNumber" IS 'نمبر جواز - License number';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."PetitionWriterDistrict" IS 'ناحیه - District';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ViolationType" IS 'نوعیت تخلف - Violation type';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ViolationActionsTaken" IS 'اجراآت - Actions taken';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ViolationRemarks" IS 'ملاحظات - Remarks';

-- Monitoring section
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."MonitoringYear" IS 'سال - Year';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."MonitoringMonth" IS 'ماه - Month (Afghan calendar: حمل، ثور، جوزا، سرطان، اسد، سنبله، میزان، عقرب، قوس، جدی، دلو، حوت)';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."MonitoringCount" IS 'تعداد نظارت - Monitoring count';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."MonitoringRemarks" IS 'ملاحظات - Remarks';

-- Audit fields
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."Status" IS 'وضعیت - Active/Inactive status (soft delete)';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."CreatedAt" IS 'تاریخ ایجاد - Creation timestamp';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."CreatedBy" IS 'ایجاد کننده - Creator user ID';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."UpdatedAt" IS 'تاریخ تغییر - Update timestamp';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."UpdatedBy" IS 'تغییر دهنده - Updater user ID';

DO $$ 
BEGIN
    RAISE NOTICE '✓ Table and column comments added';
END $$;

-- =====================================================
-- STEP 6: ADD PERMISSIONS TO ROLES
-- =====================================================

DO $$ 
DECLARE
    admin_role_id TEXT;
    authority_role_id TEXT;
    activity_monitoring_manager_role_id TEXT;
BEGIN
    -- Get role IDs
    SELECT "Id" INTO admin_role_id FROM public."AspNetRoles" WHERE "Name" = 'ADMIN';
    SELECT "Id" INTO authority_role_id FROM public."AspNetRoles" WHERE "Name" = 'AUTHORITY';
    SELECT "Id" INTO activity_monitoring_manager_role_id FROM public."AspNetRoles" WHERE "Name" = 'ACTIVITY_MONITORING_MANAGER';
    
    -- ============================================
    -- ADMIN: Full permissions (view, create, edit, delete)
    -- ============================================
    IF admin_role_id IS NOT NULL THEN
        -- View permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT admin_role_id, 'Permission', 'petitionwritermonitoring.view'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = admin_role_id AND "ClaimValue" = 'petitionwritermonitoring.view'
        );
        
        -- Create permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT admin_role_id, 'Permission', 'petitionwritermonitoring.create'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = admin_role_id AND "ClaimValue" = 'petitionwritermonitoring.create'
        );
        
        -- Edit permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT admin_role_id, 'Permission', 'petitionwritermonitoring.edit'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = admin_role_id AND "ClaimValue" = 'petitionwritermonitoring.edit'
        );
        
        -- Delete permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT admin_role_id, 'Permission', 'petitionwritermonitoring.delete'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = admin_role_id AND "ClaimValue" = 'petitionwritermonitoring.delete'
        );
        
        RAISE NOTICE '✓ ADMIN: All permissions added (view, create, edit, delete)';
    END IF;
    
    -- ============================================
    -- AUTHORITY: View only permission
    -- ============================================
    IF authority_role_id IS NOT NULL THEN
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT authority_role_id, 'Permission', 'petitionwritermonitoring.view'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = authority_role_id AND "ClaimValue" = 'petitionwritermonitoring.view'
        );
        
        RAISE NOTICE '✓ AUTHORITY: View permission added';
    END IF;
    
    -- ============================================
    -- ACTIVITY_MONITORING_MANAGER: Full permissions
    -- ============================================
    IF activity_monitoring_manager_role_id IS NOT NULL THEN
        -- View permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT activity_monitoring_manager_role_id, 'Permission', 'petitionwritermonitoring.view'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = activity_monitoring_manager_role_id AND "ClaimValue" = 'petitionwritermonitoring.view'
        );
        
        -- Create permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT activity_monitoring_manager_role_id, 'Permission', 'petitionwritermonitoring.create'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = activity_monitoring_manager_role_id AND "ClaimValue" = 'petitionwritermonitoring.create'
        );
        
        -- Edit permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT activity_monitoring_manager_role_id, 'Permission', 'petitionwritermonitoring.edit'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = activity_monitoring_manager_role_id AND "ClaimValue" = 'petitionwritermonitoring.edit'
        );
        
        -- Delete permission
        INSERT INTO public."AspNetRoleClaims" ("RoleId", "ClaimType", "ClaimValue")
        SELECT activity_monitoring_manager_role_id, 'Permission', 'petitionwritermonitoring.delete'
        WHERE NOT EXISTS (
            SELECT 1 FROM public."AspNetRoleClaims" 
            WHERE "RoleId" = activity_monitoring_manager_role_id AND "ClaimValue" = 'petitionwritermonitoring.delete'
        );
        
        RAISE NOTICE '✓ ACTIVITY_MONITORING_MANAGER: All permissions added (view, create, edit, delete)';
    END IF;
    
END $$;

DO $$ 
BEGIN
    RAISE NOTICE '✓ Permissions added to roles';
END $$;

-- =====================================================
-- STEP 7: VERIFICATION QUERIES
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
AND table_name = 'PetitionWriterMonitoringRecords'
ORDER BY ordinal_position;

-- Verify indexes
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'org'
AND tablename = 'PetitionWriterMonitoringRecords'
ORDER BY indexname;

-- Verify permissions
SELECT 
    r."Name" AS role_name,
    rc."ClaimValue" AS permission
FROM public."AspNetRoles" r
JOIN public."AspNetRoleClaims" rc ON r."Id" = rc."RoleId"
WHERE rc."ClaimType" = 'Permission'
AND rc."ClaimValue" LIKE 'petitionwritermonitoring%'
ORDER BY r."Name", rc."ClaimValue";

-- =====================================================
-- STEP 8: COMPLETION NOTICE
-- =====================================================

DO $$ 
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Petition Writer Monitoring Module Setup Complete!';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Table Created:';
    RAISE NOTICE '  - org."PetitionWriterMonitoringRecords"';
    RAISE NOTICE '';
    RAISE NOTICE 'Indexes Created: 10';
    RAISE NOTICE '  - SerialNumber, SectionType, RegistrationDate';
    RAISE NOTICE '  - Status, CreatedAt';
    RAISE NOTICE '  - PetitionWriterName, PetitionWriterLicenseNumber, PetitionWriterDistrict';
    RAISE NOTICE '  - MonitoringYear, MonitoringMonth';
    RAISE NOTICE '';
    RAISE NOTICE 'Permissions Added:';
    RAISE NOTICE '  - ADMIN: view, create, edit, delete';
    RAISE NOTICE '  - AUTHORITY: view only';
    RAISE NOTICE '  - ACTIVITY_MONITORING_MANAGER: view, create, edit, delete';
    RAISE NOTICE '';
    RAISE NOTICE 'Sections:';
    RAISE NOTICE '  1. ثبت شکایات (Complaints Registration)';
    RAISE NOTICE '  2. تخلفات عریضه نویسان (Violations)';
    RAISE NOTICE '  3. نظارت فعالیت عریضه نویسان (Monitoring Activities)';
    RAISE NOTICE '';
    RAISE NOTICE 'Next Steps:';
    RAISE NOTICE '  1. Restart backend application';
    RAISE NOTICE '  2. Log out and log back in to get updated JWT token';
    RAISE NOTICE '  3. Verify module appears in sidebar navigation';
    RAISE NOTICE '';
END $$;
