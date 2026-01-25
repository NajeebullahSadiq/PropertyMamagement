-- Script to update Electronic National ID for Company 7
-- This is a TEMPLATE - Replace 'YOUR_ELECTRONIC_ID_HERE' with the actual ID number

-- Step 1: Check current data
SELECT 
    co."Id" as "OwnerId",
    co."FirstName",
    co."FatherName",
    co."ElectronicNationalIdNumber",
    co."CompanyId",
    co."Status"
FROM org."CompanyOwner" co
WHERE co."CompanyId" = 7;

-- Step 2: Update the electronic ID (UNCOMMENT AND MODIFY BEFORE RUNNING)
-- UPDATE org."CompanyOwner"
-- SET "ElectronicNationalIdNumber" = 'YOUR_ELECTRONIC_ID_HERE'
-- WHERE "CompanyId" = 7 AND "Status" = true;

-- Step 3: Verify the update
-- SELECT 
--     co."Id",
--     co."FirstName",
--     co."FatherName",
--     co."ElectronicNationalIdNumber"
-- FROM org."CompanyOwner" co
-- WHERE co."CompanyId" = 7;

-- Step 4: Check if it appears in the LicenseView
-- SELECT 
--     "CompanyId",
--     "FirstName",
--     "FatherName",
--     "IndentityCardNumber",
--     "LicenseNumber"
-- FROM public."LicenseView"
-- WHERE "CompanyId" = 7;

-- IMPORTANT NOTES:
-- 1. This is for TESTING purposes only
-- 2. The recommended approach is to enter data through the application UI
-- 3. Replace 'YOUR_ELECTRONIC_ID_HERE' with the actual electronic Tazkira number
-- 4. Uncomment the UPDATE statement before running
-- 5. The electronic ID format should match the official Tazkira format
