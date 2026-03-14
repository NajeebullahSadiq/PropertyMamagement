using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterSecurities;

/// <summary>
/// Migration to add DeliveryDate field to PetitionWriterSecurities table
/// تاریخ تحویلی اویز - Delivery Date
/// </summary>
[Migration(20260314001)]
public class AddDeliveryDateToPetitionWriterSecurities : Migration
{
    public override void Up()
    {
        // Add DeliveryDate column
        if (!Schema.Schema("org").Table("PetitionWriterSecurities").Column("DeliveryDate").Exists())
        {
            Alter.Table("PetitionWriterSecurities").InSchema("org")
                .AddColumn("DeliveryDate").AsDate().NotNullable().WithDefaultValue(new DateTime(1900, 1, 1));
        }

        // Update existing records to set DeliveryDate same as DistributionDate
        Execute.Sql(@"
            UPDATE org.""PetitionWriterSecurities""
            SET ""DeliveryDate"" = ""DistributionDate""
            WHERE ""DeliveryDate"" = '1900-01-01'
        ");
    }

    public override void Down()
    {
        // Remove DeliveryDate column
        if (Schema.Schema("org").Table("PetitionWriterSecurities").Column("DeliveryDate").Exists())
        {
            Delete.Column("DeliveryDate").FromTable("PetitionWriterSecurities").InSchema("org");
        }
    }
}
