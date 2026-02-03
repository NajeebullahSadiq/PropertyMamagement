using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Vehicle
{
    /// <summary>
    /// Migration to add HalfPrice field to VehiclesPropertyDetails
    /// Schema: tr
    /// Table: VehiclesPropertyDetails
    /// </summary>
    public partial class AddHalfPriceField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add HalfPrice column
            migrationBuilder.AddColumn<string>(
                name: "HalfPrice",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove HalfPrice column
            migrationBuilder.DropColumn(
                name: "HalfPrice",
                schema: "tr",
                table: "VehiclesPropertyDetails");
        }
    }
}
