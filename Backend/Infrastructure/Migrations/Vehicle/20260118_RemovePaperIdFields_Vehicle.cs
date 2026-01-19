using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Vehicle
{
    /// <summary>
    /// Remove paper-based Tazkira fields from Vehicle module
    /// Keep only Electronic National ID - الیکټرونیکی تذکره
    /// </summary>
    public partial class RemovePaperIdFields_Vehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // VehiclesSellerDetail
            // ============================================
            migrationBuilder.DropColumn(
                name: "TazkiraType",
                schema: "tr",
                table: "VehiclesSellerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraVolume",
                schema: "tr",
                table: "VehiclesSellerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraPage",
                schema: "tr",
                table: "VehiclesSellerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraNumber",
                schema: "tr",
                table: "VehiclesSellerDetail");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesSellerDetail",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "VehiclesSellerDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            // ============================================
            // VehiclesBuyerDetail
            // ============================================
            migrationBuilder.DropColumn(
                name: "TazkiraType",
                schema: "tr",
                table: "VehiclesBuyerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraVolume",
                schema: "tr",
                table: "VehiclesBuyerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraPage",
                schema: "tr",
                table: "VehiclesBuyerDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraNumber",
                schema: "tr",
                table: "VehiclesBuyerDetail");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            // ============================================
            // VehiclesWitnessDetail
            // ============================================
            migrationBuilder.DropColumn(
                name: "TazkiraType",
                schema: "tr",
                table: "VehiclesWitnessDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraVolume",
                schema: "tr",
                table: "VehiclesWitnessDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraPage",
                schema: "tr",
                table: "VehiclesWitnessDetail");

            migrationBuilder.DropColumn(
                name: "TazkiraNumber",
                schema: "tr",
                table: "VehiclesWitnessDetail");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore VehiclesSellerDetail fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "VehiclesSellerDetail",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesSellerDetail",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraType",
                schema: "tr",
                table: "VehiclesSellerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraVolume",
                schema: "tr",
                table: "VehiclesSellerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraPage",
                schema: "tr",
                table: "VehiclesSellerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraNumber",
                schema: "tr",
                table: "VehiclesSellerDetail",
                type: "text",
                nullable: true);

            // Restore VehiclesBuyerDetail fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraType",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraVolume",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraPage",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraNumber",
                schema: "tr",
                table: "VehiclesBuyerDetail",
                type: "text",
                nullable: true);

            // Restore VehiclesWitnessDetail fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraType",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraVolume",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraPage",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TazkiraNumber",
                schema: "tr",
                table: "VehiclesWitnessDetail",
                type: "text",
                nullable: true);
        }
    }
}
