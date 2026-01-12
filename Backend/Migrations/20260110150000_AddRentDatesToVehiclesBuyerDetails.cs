using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRentDatesToVehiclesBuyerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add NationalIdCardPath to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'NationalIdCardPath'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""NationalIdCardPath"" text NULL;
                    END IF;
                END $$;
            ");

            // Add RoleType to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'RoleType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""RoleType"" text NULL;
                    END IF;
                END $$;
            ");

            // Add AuthorizationLetter to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'AuthorizationLetter'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""AuthorizationLetter"" text NULL;
                    END IF;
                END $$;
            ");

            // Add RentStartDate to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'RentStartDate'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""RentStartDate"" timestamp without time zone NULL;
                    END IF;
                END $$;
            ");

            // Add RentEndDate to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'RentEndDate'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""RentEndDate"" timestamp without time zone NULL;
                    END IF;
                END $$;
            ");

            // Add TazkiraType to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""TazkiraType"" text NULL;
                    END IF;
                END $$;
            ");

            // Add TazkiraVolume to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraVolume'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""TazkiraVolume"" text NULL;
                    END IF;
                END $$;
            ");

            // Add TazkiraPage to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraPage'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""TazkiraPage"" text NULL;
                    END IF;
                END $$;
            ");

            // Add TazkiraNumber to VehiclesBuyerDetails if not exists
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesBuyerDetails' 
                        AND column_name = 'TazkiraNumber'
                    ) THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" ADD COLUMN ""TazkiraNumber"" text NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove columns in reverse order
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'TazkiraNumber') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""TazkiraNumber"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'TazkiraPage') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""TazkiraPage"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'TazkiraVolume') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""TazkiraVolume"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'TazkiraType') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""TazkiraType"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'RentEndDate') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""RentEndDate"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'RentStartDate') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""RentStartDate"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'AuthorizationLetter') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""AuthorizationLetter"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'tr' AND table_name = 'VehiclesBuyerDetails' AND column_name = 'RoleType') THEN
                        ALTER TABLE tr.""VehiclesBuyerDetails"" DROP COLUMN ""RoleType"";
                    END IF;
                END $$;
            ");
        }
    }
}
