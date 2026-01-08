-- Migration: Add Owner's Own Address Fields to CompanyOwner table
-- This adds the owner's own address (آدرس اصلی مالک) separate from current residence

-- Add Owner's Own Address columns
ALTER TABLE org."CompanyOwner" ADD COLUMN IF NOT EXISTS "OwnerProvinceId" integer;
ALTER TABLE org."CompanyOwner" ADD COLUMN IF NOT EXISTS "OwnerDistrictId" integer;
ALTER TABLE org."CompanyOwner" ADD COLUMN IF NOT EXISTS "OwnerVillage" text;

-- Add foreign key constraints (if they don't exist)
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'CompanyOwner_OwnerProvinceId_fkey') THEN
        ALTER TABLE org."CompanyOwner" 
        ADD CONSTRAINT "CompanyOwner_OwnerProvinceId_fkey" 
        FOREIGN KEY ("OwnerProvinceId") REFERENCES org."Location"("Id");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'CompanyOwner_OwnerDistrictId_fkey') THEN
        ALTER TABLE org."CompanyOwner" 
        ADD CONSTRAINT "CompanyOwner_OwnerDistrictId_fkey" 
        FOREIGN KEY ("OwnerDistrictId") REFERENCES org."Location"("Id");
    END IF;
END $$;

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS "IX_CompanyOwner_OwnerProvinceId" ON org."CompanyOwner" ("OwnerProvinceId");
CREATE INDEX IF NOT EXISTS "IX_CompanyOwner_OwnerDistrictId" ON org."CompanyOwner" ("OwnerDistrictId");

-- Verify the changes
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_schema = 'org' AND table_name = 'CompanyOwner' 
AND column_name IN ('OwnerProvinceId', 'OwnerDistrictId', 'OwnerVillage');
