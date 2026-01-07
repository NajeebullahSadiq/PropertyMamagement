using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to update LicenseView to include financial and administrative fields.
    /// </summary>
    public partial class UpdateLicenseViewWithFinancialFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop and recreate the LicenseView to include new financial fields
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS public.""LicenseView"";
                
                CREATE OR REPLACE VIEW public.""LicenseView"" AS
                SELECT 
                    cd.""Id"" AS ""CompanyId"",
                    co.""PhoneNumber"",
                    co.""WhatsAppNumber"",
                    cd.""Title"",
                    cd.""TIN"" AS ""Tin"",
                    co.""FirstName"",
                    co.""FatherName"",
                    co.""GrandFatherName"",
                    co.""DateofBirth"",
                    co.""IndentityCardNumber"",
                    co.""PothoPath"" AS ""OwnerPhoto"",
                    ld.""LicenseNumber"",
                    ld.""OfficeAddress"",
                    ld.""IssueDate"",
                    ld.""ExpireDate"",
                    pp.""Dari"" AS ""PermanentProvinceName"",
                    pd.""Dari"" AS ""PermanentDistrictName"",
                    co.""PermanentVillage"",
                    tp.""Dari"" AS ""TemporaryProvinceName"",
                    td.""Dari"" AS ""TemporaryDistrictName"",
                    co.""TemporaryVillage"",
                    -- Financial and Administrative Fields
                    ld.""RoyaltyAmount"",
                    ld.""RoyaltyDate"",
                    ld.""PenaltyAmount"",
                    ld.""PenaltyDate"",
                    ld.""HrLetter"",
                    ld.""HrLetterDate""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                LEFT JOIN look.""Location"" pp ON co.""PermanentProvinceId"" = pp.""ID""
                LEFT JOIN look.""Location"" pd ON co.""PermanentDistrictId"" = pd.""ID""
                LEFT JOIN look.""Location"" tp ON co.""TemporaryProvinceId"" = tp.""ID""
                LEFT JOIN look.""Location"" td ON co.""TemporaryDistrictId"" = td.""ID"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the previous version of LicenseView without financial fields
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS public.""LicenseView"";
                
                CREATE OR REPLACE VIEW public.""LicenseView"" AS
                SELECT 
                    cd.""Id"" AS ""CompanyId"",
                    co.""PhoneNumber"",
                    co.""WhatsAppNumber"",
                    cd.""Title"",
                    cd.""TIN"" AS ""Tin"",
                    co.""FirstName"",
                    co.""FatherName"",
                    co.""GrandFatherName"",
                    co.""DateofBirth"",
                    co.""IndentityCardNumber"",
                    co.""PothoPath"" AS ""OwnerPhoto"",
                    ld.""LicenseNumber"",
                    ld.""OfficeAddress"",
                    ld.""IssueDate"",
                    ld.""ExpireDate"",
                    pp.""Dari"" AS ""PermanentProvinceName"",
                    pd.""Dari"" AS ""PermanentDistrictName"",
                    co.""PermanentVillage"",
                    tp.""Dari"" AS ""TemporaryProvinceName"",
                    td.""Dari"" AS ""TemporaryDistrictName"",
                    co.""TemporaryVillage""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                LEFT JOIN look.""Location"" pp ON co.""PermanentProvinceId"" = pp.""ID""
                LEFT JOIN look.""Location"" pd ON co.""PermanentDistrictId"" = pd.""ID""
                LEFT JOIN look.""Location"" tp ON co.""TemporaryProvinceId"" = tp.""ID""
                LEFT JOIN look.""Location"" td ON co.""TemporaryDistrictId"" = td.""ID"";
            ");
        }
    }
}
