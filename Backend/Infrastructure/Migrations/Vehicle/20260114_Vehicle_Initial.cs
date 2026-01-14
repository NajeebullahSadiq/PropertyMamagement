using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Vehicle
{
    /// <summary>
    /// Initial migration for Vehicle Transaction tables.
    /// Schema: tr
    /// Dependencies: Shared (look schema), Company (org schema)
    /// </summary>
    public partial class Vehicle_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // VehiclesPropertyDetails - Main vehicle transaction table
            migrationBuilder.CreateTable(
                name: "VehiclesPropertyDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: true),
                    VehicleType = table.Column<string>(type: "text", nullable: true),
                    VehicleBrand = table.Column<string>(type: "text", nullable: true),
                    VehicleModel = table.Column<string>(type: "text", nullable: true),
                    VehicleColor = table.Column<string>(type: "text", nullable: true),
                    VehicleYear = table.Column<int>(type: "integer", nullable: true),
                    PlateNumber = table.Column<string>(type: "text", nullable: true),
                    ChassisNumber = table.Column<string>(type: "text", nullable: true),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    LicenseNumber = table.Column<string>(type: "text", nullable: true),
                    Hand = table.Column<string>(type: "text", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    iscomplete = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "false"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesPropertyDetails_pkey", x => x.Id);
                });

            // VehiclesBuyerDetails
            migrationBuilder.CreateTable(
                name: "VehiclesBuyerDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: true),
                    GrandFatherName = table.Column<string>(type: "text", nullable: true),
                    TazkiraNumber = table.Column<string>(type: "text", nullable: true),
                    TazkiraPage = table.Column<string>(type: "text", nullable: true),
                    TazkiraVolume = table.Column<string>(type: "text", nullable: true),
                    TazkiraRegNumber = table.Column<string>(type: "text", nullable: true),
                    NationalIdNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    VehiclesPropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    RoleType = table.Column<string>(type: "text", nullable: true),
                    RentStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RentEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesBuyerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesBuyerDetails_VehiclesPropertyDetailsId_fkey",
                        column: x => x.VehiclesPropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                });

            // VehiclesSellerDetails
            migrationBuilder.CreateTable(
                name: "VehiclesSellerDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: true),
                    GrandFatherName = table.Column<string>(type: "text", nullable: true),
                    TazkiraNumber = table.Column<string>(type: "text", nullable: true),
                    TazkiraPage = table.Column<string>(type: "text", nullable: true),
                    TazkiraVolume = table.Column<string>(type: "text", nullable: true),
                    TazkiraRegNumber = table.Column<string>(type: "text", nullable: true),
                    NationalIdNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    VehiclesPropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    RoleType = table.Column<string>(type: "text", nullable: true),
                    AuthorizationLetterNumber = table.Column<string>(type: "text", nullable: true),
                    AuthorizationLetterDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesSellerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesSellerDetails_VehiclesPropertyDetailsId_fkey",
                        column: x => x.VehiclesPropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                });

            // VehiclesWitnessDetails
            migrationBuilder.CreateTable(
                name: "VehiclesWitnessDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: true),
                    TazkiraNumber = table.Column<string>(type: "text", nullable: true),
                    TazkiraPage = table.Column<string>(type: "text", nullable: true),
                    TazkiraVolume = table.Column<string>(type: "text", nullable: true),
                    TazkiraRegNumber = table.Column<string>(type: "text", nullable: true),
                    NationalIdNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    VehiclesPropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("VehiclesWitnessDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "VehiclesWitnessDetails_VehiclesPropertyDetailsId_fkey",
                        column: x => x.VehiclesPropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "VehiclesPropertyDetails",
                        principalColumn: "Id");
                });

            // Create indexes
            migrationBuilder.CreateIndex(name: "IX_VehiclesBuyerDetails_VehiclesPropertyDetailsId", schema: "tr", table: "VehiclesBuyerDetails", column: "VehiclesPropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_VehiclesSellerDetails_VehiclesPropertyDetailsId", schema: "tr", table: "VehiclesSellerDetails", column: "VehiclesPropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_VehiclesWitnessDetails_VehiclesPropertyDetailsId", schema: "tr", table: "VehiclesWitnessDetails", column: "VehiclesPropertyDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "VehiclesWitnessDetails", schema: "tr");
            migrationBuilder.DropTable(name: "VehiclesSellerDetails", schema: "tr");
            migrationBuilder.DropTable(name: "VehiclesBuyerDetails", schema: "tr");
            migrationBuilder.DropTable(name: "VehiclesPropertyDetails", schema: "tr");
        }
    }
}
