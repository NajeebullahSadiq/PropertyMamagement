-- =====================================================
-- Script: Restrict Vehicle Buyer Role Types
-- Date: 2026-01-13
-- Description: Restricts Vehicle Buyer Role Types to 3 approved options:
--   - Buyer (خریدار) - Single buyer
--   - Buyers (خریداران) - Multiple buyers allowed
--   - Purchase Agent (وکیل خرید) - Single buyer with authorization letter
-- =====================================================

-- Create backup table for audit trail before migration
CREATE TABLE IF NOT EXISTS "VehiclesBuyerRoleTypeMigrationLog" (
    "Id" SERIAL PRIMARY KEY,
    "BuyerId" INTEGER NOT NULL,
    "OldRoleType" VARCHAR(100),
    "NewRoleType" VARCHAR(100),
    "MigratedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Log and update 'Buyer in a revocable sale' -> 'Buyer'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Buyer'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" = 'Buyer in a revocable sale';

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Buyer'
WHERE "RoleType" = 'Buyer in a revocable sale';

-- Log and update 'Buyers in a revocable sale' -> 'Buyers'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Buyers'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" = 'Buyers in a revocable sale';

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Buyers'
WHERE "RoleType" = 'Buyers in a revocable sale';

-- Log and update 'Lessee' -> 'Buyer'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Buyer'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" = 'Lessee';

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Buyer'
WHERE "RoleType" = 'Lessee';

-- Log and update 'Lessees' -> 'Buyers'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Buyers'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" = 'Lessees';

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Buyers'
WHERE "RoleType" = 'Lessees';

-- Log and update 'Agent for buyer in a revocable sale' -> 'Purchase Agent'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Purchase Agent'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" = 'Agent for buyer in a revocable sale';

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Purchase Agent'
WHERE "RoleType" = 'Agent for buyer in a revocable sale';

-- Log and update 'Agent for lessee' -> 'Purchase Agent'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Purchase Agent'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" = 'Agent for lessee';

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Purchase Agent'
WHERE "RoleType" = 'Agent for lessee';

-- Log and update any other invalid values -> 'Buyer'
INSERT INTO "VehiclesBuyerRoleTypeMigrationLog" ("BuyerId", "OldRoleType", "NewRoleType")
SELECT "Id", "RoleType", 'Buyer'
FROM "VehiclesBuyerDetails"
WHERE "RoleType" NOT IN ('Buyer', 'Buyers', 'Purchase Agent')
  AND "RoleType" IS NOT NULL;

UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Buyer'
WHERE "RoleType" NOT IN ('Buyer', 'Buyers', 'Purchase Agent')
  AND "RoleType" IS NOT NULL;

-- Set default for NULL values
UPDATE "VehiclesBuyerDetails"
SET "RoleType" = 'Buyer'
WHERE "RoleType" IS NULL;

-- Add check constraint to enforce only allowed values
ALTER TABLE "VehiclesBuyerDetails"
DROP CONSTRAINT IF EXISTS "CK_VehiclesBuyerDetails_RoleType";

ALTER TABLE "VehiclesBuyerDetails"
ADD CONSTRAINT "CK_VehiclesBuyerDetails_RoleType"
CHECK ("RoleType" IN ('Buyer', 'Buyers', 'Purchase Agent'));

-- Verify migration results
SELECT 'Migration Summary' AS "Report";
SELECT "NewRoleType", COUNT(*) AS "RecordsMigrated"
FROM "VehiclesBuyerRoleTypeMigrationLog"
GROUP BY "NewRoleType";

SELECT 'Current Role Type Distribution' AS "Report";
SELECT "RoleType", COUNT(*) AS "Count"
FROM "VehiclesBuyerDetails"
GROUP BY "RoleType";
