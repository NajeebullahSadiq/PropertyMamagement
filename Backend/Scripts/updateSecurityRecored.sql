-- 2. Petition Writer Securities (org schema)
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000062', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000033';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000445', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000035';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000164', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000036';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000131', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000037';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000808', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000045';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000020', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000046';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00000352', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000047';
UPDATE org."PetitionWriterSecurities" SET "LicenseNumber" = 'KBL-00001070', "UpdatedAt" = NOW(), "UpdatedBy" = 'manual_update' WHERE "LicenseNumber" = 'KBL-00000048';


-- 3. Petition Writer Monitoring Records
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000062' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000033';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000445' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000035';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000164' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000036';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000131' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000037';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000808' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000045';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000020' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000046';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00000352' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000047';
UPDATE org."PetitionWriterMonitoringRecords" SET "PetitionWriterLicenseNumber" = 'KBL-00001070' WHERE "PetitionWriterLicenseNumber" = 'KBL-00000048';


-- 4. Activity Monitoring Records (single-table design covers violations too)
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000062' WHERE "LicenseNumber" = 'KBL-00000033';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000445' WHERE "LicenseNumber" = 'KBL-00000035';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000164' WHERE "LicenseNumber" = 'KBL-00000036';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000131' WHERE "LicenseNumber" = 'KBL-00000037';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000808' WHERE "LicenseNumber" = 'KBL-00000045';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000020' WHERE "LicenseNumber" = 'KBL-00000046';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00000352' WHERE "LicenseNumber" = 'KBL-00000047';
UPDATE org."ActivityMonitoringRecords" SET "LicenseNumber" = 'KBL-00001070' WHERE "LicenseNumber" = 'KBL-00000048';