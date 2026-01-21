using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.ActivityMonitoring
{
    [Migration(20260121000001)]
    public class ActivityMonitoring_Initial : Migration
    {
        public override void Up()
        {
            // Create ActivityMonitoringRecords table
            Create.Table("ActivityMonitoringRecords")
                .InSchema("org")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("LicenseHolderName").AsString(200).NotNullable()
                .WithColumn("TaxClearanceStatus").AsString(100).Nullable()
                .WithColumn("TaxClearanceLetterNumber").AsString(100).Nullable()
                .WithColumn("TaxClearanceDate").AsDate().Nullable()
                .WithColumn("PaidTaxAmount").AsDecimal(18, 2).Nullable()
                .WithColumn("ReportRegistrationDate").AsDate().Nullable()
                .WithColumn("SaleDeedsCount").AsInt32().Nullable()
                .WithColumn("RentalDeedsCount").AsInt32().Nullable()
                .WithColumn("BaiUlWafaDeedsCount").AsInt32().Nullable()
                .WithColumn("VehicleTransactionDeedsCount").AsInt32().Nullable()
                .WithColumn("CancelledMixedTransactions").AsInt32().Nullable()
                .WithColumn("LostDeedsCount").AsInt32().Nullable()
                .WithColumn("AnnualReportRemarks").AsString(1000).Nullable()
                .WithColumn("InspectionDate").AsDate().Nullable()
                .WithColumn("InspectedRealEstateOfficesCount").AsInt32().Nullable()
                .WithColumn("SealedOfficesCount").AsInt32().Nullable()
                .WithColumn("InspectedPetitionWritersCount").AsInt32().Nullable()
                .WithColumn("ViolatingPetitionWritersCount").AsInt32().Nullable()
                .WithColumn("Status").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("CreatedAt").AsDateTime().Nullable()
                .WithColumn("CreatedBy").AsString(50).Nullable()
                .WithColumn("UpdatedAt").AsDateTime().Nullable()
                .WithColumn("UpdatedBy").AsString(50).Nullable();

            // Create indexes
            Create.Index("IX_ActivityMonitoringRecords_LicenseHolderName")
                .OnTable("ActivityMonitoringRecords").InSchema("org")
                .OnColumn("LicenseHolderName");

            Create.Index("IX_ActivityMonitoringRecords_TaxClearanceDate")
                .OnTable("ActivityMonitoringRecords").InSchema("org")
                .OnColumn("TaxClearanceDate");

            Create.Index("IX_ActivityMonitoringRecords_InspectionDate")
                .OnTable("ActivityMonitoringRecords").InSchema("org")
                .OnColumn("InspectionDate");

            // Create ActivityMonitoringComplaints table
            Create.Table("ActivityMonitoringComplaints")
                .InSchema("org")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("ActivityMonitoringRecordId").AsInt32().NotNullable()
                .WithColumn("ComplaintSerialNumber").AsString(50).NotNullable()
                .WithColumn("ComplainantName").AsString(200).NotNullable()
                .WithColumn("ComplaintSubject").AsString(500).NotNullable()
                .WithColumn("ComplaintRegistrationDate").AsDate().Nullable()
                .WithColumn("AccusedPartyName").AsString(200).NotNullable()
                .WithColumn("ActionsTaken").AsString(1000).Nullable()
                .WithColumn("Remarks").AsString(1000).Nullable()
                .WithColumn("CreatedAt").AsDateTime().Nullable()
                .WithColumn("CreatedBy").AsString(50).Nullable();

            // Create foreign key
            Create.ForeignKey("FK_ActivityMonitoringComplaints_ActivityMonitoringRecordId")
                .FromTable("ActivityMonitoringComplaints").InSchema("org").ForeignColumn("ActivityMonitoringRecordId")
                .ToTable("ActivityMonitoringRecords").InSchema("org").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            // Create indexes
            Create.Index("IX_ActivityMonitoringComplaints_ActivityMonitoringRecordId")
                .OnTable("ActivityMonitoringComplaints").InSchema("org")
                .OnColumn("ActivityMonitoringRecordId");

            Create.Index("IX_ActivityMonitoringComplaints_ComplaintSerialNumber")
                .OnTable("ActivityMonitoringComplaints").InSchema("org")
                .OnColumn("ComplaintSerialNumber");

            // Create ActivityMonitoringRealEstateViolations table
            Create.Table("ActivityMonitoringRealEstateViolations")
                .InSchema("org")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("ActivityMonitoringRecordId").AsInt32().NotNullable()
                .WithColumn("ViolationSerialNumber").AsString(50).NotNullable()
                .WithColumn("LicenseHolderName").AsString(200).NotNullable()
                .WithColumn("ViolationType").AsString(500).NotNullable()
                .WithColumn("ViolationDate").AsDate().Nullable()
                .WithColumn("ActionsTaken").AsString(1000).Nullable()
                .WithColumn("Remarks").AsString(1000).Nullable()
                .WithColumn("CreatedAt").AsDateTime().Nullable()
                .WithColumn("CreatedBy").AsString(50).Nullable();

            // Create foreign key
            Create.ForeignKey("FK_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecordId")
                .FromTable("ActivityMonitoringRealEstateViolations").InSchema("org").ForeignColumn("ActivityMonitoringRecordId")
                .ToTable("ActivityMonitoringRecords").InSchema("org").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            // Create indexes
            Create.Index("IX_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecordId")
                .OnTable("ActivityMonitoringRealEstateViolations").InSchema("org")
                .OnColumn("ActivityMonitoringRecordId");

            Create.Index("IX_ActivityMonitoringRealEstateViolations_ViolationSerialNumber")
                .OnTable("ActivityMonitoringRealEstateViolations").InSchema("org")
                .OnColumn("ViolationSerialNumber");

            // Create ActivityMonitoringPetitionWriterViolations table
            Create.Table("ActivityMonitoringPetitionWriterViolations")
                .InSchema("org")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("ActivityMonitoringRecordId").AsInt32().NotNullable()
                .WithColumn("ViolationSerialNumber").AsString(50).NotNullable()
                .WithColumn("PetitionWriterName").AsString(200).NotNullable()
                .WithColumn("ViolationType").AsString(500).NotNullable()
                .WithColumn("ViolationDate").AsDate().Nullable()
                .WithColumn("ActionsTaken").AsString(1000).Nullable()
                .WithColumn("Remarks").AsString(1000).Nullable()
                .WithColumn("CreatedAt").AsDateTime().Nullable()
                .WithColumn("CreatedBy").AsString(50).Nullable();

            // Create foreign key
            Create.ForeignKey("FK_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecordId")
                .FromTable("ActivityMonitoringPetitionWriterViolations").InSchema("org").ForeignColumn("ActivityMonitoringRecordId")
                .ToTable("ActivityMonitoringRecords").InSchema("org").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

            // Create indexes
            Create.Index("IX_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecordId")
                .OnTable("ActivityMonitoringPetitionWriterViolations").InSchema("org")
                .OnColumn("ActivityMonitoringRecordId");

            Create.Index("IX_ActivityMonitoringPetitionWriterViolations_ViolationSerialNumber")
                .OnTable("ActivityMonitoringPetitionWriterViolations").InSchema("org")
                .OnColumn("ViolationSerialNumber");
        }

        public override void Down()
        {
            Delete.Table("ActivityMonitoringPetitionWriterViolations").InSchema("org");
            Delete.Table("ActivityMonitoringRealEstateViolations").InSchema("org");
            Delete.Table("ActivityMonitoringComplaints").InSchema("org");
            Delete.Table("ActivityMonitoringRecords").InSchema("org");
        }
    }
}
