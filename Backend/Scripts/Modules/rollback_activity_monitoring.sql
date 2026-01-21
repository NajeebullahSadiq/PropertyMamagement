-- =====================================================
-- Activity Monitoring Module Rollback Script
-- =====================================================
-- This script removes all Activity Monitoring tables
-- WARNING: This will delete all data in these tables!
-- =====================================================

\echo '=========================================='
\echo 'Activity Monitoring Module Rollback'
\echo '=========================================='
\echo ''
\echo 'WARNING: This will delete all Activity Monitoring data!'
\echo 'Press Ctrl+C to cancel, or Enter to continue...'
\prompt 'Continue? (yes/no): ' confirm

-- Only proceed if user confirms
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_tables 
        WHERE schemaname = 'org' 
        AND tablename = 'ActivityMonitoringRecords'
    ) THEN
        RAISE NOTICE 'Activity Monitoring tables do not exist. Nothing to rollback.';
        RETURN;
    END IF;
END $$;

-- Drop tables in reverse order (child tables first)
\echo 'Dropping ActivityMonitoringPetitionWriterViolations table...'
DROP TABLE IF EXISTS org."ActivityMonitoringPetitionWriterViolations" CASCADE;

\echo 'Dropping ActivityMonitoringRealEstateViolations table...'
DROP TABLE IF EXISTS org."ActivityMonitoringRealEstateViolations" CASCADE;

\echo 'Dropping ActivityMonitoringComplaints table...'
DROP TABLE IF EXISTS org."ActivityMonitoringComplaints" CASCADE;

\echo 'Dropping ActivityMonitoringRecords table...'
DROP TABLE IF EXISTS org."ActivityMonitoringRecords" CASCADE;

-- Remove migration history entry
\echo 'Removing migration history entry...'
DELETE FROM public."__EFMigrationsHistory" 
WHERE "MigrationId" = '20260121000001_ActivityMonitoring_Initial';

\echo ''
\echo '=========================================='
\echo 'Rollback Complete!'
\echo '=========================================='
\echo 'All Activity Monitoring tables have been removed.'
\echo ''

-- Verify tables were dropped
DO $$
DECLARE
    table_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO table_count
    FROM information_schema.tables 
    WHERE table_schema = 'org' 
    AND table_name IN (
        'ActivityMonitoringRecords',
        'ActivityMonitoringComplaints',
        'ActivityMonitoringRealEstateViolations',
        'ActivityMonitoringPetitionWriterViolations'
    );
    
    IF table_count = 0 THEN
        RAISE NOTICE '✓ Verification: All tables successfully removed';
    ELSE
        RAISE WARNING '⚠ Warning: % table(s) still exist', table_count;
    END IF;
END $$;
