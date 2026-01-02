using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class ImprovePropertyManagementStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Status and Approval fields to PropertyDetails
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                schema: "tr",
                table: "PropertyDetails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                schema: "tr",
                table: "PropertyDetails",
                type: "timestamp without time zone",
                nullable: true);

            // Add Share fields to BuyerDetails
            migrationBuilder.AddColumn<double>(
                name: "SharePercentage",
                schema: "tr",
                table: "BuyerDetails",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ShareAmount",
                schema: "tr",
                table: "BuyerDetails",
                type: "double precision",
                nullable: true);

            // Add Share fields to SellerDetails
            migrationBuilder.AddColumn<double>(
                name: "SharePercentage",
                schema: "tr",
                table: "SellerDetails",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ShareAmount",
                schema: "tr",
                table: "SellerDetails",
                type: "double precision",
                nullable: true);

            // Enhance WitnessDetails with address and relationship
            migrationBuilder.AddColumn<int>(
                name: "PaddressProvinceId",
                schema: "tr",
                table: "WitnessDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaddressDistrictId",
                schema: "tr",
                table: "WitnessDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaddressVillage",
                schema: "tr",
                table: "WitnessDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelationshipToParties",
                schema: "tr",
                table: "WitnessDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WitnessType",
                schema: "tr",
                table: "WitnessDetails",
                type: "text",
                nullable: true);

            // Create PropertyOwnershipHistory table
            migrationBuilder.CreateTable(
                name: "PropertyOwnershipHistory",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    OwnerName = table.Column<string>(type: "text", nullable: false),
                    OwnerFatherName = table.Column<string>(type: "text", nullable: true),
                    OwnershipStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OwnershipEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TransferDocumentPath = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyOwnershipHistory_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyOwnershipHistory_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create PropertyPayment table
            migrationBuilder.CreateTable(
                name: "PropertyPayment",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AmountPaid = table.Column<double>(type: "double precision", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    ReceiptNumber = table.Column<string>(type: "text", nullable: true),
                    BalanceRemaining = table.Column<double>(type: "double precision", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyPayment_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyPayment_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create PropertyValuation table
            migrationBuilder.CreateTable(
                name: "PropertyValuation",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    ValuationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValuationAmount = table.Column<double>(type: "double precision", nullable: false),
                    ValuatorName = table.Column<string>(type: "text", nullable: true),
                    ValuatorOrganization = table.Column<string>(type: "text", nullable: true),
                    ValuationDocumentPath = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyValuation_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyValuation_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create PropertyDocument table
            migrationBuilder.CreateTable(
                name: "PropertyDocument",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    DocumentCategory = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyDocument_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyDocument_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for performance
            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_Pnumber",
                schema: "tr",
                table: "PropertyDetails",
                column: "PNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_Status",
                schema: "tr",
                table: "PropertyDetails",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_CreatedBy",
                schema: "tr",
                table: "PropertyDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwnershipHistory_PropertyDetailsId",
                schema: "tr",
                table: "PropertyOwnershipHistory",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPayment_PropertyDetailsId",
                schema: "tr",
                table: "PropertyPayment",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValuation_PropertyDetailsId",
                schema: "tr",
                table: "PropertyValuation",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDocument_PropertyDetailsId",
                schema: "tr",
                table: "PropertyDocument",
                column: "PropertyDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_WitnessDetails_PaddressProvinceId",
                schema: "tr",
                table: "WitnessDetails",
                column: "PaddressProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_WitnessDetails_PaddressDistrictId",
                schema: "tr",
                table: "WitnessDetails",
                column: "PaddressDistrictId");

            // Add foreign keys for WitnessDetails
            migrationBuilder.AddForeignKey(
                name: "WitnessDetails_PaddressProvinceId_fkey",
                schema: "tr",
                table: "WitnessDetails",
                column: "PaddressProvinceId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "WitnessDetails_PaddressDistrictId_fkey",
                schema: "tr",
                table: "WitnessDetails",
                column: "PaddressDistrictId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "WitnessDetails_PaddressProvinceId_fkey",
                schema: "tr",
                table: "WitnessDetails");

            migrationBuilder.DropForeignKey(
                name: "WitnessDetails_PaddressDistrictId_fkey",
                schema: "tr",
                table: "WitnessDetails");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_PropertyDetails_Pnumber",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropIndex(
                name: "IX_PropertyDetails_Status",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropIndex(
                name: "IX_PropertyDetails_CreatedBy",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropIndex(
                name: "IX_WitnessDetails_PaddressProvinceId",
                schema: "tr",
                table: "WitnessDetails");

            migrationBuilder.DropIndex(
                name: "IX_WitnessDetails_PaddressDistrictId",
                schema: "tr",
                table: "WitnessDetails");

            // Drop tables
            migrationBuilder.DropTable(
                name: "PropertyOwnershipHistory",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "PropertyPayment",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "PropertyValuation",
                schema: "tr");

            migrationBuilder.DropTable(
                name: "PropertyDocument",
                schema: "tr");

            // Drop columns from PropertyDetails
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                schema: "tr",
                table: "PropertyDetails");

            // Drop columns from BuyerDetails
            migrationBuilder.DropColumn(
                name: "SharePercentage",
                schema: "tr",
                table: "BuyerDetails");

            migrationBuilder.DropColumn(
                name: "ShareAmount",
                schema: "tr",
                table: "BuyerDetails");

            // Drop columns from SellerDetails
            migrationBuilder.DropColumn(
                name: "SharePercentage",
                schema: "tr",
                table: "SellerDetails");

            migrationBuilder.DropColumn(
                name: "ShareAmount",
                schema: "tr",
                table: "SellerDetails");

            // Drop columns from WitnessDetails
            migrationBuilder.DropColumn(
                name: "PaddressProvinceId",
                schema: "tr",
                table: "WitnessDetails");

            migrationBuilder.DropColumn(
                name: "PaddressDistrictId",
                schema: "tr",
                table: "WitnessDetails");

            migrationBuilder.DropColumn(
                name: "PaddressVillage",
                schema: "tr",
                table: "WitnessDetails");

            migrationBuilder.DropColumn(
                name: "RelationshipToParties",
                schema: "tr",
                table: "WitnessDetails");

            migrationBuilder.DropColumn(
                name: "WitnessType",
                schema: "tr",
                table: "WitnessDetails");
        }
    }
}
