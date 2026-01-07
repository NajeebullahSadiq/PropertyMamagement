using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeparateOfficeAndPersonalAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename existing "Permanent" address columns to "Office" address
            migrationBuilder.RenameColumn(
                name: "PermanentProvinceId",
                schema: "org",
                table: "CompanyOwner",
                newName: "OfficeProvinceId");

            migrationBuilder.RenameColumn(
                name: "PermanentDistrictId",
                schema: "org",
                table: "CompanyOwner",
                newName: "OfficeDistrictId");

            migrationBuilder.RenameColumn(
                name: "PermanentVillage",
                schema: "org",
                table: "CompanyOwner",
                newName: "OfficeVillage");

            // Rename existing "Current" address columns to "Personal" address
            migrationBuilder.RenameColumn(
                name: "CurrentProvinceId",
                schema: "org",
                table: "CompanyOwner",
                newName: "PersonalProvinceId");

            migrationBuilder.RenameColumn(
                name: "CurrentDistrictId",
                schema: "org",
                table: "CompanyOwner",
                newName: "PersonalDistrictId");

            migrationBuilder.RenameColumn(
                name: "CurrentVillage",
                schema: "org",
                table: "CompanyOwner",
                newName: "PersonalVillage");

            // Update address history records to use new address type names
            migrationBuilder.Sql(@"
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Office' 
                WHERE ""AddressType"" = 'Permanent';
                
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Personal' 
                WHERE ""AddressType"" = 'Current';
            ");

            // Update the LicenseView to use new column names
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
                    op.""Dari"" AS ""OfficeProvinceName"",
                    od.""Dari"" AS ""OfficeDistrictName"",
                    co.""OfficeVillage"",
                    pp.""Dari"" AS ""PersonalProvinceName"",
                    pd.""Dari"" AS ""PersonalDistrictName"",
                    co.""PersonalVillage""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                LEFT JOIN look.""Location"" op ON co.""OfficeProvinceId"" = op.""ID""
                LEFT JOIN look.""Location"" od ON co.""OfficeDistrictId"" = od.""ID""
                LEFT JOIN look.""Location"" pp ON co.""PersonalProvinceId"" = pp.""ID""
                LEFT JOIN look.""Location"" pd ON co.""PersonalDistrictId"" = pd.""ID"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rename "Office" address columns back to "Permanent"
            migrationBuilder.RenameColumn(
                name: "OfficeProvinceId",
                schema: "org",
                table: "CompanyOwner",
                newName: "PermanentProvinceId");

            migrationBuilder.RenameColumn(
                name: "OfficeDistrictId",
                schema: "org",
                table: "CompanyOwner",
                newName: "PermanentDistrictId");

            migrationBuilder.RenameColumn(
                name: "OfficeVillage",
                schema: "org",
                table: "CompanyOwner",
                newName: "PermanentVillage");

            // Rename "Personal" address columns back to "Current"
            migrationBuilder.RenameColumn(
                name: "PersonalProvinceId",
                schema: "org",
                table: "CompanyOwner",
                newName: "CurrentProvinceId");

            migrationBuilder.RenameColumn(
                name: "PersonalDistrictId",
                schema: "org",
                table: "CompanyOwner",
                newName: "CurrentDistrictId");

            migrationBuilder.RenameColumn(
                name: "PersonalVillage",
                schema: "org",
                table: "CompanyOwner",
                newName: "CurrentVillage");

            // Revert address history records
            migrationBuilder.Sql(@"
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Permanent' 
                WHERE ""AddressType"" = 'Office';
                
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Current' 
                WHERE ""AddressType"" = 'Personal';
            ");

            // Restore the original LicenseView
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
                    cp.""Dari"" AS ""CurrentProvinceName"",
                    cdi.""Dari"" AS ""CurrentDistrictName"",
                    co.""CurrentVillage""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                LEFT JOIN look.""Location"" pp ON co.""PermanentProvinceId"" = pp.""ID""
                LEFT JOIN look.""Location"" pd ON co.""PermanentDistrictId"" = pd.""ID""
                LEFT JOIN look.""Location"" cp ON co.""CurrentProvinceId"" = cp.""ID""
                LEFT JOIN look.""Location"" cdi ON co.""CurrentDistrictId"" = cdi.""ID"";
            ");
        }
    }
}
