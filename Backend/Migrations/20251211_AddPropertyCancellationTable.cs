using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyCancellationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyCancellations",
                schema: "tr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyDetailsId = table.Column<int>(type: "integer", nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PropertyCancellations_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "PropertyCancellations_PropertyDetailsId_fkey",
                        column: x => x.PropertyDetailsId,
                        principalSchema: "tr",
                        principalTable: "PropertyDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCancellations_PropertyDetailsId",
                schema: "tr",
                table: "PropertyCancellations",
                column: "PropertyDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyCancellations",
                schema: "tr");
        }
    }
}
