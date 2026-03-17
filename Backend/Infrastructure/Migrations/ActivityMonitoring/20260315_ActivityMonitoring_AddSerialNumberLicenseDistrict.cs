using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260315000006)]
    public class ActivityMonitoring_AddSerialNumberLicenseDistrict : Migration
    {
        public override void Up()
        {
            // Add SerialNumber, LicenseNumber, and District columns to ActivityMonitoringRecords
            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("SerialNumber").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("SerialNumber").AsString(50).Nullable();

                Create.Index("IX_ActivityMonitoringRecords_SerialNumber")
                    .OnTable("ActivityMonitoringRecords").InSchema("org")
                    .OnColumn("SerialNumber");
            }

            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("LicenseNumber").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("LicenseNumber").AsString(50).Nullable();

                Create.Index("IX_ActivityMonitoringRecords_LicenseNumber")
                    .OnTable("ActivityMonitoringRecords").InSchema("org")
                    .OnColumn("LicenseNumber");
            }

            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("District").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("District").AsString(200).Nullable();
            }
        }

        public override void Down()
        {
            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("District").Exists())
            {
                Delete.Column("District")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("LicenseNumber").Exists())
            {
                Delete.Index("IX_ActivityMonitoringRecords_LicenseNumber")
                    .OnTable("ActivityMonitoringRecords")
                    .InSchema("org");

                Delete.Column("LicenseNumber")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("SerialNumber").Exists())
            {
                Delete.Index("IX_ActivityMonitoringRecords_SerialNumber")
                    .OnTable("ActivityMonitoringRecords")
                    .InSchema("org");

                Delete.Column("SerialNumber")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }
        }
    }
}
