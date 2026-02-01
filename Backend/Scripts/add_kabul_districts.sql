-- =====================================================
-- Script: Add 22 Districts (ناحیه) for Kabul Province
-- Date: 2026-01-31
-- Description: Adds 22 administrative districts (ناحیه) for Kabul city
-- =====================================================

-- INSTRUCTIONS:
-- 1. Connect to your PostgreSQL database
-- 2. Run this script using pgAdmin or psql command line
-- 3. Command: psql -h localhost -U postgres -d PRMIS -f add_kabul_districts.sql

-- First, find the Kabul province ID
-- Kabul province should have TypeId = 2 (province)
-- The script assumes Kabul's ID needs to be determined first

DO $$
DECLARE
    kabul_province_id INTEGER;
    district_names TEXT[] := ARRAY[
        'ناحیه اول',
        'ناحیه دوم', 
        'ناحیه سوم',
        'ناحیه چهارم',
        'ناحیه پنجم',
        'ناحیه ششم',
        'ناحیه هفتم',
        'ناحیه هشتم',
        'ناحیه نهم',
        'ناحیه دهم',
        'ناحیه یازدهم',
        'ناحیه دوازدهم',
        'ناحیه سیزدهم',
        'ناحیه چهاردهم',
        'ناحیه پانزدهم',
        'ناحیه شانزدهم',
        'ناحیه هفدهم',
        'ناحیه هجدهم',
        'ناحیه نوزدهم',
        'ناحیه بیستم',
        'ناحیه بیست و یکم',
        'ناحیه بیست و دوم'
    ];
    district_name TEXT;
    counter INTEGER := 1;
BEGIN
    -- Find Kabul province ID (looking for کابل in Dari field)
    SELECT "ID" INTO kabul_province_id
    FROM look."Location"
    WHERE "Dari" LIKE '%کابل%' 
      AND "TypeID" = 2
    LIMIT 1;

    -- Check if Kabul was found
    IF kabul_province_id IS NULL THEN
        RAISE EXCEPTION 'Kabul province not found in Location table';
    END IF;

    RAISE NOTICE 'Found Kabul province with ID: %', kabul_province_id;

    -- Insert 22 districts for Kabul
    FOREACH district_name IN ARRAY district_names
    LOOP
        -- Check if district already exists
        IF NOT EXISTS (
            SELECT 1 FROM look."Location"
            WHERE "ParentID" = kabul_province_id
              AND "Dari" = district_name
              AND "TypeID" = 3
        ) THEN
            INSERT INTO look."Location" (
                "Dari",
                "IsActive",
                "ParentID",
                "TypeID",
                "Name",
                "Path_Dari"
            ) VALUES (
                district_name,
                1,
                kabul_province_id,
                3,
                'District ' || counter,
                'کابل/' || district_name
            );
            
            RAISE NOTICE 'Inserted: %', district_name;
        ELSE
            RAISE NOTICE 'Already exists: %', district_name;
        END IF;
        
        counter := counter + 1;
    END LOOP;

    RAISE NOTICE 'Successfully added 22 districts for Kabul';
END $$;

-- Verify the insertion
SELECT 
    l."ID",
    l."Dari" as "District Name",
    l."Name" as "English Name",
    l."ParentID" as "Province ID",
    p."Dari" as "Province Name"
FROM look."Location" l
LEFT JOIN look."Location" p ON l."ParentID" = p."ID"
WHERE l."ParentID" IN (
    SELECT "ID" FROM look."Location" 
    WHERE "Dari" LIKE '%کابل%' AND "TypeID" = 2
)
AND l."TypeID" = 3
ORDER BY l."ID";


-- =====================================================
-- Verification Query
-- =====================================================
-- Run this to verify the districts were added successfully

SELECT 
    l."ID" as "District ID",
    l."Dari" as "District Name (Dari)",
    l."Name" as "District Name (English)",
    l."ParentID" as "Province ID",
    p."Dari" as "Province Name",
    l."IsActive" as "Active Status"
FROM look."Location" l
LEFT JOIN look."Location" p ON l."ParentID" = p."ID"
WHERE l."ParentID" IN (
    SELECT "ID" FROM look."Location" 
    WHERE "Dari" LIKE '%کابل%' AND "TypeID" = 2
)
AND l."TypeID" = 3
ORDER BY l."ID";
