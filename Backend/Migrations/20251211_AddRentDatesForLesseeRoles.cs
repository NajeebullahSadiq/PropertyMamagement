using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRentDatesForLesseeRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RentStartDate",
                schema: "tr",
                table: "BuyerDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentEndDate",
                schema: "tr",
                table: "BuyerDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentStartDate",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RentEndDate",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentEndDate",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.DropColumn(
                name: "RentStartDate",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.DropColumn(
                name: "RentEndDate",
                schema: "tr",
                table: "VehiclesBuyerDetails");

            migrationBuilder.DropColumn(
                name: "RentStartDate",
                schema: "tr",
                table: "VehiclesBuyerDetails");
        }
    }
}
