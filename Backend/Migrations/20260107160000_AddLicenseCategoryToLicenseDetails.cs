using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add LicenseCategory (نوعیت جواز) column to LicenseDetails table.
    /// This field stores the category of license: جدید (New), تجدید (Renewal), مثنی (Duplicate)
    /// </summary>
    public partial class AddLicenseCategoryToLicenseDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicenseCategory",
                schema: "org",
                table: "LicenseDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Add a comment to document the allowed values
            migrationBuilder.Sql(@"
                COMMENT ON COLUMN org.""LicenseDetails"".""LicenseCategory"" IS 'License Category (نوعیت جواز): جدید (New), تجدید (Renewal), مثنی (Duplicate)';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseCategory",
                schema: "org",
                table: "LicenseDetails");
        }
    }
}
