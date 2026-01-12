-- Script to create the UserProfileWithCompany view
-- Run this on production if the migration doesn't work

-- Drop existing view if it exists
DROP VIEW IF EXISTS "UserProfileWithCompany";

-- Create the view
CREATE OR REPLACE VIEW "UserProfileWithCompany" AS
SELECT 
    u."Id" AS "UserId",
    u."Email",
    u."UserName",
    u."FirstName",
    u."LastName",
    u."PhotoPath",
    c."Title" AS "CompanyName",
    COALESCE(c."PhoneNumber", u."PhoneNumber") AS "PhoneNumber"
FROM "AspNetUsers" u
LEFT JOIN "org"."CompanyDetails" c ON u."CompanyId" = c."Id";

-- Verify the view was created
SELECT * FROM "UserProfileWithCompany" LIMIT 5;
