using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    /// <inheritdoc />
    public partial class AddPetitionWriterActivityLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PetitionWriterSecurities",
                schema: "sec",
                newName: "PetitionWriterSecurities",
                newSchema: "org");

            migrationBuilder.AddColumn<string>(
                name: "Above",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApartmentNumber",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Below",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateDeedNumber",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DuplicateIssueDate",
                schema: "org",
                table: "LicenseDetails",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractEndDate",
                schema: "tr",
                table: "BuyerDetails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractStartDate",
                schema: "tr",
                table: "BuyerDetails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PetitionWriterActivityLocations",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DariName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PetitionWriterActivityLocations_pkey", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_CompanyId",
                schema: "tr",
                table: "PropertyDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionWriterActivityLocations_DariName",
                schema: "org",
                table: "PetitionWriterActivityLocations",
                column: "DariName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyDetails_CompanyDetails_CompanyId",
                schema: "tr",
                table: "PropertyDetails",
                column: "CompanyId",
                principalSchema: "org",
                principalTable: "CompanyDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyDetails_CompanyDetails_CompanyId",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropTable(
                name: "PetitionWriterActivityLocations",
                schema: "org");

            migrationBuilder.DropIndex(
                name: "IX_PropertyDetails_CompanyId",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "Above",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "Below",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "PrivateDeedNumber",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "DuplicateIssueDate",
                schema: "org",
                table: "LicenseDetails");

            migrationBuilder.DropColumn(
                name: "ContractEndDate",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.DropColumn(
                name: "ContractStartDate",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.EnsureSchema(
                name: "sec");

            migrationBuilder.RenameTable(
                name: "PetitionWriterSecurities",
                schema: "org",
                newName: "PetitionWriterSecurities",
                newSchema: "sec");
        }
    }
}
