using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPetitionWriterSecurities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            // Create unique index on RegistrationNumber
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterSecurities_RegistrationNumber",
                schema: "org",
                table: "PetitionWriterSecurities",
                column: "RegistrationNumber",
                unique: true);

            // Create index on LicenseNumber for search
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterSecurities_LicenseNumber",
                schema: "org",
                table: "PetitionWriterSecurities",
                column: "LicenseNumber");

            // Create index on BankReceiptNumber for search
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterSecurities_BankReceiptNumber",
                schema: "org",
                table: "PetitionWriterSecurities",
                column: "BankReceiptNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetitionWriterSecurities",
                schema: "org");
        }
    }
}
