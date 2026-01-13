using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to add SecuritiesDistribution table for اسناد بهادار رهنمای معاملات
    /// </summary>
    public partial class AddSecuritiesDistribution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS org.""SecuritiesDistribution"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    
                    -- Tab 1: Transaction Guide Details
                    ""RegistrationNumber"" VARCHAR(50) NOT NULL,
                    ""LicenseOwnerName"" VARCHAR(200) NOT NULL,
                    ""LicenseOwnerFatherName"" VARCHAR(200) NOT NULL,
                    ""TransactionGuideName"" VARCHAR(200) NOT NULL,
                    ""LicenseNumber"" VARCHAR(50) NOT NULL,
                    
                    -- Tab 2: Document Distribution Details
                    ""DocumentType"" INTEGER,
                    ""PropertySubType"" INTEGER,
                    ""VehicleSubType"" INTEGER,
                    
                    -- Property Document Fields
                    ""PropertySaleCount"" INTEGER,
                    ""PropertySaleSerialNumber"" VARCHAR(100),
                    ""BayWafaCount"" INTEGER,
                    ""BayWafaSerialNumber"" VARCHAR(100),
                    ""RentCount"" INTEGER,
                    ""RentSerialNumber"" VARCHAR(100),
                    
                    -- Vehicle Document Fields
                    ""VehicleSaleCount"" INTEGER,
                    ""VehicleSaleSerialNumber"" VARCHAR(100),
                    ""VehicleExchangeCount"" INTEGER,
                    ""VehicleExchangeSerialNumber"" VARCHAR(100),
                    
                    -- Registration Book Fields
                    ""RegistrationBookType"" INTEGER,
                    ""RegistrationBookCount"" INTEGER,
                    ""DuplicateBookCount"" INTEGER,
                    
                    -- Tab 3: Securities Pricing
                    ""PricePerDocument"" DECIMAL(18, 2),
                    ""TotalDocumentsPrice"" DECIMAL(18, 2),
                    ""RegistrationBookPrice"" DECIMAL(18, 2),
                    ""TotalSecuritiesPrice"" DECIMAL(18, 2),
                    
                    -- Tab 4: Bank Receipt and Distribution Dates
                    ""BankReceiptNumber"" VARCHAR(100),
                    ""DeliveryDate"" DATE,
                    ""DistributionDate"" DATE,
                    
                    -- Audit Fields
                    ""CreatedAt"" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""CreatedBy"" VARCHAR(50),
                    ""UpdatedAt"" TIMESTAMP WITHOUT TIME ZONE,
                    ""UpdatedBy"" VARCHAR(50),
                    ""Status"" BOOLEAN DEFAULT TRUE
                );

                -- Create unique index on RegistrationNumber
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_SecuritiesDistribution_RegistrationNumber"" 
                ON org.""SecuritiesDistribution"" (""RegistrationNumber"");

                -- Create index for search on LicenseNumber
                CREATE INDEX IF NOT EXISTS ""IX_SecuritiesDistribution_LicenseNumber"" 
                ON org.""SecuritiesDistribution"" (""LicenseNumber"");

                -- Create index for search on BankReceiptNumber
                CREATE INDEX IF NOT EXISTS ""IX_SecuritiesDistribution_BankReceiptNumber"" 
                ON org.""SecuritiesDistribution"" (""BankReceiptNumber"");

                -- Create index for search on TransactionGuideName
                CREATE INDEX IF NOT EXISTS ""IX_SecuritiesDistribution_TransactionGuideName"" 
                ON org.""SecuritiesDistribution"" (""TransactionGuideName"");
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS org.""SecuritiesDistribution"";");
        }
    }
}
