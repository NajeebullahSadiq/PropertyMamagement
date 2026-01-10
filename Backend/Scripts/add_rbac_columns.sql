-- RBAC Migration Script
-- Adds new columns to AspNetUsers table for role-based access control

-- Add LicenseType column
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'AspNetUsers' AND column_name = 'LicenseType') THEN
        ALTER TABLE "AspNetUsers" ADD COLUMN "LicenseType" VARCHAR(50) NULL;
        RAISE NOTICE 'Added LicenseType column';
    END IF;
END $$;

-- Add UserRole column
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'AspNetUsers' AND column_name = 'UserRole') THEN
        ALTER TABLE "AspNetUsers" ADD COLUMN "UserRole" VARCHAR(50) NULL;
        RAISE NOTICE 'Added UserRole column';
    END IF;
END $$;

-- Add CreatedAt column
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'AspNetUsers' AND column_name = 'CreatedAt') THEN
        ALTER TABLE "AspNetUsers" ADD COLUMN "CreatedAt" TIMESTAMP NULL;
        RAISE NOTICE 'Added CreatedAt column';
    END IF;
END $$;

-- Add CreatedBy column
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'AspNetUsers' AND column_name = 'CreatedBy') THEN
        ALTER TABLE "AspNetUsers" ADD COLUMN "CreatedBy" VARCHAR(255) NULL;
        RAISE NOTICE 'Added CreatedBy column';
    END IF;
END $$;

-- Update existing admin users to have ADMIN role
UPDATE "AspNetUsers" 
SET "UserRole" = 'ADMIN', "CreatedAt" = NOW(), "CreatedBy" = 'system'
WHERE "IsAdmin" = true AND "UserRole" IS NULL;

-- Create new roles if they don't exist
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

-- Verify roles were created
SELECT "Name" FROM "AspNetRoles" ORDER BY "Name";
