using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.LicenseApplication
{
    /// <summary>
    /// Initial migration for License Application tables.
    /// Schema: org
    /// Module: ثبت درخواست متقاضیان جواز رهنمای معاملات
    /// Includes: LicenseApplications, LicenseApplicationGuarantors, LicenseApplicationWithdrawals
    /// </summary>
    public partial class LicenseApplication_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LicenseApplications - ثبت درخواست متقاضیان جواز رهنمای معاملات
            migrationBuilder.CreateTable(
                name: "LicenseApplications",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RequestSerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApplicantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProposedGuideName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PermanentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PermanentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PermanentVillage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CurrentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    CurrentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    CurrentVillage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsWithdrawn = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LicenseApplications_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "LicenseApplications_PermanentProvinceId_fkey",
                        column: x => x.PermanentProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "LicenseApplications_PermanentDistrictId_fkey",
                        column: x => x.PermanentDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "LicenseApplications_CurrentProvinceId_fkey",
                        column: x => x.CurrentProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "LicenseApplications_CurrentDistrictId_fkey",
                        column: x => x.CurrentDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            // Indexes for LicenseApplications
            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_RequestSerialNumber",
                schema: "org",
                table: "LicenseApplications",
                column: "RequestSerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_ApplicantName",
                schema: "org",
                table: "LicenseApplications",
                column: "ApplicantName");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_ProposedGuideName",
                schema: "org",
                table: "LicenseApplications",
                column: "ProposedGuideName");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_PermanentProvinceId",
                schema: "org",
                table: "LicenseApplications",
                column: "PermanentProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_CurrentProvinceId",
                schema: "org",
                table: "LicenseApplications",
                column: "CurrentProvinceId");

            // LicenseApplicationGuarantors - تضمین‌کنندگان
            migrationBuilder.CreateTable(
                name: "LicenseApplicationGuarantors",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseApplicationId = table.Column<int>(type: "integer", nullable: false),
                    GuarantorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GuarantorFatherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GuaranteeTypeId = table.Column<int>(type: "integer", nullable: false),
                    CashAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ShariaDeedNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ShariaDeedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CustomaryDeedSerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PermanentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    PermanentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    PermanentVillage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CurrentProvinceId = table.Column<int>(type: "integer", nullable: true),
                    CurrentDistrictId = table.Column<int>(type: "integer", nullable: true),
                    CurrentVillage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LicenseApplicationGuarantors_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "LicenseApplicationGuarantors_LicenseApplicationId_fkey",
                        column: x => x.LicenseApplicationId,
                        principalSchema: "org",
                        principalTable: "LicenseApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "LicenseApplicationGuarantors_GuaranteeTypeId_fkey",
                        column: x => x.GuaranteeTypeId,
                        principalSchema: "look",
                        principalTable: "GuaranteeType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "LicenseApplicationGuarantors_PermanentProvinceId_fkey",
                        column: x => x.PermanentProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "LicenseApplicationGuarantors_PermanentDistrictId_fkey",
                        column: x => x.PermanentDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "LicenseApplicationGuarantors_CurrentProvinceId_fkey",
                        column: x => x.CurrentProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "LicenseApplicationGuarantors_CurrentDistrictId_fkey",
                        column: x => x.CurrentDistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID");
                });

            // Indexes for LicenseApplicationGuarantors
            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplicationGuarantors_LicenseApplicationId",
                schema: "org",
                table: "LicenseApplicationGuarantors",
                column: "LicenseApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplicationGuarantors_GuaranteeTypeId",
                schema: "org",
                table: "LicenseApplicationGuarantors",
                column: "GuaranteeTypeId");

            // LicenseApplicationWithdrawals - انصراف
            migrationBuilder.CreateTable(
                name: "LicenseApplicationWithdrawals",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LicenseApplicationId = table.Column<int>(type: "integer", nullable: false),
                    WithdrawalReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    WithdrawalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LicenseApplicationWithdrawals_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "LicenseApplicationWithdrawals_LicenseApplicationId_fkey",
                        column: x => x.LicenseApplicationId,
                        principalSchema: "org",
                        principalTable: "LicenseApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Unique index for one withdrawal per application
            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplicationWithdrawals_LicenseApplicationId",
                schema: "org",
                table: "LicenseApplicationWithdrawals",
                column: "LicenseApplicationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseApplicationWithdrawals",
                schema: "org");

            migrationBuilder.DropTable(
                name: "LicenseApplicationGuarantors",
                schema: "org");

            migrationBuilder.DropTable(
                name: "LicenseApplications",
                schema: "org");
        }
    }
}
