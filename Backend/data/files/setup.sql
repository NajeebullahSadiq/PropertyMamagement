-- Pre-Migration Setup Script
-- Run this script BEFORE the data migration to prepare lookup tables

-- ============================================================================
-- 1. CREATE SCHEMAS
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS log;
CREATE SCHEMA IF NOT EXISTS look;

-- ============================================================================
-- 2. VERIFY REQUIRED TABLES EXIST
-- ============================================================================

DO $$
DECLARE
    required_tables TEXT[][] := ARRAY[
        ARRAY['org', 'CompanyDetails'],
        ARRAY['org', 'CompanyOwners'],
        ARRAY['org', 'LicenseDetails'],
        ARRAY['org', 'CompanyCancellationInfo'],
        ARRAY['look', 'Location'],
        ARRAY['look', 'EducationLevel']
    ];
    tbl TEXT[];
    missing_tables TEXT[] := '{}';
    table_exists BOOLEAN;
BEGIN
    FOREACH tbl SLICE 1 IN ARRAY required_tables
    LOOP
        SELECT EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_schema = tbl[1] 
            AND table_name = tbl[2]
        ) INTO table_exists;
        
        IF NOT table_exists THEN
            missing_tables := array_append(missing_tables, tbl[1] || '.' || tbl[2]);
        END IF;
    END LOOP;
    
    IF array_length(missing_tables, 1) > 0 THEN
        RAISE EXCEPTION 'Missing tables: %. Please run company_module_clean_recreate.sql first.', 
            array_to_string(missing_tables, ', ');
    ELSE
        RAISE NOTICE 'All required tables exist. Ready for migration.';
    END IF;
END $$;

-- ============================================================================
-- 3. ENSURE LOOK.LOCATION TABLE HAS PROPER STRUCTURE
-- ============================================================================

-- Check if columns exist, add if missing
DO $$
BEGIN
    -- Add Type column if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'look' AND table_name = 'Location' AND column_name = 'Type'
    ) THEN
        ALTER TABLE look."Location" ADD COLUMN "Type" VARCHAR(50);
        RAISE NOTICE 'Added Type column to look.Location';
    END IF;
    
    -- Add ParentId column if it doesn't exist (note: your schema uses Parent_ID)
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'look' AND table_name = 'Location' AND column_name = 'Parent_ID'
    ) THEN
        ALTER TABLE look."Location" ADD COLUMN "Parent_ID" INTEGER;
        RAISE NOTICE 'Added Parent_ID column to look.Location';
    END IF;
END $$;

-- ============================================================================
-- 4. SAMPLE LOOKUP DATA (Based on Common Afghan Provinces)
-- ============================================================================

-- Insert common education levels (skip if already exists)
DO $$
BEGIN
    INSERT INTO look."EducationLevel" ("Name", "Dari")
    SELECT * FROM (VALUES 
        ('بکلوریا', 'بکلوریا'),
        ('چهارده پاس', 'چهارده پاس'),
        ('دوازده پاس', 'دوازده پاس'),
        ('لیسانس', 'لیسانس'),
        ('ماستری', 'ماستری'),
        ('دکتورا', 'دکتورا'),
        ('هشتم', 'هشتم'),
        ('دهم', 'دهم')
    ) AS v("Name", "Dari")
    WHERE NOT EXISTS (
        SELECT 1 FROM look."EducationLevel" e WHERE e."Name" = v."Name"
    );
END $$;

-- Insert Afghan provinces (skip if already exists)
DO $$
BEGIN
    INSERT INTO look."Location" ("Name", "Dari", "Type", "IsActive")
    SELECT * FROM (VALUES 
        ('کابل', 'کابل', 'province', 1),
        ('هرات', 'هرات', 'province', 1),
        ('بلخ', 'بلخ', 'province', 1),
        ('قندهار', 'قندهار', 'province', 1),
        ('ننگرهار', 'ننگرهار', 'province', 1),
        ('غزنی', 'غزنی', 'province', 1),
        ('باميان', 'باميان', 'province', 1),
        ('پکتیا', 'پکتیا', 'province', 1),
        ('بدخشان', 'بدخشان', 'province', 1),
        ('تخار', 'تخار', 'province', 1),
        ('کندز', 'کندز', 'province', 1),
        ('بغلان', 'بغلان', 'province', 1),
        ('پروان', 'پروان', 'province', 1),
        ('کاپیسا', 'کاپیسا', 'province', 1),
        ('لغمان', 'لغمان', 'province', 1),
        ('کنر', 'کنر', 'province', 1),
        ('نورستان', 'نورستان', 'province', 1),
        ('لوگر', 'لوگر', 'province', 1),
        ('وردک', 'وردک', 'province', 1),
        ('پکتیکا', 'پکتیکا', 'province', 1),
        ('خوست', 'خوست', 'province', 1),
        ('زابل', 'زابل', 'province', 1),
        ('ارزګان', 'ارزګان', 'province', 1),
        ('غور', 'غور', 'province', 1),
        ('فراه', 'فراه', 'province', 1),
        ('نیمروز', 'نیمروز', 'province', 1),
        ('هلمند', 'هلمند', 'province', 1),
        ('بادغیس', 'بادغیس', 'province', 1),
        ('فاریاب', 'فاریاب', 'province', 1),
        ('جوزجان', 'جوزجان', 'province', 1),
        ('سرپل', 'سرپل', 'province', 1),
        ('سمنگان', 'سمنگان', 'province', 1),
        ('پنجشیر', 'پنجشیر', 'province', 1),
        ('دایکندی', 'دایکندی', 'province', 1)
    ) AS v("Name", "Dari", "Type", "IsActive")
    WHERE NOT EXISTS (
        SELECT 1 FROM look."Location" l WHERE l."Name" = v."Name" AND l."Type" = v."Type"
    );
END $$;

-- Insert common districts (add more as needed)
-- Get Kabul province ID
DO $$
DECLARE
    kabul_id INTEGER;
BEGIN
    SELECT "ID" INTO kabul_id FROM look."Location" WHERE "Name" = 'کابل' AND "Type" = 'province';
    
    IF kabul_id IS NOT NULL THEN
        INSERT INTO look."Location" ("Name", "Dari", "Type", "Parent_ID", "IsActive")
        SELECT * FROM (VALUES 
            ('شاروالی', 'شاروالی', 'district', kabul_id, 1),
            ('پغمان', 'پغمان', 'district', kabul_id, 1),
            ('دهسبز', 'دهسبز', 'district', kabul_id, 1),
            ('چهاراسیاب', 'چهاراسیاب', 'district', kabul_id, 1),
            ('بگرامی', 'بگرامی', 'district', kabul_id, 1),
            ('خاک جبار', 'خاک جبار', 'district', kabul_id, 1),
            ('موسهی', 'موسهی', 'district', kabul_id, 1),
            ('قره باغ', 'قره باغ', 'district', kabul_id, 1),
            ('فرزه', 'فرزه', 'district', kabul_id, 1),
            ('کلکان', 'کلکان', 'district', kabul_id, 1),
            ('سروبی', 'سروبی', 'district', kabul_id, 1),
            ('میربچه کوت', 'میربچه کوت', 'district', kabul_id, 1),
            ('اسطالف', 'اسطالف', 'district', kabul_id, 1),
            ('گلدره', 'گلدره', 'district', kabul_id, 1),
            ('شکردره', 'شکردره', 'district', kabul_id, 1)
        ) AS v("Name", "Dari", "Type", "Parent_ID", "IsActive")
        WHERE NOT EXISTS (
            SELECT 1 FROM look."Location" l WHERE l."Name" = v."Name" AND l."Type" = 'district'
        );
    END IF;
END $$;

-- Insert guarantee types (GuaranteeType has CreatedAt column)
DO $$
BEGIN
    INSERT INTO look."GuaranteeType" ("Name", "Des", "CreatedAt")
    SELECT * FROM (VALUES 
        ('قباله شرعی', 'Sharia Deed', NOW()),
        ('قباله عرفی', 'Customary Deed', NOW()),
        ('پول نقد', 'Cash', NOW())
    ) AS v("Name", "Des", "CreatedAt")
    WHERE NOT EXISTS (
        SELECT 1 FROM look."GuaranteeType" g WHERE g."Name" = v."Name"
    );
END $$;

-- ============================================================================
-- 5. CREATE INDEXES FOR BETTER PERFORMANCE
-- ============================================================================

-- Location indexes
CREATE INDEX IF NOT EXISTS "idx_location_name" ON look."Location"("Name");
CREATE INDEX IF NOT EXISTS "idx_location_type" ON look."Location"("Type");
CREATE INDEX IF NOT EXISTS "idx_location_parent" ON look."Location"("Parent_ID");

-- Education level indexes
CREATE INDEX IF NOT EXISTS "idx_education_name" ON look."EducationLevel"("Name");

-- Company indexes (for migration)
CREATE INDEX IF NOT EXISTS "idx_company_id" ON org."CompanyDetails"("Id");
CREATE INDEX IF NOT EXISTS "idx_company_province" ON org."CompanyDetails"("ProvinceId");

-- Owner indexes
CREATE INDEX IF NOT EXISTS "idx_owner_company" ON org."CompanyOwners"("CompanyId");

-- License indexes
CREATE INDEX IF NOT EXISTS "idx_license_company" ON org."LicenseDetails"("CompanyId");
CREATE INDEX IF NOT EXISTS "idx_license_number" ON org."LicenseDetails"("LicenseNumber");

-- ============================================================================
-- 6. VERIFICATION QUERIES
-- ============================================================================

-- Check created lookup data
SELECT 'Education Levels' as category, COUNT(*) as count FROM look."EducationLevel"
UNION ALL
SELECT 'Provinces', COUNT(*) FROM look."Location" WHERE "Type" = 'province'
UNION ALL
SELECT 'Districts', COUNT(*) FROM look."Location" WHERE "Type" = 'district'
UNION ALL
SELECT 'Guarantee Types', COUNT(*) FROM look."GuaranteeType";

-- Show provinces
SELECT "ID", "Name", "Dari" FROM look."Location" WHERE "Type" = 'province' ORDER BY "Name";

-- Show education levels
SELECT "ID", "Name", "Dari" FROM look."EducationLevel" ORDER BY "ID";

-- ============================================================================
-- 7. BACKUP RECOMMENDATION
-- ============================================================================

-- Before migration, create a backup:
-- pg_dump -h localhost -U your_username -d your_database_name > backup_before_migration.sql

DO $$
BEGIN
    RAISE NOTICE 'Pre-migration setup completed successfully!';
    RAISE NOTICE 'Next step: Run the .NET migration tool with dotnet run';
END $$;
