using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260315004)]
    public class RestructureInspectionSection : Migration
    {
        public override void Up()
        {
            // Create new Inspections table
            if (!Schema.Schema("org").Table("ActivityMonitoringInspections").Exists())
            {
                Create.Table("ActivityMonitoringInspections")
                    .InSchema("org")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("ActivityMonitoringRecordId").AsInt32().NotNullable()
                        .ForeignKey("FK_ActivityMonitoringInspections_ActivityMonitoringRecords", 
                            "org", "ActivityMonitoringRecords", "Id")
                    .WithColumn("MonitoringType").AsString(100).NotNullable()
                    .WithColumn("Month").AsString(50).NotNullable()
                    .WithColumn("MonitoringCount").AsInt32().NotNullable()
                    .WithColumn("CreatedAt").AsDateTime().Nullable()
                    .WithColumn("CreatedBy").AsString(50).Nullable();
            }

            // Remove old inspection fields from ActivityMonitoringRecords
            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("InspectionDate").Exists())
            {
                Delete.Column("InspectionDate")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("InspectedRealEstateOfficesCount").Exists())
            {
                Delete.Column("InspectedRealEstateOfficesCount")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("SealedOfficesCount").Exists())
            {
                Delete.Column("SealedOfficesCount")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("InspectedPetitionWritersCount").Exists())
            {
                Delete.Column("InspectedPetitionWritersCount")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }

            if (Schema.Schema("org").Table("ActivityMonitoringRecords").Column("ViolatingPetitionWritersCount").Exists())
            {
                Delete.Column("ViolatingPetitionWritersCount")
                    .FromTable("ActivityMonitoringRecords")
                    .InSchema("org");
            }
        }

        public override void Down()
        {
            // Drop Inspections table
            if (Schema.Schema("org").Table("ActivityMonitoringInspections").Exists())
            {
                Delete.Table("ActivityMonitoringInspections").InSchema("org");
            }

            // Restore old fields (optional - for rollback)
            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("InspectionDate").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("InspectionDate").AsDate().Nullable();
            }

            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("InspectedRealEstateOfficesCount").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("InspectedRealEstateOfficesCount").AsInt32().Nullable();
            }

            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("SealedOfficesCount").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("SealedOfficesCount").AsInt32().Nullable();
            }

            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("InspectedPetitionWritersCount").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("InspectedPetitionWritersCount").AsInt32().Nullable();
            }

            if (!Schema.Schema("org").Table("ActivityMonitoringRecords").Column("ViolatingPetitionWritersCount").Exists())
            {
                Alter.Table("ActivityMonitoringRecords")
                    .InSchema("org")
                    .AddColumn("ViolatingPetitionWritersCount").AsInt32().Nullable();
            }
        }
    }
}
