using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOwnerAddressHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the CompanyOwnerAddressHistory table
            migrationBuilder.CreateTable(
                name: "CompanyOwnerAddressHistory",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyOwnerId = table.Column<int>(type: "integer", nullable: false),
                    ProvinceId = table.Column<int>(type: "integer", nullable: true),
                    DistrictId = table.Column<int>(type: "integer", nullable: true),
                    Village = table.Column<string>(type: "text", nullable: true),
                    AddressType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyOwnerAddressHistory_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "CompanyOwnerAddressHistory_CompanyOwnerId_fkey",
                        column: x => x.CompanyOwnerId,
                        principalSchema: "org",
                        principalTable: "CompanyOwner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "CompanyOwnerAddressHistory_ProvinceId_fkey",
                        column: x => x.ProvinceId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "CompanyOwnerAddressHistory_DistrictId_fkey",
                        column: x => x.DistrictId,
                        principalSchema: "look",
                        principalTable: "Location",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddressHistory_CompanyOwnerId",
                schema: "org",
                table: "CompanyOwnerAddressHistory",
                column: "CompanyOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddressHistory_ProvinceId",
                schema: "org",
                table: "CompanyOwnerAddressHistory",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddressHistory_DistrictId",
                schema: "org",
                table: "CompanyOwnerAddressHistory",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddressHistory_AddressType",
                schema: "org",
                table: "CompanyOwnerAddressHistory",
                column: "AddressType");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyOwnerAddressHistory_IsActive",
                schema: "org",
                table: "CompanyOwnerAddressHistory",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyOwnerAddressHistory",
                schema: "org");
        }
    }
}
