using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionTypeToBuyerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                schema: "tr",
                table: "BuyerDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionTypeDescription",
                schema: "tr",
                table: "BuyerDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionTypeDescription",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionTypeDescription",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.DropColumn(
                name: "TransactionTypeDescription",
                schema: "tr",
                table: "VehiclesBuyerDetails");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                schema: "tr",
                table: "VehiclesBuyerDetails");
        }
    }
}
