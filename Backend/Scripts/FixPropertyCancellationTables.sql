-- Combined fix for PropertyCancellation tables
-- Run this script in production to fix the schema issues

BEGIN;

-- ============================================
-- Part 1: Fix PropertyCancellationDocuments
-- ============================================
RAISE NOTICE 'Checking PropertyCancellationDocuments columns...';

-- Check current columns
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'tr' 
    AND table_name = 'PropertyCancellationDocuments'
ORDER BY ordinal_position;

-- Rename DocumentPath to FilePath (if it exists)
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'PropertyCancellationDocuments' 
        AND column_name = 'DocumentPath'
    ) THEN
        ALTER TABLE tr."PropertyCancellationDocuments" 
            RENAME COLUMN "DocumentPath" TO "FilePath";
        RAISE NOTICE 'Renamed DocumentPath to FilePath';
    ELSE
        RAISE NOTICE 'Column DocumentPath does not exist or already renamed';
    END IF;
END $$;

-- Rename DocumentName to OriginalFileName (if it exists)
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'PropertyCancellationDocuments' 
        AND column_name = 'DocumentName'
    ) THEN
        ALTER TABLE tr."PropertyCancellationDocuments" 
            RENAME COLUMN "DocumentName" TO "OriginalFileName";
        RAISE NOTICE 'Renamed DocumentName to OriginalFileName';
    ELSE
        RAISE NOTICE 'Column DocumentName does not exist or already renamed';
    END IF;
END $$;

-- Make FilePath NOT NULL since the model requires it
ALTER TABLE tr."PropertyCancellationDocuments" 
    ALTER COLUMN "FilePath" SET NOT NULL;

RAISE NOTICE 'Set FilePath to NOT NULL';

-- ============================================
-- Part 2: Fix PropertyCancellations
-- ============================================
RAISE NOTICE 'Checking PropertyCancellations columns...';

-- Check current schema
SELECT 
    column_name, 
    data_type, 
    is_nullable, 
    column_default
FROM information_schema.columns
WHERE table_schema = 'tr' 
    AND table_name = 'PropertyCancellations'
ORDER BY ordinal_position;

-- Make Status nullable and add default value
ALTER TABLE tr."PropertyCancellations" 
    ALTER COLUMN "Status" DROP NOT NULL;

ALTER TABLE tr."PropertyCancellations" 
    ALTER COLUMN "Status" SET DEFAULT 'Cancelled';

RAISE NOTICE 'Updated Status column to be nullable with default value';

-- ============================================
-- Verification
-- ============================================
RAISE NOTICE 'Verifying changes...';

-- Verify PropertyCancellationDocuments
SELECT 'PropertyCancellationDocuments' as table_name, column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'tr' 
    AND table_name = 'PropertyCancellationDocuments'
ORDER BY ordinal_position;

-- Verify PropertyCancellations
SELECT 'PropertyCancellations' as table_name, column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_schema = 'tr' 
    AND table_name = 'PropertyCancellations'
ORDER BY ordinal_position;

COMMIT;

RAISE NOTICE 'All changes completed successfully!';
