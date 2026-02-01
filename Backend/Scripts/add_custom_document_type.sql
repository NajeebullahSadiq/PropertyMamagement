-- Add CustomDocumentType column to PropertyDetails table
-- This allows users to specify custom document types when "سایر" (Other) is selected

-- Check if column exists before adding
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'tr' 
        AND table_name = 'PropertyDetails' 
        AND column_name = 'CustomDocumentType'
    ) THEN
        ALTER TABLE tr."PropertyDetails" 
        ADD COLUMN "CustomDocumentType" TEXT NULL;
        
        RAISE NOTICE 'Column CustomDocumentType added successfully to tr.PropertyDetails';
    ELSE
        RAISE NOTICE 'Column CustomDocumentType already exists in tr.PropertyDetails';
    END IF;
END $$;
