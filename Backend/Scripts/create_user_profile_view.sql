-- Script to create the UserProfileWithCompany view and seed RBAC roles
-- Run this on production if the migration doesn't work

-- =====================================================
-- 1. Create UserProfileWithCompany View
-- =====================================================
DROP VIEW IF EXISTS "UserProfileWithCompany";

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

-- =====================================================
-- 2. Seed RBAC Roles in AspNetRoles
-- =====================================================
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'ADMIN', 'ADMIN', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'ADMIN');

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'AUTHORITY', 'AUTHORITY', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'AUTHORITY');

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'COMPANY_REGISTRAR', 'COMPANY_REGISTRAR', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'COMPANY_REGISTRAR');

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'LICENSE_REVIEWER', 'LICENSE_REVIEWER', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'LICENSE_REVIEWER');

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'PROPERTY_OPERATOR', 'PROPERTY_OPERATOR', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'PROPERTY_OPERATOR');

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT gen_random_uuid()::text, 'VEHICLE_OPERATOR', 'VEHICLE_OPERATOR', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'VEHICLE_OPERATOR');

-- =====================================================
-- 3. Assign ADMIN role to existing admin users
-- =====================================================
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "AspNetUsers" u
CROSS JOIN "AspNetRoles" r
WHERE u."IsAdmin" = true 
AND r."Name" = 'ADMIN'
AND NOT EXISTS (
    SELECT 1 FROM "AspNetUserRoles" ur 
    WHERE ur."UserId" = u."Id" AND ur."RoleId" = r."Id"
);

-- =====================================================
-- 4. Update UserRole column for admin users
-- =====================================================
UPDATE "AspNetUsers" 
SET "UserRole" = 'ADMIN'
WHERE "IsAdmin" = true AND ("UserRole" IS NULL OR "UserRole" = '');

-- =====================================================
-- 5. Verify the changes
-- =====================================================
SELECT 'UserProfileWithCompany view created' AS status;
SELECT * FROM "UserProfileWithCompany" LIMIT 3;

SELECT 'Roles in AspNetRoles:' AS status;
SELECT "Name" FROM "AspNetRoles" ORDER BY "Name";

SELECT 'Admin users with ADMIN role:' AS status;
SELECT u."UserName", u."Email", u."IsAdmin", u."UserRole", r."Name" as "AssignedRole"
FROM "AspNetUsers" u
LEFT JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
LEFT JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE u."IsAdmin" = true;
