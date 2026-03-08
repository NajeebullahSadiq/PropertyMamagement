using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    [Migration(20260308000001)]
    public class RemovePropertyStatusFields : Migration
    {
        public override void Up()
        {
            // Drop index on Status column
            Delete.Index("IX_PropertyDetails_Status")
                .OnTable("PropertyDetails")
                .InSchema("tr");

            // Remove Status and related approval fields from PropertyDetails table
            Delete.Column("Status").FromTable("PropertyDetails").InSchema("tr");
            Delete.Column("VerifiedBy").FromTable("PropertyDetails").InSchema("tr");
            Delete.Column("VerifiedAt").FromTable("PropertyDetails").InSchema("tr");
            Delete.Column("ApprovedBy").FromTable("PropertyDetails").InSchema("tr");
            Delete.Column("ApprovedAt").FromTable("PropertyDetails").InSchema("tr");
        }

        public override void Down()
        {
            // Add columns back
            Alter.Table("PropertyDetails").InSchema("tr")
                .AddColumn("Status").AsString(50).WithDefaultValue("Draft").Nullable()
                .AddColumn("VerifiedBy").AsString(50).Nullable()
                .AddColumn("VerifiedAt").AsDateTime().Nullable()
                .AddColumn("ApprovedBy").AsString(50).Nullable()
                .AddColumn("ApprovedAt").AsDateTime().Nullable();

            // Recreate index
            Create.Index("IX_PropertyDetails_Status")
                .OnTable("PropertyDetails")
                .InSchema("tr")
                .OnColumn("Status");
        }
    }
}
