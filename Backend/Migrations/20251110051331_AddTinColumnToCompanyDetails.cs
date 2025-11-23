using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTinColumnToCompanyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "WitnessDetails",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesWitnessDetails",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesSellerDetails",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "SellerDetails",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LicenseNumber",
                schema: "org",
                table: "LicenseDetails",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "log",
                table: "licenseaudit",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "org",
                table: "Guarantors",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TIN",
                schema: "org",
                table: "CompanyDetails",
                type: "double precision",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "BuyerDetails",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "GetPrintType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    doctype = table.Column<string>(type: "text", nullable: false),
                    PNumber = table.Column<int>(type: "integer", nullable: false),
                    PArea = table.Column<int>(type: "integer", nullable: false),
                    NumofRooms = table.Column<int>(type: "integer", nullable: true),
                    North = table.Column<string>(type: "text", nullable: false),
                    South = table.Column<string>(type: "text", nullable: false),
                    West = table.Column<string>(type: "text", nullable: false),
                    East = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    PriceText = table.Column<string>(type: "text", nullable: false),
                    RoyaltyAmount = table.Column<double>(type: "double precision", nullable: false),
                    PropertypeType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Province = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    Village = table.Column<string>(type: "text", nullable: false),
                    SellerFirstName = table.Column<string>(type: "text", nullable: false),
                    SellerFatherName = table.Column<string>(type: "text", nullable: false),
                    SellerIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    SellerVillage = table.Column<string>(type: "text", nullable: false),
                    tSellerVillage = table.Column<string>(type: "text", nullable: false),
                    SellerPhoto = table.Column<string>(type: "text", nullable: false),
                    SellerProvince = table.Column<string>(type: "text", nullable: false),
                    SellerDistrict = table.Column<string>(type: "text", nullable: false),
                    tSellerProvince = table.Column<string>(type: "text", nullable: false),
                    tSellerDistrict = table.Column<string>(type: "text", nullable: false),
                    BuyerFirstName = table.Column<string>(type: "text", nullable: false),
                    BuyerFatherName = table.Column<string>(type: "text", nullable: false),
                    BuyerIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    BuyerVillage = table.Column<string>(type: "text", nullable: false),
                    BuyerPhoto = table.Column<string>(type: "text", nullable: false),
                    BuyerProvince = table.Column<string>(type: "text", nullable: false),
                    BuyerDistrict = table.Column<string>(type: "text", nullable: false),
                    tBuyerProvince = table.Column<string>(type: "text", nullable: false),
                    tBuyerDistrict = table.Column<string>(type: "text", nullable: false),
                    tBuyerVillage = table.Column<string>(type: "text", nullable: false),
                    WitnessOneFirstName = table.Column<string>(type: "text", nullable: false),
                    WitnessOneFatherName = table.Column<string>(type: "text", nullable: false),
                    WitnessOneIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    WitnessTwoFirstName = table.Column<string>(type: "text", nullable: false),
                    WitnessTwoFatherName = table.Column<string>(type: "text", nullable: false),
                    WitnessTwoIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    UnitType = table.Column<string>(type: "text", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GetPrintType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "getVehiclePrintData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PermitNo = table.Column<int>(type: "integer", nullable: false),
                    PilateNo = table.Column<int>(type: "integer", nullable: false),
                    TypeOfVehicle = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    EnginNo = table.Column<int>(type: "integer", nullable: false),
                    ShasiNo = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    PriceText = table.Column<string>(type: "text", nullable: false),
                    RoyaltyAmount = table.Column<double>(type: "double precision", nullable: false),
                    SellerFirstName = table.Column<string>(type: "text", nullable: false),
                    SellerFatherName = table.Column<string>(type: "text", nullable: false),
                    SellerIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    SellerVillage = table.Column<string>(type: "text", nullable: false),
                    tSellerVillage = table.Column<string>(type: "text", nullable: false),
                    SellerPhoto = table.Column<string>(type: "text", nullable: false),
                    SellerProvince = table.Column<string>(type: "text", nullable: false),
                    SellerDistrict = table.Column<string>(type: "text", nullable: false),
                    tSellerProvince = table.Column<string>(type: "text", nullable: false),
                    tSellerDistrict = table.Column<string>(type: "text", nullable: false),
                    BuyerFirstName = table.Column<string>(type: "text", nullable: false),
                    BuyerFatherName = table.Column<string>(type: "text", nullable: false),
                    BuyerIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    BuyerVillage = table.Column<string>(type: "text", nullable: false),
                    BuyerProvince = table.Column<string>(type: "text", nullable: false),
                    BuyerDistrict = table.Column<string>(type: "text", nullable: false),
                    tBuyerProvince = table.Column<string>(type: "text", nullable: false),
                    tBuyerDistrict = table.Column<string>(type: "text", nullable: false),
                    tBuyerVillage = table.Column<string>(type: "text", nullable: false),
                    BuyerPhoto = table.Column<string>(type: "text", nullable: false),
                    WitnessOneFirstName = table.Column<string>(type: "text", nullable: false),
                    WitnessOneFatherName = table.Column<string>(type: "text", nullable: false),
                    WitnessOneIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    WitnessTwoFirstName = table.Column<string>(type: "text", nullable: false),
                    WitnessTwoFatherName = table.Column<string>(type: "text", nullable: false),
                    WitnessTwoIndentityCardNumber = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_getVehiclePrintData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfileWithCompany",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PhotoPath = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileWithCompany", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GetPrintType");

            migrationBuilder.DropTable(
                name: "getVehiclePrintData");

            migrationBuilder.DropTable(
                name: "UserProfileWithCompany");

            migrationBuilder.DropColumn(
                name: "TIN",
                schema: "org",
                table: "CompanyDetails");

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "WitnessDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesWitnessDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesSellerDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesBuyerDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "SellerDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LicenseNumber",
                schema: "org",
                table: "LicenseDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "log",
                table: "licenseaudit",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "BuyerDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
