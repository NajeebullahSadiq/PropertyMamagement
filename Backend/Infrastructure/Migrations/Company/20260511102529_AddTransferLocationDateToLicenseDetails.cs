using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <inheritdoc />
    public partial class AddTransferLocationDateToLicenseDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "TransferLocationDate",
                schema: "org",
                table: "LicenseDetails",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferLocationDate",
                schema: "org",
                table: "LicenseDetails");
        }
    }
}
