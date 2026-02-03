-- =====================================================
-- Script: Change Vehicle Number Fields to String/Text
-- Description: Changes PilateNo, PermitNo, EnginNo, and ShasiNo columns from INTEGER to TEXT
-- Date: 2026-02-02
-- =====================================================

-- Change PermitNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE TEXT USING "PermitNo"::TEXT;

-- Change PilateNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PilateNo" TYPE TEXT USING "PilateNo"::TEXT;

-- Change EnginNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "EnginNo" TYPE TEXT USING "EnginNo"::TEXT;

-- Change ShasiNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "ShasiNo" TYPE TEXT USING "ShasiNo"::TEXT;

-- =====================================================
-- End of Script
-- =====================================================
