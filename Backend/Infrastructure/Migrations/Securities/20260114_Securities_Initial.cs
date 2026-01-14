using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Securities
{
    /// <summary>
    /// Initial migration for Securities tables.
    /// Schema: org
    /// Dependencies: Company (org schema)
    /// Includes: SecuritiesDistribution, PetitionWriterSecurities, SecuritiesControl
    /// </summary>
    public partial class Securities_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SecuritiesDistribution - اسناد بهادار رهنمای معاملات
            migrationBuilder.CreateTable(
                name: "SecuritiesDistribution",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LicenseOwnerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LicenseOwnerFatherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TransactionGuideName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<int>(type: "integer", nullable: true),
                    PropertySubType = table.Column<int>(type: "integer", nullable: true),
                    VehicleSubType = table.Column<int>(type: "integer", nullable: true),
                    PropertySaleCount = table.Column<int>(type: "integer", nullable: true),
                    PropertySaleSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PropertySaleSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BayWafaCount = table.Column<int>(type: "integer", nullable: true),
                    BayWafaSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BayWafaSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RentCount = table.Column<int>(type: "integer", nullable: true),
                    RentSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RentSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleSaleCount = table.Column<int>(type: "integer", nullable: true),
                    VehicleSaleSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleSaleSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleExchangeCount = table.Column<int>(type: "integer", nullable: true),
                    VehicleExchangeSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleExchangeSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegistrationBookType = table.Column<int>(type: "integer", nullable: true),
                    RegistrationBookCount = table.Column<int>(type: "integer", nullable: true),
                    DuplicateBookCount = table.Column<int>(type: "integer", nullable: true),
                    PricePerDocument = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalDocumentsPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    RegistrationBookPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalSecuritiesPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    BankReceiptNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DistributionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SecuritiesDistribution_pkey", x => x.Id);
                });

            // PetitionWriterSecurities - اسناد بهادار عریضه نویسان
            migrationBuilder.CreateTable(
                name: "PetitionWriterSecurities",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PetitionWriterName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PetitionWriterFatherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PetitionCount = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BankReceiptNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNumberStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNumberEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DistributionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PetitionWriterSecurities_pkey", x => x.Id);
                });

            // SecuritiesControl - کنترول اسناد بهادار
            migrationBuilder.CreateTable(
                name: "SecuritiesControl",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecurityDocumentType = table.Column<int>(type: "integer", nullable: false),
                    ProposalNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProposalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DistributionTicketNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SecuritiesType = table.Column<int>(type: "integer", nullable: true),
                    PropertySaleCount = table.Column<int>(type: "integer", nullable: true),
                    PropertySaleSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PropertySaleSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BayWafaCount = table.Column<int>(type: "integer", nullable: true),
                    BayWafaSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BayWafaSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RentCount = table.Column<int>(type: "integer", nullable: true),
                    RentSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RentSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleSaleCount = table.Column<int>(type: "integer", nullable: true),
                    VehicleSaleSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VehicleSaleSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExchangeCount = table.Column<int>(type: "integer", nullable: true),
                    ExchangeSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExchangeSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegistrationBookCount = table.Column<int>(type: "integer", nullable: true),
                    RegistrationBookSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegistrationBookSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrintedPetitionCount = table.Column<int>(type: "integer", nullable: true),
                    PrintedPetitionSerialStart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrintedPetitionSerialEnd = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DistributionStartNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DistributionEndNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DistributedPersonsCount = table.Column<int>(type: "integer", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SecuritiesControl_pkey", x => x.Id);
                });

            // Create indexes for SecuritiesDistribution
            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesDistribution_RegistrationNumber",
                schema: "org",
                table: "SecuritiesDistribution",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesDistribution_LicenseNumber",
                schema: "org",
                table: "SecuritiesDistribution",
                column: "LicenseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesDistribution_BankReceiptNumber",
                schema: "org",
                table: "SecuritiesDistribution",
                column: "BankReceiptNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesDistribution_TransactionGuideName",
                schema: "org",
                table: "SecuritiesDistribution",
                column: "TransactionGuideName");

            // Create indexes for PetitionWriterSecurities
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterSecurities_RegistrationNumber",
                schema: "org",
                table: "PetitionWriterSecurities",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterSecurities_LicenseNumber",
                schema: "org",
                table: "PetitionWriterSecurities",
                column: "LicenseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterSecurities_BankReceiptNumber",
                schema: "org",
                table: "PetitionWriterSecurities",
                column: "BankReceiptNumber");

            // Create index for SecuritiesControl
            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesControl_SerialNumber",
                schema: "org",
                table: "SecuritiesControl",
                column: "SerialNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SecuritiesControl", schema: "org");
            migrationBuilder.DropTable(name: "PetitionWriterSecurities", schema: "org");
            migrationBuilder.DropTable(name: "SecuritiesDistribution", schema: "org");
        }
    }
}
