-- =====================================================
-- COMPLETE FIX: Convert ALL Remaining Double Precision Columns to TEXT
-- =====================================================
-- This script converts ALL remaining numeric columns across all schemas
-- Run this in your development database NOW
-- =====================================================

-- Step 1: Drop dependent views first
DROP VIEW IF EXISTS public."LicenseView";

-- Step 2: Convert org schema columns
-- CompanyDetails.TIN
ALTER TABLE org."CompanyDetails" 
    ALTER COLUMN "TIN" TYPE text USING "TIN"::text;

-- CompanyOwner.IndentityCardNumber
ALTER TABLE org."CompanyOwner" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Guarantors.IndentityCardNumber
ALTER TABLE org."Guarantors" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Step 3: Convert tr schema vehicle columns
-- VehiclesBuyerDetails.IndentityCardNumber
ALTER TABLE tr."VehiclesBuyerDetails" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- VehiclesSellerDetails.IndentityCardNumber
ALTER TABLE tr."VehiclesSellerDetails" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- VehiclesWitnessDetails.IndentityCardNumber
ALTER TABLE tr."VehiclesWitnessDetails" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Step 4: Recreate LicenseView with all fields
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

-- Step 5: Verify - should return NO rows
SELECT 
    table_schema,
    table_name, 
    column_name, 
    data_type
FROM information_schema.columns
WHERE data_type = 'double precision'
    AND table_schema IN ('tr', 'org', 'look', 'sec', 'audit')
ORDER BY table_schema, table_name, column_name;

-- If the above query returns rows, those columns still need conversion
