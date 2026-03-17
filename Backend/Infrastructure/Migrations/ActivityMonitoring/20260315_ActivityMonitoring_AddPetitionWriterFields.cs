using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260315003)]
    public class AddLicenseNumberDistrictToPetitionWriterViolations : Migration
    {
        public override void Up()
        {
            // Add LicenseNumber column
            if (!Schema.Schema("org").Table("ActivityMonitoringPetitionWriterViolations").Column("LicenseNumber").Exists())
            {
                Alter.Table("ActivityMonitoringPetitionWriterViolations")
                    .InSchema("org")
                    .AddColumn("LicenseNumber").AsString(50).Nullable();
            }

            // Add District column
            if (!Schema.Schema("org").Table("ActivityMonitoringPetitionWriterViolations").Column("District").Exists())
            {
                Alter.Table("ActivityMonitoringPetitionWriterViolations")
                    .InSchema("org")
                    .AddColumn("District").AsString(100).Nullable();
            }
        }

        public override void Down()
        {
            // Remove District column
            if (Schema.Schema("org").Table("ActivityMonitoringPetitionWriterViolations").Column("District").Exists())
            {
                Delete.Column("District")
                    .FromTable("ActivityMonitoringPetitionWriterViolations")
                    .InSchema("org");
            }

            // Remove LicenseNumber column
            if (Schema.Schema("org").Table("ActivityMonitoringPetitionWriterViolations").Column("LicenseNumber").Exists())
            {
                Delete.Column("LicenseNumber")
                    .FromTable("ActivityMonitoringPetitionWriterViolations")
                    .InSchema("org");
            }
        }
    }
}
