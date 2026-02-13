-- Verify Company Migration Results

-- Count records in each table
SELECT 'CompanyDetails' as table_name, COUNT(*) as record_count FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwner', COUNT(*) FROM org."CompanyOwner"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails"
UNION ALL
SELECT 'Guarantors', COUNT(*) FROM org."Guarantors";

-- Sample company data
SELECT 
    c."Id",
    c."Title",
    l."LicenseNumber",
    l."IssueDate",
    l."ExpireDate",
    o."FirstName",
    o."FatherName"
FROM org."CompanyDetails" c
LEFT JOIN org."LicenseDetails" l ON c."Id" = l."CompanyId"
LEFT JOIN org."CompanyOwner" o ON c."Id" = o."CompanyId"
LIMIT 10;

-- Check guarantors by license number
SELECT 
    g.*,
    c."Title" as company_name,
    l."LicenseNumber"
FROM org."Guarantors" g
INNER JOIN org."CompanyDetails" c ON g."CompanyId" = c."Id"
INNER JOIN org."LicenseDetails" l ON c."Id" = l."CompanyId"
LIMIT 10;
