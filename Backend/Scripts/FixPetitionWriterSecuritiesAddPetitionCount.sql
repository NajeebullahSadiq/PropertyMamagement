-- Fix PetitionWriterSecurities table - Add missing PetitionCount column
-- Run this on production database

-- Check if column exists, if not add it
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'PetitionWriterSecurities' 
        AND column_name = 'PetitionCount'
    ) THEN
        ALTER TABLE org."PetitionWriterSecurities" 
        ADD COLUMN "PetitionCount" INTEGER NOT NULL DEFAULT 0;
        
        RAISE NOTICE 'Column PetitionCount added successfully';
    ELSE
        RAISE NOTICE 'Column PetitionCount already exists';
    END IF;
END $$;

-- Update existing records to calculate PetitionCount from Amount (Amount / 5)
-- Only if there are records with PetitionCount = 0
UPDATE org."PetitionWriterSecurities"
SET "PetitionCount" = CASE 
    WHEN "Amount" > 0 THEN CAST("Amount" / 5 AS INTEGER)
    ELSE 0
END
WHERE "PetitionCount" = 0;

-- Verify the fix
SELECT 
    COUNT(*) as total_records,
    COUNT(CASE WHEN "PetitionCount" > 0 THEN 1 END) as records_with_count,
    COUNT(CASE WHEN "PetitionCount" = 0 THEN 1 END) as records_without_count
FROM org."PetitionWriterSecurities";
