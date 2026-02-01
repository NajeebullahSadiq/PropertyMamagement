using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Add ProvinceId to CompanyDetails table to support province-based access control
    /// </summary>
    public partial class AddProvinceToCompanyDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ProvinceId column to CompanyDetails
            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                schema: "org",
                table: "CompanyDetails",
                type: "integer",
                nullable: true);

            // Add foreign key constraint to Location table (provinces)
            migrationBuilder.CreateIndex(
                name: "IX_CompanyDetails_ProvinceId",
                schema: "org",
                table: "CompanyDetails",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDetails_Location_ProvinceId",
                schema: "org",
                table: "CompanyDetails",
                column: "ProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDetails_Location_ProvinceId",
                schema: "org",
                table: "CompanyDetails");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_CompanyDetails_ProvinceId",
                schema: "org",
                table: "CompanyDetails");

            // Drop column
            migrationBuilder.DropColumn(
                name: "ProvinceId",
                schema: "org",
                table: "CompanyDetails");
        }
    }
}
