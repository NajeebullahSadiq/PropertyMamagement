using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.PetitionWriterLicense
{
    [Migration(20260209000001)]
    public class AddNewFieldsToPetitionWriter : Migration
    {
        public override void Up()
        {
            // Add new fields to PetitionWriterLicenses table
            Alter.Table("PetitionWriterLicenses")
                .InSchema("org")
                .AddColumn("MobileNumber").AsString(20).Nullable()
                .AddColumn("Competency").AsString(50).Nullable() // اهلیت: اعلی, اوسط, ادنی
                .AddColumn("District").AsString(200).Nullable() // ناحیه
                .AddColumn("LicensePrice").AsDecimal(18, 2).Nullable() // قیمت جواز
                .AddColumn("DetailedAddress").AsString(1000).Nullable(); // ادرس دقیق محل فعالیت
        }

        public override void Down()
        {
            Delete.Column("MobileNumber").FromTable("PetitionWriterLicenses").InSchema("org");
            Delete.Column("Competency").FromTable("PetitionWriterLicenses").InSchema("org");
            Delete.Column("District").FromTable("PetitionWriterLicenses").InSchema("org");
            Delete.Column("LicensePrice").FromTable("PetitionWriterLicenses").InSchema("org");
            Delete.Column("DetailedAddress").FromTable("PetitionWriterLicenses").InSchema("org");
        }
    }
}
