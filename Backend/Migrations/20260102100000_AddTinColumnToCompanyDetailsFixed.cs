using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTinColumnToCompanyDetailsFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add TIN column to CompanyDetails table
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_schema = 'org' 
                        AND table_name = 'CompanyDetails' 
                        AND column_name = 'TIN'
                    ) THEN
                        ALTER TABLE org.""CompanyDetails"" ADD COLUMN ""TIN"" double precision NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TIN",
                schema: "org",
                table: "CompanyDetails");
        }
    }
}
