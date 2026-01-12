using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Creates the UserProfileWithCompany view and ensures RBAC roles are seeded.
    /// </summary>
    public partial class CreateUserProfileWithCompanyView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the UserProfileWithCompany view
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW ""UserProfileWithCompany"" AS
                SELECT 
                    u.""Id"" AS ""UserId"",
                    u.""Email"",
                    u.""UserName"",
                    u.""FirstName"",
                    u.""LastName"",
                    u.""PhotoPath"",
                    c.""Title"" AS ""CompanyName"",
                    COALESCE(c.""PhoneNumber"", u.""PhoneNumber"") AS ""PhoneNumber""
                FROM ""AspNetUsers"" u
                LEFT JOIN ""org"".""CompanyDetails"" c ON u.""CompanyId"" = c.""Id"";
            ");

            // Ensure RBAC roles exist in AspNetRoles
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'ADMIN', 'ADMIN', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'ADMIN');

                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'AUTHORITY', 'AUTHORITY', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'AUTHORITY');

                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'COMPANY_REGISTRAR', 'COMPANY_REGISTRAR', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'COMPANY_REGISTRAR');

                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'LICENSE_REVIEWER', 'LICENSE_REVIEWER', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'LICENSE_REVIEWER');

                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'PROPERTY_OPERATOR', 'PROPERTY_OPERATOR', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'PROPERTY_OPERATOR');

                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT gen_random_uuid()::text, 'VEHICLE_OPERATOR', 'VEHICLE_OPERATOR', gen_random_uuid()::text
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""Name"" = 'VEHICLE_OPERATOR');
            ");

            // Ensure admin users have ADMIN role assigned in AspNetUserRoles
            migrationBuilder.Sql(@"
                INSERT INTO ""AspNetUserRoles"" (""UserId"", ""RoleId"")
                SELECT u.""Id"", r.""Id""
                FROM ""AspNetUsers"" u
                CROSS JOIN ""AspNetRoles"" r
                WHERE u.""IsAdmin"" = true 
                AND r.""Name"" = 'ADMIN'
                AND NOT EXISTS (
                    SELECT 1 FROM ""AspNetUserRoles"" ur 
                    WHERE ur.""UserId"" = u.""Id"" AND ur.""RoleId"" = r.""Id""
                );
            ");

            // Update UserRole column for admin users
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers"" 
                SET ""UserRole"" = 'ADMIN'
                WHERE ""IsAdmin"" = true AND (""UserRole"" IS NULL OR ""UserRole"" = '');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the view
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""UserProfileWithCompany"";");
        }
    }
}
