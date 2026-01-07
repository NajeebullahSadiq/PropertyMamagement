using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDariColumnToEducationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Dari column to EducationLevel table
            migrationBuilder.AddColumn<string>(
                name: "Dari",
                schema: "look",
                table: "EducationLevel",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Populate Dari column - copy Name to Dari for all records
            // This ensures all records have a Dari value
            migrationBuilder.Sql(@"
                UPDATE look.""EducationLevel"" 
                SET ""Dari"" = COALESCE(""Name"", 'نامشخص')
                WHERE ""Dari"" IS NULL OR ""Dari"" = '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dari",
                schema: "look",
                table: "EducationLevel");
        }
    }
}
