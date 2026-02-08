-- Check if SecuritiesDistribution table exists and show its structure
SELECT 
    table_schema,
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'org' 
  AND table_name = 'SecuritiesDistribution'
ORDER BY ordinal_position;

-- Check if SecuritiesDistributionItem table exists
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM information_schema.tables 
            WHERE table_schema = 'org' 
              AND table_name = 'SecuritiesDistributionItem'
        ) THEN 'SecuritiesDistributionItem table EXISTS'
        ELSE 'SecuritiesDistributionItem table DOES NOT EXIST - Migration needed'
    END as status;

-- Count existing records
SELECT 
    COUNT(*) as total_records,
    COUNT(CASE WHEN "Status" = true THEN 1 END) as active_records
FROM org."SecuritiesDistribution";
