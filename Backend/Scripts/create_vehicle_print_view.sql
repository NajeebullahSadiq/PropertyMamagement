-- Create getVehiclePrintData view for vehicle printing
-- This view combines vehicle details with seller, buyer, and witness information

-- Drop the view if it exists
DROP VIEW IF EXISTS "getVehiclePrintData";

-- Create the view
CREATE VIEW "getVehiclePrintData" AS
SELECT 
    -- Vehicle Details
    v."Id",
    v."PermitNo",
    v."PilateNo",
    v."TypeOfVehicle",
    v."Model",
    v."EnginNo",
    v."ShasiNo",
    v."Color",
    v."Des" AS "Description",
    v."Price",
    v."PriceText",
    v."HalfPrice",
    v."RoyaltyAmount",
    v."CreatedAt",
    
    -- Seller Information (first seller only)
    s."FirstName" AS "SellerFirstName",
    s."FatherName" AS "SellerFatherName",
    s."ElectronicNationalIdNumber" AS "SellerIndentityCardNumber",
    s."PaddressVillage" AS "SellerVillage",
    s."TaddressVillage" AS "tSellerVillage",
    s."Photo" AS "SellerPhoto",
    sp."Dari" AS "SellerProvince",
    sd."Dari" AS "SellerDistrict",
    tsp."Dari" AS "tSellerProvince",
    tsd."Dari" AS "tSellerDistrict",
    
    -- Buyer Information (first buyer only)
    b."FirstName" AS "BuyerFirstName",
    b."FatherName" AS "BuyerFatherName",
    b."ElectronicNationalIdNumber" AS "BuyerIndentityCardNumber",
    b."PaddressVillage" AS "BuyerVillage",
    b."TaddressVillage" AS "tBuyerVillage",
    b."Photo" AS "BuyerPhoto",
    bp."Dari" AS "BuyerProvince",
    bd."Dari" AS "BuyerDistrict",
    tbp."Dari" AS "tBuyerProvince",
    tbd."Dari" AS "tBuyerDistrict",
    
    -- Witness 1 Information
    w1."FirstName" AS "WitnessOneFirstName",
    w1."FatherName" AS "WitnessOneFatherName",
    w1."ElectronicNationalIdNumber" AS "WitnessOneIndentityCardNumber",
    
    -- Witness 2 Information
    w2."FirstName" AS "WitnessTwoFirstName",
    w2."FatherName" AS "WitnessTwoFatherName",
    w2."ElectronicNationalIdNumber" AS "WitnessTwoIndentityCardNumber"
    
FROM tr."VehiclesPropertyDetails" v

-- Join Seller (first seller only)
LEFT JOIN LATERAL (
    SELECT 
        "Id",
        "FirstName",
        "FatherName",
        "ElectronicNationalIdNumber",
        "PaddressVillage",
        "TaddressVillage",
        "Photo",
        "PaddressProvinceId",
        "PaddressDistrictId",
        "TaddressProvinceId",
        "TaddressDistrictId"
    FROM tr."VehiclesSellerDetails" 
    WHERE "PropertyDetailsId" = v."Id" 
    ORDER BY "Id" 
    LIMIT 1
) s ON true
LEFT JOIN look."Location" sp ON s."PaddressProvinceId" = sp."ID"
LEFT JOIN look."Location" sd ON s."PaddressDistrictId" = sd."ID"
LEFT JOIN look."Location" tsp ON s."TaddressProvinceId" = tsp."ID"
LEFT JOIN look."Location" tsd ON s."TaddressDistrictId" = tsd."ID"

-- Join Buyer (first buyer only)
LEFT JOIN LATERAL (
    SELECT 
        "Id",
        "FirstName",
        "FatherName",
        "ElectronicNationalIdNumber",
        "PaddressVillage",
        "TaddressVillage",
        "Photo",
        "PaddressProvinceId",
        "PaddressDistrictId",
        "TaddressProvinceId",
        "TaddressDistrictId"
    FROM tr."VehiclesBuyerDetails" 
    WHERE "PropertyDetailsId" = v."Id" 
    ORDER BY "Id" 
    LIMIT 1
) b ON true
LEFT JOIN look."Location" bp ON b."PaddressProvinceId" = bp."ID"
LEFT JOIN look."Location" bd ON b."PaddressDistrictId" = bd."ID"
LEFT JOIN look."Location" tbp ON b."TaddressProvinceId" = tbp."ID"
LEFT JOIN look."Location" tbd ON b."TaddressDistrictId" = tbd."ID"

-- Join Witness 1 (first witness)
LEFT JOIN LATERAL (
    SELECT * FROM tr."VehiclesWitnessDetails" 
    WHERE "PropertyDetailsId" = v."Id" 
    ORDER BY "Id" 
    LIMIT 1
) w1 ON true

-- Join Witness 2 (second witness)
LEFT JOIN LATERAL (
    SELECT * FROM tr."VehiclesWitnessDetails" 
    WHERE "PropertyDetailsId" = v."Id" 
    ORDER BY "Id" 
    OFFSET 1 
    LIMIT 1
) w2 ON true;

-- Verify the view was created
SELECT 'View getVehiclePrintData created successfully' AS status;

-- Test query
SELECT COUNT(*) AS total_records FROM "getVehiclePrintData";
