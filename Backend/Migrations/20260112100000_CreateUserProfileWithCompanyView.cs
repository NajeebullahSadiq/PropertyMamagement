using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Creates the UserProfileWithCompany view that joins AspNetUsers with CompanyDetails
    /// to provide user profile information with company name.
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the view
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""UserProfileWithCompany"";");
        }
    }
}
