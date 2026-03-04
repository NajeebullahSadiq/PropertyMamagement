-- Find duplicate license numbers in LicenseDetails table
SELECT "LicenseNumber", COUNT(*) as count, 
       STRING_AGG("Id"::text, ', ') as ids,
       STRING_AGG("CompanyId"::text, ', ') as company_ids
FROM org."LicenseDetails"
WHERE "LicenseNumber" IS NOT NULL
GROUP BY "LicenseNumber"
HAVING COUNT(*) > 1
ORDER BY count DESC, "LicenseNumber";

-- Find the specific duplicate KBL-00007477
SELECT ld."Id", ld."LicenseNumber", ld."CompanyId", ld."CreatedAt", ld."CreatedBy",
       cd."Title" as CompanyTitle
FROM org."LicenseDetails" ld
LEFT JOIN org."CompanyDetails" cd ON ld."CompanyId" = cd."Id"
WHERE ld."LicenseNumber" = 'KBL-00007477'
ORDER BY ld."CreatedAt";

-- Check for similar patterns (5-digit vs 8-digit)
SELECT "LicenseNumber", COUNT(*) as count
FROM org."LicenseDetails"
WHERE "LicenseNumber" LIKE 'KBL-%'
GROUP BY "LicenseNumber"
ORDER BY "LicenseNumber";
