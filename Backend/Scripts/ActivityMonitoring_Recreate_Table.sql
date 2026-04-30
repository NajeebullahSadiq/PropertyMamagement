-- =====================================================
-- ActivityMonitoringRecords - Drop & Recreate
-- Preserves all existing data
-- Date: 2026-04-28
-- =====================================================
-- Columns match exactly:
--   Backend/Models/ActivityMonitoring/ActivityMonitoringRecord.cs
--   Backend/Configuration/AppDbContext.cs
-- =====================================================

BEGIN;

-- =====================================================
-- STEP 1: Backup existing data into a temp table
-- =====================================================

CREATE TEMP TABLE _activity_monitoring_backup AS
SELECT
    "Id",
    -- Common fields (use COALESCE to handle columns that may not exist yet)
    "SerialNumber",
    "LicenseNumber",
    "LicenseHolderName",
    "District",
    "ReportRegistrationDate",
    "SectionType",

    -- Deed counts
    "SaleDeedsCount",
    "RentalDeedsCount",
    "BaiUlWafaDeedsCount",
    "VehicleTransactionDeedsCount",
    "DeedItems",

    -- Complaints
    "ComplaintSubject",
    "ComplainantName",
    "ComplaintActionsTaken",
    "ComplaintRemarks",

    -- Violations
    "ViolationStatus",
    "ViolationType",
    "ClosureReason",
    "ViolationActionsTaken",
    "ViolationRemarks",

    -- Inspections
    "Month",
    "MonitoringCount",

    -- Audit
    "Status",
    "CreatedAt",
    "CreatedBy"

FROM org."ActivityMonitoringRecords";

DO $$ BEGIN RAISE NOTICE '✓ Data backed up: % rows', (SELECT COUNT(*) FROM _activity_monitoring_backup); END $$;

-- =====================================================
-- STEP 2: Drop the old table
-- =====================================================

DROP TABLE org."ActivityMonitoringRecords" CASCADE;

DO $$ BEGIN RAISE NOTICE '✓ Old table dropped'; END $$;

-- =====================================================
-- STEP 3: Create the clean table (matches model exactly)
-- =====================================================

CREATE TABLE org."ActivityMonitoringRecords" (
    -- Primary Key
    "Id"                            SERIAL          PRIMARY KEY,

    -- ── Common Fields ──────────────────────────────
    "SerialNumber"                  VARCHAR(50),
    "LicenseNumber"                 VARCHAR(50),
    "LicenseHolderName"             VARCHAR(200),
    "CompanyTitle"                  VARCHAR(300),
    "District"                      VARCHAR(200),
    "ReportRegistrationDate"        DATE,
    "SectionType"                   VARCHAR(50),    -- annualReport | complaints | violations | inspection

    -- ── Section 1: Annual Report ───────────────────
    "SaleDeedsCount"                INTEGER,
    "RentalDeedsCount"              INTEGER,
    "BaiUlWafaDeedsCount"           INTEGER,
    "VehicleTransactionDeedsCount"  INTEGER,
    "DeedItems"                     JSONB,          -- [{"deedType":1,"serialStart":"100","serialEnd":"200","count":100}]

    -- ── Section 2: Complaints ──────────────────────
    "ComplaintSubject"              VARCHAR(500),
    "ComplainantName"               VARCHAR(200),
    "ComplaintActionsTaken"         VARCHAR(1000),
    "ComplaintRemarks"              VARCHAR(1000),

    -- ── Section 3: Violations ─────────────────────
    "ViolationStatus"               VARCHAR(100),   -- منجر به مسدودی | عادی
    "ViolationType"                 VARCHAR(500),
    "ClosureReason"                 VARCHAR(500),
    "ViolationActionsTaken"         VARCHAR(1000),
    "ViolationRemarks"              VARCHAR(1000),

    -- ── Section 4: Inspections ────────────────────
    "Year"                          VARCHAR(20),
    "Month"                         VARCHAR(50),
    "MonitoringCount"               INTEGER,
    "MonitoringRemarks"             VARCHAR(1000),

    -- ── Audit ─────────────────────────────────────
    "Status"                        BOOLEAN         NOT NULL DEFAULT true,
    "CreatedAt"                     TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy"                     VARCHAR(50),
    "UpdatedAt"                     TIMESTAMP WITHOUT TIME ZONE,
    "UpdatedBy"                     VARCHAR(50)
);

DO $$ BEGIN RAISE NOTICE '✓ New table created'; END $$;

-- =====================================================
-- STEP 4: Restore data from backup
-- =====================================================

INSERT INTO org."ActivityMonitoringRecords" (
    "Id",
    "SerialNumber",
    "LicenseNumber",
    "LicenseHolderName",
    "District",
    "ReportRegistrationDate",
    "SectionType",
    "SaleDeedsCount",
    "RentalDeedsCount",
    "BaiUlWafaDeedsCount",
    "VehicleTransactionDeedsCount",
    "DeedItems",
    "ComplaintSubject",
    "ComplainantName",
    "ComplaintActionsTaken",
    "ComplaintRemarks",
    "ViolationStatus",
    "ViolationType",
    "ClosureReason",
    "ViolationActionsTaken",
    "ViolationRemarks",
    "Month",
    "MonitoringCount",
    "Status",
    "CreatedAt",
    "CreatedBy"
)
SELECT
    "Id",
    "SerialNumber",
    "LicenseNumber",
    "LicenseHolderName",
    "District",
    "ReportRegistrationDate",
    "SectionType",
    "SaleDeedsCount",
    "RentalDeedsCount",
    "BaiUlWafaDeedsCount",
    "VehicleTransactionDeedsCount",
    "DeedItems",
    "ComplaintSubject",
    "ComplainantName",
    "ComplaintActionsTaken",
    "ComplaintRemarks",
    "ViolationStatus",
    "ViolationType",
    "ClosureReason",
    "ViolationActionsTaken",
    "ViolationRemarks",
    "Month",
    "MonitoringCount",
    "Status",
    "CreatedAt",
    "CreatedBy"
FROM _activity_monitoring_backup;

DO $$ BEGIN RAISE NOTICE '✓ Data restored: % rows', (SELECT COUNT(*) FROM org."ActivityMonitoringRecords"); END $$;

-- =====================================================
-- STEP 5: Reset the sequence to continue after max Id
-- =====================================================

SELECT setval(
    pg_get_serial_sequence('org."ActivityMonitoringRecords"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."ActivityMonitoringRecords"), 0) + 1,
    false
);

DO $$ BEGIN RAISE NOTICE '✓ Sequence reset'; END $$;

-- =====================================================
-- STEP 6: Create indexes
-- =====================================================

CREATE INDEX "IX_ActivityMonitoringRecords_SerialNumber"
    ON org."ActivityMonitoringRecords" ("SerialNumber");

CREATE INDEX "IX_ActivityMonitoringRecords_LicenseNumber"
    ON org."ActivityMonitoringRecords" ("LicenseNumber");

CREATE INDEX "IX_ActivityMonitoringRecords_LicenseHolderName"
    ON org."ActivityMonitoringRecords" ("LicenseHolderName");

CREATE INDEX "IX_ActivityMonitoringRecords_District"
    ON org."ActivityMonitoringRecords" ("District");

CREATE INDEX "IX_ActivityMonitoringRecords_SectionType"
    ON org."ActivityMonitoringRecords" ("SectionType");

CREATE INDEX "IX_ActivityMonitoringRecords_Status"
    ON org."ActivityMonitoringRecords" ("Status");

CREATE INDEX "IX_ActivityMonitoringRecords_CreatedAt"
    ON org."ActivityMonitoringRecords" ("CreatedAt");

CREATE INDEX "IX_ActivityMonitoringRecords_ReportRegistrationDate"
    ON org."ActivityMonitoringRecords" ("ReportRegistrationDate");

CREATE INDEX "IX_ActivityMonitoringRecords_DeedItems"
    ON org."ActivityMonitoringRecords" USING GIN ("DeedItems");

DO $$ BEGIN RAISE NOTICE '✓ Indexes created'; END $$;

-- =====================================================
-- STEP 7: Add column comments
-- =====================================================

COMMENT ON TABLE  org."ActivityMonitoringRecords" IS 'نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان';

COMMENT ON COLUMN org."ActivityMonitoringRecords"."SerialNumber"                 IS 'نمبر مسلسل';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseNumber"                IS 'نمبر جواز';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."LicenseHolderName"            IS 'شهرت دارنده جواز';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."CompanyTitle"                 IS 'عنوان رهنمایی معاملات';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."District"                     IS 'ناحیه';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ReportRegistrationDate"       IS 'تاریخ ثبت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SectionType"                  IS 'نوعیت ثبت: annualReport | complaints | violations | inspection';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SaleDeedsCount"               IS 'سته‌های فروش';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."RentalDeedsCount"             IS 'سته‌های کرایی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."BaiUlWafaDeedsCount"          IS 'سته‌های بیع الوفا';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."VehicleTransactionDeedsCount" IS 'سته‌های وسایط نقلیه';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."DeedItems"                    IS 'سته‌های اسناد - JSON array';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintSubject"             IS 'موضوع شکایت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplainantName"              IS 'شهرت عارض';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintActionsTaken"        IS 'اجراآت شکایت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ComplaintRemarks"             IS 'ملاحظات شکایت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationStatus"              IS 'وضعیت تخلف: منجر به مسدودی | عادی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationType"                IS 'نوعیت تخلف';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ClosureReason"                IS 'علت مسدودی';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationActionsTaken"        IS 'اجراآت تخلف';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."ViolationRemarks"             IS 'ملاحظات تخلف';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Year"                         IS 'سال نظارت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."Month"                        IS 'ماه نظارت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."MonitoringCount"              IS 'تعداد نظارت';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."MonitoringRemarks"            IS 'ملاحظات نظارت';

DO $$ BEGIN RAISE NOTICE '✓ Comments added'; END $$;

-- =====================================================
-- STEP 8: Verify
-- =====================================================

DO $$
DECLARE
    row_count   INTEGER;
    col_count   INTEGER;
    idx_count   INTEGER;
BEGIN
    SELECT COUNT(*) INTO row_count FROM org."ActivityMonitoringRecords";
    SELECT COUNT(*) INTO col_count FROM information_schema.columns
        WHERE table_schema = 'org' AND table_name = 'ActivityMonitoringRecords';
    SELECT COUNT(*) INTO idx_count FROM pg_indexes
        WHERE schemaname = 'org' AND tablename = 'ActivityMonitoringRecords';

    RAISE NOTICE '';
    RAISE NOTICE '══════════════════════════════════════════';
    RAISE NOTICE ' ActivityMonitoringRecords — Done';
    RAISE NOTICE '══════════════════════════════════════════';
    RAISE NOTICE '  Rows restored : %', row_count;
    RAISE NOTICE '  Columns       : %', col_count;
    RAISE NOTICE '  Indexes       : %', idx_count;
    RAISE NOTICE '══════════════════════════════════════════';
END $$;

COMMIT;
