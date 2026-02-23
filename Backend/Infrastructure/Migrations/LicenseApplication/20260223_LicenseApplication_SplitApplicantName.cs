using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.LicenseApplication
{
    /// <summary>
    /// Split ApplicantName into Name, FatherName, GrandfatherName and add ElectronicNumber
    /// شهرت متقاضی را به نام، نام پدر و نام پدرکلان تقسیم کرده و نمبر الکترونیکی اضافه می‌کند
    /// </summary>
    public partial class LicenseApplication_SplitApplicantName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns
            migrationBuilder.AddColumn<string>(
                name: "ApplicantFatherName",
                schema: "org",
                table: "LicenseApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantGrandfatherName",
                schema: "org",
                table: "LicenseApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantElectronicNumber",
                schema: "org",
                table: "LicenseApplications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Create unique index for ApplicantElectronicNumber (partial index - only for non-null values)
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_LicenseApplications_ApplicantElectronicNumber"" 
                ON org.""LicenseApplications"" (""ApplicantElectronicNumber"") 
                WHERE ""ApplicantElectronicNumber"" IS NOT NULL AND ""ApplicantElectronicNumber"" != '';
            ");

            // Rename ApplicantName column comment to reflect it's now just the name
            migrationBuilder.AlterColumn<string>(
                name: "ApplicantName",
                schema: "org",
                table: "LicenseApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                comment: "نام متقاضی",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop unique index
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS org.""IX_LicenseApplications_ApplicantElectronicNumber"";");

            migrationBuilder.DropColumn(
                name: "ApplicantFatherName",
                schema: "org",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ApplicantGrandfatherName",
                schema: "org",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ApplicantElectronicNumber",
                schema: "org",
                table: "LicenseApplications");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantName",
                schema: "org",
                table: "LicenseApplications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldComment: "نام متقاضی");
        }
    }
}
