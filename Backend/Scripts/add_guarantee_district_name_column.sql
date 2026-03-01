-- Add GuaranteeDistrictName column to Guarantors table
-- This allows free-text entry for district names in Customary Deed (قباله عرفی) guarantees
-- The existing GuaranteeDistrictId column is kept for backward compatibility

-- Add the new column
ALTER TABLE org."Guarantors"
ADD COLUMN IF NOT EXISTS "GuaranteeDistrictName" VARCHAR(200);

-- Add comment to explain the column
COMMENT ON COLUMN org."Guarantors"."GuaranteeDistrictName" IS 'District name for Customary Deed guarantees - free text field (ناحیه)';

-- Verify the column was added
SELECT 
    column_name,
    data_type,
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org'
  AND table_name = 'Guarantors'
  AND column_name IN ('GuaranteeDistrictId', 'GuaranteeDistrictName')
ORDER BY ordinal_position;

-- Optional: Migrate existing GuaranteeDistrictId values to GuaranteeDistrictName
-- Uncomment the following lines if you want to copy district names from the Locations table

-- UPDATE org."Guarantors" g
-- SET "GuaranteeDistrictName" = l."Dari"
-- FROM org."Locations" l
-- WHERE g."GuaranteeDistrictId" = l."Id"
--   AND g."GuaranteeDistrictName" IS NULL
--   AND g."GuaranteeDistrictId" IS NOT NULL;

-- Verify the migration (if executed)
-- SELECT 
--     "Id",
--     "FirstName",
--     "GuaranteeTypeId",
--     "GuaranteeDistrictId",
--     "GuaranteeDistrictName"
-- FROM org."Guarantors"
-- WHERE "GuaranteeTypeId" = 3  -- Customary Deed
--   AND ("GuaranteeDistrictId" IS NOT NULL OR "GuaranteeDistrictName" IS NOT NULL);
