-- ============================================
-- Add CancellationType and Revocation (لغوه) fields to CompanyCancellationInfo
-- This adds:
--   - CancellationType: 'فسخ' or 'لغوه' dropdown
--   - RevocationLetterNumber (نمبر مکتوب لغوه جواز)
--   - RevocationRevenueLetterNumber (نمبر مکتوب لغوه عواید)
--   - RevocationLetterDate (تاریخ مکتوب لغوه جواز)
-- ============================================

BEGIN;

-- Add CancellationType column (فسخ or لغوه)
ALTER TABLE org."CompanyCancellationInfo" ADD COLUMN IF NOT EXISTS "CancellationType" VARCHAR(20);
-- Default existing records to 'فسخ' since they already have فسخ fields filled
UPDATE org."CompanyCancellationInfo" SET "CancellationType" = 'فسخ' WHERE "CancellationType" IS NULL;

-- Add Revocation (لغوه) fields
ALTER TABLE org."CompanyCancellationInfo" ADD COLUMN IF NOT EXISTS "RevocationLetterNumber" VARCHAR(100);
ALTER TABLE org."CompanyCancellationInfo" ADD COLUMN IF NOT EXISTS "RevocationRevenueLetterNumber" VARCHAR(100);
ALTER TABLE org."CompanyCancellationInfo" ADD COLUMN IF NOT EXISTS "RevocationLetterDate" DATE;

COMMIT;
