-- Quick script to add ProvinceId column to LicenseDetails table

-- Add ProvinceId column
ALTER TABLE org."LicenseDetails" 
ADD COLUMN IF NOT EXISTS "ProvinceId" INTEGER NULL;

-- Add foreign key constraint
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_schema = 'org' 
        AND table_name = 'LicenseDetails' 
        AND constraint_name = 'FK_LicenseDetails_Location_ProvinceId'
    ) THEN
        ALTER TABLE org."LicenseDetails"
        ADD CONSTRAINT "FK_LicenseDetails_Location_ProvinceId"
        FOREIGN KEY ("ProvinceId")
        REFERENCES look."Location"("ID")
        ON DELETE RESTRICT;
    END IF;
END $$;

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_LicenseDetails_ProvinceId" 
ON org."LicenseDetails"("ProvinceId");

CREATE INDEX IF NOT EXISTS "IX_LicenseDetails_LicenseNumber" 
ON org."LicenseDetails"("LicenseNumber");

CREATE INDEX IF NOT EXISTS "IX_LicenseDetails_ProvinceId_LicenseNumber" 
ON org."LicenseDetails"("ProvinceId", "LicenseNumber");

-- Show result
SELECT 'ProvinceId column added successfully!' as status;
