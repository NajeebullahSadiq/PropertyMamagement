-- Reset Database Script for Testing
-- WARNING: This will DROP ALL DATA!
-- Run this to reset the database to a clean state matching the new migrations

-- Drop all views first
DROP VIEW IF EXISTS public."LicenseView" CASCADE;
DROP VIEW IF EXISTS public."GetPrintType" CASCADE;
DROP VIEW IF EXISTS public."UserProfileWithCompany" CASCADE;

-- Drop schemas (cascades to all tables)
DROP SCHEMA IF EXISTS log CASCADE;
DROP SCHEMA IF EXISTS org CASCADE;
DROP SCHEMA IF EXISTS tr CASCADE;
DROP SCHEMA IF EXISTS look CASCADE;

-- Drop Identity tables
DROP TABLE IF EXISTS public."AspNetUserTokens" CASCADE;
DROP TABLE IF EXISTS public."AspNetUserRoles" CASCADE;
DROP TABLE IF EXISTS public."AspNetUserLogins" CASCADE;
DROP TABLE IF EXISTS public."AspNetUserClaims" CASCADE;
DROP TABLE IF EXISTS public."AspNetRoleClaims" CASCADE;
DROP TABLE IF EXISTS public."AspNetUsers" CASCADE;
DROP TABLE IF EXISTS public."AspNetRoles" CASCADE;

-- Drop EF Core migration history
DROP TABLE IF EXISTS public."__EFMigrationsHistory" CASCADE;

-- Recreate schemas
CREATE SCHEMA IF NOT EXISTS look;
CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS tr;
CREATE SCHEMA IF NOT EXISTS log;

-- Done! Now run the application to apply migrations
SELECT 'Database reset complete. Run the application to apply migrations.' AS status;
