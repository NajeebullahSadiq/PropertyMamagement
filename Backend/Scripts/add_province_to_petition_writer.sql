-- Add ProvinceId column to PetitionWriterLicenses table
-- This fixes the error: column p.ProvinceId does not exist
-- Note: Foreign key is skipped due to Location table structure

DO $$
BEGIN
    -- Check if ProvinceId column exists
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'PetitionWriterLicenses' 
        AND column_name = 'ProvinceId'
    ) THEN
        -- Add ProvinceId column
        ALTER TABLE org."PetitionWriterLicenses"
        ADD COLUMN "ProvinceId" INTEGER NULL;
        
        RAISE NOTICE 'ProvinceId column added successfully';

        -- Add index for better query performance
        CREATE INDEX "IX_PetitionWriterLicenses_ProvinceId"
        ON org."PetitionWriterLicenses"("ProvinceId");
        
        RAISE NOTICE 'Index on ProvinceId created';

        -- Add index on LicenseNumber for faster lookups
        IF NOT EXISTS (
            SELECT 1 FROM pg_indexes 
            WHERE schemaname = 'org' 
            AND tablename = 'PetitionWriterLicenses' 
            AND indexname = 'IX_PetitionWriterLicenses_LicenseNumber'
        ) THEN
            CREATE INDEX "IX_PetitionWriterLicenses_LicenseNumber"
            ON org."PetitionWriterLicenses"("LicenseNumber");
            
            RAISE NOTICE 'Index on LicenseNumber created';
        END IF;

        RAISE NOTICE 'ProvinceId column setup completed successfully (without foreign key)';
        RAISE NOTICE 'The API will now work. Foreign key can be added later if needed.';
        
    ELSE
        RAISE NOTICE 'ProvinceId column already exists in PetitionWriterLicenses table';
        
        -- Add indexes if they don't exist
        IF NOT EXISTS (
            SELECT 1 FROM pg_indexes 
            WHERE schemaname = 'org' 
            AND tablename = 'PetitionWriterLicenses' 
            AND indexname = 'IX_PetitionWriterLicenses_ProvinceId'
        ) THEN
            CREATE INDEX "IX_PetitionWriterLicenses_ProvinceId"
            ON org."PetitionWriterLicenses"("ProvinceId");
            RAISE NOTICE 'Index on ProvinceId created';
        END IF;
        
        IF NOT EXISTS (
            SELECT 1 FROM pg_indexes 
            WHERE schemaname = 'org' 
            AND tablename = 'PetitionWriterLicenses' 
            AND indexname = 'IX_PetitionWriterLicenses_LicenseNumber'
        ) THEN
            CREATE INDEX "IX_PetitionWriterLicenses_LicenseNumber"
            ON org."PetitionWriterLicenses"("LicenseNumber");
            RAISE NOTICE 'Index on LicenseNumber created';
        END IF;
    END IF;
END $$;

-- Verify the column was added
SELECT 
    column_name, 
    data_type, 
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'org'
AND table_name = 'PetitionWriterLicenses'
AND column_name = 'ProvinceId';

-- Show success message
SELECT 'ProvinceId column is now available. The API error should be fixed!' as status;
