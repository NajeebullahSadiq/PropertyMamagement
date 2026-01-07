using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Consolidated migration to apply all pending schema changes:
    /// 1. CompanyAccountInfo table
    /// 2. CompanyCancellationInfo table  
    /// 3. Guarantee type conditional fields on Guarantors table
    /// </summary>
    public partial class ApplyPendingSchemaChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create CompanyAccountInfo table if not exists
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS org.""CompanyAccountInfo"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""CompanyId"" INTEGER NOT NULL,
                    ""SettlementInfo"" VARCHAR(500) NULL,
                    ""TaxPaymentAmount"" NUMERIC(18,2) NOT NULL DEFAULT 0,
                    ""SettlementYear"" INTEGER NULL,
                    ""TaxPaymentDate"" DATE NULL,
                    ""TransactionCount"" INTEGER NULL,
                    ""CompanyCommission"" NUMERIC(18,2) NULL,
                    ""CreatedAt"" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""CreatedBy"" VARCHAR(50) NULL,
                    ""Status"" BOOLEAN DEFAULT TRUE
                );
            ");

            // Add foreign key for CompanyAccountInfo if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_CompanyAccountInfo_CompanyDetails'
                    ) THEN
                        ALTER TABLE org.""CompanyAccountInfo"" 
                        ADD CONSTRAINT ""FK_CompanyAccountInfo_CompanyDetails"" 
                        FOREIGN KEY (""CompanyId"") 
                        REFERENCES org.""CompanyDetails""(""Id"") ON DELETE CASCADE;
                    END IF;
                END $$;
            ");

            // Create index for CompanyAccountInfo
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_CompanyAccountInfo_CompanyId"" 
                ON org.""CompanyAccountInfo"" (""CompanyId"");
            ");

            // 2. Create CompanyCancellationInfo table if not exists
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS org.""CompanyCancellationInfo"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""CompanyId"" INTEGER NOT NULL,
                    ""LicenseCancellationLetterNumber"" VARCHAR(100) NULL,
                    ""RevenueCancellationLetterNumber"" VARCHAR(100) NULL,
                    ""LicenseCancellationLetterDate"" DATE NULL,
                    ""Remarks"" VARCHAR(1000) NULL,
                    ""CreatedAt"" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""CreatedBy"" VARCHAR(50) NULL,
                    ""Status"" BOOLEAN DEFAULT TRUE
                );
            ");

            // Add foreign key for CompanyCancellationInfo if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_CompanyCancellationInfo_CompanyDetails'
                    ) THEN
                        ALTER TABLE org.""CompanyCancellationInfo"" 
                        ADD CONSTRAINT ""FK_CompanyCancellationInfo_CompanyDetails"" 
                        FOREIGN KEY (""CompanyId"") 
                        REFERENCES org.""CompanyDetails""(""Id"") ON DELETE CASCADE;
                    END IF;
                END $$;
            ");

            // Create index for CompanyCancellationInfo
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_CompanyCancellationInfo_CompanyId"" 
                ON org.""CompanyCancellationInfo"" (""CompanyId"");
            ");

            // 3. Add Guarantee Type Conditional Fields to Guarantors table
            // First add GrandFatherName if missing (from earlier migration)
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""GrandFatherName"" VARCHAR(100) NULL;
            ");
            
            // Sharia Deed fields
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""CourtName"" VARCHAR(255) NULL;
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""CollateralNumber"" VARCHAR(100) NULL;
            ");

            // Customary Deed fields
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""SetSerialNumber"" VARCHAR(100) NULL;
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""GuaranteeDistrictId"" INTEGER NULL;
            ");

            // Cash fields
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""BankName"" VARCHAR(255) NULL;
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""DepositNumber"" VARCHAR(100) NULL;
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" ADD COLUMN IF NOT EXISTS ""DepositDate"" DATE NULL;
            ");

            // Add index for GuaranteeDistrictId
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_Guarantors_GuaranteeDistrictId"" 
                ON org.""Guarantors"" (""GuaranteeDistrictId"");
            ");

            // Add foreign key for GuaranteeDistrict if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'Guarantors_GuaranteeDistrictId_fkey'
                    ) THEN
                        ALTER TABLE org.""Guarantors"" 
                        ADD CONSTRAINT ""Guarantors_GuaranteeDistrictId_fkey"" 
                        FOREIGN KEY (""GuaranteeDistrictId"") 
                        REFERENCES look.""Location""(""ID"") ON DELETE SET NULL;
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Guarantors conditional fields
            migrationBuilder.Sql(@"
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""DepositDate"";
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""DepositNumber"";
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""BankName"";
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""GuaranteeDistrictId"";
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""SetSerialNumber"";
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""CollateralNumber"";
                ALTER TABLE org.""Guarantors"" DROP COLUMN IF EXISTS ""CourtName"";
            ");

            // Drop tables
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS org.""CompanyCancellationInfo"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS org.""CompanyAccountInfo"";");
        }
    }
}
