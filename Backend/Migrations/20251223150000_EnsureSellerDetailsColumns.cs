using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    public partial class EnsureSellerDetailsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'AdditionalDetails'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""AdditionalDetails"" text NULL;
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
                        AND column_name = 'TaxIdentificationNumber'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""TaxIdentificationNumber"" text NULL;
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
                        AND column_name = 'NationalIdCardPath'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""NationalIdCardPath"" text NULL;
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'AdditionalDetails'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""AdditionalDetails"";
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
                        AND column_name = 'TaxIdentificationNumber'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""TaxIdentificationNumber"";
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
                        AND column_name = 'NationalIdCardPath'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""NationalIdCardPath"";
                    END IF;
                END $$;
            ");
        }
    }
}
