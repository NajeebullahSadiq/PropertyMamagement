-- Check actual column types in the database
SELECT 
    table_schema,
    table_name,
    column_name,
    data_type,
    udt_name
FROM information_schema.columns
WHERE table_schema IN ('tr', 'org')
AND column_name IN ('Price', 'RoyaltyAmount', 'Parea', 'PNumber', 'Tin', 'LicenseNumber', 
                     'SharePercentage', 'ShareAmount', 'HalfPrice', 'ValuationAmount', 
                     'AmountPaid', 'BalanceRemaining')
ORDER BY table_schema, table_name, column_name;
