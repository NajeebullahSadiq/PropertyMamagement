-- Add IsComplete column to LicenseDetails table
-- This column tracks whether all required fields for a company license are filled

DO $$
BEGIN
    -- Add IsComplete column if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'org' 
        AND table_name = 'LicenseDetails' 
        AND column_name = 'IsComplete'
    ) THEN
        ALTER TABLE org."LicenseDetails" 
        ADD COLUMN "IsComplete" BOOLEAN NOT NULL DEFAULT false;
        
        RAISE NOTICE 'Added IsComplete column to LicenseDetails table';
    ELSE
        RAISE NOTICE 'IsComplete column already exists in LicenseDetails table';
    END IF;
END $$;

-- Update existing records to set IsComplete based on current data
UPDATE org."LicenseDetails" ld
SET "IsComplete" = (
    SELECT 
        CASE 
            WHEN cd."Title" IS NOT NULL 
            AND cd."Title" != ''
            AND EXISTS (
                SELECT 1 FROM org."CompanyOwner" co 
                WHERE co."CompanyId" = cd."Id" 
                AND co."FirstName" IS NOT NULL 
                AND co."FirstName" != ''
                AND co."FatherName" IS NOT NULL 
                AND co."FatherName" != ''
                AND co."ElectronicNationalIdNumber" IS NOT NULL 
                AND co."ElectronicNationalIdNumber" != ''
            )
            AND ld."LicenseNumber" IS NOT NULL 
            AND ld."LicenseNumber" != ''
            AND ld."IssueDate" IS NOT NULL
            AND ld."ExpireDate" IS NOT NULL
            AND ld."OfficeAddress" IS NOT NULL 
            AND ld."OfficeAddress" != ''
            AND EXISTS (
                SELECT 1 FROM org."Guarantors" g 
                WHERE g."CompanyId" = cd."Id" 
                AND g."FirstName" IS NOT NULL 
                AND g."FirstName" != ''
                AND g."FatherName" IS NOT NULL 
                AND g."FatherName" != ''
                AND g."ElectronicNationalIdNumber" IS NOT NULL 
                AND g."ElectronicNationalIdNumber" != ''
            )
            THEN true
            ELSE false
        END
    FROM org."CompanyDetails" cd
    WHERE cd."Id" = ld."CompanyId"
)
WHERE ld."CompanyId" IS NOT NULL;

-- Display summary
DO $$
DECLARE
    total_count INTEGER;
    complete_count INTEGER;
    incomplete_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO total_count FROM org."LicenseDetails";
    SELECT COUNT(*) INTO complete_count FROM org."LicenseDetails" WHERE "IsComplete" = true;
    SELECT COUNT(*) INTO incomplete_count FROM org."LicenseDetails" WHERE "IsComplete" = false;
    
    RAISE NOTICE 'Migration completed:';
    RAISE NOTICE '  Total licenses: %', total_count;
    RAISE NOTICE '  Complete licenses: %', complete_count;
    RAISE NOTICE '  Incomplete licenses: %', incomplete_count;
END $$;
