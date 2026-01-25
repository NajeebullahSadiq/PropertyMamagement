-- =====================================================
-- PRODUCTION SCRIPT: Convert All Numeric Columns to TEXT
-- =====================================================
-- Purpose: Convert all double precision columns to TEXT type
-- Database: PRMIS
-- Date: 2026-01-24
-- 
-- IMPORTANT: Run this script in your production database
-- This script will:
-- 1. Drop dependent views (to avoid dependency errors)
-- 2. Convert all numeric columns to TEXT type
-- 3. Recreate all views with correct types
--
-- Tables affected:
-- - tr.PropertyDetails (2 columns)
-- - tr.BuyerDetails (5 columns)
-- - tr.SellerDetails (5 columns)
-- - tr.VehiclesPropertyDetails (2 columns)
-- - tr.VehiclesBuyerDetails (4 columns: Price, RoyaltyAmount, HalfPrice, IndentityCardNumber)
-- - tr.VehiclesSellerDetails (4 columns: Price, RoyaltyAmount, HalfPrice, IndentityCardNumber)
-- - tr.VehiclesWitnessDetails (1 column: IndentityCardNumber)
-- - org.CompanyDetails (1 column: TIN)
-- - org.CompanyOwner (1 column: IndentityCardNumber)
-- - org.Guarantors (1 column: IndentityCardNumber)
-- Total: 29 columns across 10 tables
-- =====================================================

-- Step 1: Drop dependent views
DROP VIEW IF EXISTS "GetPrintType";
DROP VIEW IF EXISTS public."LicenseView";

-- Step 2: Convert PropertyDetails columns
ALTER TABLE tr."PropertyDetails" 
    ALTER COLUMN "Price" TYPE text USING "Price"::text,
    ALTER COLUMN "RoyaltyAmount" TYPE text USING "RoyaltyAmount"::text;

-- Step 3: Convert BuyerDetails columns
ALTER TABLE tr."BuyerDetails" 
    ALTER COLUMN "Price" TYPE text USING "Price"::text,
    ALTER COLUMN "RoyaltyAmount" TYPE text USING "RoyaltyAmount"::text,
    ALTER COLUMN "HalfPrice" TYPE text USING "HalfPrice"::text,
    ALTER COLUMN "SharePercentage" TYPE text USING "SharePercentage"::text,
    ALTER COLUMN "ShareAmount" TYPE text USING "ShareAmount"::text;

-- Step 4: Convert SellerDetails columns
ALTER TABLE tr."SellerDetails" 
    ALTER COLUMN "Price" TYPE text USING "Price"::text,
    ALTER COLUMN "RoyaltyAmount" TYPE text USING "RoyaltyAmount"::text,
    ALTER COLUMN "HalfPrice" TYPE text USING "HalfPrice"::text,
    ALTER COLUMN "SharePercentage" TYPE text USING "SharePercentage"::text,
    ALTER COLUMN "ShareAmount" TYPE text USING "ShareAmount"::text;

-- Step 5: Convert VehiclesPropertyDetails columns
ALTER TABLE tr."VehiclesPropertyDetails" 
    ALTER COLUMN "Price" TYPE text USING "Price"::text,
    ALTER COLUMN "RoyaltyAmount" TYPE text USING "RoyaltyAmount"::text;

-- Step 6: Convert VehiclesBuyerDetails columns
ALTER TABLE tr."VehiclesBuyerDetails" 
    ALTER COLUMN "Price" TYPE text USING "Price"::text,
    ALTER COLUMN "RoyaltyAmount" TYPE text USING "RoyaltyAmount"::text,
    ALTER COLUMN "HalfPrice" TYPE text USING "HalfPrice"::text,
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Step 7: Convert VehiclesSellerDetails columns
ALTER TABLE tr."VehiclesSellerDetails" 
    ALTER COLUMN "Price" TYPE text USING "Price"::text,
    ALTER COLUMN "RoyaltyAmount" TYPE text USING "RoyaltyAmount"::text,
    ALTER COLUMN "HalfPrice" TYPE text USING "HalfPrice"::text,
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Step 8: Convert VehiclesWitnessDetails columns
ALTER TABLE tr."VehiclesWitnessDetails" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Step 9: Convert Company/License columns (org schema)
ALTER TABLE org."CompanyDetails" 
    ALTER COLUMN "TIN" TYPE text USING "TIN"::text;

ALTER TABLE org."CompanyOwner" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

ALTER TABLE org."Guarantors" 
    ALTER COLUMN "IndentityCardNumber" TYPE text USING "IndentityCardNumber"::text;

-- Step 10: Recreate LicenseView with all fields
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
    co."IndentityCardNumber",
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
    pp."Dari" AS "PermanentProvinceName",
    pd."Dari" AS "PermanentDistrictName",
    co."PermanentVillage"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
LEFT JOIN look."Location" pp ON co."PermanentProvinceId" = pp."ID"
LEFT JOIN look."Location" pd ON co."PermanentDistrictId" = pd."ID";

-- Step 11: Recreate GetPrintType view with correct types
CREATE OR REPLACE VIEW "GetPrintType" AS
SELECT
    pd."Id",
    pd."DocumentType" as "Doctype",
    COALESCE(pd."PNumber", '') as "Pnumber",
    COALESCE(pd."PArea", '') as "PArea",
    pd."NumofRooms",
    pd."north" as "north",
    pd."south" as "south",
    pd."west" as "west",
    pd."east" as "east",
    pd."Price",
    pd."PriceText",
    pd."RoyaltyAmount",
    pt."Name" as "PropertypeType",
    pd."CreatedAt",
    pd."TransactionDate" as "DeedDate",
    pd."FilePath",
    pd."PreviousDocumentsPath",
    pd."ExistingDocumentsPath",
    pd."DocumentType",
    pd."IssuanceNumber",
    pd."IssuanceDate",
    pd."SerialNumber",
    pd."TransactionDate",
    COALESCE(pd."PNumber", '') as "PNumber",
    pa_prov."Name" as "Province",
    pa_dist."Name" as "District",
    pa_prov."Dari" as "ProvinceDari",
    pa_dist."Dari" as "DistrictDari",
    pa."Village",
    sd."FirstName" as "SellerFirstName",
    sd."FatherName" as "SellerFatherName",
    sd."ElectronicNationalIdNumber" as "SellerIndentityCardNumber",
    sd."PaddressVillage" as "SellerVillage",
    sd."TaddressVillage" as "TSellerVillage",
    sd."photo" as "SellerPhoto",
    s_perm_prov."Name" as "SellerProvince",
    s_perm_dist."Name" as "SellerDistrict",
    s_perm_prov."Dari" as "SellerProvinceDari",
    s_perm_dist."Dari" as "SellerDistrictDari",
    s_temp_prov."Name" as "TSellerProvince",
    s_temp_dist."Name" as "TSellerDistrict",
    s_temp_prov."Dari" as "TSellerProvinceDari",
    s_temp_dist."Dari" as "TSellerDistrictDari",
    bd."FirstName" as "BuyerFirstName",
    bd."FatherName" as "BuyerFatherName",
    bd."ElectronicNationalIdNumber" as "BuyerIndentityCardNumber",
    bd."PaddressVillage" as "BuyerVillage",
    bd."TaddressVillage" as "TBuyerVillage",
    bd."photo" as "BuyerPhoto",
    b_perm_prov."Name" as "BuyerProvince",
    b_perm_dist."Name" as "BuyerDistrict",
    b_perm_prov."Dari" as "BuyerProvinceDari",
    b_perm_dist."Dari" as "BuyerDistrictDari",
    b_temp_prov."Name" as "TBuyerProvince",
    b_temp_dist."Name" as "TBuyerDistrict",
    b_temp_prov."Dari" as "TBuyerProvinceDari",
    b_temp_dist."Dari" as "TBuyerDistrictDari",
    wd1."FirstName" as "WitnessOneFirstName",
    wd1."FatherName" as "WitnessOneFatherName",
    wd1."ElectronicNationalIdNumber" as "WitnessOneIndentityCardNumber",
    wd2."FirstName" as "WitnessTwoFirstName",
    wd2."FatherName" as "WitnessTwoFatherName",
    wd2."ElectronicNationalIdNumber" as "WitnessTwoIndentityCardNumber",
    ut."Name" as "UnitType",
    tt."Name" as "TransactionType"
FROM tr."PropertyDetails" pd
LEFT JOIN look."PropertyType" pt ON pd."PropertyTypeId" = pt."Id"
LEFT JOIN look."PUnitType" ut ON pd."PUnitTypeId" = ut."Id"
LEFT JOIN look."TransactionType" tt ON pd."TransactionTypeId" = tt."Id"
LEFT JOIN tr."PropertyAddress" pa ON pd."Id" = pa."PropertyDetailsId"
LEFT JOIN look."Location" pa_prov ON pa."ProvinceId" = pa_prov."ID"
LEFT JOIN look."Location" pa_dist ON pa."DistrictId" = pa_dist."ID"
LEFT JOIN tr."SellerDetails" sd ON pd."Id" = sd."PropertyDetailsId"
LEFT JOIN look."Location" s_perm_prov ON sd."PaddressProvinceId" = s_perm_prov."ID"
LEFT JOIN look."Location" s_perm_dist ON sd."PaddressDistrictId" = s_perm_dist."ID"
LEFT JOIN look."Location" s_temp_prov ON sd."TaddressProvinceId" = s_temp_prov."ID"
LEFT JOIN look."Location" s_temp_dist ON sd."TaddressDistrictId" = s_temp_dist."ID"
LEFT JOIN tr."BuyerDetails" bd ON pd."Id" = bd."PropertyDetailsId"
LEFT JOIN look."Location" b_perm_prov ON bd."PaddressProvinceId" = b_perm_prov."ID"
LEFT JOIN look."Location" b_perm_dist ON bd."PaddressDistrictId" = b_perm_dist."ID"
LEFT JOIN look."Location" b_temp_prov ON bd."TaddressProvinceId" = b_temp_prov."ID"
LEFT JOIN look."Location" b_temp_dist ON bd."TaddressDistrictId" = b_temp_dist."ID"
LEFT JOIN LATERAL (
    SELECT "FirstName", "FatherName",
           "ElectronicNationalIdNumber" as "IndentityCardNumber",
           "ElectronicNationalIdNumber"
    FROM tr."WitnessDetails"
    WHERE "PropertyDetailsId" = pd."Id"
    ORDER BY "Id" ASC 
    LIMIT 1
) wd1 ON true
LEFT JOIN LATERAL (
    SELECT "FirstName", "FatherName",
           "ElectronicNationalIdNumber" as "IndentityCardNumber",
           "ElectronicNationalIdNumber"
    FROM tr."WitnessDetails"
    WHERE "PropertyDetailsId" = pd."Id"
    ORDER BY "Id" ASC 
    OFFSET 1 LIMIT 1
) wd2 ON true
WHERE pd."iscomplete" = true;

-- Step 12: Verify conversion
SELECT 
    table_schema,
    table_name, 
    column_name, 
    data_type
FROM information_schema.columns
WHERE table_schema IN ('tr', 'org')
    AND table_name IN (
        'PropertyDetails', 
        'BuyerDetails', 
        'SellerDetails',
        'VehiclesPropertyDetails',
        'VehiclesBuyerDetails',
        'VehiclesSellerDetails',
        'VehiclesWitnessDetails',
        'CompanyDetails',
        'CompanyOwner',
        'Guarantors'
    )
    AND column_name IN (
        'Price', 
        'RoyaltyAmount', 
        'HalfPrice', 
        'SharePercentage', 
        'ShareAmount',
        'IndentityCardNumber',
        'TIN'
    )
ORDER BY table_schema, table_name, column_name;

-- Expected result: All 29 columns should show data_type = 'text'
