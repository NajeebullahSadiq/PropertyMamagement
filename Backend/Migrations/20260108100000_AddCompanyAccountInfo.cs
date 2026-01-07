using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add CompanyAccountInfo table for storing financial/tax information
    /// </summary>
    public partial class AddCompanyAccountInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyAccountInfo",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    SettlementInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TaxPaymentAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SettlementYear = table.Column<int>(type: "integer", nullable: true),
                    TaxPaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransactionCount = table.Column<int>(type: "integer", nullable: true),
                    CompanyCommission = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyAccountInfo_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAccountInfo_CompanyDetails",
                        column: x => x.CompanyId,
                        principalSchema: "org",
                        principalTable: "CompanyDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAccountInfo_CompanyId",
                schema: "org",
                table: "CompanyAccountInfo",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyAccountInfo",
                schema: "org");
        }
    }
}
