using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add financial and administrative fields to LicenseDetails table.
    /// New fields:
    /// - RoyaltyAmount (مبلغ حق‌الامتیاز): License fee amount
    /// - RoyaltyDate (تاریخ حق‌الامتیاز): License fee date
    /// - PenaltyAmount (مبلغ جریمه): Penalty amount
    /// - PenaltyDate (تاریخ جریمه): Penalty date
    /// - HrLetter (مکتوب قوای بشری): HR letter reference number
    /// - HrLetterDate (تاریخ مکتوب قوای بشری): HR letter date
    /// </summary>
    public partial class AddFinancialAdminFieldsToLicenseDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add RoyaltyAmount column if not exists (مبلغ حق‌الامتیاز)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RoyaltyAmount') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RoyaltyAmount"" NUMERIC(18,2) NULL;
                    END IF;
                END $$;
            ");

            // Add RoyaltyDate column if not exists (تاریخ حق‌الامتیاز)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RoyaltyDate') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RoyaltyDate"" DATE NULL;
                    END IF;
                END $$;
            ");

            // Add PenaltyAmount column if not exists (مبلغ جریمه)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'PenaltyAmount') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""PenaltyAmount"" NUMERIC(18,2) NULL;
                    END IF;
                END $$;
            ");

            // Add PenaltyDate column if not exists (تاریخ جریمه)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'PenaltyDate') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""PenaltyDate"" DATE NULL;
                    END IF;
                END $$;
            ");

            // Add HrLetter column if not exists (مکتوب قوای بشری)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'HrLetter') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""HrLetter"" VARCHAR(255) NULL;
                    END IF;
                END $$;
            ");

            // Add HrLetterDate column if not exists (تاریخ مکتوب قوای بشری)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'HrLetterDate') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""HrLetterDate"" DATE NULL;
                    END IF;
                END $$;
            ");

            // Add comments to document the fields
            migrationBuilder.Sql(@"
                COMMENT ON COLUMN org.""LicenseDetails"".""RoyaltyAmount"" IS 'مبلغ حق‌الامتیاز - Royalty/License Fee Amount';
                COMMENT ON COLUMN org.""LicenseDetails"".""RoyaltyDate"" IS 'تاریخ حق‌الامتیاز - Royalty/License Fee Date';
                COMMENT ON COLUMN org.""LicenseDetails"".""PenaltyAmount"" IS 'مبلغ جریمه - Penalty Amount';
                COMMENT ON COLUMN org.""LicenseDetails"".""PenaltyDate"" IS 'تاریخ جریمه - Penalty Date';
                COMMENT ON COLUMN org.""LicenseDetails"".""HrLetter"" IS 'مکتوب قوای بشری - HR Letter Reference Number';
                COMMENT ON COLUMN org.""LicenseDetails"".""HrLetterDate"" IS 'تاریخ مکتوب قوای بشری - HR Letter Date';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE org.""LicenseDetails"" DROP COLUMN IF EXISTS ""RoyaltyAmount"";
                ALTER TABLE org.""LicenseDetails"" DROP COLUMN IF EXISTS ""RoyaltyDate"";
                ALTER TABLE org.""LicenseDetails"" DROP COLUMN IF EXISTS ""PenaltyAmount"";
                ALTER TABLE org.""LicenseDetails"" DROP COLUMN IF EXISTS ""PenaltyDate"";
                ALTER TABLE org.""LicenseDetails"" DROP COLUMN IF EXISTS ""HrLetter"";
                ALTER TABLE org.""LicenseDetails"" DROP COLUMN IF EXISTS ""HrLetterDate"";
            ");
        }
    }
}
