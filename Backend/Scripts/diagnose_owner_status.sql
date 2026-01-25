-- Diagnose CompanyOwner Status issue for Company 7

-- Check the owner record and its Status value
SELECT 
    co."Id" as "OwnerId",
    co."FirstName",
    co."FatherName",
    co."ElectronicNationalIdNumber",
    co."CompanyId",
    co."Status",
    co."CreatedAt"
FROM org."CompanyOwner" co
WHERE co."CompanyId" = 7;

-- Check what the Status column type is
SELECT 
    column_name, 
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'CompanyOwner' 
  AND column_name = 'Status';

-- Test the view without Status filter
SELECT 
    cd."Id" AS "CompanyId",
    co."FirstName",
    co."FatherName",
    co."ElectronicNationalIdNumber",
    co."Status",
    ld."LicenseNumber"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
WHERE cd."Id" = 7;

-- Test with Status = true filter
SELECT 
    cd."Id" AS "CompanyId",
    co."FirstName",
    co."FatherName",
    co."ElectronicNationalIdNumber",
    co."Status",
    ld."LicenseNumber"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId" AND co."Status" = true
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
WHERE cd."Id" = 7;
