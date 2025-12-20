using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPIBackend.Configuration;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20251218080000_AddMissingPropertyAndBuyerFields")]
    public partial class AddMissingPropertyAndBuyerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Add DocumentType to PropertyDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'DocumentType'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        ADD COLUMN ""DocumentType"" text NULL;
                    END IF;

                    -- Add IssuanceNumber to PropertyDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'IssuanceNumber'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        ADD COLUMN ""IssuanceNumber"" text NULL;
                    END IF;

                    -- Add IssuanceDate to PropertyDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'IssuanceDate'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        ADD COLUMN ""IssuanceDate"" timestamp with time zone NULL;
                    END IF;

                    -- Add SerialNumber to PropertyDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'SerialNumber'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        ADD COLUMN ""SerialNumber"" text NULL;
                    END IF;

                    -- Add TransactionDate to PropertyDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'TransactionDate'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        ADD COLUMN ""TransactionDate"" timestamp with time zone NULL;
                    END IF;

                    -- Add CustomPropertyType to BuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        ADD COLUMN ""CustomPropertyType"" text NULL;
                    END IF;

                    -- Add CustomPropertyType to SellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        ADD COLUMN ""CustomPropertyType"" text NULL;
                    END IF;

                    -- Add CustomPropertyType to VehiclesBuyerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        ADD COLUMN ""CustomPropertyType"" text NULL;
                    END IF;

                    -- Add CustomPropertyType to VehiclesSellerDetails if not exists
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        ADD COLUMN ""CustomPropertyType"" text NULL;
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
                    -- Remove DocumentType from PropertyDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'DocumentType'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        DROP COLUMN ""DocumentType"";
                    END IF;

                    -- Remove IssuanceNumber from PropertyDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'IssuanceNumber'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        DROP COLUMN ""IssuanceNumber"";
                    END IF;

                    -- Remove IssuanceDate from PropertyDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'IssuanceDate'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        DROP COLUMN ""IssuanceDate"";
                    END IF;

                    -- Remove SerialNumber from PropertyDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'SerialNumber'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        DROP COLUMN ""SerialNumber"";
                    END IF;

                    -- Remove TransactionDate from PropertyDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'PropertyDetails' 
                        AND column_name = 'TransactionDate'
                    ) THEN
                        ALTER TABLE tr.""PropertyDetails"" 
                        DROP COLUMN ""TransactionDate"";
                    END IF;

                    -- Remove CustomPropertyType from BuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" 
                        DROP COLUMN ""CustomPropertyType"";
                    END IF;

                    -- Remove CustomPropertyType from SellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" 
                        DROP COLUMN ""CustomPropertyType"";
                    END IF;

                    -- Remove CustomPropertyType from VehiclesBuyerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" 
                        DROP COLUMN ""CustomPropertyType"";
                    END IF;

                    -- Remove CustomPropertyType from VehiclesSellerDetails if exists
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'CustomPropertyType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" 
                        DROP COLUMN ""CustomPropertyType"";
                    END IF;
                END $$;
            ");
        }
    }
}
