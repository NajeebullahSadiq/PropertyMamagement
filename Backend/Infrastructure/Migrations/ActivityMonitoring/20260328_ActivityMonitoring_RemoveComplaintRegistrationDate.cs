using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    /// <summary>
    /// Migration to remove ComplaintRegistrationDate column from ActivityMonitoringRecords table
    /// </summary>
    [Migration(20260328)]
    public class ActivityMonitoring_RemoveComplaintRegistrationDate : Migration
    {
        public override void Up()
        {
            // Drop column
            Delete.Column("ComplaintRegistrationDate")
                .FromTable("ActivityMonitoringRecords")
                .InSchema("org");
        }

        public override void Down()
        {
            // Add column back
            Alter.Table("ActivityMonitoringRecords")
                .InSchema("org")
                .AddColumn("ComplaintRegistrationDate")
                .AsDate()
                .Nullable();
        }
    }
}
