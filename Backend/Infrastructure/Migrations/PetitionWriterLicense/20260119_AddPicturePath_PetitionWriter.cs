using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    [Migration(20260119000001, "Add PicturePath to PetitionWriterLicenses")]
    public class AddPicturePath_PetitionWriter : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("org").Table("PetitionWriterLicenses").Column("PicturePath").Exists())
            {
                Alter.Table("PetitionWriterLicenses")
                    .InSchema("org")
                    .AddColumn("PicturePath").AsString(500).Nullable();
            }
        }

        public override void Down()
        {
            if (Schema.Schema("org").Table("PetitionWriterLicenses").Column("PicturePath").Exists())
            {
                Delete.Column("PicturePath")
                    .FromTable("PetitionWriterLicenses")
                    .InSchema("org");
            }
        }
    }
}
