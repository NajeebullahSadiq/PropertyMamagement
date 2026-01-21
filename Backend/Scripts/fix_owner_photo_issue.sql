-- Script to diagnose and fix owner photo issues

-- 1. Check if PothoPath column exists in CompanyOwner table
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
  AND table_name = 'CompanyOwner' 
  AND column_name = 'PothoPath';

-- 2. Check current CompanyOwner records and their photo paths
SELECT 
    co."Id",
    co."FirstName",
    co."FatherName",
    co."CompanyId",
    co."PothoPath",
    cd."Title",
    cd."TIN"
FROM org."CompanyOwner" co
LEFT JOIN org."CompanyDetails" cd ON co."CompanyId" = cd."Id"
ORDER BY co."Id" DESC;

-- 3. Check the LicenseView to see if OwnerPhoto is being populated
SELECT 
    "CompanyId",
    "Title",
    "FirstName",
    "FatherName",
    "OwnerPhoto",
    "LicenseNumber"
FROM public."LicenseView"
WHERE "CompanyId" IS NOT NULL
ORDER BY "CompanyId" DESC;

-- 4. If you need to manually update a specific owner's photo path:
-- Replace YOUR_COMPANY_ID with the actual company ID
-- Replace 'YOUR_PHOTO_PATH' with the actual photo path from the upload response
/*
UPDATE org."CompanyOwner" 
SET "PothoPath" = 'Resources/Documents/Company/profile_20260121_072920_677.jpg'
WHERE "CompanyId" = YOUR_COMPANY_ID;
*/

-- 5. Verify the update worked
/*
SELECT 
    co."Id",
    co."FirstName",
    co."FatherName",
    co."CompanyId",
    co."PothoPath"
FROM org."CompanyOwner" co
WHERE co."CompanyId" = YOUR_COMPANY_ID;
*/

-- 6. Check the LicenseView again after update
/*
SELECT 
    "CompanyId",
    "Title",
    "FirstName",
    "FatherName",
    "OwnerPhoto",
    "LicenseNumber"
FROM public."LicenseView"
WHERE "CompanyId" = YOUR_COMPANY_ID;
*/
