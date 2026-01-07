using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneAndWhatsAppToCompanyOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add PhoneNumber column to CompanyOwner table
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // Add WhatsAppNumber column to CompanyOwner table
            migrationBuilder.AddColumn<string>(
                name: "WhatsAppNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "WhatsAppNumber",
                schema: "org",
                table: "CompanyOwner");
        }
    }
}
