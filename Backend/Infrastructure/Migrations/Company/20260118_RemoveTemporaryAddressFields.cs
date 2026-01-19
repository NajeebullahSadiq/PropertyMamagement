using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Remove temporary address fields from CompanyOwner table
    /// </summary>
    public partial class RemoveTemporaryAddressFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporaryProvinceId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "TemporaryDistrictId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "TemporaryVillage",
                schema: "org",
                table: "CompanyOwner");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemporaryProvinceId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemporaryDistrictId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryVillage",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);
        }
    }
}
