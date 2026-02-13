-- =====================================================
-- Petition Writer License Module - Clean Recreate Script
-- =====================================================
-- Purpose: Drop and recreate all petition writer license module tables
-- Schema: org (organization)
-- Date: 2026-02-10
-- 
-- WARNING: THIS SCRIPT WILL DELETE ALL PETITION WRITER LICENSE DATA!
-- =====================================================

-- INSTRUCTIONS:
-- 1. BACKUP YOUR DATABASE FIRST!
-- 2. This script is for TESTING/STAGING environments only
-- 3. Execute: psql -h localhost -U postgres -d PRMIS -f petition_writer_module_clean_recreate.sql
-- 4. Verify tables are created correctly

-- =====================================================
-- STEP 1: DROP ALL EXISTING TABLES
-- =====================================================

-- Drop tables in correct order (children first, then parents)
DROP TABLE IF EXISTS org."PetitionWriterRelocations" CASCADE;
DROP TABLE IF EXISTS org."PetitionWriterLicenses" CASCADE;

-- =====================================================
-- STEP 2: ENSURE SCHEMAS EXIST
-- =====================================================

CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS look;

-- =====================================================
-- STEP 3: CREATE TRANSACTION TABLES (org schema)
-- =====================================================

-- 1. PetitionWriterLicenses (Main petition writer license table)
CREATE TABLE org."PetitionWriterLicenses" (
    "Id" SERIAL PRIMARY KEY,
    
    -- License Identification
    "LicenseNumber" VARCHAR(50) NOT NULL,
    "ProvinceId" INTEGER,
    
    -- Tab 1: مشخصات عریضه‌نویس (Petition Writer Information)
    "ApplicantName" VARCHAR(200) NOT NULL,
    "ApplicantFatherName" VARCHAR(200),
    "ApplicantGrandFatherName" VARCHAR(200),
    "MobileNumber" VARCHAR(20),
    "Competency" VARCHAR(50),
    "ElectronicNationalIdNumber" VARCHAR(50) NOT NULL,
    
    -- Permanent Address (سکونت اصلی)
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" VARCHAR(500),
    
    -- Current Address (سکونت فعلی)
    "CurrentProvinceId" INTEGER,
    "CurrentDistrictId" INTEGER,
    "CurrentVillage" VARCHAR(500),
    
    -- Detailed Address (ادرس دقیق محل فعالیت)
    "DetailedAddress" VARCHAR(1000),
    
    -- Picture
    "PicturePath" VARCHAR(500),
    
    -- Tab 2: ثبت مالیه و مشخصات جواز (Financial and License Information)
    "BankReceiptNumber" VARCHAR(100),
    "BankReceiptDate" DATE,
    "District" VARCHAR(200),
    "LicenseType" VARCHAR(50),
    "LicensePrice" DECIMAL(18, 2),
    "LicenseIssueDate" DATE,
    "LicenseExpiryDate" DATE,
    
    -- Tab 3: لغو / انصراف (Cancellation/Withdrawal)
    "LicenseStatus" INTEGER DEFAULT 1,
    "CancellationDate" DATE,
    
    -- Soft delete
    "Status" BOOLEAN DEFAULT true,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_PetitionWriterLicenses_Province" 
        FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_PetitionWriterLicenses_PermanentProvince" 
        FOREIGN KEY ("PermanentProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_PetitionWriterLicenses_PermanentDistrict" 
        FOREIGN KEY ("PermanentDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_PetitionWriterLicenses_CurrentProvince" 
        FOREIGN KEY ("CurrentProvinceId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL,
    CONSTRAINT "FK_PetitionWriterLicenses_CurrentDistrict" 
        FOREIGN KEY ("CurrentDistrictId") 
        REFERENCES look."Location"("ID") 
        ON DELETE SET NULL
);

-- Add comments
COMMENT ON TABLE org."PetitionWriterLicenses" IS 'ثبت جواز عریضه‌نویسان - Petition Writer License Registration';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseNumber" IS 'نمبر جواز - License number (Format: PROVINCE_CODE-SEQUENTIAL_NUMBER)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ProvinceId" IS 'ولایت - Province for license numbering and access control';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantName" IS 'نام و تخلص - Name and surname';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantFatherName" IS 'ولد - Father name';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantGrandFatherName" IS 'نام پدر کلان - Grandfather name';
COMMENT ON COLUMN org."PetitionWriterLicenses"."MobileNumber" IS 'شماره تماس - Mobile phone number';
COMMENT ON COLUMN org."PetitionWriterLicenses"."Competency" IS 'اهلیت عریضه نویس - Competency level (high=اعلی, medium=اوسط, low=ادنی)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ElectronicNationalIdNumber" IS 'نمبر تذکره - Electronic National ID number';
COMMENT ON COLUMN org."PetitionWriterLicenses"."PermanentProvinceId" IS 'ولایت سکونت اصلی - Permanent address province';
COMMENT ON COLUMN org."PetitionWriterLicenses"."PermanentDistrictId" IS 'ولسوالی سکونت اصلی - Permanent address district';
COMMENT ON COLUMN org."PetitionWriterLicenses"."PermanentVillage" IS 'ناحیه / قریه سکونت اصلی - Permanent address village';
COMMENT ON COLUMN org."PetitionWriterLicenses"."CurrentProvinceId" IS 'ولایت سکونت فعلی - Current address province';
COMMENT ON COLUMN org."PetitionWriterLicenses"."CurrentDistrictId" IS 'ولسوالی سکونت فعلی - Current address district';
COMMENT ON COLUMN org."PetitionWriterLicenses"."CurrentVillage" IS 'ناحیه / قریه سکونت فعلی - Current address village';
COMMENT ON COLUMN org."PetitionWriterLicenses"."DetailedAddress" IS 'ادرس دقیق محل فعالیت - Detailed activity address';
COMMENT ON COLUMN org."PetitionWriterLicenses"."PicturePath" IS 'عکس - Photo path';
COMMENT ON COLUMN org."PetitionWriterLicenses"."BankReceiptNumber" IS 'نمبر رسید بانکی - Bank receipt number';
COMMENT ON COLUMN org."PetitionWriterLicenses"."BankReceiptDate" IS 'تاریخ پرداخت آویزه - Bank receipt date';
COMMENT ON COLUMN org."PetitionWriterLicenses"."District" IS 'ناحیه - District/area';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseType" IS 'نوعیت جواز - License type (new=جدید, renewal=تجدید, duplicate=مثنی)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicensePrice" IS 'قیمت جواز - License price (168 AFN for new, 85 AFN for renewal/duplicate)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseIssueDate" IS 'تاریخ صدور جواز - License issue date';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseExpiryDate" IS 'تاریخ ختم جواز - License expiry date (auto-calculated +1 year for new/renewal)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseStatus" IS 'وضعیت جواز - License status (1=Active, 2=Cancelled, 3=Withdrawn)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."CancellationDate" IS 'تاریخ لغو - Cancellation date';
COMMENT ON COLUMN org."PetitionWriterLicenses"."Status" IS 'Soft delete flag (true=active, false=deleted)';

-- 2. PetitionWriterRelocations (Relocation history table)
CREATE TABLE org."PetitionWriterRelocations" (
    "Id" SERIAL PRIMARY KEY,
    "PetitionWriterLicenseId" INTEGER NOT NULL,
    "NewActivityLocation" VARCHAR(500) NOT NULL,
    "RelocationDate" DATE,
    "Remarks" VARCHAR(1000),
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Keys
    CONSTRAINT "FK_PetitionWriterRelocations_License" 
        FOREIGN KEY ("PetitionWriterLicenseId") 
        REFERENCES org."PetitionWriterLicenses"("Id") 
        ON DELETE CASCADE
);

-- Add comments
COMMENT ON TABLE org."PetitionWriterRelocations" IS 'نقل مکان - Relocation history for petition writer licenses';
COMMENT ON COLUMN org."PetitionWriterRelocations"."PetitionWriterLicenseId" IS 'Foreign key to PetitionWriterLicenses';
COMMENT ON COLUMN org."PetitionWriterRelocations"."NewActivityLocation" IS 'محل فعالیت جدید - New activity location';
COMMENT ON COLUMN org."PetitionWriterRelocations"."RelocationDate" IS 'تاریخ نقل مکان - Relocation date';
COMMENT ON COLUMN org."PetitionWriterRelocations"."Remarks" IS 'ملاحظات - Additional remarks';

-- =====================================================
-- STEP 4: CREATE INDEXES FOR PERFORMANCE
-- =====================================================

-- PetitionWriterLicenses indexes
CREATE INDEX "IX_PetitionWriterLicenses_LicenseNumber" 
    ON org."PetitionWriterLicenses"("LicenseNumber");
CREATE INDEX "IX_PetitionWriterLicenses_ProvinceId" 
    ON org."PetitionWriterLicenses"("ProvinceId");
CREATE INDEX "IX_PetitionWriterLicenses_ApplicantName" 
    ON org."PetitionWriterLicenses"("ApplicantName");
CREATE INDEX "IX_PetitionWriterLicenses_ElectronicNationalIdNumber" 
    ON org."PetitionWriterLicenses"("ElectronicNationalIdNumber");
CREATE INDEX "IX_PetitionWriterLicenses_LicenseType" 
    ON org."PetitionWriterLicenses"("LicenseType");
CREATE INDEX "IX_PetitionWriterLicenses_LicenseStatus" 
    ON org."PetitionWriterLicenses"("LicenseStatus");
CREATE INDEX "IX_PetitionWriterLicenses_Status" 
    ON org."PetitionWriterLicenses"("Status");
CREATE INDEX "IX_PetitionWriterLicenses_LicenseIssueDate" 
    ON org."PetitionWriterLicenses"("LicenseIssueDate");
CREATE INDEX "IX_PetitionWriterLicenses_LicenseExpiryDate" 
    ON org."PetitionWriterLicenses"("LicenseExpiryDate");
CREATE INDEX "IX_PetitionWriterLicenses_CreatedAt" 
    ON org."PetitionWriterLicenses"("CreatedAt");
CREATE INDEX "IX_PetitionWriterLicenses_PermanentProvinceId" 
    ON org."PetitionWriterLicenses"("PermanentProvinceId");
CREATE INDEX "IX_PetitionWriterLicenses_CurrentProvinceId" 
    ON org."PetitionWriterLicenses"("CurrentProvinceId");

-- PetitionWriterRelocations indexes
CREATE INDEX "IX_PetitionWriterRelocations_PetitionWriterLicenseId" 
    ON org."PetitionWriterRelocations"("PetitionWriterLicenseId");
CREATE INDEX "IX_PetitionWriterRelocations_RelocationDate" 
    ON org."PetitionWriterRelocations"("RelocationDate");
CREATE INDEX "IX_PetitionWriterRelocations_CreatedAt" 
    ON org."PetitionWriterRelocations"("CreatedAt");

-- =====================================================
-- STEP 5: CREATE UNIQUE CONSTRAINTS
-- =====================================================

-- Ensure license numbers are unique (only for active records)
CREATE UNIQUE INDEX "UX_PetitionWriterLicenses_LicenseNumber_Active" 
    ON org."PetitionWriterLicenses"("LicenseNumber") 
    WHERE "Status" = true;

-- =====================================================
-- STEP 6: VERIFICATION QUERIES
-- =====================================================

-- Check if tables exist
SELECT 'Tables Created:' AS status;
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'org' 
AND table_name IN ('PetitionWriterLicenses', 'PetitionWriterRelocations')
ORDER BY table_name;

-- Check column count
SELECT 
    'PetitionWriterLicenses' AS table_name,
    COUNT(*) AS column_count
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name = 'PetitionWriterLicenses'
UNION ALL
SELECT 
    'PetitionWriterRelocations' AS table_name,
    COUNT(*) AS column_count
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name = 'PetitionWriterRelocations';

-- Check indexes created
SELECT 'Indexes Created:' AS status;
SELECT 
    indexname,
    tablename
FROM pg_indexes
WHERE schemaname = 'org'
AND tablename LIKE '%PetitionWriter%'
ORDER BY tablename, indexname;

-- Check foreign keys
SELECT 'Foreign Keys Created:' AS status;
SELECT
    tc.constraint_name,
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
AND tc.table_schema = 'org'
AND tc.table_name LIKE '%PetitionWriter%'
ORDER BY tc.table_name, tc.constraint_name;

-- List all created tables with sizes
SELECT 'Table Sizes:' AS status;
SELECT 
    schemaname AS "Schema",
    tablename AS "Table Name",
    pg_size_pretty(pg_total_relation_size('"' || schemaname || '"."' || tablename || '"')) AS "Size"
FROM pg_tables
WHERE schemaname = 'org'
AND tablename LIKE '%PetitionWriter%'
ORDER BY tablename;

-- =====================================================
-- COMPLETION MESSAGE
-- =====================================================

SELECT '========================================' AS message
UNION ALL SELECT 'Petition Writer License Module Recreation Complete!'
UNION ALL SELECT '========================================'
UNION ALL SELECT ''
UNION ALL SELECT 'Tables Created:'
UNION ALL SELECT '  ✓ org.PetitionWriterLicenses (30 columns)'
UNION ALL SELECT '  ✓ org.PetitionWriterRelocations (6 columns)'
UNION ALL SELECT ''
UNION ALL SELECT 'New Fields Included:'
UNION ALL SELECT '  ✓ MobileNumber (شماره تماس)'
UNION ALL SELECT '  ✓ Competency (اهلیت: high/medium/low)'
UNION ALL SELECT '  ✓ District (ناحیه)'
UNION ALL SELECT '  ✓ LicenseType (نوعیت جواز: new/renewal/duplicate)'
UNION ALL SELECT '  ✓ LicensePrice (قیمت جواز: 168 or 85 AFN)'
UNION ALL SELECT '  ✓ LicenseExpiryDate (تاریخ ختم جواز)'
UNION ALL SELECT '  ✓ DetailedAddress (ادرس دقیق محل فعالیت)'
UNION ALL SELECT ''
UNION ALL SELECT 'Next Steps:'
UNION ALL SELECT '  1. Restart backend: cd Backend && dotnet run'
UNION ALL SELECT '  2. Clear browser cache (Ctrl+Shift+Delete)'
UNION ALL SELECT '  3. Test in frontend - create new license'
UNION ALL SELECT '  4. Verify date auto-calculation works (+1 year)'
UNION ALL SELECT '  5. Verify price auto-population works'
UNION ALL SELECT '  6. Test print certificate'
UNION ALL SELECT ''
UNION ALL SELECT '========================================';

-- =====================================================
-- END OF SCRIPT
-- =====================================================
