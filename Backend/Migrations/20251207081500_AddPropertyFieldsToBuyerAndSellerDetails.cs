using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyFieldsToBuyerAndSellerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SAFE MIGRATION: Adding property transaction fields to buyer and seller details
            // These fields (PropertyTypeId, Price, PriceText, RoyaltyAmount, HalfPrice) are being relocated
            // from PropertyDetails to BuyerDetails/SellerDetails to support multiple buyers/sellers with different prices
            // All existing data will be preserved!

            // ===== BuyerDetails (Property) =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""PropertyTypeId"" integer NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""Price"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""PriceText"" text NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""RoyaltyAmount"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""HalfPrice"" double precision NULL;
                    END IF;
                END $$;
            ");

            // ===== SellerDetails (Property) =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""PropertyTypeId"" integer NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""Price"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""PriceText"" text NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""RoyaltyAmount"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""HalfPrice"" double precision NULL;
                    END IF;
                END $$;
            ");

            // ===== VehiclesBuyerDetails =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""PropertyTypeId"" integer NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""Price"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""PriceText"" text NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""RoyaltyAmount"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""HalfPrice"" double precision NULL;
                    END IF;
                END $$;
            ");

            // ===== VehiclesSellerDetails =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""PropertyTypeId"" integer NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""Price"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""PriceText"" text NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""RoyaltyAmount"" double precision NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""HalfPrice"" double precision NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Safe rollback - only remove the columns we added

            // ===== BuyerDetails (Property) =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""PropertyTypeId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""Price"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""PriceText"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""RoyaltyAmount"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""HalfPrice"";
                    END IF;
                END $$;
            ");

            // ===== SellerDetails (Property) =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""PropertyTypeId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""Price"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""PriceText"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""RoyaltyAmount"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""HalfPrice"";
                    END IF;
                END $$;
            ");

            // ===== VehiclesBuyerDetails =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""PropertyTypeId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""Price"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""PriceText"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""RoyaltyAmount"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""HalfPrice"";
                    END IF;
                END $$;
            ");

            // ===== VehiclesSellerDetails =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'PropertyTypeId'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""PropertyTypeId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'Price'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""Price"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'PriceText'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""PriceText"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'RoyaltyAmount'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""RoyaltyAmount"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'HalfPrice'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""HalfPrice"";
                    END IF;
                END $$;
            ");
        }
    }
}
