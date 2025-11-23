using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddElectricAndPaperIdTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Electric ID and Paper ID types into IdentityCardTypes table
            migrationBuilder.Sql(@"
                INSERT INTO look.""IdentityCardType"" (""Name"", ""Des"")
                VALUES ('الکترونیکی', 'Electric ID')
                ON CONFLICT DO NOTHING;
                
                INSERT INTO look.""IdentityCardType"" (""Name"", ""Des"")
                VALUES ('کاغذی', 'Paper ID')
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the inserted Electric ID and Paper ID types
            migrationBuilder.Sql(@"
                DELETE FROM look.""IdentityCardType""
                WHERE ""Name"" IN ('الکترونیکی', 'کاغذی');
            ");
        }
    }
}
