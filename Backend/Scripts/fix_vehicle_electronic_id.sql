-- ============================================
-- Fix Vehicle Tables - Rename IndentityCardNumber to ElectronicNationalIdNumber
-- Migration: 20260118_RemovePaperIdFields_Vehicle (corrected table names)
-- ============================================

-- VehiclesSellerDetails (note: plural)
ALTER TABLE tr."VehiclesSellerDetails" 
RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";

ALTER TABLE tr."VehiclesSellerDetails" 
ALTER COLUMN "ElectronicNationalIdNumber" TYPE character varying(50);

-- VehiclesBuyerDetails (note: plural)
ALTER TABLE tr."VehiclesBuyerDetails" 
RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";

ALTER TABLE tr."VehiclesBuyerDetails" 
ALTER COLUMN "ElectronicNationalIdNumber" TYPE character varying(50);

-- VehiclesWitnessDetails (note: plural)
ALTER TABLE tr."VehiclesWitnessDetails" 
RENAME COLUMN "IndentityCardNumber" TO "ElectronicNationalIdNumber";

ALTER TABLE tr."VehiclesWitnessDetails" 
ALTER COLUMN "ElectronicNationalIdNumber" TYPE character varying(50);

-- Verify the changes
SELECT 
    'VehiclesSellerDetails' as table_name,
    column_name, 
    data_type, 
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'tr' 
  AND table_name = 'VehiclesSellerDetails'
  AND column_name = 'ElectronicNationalIdNumber'
UNION ALL
SELECT 
    'VehiclesBuyerDetails' as table_name,
    column_name, 
    data_type, 
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'tr' 
  AND table_name = 'VehiclesBuyerDetails'
  AND column_name = 'ElectronicNationalIdNumber'
UNION ALL
SELECT 
    'VehiclesWitnessDetails' as table_name,
    column_name, 
    data_type, 
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'tr' 
  AND table_name = 'VehiclesWitnessDetails'
  AND column_name = 'ElectronicNationalIdNumber';
