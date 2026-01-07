using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class RenameAddressToPermanentTemporary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename "Office" address columns to "Permanent" address
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

            // Rename "Personal" address columns to "Temporary" address
            migrationBuilder.RenameColumn(
                name: "PersonalProvinceId",
                schema: "org",
                table: "CompanyOwner",
                newName: "TemporaryProvinceId");

            migrationBuilder.RenameColumn(
                name: "PersonalDistrictId",
                schema: "org",
                table: "CompanyOwner",
                newName: "TemporaryDistrictId");

            migrationBuilder.RenameColumn(
                name: "PersonalVillage",
                schema: "org",
                table: "CompanyOwner",
                newName: "TemporaryVillage");

            // Update address history records to use new address type names
            migrationBuilder.Sql(@"
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Permanent' 
                WHERE ""AddressType"" = 'Office';
                
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Temporary' 
                WHERE ""AddressType"" = 'Personal';
            ");

            // Update the LicenseView to use Permanent/Temporary column names
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rename "Permanent" address columns back to "Office"
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

            // Rename "Temporary" address columns back to "Personal"
            migrationBuilder.RenameColumn(
                name: "TemporaryProvinceId",
                schema: "org",
                table: "CompanyOwner",
                newName: "PersonalProvinceId");

            migrationBuilder.RenameColumn(
                name: "TemporaryDistrictId",
                schema: "org",
                table: "CompanyOwner",
                newName: "PersonalDistrictId");

            migrationBuilder.RenameColumn(
                name: "TemporaryVillage",
                schema: "org",
                table: "CompanyOwner",
                newName: "PersonalVillage");

            // Revert address history records
            migrationBuilder.Sql(@"
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Office' 
                WHERE ""AddressType"" = 'Permanent';
                
                UPDATE org.""CompanyOwnerAddressHistory"" 
                SET ""AddressType"" = 'Personal' 
                WHERE ""AddressType"" = 'Temporary';
            ");

            // Restore the Office/Personal LicenseView
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
    }
}
