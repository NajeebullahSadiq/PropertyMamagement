-- =====================================================
-- Remove Status Fields from PropertyDetails
-- Date: 2026-03-08
-- Purpose: Remove Status, VerifiedBy, VerifiedAt, ApprovedBy, ApprovedAt columns
-- =====================================================

-- Drop index first
DROP INDEX IF EXISTS tr."IX_PropertyDetails_Status";

-- Remove columns
ALTER TABLE tr."PropertyDetails" DROP COLUMN IF EXISTS "Status";
ALTER TABLE tr."PropertyDetails" DROP COLUMN IF EXISTS "VerifiedBy";
ALTER TABLE tr."PropertyDetails" DROP COLUMN IF EXISTS "VerifiedAt";
ALTER TABLE tr."PropertyDetails" DROP COLUMN IF EXISTS "ApprovedBy";
ALTER TABLE tr."PropertyDetails" DROP COLUMN IF EXISTS "ApprovedAt";

-- Confirmation
DO $$
BEGIN
    RAISE NOTICE 'Status fields removed from PropertyDetails table successfully';
END $$;
