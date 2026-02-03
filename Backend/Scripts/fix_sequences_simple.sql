-- Fix all Company module sequences after migration
-- Run this in pgAdmin or your PostgreSQL client

-- Fix CompanyDetails sequence
SELECT setval('org."CompanyDetails_Id_seq"', (SELECT MAX("Id") FROM org."CompanyDetails"), true);

-- Fix CompanyOwner sequence
SELECT setval('org."CompanyOwner_Id_seq"', (SELECT MAX("Id") FROM org."CompanyOwner"), true);

-- Fix LicenseDetails sequence
SELECT setval('org."LicenseDetails_Id_seq"', (SELECT MAX("Id") FROM org."LicenseDetails"), true);

-- Fix Guarantors sequence (only if table has data)
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM org."Guarantors") > 0 THEN
        PERFORM setval('org."Guarantors_Id_seq"', (SELECT MAX("Id") FROM org."Guarantors"), true);
    ELSE
        PERFORM setval('org."Guarantors_Id_seq"', 1, false);
    END IF;
END $$;

-- Fix CompanyCancellationInfo sequence (only if table has data)
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM org."CompanyCancellationInfo") > 0 THEN
        PERFORM setval('org."CompanyCancellationInfo_Id_seq"', (SELECT MAX("Id") FROM org."CompanyCancellationInfo"), true);
    ELSE
        PERFORM setval('org."CompanyCancellationInfo_Id_seq"', 1, false);
    END IF;
END $$;

-- Fix CompanyAccountInfo sequence (only if table has data)
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM org."CompanyAccountInfo") > 0 THEN
        PERFORM setval('org."CompanyAccountInfo_Id_seq"', (SELECT MAX("Id") FROM org."CompanyAccountInfo"), true);
    ELSE
        PERFORM setval('org."CompanyAccountInfo_Id_seq"', 1, false);
    END IF;
END $$;

-- Verify the sequences are now correct
SELECT 
    'CompanyDetails' as table_name,
    last_value as next_id,
    is_called
FROM org."CompanyDetails_Id_seq"
UNION ALL
SELECT 
    'CompanyOwner',
    last_value,
    is_called
FROM org."CompanyOwner_Id_seq"
UNION ALL
SELECT 
    'LicenseDetails',
    last_value,
    is_called
FROM org."LicenseDetails_Id_seq"
UNION ALL
SELECT 
    'Guarantors',
    last_value,
    is_called
FROM org."Guarantors_Id_seq"
UNION ALL
SELECT 
    'CompanyCancellationInfo',
    last_value,
    is_called
FROM org."CompanyCancellationInfo_Id_seq"
UNION ALL
SELECT 
    'CompanyAccountInfo',
    last_value,
    is_called
FROM org."CompanyAccountInfo_Id_seq";
