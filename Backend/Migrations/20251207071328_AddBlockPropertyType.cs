using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockPropertyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PropertyType",
                schema: "look",
                columns: new[] { "Name", "Des" },
                values: new object[] { "Block", "Residential block" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PropertyType",
                schema: "look",
                keyColumn: "Name",
                keyValue: "Block");
        }
    }
}
