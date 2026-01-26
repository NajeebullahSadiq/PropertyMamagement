-- Simple script to drop old identity columns
-- Run this to clean up the duplicate/old columns

-- Drop old columns from CompanyOwner
ALTER TABLE org."CompanyOwner" DROP COLUMN IF EXISTS "IdentityCardTypeId";
ALTER TABLE org."CompanyOwner" DROP COLUMN IF EXISTS "Jild";
ALTER TABLE org."CompanyOwner" DROP COLUMN IF EXISTS "Safha";

-- Drop old columns from Guarantor  
ALTER TABLE org."Guarantor" DROP COLUMN IF EXISTS "IdentityCardTypeId";
ALTER TABLE org."Guarantor" DROP COLUMN IF EXISTS "Jild";
ALTER TABLE org."Guarantor" DROP COLUMN IF EXISTS "Safha";

-- Show final state
SELECT 
    table_name,
    column_name,
    data_type
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name IN ('CompanyOwner', 'Guarantor')
AND (column_name LIKE '%Identity%' OR column_name LIKE '%Jild%' OR column_name LIKE '%Safha%' OR column_name = 'ElectronicNationalIdNumber')
ORDER BY table_name, column_name;
