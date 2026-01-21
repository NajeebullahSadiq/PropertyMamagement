-- Fix ElectronicNationalIdNumber columns in Property module tables
-- This script renames IndentityCardNumber to ElectronicNationalIdNumber
-- and converts the type from double precision to VARCHAR(50)
-- Step 1: Drop dependent views
-- Step 2: Rename columns and convert types
-- Step 3: Recreate views with updated column names

-- Step 1: Drop the GetPrintType view if it exists
DROP VIEW IF EXISTS "GetPrintType" CASCADE;

-- Step 2: Rename columns and convert types
DO $$
BEGIN
    -- SellerDetails table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'SellerDetails' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE tr."SellerDetails" 
        RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";
        
        RAISE NOTICE 'Renamed IndentityCardNumber to ElectronicNationalIdNumber in SellerDetails';
    ELSIF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'SellerDetails' 
        AND column_name = 'ElectronicNationalIdNumber'
    ) THEN
        ALTER TABLE tr."SellerDetails" 
        ADD COLUMN "ElectronicNationalIdNumber" VARCHAR(50);
        
        RAISE NOTICE 'Added ElectronicNationalIdNumber column to SellerDetails';
    ELSE
        RAISE NOTICE 'ElectronicNationalIdNumber already exists in SellerDetails';
    END IF;

    -- Convert type if needed for SellerDetails
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'SellerDetails' 
        AND column_name = 'ElectronicNationalIdNumber'
        AND data_type != 'character varying'
    ) THEN
        ALTER TABLE tr."SellerDetails" 
        ALTER COLUMN "ElectronicNationalIdNumber" TYPE VARCHAR(50) USING "ElectronicNationalIdNumber"::VARCHAR(50);
        
        RAISE NOTICE 'Converted ElectronicNationalIdNumber to VARCHAR(50) in SellerDetails';
    END IF;

    -- BuyerDetails table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'BuyerDetails' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE tr."BuyerDetails" 
        RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";
        
        RAISE NOTICE 'Renamed IndentityCardNumber to ElectronicNationalIdNumber in BuyerDetails';
    ELSIF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'BuyerDetails' 
        AND column_name = 'ElectronicNationalIdNumber'
    ) THEN
        ALTER TABLE tr."BuyerDetails" 
        ADD COLUMN "ElectronicNationalIdNumber" VARCHAR(50);
        
        RAISE NOTICE 'Added ElectronicNationalIdNumber column to BuyerDetails';
    ELSE
        RAISE NOTICE 'ElectronicNationalIdNumber already exists in BuyerDetails';
    END IF;

    -- Convert type if needed for BuyerDetails
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'BuyerDetails' 
        AND column_name = 'ElectronicNationalIdNumber'
        AND data_type != 'character varying'
    ) THEN
        ALTER TABLE tr."BuyerDetails" 
        ALTER COLUMN "ElectronicNationalIdNumber" TYPE VARCHAR(50) USING "ElectronicNationalIdNumber"::VARCHAR(50);
        
        RAISE NOTICE 'Converted ElectronicNationalIdNumber to VARCHAR(50) in BuyerDetails';
    END IF;

    -- WitnessDetails table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'WitnessDetails' 
        AND column_name = 'IndentityCardNumber'
    ) THEN
        ALTER TABLE tr."WitnessDetails" 
        RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";
        
        RAISE NOTICE 'Renamed IndentityCardNumber to ElectronicNationalIdNumber in WitnessDetails';
    ELSIF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'WitnessDetails' 
        AND column_name = 'ElectronicNationalIdNumber'
    ) THEN
        ALTER TABLE tr."WitnessDetails" 
        ADD COLUMN "ElectronicNationalIdNumber" VARCHAR(50);
        
        RAISE NOTICE 'Added ElectronicNationalIdNumber column to WitnessDetails';
    ELSE
        RAISE NOTICE 'ElectronicNationalIdNumber already exists in WitnessDetails';
    END IF;

    -- Convert type if needed for WitnessDetails
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'WitnessDetails' 
        AND column_name = 'ElectronicNationalIdNumber'
        AND data_type != 'character varying'
    ) THEN
        ALTER TABLE tr."WitnessDetails" 
        ALTER COLUMN "ElectronicNationalIdNumber" TYPE VARCHAR(50) USING "ElectronicNationalIdNumber"::VARCHAR(50);
        
        RAISE NOTICE 'Converted ElectronicNationalIdNumber to VARCHAR(50) in WitnessDetails';
    END IF;

END $$;

-- Step 3: Recreate the GetPrintType view with updated column names
CREATE OR REPLACE VIEW "GetPrintType" AS
SELECT 
    pd."Id",
    pd."DocumentType" as "Doctype",
    pd."PNumber" as "Pnumber",
    pd."PArea",
    pd."NumofRooms",
    pd."north",
    pd."south",
    pd."west",
    pd."east",
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
    pd."PNumber",
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
    SELECT "FirstName", "FatherName", "ElectronicNationalIdNumber"
    FROM tr."WitnessDetails"
    WHERE "PropertyDetailsId" = pd."Id"
    ORDER BY "Id" ASC LIMIT 1
) wd1 ON true
LEFT JOIN LATERAL (
    SELECT "FirstName", "FatherName", "ElectronicNationalIdNumber"
    FROM tr."WitnessDetails"
    WHERE "PropertyDetailsId" = pd."Id"
    ORDER BY "Id" ASC OFFSET 1 LIMIT 1
) wd2 ON true
WHERE pd."iscomplete" = true;
