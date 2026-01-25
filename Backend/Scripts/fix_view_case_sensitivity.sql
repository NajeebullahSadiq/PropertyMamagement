-- Fix: Drop both possible views and recreate with correct case
DROP VIEW IF EXISTS public."LicenseView" CASCADE;
DROP VIEW IF EXISTS public.licenseview CASCADE;
DROP VIEW IF EXISTS public."licenseview" CASCADE;

-- Create with explicit case-sensitive name
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

-- Verify it was created with correct case
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_type = 'VIEW'
  AND table_name LIKE '%icense%';

-- Test the view
SELECT "OwnerProvinceName", "OwnerDistrictName", "PermanentProvinceName", "PermanentDistrictName"
FROM public."LicenseView" 
LIMIT 1;
