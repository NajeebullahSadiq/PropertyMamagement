-- =====================================================
-- RECREATE LicenseView with Owner Address Fields
-- Run this script in your PostgreSQL database
-- =====================================================

-- Step 1: Drop the existing view
DROP VIEW IF EXISTS public."LicenseView" CASCADE;

-- Step 2: Recreate the view with all address fields
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

-- Step 3: Verify the view was created
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'LicenseView' 
  AND table_schema = 'public'
ORDER BY ordinal_position;

-- Step 4: Test with a sample query
SELECT "CompanyId", "OwnerProvinceName", "OwnerDistrictName", "OwnerVillage",
       "PermanentProvinceName", "PermanentDistrictName", "PermanentVillage"
FROM public."LicenseView" 
LIMIT 1;
