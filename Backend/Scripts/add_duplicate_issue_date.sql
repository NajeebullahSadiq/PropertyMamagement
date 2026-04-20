-- Add DuplicateIssueDate (تاریخ صدور مثنی) column to LicenseDetails table
-- This field is only applicable when LicenseCategory is 'مثنی' (Duplicate)

ALTER TABLE org."LicenseDetails" ADD COLUMN IF NOT EXISTS "DuplicateIssueDate" date NULL;

COMMENT ON COLUMN org."LicenseDetails"."DuplicateIssueDate" IS 'تاریخ صدور مثنی - Duplicate Issue Date (only applicable when LicenseCategory is مثنی)';
