using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    [Migration(20260127000001)]
    public class RemovePaperTazkiraFields_Property : Migration
    {
        public override void Up()
        {
            // Remove paper-based Tazkira fields from SellerDetails
            if (Schema.Schema("tr").Table("SellerDetails").Column("TazkiraType").Exists())
            {
                Delete.Column("TazkiraType").FromTable("SellerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("SellerDetails").Column("TazkiraVolume").Exists())
            {
                Delete.Column("TazkiraVolume").FromTable("SellerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("SellerDetails").Column("TazkiraPage").Exists())
            {
                Delete.Column("TazkiraPage").FromTable("SellerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("SellerDetails").Column("TazkiraNumber").Exists())
            {
                Delete.Column("TazkiraNumber").FromTable("SellerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("SellerDetails").Column("TazkiraRegNumber").Exists())
            {
                Delete.Column("TazkiraRegNumber").FromTable("SellerDetails").InSchema("tr");
            }

            // Remove paper-based Tazkira fields from BuyerDetails
            if (Schema.Schema("tr").Table("BuyerDetails").Column("TazkiraType").Exists())
            {
                Delete.Column("TazkiraType").FromTable("BuyerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("BuyerDetails").Column("TazkiraVolume").Exists())
            {
                Delete.Column("TazkiraVolume").FromTable("BuyerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("BuyerDetails").Column("TazkiraPage").Exists())
            {
                Delete.Column("TazkiraPage").FromTable("BuyerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("BuyerDetails").Column("TazkiraNumber").Exists())
            {
                Delete.Column("TazkiraNumber").FromTable("BuyerDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("BuyerDetails").Column("TazkiraRegNumber").Exists())
            {
                Delete.Column("TazkiraRegNumber").FromTable("BuyerDetails").InSchema("tr");
            }

            // Remove paper-based Tazkira fields from WitnessDetails
            if (Schema.Schema("tr").Table("WitnessDetails").Column("TazkiraType").Exists())
            {
                Delete.Column("TazkiraType").FromTable("WitnessDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("WitnessDetails").Column("TazkiraVolume").Exists())
            {
                Delete.Column("TazkiraVolume").FromTable("WitnessDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("WitnessDetails").Column("TazkiraPage").Exists())
            {
                Delete.Column("TazkiraPage").FromTable("WitnessDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("WitnessDetails").Column("TazkiraNumber").Exists())
            {
                Delete.Column("TazkiraNumber").FromTable("WitnessDetails").InSchema("tr");
            }
            if (Schema.Schema("tr").Table("WitnessDetails").Column("TazkiraRegNumber").Exists())
            {
                Delete.Column("TazkiraRegNumber").FromTable("WitnessDetails").InSchema("tr");
            }
        }

        public override void Down()
        {
            // Restore paper-based Tazkira fields to SellerDetails
            Create.Column("TazkiraType").OnTable("SellerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraVolume").OnTable("SellerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraPage").OnTable("SellerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraNumber").OnTable("SellerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraRegNumber").OnTable("SellerDetails").InSchema("tr").AsString().Nullable();

            // Restore paper-based Tazkira fields to BuyerDetails
            Create.Column("TazkiraType").OnTable("BuyerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraVolume").OnTable("BuyerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraPage").OnTable("BuyerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraNumber").OnTable("BuyerDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraRegNumber").OnTable("BuyerDetails").InSchema("tr").AsString().Nullable();

            // Restore paper-based Tazkira fields to WitnessDetails
            Create.Column("TazkiraType").OnTable("WitnessDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraVolume").OnTable("WitnessDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraPage").OnTable("WitnessDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraNumber").OnTable("WitnessDetails").InSchema("tr").AsString().Nullable();
            Create.Column("TazkiraRegNumber").OnTable("WitnessDetails").InSchema("tr").AsString().Nullable();
        }
    }
}
