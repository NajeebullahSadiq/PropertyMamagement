using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    /// <summary>
    /// Migration to remove AnnualReportRemarks and MonitoringType columns, add MonitoringRemarks column
    /// </summary>
    [Migration(20260328_2)]
    public class ActivityMonitoring_UpdateInspectionFields : Migration
    {
        public override void Up()
        {
            // Drop AnnualReportRemarks column
            Delete.Column("AnnualReportRemarks")
                .FromTable("ActivityMonitoringRecords")
                .InSchema("org");

            // Drop MonitoringType column
            Delete.Column("MonitoringType")
                .FromTable("ActivityMonitoringRecords")
                .InSchema("org");

            // Add MonitoringRemarks column
            Alter.Table("ActivityMonitoringRecords")
                .InSchema("org")
                .AddColumn("MonitoringRemarks")
                .AsString(1000)
                .Nullable();
        }

        public override void Down()
        {
            // Add AnnualReportRemarks column back
            Alter.Table("ActivityMonitoringRecords")
                .InSchema("org")
                .AddColumn("AnnualReportRemarks")
                .AsString(1000)
                .Nullable();

            // Add MonitoringType column back
            Alter.Table("ActivityMonitoringRecords")
                .InSchema("org")
                .AddColumn("MonitoringType")
                .AsString(100)
                .Nullable();

            // Drop MonitoringRemarks column
            Delete.Column("MonitoringRemarks")
                .FromTable("ActivityMonitoringRecords")
                .InSchema("org");
        }
    }
}
