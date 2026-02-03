-- Check current state of all CompanyOwner tables

SELECT 
    tablename,
    (SELECT COUNT(*) 
     FROM information_schema.columns 
     WHERE table_schema = 'org' 
       AND table_name = t.tablename) as column_count
FROM pg_tables t
WHERE schemaname = 'org' 
  AND tablename LIKE '%ompanyOwner%'
ORDER BY tablename;

-- Check record counts for each table
DO $$
DECLARE
    rec RECORD;
    cnt INTEGER;
BEGIN
    FOR rec IN 
        SELECT tablename 
        FROM pg_tables 
        WHERE schemaname = 'org' 
          AND tablename LIKE '%ompanyOwner%'
    LOOP
        EXECUTE format('SELECT COUNT(*) FROM org."%I"', rec.tablename) INTO cnt;
        RAISE NOTICE 'Table: % - Records: %', rec.tablename, cnt;
    END LOOP;
END $$;
