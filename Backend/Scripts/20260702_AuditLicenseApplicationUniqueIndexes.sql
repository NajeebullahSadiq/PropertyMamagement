-- =============================================================================
-- Audit: License Application unique indexes vs soft delete (Status=false)
-- Date: 2026-07-02
-- Safe to run anytime. READ-ONLY — does not change data.
--
-- Use this before/after running:
--   20260702_UpdateLicenseApplicationSoftDeleteUniqueIndexes.sql
-- =============================================================================

-- All unique indexes on license application tables
SELECT
    n.nspname AS schema_name,
    t.relname AS table_name,
    c.relname AS index_name,
    i.indisunique AS is_unique,
    i.indpred IS NOT NULL AS is_filtered,
    pg_get_expr(i.indpred, i.indrelid) AS filter_expression,
    pg_get_indexdef(i.indexrelid) AS index_definition
FROM pg_index i
JOIN pg_class c ON c.oid = i.indexrelid
JOIN pg_class t ON t.oid = i.indrelid
JOIN pg_namespace n ON n.oid = t.relnamespace
WHERE n.nspname = 'org'
  AND t.relname IN (
      'LicenseApplications',
      'LicenseApplicationGuarantors',
      'LicenseApplicationWithdrawals'
  )
  AND i.indisunique = TRUE
ORDER BY t.relname, c.relname;

-- Flag indexes that likely need Status=true filter for soft delete
SELECT
    c.relname AS index_name,
    t.relname AS table_name,
    pg_get_expr(i.indpred, i.indrelid) AS current_filter,
    CASE
        WHEN t.relname = 'LicenseApplications'
             AND c.relname IN (
                 'IX_LicenseApplications_RequestSerialNumber',
                 'IX_LicenseApplications_ApplicantElectronicNumber'
             )
             AND (
                 i.indpred IS NULL
                 OR pg_get_expr(i.indpred, i.indrelid) NOT ILIKE '%"Status"%true%'
             )
        THEN 'NEEDS FIX — blocks reuse after soft delete'
        WHEN t.relname = 'LicenseApplicationWithdrawals'
             AND c.relname = 'IX_LicenseApplicationWithdrawals_LicenseApplicationId'
        THEN 'OK — one withdrawal row per application Id (not a business-key reuse issue)'
        ELSE 'OK or review manually'
    END AS assessment
FROM pg_index i
JOIN pg_class c ON c.oid = i.indexrelid
JOIN pg_class t ON t.oid = i.indrelid
JOIN pg_namespace n ON n.oid = t.relnamespace
WHERE n.nspname = 'org'
  AND t.relname IN (
      'LicenseApplications',
      'LicenseApplicationGuarantors',
      'LicenseApplicationWithdrawals'
  )
  AND i.indisunique = TRUE
ORDER BY t.relname, c.relname;

-- Fields validated in API only (no DB unique index) — soft delete already respected in code
SELECT field_name, validation_type, db_unique_index, soft_delete_safe_in_code
FROM (
    VALUES
        ('ProposedGuideName', 'API duplicate check (active records only)', 'No', 'Yes'),
        ('ApplicantName + Father + Grandfather + Electronic (composite)', 'API duplicate check (active records only)', 'No', 'Yes'),
        ('GuarantorName + GuarantorFatherName', 'API duplicate check (active parent application only)', 'No', 'Yes'),
        ('ShariaDeedNumber', 'API duplicate check (active parent application only)', 'No', 'Yes'),
        ('CustomaryDeedSerialNumber', 'API duplicate check (active parent application only)', 'No', 'Yes'),
        ('RequestSerialNumber', 'API + DB unique index', 'IX_LicenseApplications_RequestSerialNumber', 'Needs filtered DB index'),
        ('ApplicantElectronicNumber', 'API composite + DB unique index', 'IX_LicenseApplications_ApplicantElectronicNumber', 'Needs filtered DB index')
) AS checks(field_name, validation_type, db_unique_index, soft_delete_safe_in_code);

-- Active duplicate samples (would block migration if any exist)
SELECT 'RequestSerialNumber' AS field_name, "RequestSerialNumber" AS field_value, COUNT(*) AS active_count
FROM org."LicenseApplications"
WHERE "Status" = TRUE
GROUP BY "RequestSerialNumber"
HAVING COUNT(*) > 1

UNION ALL

SELECT 'ApplicantElectronicNumber', "ApplicantElectronicNumber", COUNT(*)
FROM org."LicenseApplications"
WHERE "Status" = TRUE
  AND "ApplicantElectronicNumber" IS NOT NULL
  AND BTRIM("ApplicantElectronicNumber") <> ''
GROUP BY "ApplicantElectronicNumber"
HAVING COUNT(*) > 1

ORDER BY field_name, active_count DESC;
