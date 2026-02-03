-- Test query to verify vehicle print view is returning photo data

-- First, check what's in the actual tables
SELECT 
    v."Id" as vehicle_id,
    v."PermitNo",
    s."Id" as seller_id,
    s."FirstName" as seller_name,
    s."Photo" as seller_photo,
    b."Id" as buyer_id,
    b."FirstName" as buyer_name,
    b."Photo" as buyer_photo
FROM tr."VehiclesPropertyDetails" v
LEFT JOIN tr."VehiclesSellerDetails" s ON s."PropertyDetailsId" = v."Id"
LEFT JOIN tr."VehiclesBuyerDetails" b ON b."PropertyDetailsId" = v."Id"
WHERE v."Id" = 2;

-- Now check what the view returns
SELECT 
    "Id",
    "PermitNo",
    "SellerFirstName",
    "SellerPhoto",
    "BuyerFirstName",
    "BuyerPhoto"
FROM "getVehiclePrintData"
WHERE "Id" = 2;
