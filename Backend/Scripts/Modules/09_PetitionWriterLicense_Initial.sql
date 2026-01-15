-- =============================================
-- Module: Petition Writer License (ثبت جواز عریضه‌نویسان)
-- Schema: org
-- Date: 2026-01-15
-- Description: Creates tables for petition writer license management
-- Dependencies: Shared module (Location)
-- =============================================

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- =============================================
-- Table: PetitionWriterLicenses
-- Description: Main table for petition writer licenses
-- =============================================
CREATE TABLE IF NOT EXISTS org."PetitionWriterLicenses" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Tab 1: مشخصات عریضه‌نویس
    "LicenseNumber" VARCHAR(50) NOT NULL,
    "ApplicantName" VARCHAR(200) NOT NULL,
    "ApplicantFatherName" VARCHAR(200),
    "ApplicantGrandFatherName" VARCHAR(200),
    
    -- Identity Card Type (1 = Electronic, 2 = Paper)
    "IdentityCardType" INTEGER NOT NULL DEFAULT 1,
    
    -- Electronic ID fields
    "ElectronicIdNumber" VARCHAR(50),
    
    -- Paper ID fields
    "PaperIdNumber" VARCHAR(50),
    "PaperIdVolume" VARCHAR(20),
    "PaperIdPage" VARCHAR(20),
    "PaperIdRegNumber" VARCHAR(50),
    
    -- Permanent Address (سکونت اصلی)
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" VARCHAR(500),
    
    -- Current Address (سکونت فعلی)
    "CurrentProvinceId" INTEGER,
    "CurrentDistrictId" INTEGER,
    "CurrentVillage" VARCHAR(500),
    
    -- Activity Location (محل فعالیت)
    "ActivityLocation" VARCHAR(500),
    
    -- Tab 2: ثبت مالیه و مشخصات جواز
    "BankReceiptNumber" VARCHAR(100),
    "BankReceiptDate" DATE,
    "LicenseType" VARCHAR(50),
    "LicenseIssueDate" DATE,
    "LicenseExpiryDate" DATE,
    
    -- Tab 3: لغو / انصراف
    -- License Status (1 = Active, 2 = Cancelled, 3 = Withdrawn)
    "LicenseStatus" INTEGER NOT NULL DEFAULT 1,
    "CancellationDate" DATE,
    
    -- Soft delete
    "Status" BOOLEAN NOT NULL DEFAULT TRUE,
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    
    -- Foreign Key Constraints
    CONSTRAINT "PetitionWriterLicenses_PermanentProvinceId_fkey" 
        FOREIGN KEY ("PermanentProvinceId") REFERENCES look."Location"("ID"),
    CONSTRAINT "PetitionWriterLicenses_PermanentDistrictId_fkey" 
        FOREIGN KEY ("PermanentDistrictId") REFERENCES look."Location"("ID"),
    CONSTRAINT "PetitionWriterLicenses_CurrentProvinceId_fkey" 
        FOREIGN KEY ("CurrentProvinceId") REFERENCES look."Location"("ID"),
    CONSTRAINT "PetitionWriterLicenses_CurrentDistrictId_fkey" 
        FOREIGN KEY ("CurrentDistrictId") REFERENCES look."Location"("ID")
);

-- Indexes for PetitionWriterLicenses
CREATE UNIQUE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_LicenseNumber" 
    ON org."PetitionWriterLicenses" ("LicenseNumber") WHERE "Status" = TRUE;
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_ApplicantName" 
    ON org."PetitionWriterLicenses" ("ApplicantName");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_ActivityLocation" 
    ON org."PetitionWriterLicenses" ("ActivityLocation");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_LicenseStatus" 
    ON org."PetitionWriterLicenses" ("LicenseStatus");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_Status" 
    ON org."PetitionWriterLicenses" ("Status");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_LicenseIssueDate" 
    ON org."PetitionWriterLicenses" ("LicenseIssueDate");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterLicenses_LicenseExpiryDate" 
    ON org."PetitionWriterLicenses" ("LicenseExpiryDate");

-- =============================================
-- Table: PetitionWriterRelocations
-- Description: Relocation history for petition writer licenses
-- =============================================
CREATE TABLE IF NOT EXISTS org."PetitionWriterRelocations" (
    "Id" SERIAL PRIMARY KEY,
    "PetitionWriterLicenseId" INTEGER NOT NULL,
    "NewActivityLocation" VARCHAR(500) NOT NULL,
    "RelocationDate" DATE,
    "Remarks" VARCHAR(1000),
    
    -- Audit Fields
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    
    -- Foreign Key Constraint
    CONSTRAINT "PetitionWriterRelocations_PetitionWriterLicenseId_fkey" 
        FOREIGN KEY ("PetitionWriterLicenseId") 
        REFERENCES org."PetitionWriterLicenses"("Id") 
        ON DELETE CASCADE
);

-- Indexes for PetitionWriterRelocations
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterRelocations_PetitionWriterLicenseId" 
    ON org."PetitionWriterRelocations" ("PetitionWriterLicenseId");
CREATE INDEX IF NOT EXISTS "IX_PetitionWriterRelocations_RelocationDate" 
    ON org."PetitionWriterRelocations" ("RelocationDate");

-- =============================================
-- Comments for documentation
-- =============================================
COMMENT ON TABLE org."PetitionWriterLicenses" IS 'ثبت جواز عریضه‌نویسان - Petition Writer License Registration';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseNumber" IS 'نمبر جواز';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantName" IS 'نام متقاضی';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantFatherName" IS 'نام پدر';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantGrandFatherName" IS 'نام پدر کلان';
COMMENT ON COLUMN org."PetitionWriterLicenses"."IdentityCardType" IS 'نوع تذکره (1=الکترونیکی، 2=کاغذی)';
COMMENT ON COLUMN org."PetitionWriterLicenses"."ActivityLocation" IS 'محل فعالیت عریضه‌نویس';
COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseStatus" IS 'وضعیت جواز (1=فعال، 2=لغو، 3=انصراف)';

COMMENT ON TABLE org."PetitionWriterRelocations" IS 'تاریخچه نقل مکان عریضه‌نویسان';
COMMENT ON COLUMN org."PetitionWriterRelocations"."NewActivityLocation" IS 'محل فعالیت جدید';
COMMENT ON COLUMN org."PetitionWriterRelocations"."RelocationDate" IS 'تاریخ نقل مکان';
