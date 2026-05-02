-- =====================================================
-- Add TaxAmount column to ActivityMonitoringRecords
-- Date: 2026-05-02
-- =====================================================

BEGIN;

-- Add TaxAmount column (nullable decimal for annual report tax amount)
ALTER TABLE org."ActivityMonitoringRecords"
    ADD COLUMN IF NOT EXISTS "TaxAmount" DECIMAL(18,2);

-- Add SealRemovalReason column (for رفع مهرلاک violation status)
ALTER TABLE org."ActivityMonitoringRecords"
    ADD COLUMN IF NOT EXISTS "SealRemovalReason" VARCHAR(500);

-- Add column comments
COMMENT ON COLUMN org."ActivityMonitoringRecords"."TaxAmount" IS 'مقدار مالیات - for annualReport section';
COMMENT ON COLUMN org."ActivityMonitoringRecords"."SealRemovalReason" IS 'علت رفع مهرلاک - for رفع مهرلاک violation status';

DO $$ BEGIN RAISE NOTICE '✓ TaxAmount and SealRemovalReason columns added to ActivityMonitoringRecords'; END $$;

COMMIT;
