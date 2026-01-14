-- =====================================================
-- Migration Cleanup Analysis Script
-- =====================================================
-- This script analyzes the current migration state and
-- identifies duplicates, orphans, and inconsistencies.
-- Run this BEFORE applying any cleanup operations.
-- =====================================================

-- 1. List all applied migrations
\echo '=== Applied Migrations ==='
SELECT "MigrationId", "ProductVersion" 
FROM "__EFMigrationsHistory" 
ORDER BY "MigrationId";

-- 2. Check for duplicate table definitions
\echo '=== Checking for Duplicate Tables ==='
SELECT table_schema, table_name, COUNT(*) as count
FROM information_schema.tables
WHERE table_schema IN ('public', 'look', 'org', 'tr', 'log')
GROUP BY table_schema, table_name
HAVING COUNT(*) > 1;

-- 3. List all tables by schema
\echo '=== Tables by Schema ==='
SELECT table_schema, table_name
FROM information_schema.tables
WHERE table_schema IN ('public', 'look', 'org', 'tr', 'log')
ORDER BY table_schema, table_name;

-- 4. Check for orphaned foreign keys
\echo '=== Orphaned Foreign Keys ==='
SELECT 
    tc.constraint_name,
    tc.table_schema,
    tc.table_name,
    kcu.column_name,
    ccu.table_schema AS foreign_table_schema,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
    AND tc.table_schema IN ('public', 'look', 'org', 'tr', 'log')
    AND NOT EXISTS (
        SELECT 1 FROM information_schema.tables t
        WHERE t.table_schema = ccu.table_schema
        AND t.table_name = ccu.table_name
    );

-- 5. Check for missing indexes on foreign keys
\echo '=== Foreign Keys Without Indexes ==='
SELECT 
    tc.table_schema,
    tc.table_name,
    kcu.column_name,
    tc.constraint_name
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu 
    ON tc.constraint_name = kcu.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
    AND tc.table_schema IN ('public', 'look', 'org', 'tr', 'log')
    AND NOT EXISTS (
        SELECT 1 FROM pg_indexes pi
        WHERE pi.schemaname = tc.table_schema
        AND pi.tablename = tc.table_name
        AND pi.indexdef LIKE '%' || kcu.column_name || '%'
    );

-- 6. Summary of table counts by schema
\echo '=== Table Count by Schema ==='
SELECT table_schema, COUNT(*) as table_count
FROM information_schema.tables
WHERE table_schema IN ('public', 'look', 'org', 'tr', 'log')
GROUP BY table_schema
ORDER BY table_schema;

-- 7. Check migration history consistency
\echo '=== Migration History Analysis ==='
SELECT 
    "MigrationId",
    CASE 
        WHEN "MigrationId" LIKE '%Shared%' THEN 'Shared'
        WHEN "MigrationId" LIKE '%User%' THEN 'UserManagement'
        WHEN "MigrationId" LIKE '%Company%' THEN 'Company'
        WHEN "MigrationId" LIKE '%Property%' THEN 'Property'
        WHEN "MigrationId" LIKE '%Vehicle%' THEN 'Vehicle'
        WHEN "MigrationId" LIKE '%Securities%' THEN 'Securities'
        WHEN "MigrationId" LIKE '%Audit%' THEN 'Audit'
        ELSE 'Legacy/Mixed'
    END as module_category
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";

-- =====================================================
-- End of Analysis Script
-- =====================================================
