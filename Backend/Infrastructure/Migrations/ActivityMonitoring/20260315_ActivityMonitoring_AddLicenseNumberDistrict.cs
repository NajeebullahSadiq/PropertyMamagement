using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260315001)]
    public class AddLicenseNumberDistrictToRealEstateViolations : Migration
    {
        public override void Up()
        {
            // Add LicenseNumber column
            if (!Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("LicenseNumber").Exists())
            {
                Alter.Table("ActivityMonitoringRealEstateViolations")
                    .InSchema("org")
                    .AddColumn("LicenseNumber").AsString(50).Nullable();
            }

            // Add District column
            if (!Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("District").Exists())
            {
                Alter.Table("ActivityMonitoringRealEstateViolations")
                    .InSchema("org")
                    .AddColumn("District").AsString(100).Nullable();
            }
        }

        public override void Down()
        {
            // Remove District column
            if (Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("District").Exists())
            {
                Delete.Column("District")
                    .FromTable("ActivityMonitoringRealEstateViolations")
                    .InSchema("org");
            }

            // Remove LicenseNumber column
            if (Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("LicenseNumber").Exists())
            {
                Delete.Column("LicenseNumber")
                    .FromTable("ActivityMonitoringRealEstateViolations")
                    .InSchema("org");
            }
        }
    }
}
