-- Check EducationLevel table structure
SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'look' 
  AND table_name = 'EducationLevel'
ORDER BY ordinal_position;

-- Check Location table structure
SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'look' 
  AND table_name = 'Location'
ORDER BY ordinal_position;

-- Check GuaranteeType table structure
SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'look' 
  AND table_name = 'GuaranteeType'
ORDER BY ordinal_position;
