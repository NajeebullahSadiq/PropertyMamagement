using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    /// <summary>
    /// Migration to add Year column to ActivityMonitoringRecords table
    /// for the inspection section (۴. نظارت وبررسی فعالیت دفاتر رهنمای معاملات)
    /// </summary>
    [Migration(20260328)]
    public class ActivityMonitoring_AddYearColumn : Migration
    {
        public override void Up()
        {
            // Add Year column for inspection section
            Alter.Table("ActivityMonitoringRecords")
                .InSchema("org")
                .AddColumn("Year")
                .AsString(20)
                .Nullable();

            // Create index on Year column for better query performance
            Create.Index("IX_ActivityMonitoringRecords_Year")
                .OnTable("ActivityMonitoringRecords")
                .InSchema("org")
                .OnColumn("Year");
        }

        public override void Down()
        {
            // Drop index first
            Delete.Index("IX_ActivityMonitoringRecords_Year")
                .OnTable("ActivityMonitoringRecords")
                .InSchema("org");

            // Drop column
            Delete.Column("Year")
                .FromTable("ActivityMonitoringRecords")
                .InSchema("org");
        }
    }
}
