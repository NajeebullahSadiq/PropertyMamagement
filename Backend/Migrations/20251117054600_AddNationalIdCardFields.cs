using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalIdCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalIdCard",
                schema: "tr",
                table: "SellerDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdCard",
                schema: "tr",
                table: "BuyerDetails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalIdCard",
                schema: "tr",
                table: "SellerDetails");

            migrationBuilder.DropColumn(
                name: "NationalIdCard",
                schema: "tr",
                table: "BuyerDetails");
        }
    }
}
