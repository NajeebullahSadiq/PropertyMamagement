using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260315005)]
    public class RemoveCancelledAndLostDeedsFields : Migration
    {
        public override void Up()
        {
            // Remove CancelledMixedTransactions column
            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("CancelledMixedTransactions").Exists())
            {
                Delete.Column("CancelledMixedTransactions")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            // Remove LostDeedsCount column
            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("LostDeedsCount").Exists())
            {
                Delete.Column("LostDeedsCount")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }
        }

        public override void Down()
        {
            // Restore CancelledMixedTransactions column
            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("CancelledMixedTransactions").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("CancelledMixedTransactions").AsInt32().Nullable();
            }

            // Restore LostDeedsCount column
            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("LostDeedsCount").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("LostDeedsCount").AsInt32().Nullable();
            }
        }
    }
}
