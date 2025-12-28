-- Script to update GetPrintType view to include property image and document fields
-- This script drops and recreates the GetPrintType view with additional columns for property images and documents
-- NOTE: This migration has been created as: 20251227120000_UpdateGetPrintTypeViewWithDocuments.cs
-- You can apply this using: dotnet ef database update

-- Drop the existing view if it exists
DROP VIEW IF EXISTS "GetPrintType";

-- Create the updated view with all necessary fields including property documents
CREATE OR REPLACE VIEW "GetPrintType" AS
SELECT 
    pd."Id",
    pd."DocumentType" as "Doctype",
    pd."PNumber" as "Pnumber",
    pd."Parea",
    pd."NumofRooms",
    pd."North",
    pd."South",
    pd."West",
    pd."East",
    pd."Price",
    pd."PriceText",
    pd."RoyaltyAmount",
    pt."Name" as "PropertypeType",
    pd."CreatedAt",
    pd."TransactionDate" as "DeedDate",
    
    -- Property Images and Documents
    pd."FilePath",
    pd."PreviousDocumentsPath",
    pd."ExistingDocumentsPath",
    pd."DocumentType",
    pd."IssuanceNumber",
    pd."IssuanceDate",
    pd."SerialNumber",
    pd."TransactionDate",
    pd."PNumber",
    
    -- Property Address Information
    pa_prov."Name" as "Province",
    pa_dist."Name" as "District",
    pa."Village",
    
    -- Seller Details
    sd."FirstName" as "SellerFirstName",
    sd."FatherName" as "SellerFatherName",
    sd."IndentityCardNumber" as "SellerIndentityCardNumber",
    sd."Village" as "SellerVillage",
    sd."TempVillage" as "TSellerVillage",
    sd."photo" as "SellerPhoto",
    
    -- Seller Address Information
    s_perm_prov."Name" as "SellerProvince",
    s_perm_dist."Name" as "SellerDistrict",
    s_temp_prov."Name" as "TSellerProvince",
    s_temp_dist."Name" as "TSellerDistrict",
    
    -- Buyer Details
    bd."FirstName" as "BuyerFirstName",
    bd."FatherName" as "BuyerFatherName",
    bd."IndentityCardNumber" as "BuyerIndentityCardNumber",
    bd."Village" as "BuyerVillage",
    bd."TempVillage" as "TBuyerVillage",
    bd."photo" as "BuyerPhoto",
    
    -- Buyer Address Information
    b_perm_prov."Name" as "BuyerProvince",
    b_perm_dist."Name" as "BuyerDistrict",
    b_temp_prov."Name" as "TBuyerProvince",
    b_temp_dist."Name" as "TBuyerDistrict",
    
    -- Witness 1 Details
    wd1."FirstName" as "WitnessOneFirstName",
    wd1."FatherName" as "WitnessOneFatherName",
    wd1."IndentityCardNumber" as "WitnessOneIndentityCardNumber",
    
    -- Witness 2 Details
    wd2."FirstName" as "WitnessTwoFirstName",
    wd2."FatherName" as "WitnessTwoFatherName",
    wd2."IndentityCardNumber" as "WitnessTwoIndentityCardNumber",
    
    -- Unit Type and Transaction Type
    ut."Name" as "UnitType",
    tt."Name" as "TransactionType"

FROM tr."PropertyDetails" pd
LEFT JOIN look."PropertyType" pt ON pd."PropertyTypeId" = pt."Id"
LEFT JOIN look."PunitType" ut ON pd."PunitTypeId" = ut."Id"
LEFT JOIN look."TransactionType" tt ON pd."TransactionTypeId" = tt."Id"

-- Property Address
LEFT JOIN tr."PropertyAddress" pa ON pd."Id" = pa."PropertyDetailsId"
LEFT JOIN look."Province" pa_prov ON pa."ProvinceId" = pa_prov."Id"
LEFT JOIN look."District" pa_dist ON pa."DistrictId" = pa_dist."Id"

-- Seller Details and Address
LEFT JOIN tr."SellerDetails" sd ON pd."Id" = sd."PropertyDetailsId"
LEFT JOIN look."Province" s_perm_prov ON sd."PaddressProvinceId" = s_perm_prov."Id"
LEFT JOIN look."District" s_perm_dist ON sd."PaddressDistrictId" = s_perm_dist."Id"
LEFT JOIN look."Province" s_temp_prov ON sd."TaddressProvinceId" = s_temp_prov."Id"
LEFT JOIN look."District" s_temp_dist ON sd."TaddressDistrictId" = s_temp_dist."Id"

-- Buyer Details and Address
LEFT JOIN tr."BuyerDetails" bd ON pd."Id" = bd."PropertyDetailsId"
LEFT JOIN look."Province" b_perm_prov ON bd."PaddressProvinceId" = b_perm_prov."Id"
LEFT JOIN look."District" b_perm_dist ON bd."PaddressDistrictId" = b_perm_dist."Id"
LEFT JOIN look."Province" b_temp_prov ON bd."TaddressProvinceId" = b_temp_prov."Id"
LEFT JOIN look."District" b_temp_dist ON bd."TaddressDistrictId" = b_temp_dist."Id"

-- Witness 1 Details (First witness record)
LEFT JOIN LATERAL (
    SELECT "FirstName", "FatherName", "IndentityCardNumber"
    FROM tr."WitnessDetails"
    WHERE "PropertyDetailsId" = pd."Id"
    ORDER BY "Id" ASC
    LIMIT 1
) wd1 ON true

-- Witness 2 Details (Second witness record)
LEFT JOIN LATERAL (
    SELECT "FirstName", "FatherName", "IndentityCardNumber"
    FROM tr."WitnessDetails"
    WHERE "PropertyDetailsId" = pd."Id"
    ORDER BY "Id" ASC
    OFFSET 1
    LIMIT 1
) wd2 ON true

WHERE pd."iscomplete" = true;

-- Add comment to the view
COMMENT ON VIEW "GetPrintType" IS 'View for property print data including all property details, images, documents, seller, buyer, and witness information';
