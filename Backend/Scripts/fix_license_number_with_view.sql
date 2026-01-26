-- =====================================================
-- Fix LicenseNumber Type with View Recreation
-- This fixes the 500 error on /api/CompanyDetails
-- =====================================================

-- Step 1: Drop the view that depends on LicenseNumber
DROP VIEW IF EXISTS public."LicenseView" CASCADE;

-- Step 2: Convert LicenseNumber from double precision to TEXT
ALTER TABLE org."LicenseDetails" 
ALTER COLUMN "LicenseNumber" TYPE TEXT 
USING CASE 
    WHEN "LicenseNumber" IS NULL THEN NULL
    ELSE "LicenseNumber"::TEXT
END;

-- Step 3: Recreate the LicenseView
CREATE VIEW public."LicenseView" AS
SELECT 
    cd."Id" AS "CompanyId",
    co."PhoneNumber",
    co."WhatsAppNumber",
    cd."Title",
    cd."TIN" AS "Tin",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    co."DateofBirth",
    co."ElectronicNationalIdNumber" AS "IndentityCardNumber",
    co."PothoPath" AS "OwnerPhoto",
    ld."LicenseNumber",
    ld."OfficeAddress",
    ld."IssueDate",
    ld."ExpireDate",
    ld."RoyaltyAmount",
    ld."RoyaltyDate",
    ld."TariffNumber",
    ld."PenaltyAmount",
    ld."PenaltyDate",
    ld."HrLetter",
    ld."HrLetterDate",
    -- Owner's Own Address (آدرس اصلی مالک)
    op."Dari" AS "OwnerProvinceName",
    od."Dari" AS "OwnerDistrictName",
    co."OwnerVillage",
    -- Permanent/Current Residence Address (آدرس فعلی سکونت)
    pp."Dari" AS "PermanentProvinceName",
    pd."Dari" AS "PermanentDistrictName",
    co."PermanentVillage"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
-- Owner's Own Address joins
LEFT JOIN look."Location" op ON co."OwnerProvinceId" = op."ID"
LEFT JOIN look."Location" od ON co."OwnerDistrictId" = od."ID"
-- Permanent Address joins
LEFT JOIN look."Location" pp ON co."PermanentProvinceId" = pp."ID"
LEFT JOIN look."Location" pd ON co."PermanentDistrictId" = pd."ID";

-- Step 4: Verify the column type change
SELECT 
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'LicenseDetails'
AND column_name = 'LicenseNumber';

-- Step 5: Verify no more double precision columns remain
SELECT 
    table_name,
    column_name,
    data_type
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND data_type = 'double precision'
ORDER BY table_name, column_name;

-- Step 6: Verify the view was recreated
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'LicenseView' 
  AND table_schema = 'public'
ORDER BY ordinal_position;
