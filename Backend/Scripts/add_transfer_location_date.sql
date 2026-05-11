-- Add TransferLocationDate column to LicenseDetails table
-- تاریخ محل انتقال - Transfer Location Date

ALTER TABLE org."LicenseDetails" ADD COLUMN IF NOT EXISTS "TransferLocationDate" DATE NULL;

-- Drop and recreate LicenseView to include the new column
-- (Views don't store data, so DROP/CREATE is safe)
DROP VIEW IF EXISTS public."LicenseView";

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
    ld."LicenseCategory",
    ld."OfficeAddress",
    ld."IssueDate",
    ld."ExpireDate",
    ld."DuplicateIssueDate",
    pp."Dari" AS "PermanentProvinceName",
    pd."Dari" AS "PermanentDistrictName",
    co."PermanentVillage",
    ld."RoyaltyAmount",
    ld."RoyaltyDate",
    ld."TariffNumber",
    ld."PenaltyAmount",
    ld."PenaltyDate",
    ld."HrLetter",
    ld."HrLetterDate",
    ld."TransferLocationDate"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
LEFT JOIN look."Location" pp ON co."PermanentProvinceId" = pp."ID"
LEFT JOIN look."Location" pd ON co."PermanentDistrictId" = pd."ID";
