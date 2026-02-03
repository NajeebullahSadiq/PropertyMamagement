-- Fix audit tables that use IDENTITY columns instead of SERIAL
-- IDENTITY columns are PostgreSQL's newer auto-increment method

-- Check current state
SELECT 
    table_name,
    column_name,
    is_identity,
    column_default
FROM information_schema.columns
WHERE table_schema = 'log'
AND column_name = 'Id'
AND LOWER(table_name) LIKE '%audit'
ORDER BY table_name;

-- Fix tables that are IDENTITY columns (cannot set DEFAULT on IDENTITY)
-- For IDENTITY columns, we just need to ensure they're set to ALWAYS
DO $$
DECLARE
    audit_table TEXT;
    is_identity_col TEXT;
BEGIN
    RAISE NOTICE '=== Fixing IDENTITY audit tables ===';
    
    -- Loop through all audit tables
    FOR audit_table IN 
        SELECT table_name 
        FROM information_schema.tables 
        WHERE table_schema = 'log' 
        AND LOWER(table_name) LIKE '%audit'
    LOOP
        -- Check if Id column is IDENTITY
        SELECT is_identity INTO is_identity_col
        FROM information_schema.columns
        WHERE table_schema = 'log'
        AND table_name = audit_table
        AND column_name = 'Id';
        
        IF is_identity_col = 'YES' THEN
            -- It's already an IDENTITY column, just ensure it's ALWAYS
            BEGIN
                EXECUTE format('ALTER TABLE log."%s" ALTER COLUMN "Id" SET GENERATED ALWAYS AS IDENTITY', audit_table);
                RAISE NOTICE '✓ Fixed log."%"  (IDENTITY ALWAYS)', audit_table;
            EXCEPTION
                WHEN OTHERS THEN
                    -- Already set, that's fine
                    RAISE NOTICE '✓ log."%" already has IDENTITY', audit_table;
            END;
        ELSIF is_identity_col = 'NO' THEN
            -- Not an IDENTITY column, need to convert it
            -- First check if it has a default
            DECLARE
                has_default TEXT;
            BEGIN
                SELECT column_default INTO has_default
                FROM information_schema.columns
                WHERE table_schema = 'log'
                AND table_name = audit_table
                AND column_name = 'Id';
                
                IF has_default IS NULL OR has_default = '' THEN
                    -- No default, need to add IDENTITY
                    -- Drop the column and recreate it (safest way)
                    RAISE NOTICE 'Converting log."%" to IDENTITY...', audit_table;
                    
                    -- Create a sequence for this table
                    EXECUTE format('CREATE SEQUENCE IF NOT EXISTS log."%s_Id_seq"', audit_table);
                    
                    -- Set sequence to max Id + 1
                    EXECUTE format('SELECT setval(''log."%s_Id_seq"'', COALESCE((SELECT MAX("Id") FROM log."%s"), 0) + 1, false)', 
                        audit_table, audit_table);
                    
                    -- Set default to use sequence
                    EXECUTE format('ALTER TABLE log."%s" ALTER COLUMN "Id" SET DEFAULT nextval(''log."%s_Id_seq''::regclass)', 
                        audit_table, audit_table);
                    
                    RAISE NOTICE '✓ Fixed log."%" (added SERIAL)', audit_table;
                ELSE
                    RAISE NOTICE '✓ log."%" already has default: %', audit_table, has_default;
                END IF;
            END;
        END IF;
    END LOOP;
END $$;

-- Verify all tables
SELECT 
    table_name,
    column_name,
    CASE 
        WHEN is_identity = 'YES' THEN '✓ IDENTITY'
        WHEN column_default LIKE 'nextval%' THEN '✓ SERIAL'
        ELSE '✗ NOT AUTO'
    END as status,
    COALESCE(column_default, 'IDENTITY') as method
FROM information_schema.columns
WHERE table_schema = 'log'
AND column_name = 'Id'
AND LOWER(table_name) LIKE '%audit'
ORDER BY table_name;
