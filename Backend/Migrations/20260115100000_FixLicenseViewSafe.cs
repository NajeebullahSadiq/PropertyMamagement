using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <summary>
    /// Safe migration to fix LicenseView - uses IF NOT EXISTS checks to avoid errors
    /// when columns/tables already exist.
    /// </summary>
    public partial class FixLicenseViewSafe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns to CompanyOwner table (safe - uses IF NOT EXISTS)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Add PhoneNumber column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PhoneNumber') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PhoneNumber"" VARCHAR(20) NULL;
                    END IF;
                    
                    -- Add WhatsAppNumber column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'WhatsAppNumber') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""WhatsAppNumber"" VARCHAR(20) NULL;
                    END IF;
                    
                    -- Add PermanentProvinceId column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentProvinceId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentProvinceId"" INTEGER NULL;
                    END IF;
                    
                    -- Add PermanentDistrictId column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentDistrictId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentDistrictId"" INTEGER NULL;
                    END IF;
                    
                    -- Add PermanentVillage column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentVillage') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentVillage"" TEXT NULL;
                    END IF;
                    
                    -- Add TemporaryProvinceId column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryProvinceId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryProvinceId"" INTEGER NULL;
                    END IF;
                    
                    -- Add TemporaryDistrictId column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryDistrictId') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryDistrictId"" INTEGER NULL;
                    END IF;
                    
                    -- Add TemporaryVillage column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryVillage') THEN
                        ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryVillage"" TEXT NULL;
                    END IF;
                END $$;
            ");

            // Add missing columns to LicenseDetails table (safe - uses IF NOT EXISTS)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Add RoyaltyAmount column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RoyaltyAmount') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RoyaltyAmount"" NUMERIC(18,2) NULL;
                    END IF;
                    
                    -- Add RoyaltyDate column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RoyaltyDate') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RoyaltyDate"" DATE NULL;
                    END IF;
                    
                    -- Add PenaltyAmount column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'PenaltyAmount') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""PenaltyAmount"" NUMERIC(18,2) NULL;
                    END IF;
                    
                    -- Add PenaltyDate column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'PenaltyDate') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""PenaltyDate"" DATE NULL;
                    END IF;
                    
                    -- Add HrLetter column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'HrLetter') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""HrLetter"" VARCHAR(255) NULL;
                    END IF;
                    
                    -- Add HrLetterDate column if not exists
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                        WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'HrLetterDate') THEN
                        ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""HrLetterDate"" DATE NULL;
                    END IF;
                END $$;
            ");

            // Drop and recreate the LicenseView with all required columns
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
                    co.""TemporaryVillage"",
                    -- Financial and Administrative Fields
                    ld.""RoyaltyAmount"",
                    ld.""RoyaltyDate"",
                    ld.""PenaltyAmount"",
                    ld.""PenaltyDate"",
                    ld.""HrLetter"",
                    ld.""HrLetterDate""
                FROM org.""CompanyDetails"" cd
                LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                LEFT JOIN look.""Location"" pp ON co.""PermanentProvinceId"" = pp.""ID""
                LEFT JOIN look.""Location"" pd ON co.""PermanentDistrictId"" = pd.""ID""
                LEFT JOIN look.""Location"" tp ON co.""TemporaryProvinceId"" = tp.""ID""
                LEFT JOIN look.""Location"" td ON co.""TemporaryDistrictId"" = td.""ID"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the basic LicenseView without financial fields
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
    }
}
