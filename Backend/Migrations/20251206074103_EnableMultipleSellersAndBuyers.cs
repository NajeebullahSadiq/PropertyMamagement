using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class EnableMultipleSellersAndBuyers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SAFE MIGRATION: Enabling support for multiple sellers and buyers
            // This migration removes any unique constraints that might prevent multiple entries
            // The database structure already supports one-to-many relationships via PropertyDetailsId foreign keys
            // All existing data will be preserved!

            // Remove unique constraint on SellerDetails.PropertyDetailsId if it exists (Property module)
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'tr' 
                        AND tablename = 'SellerDetails' 
                        AND indexname = 'SellerDetails_PropertyDetailsId_key'
                    ) THEN
                        DROP INDEX IF EXISTS tr.""SellerDetails_PropertyDetailsId_key"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'tr' 
                        AND tablename = 'SellerDetails' 
                        AND indexname = 'unique_propertyid'
                    ) THEN
                        DROP INDEX IF EXISTS tr.""unique_propertyid"";
                    END IF;
                END $$;
            ");

            // Remove unique constraint on BuyerDetails.PropertyDetailsId if it exists (Property module)
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'tr' 
                        AND tablename = 'BuyerDetails' 
                        AND indexname = 'BuyerDetails_PropertyDetailsId_key'
                    ) THEN
                        DROP INDEX IF EXISTS tr.""BuyerDetails_PropertyDetailsId_key"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'tr' 
                        AND tablename = 'BuyerDetails' 
                        AND indexname = 'unique_propertyid'
                    ) THEN
                        DROP INDEX IF EXISTS tr.""unique_propertyid"";
                    END IF;
                END $$;
            ");

            // Remove unique constraint on VehiclesSellerDetails.PropertyDetailsId if it exists (Vehicle module)
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'tr' 
                        AND tablename = 'VehiclesSellerDetails' 
                        AND indexname = 'VehiclesSellerDetails_PropertyDetailsId_key'
                    ) THEN
                        DROP INDEX IF EXISTS tr.""VehiclesSellerDetails_PropertyDetailsId_key"";
                    END IF;
                END $$;
            ");

            // Remove unique constraint on VehiclesBuyerDetails.PropertyDetailsId if it exists (Vehicle module)
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE schemaname = 'tr' 
                        AND tablename = 'VehiclesBuyerDetails' 
                        AND indexname = 'VehiclesBuyerDetails_PropertyDetailsId_key'
                    ) THEN
                        DROP INDEX IF EXISTS tr.""VehiclesBuyerDetails_PropertyDetailsId_key"";
                    END IF;
                END $$;
            ");

            // Note: The foreign key relationships are already configured correctly in AppDbContext
            // with WithMany() which allows multiple sellers/buyers per property/vehicle
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Re-adding unique constraints is not recommended as it would break the multiple sellers/buyers feature
            // If rollback is absolutely necessary, unique constraints would need to be manually recreated
            // but this would prevent the application from functioning correctly
        }
    }
}
