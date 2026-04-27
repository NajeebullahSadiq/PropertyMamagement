-- Add GuaranteeLocation (محل ضمانت) column to LicenseApplicationGuarantors table
-- This field is shown for قباله شرعی and قباله عرفی guarantee types only
-- Hidden when پول نقد is selected

ALTER TABLE org."LicenseApplicationGuarantors"
ADD COLUMN IF NOT EXISTS "GuaranteeLocation" VARCHAR(500) NULL;

COMMENT ON COLUMN org."LicenseApplicationGuarantors"."GuaranteeLocation" IS 'محل ضمانت - Location of Guarantee. Only applicable for Sharia Deed (قباله شرعی) and Customary Deed (قباله عرفی). NULL for Cash (پول نقد).';
