using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class RenameDatabaseNumberToLicenseNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the table exists before attempting to rename the column
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_schema = 'org' 
                        AND table_name = 'CompanyDetails'
                    ) THEN
                        -- Check if the old column exists
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'org'
                            AND table_name = 'CompanyDetails'
                            AND column_name = 'DatabaseNumber'
                        ) THEN
                            -- Rename the column
                            ALTER TABLE org.""CompanyDetails"" RENAME COLUMN ""DatabaseNumber"" TO ""LicenseNumber"";
                            -- Change the column type from integer to text
                            ALTER TABLE org.""CompanyDetails"" ALTER COLUMN ""LicenseNumber"" TYPE text;
                        END IF;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Check if the table exists before attempting to rename the column back
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_schema = 'org' 
                        AND table_name = 'CompanyDetails'
                    ) THEN
                        -- Check if the new column exists
                        IF EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'org'
                            AND table_name = 'CompanyDetails'
                            AND column_name = 'LicenseNumber'
                        ) THEN
                            -- Change the column type back from text to integer
                            ALTER TABLE org.""CompanyDetails"" ALTER COLUMN ""LicenseNumber"" TYPE integer;
                            -- Rename the column back
                            ALTER TABLE org.""CompanyDetails"" RENAME COLUMN ""LicenseNumber"" TO ""DatabaseNumber"";
                        END IF;
                    END IF;
                END $$;
            ");
        }
    }
}
