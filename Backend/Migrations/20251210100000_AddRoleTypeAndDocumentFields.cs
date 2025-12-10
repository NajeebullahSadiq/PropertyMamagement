using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTypeAndDocumentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SAFE MIGRATION: Adding RoleType, AuthorizationLetter, and HeirsLetter fields
            // These fields support conditional document requirements based on seller role type
            // All fields are nullable to maintain backward compatibility

            // ===== SellerDetails (Property) =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'RoleType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""RoleType"" text NULL;
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
                        AND column_name = 'AuthorizationLetter'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""AuthorizationLetter"" text NULL;
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
                        AND column_name = 'HeirsLetter'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" ADD COLUMN ""HeirsLetter"" text NULL;
                    END IF;
                END $$;
            ");

            // ===== VehiclesSellerDetails (Vehicle) =====
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'VehiclesSellerDetails' 
                        AND column_name = 'RoleType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""RoleType"" text NULL;
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
                        AND column_name = 'AuthorizationLetter'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""AuthorizationLetter"" text NULL;
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
                        AND column_name = 'HeirsLetter'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" ADD COLUMN ""HeirsLetter"" text NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Remove the new columns
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'tr' 
                        AND table_name = 'SellerDetails' 
                        AND column_name = 'RoleType'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""RoleType"";
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
                        AND column_name = 'AuthorizationLetter'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""AuthorizationLetter"";
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
                        AND column_name = 'HeirsLetter'
                    ) THEN
                        ALTER TABLE tr.""SellerDetails"" DROP COLUMN ""HeirsLetter"";
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
                        AND column_name = 'RoleType'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""RoleType"";
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
                        AND column_name = 'AuthorizationLetter'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""AuthorizationLetter"";
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
                        AND column_name = 'HeirsLetter'
                    ) THEN
                        ALTER TABLE tr.""VehiclesSellerDetails"" DROP COLUMN ""HeirsLetter"";
                    END IF;
                END $$;
            ");
        }
    }
}
