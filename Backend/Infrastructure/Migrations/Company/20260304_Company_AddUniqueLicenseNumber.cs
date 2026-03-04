using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    [Migration(20260304000001)]
    public class Company_AddUniqueLicenseNumber : Migration
    {
        public override void Up()
        {
            // Create unique index on LicenseNumber (partial index - only for non-null values)
            Create.Index("IX_LicenseDetails_LicenseNumber")
                .OnTable("LicenseDetails")
                .InSchema("org")
                .OnColumn("LicenseNumber")
                .Ascending()
                .WithOptions()
                .Unique();
        }

        public override void Down()
        {
            // Remove the unique index
            Delete.Index("IX_LicenseDetails_LicenseNumber")
                .OnTable("LicenseDetails")
                .InSchema("org");
        }
    }
}
