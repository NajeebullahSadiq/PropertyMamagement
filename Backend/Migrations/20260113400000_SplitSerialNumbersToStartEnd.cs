using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to split single serial number fields into start/end pairs for all document types
    /// </summary>
    public partial class SplitSerialNumbersToStartEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new start/end columns for Property Sale
            migrationBuilder.AddColumn<string>(
                name: "PropertySaleSerialStart",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertySaleSerialEnd",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Add new start/end columns for Bay Wafa
            migrationBuilder.AddColumn<string>(
                name: "BayWafaSerialStart",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BayWafaSerialEnd",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Add new start/end columns for Rent
            migrationBuilder.AddColumn<string>(
                name: "RentSerialStart",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RentSerialEnd",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Add new start/end columns for Vehicle Sale
            migrationBuilder.AddColumn<string>(
                name: "VehicleSaleSerialStart",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleSaleSerialEnd",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Add new start/end columns for Vehicle Exchange
            migrationBuilder.AddColumn<string>(
                name: "VehicleExchangeSerialStart",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleExchangeSerialEnd",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Migrate existing data: copy old serial numbers to start fields
            migrationBuilder.Sql(@"
                UPDATE ""SecuritiesDistributions""
                SET ""PropertySaleSerialStart"" = ""PropertySaleSerialNumber""
                WHERE ""PropertySaleSerialNumber"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""BayWafaSerialStart"" = ""BayWafaSerialNumber""
                WHERE ""BayWafaSerialNumber"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""RentSerialStart"" = ""RentSerialNumber""
                WHERE ""RentSerialNumber"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""VehicleSaleSerialStart"" = ""VehicleSaleSerialNumber""
                WHERE ""VehicleSaleSerialNumber"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""VehicleExchangeSerialStart"" = ""VehicleExchangeSerialNumber""
                WHERE ""VehicleExchangeSerialNumber"" IS NOT NULL;
            ");

            // Drop old columns
            migrationBuilder.DropColumn(
                name: "PropertySaleSerialNumber",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "BayWafaSerialNumber",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "RentSerialNumber",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "VehicleSaleSerialNumber",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "VehicleExchangeSerialNumber",
                table: "SecuritiesDistributions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add old columns
            migrationBuilder.AddColumn<string>(
                name: "PropertySaleSerialNumber",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BayWafaSerialNumber",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RentSerialNumber",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleSaleSerialNumber",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleExchangeSerialNumber",
                table: "SecuritiesDistributions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Migrate data back: copy start fields to old serial number columns
            migrationBuilder.Sql(@"
                UPDATE ""SecuritiesDistributions""
                SET ""PropertySaleSerialNumber"" = ""PropertySaleSerialStart""
                WHERE ""PropertySaleSerialStart"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""BayWafaSerialNumber"" = ""BayWafaSerialStart""
                WHERE ""BayWafaSerialStart"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""RentSerialNumber"" = ""RentSerialStart""
                WHERE ""RentSerialStart"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""VehicleSaleSerialNumber"" = ""VehicleSaleSerialStart""
                WHERE ""VehicleSaleSerialStart"" IS NOT NULL;

                UPDATE ""SecuritiesDistributions""
                SET ""VehicleExchangeSerialNumber"" = ""VehicleExchangeSerialStart""
                WHERE ""VehicleExchangeSerialStart"" IS NOT NULL;
            ");

            // Drop new columns
            migrationBuilder.DropColumn(
                name: "PropertySaleSerialStart",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "PropertySaleSerialEnd",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "BayWafaSerialStart",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "BayWafaSerialEnd",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "RentSerialStart",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "RentSerialEnd",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "VehicleSaleSerialStart",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "VehicleSaleSerialEnd",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "VehicleExchangeSerialStart",
                table: "SecuritiesDistributions");

            migrationBuilder.DropColumn(
                name: "VehicleExchangeSerialEnd",
                table: "SecuritiesDistributions");
        }
    }
}
