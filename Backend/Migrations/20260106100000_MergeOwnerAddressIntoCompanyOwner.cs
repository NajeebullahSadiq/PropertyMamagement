using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class MergeOwnerAddressIntoCompanyOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Permanent Address columns to CompanyOwner
            migrationBuilder.AddColumn<int>(
                name: "PermanentProvinceId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermanentDistrictId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentVillage",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);

            // Add Current Address columns to CompanyOwner
            migrationBuilder.AddColumn<int>(
                name: "CurrentProvinceId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentDistrictId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentVillage",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_PermanentProvinceId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "PermanentProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_PermanentDistrictId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "PermanentDistrictId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_CurrentProvinceId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "CurrentProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_CurrentDistrictId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "CurrentDistrictId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            // Create indexes for the new foreign key columns
            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_PermanentProvinceId",
                schema: "org",
                table: "CompanyOwner",
                column: "PermanentProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_PermanentDistrictId",
                schema: "org",
                table: "CompanyOwner",
                column: "PermanentDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_CurrentProvinceId",
                schema: "org",
                table: "CompanyOwner",
                column: "CurrentProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_CurrentDistrictId",
                schema: "org",
                table: "CompanyOwner",
                column: "CurrentDistrictId");

            // Migrate existing data from CompanyOwnerAddress to CompanyOwner
            // Migrate Permanent Address (AddressTypeId = 1 or name contains 'دایمی')
            migrationBuilder.Sql(@"
                UPDATE org.""CompanyOwner"" co
                SET 
                    ""PermanentProvinceId"" = coa.""ProvinceId"",
                    ""PermanentDistrictId"" = coa.""DistrictId"",
                    ""PermanentVillage"" = coa.""Village""
                FROM org.""CompanyOwnerAddress"" coa
                INNER JOIN look.""AddressType"" at ON coa.""AddressTypeId"" = at.""Id""
                WHERE coa.""CompanyOwnerId"" = co.""Id""
                AND (at.""Name"" LIKE '%دایمی%' OR at.""Name"" ILIKE '%permanent%' OR at.""Id"" = 1);
            ");

            // Migrate Current Address (AddressTypeId = 2 or name contains 'فعلی')
            migrationBuilder.Sql(@"
                UPDATE org.""CompanyOwner"" co
                SET 
                    ""CurrentProvinceId"" = coa.""ProvinceId"",
                    ""CurrentDistrictId"" = coa.""DistrictId"",
                    ""CurrentVillage"" = coa.""Village""
                FROM org.""CompanyOwnerAddress"" coa
                INNER JOIN look.""AddressType"" at ON coa.""AddressTypeId"" = at.""Id""
                WHERE coa.""CompanyOwnerId"" = co.""Id""
                AND (at.""Name"" LIKE '%فعلی%' OR at.""Name"" ILIKE '%current%' OR at.""Id"" = 2);
            ");

            // Update the LicenseView to include owner address information
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS public.""LicenseView"";
                
                CREATE OR REPLACE VIEW public.""LicenseView"" AS
                SELECT 
                    cd.""Id"" AS ""CompanyId"",
                    cd.""PhoneNumber"",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the original LicenseView without address fields
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS public.""LicenseView"";
                
                CREATE OR REPLACE VIEW public.""LicenseView"" AS
                SELECT 
                    cd.""Id"" AS ""CompanyId"",
                    cd.""PhoneNumber"",
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
                    ld.""ExpireDate""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId"";
            ");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_CompanyOwner_CurrentDistrictId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropIndex(
                name: "IX_CompanyOwner_CurrentProvinceId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropIndex(
                name: "IX_CompanyOwner_PermanentDistrictId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropIndex(
                name: "IX_CompanyOwner_PermanentProvinceId",
                schema: "org",
                table: "CompanyOwner");

            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_CurrentDistrictId_fkey",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_CurrentProvinceId_fkey",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_PermanentDistrictId_fkey",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_PermanentProvinceId_fkey",
                schema: "org",
                table: "CompanyOwner");

            // Drop columns
            migrationBuilder.DropColumn(
                name: "CurrentVillage",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "CurrentDistrictId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "CurrentProvinceId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "PermanentVillage",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "PermanentDistrictId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "PermanentProvinceId",
                schema: "org",
                table: "CompanyOwner");
        }
    }
}
