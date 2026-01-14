-- Script to fix/recreate the LicenseView
-- Run this if you get a 500 error on GetLicenseView endpoint

-- First, check if the required columns exist in CompanyOwner table
DO $$
BEGIN
    -- Add PermanentProvinceId if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'PermanentProvinceId') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "PermanentProvinceId" INTEGER;
    END IF;

    -- Add PermanentDistrictId if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'PermanentDistrictId') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "PermanentDistrictId" INTEGER;
    END IF;

    -- Add PermanentVillage if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'PermanentVillage') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "PermanentVillage" VARCHAR(255);
    END IF;

    -- Add TemporaryProvinceId if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'TemporaryProvinceId') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "TemporaryProvinceId" INTEGER;
    END IF;

    -- Add TemporaryDistrictId if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'TemporaryDistrictId') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "TemporaryDistrictId" INTEGER;
    END IF;

    -- Add TemporaryVillage if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'TemporaryVillage') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "TemporaryVillage" VARCHAR(255);
    END IF;

    -- Add PhoneNumber if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'PhoneNumber') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "PhoneNumber" VARCHAR(20);
    END IF;

    -- Add WhatsAppNumber if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
                   AND column_name = 'WhatsAppNumber') THEN
        ALTER TABLE org."CompanyOwner" ADD COLUMN "WhatsAppNumber" VARCHAR(20);
    END IF;
END $$;

-- Check if LicenseDetails has the required financial columns
DO $$
BEGIN
    -- Add RoyaltyAmount if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'LicenseDetails' 
                   AND column_name = 'RoyaltyAmount') THEN
        ALTER TABLE org."LicenseDetails" ADD COLUMN "RoyaltyAmount" DECIMAL(18,2);
    END IF;

    -- Add RoyaltyDate if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'LicenseDetails' 
                   AND column_name = 'RoyaltyDate') THEN
        ALTER TABLE org."LicenseDetails" ADD COLUMN "RoyaltyDate" DATE;
    END IF;

    -- Add PenaltyAmount if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'LicenseDetails' 
                   AND column_name = 'PenaltyAmount') THEN
        ALTER TABLE org."LicenseDetails" ADD COLUMN "PenaltyAmount" DECIMAL(18,2);
    END IF;

    -- Add PenaltyDate if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'LicenseDetails' 
                   AND column_name = 'PenaltyDate') THEN
        ALTER TABLE org."LicenseDetails" ADD COLUMN "PenaltyDate" DATE;
    END IF;

    -- Add HrLetter if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'LicenseDetails' 
                   AND column_name = 'HrLetter') THEN
        ALTER TABLE org."LicenseDetails" ADD COLUMN "HrLetter" VARCHAR(100);
    END IF;

    -- Add HrLetterDate if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                   WHERE table_schema = 'org' AND table_name = 'LicenseDetails' 
                   AND column_name = 'HrLetterDate') THEN
        ALTER TABLE org."LicenseDetails" ADD COLUMN "HrLetterDate" DATE;
    END IF;
END $$;

-- Drop and recreate the LicenseView
DROP VIEW IF EXISTS public."LicenseView";

CREATE OR REPLACE VIEW public."LicenseView" AS
SELECT 
    cd."Id" AS "CompanyId",
    co."PhoneNumber",
    co."WhatsAppNumber",
    cd."Title",
    cd."TIN",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    co."DateofBirth",
    co."IndentityCardNumber",
    co."PothoPath" AS "OwnerPhoto",
    ld."LicenseNumber",
    ld."OfficeAddress",
    ld."IssueDate",
    ld."ExpireDate",
    pp."Dari" AS "PermanentProvinceName",
    pd."Dari" AS "PermanentDistrictName",
    co."PermanentVillage",
    tp."Dari" AS "TemporaryProvinceName",
    td."Dari" AS "TemporaryDistrictName",
    co."TemporaryVillage",
    -- Financial and Administrative Fields
    ld."RoyaltyAmount",
    ld."RoyaltyDate",
    ld."PenaltyAmount",
    ld."PenaltyDate",
    ld."HrLetter",
    ld."HrLetterDate"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
LEFT JOIN look."Location" pp ON co."PermanentProvinceId" = pp."ID"
LEFT JOIN look."Location" pd ON co."PermanentDistrictId" = pd."ID"
LEFT JOIN look."Location" tp ON co."TemporaryProvinceId" = tp."ID"
LEFT JOIN look."Location" td ON co."TemporaryDistrictId" = td."ID";

-- Verify the view was created
SELECT 'LicenseView created successfully' AS status;
