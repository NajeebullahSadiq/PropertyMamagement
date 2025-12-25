using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxIdentificationNumberAndAdditionalDetailsToBuyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TaxIdentificationNumber'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""TaxIdentificationNumber"" text NULL;
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
                        AND column_name = 'AdditionalDetails'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" ADD COLUMN ""AdditionalDetails"" text NULL;
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
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'BuyerDetails' 
                        AND column_name = 'TaxIdentificationNumber'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""TaxIdentificationNumber"";
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
                        AND column_name = 'AdditionalDetails'
                    ) THEN
                        ALTER TABLE tr.""BuyerDetails"" DROP COLUMN ""AdditionalDetails"";
                    END IF;
                END $$;
            ");
        }
    }
}
