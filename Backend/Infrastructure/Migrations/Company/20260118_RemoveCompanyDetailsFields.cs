using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Migration to remove PhoneNumber, LicenseNumber, PetitionDate, and PetitionNumber fields from CompanyDetails table.
    /// These fields are no longer needed in the Company Details section.
    /// </summary>
    public partial class RemoveCompanyDetailsFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the 4 columns from CompanyDetails table
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "org",
                table: "CompanyDetails");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                schema: "org",
                table: "CompanyDetails");

            migrationBuilder.DropColumn(
                name: "PetitionDate",
                schema: "org",
                table: "CompanyDetails");

            migrationBuilder.DropColumn(
                name: "PetitionNumber",
                schema: "org",
                table: "CompanyDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add the columns if migration is rolled back
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "org",
                table: "CompanyDetails",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                schema: "org",
                table: "CompanyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PetitionDate",
                schema: "org",
                table: "CompanyDetails",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetitionNumber",
                schema: "org",
                table: "CompanyDetails",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);
        }
    }
}
