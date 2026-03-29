-- Add Dari column to PUnitType table and update with translations

-- Add Dari column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'look' 
        AND table_name = 'PUnitType' 
        AND column_name = 'Dari'
    ) THEN
        ALTER TABLE look."PUnitType" ADD COLUMN "Dari" VARCHAR(100);
    END IF;
END $$;

-- Update existing records with Dari translations
UPDATE look."PUnitType" SET "Dari" = 'متر مربع' WHERE "Name" = 'Square Meter (m²)';
UPDATE look."PUnitType" SET "Dari" = 'فوت مربع' WHERE "Name" = 'Square Foot (ft²)';
UPDATE look."PUnitType" SET "Dari" = 'جریب' WHERE "Name" = 'Jerib';
UPDATE look."PUnitType" SET "Dari" = 'ایکړ' WHERE "Name" = 'Acre';
UPDATE look."PUnitType" SET "Dari" = 'هکتار' WHERE "Name" = 'Hectare';
UPDATE look."PUnitType" SET "Dari" = 'بسوا' WHERE "Name" = 'Biswa';
UPDATE look."PUnitType" SET "Dari" = 'کنال' WHERE "Name" = 'Kanal';
UPDATE look."PUnitType" SET "Dari" = 'مرله' WHERE "Name" = 'Marla';

-- Verify the updates
SELECT "Id", "Name", "Dari" FROM look."PUnitType";
