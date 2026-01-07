using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingCompanyOwnerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add PhoneNumber column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PhoneNumber') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PhoneNumber"" VARCHAR(20) NULL;
                    END IF;
                END $$;
            ");

            // Add WhatsAppNumber column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'WhatsAppNumber') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""WhatsAppNumber"" VARCHAR(20) NULL;
                    END IF;
                END $$;
            ");

            // Add PermanentProvinceId column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentProvinceId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentProvinceId"" INTEGER NULL;
                    END IF;
                END $$;
            ");

            // Add PermanentDistrictId column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentDistrictId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentDistrictId"" INTEGER NULL;
                    END IF;
                END $$;
            ");

            // Add PermanentVillage column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentVillage') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentVillage"" TEXT NULL;
                    END IF;
                END $$;
            ");

            // Add TemporaryProvinceId column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryProvinceId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryProvinceId"" INTEGER NULL;
                    END IF;
                END $$;
            ");

            // Add TemporaryDistrictId column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryDistrictId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryDistrictId"" INTEGER NULL;
                    END IF;
                END $$;
            ");

            // Add TemporaryVillage column if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryVillage') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryVillage"" TEXT NULL;
                    END IF;
                END $$;
            ");

            // Create CompanyOwnerAddressHistory table if not exists
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS org.""CompanyOwnerAddressHistory"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""CompanyOwnerId"" INTEGER NOT NULL,
                    ""ProvinceId"" INTEGER NULL,
                    ""DistrictId"" INTEGER NULL,
                    ""Village"" TEXT NULL,
                    ""AddressType"" VARCHAR(50) NOT NULL DEFAULT 'Permanent',
                    ""EffectiveFrom"" TIMESTAMP NULL,
                    ""EffectiveTo"" TIMESTAMP NULL,
                    ""IsActive"" BOOLEAN NOT NULL DEFAULT FALSE,
                    ""CreatedAt"" TIMESTAMP NULL,
                    ""CreatedBy"" VARCHAR(255) NULL,
                    CONSTRAINT ""FK_CompanyOwnerAddressHistory_CompanyOwner"" FOREIGN KEY (""CompanyOwnerId"") 
                        REFERENCES org.""CompanyOwner"" (""Id"") ON DELETE CASCADE
                );
            ");

            // Add foreign key constraints for address columns if not exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_CompanyOwner_PermanentProvince' AND table_schema = 'org') THEN
                        ALTER TABLE org.""CompanyOwner"" 
                            ADD CONSTRAINT ""FK_CompanyOwner_PermanentProvince"" 
                            FOREIGN KEY (""PermanentProvinceId"") REFERENCES look.""Location"" (""ID"");
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_CompanyOwner_PermanentDistrict' AND table_schema = 'org') THEN
                        ALTER TABLE org.""CompanyOwner"" 
                            ADD CONSTRAINT ""FK_CompanyOwner_PermanentDistrict"" 
                            FOREIGN KEY (""PermanentDistrictId"") REFERENCES look.""Location"" (""ID"");
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_CompanyOwner_TemporaryProvince' AND table_schema = 'org') THEN
                        ALTER TABLE org.""CompanyOwner"" 
                            ADD CONSTRAINT ""FK_CompanyOwner_TemporaryProvince"" 
                            FOREIGN KEY (""TemporaryProvinceId"") REFERENCES look.""Location"" (""ID"");
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_CompanyOwner_TemporaryDistrict' AND table_schema = 'org') THEN
                        ALTER TABLE org.""CompanyOwner"" 
                            ADD CONSTRAINT ""FK_CompanyOwner_TemporaryDistrict"" 
                            FOREIGN KEY (""TemporaryDistrictId"") REFERENCES look.""Location"" (""ID"");
                    END IF;
                END $$;
            ");

            // Update LicenseView with all new columns
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS public.""LicenseView"";
                
                CREATE OR REPLACE VIEW public.""LicenseView"" AS
                SELECT 
                    cd.""Id"" AS ""CompanyId"",
                    co.""PhoneNumber"",
                    co.""WhatsAppNumber"",
                    cd.""Title"",
                    cd.""TIN"" AS ""Tin"",
                    co.""FirstName"",
                    co.""FatherName"",
                    co.""GrandFatherName"",
                    co.""DateofBirth"",
                    co.""IndentityCardNumber"",
                    co.""PothoPath"" AS ""OwnerPhoto"",
                    ld.""LicenseNumber"",
                    ld.""OfficeAddress"",
                    ld.""IssueDate"",
                    ld.""ExpireDate"",
                    pp.""Dari"" AS ""PermanentProvinceName"",
                    pd.""Dari"" AS ""PermanentDistrictName"",
                    co.""PermanentVillage"",
                    tp.""Dari"" AS ""TemporaryProvinceName"",
                    td.""Dari"" AS ""TemporaryDistrictName"",
                    co.""TemporaryVillage""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                LEFT JOIN look.""Location"" pp ON co.""PermanentProvinceId"" = pp.""ID""
                LEFT JOIN look.""Location"" pd ON co.""PermanentDistrictId"" = pd.""ID""
                LEFT JOIN look.""Location"" tp ON co.""TemporaryProvinceId"" = tp.""ID""
                LEFT JOIN look.""Location"" td ON co.""TemporaryDistrictId"" = td.""ID"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints
            migrationBuilder.Sql(@"
                ALTER TABLE org.""CompanyOwner"" DROP CONSTRAINT IF EXISTS ""FK_CompanyOwner_PermanentProvince"";
                ALTER TABLE org.""CompanyOwner"" DROP CONSTRAINT IF EXISTS ""FK_CompanyOwner_PermanentDistrict"";
                ALTER TABLE org.""CompanyOwner"" DROP CONSTRAINT IF EXISTS ""FK_CompanyOwner_TemporaryProvince"";
                ALTER TABLE org.""CompanyOwner"" DROP CONSTRAINT IF EXISTS ""FK_CompanyOwner_TemporaryDistrict"";
            ");

            // Drop columns
            migrationBuilder.Sql(@"
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""PhoneNumber"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""WhatsAppNumber"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""PermanentProvinceId"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""PermanentDistrictId"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""PermanentVillage"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""TemporaryProvinceId"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""TemporaryDistrictId"";
                ALTER TABLE org.""CompanyOwner"" DROP COLUMN IF EXISTS ""TemporaryVillage"";
            ");

            // Drop address history table
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS org.""CompanyOwnerAddressHistory"";");

            // Restore original LicenseView
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS public.""LicenseView"";
                
                CREATE OR REPLACE VIEW public.""LicenseView"" AS
                SELECT 
                    cd.""Id"" AS ""CompanyId"",
                    cd.""Title"",
                    cd.""TIN"" AS ""Tin"",
                    co.""FirstName"",
                    co.""FatherName"",
                    co.""GrandFatherName"",
                    co.""DateofBirth"",
                    co.""IndentityCardNumber"",
                    co.""PothoPath"" AS ""OwnerPhoto"",
                    ld.""LicenseNumber"",
                    ld.""OfficeAddress"",
                    ld.""IssueDate"",
                    ld.""ExpireDate""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId"";
            ");
        }
    }
}
