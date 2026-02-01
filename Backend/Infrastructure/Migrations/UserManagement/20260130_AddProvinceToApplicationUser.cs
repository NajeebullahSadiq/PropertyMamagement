using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.UserManagement
{
    /// <summary>
    /// Add ProvinceId to AspNetUsers table to support province-based access control for COMPANY_REGISTRAR users
    /// </summary>
    public partial class AddProvinceToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ProvinceId column to AspNetUsers
            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            // Add foreign key constraint to Location table (provinces)
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProvinceId",
                table: "AspNetUsers",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Location_ProvinceId",
                table: "AspNetUsers",
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
                name: "FK_AspNetUsers_Location_ProvinceId",
                table: "AspNetUsers");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProvinceId",
                table: "AspNetUsers");

            // Drop column
            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "AspNetUsers");
        }
    }
}
