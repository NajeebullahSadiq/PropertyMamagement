-- SQL Script to apply pending migrations
-- Run this script in your PostgreSQL database (pgAdmin or psql)

-- ============================================
-- 1. Create CompanyAccountInfo table
-- ============================================
CREATE TABLE IF NOT EXISTS org."CompanyAccountInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL,
    "SettlementInfo" VARCHAR(500) NULL,
    "TaxPaymentAmount" NUMERIC(18,2) NOT NULL DEFAULT 0,
    "SettlementYear" INTEGER NULL,
    "TaxPaymentDate" DATE NULL,
    "TransactionCount" INTEGER NULL,
    "CompanyCommission" NUMERIC(18,2) NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50) NULL,
    "Status" BOOLEAN DEFAULT TRUE,
    CONSTRAINT "FK_CompanyAccountInfo_CompanyDetails" 
        FOREIGN KEY ("CompanyId") REFERENCES org."CompanyDetails"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_CompanyAccountInfo_CompanyId" 
    ON org."CompanyAccountInfo" ("CompanyId");

-- ============================================
-- 2. Create CompanyCancellationInfo table
-- ============================================
CREATE TABLE IF NOT EXISTS org."CompanyCancellationInfo" (
    "Id" SERIAL PRIMARY KEY,
    "CompanyId" INTEGER NOT NULL,
    "LicenseCancellationLetterNumber" VARCHAR(100) NULL,
    "RevenueCancellationLetterNumber" VARCHAR(100) NULL,
    "LicenseCancellationLetterDate" DATE NULL,
    "Remarks" VARCHAR(1000) NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(50) NULL,
    "Status" BOOLEAN DEFAULT TRUE,
    CONSTRAINT "FK_CompanyCancellationInfo_CompanyDetails" 
        FOREIGN KEY ("CompanyId") REFERENCES org."CompanyDetails"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_CompanyCancellationInfo_CompanyId" 
    ON org."CompanyCancellationInfo" ("CompanyId");

-- ============================================
-- 3. Add Guarantee Type Conditional Fields to Guarantors table
-- ============================================

-- Sharia Deed fields
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "CourtName" VARCHAR(255) NULL;
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "CollateralNumber" VARCHAR(100) NULL;

-- Customary Deed fields
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "SetSerialNumber" VARCHAR(100) NULL;
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "GuaranteeDistrictId" INTEGER NULL;

-- Cash fields
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "BankName" VARCHAR(255) NULL;
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "DepositNumber" VARCHAR(100) NULL;
ALTER TABLE org."Guarantors" ADD COLUMN IF NOT EXISTS "DepositDate" DATE NULL;

-- Add index for GuaranteeDistrictId (if not exists)
CREATE INDEX IF NOT EXISTS "IX_Guarantors_GuaranteeDistrictId" 
    ON org."Guarantors" ("GuaranteeDistrictId");

-- Add foreign key for GuaranteeDistrict (check if exists first)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'Guarantors_GuaranteeDistrictId_fkey'
    ) THEN
        ALTER TABLE org."Guarantors" 
        ADD CONSTRAINT "Guarantors_GuaranteeDistrictId_fkey" 
        FOREIGN KEY ("GuaranteeDistrictId") 
        REFERENCES look."Location"("Id") ON DELETE SET NULL;
    END IF;
END $$;

-- ============================================
-- 4. Update GuaranteeType lookup table (optional - only 3 types)
-- ============================================
-- Uncomment below if you want to update the guarantee types to only 3 options
-- WARNING: This will delete existing guarantee types - backup first!

-- DELETE FROM look."GuaranteeType";
-- INSERT INTO look."GuaranteeType" ("Id", "Name") VALUES 
--     (1, 'Cash'),
--     (2, 'ShariaDeed'),
--     (3, 'CustomaryDeed');

-- ============================================
-- Done! Verify the changes
-- ============================================
SELECT 'CompanyAccountInfo table created' AS status WHERE EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'org' AND table_name = 'CompanyAccountInfo');
SELECT 'CompanyCancellationInfo table created' AS status WHERE EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'org' AND table_name = 'CompanyCancellationInfo');
SELECT 'Guarantors columns added' AS status WHERE EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'org' AND table_name = 'Guarantors' AND column_name = 'BankName');
