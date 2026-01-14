using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Migration to split single serial number fields into start/end pairs for all document types
    /// </summary>
    public partial class SplitSerialNumbersToStartEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to add columns with correct schema
            migrationBuilder.Sql(@"
                -- Add new start/end columns for Property Sale
                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""PropertySaleSerialStart"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""PropertySaleSerialEnd"" VARCHAR(100);

                -- Add new start/end columns for Bay Wafa
                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""BayWafaSerialStart"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""BayWafaSerialEnd"" VARCHAR(100);

                -- Add new start/end columns for Rent
                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""RentSerialStart"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""RentSerialEnd"" VARCHAR(100);

                -- Add new start/end columns for Vehicle Sale
                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""VehicleSaleSerialStart"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""VehicleSaleSerialEnd"" VARCHAR(100);

                -- Add new start/end columns for Vehicle Exchange
                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""VehicleExchangeSerialStart"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""VehicleExchangeSerialEnd"" VARCHAR(100);

                -- Migrate existing data if old columns exist
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.columns 
                               WHERE table_schema = 'org' 
                               AND table_name = 'SecuritiesDistribution' 
                               AND column_name = 'PropertySaleSerialNumber') THEN
                        UPDATE org.""SecuritiesDistribution""
                        SET ""PropertySaleSerialStart"" = ""PropertySaleSerialNumber""
                        WHERE ""PropertySaleSerialNumber"" IS NOT NULL;

                        ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN ""PropertySaleSerialNumber"";
                    END IF;

                    IF EXISTS (SELECT 1 FROM information_schema.columns 
                               WHERE table_schema = 'org' 
                               AND table_name = 'SecuritiesDistribution' 
                               AND column_name = 'BayWafaSerialNumber') THEN
                        UPDATE org.""SecuritiesDistribution""
                        SET ""BayWafaSerialStart"" = ""BayWafaSerialNumber""
                        WHERE ""BayWafaSerialNumber"" IS NOT NULL;

                        ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN ""BayWafaSerialNumber"";
                    END IF;

                    IF EXISTS (SELECT 1 FROM information_schema.columns 
                               WHERE table_schema = 'org' 
                               AND table_name = 'SecuritiesDistribution' 
                               AND column_name = 'RentSerialNumber') THEN
                        UPDATE org.""SecuritiesDistribution""
                        SET ""RentSerialStart"" = ""RentSerialNumber""
                        WHERE ""RentSerialNumber"" IS NOT NULL;

                        ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN ""RentSerialNumber"";
                    END IF;

                    IF EXISTS (SELECT 1 FROM information_schema.columns 
                               WHERE table_schema = 'org' 
                               AND table_name = 'SecuritiesDistribution' 
                               AND column_name = 'VehicleSaleSerialNumber') THEN
                        UPDATE org.""SecuritiesDistribution""
                        SET ""VehicleSaleSerialStart"" = ""VehicleSaleSerialNumber""
                        WHERE ""VehicleSaleSerialNumber"" IS NOT NULL;

                        ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN ""VehicleSaleSerialNumber"";
                    END IF;

                    IF EXISTS (SELECT 1 FROM information_schema.columns 
                               WHERE table_schema = 'org' 
                               AND table_name = 'SecuritiesDistribution' 
                               AND column_name = 'VehicleExchangeSerialNumber') THEN
                        UPDATE org.""SecuritiesDistribution""
                        SET ""VehicleExchangeSerialStart"" = ""VehicleExchangeSerialNumber""
                        WHERE ""VehicleExchangeSerialNumber"" IS NOT NULL;

                        ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN ""VehicleExchangeSerialNumber"";
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Re-add old columns
                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""PropertySaleSerialNumber"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""BayWafaSerialNumber"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""RentSerialNumber"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""VehicleSaleSerialNumber"" VARCHAR(100);

                ALTER TABLE org.""SecuritiesDistribution"" 
                ADD COLUMN IF NOT EXISTS ""VehicleExchangeSerialNumber"" VARCHAR(100);

                -- Migrate data back
                UPDATE org.""SecuritiesDistribution""
                SET ""PropertySaleSerialNumber"" = ""PropertySaleSerialStart""
                WHERE ""PropertySaleSerialStart"" IS NOT NULL;

                UPDATE org.""SecuritiesDistribution""
                SET ""BayWafaSerialNumber"" = ""BayWafaSerialStart""
                WHERE ""BayWafaSerialStart"" IS NOT NULL;

                UPDATE org.""SecuritiesDistribution""
                SET ""RentSerialNumber"" = ""RentSerialStart""
                WHERE ""RentSerialStart"" IS NOT NULL;

                UPDATE org.""SecuritiesDistribution""
                SET ""VehicleSaleSerialNumber"" = ""VehicleSaleSerialStart""
                WHERE ""VehicleSaleSerialStart"" IS NOT NULL;

                UPDATE org.""SecuritiesDistribution""
                SET ""VehicleExchangeSerialNumber"" = ""VehicleExchangeSerialStart""
                WHERE ""VehicleExchangeSerialStart"" IS NOT NULL;

                -- Drop new columns
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""PropertySaleSerialStart"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""PropertySaleSerialEnd"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""BayWafaSerialStart"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""BayWafaSerialEnd"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""RentSerialStart"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""RentSerialEnd"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""VehicleSaleSerialStart"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""VehicleSaleSerialEnd"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""VehicleExchangeSerialStart"";
                ALTER TABLE org.""SecuritiesDistribution"" DROP COLUMN IF EXISTS ""VehicleExchangeSerialEnd"";
            ");
        }
    }
}
