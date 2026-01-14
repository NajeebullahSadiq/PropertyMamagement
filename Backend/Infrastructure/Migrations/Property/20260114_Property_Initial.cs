using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    /// <summary>
    /// Initial migration for Property Transaction tables.
    /// Schema: tr
    /// Dependencies: Shared (look schema), Company (org schema)
    /// </summary>
    public partial class Property_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "tr");

            // PropertyDetails - Main property transaction table
            migrationBuilder.CreateTable(
                name: "PropertyDetails",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: true),
                    PropertyTypeId = table.Column<int>(type: "integer", nullable: true),
                    DocumentType = table.Column<string>(type: "text", nullable: true),
                    IssuanceNumber = table.Column<string>(type: "text", nullable: true),
                    IssuanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PrivateNumber = table.Column<string>(type: "text", nullable: true),
                    East = table.Column<string>(type: "text", nullable: true),
                    West = table.Column<string>(type: "text", nullable: true),
                    North = table.Column<string>(type: "text", nullable: true),
                    South = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<decimal>(type: "numeric", nullable: true),
                    PunitTypeId = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    iscomplete = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "false"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyDetails_pkey", x => x.Id);
                });

            // PropertyAddress
            migrationBuilder.CreateTable(
                name: "PropertyAddress",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    Village = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyAddress_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyAddress_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            // BuyerDetails
            migrationBuilder.CreateTable(
                name: "BuyerDetails",
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
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    photo = table.Column<string>(type: "text", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    RoleType = table.Column<string>(type: "text", nullable: true),
                    TransactionType = table.Column<string>(type: "text", nullable: true),
                    RentStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RentEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaddressProvinceId = table.Column<int>(type: "integer", nullable: true),
                    TaddressDistrictId = table.Column<int>(type: "integer", nullable: true),
                    TaddressVillage = table.Column<string>(type: "text", nullable: true),
                    TaxIdentificationNumber = table.Column<string>(type: "text", nullable: true),
                    AdditionalDetails = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("BuyerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "BuyerDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            // SellerDetails
            migrationBuilder.CreateTable(
                name: "SellerDetails",
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
                    PhoneNumber = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
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
                    TaxIdentificationNumber = table.Column<string>(type: "text", nullable: true),
                    AdditionalDetails = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SellerDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "SellerDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            // WitnessDetails
            migrationBuilder.CreateTable(
                name: "WitnessDetails",
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
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: true),
                    IdentityCardTypeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("WitnessDetails_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "WitnessDetails_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            // PropertyCancellations
            migrationBuilder.CreateTable(
                name: "PropertyCancellations",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyCancellations_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyCancellations_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            // PropertyCancellationDocuments
            migrationBuilder.CreateTable(
                name: "PropertyCancellationDocuments",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyCancellationId = table.Column<int>(type: "integer", nullable: false),
                    DocumentPath = table.Column<string>(type: "text", nullable: true),
                    DocumentName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyCancellationDocuments_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyCancellationDocuments_PropertyCancellationId_fkey",
                        column: x => x.PropertyCancellationId,
                        principalSchema: "tr",
                        principalTable: "PropertyCancellations",
                        principalColumn: "Id");
                });

            // Create indexes
            migrationBuilder.CreateIndex(name: "IX_PropertyAddress_PropertyDetailsId", schema: "tr", table: "PropertyAddress", column: "PropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_BuyerDetails_PropertyDetailsId", schema: "tr", table: "BuyerDetails", column: "PropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_SellerDetails_PropertyDetailsId", schema: "tr", table: "SellerDetails", column: "PropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_WitnessDetails_PropertyDetailsId", schema: "tr", table: "WitnessDetails", column: "PropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_PropertyCancellations_PropertyDetailsId", schema: "tr", table: "PropertyCancellations", column: "PropertyDetailsId");
            migrationBuilder.CreateIndex(name: "IX_PropertyCancellationDocuments_PropertyCancellationId", schema: "tr", table: "PropertyCancellationDocuments", column: "PropertyCancellationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PropertyCancellationDocuments", schema: "tr");
            migrationBuilder.DropTable(name: "PropertyCancellations", schema: "tr");
            migrationBuilder.DropTable(name: "WitnessDetails", schema: "tr");
            migrationBuilder.DropTable(name: "SellerDetails", schema: "tr");
            migrationBuilder.DropTable(name: "BuyerDetails", schema: "tr");
            migrationBuilder.DropTable(name: "PropertyAddress", schema: "tr");
            migrationBuilder.DropTable(name: "PropertyDetails", schema: "tr");
        }
    }
}
