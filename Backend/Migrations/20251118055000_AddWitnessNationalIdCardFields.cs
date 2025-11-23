using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddWitnessNationalIdCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SAFE MIGRATION: Only adding new columns to witness tables
            // This migration DOES NOT affect any lookup/dropdown tables:
            // - PropertyTypes, TransactionTypes, EducationLevels
            // - IdentityCardTypes, AddressTypes, GuaranteeTypes
            // - PunitTypes, Areas, ViolationTypes, LostdocumentsTypes
            // - Locations (provinces and districts)
            // All existing data will be preserved!

            // Check if columns already exist to prevent errors
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'NationalIdCard'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" ADD COLUMN ""NationalIdCard"" text NULL;
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'NationalIdCard'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" ADD COLUMN ""NationalIdCard"" text NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Safe rollback - only remove the columns we added
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'WitnessDetails' 
                        AND column_name = 'NationalIdCard'
                    ) THEN
                        ALTER TABLE tr.""WitnessDetails"" DROP COLUMN ""NationalIdCard"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesWitnessDetails' 
                        AND column_name = 'NationalIdCard'
                    ) THEN
                        ALTER TABLE tr.""VehiclesWitnessDetails"" DROP COLUMN ""NationalIdCard"";
                    END IF;
                END $$;
            ");
        }
    }
}
