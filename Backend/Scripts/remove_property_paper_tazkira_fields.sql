-- Remove paper-based Tazkira fields from Property module
-- Keep only ElectronicNationalIdNumber for all tables
-- Date: 2026-01-27

BEGIN;

-- Drop paper Tazkira columns from SellerDetails if they exist
DO $$ 
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'SellerDetails' AND column_name = 'TazkiraType') THEN
        ALTER TABLE tr."SellerDetails" DROP COLUMN "TazkiraType";
        RAISE NOTICE 'Dropped TazkiraType from SellerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'SellerDetails' AND column_name = 'TazkiraVolume') THEN
        ALTER TABLE tr."SellerDetails" DROP COLUMN "TazkiraVolume";
        RAISE NOTICE 'Dropped TazkiraVolume from SellerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'SellerDetails' AND column_name = 'TazkiraPage') THEN
        ALTER TABLE tr."SellerDetails" DROP COLUMN "TazkiraPage";
        RAISE NOTICE 'Dropped TazkiraPage from SellerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'SellerDetails' AND column_name = 'TazkiraNumber') THEN
        ALTER TABLE tr."SellerDetails" DROP COLUMN "TazkiraNumber";
        RAISE NOTICE 'Dropped TazkiraNumber from SellerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'SellerDetails' AND column_name = 'TazkiraRegNumber') THEN
        ALTER TABLE tr."SellerDetails" DROP COLUMN "TazkiraRegNumber";
        RAISE NOTICE 'Dropped TazkiraRegNumber from SellerDetails';
    END IF;
END $$;

-- Drop paper Tazkira columns from BuyerDetails if they exist
DO $$ 
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'BuyerDetails' AND column_name = 'TazkiraType') THEN
        ALTER TABLE tr."BuyerDetails" DROP COLUMN "TazkiraType";
        RAISE NOTICE 'Dropped TazkiraType from BuyerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'BuyerDetails' AND column_name = 'TazkiraVolume') THEN
        ALTER TABLE tr."BuyerDetails" DROP COLUMN "TazkiraVolume";
        RAISE NOTICE 'Dropped TazkiraVolume from BuyerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'BuyerDetails' AND column_name = 'TazkiraPage') THEN
        ALTER TABLE tr."BuyerDetails" DROP COLUMN "TazkiraPage";
        RAISE NOTICE 'Dropped TazkiraPage from BuyerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'BuyerDetails' AND column_name = 'TazkiraNumber') THEN
        ALTER TABLE tr."BuyerDetails" DROP COLUMN "TazkiraNumber";
        RAISE NOTICE 'Dropped TazkiraNumber from BuyerDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'BuyerDetails' AND column_name = 'TazkiraRegNumber') THEN
        ALTER TABLE tr."BuyerDetails" DROP COLUMN "TazkiraRegNumber";
        RAISE NOTICE 'Dropped TazkiraRegNumber from BuyerDetails';
    END IF;
END $$;

-- Drop paper Tazkira columns from WitnessDetails if they exist
DO $$ 
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'WitnessDetails' AND column_name = 'TazkiraType') THEN
        ALTER TABLE tr."WitnessDetails" DROP COLUMN "TazkiraType";
        RAISE NOTICE 'Dropped TazkiraType from WitnessDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'WitnessDetails' AND column_name = 'TazkiraVolume') THEN
        ALTER TABLE tr."WitnessDetails" DROP COLUMN "TazkiraVolume";
        RAISE NOTICE 'Dropped TazkiraVolume from WitnessDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'WitnessDetails' AND column_name = 'TazkiraPage') THEN
        ALTER TABLE tr."WitnessDetails" DROP COLUMN "TazkiraPage";
        RAISE NOTICE 'Dropped TazkiraPage from WitnessDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'WitnessDetails' AND column_name = 'TazkiraNumber') THEN
        ALTER TABLE tr."WitnessDetails" DROP COLUMN "TazkiraNumber";
        RAISE NOTICE 'Dropped TazkiraNumber from WitnessDetails';
    END IF;
    
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'WitnessDetails' AND column_name = 'TazkiraRegNumber') THEN
        ALTER TABLE tr."WitnessDetails" DROP COLUMN "TazkiraRegNumber";
        RAISE NOTICE 'Dropped TazkiraRegNumber from WitnessDetails';
    END IF;
END $$;

COMMIT;

-- Verify the changes
SELECT 'Verification: Remaining Tazkira-related columns in Property module' as message;

SELECT 
    table_name,
    column_name, 
    data_type
FROM information_schema.columns 
WHERE table_schema = 'tr' 
    AND table_name IN ('SellerDetails', 'BuyerDetails', 'WitnessDetails')
    AND (column_name ILIKE '%tazkira%' OR column_name = 'ElectronicNationalIdNumber')
ORDER BY table_name, column_name;
