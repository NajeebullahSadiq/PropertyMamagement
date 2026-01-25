-- Fix all audit table Id columns to be auto-increment
-- This script handles both IDENTITY columns and regular columns with sequences

DO $$
DECLARE
    audit_table TEXT;
    audit_tables TEXT[] := ARRAY[
        'licenseaudit',
        'propertyaudit',
        'propertybuyeraudit',
        'propertyselleraudit',
        'vehicleaudit',
        'vehiclebuyeraudit',
        'vehicleselleraudit',
        'guarantorsaudit',
        'graunteeaudit',
        'companyowneraudit',
        'companydetailsaudit'
    ];
    seq_name TEXT;
    max_id INTEGER;
    col_is_identity BOOLEAN;
    col_has_default BOOLEAN;
BEGIN
    FOREACH audit_table IN ARRAY audit_tables
    LOOP
        RAISE NOTICE 'Processing table: %', audit_table;
        
        -- Check if the column is an IDENTITY column
        SELECT 
            CASE WHEN c.is_identity = 'YES' THEN TRUE ELSE FALSE END,
            CASE WHEN c.column_default IS NOT NULL THEN TRUE ELSE FALSE END
        INTO col_is_identity, col_has_default
        FROM information_schema.columns c
        WHERE c.table_schema = 'log' 
          AND c.table_name = audit_table 
          AND c.column_name = 'Id';
        
        IF col_is_identity THEN
            RAISE NOTICE 'Table % already has IDENTITY column - skipping', audit_table;
            CONTINUE;
        END IF;
        
        seq_name := audit_table || '_id_seq';
        
        -- Drop the existing default if any (only if not identity)
        IF col_has_default THEN
            BEGIN
                EXECUTE format('ALTER TABLE log.%I ALTER COLUMN "Id" DROP DEFAULT', audit_table);
                RAISE NOTICE 'Dropped existing default for %', audit_table;
            EXCEPTION WHEN OTHERS THEN
                RAISE NOTICE 'Could not drop default for % (may not exist): %', audit_table, SQLERRM;
            END;
        END IF;
        
        -- Create a sequence if it doesn't exist
        IF NOT EXISTS (SELECT 1 FROM pg_sequences WHERE schemaname = 'log' AND sequencename = seq_name) THEN
            EXECUTE format('CREATE SEQUENCE log.%I', seq_name);
            RAISE NOTICE 'Created sequence: log.%', seq_name;
        ELSE
            RAISE NOTICE 'Sequence log.% already exists', seq_name;
        END IF;
        
        -- Get max Id from the table
        EXECUTE format('SELECT COALESCE(MAX("Id"), 0) FROM log.%I', audit_table) INTO max_id;
        
        -- Set the sequence to start from the max existing Id + 1
        EXECUTE format('SELECT setval(''log.%I'', %s, false)', seq_name, max_id + 1);
        RAISE NOTICE 'Set sequence % to start at %', seq_name, max_id + 1;
        
        -- Set the default value to use the sequence
        EXECUTE format('ALTER TABLE log.%I ALTER COLUMN "Id" SET DEFAULT nextval(''log.%I'')', audit_table, seq_name);
        
        -- Make sure the sequence is owned by the column
        EXECUTE format('ALTER SEQUENCE log.%I OWNED BY log.%I."Id"', seq_name, audit_table);
        
        RAISE NOTICE 'Successfully fixed table: %', audit_table;
    END LOOP;
    
    RAISE NOTICE '=== All audit tables have been processed! ===';
END $$;

-- Verify the fix for all tables
SELECT 
    table_name,
    column_name, 
    data_type, 
    column_default,
    is_nullable,
    is_identity
FROM information_schema.columns
WHERE table_schema = 'log' 
  AND table_name IN (
    'licenseaudit',
    'propertyaudit',
    'propertybuyeraudit',
    'propertyselleraudit',
    'vehicleaudit',
    'vehiclebuyeraudit',
    'vehicleselleraudit',
    'guarantorsaudit',
    'graunteeaudit',
    'companyowneraudit',
    'companydetailsaudit'
  )
  AND column_name = 'Id'
ORDER BY table_name;
