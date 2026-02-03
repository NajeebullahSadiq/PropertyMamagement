-- Cleanup Duplicate Audit Tables
-- Production has both PascalCase and lowercase versions
-- We need to keep lowercase (what AppDbContext uses) and drop PascalCase duplicates

-- STEP 1: Check which tables have data
DO $$
BEGIN
    RAISE NOTICE '=== Checking for data in audit tables ===';
END $$;

SELECT 
    'PascalCase: ' || table_name as table_info,
    (SELECT COUNT(*) FROM information_schema.tables t2 
     WHERE t2.table_schema = 'log' AND t2.table_name = t.table_name) as exists,
    'Records: ' || COALESCE(
        (SELECT COUNT(*)::text FROM log."Companydetailsaudit" WHERE table_name = 'Companydetailsaudit'),
        '0'
    ) as record_count
FROM information_schema.tables t
WHERE table_schema = 'log' 
AND table_name IN ('Companydetailsaudit', 'Companyowneraudit', 'Guarantorsaudit', 'Graunteeaudit', 'Licenseaudit')
UNION ALL
SELECT 
    'lowercase: ' || table_name,
    1,
    'Records: ' || COALESCE(
        (SELECT COUNT(*)::text FROM log."companydetailsaudit" WHERE table_name = 'companydetailsaudit'),
        '0'
    )
FROM information_schema.tables t
WHERE table_schema = 'log' 
AND table_name IN ('companydetailsaudit', 'companyowneraudit', 'guarantorsaudit', 'graunteeaudit', 'licenseaudit');

-- STEP 2: Drop PascalCase duplicate tables (keep lowercase)
DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=== Dropping PascalCase duplicate tables ===';
    
    -- Drop PascalCase versions (if they exist)
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Companydetailsaudit') THEN
        DROP TABLE IF EXISTS log."Companydetailsaudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Companydetailsaudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Companyowneraudit') THEN
        DROP TABLE IF EXISTS log."Companyowneraudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Companyowneraudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Guarantorsaudit') THEN
        DROP TABLE IF EXISTS log."Guarantorsaudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Guarantorsaudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Graunteeaudit') THEN
        DROP TABLE IF EXISTS log."Graunteeaudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Graunteeaudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Licenseaudit') THEN
        DROP TABLE IF EXISTS log."Licenseaudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Licenseaudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Propertyaudit') THEN
        DROP TABLE IF EXISTS log."Propertyaudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Propertyaudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Propertybuyeraudit') THEN
        DROP TABLE IF EXISTS log."Propertybuyeraudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Propertybuyeraudit"';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'Propertyselleraudit') THEN
        DROP TABLE IF EXISTS log."Propertyselleraudit" CASCADE;
        RAISE NOTICE '✓ Dropped log."Propertyselleraudit"';
    END IF;
    
    -- Drop PascalCase sequences too
    DROP SEQUENCE IF EXISTS log."Companydetailsaudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Companyowneraudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Guarantorsaudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Graunteeaudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Licenseaudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Propertyaudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Propertybuyeraudit_Id_seq" CASCADE;
    DROP SEQUENCE IF EXISTS log."Propertyselleraudit_Id_seq" CASCADE;
    
    RAISE NOTICE '✓ Dropped all PascalCase sequences';
END $$;

-- STEP 3: Fix lowercase tables to have SERIAL Id columns
DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=== Fixing lowercase audit tables ===';
END $$;

-- Fix licenseaudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'licenseaudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."licenseaudit_Id_seq";
        PERFORM setval('log."licenseaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."licenseaudit"), 0) + 1, false);
        ALTER TABLE log."licenseaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."licenseaudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."licenseaudit"';
    END IF;
END $$;

-- Fix companydetailsaudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'companydetailsaudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."companydetailsaudit_Id_seq";
        PERFORM setval('log."companydetailsaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."companydetailsaudit"), 0) + 1, false);
        ALTER TABLE log."companydetailsaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."companydetailsaudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."companydetailsaudit"';
    END IF;
END $$;

-- Fix companyowneraudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'companyowneraudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."companyowneraudit_Id_seq";
        PERFORM setval('log."companyowneraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."companyowneraudit"), 0) + 1, false);
        ALTER TABLE log."companyowneraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."companyowneraudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."companyowneraudit"';
    END IF;
END $$;

-- Fix guarantorsaudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'guarantorsaudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."guarantorsaudit_Id_seq";
        PERFORM setval('log."guarantorsaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."guarantorsaudit"), 0) + 1, false);
        ALTER TABLE log."guarantorsaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."guarantorsaudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."guarantorsaudit"';
    END IF;
END $$;

-- Fix graunteeaudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'graunteeaudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."graunteeaudit_Id_seq";
        PERFORM setval('log."graunteeaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."graunteeaudit"), 0) + 1, false);
        ALTER TABLE log."graunteeaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."graunteeaudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."graunteeaudit"';
    END IF;
END $$;

-- Fix propertyaudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'propertyaudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."propertyaudit_Id_seq";
        PERFORM setval('log."propertyaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."propertyaudit"), 0) + 1, false);
        ALTER TABLE log."propertyaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."propertyaudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."propertyaudit"';
    END IF;
END $$;

-- Fix propertybuyeraudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'propertybuyeraudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."propertybuyeraudit_Id_seq";
        PERFORM setval('log."propertybuyeraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."propertybuyeraudit"), 0) + 1, false);
        ALTER TABLE log."propertybuyeraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."propertybuyeraudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."propertybuyeraudit"';
    END IF;
END $$;

-- Fix propertyselleraudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'propertyselleraudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."propertyselleraudit_Id_seq";
        PERFORM setval('log."propertyselleraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."propertyselleraudit"), 0) + 1, false);
        ALTER TABLE log."propertyselleraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."propertyselleraudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."propertyselleraudit"';
    END IF;
END $$;

-- Fix vehicleaudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'vehicleaudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."vehicleaudit_Id_seq";
        PERFORM setval('log."vehicleaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."vehicleaudit"), 0) + 1, false);
        ALTER TABLE log."vehicleaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."vehicleaudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."vehicleaudit"';
    END IF;
END $$;

-- Fix vehiclebuyeraudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'vehiclebuyeraudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."vehiclebuyeraudit_Id_seq";
        PERFORM setval('log."vehiclebuyeraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."vehiclebuyeraudit"), 0) + 1, false);
        ALTER TABLE log."vehiclebuyeraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."vehiclebuyeraudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."vehiclebuyeraudit"';
    END IF;
END $$;

-- Fix vehicleselleraudit
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'vehicleselleraudit') THEN
        CREATE SEQUENCE IF NOT EXISTS log."vehicleselleraudit_Id_seq";
        PERFORM setval('log."vehicleselleraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."vehicleselleraudit"), 0) + 1, false);
        ALTER TABLE log."vehicleselleraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."vehicleselleraudit_Id_seq"'::regclass);
        RAISE NOTICE '✓ Fixed log."vehicleselleraudit"';
    END IF;
END $$;

-- STEP 4: Verify - should only see lowercase tables now, all with SERIAL
DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=== Verification ===';
END $$;

SELECT 
    table_name,
    column_name,
    CASE 
        WHEN column_default LIKE 'nextval%' THEN '✓ SERIAL'
        ELSE '✗ NOT SERIAL'
    END as status
FROM information_schema.columns
WHERE table_schema = 'log'
AND column_name = 'Id'
AND LOWER(table_name) LIKE '%audit'
ORDER BY table_name;
