using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGetPrintTypeViewWithDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    pa_prov.""Dari"" as ""ProvinceDari"",
                    pa_dist.""Dari"" as ""DistrictDari"",
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
                    s_perm_prov.""Dari"" as ""SellerProvinceDari"",
                    s_perm_dist.""Dari"" as ""SellerDistrictDari"",
                    s_temp_prov.""Name"" as ""TSellerProvince"",
                    s_temp_dist.""Name"" as ""TSellerDistrict"",
                    s_temp_prov.""Dari"" as ""TSellerProvinceDari"",
                    s_temp_dist.""Dari"" as ""TSellerDistrictDari"",
                    
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
                    b_perm_prov.""Dari"" as ""BuyerProvinceDari"",
                    b_perm_dist.""Dari"" as ""BuyerDistrictDari"",
                    b_temp_prov.""Name"" as ""TBuyerProvince"",
                    b_temp_dist.""Name"" as ""TBuyerDistrict"",
                    b_temp_prov.""Dari"" as ""TBuyerProvinceDari"",
                    b_temp_dist.""Dari"" as ""TBuyerDistrictDari"",
                    
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS ""GetPrintType"";
                
                CREATE VIEW ""GetPrintType"" AS
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
                    s_perm_prov.""Dari"" as ""SellerProvinceDari"",
                    s_perm_dist.""Dari"" as ""SellerDistrictDari"",
                    s_temp_prov.""Name"" as ""TSellerProvince"",
                    s_temp_dist.""Name"" as ""TSellerDistrict"",
                    s_temp_prov.""Dari"" as ""TSellerProvinceDari"",
                    s_temp_dist.""Dari"" as ""TSellerDistrictDari"",
                    
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
                    b_perm_prov.""Dari"" as ""BuyerProvinceDari"",
                    b_perm_dist.""Dari"" as ""BuyerDistrictDari"",
                    b_temp_prov.""Name"" as ""TBuyerProvince"",
                    b_temp_dist.""Name"" as ""TBuyerDistrict"",
                    b_temp_prov.""Dari"" as ""TBuyerProvinceDari"",
                    b_temp_dist.""Dari"" as ""TBuyerDistrictDari"",
                    
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
