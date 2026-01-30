-- Check PropertyAddresses table for property ID 6
SELECT * FROM tr."PropertyAddresses" 
WHERE "PropertyDetailsId" = 6;

-- Check all PropertyAddresses
SELECT * FROM tr."PropertyAddresses" 
ORDER BY "PropertyDetailsId";

-- Check PropertyDetails table
SELECT "Id", "Pnumber", "CreatedAt", "CreatedBy" 
FROM tr."PropertyDetails" 
WHERE "Id" = 6;
