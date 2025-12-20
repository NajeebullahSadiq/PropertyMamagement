using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAPIBackend.Configuration;

#nullable disable

namespace WebAPIBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251220121500_AddBuyerTransactionTypeColumns")]
    public partial class AddBuyerTransactionTypeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- BuyerDetails
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'TransactionType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails""
                        ADD COLUMN ""TransactionType"" text NULL;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'TransactionTypeDescription'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails""
                        ADD COLUMN ""TransactionTypeDescription"" text NULL;
                    END IF;

                    -- VehiclesBuyerDetails
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'VehiclesBuyerDetails'
                          AND column_name = 'TransactionType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails""
                        ADD COLUMN ""TransactionType"" text NULL;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'VehiclesBuyerDetails'
                          AND column_name = 'TransactionTypeDescription'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails""
                        ADD COLUMN ""TransactionTypeDescription"" text NULL;
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- BuyerDetails
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'TransactionTypeDescription'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails""
                        DROP COLUMN ""TransactionTypeDescription"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'BuyerDetails'
                          AND column_name = 'TransactionType'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails""
                        DROP COLUMN ""TransactionType"";
                    END IF;

                    -- VehiclesBuyerDetails
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'VehiclesBuyerDetails'
                          AND column_name = 'TransactionTypeDescription'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails""
                        DROP COLUMN ""TransactionTypeDescription"";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'tr'
                          AND table_name = 'VehiclesBuyerDetails'
                          AND column_name = 'TransactionType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails""
                        DROP COLUMN ""TransactionType"";
                    END IF;
                END $$;
            ");
        }
    }
}
