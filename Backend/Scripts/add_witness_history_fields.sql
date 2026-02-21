-- Add witness history tracking fields to Guarantors table
-- This enables tracking of active/inactive witnesses with full history

-- Add IsActive field (default true for existing records)
ALTER TABLE org."Guarantors" 
ADD COLUMN IF NOT EXISTS "IsActive" boolean DEFAULT true;

-- Add ExpiredAt field (when witness was replaced)
ALTER TABLE org."Guarantors" 
ADD COLUMN IF NOT EXISTS "ExpiredAt" timestamp with time zone;

-- Add ExpiredBy field (user who replaced the witness)
ALTER TABLE org."Guarantors" 
ADD COLUMN IF NOT EXISTS "ExpiredBy" text;

-- Add ReplacedByGuarantorId field (reference to new witness)
ALTER TABLE org."Guarantors"
ADD COLUMN IF NOT EXISTS "ReplacedByGuarantorId" integer;

-- Add foreign key constraint for ReplacedByGuarantorId
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'FK_Guarantor_ReplacedBy'
    ) THEN
        ALTER TABLE org."Guarantors"
        ADD CONSTRAINT "FK_Guarantor_ReplacedBy"
        FOREIGN KEY ("ReplacedByGuarantorId")
        REFERENCES org."Guarantors"("Id")
        ON DELETE SET NULL;
    END IF;
END $$;

-- Create index for better query performance
CREATE INDEX IF NOT EXISTS "IX_Guarantor_CompanyId_IsActive"
ON org."Guarantors"("CompanyId", "IsActive");

-- Set all existing witnesses as active
UPDATE org."Guarantors"
SET "IsActive" = true
WHERE "IsActive" IS NULL;

-- For companies with multiple witnesses, keep only the most recent as active
WITH RankedGuarantors AS (
    SELECT
        "Id",
        "CompanyId",
        ROW_NUMBER() OVER (PARTITION BY "CompanyId" ORDER BY "CreatedAt" DESC) as rn
    FROM org."Guarantors"
    WHERE "CompanyId" IS NOT NULL
)
UPDATE org."Guarantors" g
SET "IsActive" = false,
    "ExpiredAt" = NOW()
FROM RankedGuarantors rg
WHERE g."Id" = rg."Id"
  AND rg.rn > 1;

-- Verify the changes
SELECT
    "CompanyId",
    COUNT(*) as total_witnesses,
    SUM(CASE WHEN "IsActive" = true THEN 1 ELSE 0 END) as active_witnesses,
    SUM(CASE WHEN "IsActive" = false THEN 1 ELSE 0 END) as inactive_witnesses
FROM org."Guarantors"
WHERE "CompanyId" IS NOT NULL
GROUP BY "CompanyId"
ORDER BY "CompanyId";
