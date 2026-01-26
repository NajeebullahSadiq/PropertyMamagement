-- Check the current photo paths in CompanyOwner table
SELECT 
    "Id",
    "FirstName",
    "FatherName",
    "PothoPath",
    CASE 
        WHEN "PothoPath" IS NULL THEN 'NULL'
        WHEN "PothoPath" LIKE 'Resources/%' THEN 'Has Resources prefix'
        WHEN "PothoPath" LIKE 'profile_%' THEN 'Just filename - NEEDS FIX'
        ELSE 'Other format'
    END AS "PathStatus"
FROM org."CompanyOwner"
WHERE "PothoPath" IS NOT NULL
ORDER BY "Id"
LIMIT 20;

-- Count by path status
SELECT 
    CASE 
        WHEN "PothoPath" IS NULL THEN 'NULL'
        WHEN "PothoPath" LIKE 'Resources/%' THEN 'Has Resources prefix'
        WHEN "PothoPath" LIKE 'profile_%' THEN 'Just filename - NEEDS FIX'
        ELSE 'Other format'
    END AS "PathStatus",
    COUNT(*) as "Count"
FROM org."CompanyOwner"
GROUP BY "PathStatus";
