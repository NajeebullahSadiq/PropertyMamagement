-- Fix PetitionWriterLicenses table schema
-- This script adds the missing PicturePath column

-- Add PicturePath column if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'PetitionWriterLicenses' 
        AND column_name = 'PicturePath'
    ) THEN
        ALTER TABLE org."PetitionWriterLicenses"
        ADD COLUMN "PicturePath" character varying(500);
        
        RAISE NOTICE 'Added PicturePath column to org.PetitionWriterLicenses';
    ELSE
        RAISE NOTICE 'PicturePath column already exists in org.PetitionWriterLicenses';
    END IF;
END $$;

-- Verify the column was added
SELECT column_name, data_type, character_maximum_length, is_nullable
FROM information_schema.columns
WHERE table_schema = 'org' 
AND table_name = 'PetitionWriterLicenses'
AND column_name = 'PicturePath';
