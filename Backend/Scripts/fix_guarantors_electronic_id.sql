-- Fix ElectronicNationalIdNumber in Guarantors table (note the 's')
-- Convert from double precision to VARCHAR(50)

DO $$
BEGIN
    -- Check if ElectronicNationalIdNumber exists and is double precision
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'Guarantors' 
        AND column_name = 'ElectronicNationalIdNumber'
        AND data_type = 'double precision'
    ) THEN
        ALTER TABLE org."Guarantors" 
        ALTER COLUMN "ElectronicNationalIdNumber" TYPE VARCHAR(50) 
        USING CASE 
            WHEN "ElectronicNationalIdNumber" IS NULL THEN NULL
            ELSE "ElectronicNationalIdNumber"::VARCHAR(50)
        END;
        
        RAISE NOTICE 'Converted ElectronicNationalIdNumber to VARCHAR(50) in Guarantors';
    ELSE
        RAISE NOTICE 'ElectronicNationalIdNumber in Guarantors is already correct type or does not exist';
    END IF;

END $$;

-- Verify the change
SELECT 
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name = 'Guarantors'
AND column_name = 'ElectronicNationalIdNumber';
