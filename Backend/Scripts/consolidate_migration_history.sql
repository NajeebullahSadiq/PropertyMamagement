-- =====================================================
-- Migration History Consolidation Script
-- =====================================================
-- This script consolidates the EF Core migration history
-- to reflect the new module-based migration structure.
-- 
-- IMPORTANT: Run this ONLY after verifying that the
-- database schema matches the expected state.
-- =====================================================

-- Backup existing migration history
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory_Backup" AS 
SELECT * FROM "__EFMigrationsHistory";

-- Clear existing migration history (optional - uncomment if needed)
-- DELETE FROM "__EFMigrationsHistory";

-- Insert consolidated migration entries
-- These represent the new module-based migrations

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000001_Shared_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000001_Shared_Initial'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000002_UserManagement_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000002_UserManagement_Initial'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000003_Company_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000003_Company_Initial'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000004_Property_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000004_Property_Initial'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000005_Vehicle_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000005_Vehicle_Initial'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000006_Securities_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000006_Securities_Initial'
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20260114000007_Audit_Initial', '7.0.0'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" 
    WHERE "MigrationId" = '20260114000007_Audit_Initial'
);

-- Verify migration history
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";

-- =====================================================
-- End of Migration History Consolidation
-- =====================================================
