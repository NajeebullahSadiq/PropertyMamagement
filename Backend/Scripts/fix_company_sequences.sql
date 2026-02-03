-- Fix sequences for Company module tables after migration
-- This ensures new records get correct IDs after migration

-- Fix CompanyDetails sequence
SELECT setval(
    pg_get_serial_sequence('org."CompanyDetails"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."CompanyDetails"), 1),
    true
);

-- Fix CompanyOwner sequence
SELECT setval(
    pg_get_serial_sequence('org."CompanyOwner"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."CompanyOwner"), 1),
    true
);

-- Fix LicenseDetails sequence
SELECT setval(
    pg_get_serial_sequence('org."LicenseDetails"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."LicenseDetails"), 1),
    true
);

-- Fix Guarantors sequence
SELECT setval(
    pg_get_serial_sequence('org."Guarantors"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."Guarantors"), 1),
    true
);

-- Fix CompanyCancellationInfo sequence
SELECT setval(
    pg_get_serial_sequence('org."CompanyCancellationInfo"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM org."CompanyCancellationInfo"), 1),
    true
);

-- Verify sequences
SELECT 
    'CompanyDetails' as table_name,
    last_value as next_id
FROM pg_sequences 
WHERE schemaname = 'org' AND sequencename = 'CompanyDetails_Id_seq'
UNION ALL
SELECT 
    'CompanyOwner',
    last_value
FROM pg_sequences 
WHERE schemaname = 'org' AND sequencename = 'CompanyOwner_Id_seq'
UNION ALL
SELECT 
    'LicenseDetails',
    last_value
FROM pg_sequences 
WHERE schemaname = 'org' AND sequencename = 'LicenseDetails_Id_seq'
UNION ALL
SELECT 
    'Guarantors',
    last_value
FROM pg_sequences 
WHERE schemaname = 'org' AND sequencename = 'Guarantors_Id_seq';
