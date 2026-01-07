using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add CompanyCancellationInfo table for storing license cancellation/revocation information
    /// </summary>
    public partial class AddCompanyCancellationInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyCancellationInfo",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    LicenseCancellationLetterNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RevenueCancellationLetterNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LicenseCancellationLetterDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyCancellationInfo_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyCancellationInfo_CompanyDetails",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCancellationInfo_CompanyId",
                schema: "org",
                table: "CompanyCancellationInfo",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyCancellationInfo",
                schema: "org");
        }
    }
}
