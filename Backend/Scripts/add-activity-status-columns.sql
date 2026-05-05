-- Migration: Add ActivityStatus and ActivityPermissionReason columns to PetitionWriterMonitoringRecords table
-- Date: 2026-05-05
-- Description: Add وضعیت (ActivityStatus) dropdown and علت اجازه فعالیت (ActivityPermissionReason) fields
--              to the violations section of petition writer monitoring

-- Add ActivityStatus column
ALTER TABLE org."PetitionWriterMonitoringRecords"
ADD COLUMN IF NOT EXISTS "ActivityStatus" VARCHAR(50) NULL;

-- Add ActivityPermissionReason column
ALTER TABLE org."PetitionWriterMonitoringRecords"
ADD COLUMN IF NOT EXISTS "ActivityPermissionReason" VARCHAR(500) NULL;

-- Comment on columns
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ActivityStatus" IS 'وضعیت - activity_prevention (جلوګیری فعالیت) or activity_permission (اجازه فعالیت)';
COMMENT ON COLUMN org."PetitionWriterMonitoringRecords"."ActivityPermissionReason" IS 'علت اجازه فعالیت - Reason for activity permission, shown when ActivityStatus = activity_permission';
