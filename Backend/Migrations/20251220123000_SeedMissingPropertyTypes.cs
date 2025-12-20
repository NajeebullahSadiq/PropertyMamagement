using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPIBackend.Configuration;

#nullable disable

namespace WebAPIBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251220123000_SeedMissingPropertyTypes")]
    public partial class SeedMissingPropertyTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Normalize legacy name: OtherT -> Other (only if Other does not already exist)
                    IF EXISTS (
                        SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'OtherT'
                    ) AND NOT EXISTS (
                        SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Other'
                    ) THEN
                        UPDATE look.""PropertyType"" SET ""Name"" = 'Other' WHERE ""Name"" = 'OtherT';
                    END IF;

                    -- Ensure required property types exist
                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'House') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('House', 'House');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Apartment') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Apartment', 'Apartment');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Shop') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Shop', 'Shop');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Block') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Block', 'Residential block');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Land') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Land', 'Land');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Garden') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Garden', 'Garden');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Hill') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Hill', 'Hill');
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM look.""PropertyType"" WHERE ""Name"" = 'Other') THEN
                        INSERT INTO look.""PropertyType"" (""Name"", ""Des"") VALUES ('Other', 'Other');
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: we don't delete lookup rows on rollback to avoid breaking existing records.
        }
    }
}
