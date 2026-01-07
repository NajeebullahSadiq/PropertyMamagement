using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add conditional fields for guarantee types:
    /// - Sharia Deed: CourtName, CollateralNumber
    /// - Customary Deed: SetSerialNumber, GuaranteeDistrictId
    /// - Cash: BankName, DepositNumber, DepositDate
    /// </summary>
    public partial class AddGuaranteeTypeConditionalFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sharia Deed fields
            migrationBuilder.AddColumn<string>(
                name: "CourtName",
                schema: "org",
                table: "Guarantors",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollateralNumber",
                schema: "org",
                table: "Guarantors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Customary Deed fields
            migrationBuilder.AddColumn<string>(
                name: "SetSerialNumber",
                schema: "org",
                table: "Guarantors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuaranteeDistrictId",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true);

            // Cash fields
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                schema: "org",
                table: "Guarantors",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositNumber",
                schema: "org",
                table: "Guarantors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DepositDate",
                schema: "org",
                table: "Guarantors",
                type: "date",
                nullable: true);

            // Add index for GuaranteeDistrictId
            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_GuaranteeDistrictId",
                schema: "org",
                table: "Guarantors",
                column: "GuaranteeDistrictId");

            // Add foreign key for GuaranteeDistrict
            migrationBuilder.AddForeignKey(
                name: "Guarantors_GuaranteeDistrictId_fkey",
                schema: "org",
                table: "Guarantors",
                column: "GuaranteeDistrictId",
                principalSchema: "look",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key
            migrationBuilder.DropForeignKey(
                name: "Guarantors_GuaranteeDistrictId_fkey",
                schema: "org",
                table: "Guarantors");

            // Remove index
            migrationBuilder.DropIndex(
                name: "IX_Guarantors_GuaranteeDistrictId",
                schema: "org",
                table: "Guarantors");

            // Remove Cash fields
            migrationBuilder.DropColumn(
                name: "DepositDate",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "DepositNumber",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "BankName",
                schema: "org",
                table: "Guarantors");

            // Remove Customary Deed fields
            migrationBuilder.DropColumn(
                name: "GuaranteeDistrictId",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "SetSerialNumber",
                schema: "org",
                table: "Guarantors");

            // Remove Sharia Deed fields
            migrationBuilder.DropColumn(
                name: "CollateralNumber",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "CourtName",
                schema: "org",
                table: "Guarantors");
        }
    }
}
