-- SAFE VERSION: Update petition writer license numbers to new format
-- This version allows you to review before committing
-- Run each section separately and review results before proceeding

-- ============================================
-- SECTION 1: BACKUP CURRENT DATA (RECOMMENDED)
-- ============================================
-- Create a backup table before making changes
CREATE TABLE IF NOT EXISTS "PetitionWriterLicenses_Backup_BeforeLicenseUpdate" AS
SELECT * FROM "PetitionWriterLicenses";

SELECT 'Backup created with ' || COUNT(*) || ' records' as backup_status
FROM "PetitionWriterLicenses_Backup_BeforeLicenseUpdate";

-- ============================================
-- SECTION 2: REVIEW CURRENT STATE
-- ============================================
-- Check current license numbers
SELECT 
    COUNT(*) as total_active_licenses,
    COUNT(CASE WHEN "LicenseNumber" IS NULL OR "LicenseNumber" = '' THEN 1 END) as without_license_number,
    COUNT(CASE WHEN "LicenseNumber" IS NOT NULL AND "LicenseNumber" != '' THEN 1 END) as with_license_number
FROM "PetitionWriterLicenses"
WHERE "Status" = true;

-- Show breakdown by province
SELECT 
    COALESCE(l."Name", 'No Province') as province_name,
    COUNT(pwl."Id") as license_count
FROM "PetitionWriterLicenses" pwl
LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
WHERE pwl."Status" = true
GROUP BY l."Name"
ORDER BY license_count DESC;

-- ============================================
-- SECTION 3: PREVIEW NEW LICENSE NUMBERS
-- ============================================
-- This shows what the new license numbers will be WITHOUT updating
WITH province_codes AS (
    SELECT 
        l."Id" as province_id,
        l."Name" as province_name,
        CASE l."Name"
            WHEN 'Kabul' THEN 'KBL'
            WHEN 'Herat' THEN 'HRT'
            WHEN 'Kandahar' THEN 'KHR'
            WHEN 'Balkh' THEN 'BLK'
            WHEN 'Nangarhar' THEN 'NGR'
            WHEN 'Ghazni' THEN 'GHZ'
            WHEN 'Helmand' THEN 'HLM'
            WHEN 'Badakhshan' THEN 'BDK'
            WHEN 'Takhar' THEN 'TKR'
            WHEN 'Kunduz' THEN 'KDZ'
            WHEN 'Baghlan' THEN 'BGL'
            WHEN 'Bamyan' THEN 'BMN'
            WHEN 'Farah' THEN 'FRH'
            WHEN 'Faryab' THEN 'FRB'
            WHEN 'Ghor' THEN 'GHR'
            WHEN 'Jawzjan' THEN 'JWZ'
            WHEN 'Kapisa' THEN 'KPS'
            WHEN 'Khost' THEN 'KHT'
            WHEN 'Kunar' THEN 'KNR'
            WHEN 'Laghman' THEN 'LGM'
            WHEN 'Logar' THEN 'LGR'
            WHEN 'Nimroz' THEN 'NMZ'
            WHEN 'Nuristan' THEN 'NRS'
            WHEN 'Paktia' THEN 'PKT'
            WHEN 'Paktika' THEN 'PKK'
            WHEN 'Panjshir' THEN 'PNJ'
            WHEN 'Parwan' THEN 'PRW'
            WHEN 'Samangan' THEN 'SMG'
            WHEN 'Sar-e Pol' THEN 'SRP'
            WHEN 'Uruzgan' THEN 'URZ'
            WHEN 'Wardak' THEN 'WRD'
            WHEN 'Zabul' THEN 'ZBL'
            WHEN 'Badghis' THEN 'BDG'
            WHEN 'Daykundi' THEN 'DYK'
            ELSE UPPER(SUBSTRING(l."Name", 1, 3))
        END as province_code
    FROM "Locations" l
    WHERE l."TypeId" = 2
),
numbered_licenses AS (
    SELECT 
        pwl."Id",
        pwl."LicenseNumber" as old_license_number,
        pwl."ApplicantName",
        pc.province_name,
        pc.province_code,
        ROW_NUMBER() OVER (PARTITION BY pwl."ProvinceId" ORDER BY pwl."CreatedAt", pwl."Id") as seq_num,
        pc.province_code || '-' || LPAD(ROW_NUMBER() OVER (PARTITION BY pwl."ProvinceId" ORDER BY pwl."CreatedAt", pwl."Id")::TEXT, 8, '0') as new_license_number
    FROM "PetitionWriterLicenses" pwl
    LEFT JOIN province_codes pc ON pwl."ProvinceId" = pc.province_id
    WHERE pwl."Status" = true
)
SELECT 
    province_name,
    province_code,
    COUNT(*) as total_licenses,
    MIN(new_license_number) as first_license,
    MAX(new_license_number) as last_license
FROM numbered_licenses
GROUP BY province_name, province_code
ORDER BY total_licenses DESC;

-- Show first 50 records with old and new license numbers
WITH province_codes AS (
    SELECT 
        l."Id" as province_id,
        l."Name" as province_name,
        CASE l."Name"
            WHEN 'Kabul' THEN 'KBL'
            WHEN 'Herat' THEN 'HRT'
            WHEN 'Kandahar' THEN 'KHR'
            WHEN 'Balkh' THEN 'BLK'
            WHEN 'Nangarhar' THEN 'NGR'
            ELSE UPPER(SUBSTRING(l."Name", 1, 3))
        END as province_code
    FROM "Locations" l
    WHERE l."TypeId" = 2
)
SELECT 
    pwl."Id",
    pc.province_name,
    pwl."LicenseNumber" as old_license_number,
    pc.province_code || '-' || LPAD(ROW_NUMBER() OVER (PARTITION BY pwl."ProvinceId" ORDER BY pwl."CreatedAt", pwl."Id")::TEXT, 8, '0') as new_license_number,
    pwl."ApplicantName"
FROM "PetitionWriterLicenses" pwl
LEFT JOIN province_codes pc ON pwl."ProvinceId" = pc.province_id
WHERE pwl."Status" = true
ORDER BY pc.province_name, pwl."CreatedAt"
LIMIT 50;

-- ============================================
-- SECTION 4: PERFORM THE UPDATE (RUN THIS AFTER REVIEWING PREVIEW)
-- ============================================
-- IMPORTANT: Review the preview above before running this!
-- This will actually update the license numbers

BEGIN;

WITH province_codes AS (
    SELECT 
        l."Id" as province_id,
        CASE l."Name"
            WHEN 'Kabul' THEN 'KBL'
            WHEN 'Herat' THEN 'HRT'
            WHEN 'Kandahar' THEN 'KHR'
            WHEN 'Balkh' THEN 'BLK'
            WHEN 'Nangarhar' THEN 'NGR'
            WHEN 'Ghazni' THEN 'GHZ'
            WHEN 'Helmand' THEN 'HLM'
            WHEN 'Badakhshan' THEN 'BDK'
            WHEN 'Takhar' THEN 'TKR'
            WHEN 'Kunduz' THEN 'KDZ'
            WHEN 'Baghlan' THEN 'BGL'
            WHEN 'Bamyan' THEN 'BMN'
            WHEN 'Farah' THEN 'FRH'
            WHEN 'Faryab' THEN 'FRB'
            WHEN 'Ghor' THEN 'GHR'
            WHEN 'Jawzjan' THEN 'JWZ'
            WHEN 'Kapisa' THEN 'KPS'
            WHEN 'Khost' THEN 'KHT'
            WHEN 'Kunar' THEN 'KNR'
            WHEN 'Laghman' THEN 'LGM'
            WHEN 'Logar' THEN 'LGR'
            WHEN 'Nimroz' THEN 'NMZ'
            WHEN 'Nuristan' THEN 'NRS'
            WHEN 'Paktia' THEN 'PKT'
            WHEN 'Paktika' THEN 'PKK'
            WHEN 'Panjshir' THEN 'PNJ'
            WHEN 'Parwan' THEN 'PRW'
            WHEN 'Samangan' THEN 'SMG'
            WHEN 'Sar-e Pol' THEN 'SRP'
            WHEN 'Uruzgan' THEN 'URZ'
            WHEN 'Wardak' THEN 'WRD'
            WHEN 'Zabul' THEN 'ZBL'
            WHEN 'Badghis' THEN 'BDG'
            WHEN 'Daykundi' THEN 'DYK'
            ELSE UPPER(SUBSTRING(l."Name", 1, 3))
        END as province_code
    FROM "Locations" l
    WHERE l."TypeId" = 2
),
numbered_licenses AS (
    SELECT 
        pwl."Id",
        pc.province_code || '-' || LPAD(ROW_NUMBER() OVER (PARTITION BY pwl."ProvinceId" ORDER BY pwl."CreatedAt", pwl."Id")::TEXT, 8, '0') as new_license_number
    FROM "PetitionWriterLicenses" pwl
    LEFT JOIN province_codes pc ON pwl."ProvinceId" = pc.province_id
    WHERE pwl."Status" = true
)
UPDATE "PetitionWriterLicenses" pwl
SET 
    "LicenseNumber" = nl.new_license_number,
    "UpdatedAt" = NOW(),
    "UpdatedBy" = 'system_migration'
FROM numbered_licenses nl
WHERE pwl."Id" = nl."Id";

-- Show how many records were updated
SELECT 'Updated ' || COUNT(*) || ' license records' as update_status
FROM "PetitionWriterLicenses"
WHERE "Status" = true;

-- If everything looks good, COMMIT the transaction
-- If something is wrong, run ROLLBACK instead
COMMIT;
-- To undo: ROLLBACK;

-- ============================================
-- SECTION 5: VERIFY THE UPDATE
-- ============================================
-- Check for duplicates (should be 0)
SELECT 
    "LicenseNumber",
    COUNT(*) as count
FROM "PetitionWriterLicenses"
WHERE "Status" = true
GROUP BY "LicenseNumber"
HAVING COUNT(*) > 1;

-- Check for missing license numbers (should be 0)
SELECT COUNT(*) as missing_license_numbers
FROM "PetitionWriterLicenses"
WHERE "Status" = true 
AND ("LicenseNumber" IS NULL OR "LicenseNumber" = '');

-- Show summary by province
SELECT 
    l."Name" as province_name,
    COUNT(pwl."Id") as total_licenses,
    MIN(pwl."LicenseNumber") as first_license,
    MAX(pwl."LicenseNumber") as last_license
FROM "PetitionWriterLicenses" pwl
LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
WHERE pwl."Status" = true
GROUP BY l."Name"
ORDER BY total_licenses DESC;

-- Show sample of updated records
SELECT 
    l."Name" as province_name,
    pwl."LicenseNumber",
    pwl."ApplicantName",
    pwl."UpdatedAt"
FROM "PetitionWriterLicenses" pwl
LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
WHERE pwl."Status" = true
ORDER BY l."Name", pwl."LicenseNumber"
LIMIT 30;

-- ============================================
-- SECTION 6: CLEANUP (OPTIONAL)
-- ============================================
-- After verifying everything is correct, you can drop the backup table
-- ONLY RUN THIS IF YOU'RE SURE EVERYTHING IS CORRECT!
-- DROP TABLE IF EXISTS "PetitionWriterLicenses_Backup_BeforeLicenseUpdate";
