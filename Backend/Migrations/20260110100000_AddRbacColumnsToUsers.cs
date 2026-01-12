using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRbacColumnsToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Safely add columns using DO block to check if they exist first
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    -- Set default value for Discriminator column if it exists
                    IF EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AspNetUsers' AND column_name = 'Discriminator') THEN
                        ALTER TABLE ""AspNetUsers"" ALTER COLUMN ""Discriminator"" SET DEFAULT 'ApplicationUser';
                        UPDATE ""AspNetUsers"" SET ""Discriminator"" = 'ApplicationUser' WHERE ""Discriminator"" IS NULL;
                    END IF;
                    
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AspNetUsers' AND column_name = 'LicenseType') THEN
                        ALTER TABLE ""AspNetUsers"" ADD COLUMN ""LicenseType"" VARCHAR(50) NULL;
                    END IF;
                    
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AspNetUsers' AND column_name = 'UserRole') THEN
                        ALTER TABLE ""AspNetUsers"" ADD COLUMN ""UserRole"" VARCHAR(50) NULL;
                    END IF;
                    
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AspNetUsers' AND column_name = 'CreatedAt') THEN
                        ALTER TABLE ""AspNetUsers"" ADD COLUMN ""CreatedAt"" TIMESTAMP NULL;
                    END IF;
                    
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'AspNetUsers' AND column_name = 'CreatedBy') THEN
                        ALTER TABLE ""AspNetUsers"" ADD COLUMN ""CreatedBy"" VARCHAR(255) NULL;
                    END IF;
                END $$;
            ");

            // Update existing admin users to have ADMIN role
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers"" 
                SET ""UserRole"" = 'ADMIN', ""CreatedAt"" = NOW()
                WHERE ""IsAdmin"" = true AND ""UserRole"" IS NULL;
            ");

            // Seed RBAC roles - مقام / رهبری
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'AUTHORITY', 'AUTHORITY', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'AUTHORITY');
            ");

            // کاربر ثبت جواز شرکت
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'COMPANY_REGISTRAR', 'COMPANY_REGISTRAR', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'COMPANY_REGISTRAR');
            ");

            // ریاست بررسی و ثبت جواز
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'LICENSE_REVIEWER', 'LICENSE_REVIEWER', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'LICENSE_REVIEWER');
            ");

            // کاربر عملیاتی املاک
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'PROPERTY_OPERATOR', 'PROPERTY_OPERATOR', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'PROPERTY_OPERATOR');
            ");

            // کاربر عملیاتی موتر فروشی
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'VEHICLE_OPERATOR', 'VEHICLE_OPERATOR', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'VEHICLE_OPERATOR');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LicenseType", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "UserRole", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "CreatedBy", table: "AspNetUsers");

            // Remove seeded roles
            migrationBuilder.Sql(@"
                DELETE FROM ""AspNetRoles"" 
                WHERE ""Name"" IN ('AUTHORITY', 'COMPANY_REGISTRAR', 'LICENSE_REVIEWER', 'PROPERTY_OPERATOR', 'VEHICLE_OPERATOR');
            ");
        }
    }
}
