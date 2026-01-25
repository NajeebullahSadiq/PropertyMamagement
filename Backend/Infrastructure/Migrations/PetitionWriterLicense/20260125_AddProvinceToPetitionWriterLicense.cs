using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    [Migration(20260125120000)]
    public class AddProvinceToPetitionWriterLicense : Migration
    {
        public override void Up()
        {
            // Add ProvinceId column to PetitionWriterLicenses table
            if (!Schema.Schema("org").Table("PetitionWriterLicenses").Column("ProvinceId").Exists())
            {
                Alter.Table("PetitionWriterLicenses")
                    .InSchema("org")
                    .AddColumn("ProvinceId").AsInt32().Nullable();

                // Add foreign key to Locations table
                Create.ForeignKey("FK_PetitionWriterLicenses_Province")
                    .FromTable("PetitionWriterLicenses").InSchema("org").ForeignColumn("ProvinceId")
                    .ToTable("Locations").InSchema("shared").PrimaryColumn("Id");

                // Add index for better query performance
                Create.Index("IX_PetitionWriterLicenses_ProvinceId")
                    .OnTable("PetitionWriterLicenses").InSchema("org")
                    .OnColumn("ProvinceId");

                // Add index on LicenseNumber for faster lookups
                Create.Index("IX_PetitionWriterLicenses_LicenseNumber")
                    .OnTable("PetitionWriterLicenses").InSchema("org")
                    .OnColumn("LicenseNumber");
            }
        }

        public override void Down()
        {
            // Drop indexes
            if (Schema.Schema("org").Table("PetitionWriterLicenses").Index("IX_PetitionWriterLicenses_LicenseNumber").Exists())
            {
                Delete.Index("IX_PetitionWriterLicenses_LicenseNumber")
                    .OnTable("PetitionWriterLicenses").InSchema("org");
            }

            if (Schema.Schema("org").Table("PetitionWriterLicenses").Index("IX_PetitionWriterLicenses_ProvinceId").Exists())
            {
                Delete.Index("IX_PetitionWriterLicenses_ProvinceId")
                    .OnTable("PetitionWriterLicenses").InSchema("org");
            }

            // Drop foreign key
            if (Schema.Schema("org").Table("PetitionWriterLicenses").Constraint("FK_PetitionWriterLicenses_Province").Exists())
            {
                Delete.ForeignKey("FK_PetitionWriterLicenses_Province")
                    .OnTable("PetitionWriterLicenses").InSchema("org");
            }

            // Drop column
            if (Schema.Schema("org").Table("PetitionWriterLicenses").Column("ProvinceId").Exists())
            {
                Delete.Column("ProvinceId")
                    .FromTable("PetitionWriterLicenses").InSchema("org");
            }
        }
    }
}
