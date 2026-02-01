using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Property
{
    [Migration(20260131000001)]
    public class AddCustomDocumentTypeToProperty : Migration
    {
        public override void Up()
        {
            // Add CustomDocumentType column to PropertyDetails table
            if (!Schema.Schema("tr").Table("PropertyDetails").Column("CustomDocumentType").Exists())
            {
                Alter.Table("PropertyDetails")
                    .InSchema("tr")
                    .AddColumn("CustomDocumentType").AsString().Nullable();
            }
        }

        public override void Down()
        {
            // Remove CustomDocumentType column from PropertyDetails table
            if (Schema.Schema("tr").Table("PropertyDetails").Column("CustomDocumentType").Exists())
            {
                Delete.Column("CustomDocumentType")
                    .FromTable("PropertyDetails")
                    .InSchema("tr");
            }
        }
    }
}
