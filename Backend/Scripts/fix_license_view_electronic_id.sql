-- =============================================
-- Fix LicenseView to properly show Electronic National ID Number
-- Issue: IndentityCardNumber field is not showing in print
-- Solution: Recreate view with correct mapping
-- Date: 2026-01-25
-- =============================================

-- Drop existing view
DROP VIEW IF EXISTS public."LicenseView";

-- Recreate view with correct ElectronicNationalIdNumber mapping
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
    ld."HrLetterDate"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId" AND co."Status" = true
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId" AND ld."Status" = true;

-- Verify the view
SELECT 
    "CompanyId",
    "FirstName",
    "FatherName",
    "IndentityCardNumber",
    "LicenseNumber"
FROM public."LicenseView"
WHERE "IndentityCardNumber" IS NOT NULL
LIMIT 5;

-- Display message
DO $$
BEGIN
    RAISE NOTICE 'LicenseView has been recreated successfully!';
    RAISE NOTICE 'Electronic National ID Number (تذکره الکترونیکی) should now appear in license prints.';
END $$;
