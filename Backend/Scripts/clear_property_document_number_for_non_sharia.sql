-- Clear PropertyDocumentNumber and PropertyDocumentDate for non-Sharia Deed guarantee types
-- This script ensures data consistency after the business rule change
-- PropertyDocumentNumber should only exist for Sharia Deed (GuaranteeTypeId = 2)

-- Guarantee Type IDs:
-- 1 = پول نقد (Cash)
-- 2 = قباله شرعی (Sharia Deed) - ONLY type that should have PropertyDocumentNumber
-- 3 = قباله عرفی (Customary Deed)

-- Preview the records that will be updated
SELECT 
    "Id",
    "FirstName",
    "FatherName",
    "GuaranteeTypeId",
    "PropertyDocumentNumber",
    "PropertyDocumentDate",
    CASE 
        WHEN "GuaranteeTypeId" = 1 THEN 'پول نقد (Cash)'
        WHEN "GuaranteeTypeId" = 2 THEN 'قباله شرعی (Sharia Deed)'
        WHEN "GuaranteeTypeId" = 3 THEN 'قباله عرفی (Customary Deed)'
        ELSE 'Unknown'
    END as "GuaranteeTypeName"
FROM org."Guarantors"
WHERE "GuaranteeTypeId" IN (1, 3) -- Cash or Customary Deed
  AND ("PropertyDocumentNumber" IS NOT NULL OR "PropertyDocumentDate" IS NOT NULL);

-- Uncomment the following lines to execute the cleanup
-- BEGIN;

-- Clear PropertyDocumentNumber and PropertyDocumentDate for Cash (GuaranteeTypeId = 1)
-- UPDATE org."Guarantors"
-- SET 
--     "PropertyDocumentNumber" = NULL,
--     "PropertyDocumentDate" = NULL
-- WHERE "GuaranteeTypeId" = 1
--   AND ("PropertyDocumentNumber" IS NOT NULL OR "PropertyDocumentDate" IS NOT NULL);

-- Clear PropertyDocumentNumber and PropertyDocumentDate for Customary Deed (GuaranteeTypeId = 3)
-- UPDATE org."Guarantors"
-- SET 
--     "PropertyDocumentNumber" = NULL,
--     "PropertyDocumentDate" = NULL
-- WHERE "GuaranteeTypeId" = 3
--   AND ("PropertyDocumentNumber" IS NOT NULL OR "PropertyDocumentDate" IS NOT NULL);

-- COMMIT;

-- Verify the cleanup
-- SELECT 
--     "GuaranteeTypeId",
--     COUNT(*) as "TotalRecords",
--     COUNT("PropertyDocumentNumber") as "RecordsWithDocNumber",
--     COUNT("PropertyDocumentDate") as "RecordsWithDocDate"
-- FROM org."Guarantors"
-- GROUP BY "GuaranteeTypeId"
-- ORDER BY "GuaranteeTypeId";
