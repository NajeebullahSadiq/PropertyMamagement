using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPIBackend.Configuration;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20251221_AddTazkiraFields")]
    public partial class AddTazkiraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Add TazkiraType to SellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;

                    -- Add TazkiraVolume to SellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;

                    -- Add TazkiraPage to SellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;

                    -- Add TazkiraNumber to SellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;

                    -- Add TazkiraType to BuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;

                    -- Add TazkiraVolume to BuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;

                    -- Add TazkiraPage to BuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;

                    -- Add TazkiraNumber to BuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;

                    -- Add TazkiraType to WitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;

                    -- Add TazkiraVolume to WitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;

                    -- Add TazkiraPage to WitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;

                    -- Add TazkiraNumber to WitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;

                    -- Add TazkiraType to VehiclesSellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;

                    -- Add TazkiraVolume to VehiclesSellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;

                    -- Add TazkiraPage to VehiclesSellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;

                    -- Add TazkiraNumber to VehiclesSellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;

                    -- Add TazkiraType to VehiclesBuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;

                    -- Add TazkiraVolume to VehiclesBuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;

                    -- Add TazkiraPage to VehiclesBuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;

                    -- Add TazkiraNumber to VehiclesBuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;

                    -- Add TazkiraType to VehiclesWitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;

                    -- Add TazkiraVolume to VehiclesWitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;

                    -- Add TazkiraPage to VehiclesWitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;

                    -- Add TazkiraNumber to VehiclesWitnessDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Remove TazkiraType from SellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        DROP COLUMN ""TazkiraType"";
                    END IF;

                    -- Remove TazkiraVolume from SellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        DROP COLUMN ""TazkiraVolume"";
                    END IF;

                    -- Remove TazkiraPage from SellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        DROP COLUMN ""TazkiraPage"";
                    END IF;

                    -- Remove TazkiraNumber from SellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        DROP COLUMN ""TazkiraNumber"";
                    END IF;

                    -- Remove TazkiraType from BuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        DROP COLUMN ""TazkiraType"";
                    END IF;

                    -- Remove TazkiraVolume from BuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        DROP COLUMN ""TazkiraVolume"";
                    END IF;

                    -- Remove TazkiraPage from BuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        DROP COLUMN ""TazkiraPage"";
                    END IF;

                    -- Remove TazkiraNumber from BuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        DROP COLUMN ""TazkiraNumber"";
                    END IF;

                    -- Remove TazkiraType from WitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        DROP COLUMN ""TazkiraType"";
                    END IF;

                    -- Remove TazkiraVolume from WitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        DROP COLUMN ""TazkiraVolume"";
                    END IF;

                    -- Remove TazkiraPage from WitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        DROP COLUMN ""TazkiraPage"";
                    END IF;

                    -- Remove TazkiraNumber from WitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" 
                        DROP COLUMN ""TazkiraNumber"";
                    END IF;

                    -- Remove TazkiraType from VehiclesSellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        DROP COLUMN ""TazkiraType"";
                    END IF;

                    -- Remove TazkiraVolume from VehiclesSellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        DROP COLUMN ""TazkiraVolume"";
                    END IF;

                    -- Remove TazkiraPage from VehiclesSellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        DROP COLUMN ""TazkiraPage"";
                    END IF;

                    -- Remove TazkiraNumber from VehiclesSellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        DROP COLUMN ""TazkiraNumber"";
                    END IF;

                    -- Remove TazkiraType from VehiclesBuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        DROP COLUMN ""TazkiraType"";
                    END IF;

                    -- Remove TazkiraVolume from VehiclesBuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        DROP COLUMN ""TazkiraVolume"";
                    END IF;

                    -- Remove TazkiraPage from VehiclesBuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        DROP COLUMN ""TazkiraPage"";
                    END IF;

                    -- Remove TazkiraNumber from VehiclesBuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        DROP COLUMN ""TazkiraNumber"";
                    END IF;

                    -- Remove TazkiraType from VehiclesWitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        DROP COLUMN ""TazkiraType"";
                    END IF;

                    -- Remove TazkiraVolume from VehiclesWitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        DROP COLUMN ""TazkiraVolume"";
                    END IF;

                    -- Remove TazkiraPage from VehiclesWitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        DROP COLUMN ""TazkiraPage"";
                    END IF;

                    -- Remove TazkiraNumber from VehiclesWitnessDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" 
                        DROP COLUMN ""TazkiraNumber"";
                    END IF;
                END $$;
            ");
        }
    }
}
