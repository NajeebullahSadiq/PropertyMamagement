-- Migration script to restrict Vehicle Seller Role Types
-- Approved values: 'Seller', 'Sellers', 'Sales Agent', 'Heirs'
-- Run this script to migrate existing data to approved role types

-- Map 'Lessor' -> 'Seller' (single lessor becomes single seller)
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Seller' 
WHERE "RoleType" = 'Lessor';

-- Map 'Lessors' -> 'Sellers' (multiple lessors become multiple sellers)
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Sellers' 
WHERE "RoleType" = 'Lessors';

-- Map 'Seller in a revocable sale' -> 'Seller'
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Seller' 
WHERE "RoleType" = 'Seller in a revocable sale';

-- Map 'Sellers in a revocable sale' -> 'Sellers'
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Sellers' 
WHERE "RoleType" = 'Sellers in a revocable sale';

-- Map 'Lease Agent' -> 'Sales Agent'
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Sales Agent' 
WHERE "RoleType" = 'Lease Agent';

-- Map 'Agent for a revocable sale' -> 'Sales Agent'
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Sales Agent' 
WHERE "RoleType" = 'Agent for a revocable sale';

-- Set NULL or empty values to 'Seller' (default)
UPDATE tr."VehiclesSellerDetails" 
SET "RoleType" = 'Seller' 
WHERE "RoleType" IS NULL OR "RoleType" = '';

-- Verify the migration
SELECT "RoleType", COUNT(*) as count 
FROM tr."VehiclesSellerDetails" 
GROUP BY "RoleType" 
ORDER BY "RoleType";
