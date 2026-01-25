using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Add ProvinceId to LicenseDetails table to support province-specific license numbering
    /// Format: PROVINCE_CODE-SEQUENTIAL_NUMBER (e.g., KBL-0001, KHR-0234)
    /// </summary>
    public partial class AddProvinceToLicenseDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ProvinceId column to LicenseDetails
            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                schema: "org",
                table: "LicenseDetails",
                type: "integer",
                nullable: true);

            // Add foreign key constraint
            migrationBuilder.CreateIndex(
                name: "IX_LicenseDetails_ProvinceId",
                schema: "org",
                table: "LicenseDetails",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseDetails_Location_ProvinceId",
                schema: "org",
                table: "LicenseDetails",
                column: "ProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            // Add index on LicenseNumber for faster lookups
            migrationBuilder.CreateIndex(
                name: "IX_LicenseDetails_LicenseNumber",
                schema: "org",
                table: "LicenseDetails",
                column: "LicenseNumber");

            // Add composite index for province-based queries
            migrationBuilder.CreateIndex(
                name: "IX_LicenseDetails_ProvinceId_LicenseNumber",
                schema: "org",
                table: "LicenseDetails",
                columns: new[] { "ProvinceId", "LicenseNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_LicenseDetails_ProvinceId_LicenseNumber",
                schema: "org",
                table: "LicenseDetails");

            migrationBuilder.DropIndex(
                name: "IX_LicenseDetails_LicenseNumber",
                schema: "org",
                table: "LicenseDetails");

            migrationBuilder.DropIndex(
                name: "IX_LicenseDetails_ProvinceId",
                schema: "org",
                table: "LicenseDetails");

            // Drop foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseDetails_Location_ProvinceId",
                schema: "org",
                table: "LicenseDetails");

            // Drop column
            migrationBuilder.DropColumn(
                name: "ProvinceId",
                schema: "org",
                table: "LicenseDetails");
        }
    }
}
