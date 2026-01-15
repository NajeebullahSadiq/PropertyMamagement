-- =============================================
-- Module: License Application (ثبت درخواست متقاضیان جواز رهنمای معاملات)
-- Schema: org
-- Date: 2026-01-15
-- Description: Creates tables for license application management
-- Dependencies: Shared module (Location, GuaranteeType)
-- =============================================

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- =============================================
-- Table: LicenseApplications
-- Description: Main table for license applications
-- =============================================
CREATE TABLE IF NOT EXISTS org."LicenseApplications" (
    "Id" SERIAL PRIMARY KEY,
    "RequestDate" DATE,
    "RequestSerialNumber" VARCHAR(50) NOT NULL,
    "ApplicantName" VARCHAR(200) NOT NULL,
    "ProposedGuideName" VARCHAR(200) NOT NULL,
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" VARCHAR(500),
    "CurrentProvinceId" INTEGER,
    "CurrentDistrictId" INTEGER,
    "CurrentVillage" VARCHAR(500),
    "Status" BOOLEAN NOT NULL DEFAULT TRUE,
    "IsWithdrawn" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50),
    CONSTRAINT "LicenseApplications_PermanentProvinceId_fkey" 
        FOREIGN KEY ("PermanentProvinceId") REFERENCES look."Location"("ID"),
    CONSTRAINT "LicenseApplications_PermanentDistrictId_fkey" 
        FOREIGN KEY ("PermanentDistrictId") REFERENCES look."Location"("ID"),
    CONSTRAINT "LicenseApplications_CurrentProvinceId_fkey" 
        FOREIGN KEY ("CurrentProvinceId") REFERENCES look."Location"("ID"),
    CONSTRAINT "LicenseApplications_CurrentDistrictId_fkey" 
        FOREIGN KEY ("CurrentDistrictId") REFERENCES look."Location"("ID")
);

-- Indexes for LicenseApplications
CREATE UNIQUE INDEX IF NOT EXISTS "IX_LicenseApplications_RequestSerialNumber" 
    ON org."LicenseApplications" ("RequestSerialNumber");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplications_ApplicantName" 
    ON org."LicenseApplications" ("ApplicantName");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplications_ProposedGuideName" 
    ON org."LicenseApplications" ("ProposedGuideName");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplications_PermanentProvinceId" 
    ON org."LicenseApplications" ("PermanentProvinceId");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplications_CurrentProvinceId" 
    ON org."LicenseApplications" ("CurrentProvinceId");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplications_Status" 
    ON org."LicenseApplications" ("Status");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplications_IsWithdrawn" 
    ON org."LicenseApplications" ("IsWithdrawn");

-- =============================================
-- Table: LicenseApplicationGuarantors
-- Description: Guarantors for license applications
-- =============================================
CREATE TABLE IF NOT EXISTS org."LicenseApplicationGuarantors" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseApplicationId" INTEGER NOT NULL,
    "GuarantorName" VARCHAR(200) NOT NULL,
    "GuarantorFatherName" VARCHAR(200),
    "GuaranteeTypeId" INTEGER NOT NULL,
    "CashAmount" DECIMAL(18,2),
    "ShariaDeedNumber" VARCHAR(100),
    "ShariaDeedDate" DATE,
    "CustomaryDeedSerialNumber" VARCHAR(100),
    "PermanentProvinceId" INTEGER,
    "PermanentDistrictId" INTEGER,
    "PermanentVillage" VARCHAR(500),
    "CurrentProvinceId" INTEGER,
    "CurrentDistrictId" INTEGER,
    "CurrentVillage" VARCHAR(500),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "LicenseApplicationGuarantors_LicenseApplicationId_fkey" 
        FOREIGN KEY ("LicenseApplicationId") REFERENCES org."LicenseApplications"("Id") ON DELETE CASCADE,
    CONSTRAINT "LicenseApplicationGuarantors_GuaranteeTypeId_fkey" 
        FOREIGN KEY ("GuaranteeTypeId") REFERENCES look."GuaranteeType"("Id"),
    CONSTRAINT "LicenseApplicationGuarantors_PermanentProvinceId_fkey" 
        FOREIGN KEY ("PermanentProvinceId") REFERENCES look."Location"("ID"),
    CONSTRAINT "LicenseApplicationGuarantors_PermanentDistrictId_fkey" 
        FOREIGN KEY ("PermanentDistrictId") REFERENCES look."Location"("ID"),
    CONSTRAINT "LicenseApplicationGuarantors_CurrentProvinceId_fkey" 
        FOREIGN KEY ("CurrentProvinceId") REFERENCES look."Location"("ID"),
    CONSTRAINT "LicenseApplicationGuarantors_CurrentDistrictId_fkey" 
        FOREIGN KEY ("CurrentDistrictId") REFERENCES look."Location"("ID")
);

-- Indexes for LicenseApplicationGuarantors
CREATE INDEX IF NOT EXISTS "IX_LicenseApplicationGuarantors_LicenseApplicationId" 
    ON org."LicenseApplicationGuarantors" ("LicenseApplicationId");
CREATE INDEX IF NOT EXISTS "IX_LicenseApplicationGuarantors_GuaranteeTypeId" 
    ON org."LicenseApplicationGuarantors" ("GuaranteeTypeId");

-- =============================================
-- Table: LicenseApplicationWithdrawals
-- Description: Withdrawal information for license applications
-- =============================================
CREATE TABLE IF NOT EXISTS org."LicenseApplicationWithdrawals" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseApplicationId" INTEGER NOT NULL,
    "WithdrawalReason" VARCHAR(1000) NOT NULL,
    "WithdrawalDate" DATE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "LicenseApplicationWithdrawals_LicenseApplicationId_fkey" 
        FOREIGN KEY ("LicenseApplicationId") REFERENCES org."LicenseApplications"("Id") ON DELETE CASCADE
);

-- Unique index - one withdrawal per application
CREATE UNIQUE INDEX IF NOT EXISTS "IX_LicenseApplicationWithdrawals_LicenseApplicationId" 
    ON org."LicenseApplicationWithdrawals" ("LicenseApplicationId");

-- =============================================
-- Comments for documentation
-- =============================================
COMMENT ON TABLE org."LicenseApplications" IS 'ثبت درخواست متقاضیان جواز رهنمای معاملات - License Applications for Real Estate Guides';
COMMENT ON TABLE org."LicenseApplicationGuarantors" IS 'تضمین‌کنندگان - Guarantors for License Applications';
COMMENT ON TABLE org."LicenseApplicationWithdrawals" IS 'انصراف - Withdrawal Information for License Applications';

COMMENT ON COLUMN org."LicenseApplications"."RequestSerialNumber" IS 'نمبر مسلسل درخواست';
COMMENT ON COLUMN org."LicenseApplications"."ApplicantName" IS 'شهرت متقاضی';
COMMENT ON COLUMN org."LicenseApplications"."ProposedGuideName" IS 'نام پیشنهادی رهنما';
COMMENT ON COLUMN org."LicenseApplications"."IsWithdrawn" IS 'وضعیت انصراف';

COMMENT ON COLUMN org."LicenseApplicationGuarantors"."GuaranteeTypeId" IS '1=پول نقد, 2=قباله شرعی, 3=قباله عرفی';
COMMENT ON COLUMN org."LicenseApplicationGuarantors"."CashAmount" IS 'مبلغ پول نقد (if GuaranteeTypeId=1)';
COMMENT ON COLUMN org."LicenseApplicationGuarantors"."ShariaDeedNumber" IS 'نمبر قباله شرعی (if GuaranteeTypeId=2)';
COMMENT ON COLUMN org."LicenseApplicationGuarantors"."CustomaryDeedSerialNumber" IS 'سریال نمبر سته قباله عرفی (if GuaranteeTypeId=3)';

-- =============================================
-- Record migration in history
-- =============================================
INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260115000000_LicenseApplication_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM public."__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260115000000_LicenseApplication_Initial'
);

-- Success message
DO $$
BEGIN
    RAISE NOTICE 'License Application module tables created successfully';
END $$;
