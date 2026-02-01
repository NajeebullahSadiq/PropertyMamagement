using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Vehicle
{
    [Migration(20260130_02)]
    public class AddCompanyIdToVehicle : Migration
    {
        public override void Up()
        {
            // Add CompanyId column to VehiclesPropertyDetails table
            Alter.Table("VehiclesPropertyDetails")
                .AddColumn("CompanyId").AsInt32().Nullable();

            // Create index for better query performance
            Create.Index("IX_VehiclesPropertyDetails_CompanyId")
                .OnTable("VehiclesPropertyDetails")
                .OnColumn("CompanyId");
        }

        public override void Down()
        {
            // Remove index
            Delete.Index("IX_VehiclesPropertyDetails_CompanyId").OnTable("VehiclesPropertyDetails");

            // Remove CompanyId column
            Delete.Column("CompanyId").FromTable("VehiclesPropertyDetails");
        }
    }
}
