-- Check if CompanyOwner records have PothoPath set
SELECT 
    co."Id",
    co."FirstName",
    co."FatherName",
    co."CompanyId",
    co."PothoPath",
    cd."Title"
FROM org."CompanyOwner" co
LEFT JOIN org."CompanyDetails" cd ON co."CompanyId" = cd."Id"
ORDER BY co."Id" DESC
LIMIT 20;

-- Check the LicenseView to see if OwnerPhoto is being returned
SELECT 
    "CompanyId",
    "Title",
    "FirstName",
    "FatherName",
    "OwnerPhoto",
    "LicenseNumber"
FROM public."LicenseView"
ORDER BY "CompanyId" DESC
LIMIT 20;

-- If you need to manually update a specific owner's photo path, use this:
-- UPDATE org."CompanyOwner" 
-- SET "PothoPath" = 'Resources/Documents/Company/profile_20260121_072920_677.jpg'
-- WHERE "CompanyId" = YOUR_COMPANY_ID;
