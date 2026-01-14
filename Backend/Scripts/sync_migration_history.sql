-- Script to sync EF Core migration history with existing database
-- Run this ONCE to mark all existing migrations as applied
-- This will stop the "relation already exists" errors on startup

-- First, ensure the __EFMigrationsHistory table exists
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Insert all migration records (will skip if already exists)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES 
    ('20221206082036_firstMigrate', '9.0.0'),
    ('20230717105911_AddNewColumnToIdentityUser', '9.0.0'),
    ('20251110051331_AddTinColumnToCompanyDetails', '9.0.0'),
    ('20251117054600_AddNationalIdCardFields', '9.0.0'),
    ('20251118055000_AddWitnessNationalIdCardFields', '9.0.0'),
    ('20251122042500_AddRoleTypeAndAuthorizationLetter', '9.0.0'),
    ('20251122050000_AddVehicleHandField', '9.0.0'),
    ('20251122051000_RenameDatabaseNumberToLicenseNumber', '9.0.0'),
    ('20251122052000_AddElectricAndPaperIdTypes', '9.0.0'),
    ('20251123091600_AddLicenseTypeToLicenseDetails', '9.0.0'),
    ('20251206074103_EnableMultipleSellersAndBuyers', '9.0.0'),
    ('20251207071328_AddBlockPropertyType', '9.0.0'),
    ('20251207081500_AddPropertyFieldsToBuyerAndSellerDetails', '9.0.0'),
    ('20251210100000_AddRoleTypeAndDocumentFields', '9.0.0'),
    ('20251211_AddDeedDateToPropertyDetails', '9.0.0'),
    ('20251211_AddPrivateNumberToPropertyDetails', '9.0.0'),
    ('20251211_AddPropertyCancellationTable', '9.0.0'),
    ('20251211_AddRentDatesForLesseeRoles', '9.0.0'),
    ('20251218_AddDynamicDocumentFields', '9.0.0'),
    ('20251218_AddPropertyDocumentCategories', '9.0.0'),
    ('20251218_AddTransactionTypeToBuyerDetails', '9.0.0'),
    ('20251218080000_AddMissingPropertyAndBuyerFields', '9.0.0'),
    ('20251220121500_AddBuyerTransactionTypeColumns', '9.0.0'),
    ('20251220123000_SeedMissingPropertyTypes', '9.0.0'),
    ('20251221_AddTazkiraFields', '9.0.0'),
    ('20251221113243_AddTaxIdentificationNumberAndAdditionalDetailsToSeller', '9.0.0'),
    ('20251223150000_EnsureSellerDetailsColumns', '9.0.0'),
    ('20251225090000_AddTaxIdentificationNumberAndAdditionalDetailsToBuyer', '9.0.0'),
    ('20251227093000_AddPropertyCancellationDocuments', '9.0.0'),
    ('20251227120000_UpdateGetPrintTypeViewWithDocuments', '9.0.0'),
    ('20251228153000_MovePropertyTypeToPropertyDetails', '9.0.0'),
    ('20260102100000_AddTinColumnToCompanyDetailsFixed', '9.0.0'),
    ('20260102120000_ChangeGuaranteeNumbersToBigInt', '9.0.0'),
    ('20260106100000_MergeOwnerAddressIntoCompanyOwner', '9.0.0'),
    ('20260106110000_AddCompanyOwnerAddressHistory', '9.0.0'),
    ('20260106120000_AddDariColumnToEducationLevel', '9.0.0'),
    ('20260106130000_UpdateEducationLevelDariTranslations', '9.0.0'),
    ('20260106140000_AddPhoneAndWhatsAppToCompanyOwner', '9.0.0'),
    ('20260106150000_UpdateLicenseViewWithOwnerPhones', '9.0.0'),
    ('20260107100000_SeparateOfficeAndPersonalAddress', '9.0.0'),
    ('20260107110000_RenameAddressToPermanentTemporary', '9.0.0'),
    ('20260107120000_AddMissingCompanyOwnerColumns', '9.0.0'),
    ('20260107150000_MergeGuaranteeIntoGuarantors', '9.0.0'),
    ('20260107160000_AddLicenseCategoryToLicenseDetails', '9.0.0'),
    ('20260107180000_AddFinancialAdminFieldsToLicenseDetails', '9.0.0'),
    ('20260107180001_UpdateLicenseViewWithFinancialFields', '9.0.0'),
    ('20260108100000_AddCompanyAccountInfo', '9.0.0'),
    ('20260108110000_AddCompanyCancellationInfo', '9.0.0'),
    ('20260108120000_AddGrandFatherNameToGuarantor', '9.0.0'),
    ('20260108200000_AddGuaranteeTypeConditionalFields', '9.0.0'),
    ('20260108200001_UpdateGuaranteeTypeLookup', '9.0.0'),
    ('20260108300000_ApplyPendingSchemaChanges', '9.0.0'),
    ('20260108400000_AddMissingGuarantorColumns', '9.0.0'),
    ('20260108500000_UpdateGuaranteeTypesTo3Options', '9.0.0'),
    ('20260108600000_AddOwnerOwnAddressFields', '9.0.0'),
    ('20260110100000_AddRbacColumnsToUsers', '9.0.0'),
    ('20260110150000_AddRentDatesToVehiclesBuyerDetails', '9.0.0'),
    ('20260112100000_CreateUserProfileWithCompanyView', '9.0.0'),
    ('20260113100000_RestrictVehicleSellerRoleTypes', '9.0.0'),
    ('20260113200000_RestrictVehicleBuyerRoleTypes', '9.0.0'),
    ('20260113300000_AddSecuritiesDistribution', '9.0.0'),
    ('20260113400000_SplitSerialNumbersToStartEnd', '9.0.0'),
    ('20260113500000_AddPetitionWriterSecurities', '9.0.0'),
    ('20260114102718_AddSecuritiesControlTable', '9.0.0'),
    ('20260115000000_ImprovePropertyManagementStructure', '9.0.0'),
    ('20260115100000_FixLicenseViewSafe', '9.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- Verify the LicenseView exists and recreate if needed
DROP VIEW IF EXISTS public."LicenseView";

CREATE OR REPLACE VIEW public."LicenseView" AS
SELECT 
    cd."Id" AS "CompanyId",
    co."PhoneNumber",
    co."WhatsAppNumber",
    cd."Title",
    cd."TIN",
    co."FirstName",
    co."FatherName",
    co."GrandFatherName",
    co."DateofBirth",
    co."IndentityCardNumber",
    co."PothoPath" AS "OwnerPhoto",
    ld."LicenseNumber",
    ld."OfficeAddress",
    ld."IssueDate",
    ld."ExpireDate",
    pp."Dari" AS "PermanentProvinceName",
    pd."Dari" AS "PermanentDistrictName",
    co."PermanentVillage",
    tp."Dari" AS "TemporaryProvinceName",
    td."Dari" AS "TemporaryDistrictName",
    co."TemporaryVillage",
    ld."RoyaltyAmount",
    ld."RoyaltyDate",
    ld."PenaltyAmount",
    ld."PenaltyDate",
    ld."HrLetter",
    ld."HrLetterDate"
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
LEFT JOIN org."LicenseDetails" ld ON cd."Id" = ld."CompanyId"
LEFT JOIN look."Location" pp ON co."PermanentProvinceId" = pp."ID"
LEFT JOIN look."Location" pd ON co."PermanentDistrictId" = pd."ID"
LEFT JOIN look."Location" tp ON co."TemporaryProvinceId" = tp."ID"
LEFT JOIN look."Location" td ON co."TemporaryDistrictId" = td."ID";

SELECT 'Migration history synced successfully!' AS status;
