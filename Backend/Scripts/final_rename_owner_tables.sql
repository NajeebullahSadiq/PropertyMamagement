-- Final rename - only rename what still exists

-- Check what tables exist now
SELECT tablename 
FROM pg_tables 
WHERE schemaname = 'org' 
  AND tablename LIKE '%ompanyOwner%'
ORDER BY tablename;

-- Rename CompanyOwners to CompanyOwner (if it still exists)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'org' AND tablename = 'CompanyOwners') THEN
        ALTER TABLE org."CompanyOwners" RENAME TO "CompanyOwner";
        RAISE NOTICE 'Renamed CompanyOwners to CompanyOwner';
    ELSE
        RAISE NOTICE 'CompanyOwners table does not exist, skipping';
    END IF;
END $$;

-- Rename CompanyOwnerAddresses to CompanyOwnerAddress (if it still exists)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'org' AND tablename = 'CompanyOwnerAddresses') THEN
        ALTER TABLE org."CompanyOwnerAddresses" RENAME TO "CompanyOwnerAddress";
        RAISE NOTICE 'Renamed CompanyOwnerAddresses to CompanyOwnerAddress';
    ELSE
        RAISE NOTICE 'CompanyOwnerAddresses table does not exist, skipping';
    END IF;
END $$;

-- Rename CompanyOwnerAddressHistories to CompanyOwnerAddressHistory (if it still exists)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'org' AND tablename = 'CompanyOwnerAddressHistories') THEN
        ALTER TABLE org."CompanyOwnerAddressHistories" RENAME TO "CompanyOwnerAddressHistory";
        RAISE NOTICE 'Renamed CompanyOwnerAddressHistories to CompanyOwnerAddressHistory';
    ELSE
        RAISE NOTICE 'CompanyOwnerAddressHistories table does not exist, skipping';
    END IF;
END $$;

-- Verify the final result
SELECT 'Final table check:' as status;
SELECT tablename, 
       (SELECT COUNT(*) FROM org."CompanyOwner" WHERE tablename = 'CompanyOwner') as record_count
FROM pg_tables 
WHERE schemaname = 'org' 
  AND tablename LIKE '%ompanyOwner%'
ORDER BY tablename;
