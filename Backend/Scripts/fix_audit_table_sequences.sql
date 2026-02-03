-- Fix audit table sequences to auto-generate IDs
-- This fixes the "null value in column Id violates not-null constraint" error

-- Check if licenseaudit table exists and fix it
DO $$
BEGIN
    -- Drop and recreate licenseaudit table with proper SERIAL
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' 
               AND LOWER(table_name) = 'licenseaudit') THEN
        
        -- Get the actual table name (case-sensitive)
        DECLARE
            actual_table_name TEXT;
        BEGIN
            SELECT table_name INTO actual_table_name
            FROM information_schema.tables 
            WHERE table_schema = 'log' 
            AND LOWER(table_name) = 'licenseaudit';
            
            -- Check if Id column is SERIAL
            IF NOT EXISTS (
                SELECT 1 
                FROM information_schema.columns 
                WHERE table_schema = 'log' 
                AND table_name = actual_table_name
                AND column_name = 'Id'
                AND column_default LIKE 'nextval%'
            ) THEN
                RAISE NOTICE 'Fixing licenseaudit table - Id column is not SERIAL';
                
                -- Create sequence if it doesn't exist
                EXECUTE format('CREATE SEQUENCE IF NOT EXISTS log."%s_Id_seq"', actual_table_name);
                
                -- Set the sequence to start from max existing Id + 1
                EXECUTE format('SELECT setval(''log."%s_Id_seq"'', COALESCE((SELECT MAX("Id") FROM log."%s"), 0) + 1, false)', 
                    actual_table_name, actual_table_name);
                
                -- Alter the column to use the sequence
                EXECUTE format('ALTER TABLE log."%s" ALTER COLUMN "Id" SET DEFAULT nextval(''log."%s_Id_seq''::regclass)', 
                    actual_table_name, actual_table_name);
                
                RAISE NOTICE 'Fixed licenseaudit table successfully';
            ELSE
                RAISE NOTICE 'licenseaudit table Id column is already SERIAL';
            END IF;
        END;
    ELSE
        RAISE NOTICE 'licenseaudit table does not exist';
    END IF;
END $$;

-- Fix all other audit tables
DO $$
DECLARE
    audit_table TEXT;
    audit_tables TEXT[] := ARRAY[
        'Companydetailsaudit',
        'Companyowneraudit', 
        'Guarantorsaudit',
        'Graunteeaudit',
        'Licenseaudit'
    ];
BEGIN
    FOREACH audit_table IN ARRAY audit_tables
    LOOP
        -- Check if table exists (case-insensitive)
        IF EXISTS (SELECT 1 FROM information_schema.tables 
                   WHERE table_schema = 'log' 
                   AND LOWER(table_name) = LOWER(audit_table)) THEN
            
            DECLARE
                actual_table_name TEXT;
            BEGIN
                SELECT table_name INTO actual_table_name
                FROM information_schema.tables 
                WHERE table_schema = 'log' 
                AND LOWER(table_name) = LOWER(audit_table);
                
                -- Check if Id column is SERIAL
                IF NOT EXISTS (
                    SELECT 1 
                    FROM information_schema.columns 
                    WHERE table_schema = 'log' 
                    AND table_name = actual_table_name
                    AND column_name = 'Id'
                    AND column_default LIKE 'nextval%'
                ) THEN
                    RAISE NOTICE 'Fixing % table - Id column is not SERIAL', actual_table_name;
                    
                    -- Create sequence if it doesn't exist
                    EXECUTE format('CREATE SEQUENCE IF NOT EXISTS log."%s_Id_seq"', actual_table_name);
                    
                    -- Set the sequence to start from max existing Id + 1
                    EXECUTE format('SELECT setval(''log."%s_Id_seq"'', COALESCE((SELECT MAX("Id") FROM log."%s"), 0) + 1, false)', 
                        actual_table_name, actual_table_name);
                    
                    -- Alter the column to use the sequence
                    EXECUTE format('ALTER TABLE log."%s" ALTER COLUMN "Id" SET DEFAULT nextval(''log."%s_Id_seq''::regclass)', 
                        actual_table_name, actual_table_name);
                    
                    RAISE NOTICE 'Fixed % table successfully', actual_table_name;
                ELSE
                    RAISE NOTICE '% table Id column is already SERIAL', actual_table_name;
                END IF;
            END;
        END IF;
    END LOOP;
END $$;

-- Verify all audit tables have SERIAL Id columns
SELECT 
    table_name,
    column_name,
    column_default,
    CASE 
        WHEN column_default LIKE 'nextval%' THEN '✓ SERIAL'
        ELSE '✗ NOT SERIAL'
    END as status
FROM information_schema.columns
WHERE table_schema = 'log'
AND column_name = 'Id'
AND LOWER(table_name) LIKE '%audit'
ORDER BY table_name;
