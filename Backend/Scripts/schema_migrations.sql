-- ============================================================
-- One-time Schema Migrations
-- Previously ran on every application startup in Program.cs
-- Run this script once against the database, then it no longer
-- needs to execute on every app start (saves 2-5 sec startup)
-- ============================================================

-- Add ElectronicNationalIdNumber columns
ALTER TABLE IF EXISTS org."CompanyOwner" ADD COLUMN IF NOT EXISTS "ElectronicNationalIdNumber" VARCHAR(50) NULL;
ALTER TABLE IF EXISTS org."Guarantors" ADD COLUMN IF NOT EXISTS "ElectronicNationalIdNumber" VARCHAR(50) NULL;

-- Add LicenseDetails columns
ALTER TABLE IF EXISTS org."LicenseDetails" ADD COLUMN IF NOT EXISTS "RenewalRound" INTEGER NULL;
ALTER TABLE IF EXISTS org."LicenseDetails" ADD COLUMN IF NOT EXISTS "TariffNumber" VARCHAR(100) NULL;

-- Add PicturePath to PetitionWriterLicenses
ALTER TABLE IF EXISTS org."PetitionWriterLicenses" ADD COLUMN IF NOT EXISTS "PicturePath" VARCHAR(500) NULL;

-- Add ProvinceId columns for province-based access control
ALTER TABLE IF EXISTS public."AspNetUsers" ADD COLUMN IF NOT EXISTS "ProvinceId" INTEGER NULL;
ALTER TABLE IF EXISTS org."CompanyDetails" ADD COLUMN IF NOT EXISTS "ProvinceId" INTEGER NULL;

-- Add foreign key constraints
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'FK_AspNetUsers_Location_ProvinceId'
    ) THEN
        ALTER TABLE public."AspNetUsers"
        ADD CONSTRAINT "FK_AspNetUsers_Location_ProvinceId"
        FOREIGN KEY ("ProvinceId") REFERENCES look."Location"("ID")
        ON DELETE RESTRICT;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'FK_CompanyDetails_Location_ProvinceId'
    ) THEN
        ALTER TABLE org."CompanyDetails"
        ADD CONSTRAINT "FK_CompanyDetails_Location_ProvinceId"
        FOREIGN KEY ("ProvinceId") REFERENCES look."Location"("ID")
        ON DELETE RESTRICT;
    END IF;
END $$;

-- Add indexes for performance
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_ProvinceId" 
ON public."AspNetUsers"("ProvinceId");

CREATE INDEX IF NOT EXISTS "IX_CompanyDetails_ProvinceId" 
ON org."CompanyDetails"("ProvinceId");

-- Rename ElectronicIdNumber to ElectronicNationalIdNumber if the old column exists
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema='org' AND table_name='PetitionWriterLicenses' AND column_name='ElectronicIdNumber'
    ) THEN
        ALTER TABLE org."PetitionWriterLicenses"
        RENAME COLUMN "ElectronicIdNumber" TO "ElectronicNationalIdNumber";
    END IF;
END $$;

-- Migrate data from old columns to ElectronicNationalIdNumber
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema='org' AND table_name='CompanyOwner' AND column_name='IndentityCardNumber'
    ) THEN
        UPDATE org."CompanyOwner"
        SET "ElectronicNationalIdNumber" = "IndentityCardNumber"::text
        WHERE "ElectronicNationalIdNumber" IS NULL;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema='org' AND table_name='Guarantors' AND column_name='NationalIdNumber'
    ) THEN
        UPDATE org."Guarantors"
        SET "ElectronicNationalIdNumber" = "NationalIdNumber"
        WHERE "ElectronicNationalIdNumber" IS NULL;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema='org' AND table_name='Guarantors' AND column_name='IndentityCardNumber'
    ) THEN
        UPDATE org."Guarantors"
        SET "ElectronicNationalIdNumber" = "IndentityCardNumber"::text
        WHERE "ElectronicNationalIdNumber" IS NULL;
    END IF;
END $$;

-- ============================================================
-- Performance Indexes for Common Query Patterns
-- Run once to improve query performance
-- ============================================================

-- PropertyDetails indexes
CREATE INDEX IF NOT EXISTS "IX_PropertyDetails_CreatedBy"
ON org."PropertyDetails"("CreatedBy");

CREATE INDEX IF NOT EXISTS "IX_PropertyDetails_CompanyId"
ON org."PropertyDetails"("CompanyId");

CREATE INDEX IF NOT EXISTS "IX_PropertyDetails_TransactionTypeId"
ON org."PropertyDetails"("TransactionTypeId");

-- BuyerDetails/SellerDetails composite index for lookup by property + national ID
CREATE INDEX IF NOT EXISTS "IX_BuyerDetails_PropertyDetailsId_ElectronicNationalIdNumber"
ON org."BuyerDetails"("PropertyDetailsId", "ElectronicNationalIdNumber");

CREATE INDEX IF NOT EXISTS "IX_SellerDetails_PropertyDetailsId_ElectronicNationalIdNumber"
ON org."SellerDetails"("PropertyDetailsId", "ElectronicNationalIdNumber");

-- ComprehensiveAuditLogs table and indexes for audit-log module
CREATE SCHEMA IF NOT EXISTS audit;

CREATE TABLE IF NOT EXISTS audit."ComprehensiveAuditLogs" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserId" VARCHAR(450) NOT NULL,
    "UserName" VARCHAR(200),
    "UserRole" VARCHAR(100),
    "ActionType" VARCHAR(50) NOT NULL,
    "Module" VARCHAR(100) NOT NULL,
    "EntityType" VARCHAR(100),
    "EntityId" VARCHAR(100),
    "Description" VARCHAR(1000) NOT NULL,
    "DescriptionDari" VARCHAR(1000),
    "OldValues" TEXT,
    "NewValues" TEXT,
    "IpAddress" VARCHAR(45),
    "UserAgent" VARCHAR(500),
    "RequestUrl" VARCHAR(500),
    "HttpMethod" VARCHAR(10),
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Success',
    "ErrorMessage" VARCHAR(1000),
    "Metadata" TEXT,
    "UserProvince" VARCHAR(100),
    "Timestamp" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "DurationMs" BIGINT
);

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_UserId"
ON audit."ComprehensiveAuditLogs"("UserId");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_ActionType"
ON audit."ComprehensiveAuditLogs"("ActionType");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Module"
ON audit."ComprehensiveAuditLogs"("Module");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Timestamp"
ON audit."ComprehensiveAuditLogs"("Timestamp" DESC);

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Status"
ON audit."ComprehensiveAuditLogs"("Status");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_EntityType_EntityId"
ON audit."ComprehensiveAuditLogs"("EntityType", "EntityId");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_EntityType_EntityId_ActionType"
ON audit."ComprehensiveAuditLogs"("EntityType", "EntityId", "ActionType");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_UserProvince"
ON audit."ComprehensiveAuditLogs"("UserProvince");

CREATE INDEX IF NOT EXISTS "IX_ComprehensiveAuditLogs_Module_ActionType_Timestamp"
ON audit."ComprehensiveAuditLogs"("Module", "ActionType", "Timestamp" DESC);

CREATE OR REPLACE VIEW audit."AuditStatistics" AS
SELECT
    DATE("Timestamp") AS "Date",
    "Module",
    "ActionType",
    COUNT(*) AS "TotalCount",
    COUNT(*) FILTER (WHERE "Status" = 'Success') AS "SuccessCount",
    COUNT(*) FILTER (WHERE "Status" IN ('Failed', 'Error')) AS "FailedCount"
FROM audit."ComprehensiveAuditLogs"
GROUP BY DATE("Timestamp"), "Module", "ActionType";
