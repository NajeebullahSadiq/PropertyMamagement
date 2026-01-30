-- Check what photo paths are stored in the database for Property module

-- First, let's find the correct schema and table names
SELECT 
    table_schema,
    table_name
FROM information_schema.tables
WHERE table_name ILIKE '%seller%' OR table_name ILIKE '%buyer%' OR table_name ILIKE '%property%'
ORDER BY table_schema, table_name;

-- Check Seller photos (tr schema)
SELECT 
    'Seller' as type,
    "Id",
    "Name",
    "FatherName",
    "Photo",
    "PropertyDetailsId"
FROM tr."SellerDetails"
WHERE "Photo" IS NOT NULL AND "Photo" != ''
ORDER BY "Id" DESC
LIMIT 10;

-- Check Buyer photos (note: lowercase "photo" column)
SELECT 
    'Buyer' as type,
    "Id",
    "Name",
    "FatherName",
    "photo",
    "PropertyDetailsId"
FROM tr."BuyerDetails"
WHERE "photo" IS NOT NULL AND "photo" != ''
ORDER BY "Id" DESC
LIMIT 10;

-- Check Property images
SELECT 
    'Property' as type,
    "Id",
    "FilePath",
    "PreviousDocumentsPath",
    "ExistingDocumentsPath"
FROM tr."PropertyDetails"
WHERE "FilePath" IS NOT NULL OR "PreviousDocumentsPath" IS NOT NULL OR "ExistingDocumentsPath" IS NOT NULL
ORDER BY "Id" DESC
LIMIT 10;
