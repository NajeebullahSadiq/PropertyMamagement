using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add any missing columns to Guarantors table
    /// </summary>
    public partial class AddMissingGuarantorColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add GrandFatherName if missing
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""GrandFatherName"" VARCHAR(100) NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""GrandFatherName"";
            ");
        }
    }
}
