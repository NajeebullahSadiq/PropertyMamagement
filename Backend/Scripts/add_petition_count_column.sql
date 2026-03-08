-- Add PetitionCount column to PetitionWriterSecurities table if it doesn't exist
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
        ADD COLUMN "PetitionCount" integer NOT NULL DEFAULT 1;
        
        RAISE NOTICE 'Column PetitionCount added successfully';
    ELSE
        RAISE NOTICE 'Column PetitionCount already exists';
    END IF;
END $$;
