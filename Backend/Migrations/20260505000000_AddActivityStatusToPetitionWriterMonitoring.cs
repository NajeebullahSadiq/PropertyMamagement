using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityStatusToPetitionWriterMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityStatus",
                schema: "org",
                table: "PetitionWriterMonitoringRecords",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivityPermissionReason",
                schema: "org",
                table: "PetitionWriterMonitoringRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityStatus",
                schema: "org",
                table: "PetitionWriterMonitoringRecords");

            migrationBuilder.DropColumn(
                name: "ActivityPermissionReason",
                schema: "org",
                table: "PetitionWriterMonitoringRecords");
        }
    }
}
