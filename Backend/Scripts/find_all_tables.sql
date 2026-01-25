-- Find all tables in the tr schema
SELECT 
    table_schema,
    table_name
FROM information_schema.tables
WHERE table_schema = 'tr'
ORDER BY table_name;

-- Find all columns with numeric types that might need conversion
SELECT 
    table_schema,
    table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_schema = 'tr'
  AND data_type IN ('double precision', 'numeric', 'integer', 'bigint')
  AND column_name IN (
    'Price', 'RoyaltyAmount', 'PArea', 'PNumber',
    'HalfPrice', 'SharePercentage', 'ShareAmount',
    'Tin', 'LicenseNumber', 'ValuationAmount', 
    'AmountPaid', 'BalanceRemaining'
  )
ORDER BY table_name, column_name;
