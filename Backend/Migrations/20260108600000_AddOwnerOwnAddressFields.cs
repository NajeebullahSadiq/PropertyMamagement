using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerOwnAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Owner's Own Address columns to CompanyOwner table
            migrationBuilder.AddColumn<int>(
                name: "OwnerProvinceId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerDistrictId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerVillage",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_OwnerProvinceId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "OwnerProvinceId",
                principalSchema: "org",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_OwnerDistrictId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "OwnerDistrictId",
                principalSchema: "org",
                principalTable: "Location",
                principalColumn: "Id");

            // Create indexes for better query performance
            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_OwnerProvinceId",
                schema: "org",
                table: "CompanyOwner",
                column: "OwnerProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwner_OwnerDistrictId",
                schema: "org",
                table: "CompanyOwner",
                column: "OwnerDistrictId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_OwnerProvinceId_fkey",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_OwnerDistrictId_fkey",
                schema: "org",
                table: "CompanyOwner");

            // Remove indexes
            migrationBuilder.DropIndex(
                name: "IX_CompanyOwner_OwnerProvinceId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropIndex(
                name: "IX_CompanyOwner_OwnerDistrictId",
                schema: "org",
                table: "CompanyOwner");

            // Remove columns
            migrationBuilder.DropColumn(
                name: "OwnerProvinceId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "OwnerDistrictId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "OwnerVillage",
                schema: "org",
                table: "CompanyOwner");
        }
    }
}
