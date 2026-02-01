using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    [Migration(20260130_01)]
    public class AddCompanyIdToProperty : Migration
    {
        public override void Up()
        {
            // Add CompanyId column to PropertyDetails table
            Alter.Table("PropertyDetails")
                .AddColumn("CompanyId").AsInt32().Nullable();

            // Create index for better query performance
            Create.Index("IX_PropertyDetails_CompanyId")
                .OnTable("PropertyDetails")
                .OnColumn("CompanyId");
        }

        public override void Down()
        {
            // Remove index
            Delete.Index("IX_PropertyDetails_CompanyId").OnTable("PropertyDetails");

            // Remove CompanyId column
            Delete.Column("CompanyId").FromTable("PropertyDetails");
        }
    }
}
