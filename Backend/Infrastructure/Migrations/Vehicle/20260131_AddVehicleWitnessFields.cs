using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Vehicle
{
    /// <summary>
    /// Migration to add GrandFatherName, WitnessSide, and Des fields to VehiclesWitnessDetails table
    /// </summary>
    [Migration(20260131_03)]
    public class AddVehicleWitnessFields : Migration
    {
        public override void Up()
        {
            // Add GrandFatherName field
            if (!Schema.Table("VehiclesWitnessDetails").Column("GrandFatherName").Exists())
            {
                Alter.Table("VehiclesWitnessDetails")
                    .InSchema("org")
                    .AddColumn("GrandFatherName").AsString(255).Nullable();
            }

            // Add WitnessSide field (Buyer or Seller)
            if (!Schema.Table("VehiclesWitnessDetails").Column("WitnessSide").Exists())
            {
                Alter.Table("VehiclesWitnessDetails")
                    .InSchema("org")
                    .AddColumn("WitnessSide").AsString(50).Nullable();
            }

            // Add Des (جزئیات دیگر) field
            if (!Schema.Table("VehiclesWitnessDetails").Column("Des").Exists())
            {
                Alter.Table("VehiclesWitnessDetails")
                    .InSchema("org")
                    .AddColumn("Des").AsString(1000).Nullable();
            }
        }

        public override void Down()
        {
            // Remove the added columns
            if (Schema.Table("VehiclesWitnessDetails").Column("GrandFatherName").Exists())
            {
                Delete.Column("GrandFatherName").FromTable("VehiclesWitnessDetails").InSchema("org");
            }

            if (Schema.Table("VehiclesWitnessDetails").Column("WitnessSide").Exists())
            {
                Delete.Column("WitnessSide").FromTable("VehiclesWitnessDetails").InSchema("org");
            }

            if (Schema.Table("VehiclesWitnessDetails").Column("Des").Exists())
            {
                Delete.Column("Des").FromTable("VehiclesWitnessDetails").InSchema("org");
            }
        }
    }
}
