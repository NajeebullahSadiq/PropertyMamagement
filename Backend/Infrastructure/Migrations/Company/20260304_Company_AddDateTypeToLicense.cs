using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    [Migration(20260304120000)]
    public class AddDateTypeToLicense : Migration
    {
        public override void Up()
        {
            // Add DateType column to LicenseDetails table
            Alter.Table("LicenseDetails")
                .InSchema("org")
                .AddColumn("DateType").AsString(20).Nullable()
                .WithDefaultValue("hijriShamsi"); // Default to Shamsi calendar
        }

        public override void Down()
        {
            Delete.Column("DateType")
                .FromTable("LicenseDetails")
                .InSchema("org");
        }
    }
}
