-- Script to update existing petition writer license numbers to new format
-- Format: PROVINCE_CODE-00000001 (8 digits with leading zeros)
-- This script updates all existing licenses with province-based sequential numbers

-- Step 1: Create a temporary function to get province code
CREATE OR REPLACE FUNCTION get_province_code(province_name TEXT) 
RETURNS TEXT AS $$
BEGIN
    RETURN CASE province_name
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
        ELSE UPPER(SUBSTRING(province_name, 1, 3))
    END;
END;
$$ LANGUAGE plpgsql;

-- Step 2: Show current state before update
SELECT 
    COUNT(*) as total_licenses,
    COUNT(DISTINCT "ProvinceId") as provinces_with_licenses,
    COUNT(CASE WHEN "LicenseNumber" IS NULL OR "LicenseNumber" = '' THEN 1 END) as licenses_without_number
FROM "PetitionWriterLicenses"
WHERE "Status" = true;

-- Step 3: Show breakdown by province
SELECT 
    l."Name" as province_name,
    COUNT(pwl."Id") as license_count
FROM "PetitionWriterLicenses" pwl
LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
WHERE pwl."Status" = true
GROUP BY l."Name"
ORDER BY license_count DESC;

-- Step 4: Update license numbers with province-based sequential format
-- This uses ROW_NUMBER() to assign sequential numbers within each province
WITH numbered_licenses AS (
    SELECT 
        pwl."Id",
        l."Name" as province_name,
        get_province_code(l."Name") as province_code,
        ROW_NUMBER() OVER (PARTITION BY pwl."ProvinceId" ORDER BY pwl."CreatedAt", pwl."Id") as seq_num
    FROM "PetitionWriterLicenses" pwl
    LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
    WHERE pwl."Status" = true
)
UPDATE "PetitionWriterLicenses" pwl
SET 
    "LicenseNumber" = nl.province_code || '-' || LPAD(nl.seq_num::TEXT, 8, '0'),
    "UpdatedAt" = NOW(),
    "UpdatedBy" = 'system_migration'
FROM numbered_licenses nl
WHERE pwl."Id" = nl."Id";

-- Step 5: Verify the update
SELECT 
    l."Name" as province_name,
    get_province_code(l."Name") as province_code,
    COUNT(pwl."Id") as total_licenses,
    MIN(pwl."LicenseNumber") as first_license,
    MAX(pwl."LicenseNumber") as last_license
FROM "PetitionWriterLicenses" pwl
LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
WHERE pwl."Status" = true
GROUP BY l."Name", l."Id"
ORDER BY total_licenses DESC;

-- Step 6: Check for any duplicates (should be 0)
SELECT 
    "LicenseNumber",
    COUNT(*) as duplicate_count
FROM "PetitionWriterLicenses"
WHERE "Status" = true
GROUP BY "LicenseNumber"
HAVING COUNT(*) > 1;

-- Step 7: Check for any licenses without numbers (should be 0)
SELECT COUNT(*) as licenses_without_number
FROM "PetitionWriterLicenses"
WHERE "Status" = true 
AND ("LicenseNumber" IS NULL OR "LicenseNumber" = '');

-- Step 8: Show sample of updated licenses
SELECT 
    pwl."Id",
    l."Name" as province_name,
    pwl."LicenseNumber",
    pwl."ApplicantName",
    pwl."CreatedAt"
FROM "PetitionWriterLicenses" pwl
LEFT JOIN "Locations" l ON pwl."ProvinceId" = l."Id"
WHERE pwl."Status" = true
ORDER BY l."Name", pwl."LicenseNumber"
LIMIT 20;

-- Step 9: Clean up temporary function
DROP FUNCTION IF EXISTS get_province_code(TEXT);

-- Summary
SELECT 
    'Migration completed successfully!' as status,
    COUNT(*) as total_updated_licenses,
    COUNT(DISTINCT "ProvinceId") as provinces_updated
FROM "PetitionWriterLicenses"
WHERE "Status" = true;
