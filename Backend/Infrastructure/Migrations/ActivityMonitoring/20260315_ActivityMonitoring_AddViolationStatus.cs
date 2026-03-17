using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260315002)]
    public class AddViolationStatusFields : Migration
    {
        public override void Up()
        {
            // Add ViolationStatus column
            if (!Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("ViolationStatus").Exists())
            {
                Alter.Table("ActivityMonitoringRealEstateViolations")
                    .InSchema("org")
                    .AddColumn("ViolationStatus").AsString(50).Nullable();
            }

            // Add ClosureDate column
            if (!Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("ClosureDate").Exists())
            {
                Alter.Table("ActivityMonitoringRealEstateViolations")
                    .InSchema("org")
                    .AddColumn("ClosureDate").AsDate().Nullable();
            }

            // Add ClosureReason column
            if (!Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("ClosureReason").Exists())
            {
                Alter.Table("ActivityMonitoringRealEstateViolations")
                    .InSchema("org")
                    .AddColumn("ClosureReason").AsString(1000).Nullable();
            }

            // Make ViolationType and ViolationDate nullable since they're conditional
            Alter.Column("ViolationType")
                .OnTable("ActivityMonitoringRealEstateViolations")
                .InSchema("org")
                .AsString(500).Nullable();

            Alter.Column("ViolationDate")
                .OnTable("ActivityMonitoringRealEstateViolations")
                .InSchema("org")
                .AsDate().Nullable();
        }

        public override void Down()
        {
            // Remove ClosureReason column
            if (Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("ClosureReason").Exists())
            {
                Delete.Column("ClosureReason")
                    .FromTable("ActivityMonitoringRealEstateViolations")
                    .InSchema("org");
            }

            // Remove ClosureDate column
            if (Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("ClosureDate").Exists())
            {
                Delete.Column("ClosureDate")
                    .FromTable("ActivityMonitoringRealEstateViolations")
                    .InSchema("org");
            }

            // Remove ViolationStatus column
            if (Schema.Schema("org").Table("ActivityMonitoringRealEstateViolations").Column("ViolationStatus").Exists())
            {
                Delete.Column("ViolationStatus")
                    .FromTable("ActivityMonitoringRealEstateViolations")
                    .InSchema("org");
            }
        }
    }
}
