using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Infrastructure.Migrations.Vehicle
{
    /// <summary>
    /// Migration to change PilateNo, PermitNo, EnginNo, and ShasiNo from integer to string/text
    /// Schema: tr
    /// Table: VehiclesPropertyDetails
    /// </summary>
    public partial class ChangePlateNumberToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Change PermitNo from integer to text
            migrationBuilder.AlterColumn<string>(
                name: "PermitNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            // Change PilateNo from integer to text
            migrationBuilder.AlterColumn<string>(
                name: "PilateNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            // Change EnginNo from integer to text
            migrationBuilder.AlterColumn<string>(
                name: "EnginNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Change ShasiNo from integer to text
            migrationBuilder.AlterColumn<string>(
                name: "ShasiNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert ShasiNo back to integer
            migrationBuilder.AlterColumn<int>(
                name: "ShasiNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Revert EnginNo back to integer
            migrationBuilder.AlterColumn<int>(
                name: "EnginNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Revert PilateNo back to integer
            migrationBuilder.AlterColumn<int>(
                name: "PilateNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Revert PermitNo back to integer
            migrationBuilder.AlterColumn<int>(
                name: "PermitNo",
                schema: "tr",
                table: "VehiclesPropertyDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
