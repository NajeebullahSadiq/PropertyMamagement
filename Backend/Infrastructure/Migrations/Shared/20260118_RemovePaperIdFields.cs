using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Shared
{
    /// <summary>
    /// Remove all paper-based ID fields and keep only Electronic National ID
    /// الیکټرونیکی تذکره - Electronic National ID Only
    /// </summary>
    public partial class RemovePaperIdFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // 1. Remove IdentityCardType lookup table
            // ============================================
            migrationBuilder.DropTable(
                name: "IdentityCardType",
                schema: "look");

            // ============================================
            // 2. Company Module - CompanyOwner
            // ============================================
            migrationBuilder.DropForeignKey(
                name: "CompanyOwner_IdentityCardTypeId_fkey",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "IdentityCardTypeId",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "Jild",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "Safha",
                schema: "org",
                table: "CompanyOwner");

            migrationBuilder.DropColumn(
                name: "SabtNumber",
                schema: "org",
                table: "CompanyOwner");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "org",
                table: "CompanyOwner",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            // ============================================
            // 3. Company Module - Guarantor
            // ============================================
            migrationBuilder.DropForeignKey(
                name: "Guarantors_IdentityCardTypeId_fkey",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "IdentityCardTypeId",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "Jild",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "Safha",
                schema: "org",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "SabtNumber",
                schema: "org",
                table: "Guarantors");

            // Rename IndentityCardNumber to ElectronicNationalIdNumber
            migrationBuilder.RenameColumn(
                name: "IndentityCardNumber",
                schema: "org",
                table: "Guarantors",
                newName: "ElectronicNationalIdNumber");

            migrationBuilder.AlterColumn<string>(
                name: "ElectronicNationalIdNumber",
                schema: "org",
                table: "Guarantors",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore IdentityCardType lookup table
            migrationBuilder.CreateTable(
                name: "IdentityCardType",
                schema: "look",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("IdentityCardType_pkey", x => x.Id);
                });

            // Restore CompanyOwner fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "org",
                table: "CompanyOwner",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityCardTypeId",
                schema: "org",
                table: "CompanyOwner",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Jild",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Safha",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SabtNumber",
                schema: "org",
                table: "CompanyOwner",
                type: "text",
                nullable: true);

            // Restore Guarantor fields
            migrationBuilder.RenameColumn(
                name: "ElectronicNationalIdNumber",
                schema: "org",
                table: "Guarantors",
                newName: "IndentityCardNumber");

            migrationBuilder.AlterColumn<double>(
                name: "IndentityCardNumber",
                schema: "org",
                table: "Guarantors",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityCardTypeId",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Jild",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Safha",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SabtNumber",
                schema: "org",
                table: "Guarantors",
                type: "integer",
                nullable: true);

            // Restore foreign keys
            migrationBuilder.AddForeignKey(
                name: "CompanyOwner_IdentityCardTypeId_fkey",
                schema: "org",
                table: "CompanyOwner",
                column: "IdentityCardTypeId",
                principalSchema: "look",
                principalTable: "IdentityCardType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "Guarantors_IdentityCardTypeId_fkey",
                schema: "org",
                table: "Guarantors",
                column: "IdentityCardTypeId",
                principalSchema: "look",
                principalTable: "IdentityCardType",
                principalColumn: "Id");
        }
    }
}
