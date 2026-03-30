-- Add Dari column to PropertyType table and update with translations

-- Add Dari column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'look' 
        AND table_name = 'PropertyType' 
        AND column_name = 'Dari'
    ) THEN
        ALTER TABLE look."PropertyType" ADD COLUMN "Dari" VARCHAR(100);
    END IF;
END $$;

-- Update existing records with Dari translations
UPDATE look."PropertyType" SET "Dari" = 'زمین' WHERE "Name" = 'Land';
UPDATE look."PropertyType" SET "Dari" = 'منزل' WHERE "Name" = 'Apartment';
UPDATE look."PropertyType" SET "Dari" = 'خانه' WHERE "Name" = 'House';
UPDATE look."PropertyType" SET "Dari" = 'مغازه' WHERE "Name" = 'Shop';
UPDATE look."PropertyType" SET "Dari" = 'دفتر' WHERE "Name" = 'Office';
UPDATE look."PropertyType" SET "Dari" = 'انبار' WHERE "Name" = 'Warehouse';
UPDATE look."PropertyType" SET "Dari" = 'بلاک' WHERE "Name" = 'Block';
UPDATE look."PropertyType" SET "Dari" = 'باغ' WHERE "Name" = 'Garden';
UPDATE look."PropertyType" SET "Dari" = 'تل' WHERE "Name" = 'Hill';
UPDATE look."PropertyType" SET "Dari" = 'غیره' WHERE "Name" = 'Other';

-- Verify the updates
SELECT "Id", "Name", "Dari" FROM look."PropertyType";
