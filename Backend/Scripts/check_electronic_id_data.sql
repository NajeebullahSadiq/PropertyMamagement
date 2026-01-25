-- Check if ElectronicNationalIdNumber exists in CompanyOwner table for company ID 7
SELECT 
    co."Id" as "OwnerId",
    co."FirstName",
    co."FatherName",
    co."ElectronicNationalIdNumber",
    co."CompanyId",
    co."Status"
FROM org."CompanyOwner" co
WHERE co."CompanyId" = 7;

-- Check the LicenseView for company ID 7
SELECT 
    "CompanyId",
    "FirstName",
    "FatherName", 
    "IndentityCardNumber",
    "LicenseNumber"
FROM public."LicenseView"
WHERE "CompanyId" = 7;

-- Check if the column exists and its data type
SELECT 
    column_name, 
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'CompanyOwner' 
  AND column_name = 'ElectronicNationalIdNumber';
