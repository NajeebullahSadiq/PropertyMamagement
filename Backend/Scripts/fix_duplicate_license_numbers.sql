-- Fix duplicate license numbers by reassigning new numbers to duplicates
-- This script identifies duplicates and assigns new sequential license numbers

BEGIN;

-- Step 1: Find and display all duplicates
DO $$
DECLARE
    duplicate_record RECORD;
    duplicate_count INTEGER;
BEGIN
    RAISE NOTICE '=== Finding Duplicate License Numbers ===';
    
    FOR duplicate_record IN 
        SELECT "LicenseNumber", COUNT(*) as count, 
               STRING_AGG("Id"::text, ', ') as ids,
               STRING_AGG("CompanyId"::text, ', ') as company_ids
        FROM org."LicenseDetails"
        WHERE "LicenseNumber" IS NOT NULL
        GROUP BY "LicenseNumber"
        HAVING COUNT(*) > 1
        ORDER BY "LicenseNumber"
    LOOP
        RAISE NOTICE 'Duplicate: % (Count: %, IDs: %, Companies: %)', 
            duplicate_record."LicenseNumber", 
            duplicate_record.count,
            duplicate_record.ids,
            duplicate_record.company_ids;
    END LOOP;
END $$;

-- Step 2: Create a temporary table to store the reassignments
CREATE TEMP TABLE license_reassignments (
    license_detail_id INTEGER,
    old_license_number TEXT,
    new_license_number TEXT,
    company_id INTEGER,
    created_at TIMESTAMP
);

-- Step 3: For each duplicate, keep the oldest one and reassign the rest
DO $$
DECLARE
    dup_license TEXT;
    dup_record RECORD;
    province_code TEXT;
    max_number INTEGER;
    new_number INTEGER;
    new_license TEXT;
BEGIN
    -- Loop through each duplicate license number
    FOR dup_license IN 
        SELECT "LicenseNumber"
        FROM org."LicenseDetails"
        WHERE "LicenseNumber" IS NOT NULL
        GROUP BY "LicenseNumber"
        HAVING COUNT(*) > 1
    LOOP
        RAISE NOTICE 'Processing duplicate: %', dup_license;
        
        -- Extract province code (e.g., 'KBL' from 'KBL-00007477')
        province_code := SPLIT_PART(dup_license, '-', 1);
        
        -- Find the maximum number for this province
        SELECT COALESCE(MAX(CAST(SPLIT_PART("LicenseNumber", '-', 2) AS INTEGER)), 0)
        INTO max_number
        FROM org."LicenseDetails"
        WHERE "LicenseNumber" LIKE province_code || '-%';
        
        new_number := max_number;
        
        -- For each duplicate record except the first (oldest), assign a new number
        FOR dup_record IN 
            SELECT "Id", "CompanyId", "CreatedAt"
            FROM org."LicenseDetails"
            WHERE "LicenseNumber" = dup_license
            ORDER BY "CreatedAt", "Id"
            OFFSET 1  -- Skip the first (oldest) record
        LOOP
            new_number := new_number + 1;
            new_license := province_code || '-' || LPAD(new_number::TEXT, 8, '0');
            
            -- Store the reassignment
            INSERT INTO license_reassignments 
            VALUES (dup_record."Id", dup_license, new_license, dup_record."CompanyId", dup_record."CreatedAt");
            
            RAISE NOTICE 'Will reassign License Detail ID % from % to %', 
                dup_record."Id", dup_license, new_license;
        END LOOP;
    END LOOP;
END $$;

-- Step 4: Display the reassignment plan
SELECT * FROM license_reassignments ORDER BY license_detail_id;

-- Step 5: Apply the reassignments
UPDATE org."LicenseDetails" ld
SET "LicenseNumber" = lr.new_license_number
FROM license_reassignments lr
WHERE ld."Id" = lr.license_detail_id;

-- Step 6: Verify no duplicates remain
DO $$
DECLARE
    remaining_duplicates INTEGER;
BEGIN
    SELECT COUNT(*) INTO remaining_duplicates
    FROM (
        SELECT "LicenseNumber"
        FROM org."LicenseDetails"
        WHERE "LicenseNumber" IS NOT NULL
        GROUP BY "LicenseNumber"
        HAVING COUNT(*) > 1
    ) dups;
    
    IF remaining_duplicates > 0 THEN
        RAISE EXCEPTION 'Still have % duplicate license numbers after fix!', remaining_duplicates;
    ELSE
        RAISE NOTICE 'SUCCESS: All duplicates have been fixed!';
    END IF;
END $$;

-- Step 7: Show summary
SELECT 
    COUNT(*) as total_reassigned,
    MIN(old_license_number) as first_duplicate,
    MAX(old_license_number) as last_duplicate
FROM license_reassignments;

COMMIT;

-- After running this successfully, run: add_unique_constraint_license_number.sql
