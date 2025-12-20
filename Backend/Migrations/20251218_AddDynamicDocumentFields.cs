using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicDocumentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove old document fields
            migrationBuilder.DropColumn(
                name: "Doctype",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "DeedDate",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "PrivateNumber",
                schema: "tr",
                table: "PropertyDetails");

            // Add new dynamic document fields
            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuanceNumber",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuanceDate",
                schema: "tr",
                table: "PropertyDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                schema: "tr",
                table: "PropertyDetails",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove new fields
            migrationBuilder.DropColumn(
                name: "DocumentType",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "IssuanceNumber",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "IssuanceDate",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                schema: "tr",
                table: "PropertyDetails");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                schema: "tr",
                table: "PropertyDetails");

            // Restore old fields
            migrationBuilder.AddColumn<string>(
                name: "Doctype",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeedDate",
                schema: "tr",
                table: "PropertyDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateNumber",
                schema: "tr",
                table: "PropertyDetails",
                type: "text",
                nullable: true);
        }
    }
}
