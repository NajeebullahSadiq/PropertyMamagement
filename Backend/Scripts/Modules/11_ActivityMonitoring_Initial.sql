-- =============================================
-- Module: Activity Monitoring (نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان)
-- Schema: org
-- Date: 2026-01-21
-- Description: Creates tables for monitoring real estate offices and petition writers activities
-- Dependencies: None (standalone module)
-- =============================================

-- Ensure org schema exists
CREATE SCHEMA IF NOT EXISTS org;

-- =============================================
-- Table: ActivityMonitoringRecords
-- Description: Main table for activity monitoring records
-- Sections: Financial Clearance, Annual Report, Inspection Summary
-- =============================================
CREATE TABLE IF NOT EXISTS org."ActivityMonitoringRecords" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Section 1: Financial Clearance (Tax Compliance)
    "LicenseHolderName" VARCHAR(200) NOT NULL,
    "TaxClearanceStatus" VARCHAR(100),
    "TaxClearanceLetterNumber" VARCHAR(100),
    "TaxClearanceDate" DATE,
    "PaidTaxAmount" DECIMAL(18,2),
    
    -- Section 2: Annual Activity Report
    "ReportRegistrationDate" DATE,
    "SaleDeedsCount" INTEGER,
    "RentalDeedsCount" INTEGER,
    "BaiUlWafaDeedsCount" INTEGER,
    "VehicleTransactionDeedsCount" INTEGER,
    "CancelledMixedTransactions" INTEGER,
    "LostDeedsCount" INTEGER,
    "AnnualReportRemarks" VARCHAR(1000),
    
    -- Section 6: Inspection & Supervision Summary
    "InspectionDate" DATE,
    "InspectedRealEstateOfficesCount" INTEGER,
    "SealedOfficesCount" INTEGER,
    "InspectedPetitionWritersCount" INTEGER,
    "ViolatingPetitionWritersCount" INTEGER,
    
    -- Audit Fields
    "Status" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy" VARCHAR(50)
);

-- Indexes for ActivityMonitoringRecords
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRecords_LicenseHolderName" 
    ON org."ActivityMonitoringRecords" ("LicenseHolderName");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRecords_TaxClearanceDate" 
    ON org."ActivityMonitoringRecords" ("TaxClearanceDate");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRecords_InspectionDate" 
    ON org."ActivityMonitoringRecords" ("InspectionDate");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRecords_Status" 
    ON org."ActivityMonitoringRecords" ("Status");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRecords_CreatedAt" 
    ON org."ActivityMonitoringRecords" ("CreatedAt");

-- =============================================
-- Table: ActivityMonitoringComplaints
-- Description: Section 3 - Complaints Registration
-- =============================================
CREATE TABLE IF NOT EXISTS org."ActivityMonitoringComplaints" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    "ComplaintSerialNumber" VARCHAR(50) NOT NULL,
    "ComplainantName" VARCHAR(200) NOT NULL,
    "ComplaintSubject" VARCHAR(500) NOT NULL,
    "ComplaintRegistrationDate" DATE,
    "AccusedPartyName" VARCHAR(200) NOT NULL,
    "ActionsTaken" VARCHAR(1000),
    "Remarks" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "FK_ActivityMonitoringComplaints_ActivityMonitoringRecordId" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- Indexes for ActivityMonitoringComplaints
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringComplaints_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringComplaints" ("ActivityMonitoringRecordId");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringComplaints_ComplaintSerialNumber" 
    ON org."ActivityMonitoringComplaints" ("ComplaintSerialNumber");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringComplaints_ComplainantName" 
    ON org."ActivityMonitoringComplaints" ("ComplainantName");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringComplaints_ComplaintRegistrationDate" 
    ON org."ActivityMonitoringComplaints" ("ComplaintRegistrationDate");

-- =============================================
-- Table: ActivityMonitoringRealEstateViolations
-- Description: Section 4 - Violations of Real Estate Offices
-- =============================================
CREATE TABLE IF NOT EXISTS org."ActivityMonitoringRealEstateViolations" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    "ViolationSerialNumber" VARCHAR(50) NOT NULL,
    "LicenseHolderName" VARCHAR(200) NOT NULL,
    "ViolationType" VARCHAR(500) NOT NULL,
    "ViolationDate" DATE,
    "ActionsTaken" VARCHAR(1000),
    "Remarks" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "FK_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecordId" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- Indexes for ActivityMonitoringRealEstateViolations
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringRealEstateViolations" ("ActivityMonitoringRecordId");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRealEstateViolations_ViolationSerialNumber" 
    ON org."ActivityMonitoringRealEstateViolations" ("ViolationSerialNumber");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRealEstateViolations_LicenseHolderName" 
    ON org."ActivityMonitoringRealEstateViolations" ("LicenseHolderName");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringRealEstateViolations_ViolationDate" 
    ON org."ActivityMonitoringRealEstateViolations" ("ViolationDate");

-- =============================================
-- Table: ActivityMonitoringPetitionWriterViolations
-- Description: Section 5 - Violations of Petition Writers
-- =============================================
CREATE TABLE IF NOT EXISTS org."ActivityMonitoringPetitionWriterViolations" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityMonitoringRecordId" INTEGER NOT NULL,
    "ViolationSerialNumber" VARCHAR(50) NOT NULL,
    "PetitionWriterName" VARCHAR(200) NOT NULL,
    "ViolationType" VARCHAR(500) NOT NULL,
    "ViolationDate" DATE,
    "ActionsTaken" VARCHAR(1000),
    "Remarks" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE,
    "CreatedBy" VARCHAR(50),
    CONSTRAINT "FK_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecordId" 
        FOREIGN KEY ("ActivityMonitoringRecordId") 
        REFERENCES org."ActivityMonitoringRecords"("Id") 
        ON DELETE CASCADE
);

-- Indexes for ActivityMonitoringPetitionWriterViolations
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecordId" 
    ON org."ActivityMonitoringPetitionWriterViolations" ("ActivityMonitoringRecordId");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringPetitionWriterViolations_ViolationSerialNumber" 
    ON org."ActivityMonitoringPetitionWriterViolations" ("ViolationSerialNumber");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringPetitionWriterViolations_PetitionWriterName" 
    ON org."ActivityMonitoringPetitionWriterViolations" ("PetitionWriterName");
CREATE INDEX IF NOT EXISTS "IX_ActivityMonitoringPetitionWriterViolations_ViolationDate" 
    ON org."ActivityMonitoringPetitionWriterViolations" ("ViolationDate");

-- =============================================
-- Comments for documentation
-- =============================================
COMMENT ON TABLE org."ActivityMonitoringRecords" IS 'نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان - Monitoring of Real Estate Offices & Petition Writers Activities';
COMMENT ON TABLE org."ActivityMonitoringComplaints" IS 'ثبت شکایات - Complaints Registration';
COMMENT ON TABLE org."ActivityMonitoringRealEstateViolations" IS 'تخلفات دفاتر رهنمای معاملات - Real Estate Office Violations';
COMMENT ON TABLE org."ActivityMonitoringPetitionWriterViolations" IS 'تخلفات عریضه نویسان - Petition Writer Violations';

-- Main Record Comments
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseHolderName" IS 'شهرت دارنده جواز دفتر رهنمای معاملات';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."TaxClearanceStatus" IS 'تصفیه وجایب مالیاتی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."TaxClearanceLetterNumber" IS 'نمبر مکتوب تصفیه مالیاتی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."TaxClearanceDate" IS 'تاریخ تصفیه مالیاتی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."PaidTaxAmount" IS 'مقدار مالیه پرداخت شده';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ReportRegistrationDate" IS 'تاریخ ثبت گزارش سالانه';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SaleDeedsCount" IS 'سته‌های فروش';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."RentalDeedsCount" IS 'سته‌های کرایی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."BaiUlWafaDeedsCount" IS 'سته‌های بیع الوفا';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."VehicleTransactionDeedsCount" IS 'سته‌های وسایط نقلیه';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."CancelledMixedTransactions" IS 'معاملات مختلط ابطال شده';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LostDeedsCount" IS 'سته‌های مفقودی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."AnnualReportRemarks" IS 'ملاحظات گزارش سالانه';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."InspectionDate" IS 'تاریخ نظارت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."InspectedRealEstateOfficesCount" IS 'تعداد دفاتر رهنمای معاملات نظارت شده';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SealedOfficesCount" IS 'تعداد دفاتر مهرولاک شده';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."InspectedPetitionWritersCount" IS 'تعداد عریضه نویسان نظارت شده';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolatingPetitionWritersCount" IS 'تعداد عریضه نویسان متخلف';

-- Complaints Comments
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplaintSerialNumber" IS 'نمبر مسلسل شکایت';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplainantName" IS 'شهرت عارض';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplaintSubject" IS 'موضوع شکایت';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ComplaintRegistrationDate" IS 'تاریخ ثبت شکایت';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."AccusedPartyName" IS 'شهرت معروض علیه';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."ActionsTaken" IS 'اجراآت';
COMMENT ON COLUMN org."ActivityMonitoringComplaints"."Remarks" IS 'ملاحظات';

-- Real Estate Violations Comments
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationSerialNumber" IS 'نمبر مسلسل تخلف';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."LicenseHolderName" IS 'شهرت دارنده جواز فعالیت';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationType" IS 'نوعیت تخلف';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ViolationDate" IS 'تاریخ ثبت تخلف';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."ActionsTaken" IS 'اجراآت';
COMMENT ON COLUMN org."ActivityMonitoringRealEstateViolations"."Remarks" IS 'ملاحظات';

-- Petition Writer Violations Comments
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ViolationSerialNumber" IS 'نمبر مسلسل تخلف';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."PetitionWriterName" IS 'شهرت عریضه نویس';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ViolationType" IS 'نوعیت تخلف';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ViolationDate" IS 'تاریخ ثبت تخلف';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."ActionsTaken" IS 'اجراآت';
COMMENT ON COLUMN org."ActivityMonitoringPetitionWriterViolations"."Remarks" IS 'ملاحظات';

-- =============================================
-- Record migration in history
-- =============================================
INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260121000001_ActivityMonitoring_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM public."__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260121000001_ActivityMonitoring_Initial'
);

-- =============================================
-- Grant permissions (adjust as needed for your environment)
-- =============================================
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA org TO your_app_user;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA org TO your_app_user;

-- Success message
DO $$
BEGIN
    RAISE NOTICE '✓ Activity Monitoring module tables created successfully';
    RAISE NOTICE '  - ActivityMonitoringRecords (main table)';
    RAISE NOTICE '  - ActivityMonitoringComplaints (child table)';
    RAISE NOTICE '  - ActivityMonitoringRealEstateViolations (child table)';
    RAISE NOTICE '  - ActivityMonitoringPetitionWriterViolations (child table)';
    RAISE NOTICE '  Total: 4 tables with indexes and foreign keys';
END $$;
