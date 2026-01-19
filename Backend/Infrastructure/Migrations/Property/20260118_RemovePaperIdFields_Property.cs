using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    /// <summary>
    /// Remove paper-based Tazkira fields from Property module
    /// Keep only Electronic National ID - الیکټرونیکی تذکره
    /// </summary>
    public partial class RemovePaperIdFields_Property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // SellerDetail
            // ============================================
            migrationBuilder.DropColumn(
                name: "TazkiraType",
                schema: "tr",
                table: "SellerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraVolume",
                schema: "tr",
                table: "SellerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraPage",
                schema: "tr",
                table: "SellerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraNumber",
                schema: "tr",
                table: "SellerDetail");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "SellerDetail",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "SellerDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            // ============================================
            // BuyerDetail
            // ============================================
            migrationBuilder.DropColumn(
                name: "TazkiraType",
                schema: "tr",
                table: "BuyerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraVolume",
                schema: "tr",
                table: "BuyerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraPage",
                schema: "tr",
                table: "BuyerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraNumber",
                schema: "tr",
                table: "BuyerDetail");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "BuyerDetail",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "BuyerDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            // ============================================
            // WitnessDetail
            // ============================================
            migrationBuilder.DropColumn(
                name: "TazkiraType",
                schema: "tr",
                table: "WitnessDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraVolume",
                schema: "tr",
                table: "WitnessDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraPage",
                schema: "tr",
                table: "WitnessDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraNumber",
                schema: "tr",
                table: "WitnessDetail");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "WitnessDetail",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "WitnessDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore SellerDetail fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "SellerDetail",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "SellerDetail",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraType",
                schema: "tr",
                table: "SellerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraVolume",
                schema: "tr",
                table: "SellerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraPage",
                schema: "tr",
                table: "SellerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraNumber",
                schema: "tr",
                table: "SellerDetail",
                type: "text",
                nullable: true);

            // Restore BuyerDetail fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "BuyerDetail",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "BuyerDetail",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraType",
                schema: "tr",
                table: "BuyerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraVolume",
                schema: "tr",
                table: "BuyerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraPage",
                schema: "tr",
                table: "BuyerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraNumber",
                schema: "tr",
                table: "BuyerDetail",
                type: "text",
                nullable: true);

            // Restore WitnessDetail fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "WitnessDetail",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "WitnessDetail",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraType",
                schema: "tr",
                table: "WitnessDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraVolume",
                schema: "tr",
                table: "WitnessDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraPage",
                schema: "tr",
                table: "WitnessDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraNumber",
                schema: "tr",
                table: "WitnessDetail",
                type: "text",
                nullable: true);
        }
    }
}
