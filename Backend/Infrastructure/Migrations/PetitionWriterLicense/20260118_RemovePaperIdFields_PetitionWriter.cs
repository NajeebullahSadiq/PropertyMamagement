using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    /// <summary>
    /// Remove paper-based ID fields from Petition Writer License
    /// Keep only Electronic National ID - الیکټرونیکی تذکره
    /// </summary>
    public partial class RemovePaperIdFields_PetitionWriter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove IdentityCardType column (no longer needed)
            migrationBuilder.DropColumn(
                name: "IdentityCardType",
                schema: "org",
                table: "PetitionWriterLicenses");

            // Remove all paper ID fields
            migrationBuilder.DropColumn(
                name: "PaperIdNumber",
                schema: "org",
                table: "PetitionWriterLicenses");

            migrationBuilder.DropColumn(
                name: "PaperIdVolume",
                schema: "org",
                table: "PetitionWriterLicenses");

            migrationBuilder.DropColumn(
                name: "PaperIdPage",
                schema: "org",
                table: "PetitionWriterLicenses");

            migrationBuilder.DropColumn(
                name: "PaperIdRegNumber",
                schema: "org",
                table: "PetitionWriterLicenses");

            // Rename ElectronicIdNumber to ElectronicNationalIdNumber for consistency
            migrationBuilder.RenameColumn(
                name: "ElectronicIdNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                newName: "ElectronicNationalIdNumber");

            // Make ElectronicNationalIdNumber required
            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore IdentityCardType
            migrationBuilder.AddColumn<int>(
                name: "IdentityCardType",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            // Restore paper ID fields
            migrationBuilder.AddColumn<string>(
                name: "PaperIdNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaperIdVolume",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaperIdPage",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaperIdRegNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Restore original column name
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                newName: "ElectronicIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicIdNumber",
                schema: "org",
                table: "PetitionWriterLicenses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: false);
        }
    }
}
