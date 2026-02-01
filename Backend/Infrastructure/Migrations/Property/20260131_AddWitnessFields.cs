using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    /// <summary>
    /// Migration to add GrandFatherName, WitnessSide, and Des fields to WitnessDetails table
    /// </summary>
    [Migration(20260131_02)]
    public class AddWitnessFields : Migration
    {
        public override void Up()
        {
            // Add GrandFatherName field
            if (!Schema.Table("WitnessDetails").Column("GrandFatherName").Exists())
            {
                Alter.Table("WitnessDetails")
                    .InSchema("org")
                    .AddColumn("GrandFatherName").AsString(255).Nullable();
            }

            // Add WitnessSide field (Buyer or Seller)
            if (!Schema.Table("WitnessDetails").Column("WitnessSide").Exists())
            {
                Alter.Table("WitnessDetails")
                    .InSchema("org")
                    .AddColumn("WitnessSide").AsString(50).Nullable();
            }

            // Add Des (جزئیات دیگر) field
            if (!Schema.Table("WitnessDetails").Column("Des").Exists())
            {
                Alter.Table("WitnessDetails")
                    .InSchema("org")
                    .AddColumn("Des").AsString(1000).Nullable();
            }
        }

        public override void Down()
        {
            // Remove the added columns
            if (Schema.Table("WitnessDetails").Column("GrandFatherName").Exists())
            {
                Delete.Column("GrandFatherName").FromTable("WitnessDetails").InSchema("org");
            }

            if (Schema.Table("WitnessDetails").Column("WitnessSide").Exists())
            {
                Delete.Column("WitnessSide").FromTable("WitnessDetails").InSchema("org");
            }

            if (Schema.Table("WitnessDetails").Column("Des").Exists())
            {
                Delete.Column("Des").FromTable("WitnessDetails").InSchema("org");
            }
        }
    }
}
