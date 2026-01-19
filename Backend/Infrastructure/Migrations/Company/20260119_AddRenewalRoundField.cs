using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Add RenewalRound field to LicenseDetails table
    /// </summary>
    public partial class AddRenewalRoundField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RenewalRound",
                schema: "org",
                table: "LicenseDetails",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewalRound",
                schema: "org",
                table: "LicenseDetails");
        }
    }
}
