using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    /// <summary>
    /// Initial migration for Petition Writer License module (ثبت جواز عریضه‌نویسان)
    /// </summary>
    public partial class PetitionWriterLicense_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure org schema exists
            migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS org;");

            // Create PetitionWriterLicenses table
            migrationBuilder.CreateTable(
                name: "PetitionWriterLicenses",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApplicantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApplicantFatherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ApplicantGrandFatherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IdentityCardType = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ElectronicIdNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaperIdNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaperIdVolume = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaperIdPage = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaperIdRegNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PermanentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PermanentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PermanentVillage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CurrentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    CurrentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    CurrentVillage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActivityLocation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BankReceiptNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankReceiptDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LicenseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LicenseIssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LicenseExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LicenseStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CancellationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PetitionWriterLicenses_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PetitionWriterLicenses_PermanentProvinceId_fkey",
                        column: x => x.PermanentProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "PetitionWriterLicenses_PermanentDistrictId_fkey",
                        column: x => x.PermanentDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "PetitionWriterLicenses_CurrentProvinceId_fkey",
                        column: x => x.CurrentProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "PetitionWriterLicenses_CurrentDistrictId_fkey",
                        column: x => x.CurrentDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            // Create PetitionWriterRelocations table
            migrationBuilder.CreateTable(
                name: "PetitionWriterRelocations",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetitionWriterLicenseId = table.Column<int>(type: "integer", nullable: false),
                    NewActivityLocation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RelocationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PetitionWriterRelocations_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PetitionWriterRelocations_PetitionWriterLicenseId_fkey",
                        column: x => x.PetitionWriterLicenseId,
                        principalSchema: "org",
                        principalTable: "PetitionWriterLicenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for PetitionWriterLicenses
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_LicenseNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_ApplicantName",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "ApplicantName");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_LicenseStatus",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "LicenseStatus");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_Status",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_PermanentProvinceId",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "PermanentProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterLicenses_CurrentProvinceId",
                schema: "org",
                table: "PetitionWriterLicenses",
                column: "CurrentProvinceId");

            // Create indexes for PetitionWriterRelocations
            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterRelocations_PetitionWriterLicenseId",
                schema: "org",
                table: "PetitionWriterRelocations",
                column: "PetitionWriterLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterRelocations_RelocationDate",
                schema: "org",
                table: "PetitionWriterRelocations",
                column: "RelocationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetitionWriterRelocations",
                schema: "org");

            migrationBuilder.DropTable(
                name: "PetitionWriterLicenses",
                schema: "org");
        }
    }
}
