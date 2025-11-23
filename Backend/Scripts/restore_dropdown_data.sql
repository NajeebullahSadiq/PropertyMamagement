-- RESTORE SCRIPT FOR DROPDOWN DATA
-- Run this script ONLY if dropdown data was lost during migration
-- This will restore all lookup table data from the backup tables

-- Restore PropertyTypes
INSERT INTO tr."PropertyTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_PropertyTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."PropertyTypes" pt WHERE pt.Id = backup_PropertyTypes.Id
);

-- Restore TransactionTypes
INSERT INTO tr."TransactionTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_TransactionTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."TransactionTypes" tt WHERE tt.Id = backup_TransactionTypes.Id
);

-- Restore EducationLevels
INSERT INTO tr."EducationLevels" (Id, Name, Sorter)
SELECT Id, Name, Sorter FROM tr.backup_EducationLevels
WHERE NOT EXISTS (
    SELECT 1 FROM tr."EducationLevels" el WHERE el.Id = backup_EducationLevels.Id
);

-- Restore IdentityCardTypes
INSERT INTO tr."IdentityCardTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_IdentityCardTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."IdentityCardTypes" ict WHERE ict.Id = backup_IdentityCardTypes.Id
);

-- Restore AddressTypes
INSERT INTO tr."AddressTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_AddressTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."AddressTypes" at WHERE at.Id = backup_AddressTypes.Id
);

-- Restore GuaranteeTypes
INSERT INTO tr."GuaranteeTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_GuaranteeTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."GuaranteeTypes" gt WHERE gt.Id = backup_GuaranteeTypes.Id
);

-- Restore PunitTypes
INSERT INTO tr."PunitTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_PunitTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."PunitTypes" pt WHERE pt.Id = backup_PunitTypes.Id
);

-- Restore Areas
INSERT INTO tr."Areas" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_Areas
WHERE NOT EXISTS (
    SELECT 1 FROM tr."Areas" a WHERE a.Id = backup_Areas.Id
);

-- Restore ViolationTypes
INSERT INTO tr."ViolationTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_ViolationTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."ViolationTypes" vt WHERE vt.Id = backup_ViolationTypes.Id
);

-- Restore LostdocumentsTypes
INSERT INTO tr."LostdocumentsTypes" (Id, Name, Des)
SELECT Id, Name, Des FROM tr.backup_LostdocumentsTypes
WHERE NOT EXISTS (
    SELECT 1 FROM tr."LostdocumentsTypes" ldt WHERE ldt.Id = backup_LostdocumentsTypes.Id
);

-- Restore Locations
INSERT INTO tr."Locations" (Id, Name, ParentId, Type)
SELECT Id, Name, ParentId, Type FROM tr.backup_Locations
WHERE NOT EXISTS (
    SELECT 1 FROM tr."Locations" l WHERE l.Id = backup_Locations.Id
);

-- Verify restoration
SELECT 
    'PropertyTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."PropertyTypes"
UNION ALL
SELECT 
    'TransactionTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."TransactionTypes"
UNION ALL
SELECT 
    'EducationLevels' as table_name, 
    COUNT(*) as record_count 
FROM tr."EducationLevels"
UNION ALL
SELECT 
    'IdentityCardTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."IdentityCardTypes"
UNION ALL
SELECT 
    'AddressTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."AddressTypes"
UNION ALL
SELECT 
    'GuaranteeTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."GuaranteeTypes"
UNION ALL
SELECT 
    'PunitTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."PunitTypes"
UNION ALL
SELECT 
    'Areas' as table_name, 
    COUNT(*) as record_count 
FROM tr."Areas"
UNION ALL
SELECT 
    'ViolationTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."ViolationTypes"
UNION ALL
SELECT 
    'LostdocumentsTypes' as table_name, 
    COUNT(*) as record_count 
FROM tr."LostdocumentsTypes"
UNION ALL
SELECT 
    'Locations' as table_name, 
    COUNT(*) as record_count 
FROM tr."Locations";

-- Display message
SELECT 'RESTORATION COMPLETED - All dropdown data has been restored!' as status;
