-- Fix lowercase audit tables (the ones actually being used by the application)
-- The production database has duplicate tables with different case

-- Fix licenseaudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'licenseaudit') THEN
        
        -- Create sequence if it doesn't exist
        CREATE SEQUENCE IF NOT EXISTS log."licenseaudit_Id_seq";
        
        -- Set the sequence to start from max existing Id + 1
        PERFORM setval('log."licenseaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."licenseaudit"), 0) + 1, false);
        
        -- Alter the column to use the sequence
        ALTER TABLE log."licenseaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."licenseaudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed licenseaudit (lowercase)';
    END IF;
END $$;

-- Fix companydetailsaudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'companydetailsaudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."companydetailsaudit_Id_seq";
        
        PERFORM setval('log."companydetailsaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."companydetailsaudit"), 0) + 1, false);
        
        ALTER TABLE log."companydetailsaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."companydetailsaudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed companydetailsaudit (lowercase)';
    END IF;
END $$;

-- Fix companyowneraudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'companyowneraudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."companyowneraudit_Id_seq";
        
        PERFORM setval('log."companyowneraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."companyowneraudit"), 0) + 1, false);
        
        ALTER TABLE log."companyowneraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."companyowneraudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed companyowneraudit (lowercase)';
    END IF;
END $$;

-- Fix guarantorsaudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'guarantorsaudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."guarantorsaudit_Id_seq";
        
        PERFORM setval('log."guarantorsaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."guarantorsaudit"), 0) + 1, false);
        
        ALTER TABLE log."guarantorsaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."guarantorsaudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed guarantorsaudit (lowercase)';
    END IF;
END $$;

-- Fix graunteeaudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'graunteeaudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."graunteeaudit_Id_seq";
        
        PERFORM setval('log."graunteeaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."graunteeaudit"), 0) + 1, false);
        
        ALTER TABLE log."graunteeaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."graunteeaudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed graunteeaudit (lowercase)';
    END IF;
END $$;

-- Fix propertyaudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'propertyaudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."propertyaudit_Id_seq";
        
        PERFORM setval('log."propertyaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."propertyaudit"), 0) + 1, false);
        
        ALTER TABLE log."propertyaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."propertyaudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed propertyaudit (lowercase)';
    END IF;
END $$;

-- Fix propertybuyeraudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'propertybuyeraudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."propertybuyeraudit_Id_seq";
        
        PERFORM setval('log."propertybuyeraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."propertybuyeraudit"), 0) + 1, false);
        
        ALTER TABLE log."propertybuyeraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."propertybuyeraudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed propertybuyeraudit (lowercase)';
    END IF;
END $$;

-- Fix propertyselleraudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'propertyselleraudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."propertyselleraudit_Id_seq";
        
        PERFORM setval('log."propertyselleraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."propertyselleraudit"), 0) + 1, false);
        
        ALTER TABLE log."propertyselleraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."propertyselleraudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed propertyselleraudit (lowercase)';
    END IF;
END $$;

-- Fix vehicleaudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'vehicleaudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."vehicleaudit_Id_seq";
        
        PERFORM setval('log."vehicleaudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."vehicleaudit"), 0) + 1, false);
        
        ALTER TABLE log."vehicleaudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."vehicleaudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed vehicleaudit (lowercase)';
    END IF;
END $$;

-- Fix vehiclebuyeraudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'vehiclebuyeraudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."vehiclebuyeraudit_Id_seq";
        
        PERFORM setval('log."vehiclebuyeraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."vehiclebuyeraudit"), 0) + 1, false);
        
        ALTER TABLE log."vehiclebuyeraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."vehiclebuyeraudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed vehiclebuyeraudit (lowercase)';
    END IF;
END $$;

-- Fix vehicleselleraudit (lowercase)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables 
               WHERE table_schema = 'log' AND table_name = 'vehicleselleraudit') THEN
        
        CREATE SEQUENCE IF NOT EXISTS log."vehicleselleraudit_Id_seq";
        
        PERFORM setval('log."vehicleselleraudit_Id_seq"', 
            COALESCE((SELECT MAX("Id") FROM log."vehicleselleraudit"), 0) + 1, false);
        
        ALTER TABLE log."vehicleselleraudit" 
            ALTER COLUMN "Id" SET DEFAULT nextval('log."vehicleselleraudit_Id_seq"'::regclass);
        
        RAISE NOTICE '✓ Fixed vehicleselleraudit (lowercase)';
    END IF;
END $$;

-- Verify all lowercase audit tables now have SERIAL
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
AND table_name ~ '^[a-z].*audit$'  -- lowercase tables ending with 'audit'
ORDER BY table_name;
