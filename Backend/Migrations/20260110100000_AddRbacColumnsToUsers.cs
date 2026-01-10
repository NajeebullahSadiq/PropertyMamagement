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
            // Add LicenseType column to AspNetUsers
            migrationBuilder.AddColumn<string>(
                name: "LicenseType",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Add UserRole column to AspNetUsers
            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Add CreatedAt column to AspNetUsers
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            // Add CreatedBy column to AspNetUsers
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

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
