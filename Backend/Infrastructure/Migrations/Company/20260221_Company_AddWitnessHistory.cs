using FluentMigrator;

namespace WebAPIBackend.Infrastructure.Migrations.Company
{
    /// <summary>
    /// Migration to add witness history tracking fields to Guarantor table
    /// This enables tracking of active/inactive witnesses with full history
    /// </summary>
    [Migration(20260221_001)]
    public class AddWitnessHistoryToGuarantor : Migration
    {
        public override void Up()
        {
            // Add IsActive field (default true for existing records)
            Alter.Table("Guarantor")
                .AddColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

            // Add ExpiredAt field (when witness was replaced)
            Alter.Table("Guarantor")
                .AddColumn("ExpiredAt").AsDateTime().Nullable();

            // Add ExpiredBy field (user who replaced the witness)
            Alter.Table("Guarantor")
                .AddColumn("ExpiredBy").AsString().Nullable();

            // Add ReplacedByGuarantorId field (reference to new witness)
            Alter.Table("Guarantor")
                .AddColumn("ReplacedByGuarantorId").AsInt32().Nullable()
                .ForeignKey("FK_Guarantor_ReplacedBy", "Guarantor", "Id")
                .OnDelete(System.Data.Rule.SetNull);

            // Create index for better query performance
            Create.Index("IX_Guarantor_CompanyId_IsActive")
                .OnTable("Guarantor")
                .OnColumn("CompanyId").Ascending()
                .OnColumn("IsActive").Ascending();

            // For companies with multiple witnesses, keep only the most recent as active
            Execute.Sql(@"
                WITH RankedGuarantors AS (
                    SELECT 
                        ""Id"",
                        ""CompanyId"",
                        ROW_NUMBER() OVER (PARTITION BY ""CompanyId"" ORDER BY ""CreatedAt"" DESC) as rn
                    FROM ""Guarantor""
                    WHERE ""CompanyId"" IS NOT NULL
                )
                UPDATE ""Guarantor"" g
                SET ""IsActive"" = false,
                    ""ExpiredAt"" = NOW()
                FROM RankedGuarantors rg
                WHERE g.""Id"" = rg.""Id"" 
                  AND rg.rn > 1;
            ");
        }

        public override void Down()
        {
            // Drop the index
            Delete.Index("IX_Guarantor_CompanyId_IsActive").OnTable("Guarantor");

            // Drop the foreign key constraint
            Delete.ForeignKey("FK_Guarantor_ReplacedBy").OnTable("Guarantor");

            // Drop the columns
            Delete.Column("ReplacedByGuarantorId").FromTable("Guarantor");
            Delete.Column("ExpiredBy").FromTable("Guarantor");
            Delete.Column("ExpiredAt").FromTable("Guarantor");
            Delete.Column("IsActive").FromTable("Guarantor");
        }
    }
}
