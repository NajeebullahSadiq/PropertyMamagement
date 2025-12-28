using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPIBackend.Configuration;

#nullable disable

namespace WebAPIBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251228153000_MovePropertyTypeToPropertyDetails")]
    public partial class MovePropertyTypeToPropertyDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Add CustomPropertyType to PropertyDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'PropertyDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" ADD COLUMN ""CustomPropertyType"" text NULL;
                    END IF;

                    -- Backfill PropertyTypeId from BuyerDetails where PropertyDetails.PropertyTypeId is NULL
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'PropertyTypeId'
                    ) THEN
                        UPDATE tr.""PropertyDetails"" pd
                        SET ""PropertyTypeId"" = bd.""PropertyTypeId""
                        FROM tr.""BuyerDetails"" bd
                        WHERE bd.""PropertyDetailsId"" = pd.""Id""
                          AND pd.""PropertyTypeId"" IS NULL
                          AND bd.""PropertyTypeId"" IS NOT NULL;
                    END IF;

                    -- Backfill CustomPropertyType from BuyerDetails where PropertyDetails.CustomPropertyType is NULL
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        UPDATE tr.""PropertyDetails"" pd
                        SET ""CustomPropertyType"" = bd.""CustomPropertyType""
                        FROM tr.""BuyerDetails"" bd
                        WHERE bd.""PropertyDetailsId"" = pd.""Id""
                          AND pd.""CustomPropertyType"" IS NULL
                          AND bd.""CustomPropertyType"" IS NOT NULL;
                    END IF;

                    -- Backfill PropertyTypeId from SellerDetails where PropertyDetails.PropertyTypeId is NULL
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'SellerDetails'
                          AND column_name = 'PropertyTypeId'
                    ) THEN
                        UPDATE tr.""PropertyDetails"" pd
                        SET ""PropertyTypeId"" = sd.""PropertyTypeId""
                        FROM tr.""SellerDetails"" sd
                        WHERE sd.""PropertyDetailsId"" = pd.""Id""
                          AND pd.""PropertyTypeId"" IS NULL
                          AND sd.""PropertyTypeId"" IS NOT NULL;
                    END IF;

                    -- Backfill CustomPropertyType from SellerDetails where PropertyDetails.CustomPropertyType is NULL
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'SellerDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        UPDATE tr.""PropertyDetails"" pd
                        SET ""CustomPropertyType"" = sd.""CustomPropertyType""
                        FROM tr.""SellerDetails"" sd
                        WHERE sd.""PropertyDetailsId"" = pd.""Id""
                          AND pd.""CustomPropertyType"" IS NULL
                          AND sd.""CustomPropertyType"" IS NOT NULL;
                    END IF;

                    -- Remove old columns to avoid duplicate/conflicting storage
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""PropertyTypeId"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""CustomPropertyType"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'SellerDetails'
                          AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""PropertyTypeId"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'SellerDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""CustomPropertyType"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS ""GetPrintType"";

                CREATE OR REPLACE VIEW ""GetPrintType"" AS
                SELECT 
                    pd.""Id"",
                    pd.""DocumentType"" as ""Doctype"",
                    pd.""PNumber"" as ""Pnumber"",
                    pd.""PArea"",
                    pd.""NumofRooms"",
                    pd.""north"",
                    pd.""south"",
                    pd.""west"",
                    pd.""east"",
                    pd.""Price"",
                    pd.""PriceText"",
                    pd.""RoyaltyAmount"",
                    CASE
                        WHEN pt.""Name"" ILIKE 'Other' AND pd.""CustomPropertyType"" IS NOT NULL THEN pd.""CustomPropertyType""
                        ELSE pt.""Name""
                    END as ""PropertypeType"",
                    pd.""CreatedAt"",
                    pd.""TransactionDate"" as ""DeedDate"",

                    -- Property Images and Documents
                    pd.""FilePath"",
                    pd.""PreviousDocumentsPath"",
                    pd.""ExistingDocumentsPath"",
                    pd.""DocumentType"",
                    pd.""IssuanceNumber"",
                    pd.""IssuanceDate"",
                    pd.""SerialNumber"",
                    pd.""TransactionDate"",
                    pd.""PNumber"",

                    -- Property Address Information
                    pa_prov.""Name"" as ""Province"",
                    pa_dist.""Name"" as ""District"",
                    pa.""Village"",

                    -- Seller Details
                    sd.""FirstName"" as ""SellerFirstName"",
                    sd.""FatherName"" as ""SellerFatherName"",
                    sd.""IndentityCardNumber"" as ""SellerIndentityCardNumber"",
                    sd.""PaddressVillage"" as ""SellerVillage"",
                    sd.""TaddressVillage"" as ""TSellerVillage"",
                    sd.""photo"" as ""SellerPhoto"",

                    -- Seller Address Information
                    s_perm_prov.""Name"" as ""SellerProvince"",
                    s_perm_dist.""Name"" as ""SellerDistrict"",
                    s_temp_prov.""Name"" as ""TSellerProvince"",
                    s_temp_dist.""Name"" as ""TSellerDistrict"",

                    -- Buyer Details
                    bd.""FirstName"" as ""BuyerFirstName"",
                    bd.""FatherName"" as ""BuyerFatherName"",
                    bd.""IndentityCardNumber"" as ""BuyerIndentityCardNumber"",
                    bd.""PaddressVillage"" as ""BuyerVillage"",
                    bd.""TaddressVillage"" as ""TBuyerVillage"",
                    bd.""photo"" as ""BuyerPhoto"",

                    -- Buyer Address Information
                    b_perm_prov.""Name"" as ""BuyerProvince"",
                    b_perm_dist.""Name"" as ""BuyerDistrict"",
                    b_temp_prov.""Name"" as ""TBuyerProvince"",
                    b_temp_dist.""Name"" as ""TBuyerDistrict"",

                    -- Witness 1 Details
                    wd1.""FirstName"" as ""WitnessOneFirstName"",
                    wd1.""FatherName"" as ""WitnessOneFatherName"",
                    wd1.""IndentityCardNumber"" as ""WitnessOneIndentityCardNumber"",

                    -- Witness 2 Details
                    wd2.""FirstName"" as ""WitnessTwoFirstName"",
                    wd2.""FatherName"" as ""WitnessTwoFatherName"",
                    wd2.""IndentityCardNumber"" as ""WitnessTwoIndentityCardNumber"",

                    -- Unit Type and Transaction Type
                    ut.""Name"" as ""UnitType"",
                    tt.""Name"" as ""TransactionType""

                FROM tr.""PropertyDetails"" pd
                LEFT JOIN look.""PropertyType"" pt ON pd.""PropertyTypeId"" = pt.""Id""
                LEFT JOIN look.""PUnitType"" ut ON pd.""PUnitTypeId"" = ut.""Id""
                LEFT JOIN look.""TransactionType"" tt ON pd.""TransactionTypeId"" = tt.""Id""

                -- Property Address
                LEFT JOIN tr.""PropertyAddress"" pa ON pd.""Id"" = pa.""PropertyDetailsId""
                LEFT JOIN look.""Location"" pa_prov ON pa.""ProvinceId"" = pa_prov.""ID""
                LEFT JOIN look.""Location"" pa_dist ON pa.""DistrictId"" = pa_dist.""ID""

                -- Seller Details and Address
                LEFT JOIN tr.""SellerDetails"" sd ON pd.""Id"" = sd.""PropertyDetailsId""
                LEFT JOIN look.""Location"" s_perm_prov ON sd.""PaddressProvinceId"" = s_perm_prov.""ID""
                LEFT JOIN look.""Location"" s_perm_dist ON sd.""PaddressDistrictId"" = s_perm_dist.""ID""
                LEFT JOIN look.""Location"" s_temp_prov ON sd.""TaddressProvinceId"" = s_temp_prov.""ID""
                LEFT JOIN look.""Location"" s_temp_dist ON sd.""TaddressDistrictId"" = s_temp_dist.""ID""

                -- Buyer Details and Address
                LEFT JOIN tr.""BuyerDetails"" bd ON pd.""Id"" = bd.""PropertyDetailsId""
                LEFT JOIN look.""Location"" b_perm_prov ON bd.""PaddressProvinceId"" = b_perm_prov.""ID""
                LEFT JOIN look.""Location"" b_perm_dist ON bd.""PaddressDistrictId"" = b_perm_dist.""ID""
                LEFT JOIN look.""Location"" b_temp_prov ON bd.""TaddressProvinceId"" = b_temp_prov.""ID""
                LEFT JOIN look.""Location"" b_temp_dist ON bd.""TaddressDistrictId"" = b_temp_dist.""ID""

                -- Witness 1 Details (First witness record)
                LEFT JOIN LATERAL (
                    SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                    FROM tr.""WitnessDetails""
                    WHERE ""PropertyDetailsId"" = pd.""Id""
                    ORDER BY ""Id"" ASC
                    LIMIT 1
                ) wd1 ON true

                -- Witness 2 Details (Second witness record)
                LEFT JOIN LATERAL (
                    SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                    FROM tr.""WitnessDetails""
                    WHERE ""PropertyDetailsId"" = pd.""Id""
                    ORDER BY ""Id"" ASC
                    OFFSET 1
                    LIMIT 1
                ) wd2 ON true

                WHERE pd.""iscomplete"" = true;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Restore BuyerDetails/SellerDetails columns (best-effort)
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""PropertyTypeId"" integer NULL;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""CustomPropertyType"" text NULL;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'SellerDetails'
                          AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""PropertyTypeId"" integer NULL;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'SellerDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""CustomPropertyType"" text NULL;
                    END IF;

                    -- Remove CustomPropertyType from PropertyDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'PropertyDetails'
                          AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" DROP COLUMN ""CustomPropertyType"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS ""GetPrintType"";

                CREATE OR REPLACE VIEW ""GetPrintType"" AS
                SELECT 
                    pd.""Id"",
                    pd.""DocumentType"" as ""Doctype"",
                    pd.""PNumber"" as ""Pnumber"",
                    pd.""PArea"",
                    pd.""NumofRooms"",
                    pd.""north"",
                    pd.""south"",
                    pd.""west"",
                    pd.""east"",
                    pd.""Price"",
                    pd.""PriceText"",
                    pd.""RoyaltyAmount"",
                    pt.""Name"" as ""PropertypeType"",
                    pd.""CreatedAt"",
                    pd.""TransactionDate"" as ""DeedDate"",

                    -- Property Images and Documents
                    pd.""FilePath"",
                    pd.""PreviousDocumentsPath"",
                    pd.""ExistingDocumentsPath"",
                    pd.""DocumentType"",
                    pd.""IssuanceNumber"",
                    pd.""IssuanceDate"",
                    pd.""SerialNumber"",
                    pd.""TransactionDate"",
                    pd.""PNumber"",

                    -- Property Address Information
                    pa_prov.""Name"" as ""Province"",
                    pa_dist.""Name"" as ""District"",
                    pa.""Village"",

                    -- Seller Details
                    sd.""FirstName"" as ""SellerFirstName"",
                    sd.""FatherName"" as ""SellerFatherName"",
                    sd.""IndentityCardNumber"" as ""SellerIndentityCardNumber"",
                    sd.""PaddressVillage"" as ""SellerVillage"",
                    sd.""TaddressVillage"" as ""TSellerVillage"",
                    sd.""photo"" as ""SellerPhoto"",

                    -- Seller Address Information
                    s_perm_prov.""Name"" as ""SellerProvince"",
                    s_perm_dist.""Name"" as ""SellerDistrict"",
                    s_temp_prov.""Name"" as ""TSellerProvince"",
                    s_temp_dist.""Name"" as ""TSellerDistrict"",

                    -- Buyer Details
                    bd.""FirstName"" as ""BuyerFirstName"",
                    bd.""FatherName"" as ""BuyerFatherName"",
                    bd.""IndentityCardNumber"" as ""BuyerIndentityCardNumber"",
                    bd.""PaddressVillage"" as ""BuyerVillage"",
                    bd.""TaddressVillage"" as ""TBuyerVillage"",
                    bd.""photo"" as ""BuyerPhoto"",

                    -- Buyer Address Information
                    b_perm_prov.""Name"" as ""BuyerProvince"",
                    b_perm_dist.""Name"" as ""BuyerDistrict"",
                    b_temp_prov.""Name"" as ""TBuyerProvince"",
                    b_temp_dist.""Name"" as ""TBuyerDistrict"",

                    -- Witness 1 Details
                    wd1.""FirstName"" as ""WitnessOneFirstName"",
                    wd1.""FatherName"" as ""WitnessOneFatherName"",
                    wd1.""IndentityCardNumber"" as ""WitnessOneIndentityCardNumber"",

                    -- Witness 2 Details
                    wd2.""FirstName"" as ""WitnessTwoFirstName"",
                    wd2.""FatherName"" as ""WitnessTwoFatherName"",
                    wd2.""IndentityCardNumber"" as ""WitnessTwoIndentityCardNumber"",

                    -- Unit Type and Transaction Type
                    ut.""Name"" as ""UnitType"",
                    tt.""Name"" as ""TransactionType""

                FROM tr.""PropertyDetails"" pd
                LEFT JOIN look.""PropertyType"" pt ON pd.""PropertyTypeId"" = pt.""Id""
                LEFT JOIN look.""PUnitType"" ut ON pd.""PUnitTypeId"" = ut.""Id""
                LEFT JOIN look.""TransactionType"" tt ON pd.""TransactionTypeId"" = tt.""Id""

                -- Property Address
                LEFT JOIN tr.""PropertyAddress"" pa ON pd.""Id"" = pa.""PropertyDetailsId""
                LEFT JOIN look.""Location"" pa_prov ON pa.""ProvinceId"" = pa_prov.""ID""
                LEFT JOIN look.""Location"" pa_dist ON pa.""DistrictId"" = pa_dist.""ID""

                -- Seller Details and Address
                LEFT JOIN tr.""SellerDetails"" sd ON pd.""Id"" = sd.""PropertyDetailsId""
                LEFT JOIN look.""Location"" s_perm_prov ON sd.""PaddressProvinceId"" = s_perm_prov.""ID""
                LEFT JOIN look.""Location"" s_perm_dist ON sd.""PaddressDistrictId"" = s_perm_dist.""ID""
                LEFT JOIN look.""Location"" s_temp_prov ON sd.""TaddressProvinceId"" = s_temp_prov.""ID""
                LEFT JOIN look.""Location"" s_temp_dist ON sd.""TaddressDistrictId"" = s_temp_dist.""ID""

                -- Buyer Details and Address
                LEFT JOIN tr.""BuyerDetails"" bd ON pd.""Id"" = bd.""PropertyDetailsId""
                LEFT JOIN look.""Location"" b_perm_prov ON bd.""PaddressProvinceId"" = b_perm_prov.""ID""
                LEFT JOIN look.""Location"" b_perm_dist ON bd.""PaddressDistrictId"" = b_perm_dist.""ID""
                LEFT JOIN look.""Location"" b_temp_prov ON bd.""TaddressProvinceId"" = b_temp_prov.""ID""
                LEFT JOIN look.""Location"" b_temp_dist ON bd.""TaddressDistrictId"" = b_temp_dist.""ID""

                -- Witness 1 Details (First witness record)
                LEFT JOIN LATERAL (
                    SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                    FROM tr.""WitnessDetails""
                    WHERE ""PropertyDetailsId"" = pd.""Id""
                    ORDER BY ""Id"" ASC
                    LIMIT 1
                ) wd1 ON true

                -- Witness 2 Details (Second witness record)
                LEFT JOIN LATERAL (
                    SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                    FROM tr.""WitnessDetails""
                    WHERE ""PropertyDetailsId"" = pd.""Id""
                    ORDER BY ""Id"" ASC
                    OFFSET 1
                    LIMIT 1
                ) wd2 ON true

                WHERE pd.""iscomplete"" = true;
            ");
        }
    }
}
