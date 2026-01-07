using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;

namespace WebAPIBackend.Configuration
{
    public class DatabaseSeeder
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database exists and apply pending migrations
            Console.WriteLine("Ensuring database exists and applying migrations...");
            await context.Database.MigrateAsync();

            // Apply Permanent/Temporary address column rename if not already done
            Console.WriteLine("Checking and applying Permanent/Temporary address columns...");
            try
            {
                // Check if old columns exist (CurrentProvinceId) and rename them to Temporary
                var columnExists = await context.Database.ExecuteSqlRawAsync(@"
                    DO $$
                    BEGIN
                        -- Check if CurrentProvinceId exists (old naming) and rename to Temporary
                        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'CurrentProvinceId') THEN
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""CurrentProvinceId"" TO ""TemporaryProvinceId"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""CurrentDistrictId"" TO ""TemporaryDistrictId"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""CurrentVillage"" TO ""TemporaryVillage"";
                            RAISE NOTICE 'Columns renamed from Current to Temporary';
                        END IF;
                        
                        -- Check if OfficeProvinceId exists (previous change) and rename back to Permanent/Temporary
                        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'OfficeProvinceId') THEN
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""OfficeProvinceId"" TO ""PermanentProvinceId"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""OfficeDistrictId"" TO ""PermanentDistrictId"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""OfficeVillage"" TO ""PermanentVillage"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""PersonalProvinceId"" TO ""TemporaryProvinceId"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""PersonalDistrictId"" TO ""TemporaryDistrictId"";
                            ALTER TABLE org.""CompanyOwner"" RENAME COLUMN ""PersonalVillage"" TO ""TemporaryVillage"";
                            RAISE NOTICE 'Columns renamed from Office/Personal to Permanent/Temporary';
                        END IF;
                        
                        -- Update address history types
                        UPDATE org.""CompanyOwnerAddressHistory"" SET ""AddressType"" = 'Permanent' WHERE ""AddressType"" = 'Office';
                        UPDATE org.""CompanyOwnerAddressHistory"" SET ""AddressType"" = 'Temporary' WHERE ""AddressType"" = 'Personal';
                        UPDATE org.""CompanyOwnerAddressHistory"" SET ""AddressType"" = 'Temporary' WHERE ""AddressType"" = 'Current';
                    END $$;
                ");
                Console.WriteLine("✓ Permanent/Temporary address columns checked/updated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Column rename check completed (may already be applied): {ex.Message}");
            }

            // Add missing columns to CompanyOwner table
            Console.WriteLine("Adding missing columns to CompanyOwner table...");
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    DO $$
                    BEGIN
                        -- Add PhoneNumber column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PhoneNumber') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PhoneNumber"" VARCHAR(20) NULL;
                            RAISE NOTICE 'Added PhoneNumber column';
                        END IF;
                        
                        -- Add WhatsAppNumber column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'WhatsAppNumber') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""WhatsAppNumber"" VARCHAR(20) NULL;
                            RAISE NOTICE 'Added WhatsAppNumber column';
                        END IF;
                        
                        -- Add PermanentProvinceId column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentProvinceId') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentProvinceId"" INTEGER NULL;
                            RAISE NOTICE 'Added PermanentProvinceId column';
                        END IF;
                        
                        -- Add PermanentDistrictId column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentDistrictId') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentDistrictId"" INTEGER NULL;
                            RAISE NOTICE 'Added PermanentDistrictId column';
                        END IF;
                        
                        -- Add PermanentVillage column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentVillage') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentVillage"" TEXT NULL;
                            RAISE NOTICE 'Added PermanentVillage column';
                        END IF;
                        
                        -- Add TemporaryProvinceId column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryProvinceId') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryProvinceId"" INTEGER NULL;
                            RAISE NOTICE 'Added TemporaryProvinceId column';
                        END IF;
                        
                        -- Add TemporaryDistrictId column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryDistrictId') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryDistrictId"" INTEGER NULL;
                            RAISE NOTICE 'Added TemporaryDistrictId column';
                        END IF;
                        
                        -- Add TemporaryVillage column if not exists
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'TemporaryVillage') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""TemporaryVillage"" TEXT NULL;
                            RAISE NOTICE 'Added TemporaryVillage column';
                        END IF;
                    END $$;
                ");
                Console.WriteLine("✓ Missing columns added to CompanyOwner table");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Column addition completed (may already exist): {ex.Message}");
            }

            // Create CompanyOwnerAddressHistory table if not exists
            Console.WriteLine("Creating CompanyOwnerAddressHistory table if not exists...");
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
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
                Console.WriteLine("✓ CompanyOwnerAddressHistory table created/verified");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: CompanyOwnerAddressHistory table check completed: {ex.Message}");
            }

            // Update LicenseView with new column names
            Console.WriteLine("Updating LicenseView with Permanent/Temporary address columns...");
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
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
                Console.WriteLine("✓ LicenseView updated with Permanent/Temporary address columns");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: LicenseView update issue: {ex.Message}");
            }
            
            // Force update the GetPrintType view with Dari translations
            Console.WriteLine("Updating GetPrintType view with Dari translations...");
            await context.Database.ExecuteSqlRawAsync(@"
                DROP VIEW IF EXISTS ""GetPrintType"";
                
                CREATE OR REPLACE VIEW ""GetPrintType"" AS
                SELECT 
                    pd.""Id"",
                    pd.""DocumentType"" as ""Doctype"",
                    pd.""PNumber"" as ""Pnumber"",
                    pd.""PArea"",
                    pd.""NumofRooms"",
                    pd.""north"",
                    pd.""south"",
                    pd.""west"",
                    pd.""east"",
                    pd.""Price"",
                    pd.""PriceText"",
                    pd.""RoyaltyAmount"",
                    pt.""Name"" as ""PropertypeType"",
                    pd.""CreatedAt"",
                    pd.""TransactionDate"" as ""DeedDate"",
                    
                    -- Property Images and Documents
                    pd.""FilePath"",
                    pd.""PreviousDocumentsPath"",
                    pd.""ExistingDocumentsPath"",
                    pd.""DocumentType"",
                    pd.""IssuanceNumber"",
                    pd.""IssuanceDate"",
                    pd.""SerialNumber"",
                    pd.""TransactionDate"",
                    pd.""PNumber"",
                    
                    -- Property Address Information
                    pa_prov.""Name"" as ""Province"",
                    pa_dist.""Name"" as ""District"",
                    pa_prov.""Dari"" as ""ProvinceDari"",
                    pa_dist.""Dari"" as ""DistrictDari"",
                    pa.""Village"",
                    
                    -- Seller Details
                    sd.""FirstName"" as ""SellerFirstName"",
                    sd.""FatherName"" as ""SellerFatherName"",
                    sd.""IndentityCardNumber"" as ""SellerIndentityCardNumber"",
                    sd.""PaddressVillage"" as ""SellerVillage"",
                    sd.""TaddressVillage"" as ""TSellerVillage"",
                    sd.""photo"" as ""SellerPhoto"",
                    
                    -- Seller Address Information
                    s_perm_prov.""Name"" as ""SellerProvince"",
                    s_perm_dist.""Name"" as ""SellerDistrict"",
                    s_perm_prov.""Dari"" as ""SellerProvinceDari"",
                    s_perm_dist.""Dari"" as ""SellerDistrictDari"",
                    s_temp_prov.""Name"" as ""TSellerProvince"",
                    s_temp_dist.""Name"" as ""TSellerDistrict"",
                    s_temp_prov.""Dari"" as ""TSellerProvinceDari"",
                    s_temp_dist.""Dari"" as ""TSellerDistrictDari"",
                    
                    -- Buyer Details
                    bd.""FirstName"" as ""BuyerFirstName"",
                    bd.""FatherName"" as ""BuyerFatherName"",
                    bd.""IndentityCardNumber"" as ""BuyerIndentityCardNumber"",
                    bd.""PaddressVillage"" as ""BuyerVillage"",
                    bd.""TaddressVillage"" as ""TBuyerVillage"",
                    bd.""photo"" as ""BuyerPhoto"",
                    
                    -- Buyer Address Information
                    b_perm_prov.""Name"" as ""BuyerProvince"",
                    b_perm_dist.""Name"" as ""BuyerDistrict"",
                    b_perm_prov.""Dari"" as ""BuyerProvinceDari"",
                    b_perm_dist.""Dari"" as ""BuyerDistrictDari"",
                    b_temp_prov.""Name"" as ""TBuyerProvince"",
                    b_temp_dist.""Name"" as ""TBuyerDistrict"",
                    b_temp_prov.""Dari"" as ""TBuyerProvinceDari"",
                    b_temp_dist.""Dari"" as ""TBuyerDistrictDari"",
                    
                    -- Witness 1 Details
                    wd1.""FirstName"" as ""WitnessOneFirstName"",
                    wd1.""FatherName"" as ""WitnessOneFatherName"",
                    wd1.""IndentityCardNumber"" as ""WitnessOneIndentityCardNumber"",
                    
                    -- Witness 2 Details
                    wd2.""FirstName"" as ""WitnessTwoFirstName"",
                    wd2.""FatherName"" as ""WitnessTwoFatherName"",
                    wd2.""IndentityCardNumber"" as ""WitnessTwoIndentityCardNumber"",
                    
                    -- Unit Type and Transaction Type
                    ut.""Name"" as ""UnitType"",
                    tt.""Name"" as ""TransactionType""

                FROM tr.""PropertyDetails"" pd
                LEFT JOIN look.""PropertyType"" pt ON pd.""PropertyTypeId"" = pt.""Id""
                LEFT JOIN look.""PUnitType"" ut ON pd.""PUnitTypeId"" = ut.""Id""
                LEFT JOIN look.""TransactionType"" tt ON pd.""TransactionTypeId"" = tt.""Id""

                -- Property Address
                LEFT JOIN tr.""PropertyAddress"" pa ON pd.""Id"" = pa.""PropertyDetailsId""
                LEFT JOIN look.""Location"" pa_prov ON pa.""ProvinceId"" = pa_prov.""ID""
                LEFT JOIN look.""Location"" pa_dist ON pa.""DistrictId"" = pa_dist.""ID""

                -- Seller Details and Address
                LEFT JOIN tr.""SellerDetails"" sd ON pd.""Id"" = sd.""PropertyDetailsId""
                LEFT JOIN look.""Location"" s_perm_prov ON sd.""PaddressProvinceId"" = s_perm_prov.""ID""
                LEFT JOIN look.""Location"" s_perm_dist ON sd.""PaddressDistrictId"" = s_perm_dist.""ID""
                LEFT JOIN look.""Location"" s_temp_prov ON sd.""TaddressProvinceId"" = s_temp_prov.""ID""
                LEFT JOIN look.""Location"" s_temp_dist ON sd.""TaddressDistrictId"" = s_temp_dist.""ID""

                -- Buyer Details and Address
                LEFT JOIN tr.""BuyerDetails"" bd ON pd.""Id"" = bd.""PropertyDetailsId""
                LEFT JOIN look.""Location"" b_perm_prov ON bd.""PaddressProvinceId"" = b_perm_prov.""ID""
                LEFT JOIN look.""Location"" b_perm_dist ON bd.""PaddressDistrictId"" = b_perm_dist.""ID""
                LEFT JOIN look.""Location"" b_temp_prov ON bd.""TaddressProvinceId"" = b_temp_prov.""ID""
                LEFT JOIN look.""Location"" b_temp_dist ON bd.""TaddressDistrictId"" = b_temp_dist.""ID""

                -- Witness 1 Details (First witness record)
                LEFT JOIN LATERAL (
                    SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                    FROM tr.""WitnessDetails""
                    WHERE ""PropertyDetailsId"" = pd.""Id""
                    ORDER BY ""Id"" ASC
                    LIMIT 1
                ) wd1 ON true

                -- Witness 2 Details (Second witness record)
                LEFT JOIN LATERAL (
                    SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                    FROM tr.""WitnessDetails""
                    WHERE ""PropertyDetailsId"" = pd.""Id""
                    ORDER BY ""Id"" ASC
                    OFFSET 1
                    LIMIT 1
                ) wd2 ON true

                WHERE pd.""iscomplete"" = true;
            ");
            
            Console.WriteLine("Database migrations applied successfully.");

            // Seed Admin Role (using ADMIN to match controller authorization)
            if (!await roleManager.RoleExistsAsync("ADMIN"))
            {
                var adminRole = new IdentityRole("ADMIN");
                await roleManager.CreateAsync(adminRole);

                // Add claims to Admin role - grant all permissions
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermissions.View));
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermissions.ViewUserTest));
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermissions.Add));
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermissions.Edit));
            }

            // Seed default admin user
            var adminEmail = "admin@prmis.gov.af";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    PhotoPath = "",
                    IsAdmin = true,
                    IsLocked = false,
                    CompanyId = 0,
                    PhoneNumber = "0700000000"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // Assign ADMIN role
                    await userManager.AddToRoleAsync(adminUser, "ADMIN");
                    
                    Console.WriteLine("✓ Default admin user created successfully!");
                    Console.WriteLine($"  Email: {adminEmail}");
                    Console.WriteLine($"  Username: admin");
                    Console.WriteLine($"  Password: Admin@123");
                }
                else
                {
                    Console.WriteLine("✗ Failed to create admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  - {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("✓ Admin user already exists.");
            }

            // Seed lookup tables for dropdown data
            await SeedLookupTables(context);
        }

        private static async Task SeedLookupTables(AppDbContext context)
        {
            Console.WriteLine("Seeding lookup tables...");

            // Seed PropertyTypes
            if (!context.PropertyTypes.Any())
            {
                var propertyTypes = new[]
                {
                    new PropertyType { Name = "House", Des = "Residential house" },
                    new PropertyType { Name = "Apartment", Des = "Apartment unit" },
                    new PropertyType { Name = "Land", Des = "Empty land/plot" },
                    new PropertyType { Name = "Commercial Building", Des = "Commercial property" },
                    new PropertyType { Name = "Office", Des = "Office space" },
                    new PropertyType { Name = "Shop", Des = "Retail shop" },
                    new PropertyType { Name = "Warehouse", Des = "Storage facility" },
                    new PropertyType { Name = "Factory", Des = "Industrial facility" },
                    new PropertyType { Name = "Farm", Des = "Agricultural land" },
                    new PropertyType { Name = "Villa", Des = "Luxury residential property" },
                    new PropertyType { Name = "Block", Des = "Residential block" }
                };
                await context.PropertyTypes.AddRangeAsync(propertyTypes);
                Console.WriteLine("✓ PropertyTypes seeded");
            }

            // Seed TransactionTypes
            if (!context.TransactionTypes.Any())
            {
                var transactionTypes = new[]
                {
                    new TransactionType { Name = "Sale", Des = "Property sale transaction" },
                    new TransactionType { Name = "Rent", Des = "Property rental" },
                    new TransactionType { Name = "Lease", Des = "Long-term lease" },
                    new TransactionType { Name = "Mortgage", Des = "Mortgage transaction" },
                    new TransactionType { Name = "Exchange", Des = "Property exchange" },
                    new TransactionType { Name = "Gift", Des = "Property gift/donation" },
                    new TransactionType { Name = "Inheritance", Des = "Inherited property" }
                };
                await context.TransactionTypes.AddRangeAsync(transactionTypes);
                Console.WriteLine("✓ TransactionTypes seeded");
            }

            // Seed/Update EducationLevels with Dari translations
            var educationLevelData = new[]
            {
                new { Name = "Illiterate", Dari = "بی سواد", Sorter = "01" },
                new { Name = "Primary School", Dari = "مکتب ابتدایی", Sorter = "02" },
                new { Name = "Secondary School", Dari = "مکتب متوسطه", Sorter = "03" },
                new { Name = "High School", Dari = "لیسه", Sorter = "04" },
                new { Name = "Diploma", Dari = "دیپلوم", Sorter = "05" },
                new { Name = "Bachelor's Degree", Dari = "لیسانس", Sorter = "06" },
                new { Name = "Master's Degree", Dari = "ماستر", Sorter = "07" },
                new { Name = "PhD/Doctorate", Dari = "دکتورا", Sorter = "08" },
                new { Name = "Religious Education", Dari = "تعلیمات دینی", Sorter = "09" },
                new { Name = "Technical/Vocational", Dari = "فنی و حرفوی", Sorter = "10" }
            };
            
            foreach (var eduLevel in educationLevelData)
            {
                var existing = await context.EducationLevels.FirstOrDefaultAsync(e => e.Name == eduLevel.Name);
                if (existing == null)
                {
                    await context.EducationLevels.AddAsync(new EducationLevel 
                    { 
                        Name = eduLevel.Name, 
                        Dari = eduLevel.Dari, 
                        Sorter = eduLevel.Sorter 
                    });
                }
                else
                {
                    existing.Dari = eduLevel.Dari;
                    existing.Sorter = eduLevel.Sorter;
                    context.EducationLevels.Update(existing);
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine("✓ EducationLevels seeded/updated with Dari translations");

            // Seed IdentityCardTypes
            if (!context.IdentityCardTypes.Any())
            {
                var identityCardTypes = new[]
                {
                    new IdentityCardType { Name = "National ID Card (Tazkira)", Des = "Afghan national identity card" },
                    new IdentityCardType { Name = "Passport", Des = "International passport" },
                    new IdentityCardType { Name = "Driver's License", Des = "Driving license" },
                    new IdentityCardType { Name = "Birth Certificate", Des = "Birth certificate" },
                    new IdentityCardType { Name = "Military ID", Des = "Military identification" },
                    new IdentityCardType { Name = "Student ID", Des = "Student identification" },
                    new IdentityCardType { Name = "Employee ID", Des = "Employee identification" }
                };
                await context.IdentityCardTypes.AddRangeAsync(identityCardTypes);
                Console.WriteLine("✓ IdentityCardTypes seeded");
            }

            // Seed AddressTypes
            if (!context.AddressTypes.Any())
            {
                var addressTypes = new[]
                {
                    new AddressType { Name = "Permanent Address", Des = "Permanent residence address" },
                    new AddressType { Name = "Temporary Address", Des = "Temporary residence address" },
                    new AddressType { Name = "Business Address", Des = "Business/work address" },
                    new AddressType { Name = "Mailing Address", Des = "Postal/mailing address" },
                    new AddressType { Name = "Emergency Contact Address", Des = "Emergency contact address" }
                };
                await context.AddressTypes.AddRangeAsync(addressTypes);
                Console.WriteLine("✓ AddressTypes seeded");
            }

            // Seed GuaranteeTypes
            if (!context.GuaranteeTypes.Any())
            {
                var guaranteeTypes = new[]
                {
                    new GuaranteeType { Name = "Bank Guarantee", Des = "Bank-issued guarantee" },
                    new GuaranteeType { Name = "Personal Guarantee", Des = "Personal guarantee from individual" },
                    new GuaranteeType { Name = "Corporate Guarantee", Des = "Company-issued guarantee" },
                    new GuaranteeType { Name = "Property Guarantee", Des = "Property as collateral" },
                    new GuaranteeType { Name = "Cash Deposit", Des = "Cash security deposit" },
                    new GuaranteeType { Name = "Government Guarantee", Des = "Government-backed guarantee" },
                    new GuaranteeType { Name = "Insurance Guarantee", Des = "Insurance-backed guarantee" }
                };
                await context.GuaranteeTypes.AddRangeAsync(guaranteeTypes);
                Console.WriteLine("✓ GuaranteeTypes seeded");
            }

            // Seed PunitTypes (Property Unit Types)
            if (!context.PunitTypes.Any())
            {
                var punitTypes = new[]
                {
                    new PunitType { Name = "Square Meter (m²)", Des = "Square meter measurement" },
                    new PunitType { Name = "Square Foot (ft²)", Des = "Square foot measurement" },
                    new PunitType { Name = "Jerib", Des = "Traditional Afghan land measurement (1 Jerib ≈ 2000 m²)" },
                    new PunitType { Name = "Acre", Des = "Acre measurement" },
                    new PunitType { Name = "Hectare", Des = "Hectare measurement" },
                    new PunitType { Name = "Biswa", Des = "Traditional measurement unit" },
                    new PunitType { Name = "Kanal", Des = "Traditional measurement unit" },
                    new PunitType { Name = "Marla", Des = "Traditional measurement unit" }
                };
                await context.PunitTypes.AddRangeAsync(punitTypes);
                Console.WriteLine("✓ PunitTypes seeded");
            }

            // Seed Areas (Business/License Areas)
            if (!context.Areas.Any())
            {
                var areas = new[]
                {
                    new Area { Name = "Construction", Des = "Construction and building services" },
                    new Area { Name = "Real Estate", Des = "Real estate services and trading" },
                    new Area { Name = "Import/Export", Des = "Import and export business" },
                    new Area { Name = "Manufacturing", Des = "Manufacturing and production" },
                    new Area { Name = "Retail Trade", Des = "Retail trading and sales" },
                    new Area { Name = "Wholesale Trade", Des = "Wholesale trading" },
                    new Area { Name = "Transportation", Des = "Transportation services" },
                    new Area { Name = "Agriculture", Des = "Agricultural activities" },
                    new Area { Name = "Mining", Des = "Mining and extraction" },
                    new Area { Name = "Tourism", Des = "Tourism and hospitality" },
                    new Area { Name = "Healthcare", Des = "Healthcare services" },
                    new Area { Name = "Education", Des = "Educational services" },
                    new Area { Name = "Financial Services", Des = "Banking and financial services" },
                    new Area { Name = "Technology", Des = "IT and technology services" },
                    new Area { Name = "Consulting", Des = "Professional consulting services" }
                };
                await context.Areas.AddRangeAsync(areas);
                Console.WriteLine("✓ Areas seeded");
            }

            // Seed ViolationTypes
            if (!context.ViolationTypes.Any())
            {
                var violationTypes = new[]
                {
                    new ViolationType { Name = "License Violation", Des = "Operating without proper license" },
                    new ViolationType { Name = "Tax Evasion", Des = "Tax-related violations" },
                    new ViolationType { Name = "Building Code Violation", Des = "Construction code violations" },
                    new ViolationType { Name = "Environmental Violation", Des = "Environmental regulation violations" },
                    new ViolationType { Name = "Safety Violation", Des = "Safety regulation violations" },
                    new ViolationType { Name = "Documentation Violation", Des = "Improper or missing documentation" },
                    new ViolationType { Name = "Zoning Violation", Des = "Land use zoning violations" },
                    new ViolationType { Name = "Contract Violation", Des = "Contract terms violations" },
                    new ViolationType { Name = "Quality Standards Violation", Des = "Quality standard violations" },
                    new ViolationType { Name = "Permit Violation", Des = "Operating without required permits" }
                };
                await context.ViolationTypes.AddRangeAsync(violationTypes);
                Console.WriteLine("✓ ViolationTypes seeded");
            }

            // Seed LostdocumentsTypes
            if (!context.LostdocumentsTypes.Any())
            {
                var lostDocumentTypes = new[]
                {
                    new LostdocumentsType { Name = "Property Deed", Des = "Property ownership document" },
                    new LostdocumentsType { Name = "Business License", Des = "Business operation license" },
                    new LostdocumentsType { Name = "Tax Certificate", Des = "Tax clearance certificate" },
                    new LostdocumentsType { Name = "Construction Permit", Des = "Building construction permit" },
                    new LostdocumentsType { Name = "Identity Card", Des = "National identity card" },
                    new LostdocumentsType { Name = "Passport", Des = "Travel passport" },
                    new LostdocumentsType { Name = "Vehicle Registration", Des = "Vehicle registration document" },
                    new LostdocumentsType { Name = "Insurance Policy", Des = "Insurance policy document" },
                    new LostdocumentsType { Name = "Contract Agreement", Des = "Legal contract document" },
                    new LostdocumentsType { Name = "Bank Statement", Des = "Financial bank statement" }
                };
                await context.LostdocumentsTypes.AddRangeAsync(lostDocumentTypes);
                Console.WriteLine("✓ LostdocumentsTypes seeded");
            }

            // Seed Locations (Afghanistan Provinces and Districts)
            // Always seed/update provinces
            var provinces = new[]
            {
                new Location { Name = "Kabul", Dari = "کابل", TypeId = 2, Code = "01", IsActive = 1 },
                new Location { Name = "Herat", Dari = "هرات", TypeId = 2, Code = "02", IsActive = 1 },
                new Location { Name = "Kandahar", Dari = "کندهار", TypeId = 2, Code = "03", IsActive = 1 },
                new Location { Name = "Balkh", Dari = "بلخ", TypeId = 2, Code = "04", IsActive = 1 },
                new Location { Name = "Nangarhar", Dari = "ننگرهار", TypeId = 2, Code = "05", IsActive = 1 },
                new Location { Name = "Ghazni", Dari = "غزنی", TypeId = 2, Code = "06", IsActive = 1 },
                new Location { Name = "Helmand", Dari = "هلمند", TypeId = 2, Code = "07", IsActive = 1 },
                new Location { Name = "Badakhshan", Dari = "بدخشان", TypeId = 2, Code = "08", IsActive = 1 },
                new Location { Name = "Takhar", Dari = "تخار", TypeId = 2, Code = "09", IsActive = 1 },
                new Location { Name = "Kunduz", Dari = "کندز", TypeId = 2, Code = "10", IsActive = 1 },
                new Location { Name = "Baghlan", Dari = "بغلان", TypeId = 2, Code = "11", IsActive = 1 },
                new Location { Name = "Bamyan", Dari = "بامیان", TypeId = 2, Code = "12", IsActive = 1 },
                new Location { Name = "Farah", Dari = "فراه", TypeId = 2, Code = "13", IsActive = 1 },
                new Location { Name = "Faryab", Dari = "فاریاب", TypeId = 2, Code = "14", IsActive = 1 },
                new Location { Name = "Ghor", Dari = "غور", TypeId = 2, Code = "15", IsActive = 1 },
                new Location { Name = "Jawzjan", Dari = "جوزجان", TypeId = 2, Code = "16", IsActive = 1 },
                new Location { Name = "Kapisa", Dari = "کاپیسا", TypeId = 2, Code = "17", IsActive = 1 },
                new Location { Name = "Khost", Dari = "خوست", TypeId = 2, Code = "18", IsActive = 1 },
                new Location { Name = "Kunar", Dari = "کنر", TypeId = 2, Code = "19", IsActive = 1 },
                new Location { Name = "Laghman", Dari = "لغمان", TypeId = 2, Code = "20", IsActive = 1 },
                new Location { Name = "Logar", Dari = "لوگر", TypeId = 2, Code = "21", IsActive = 1 },
                new Location { Name = "Nimroz", Dari = "نیمروز", TypeId = 2, Code = "22", IsActive = 1 },
                new Location { Name = "Nuristan", Dari = "نورستان", TypeId = 2, Code = "23", IsActive = 1 },
                new Location { Name = "Paktia", Dari = "پکتیا", TypeId = 2, Code = "24", IsActive = 1 },
                new Location { Name = "Paktika", Dari = "پکتیکا", TypeId = 2, Code = "25", IsActive = 1 },
                new Location { Name = "Panjshir", Dari = "پنجشیر", TypeId = 2, Code = "26", IsActive = 1 },
                new Location { Name = "Parwan", Dari = "پروان", TypeId = 2, Code = "27", IsActive = 1 },
                new Location { Name = "Samangan", Dari = "سمنگان", TypeId = 2, Code = "28", IsActive = 1 },
                new Location { Name = "Sar-e Pol", Dari = "سرپل", TypeId = 2, Code = "29", IsActive = 1 },
                new Location { Name = "Uruzgan", Dari = "ارزگان", TypeId = 2, Code = "30", IsActive = 1 },
                new Location { Name = "Wardak", Dari = "وردک", TypeId = 2, Code = "31", IsActive = 1 },
                new Location { Name = "Zabul", Dari = "زابل", TypeId = 2, Code = "32", IsActive = 1 },
                new Location { Name = "Badghis", Dari = "بادغیس", TypeId = 2, Code = "33", IsActive = 1 },
                new Location { Name = "Daykundi", Dari = "دایکندی", TypeId = 2, Code = "34", IsActive = 1 }
            };
            
            // Upsert provinces (add if not exist, update if exists)
            foreach (var province in provinces)
            {
                var existingProvince = await context.Locations.FirstOrDefaultAsync(l => l.Name == province.Name && l.TypeId == 2);
                if (existingProvince == null)
                {
                    await context.Locations.AddAsync(province);
                }
                else
                {
                    existingProvince.Dari = province.Dari;
                    existingProvince.Code = province.Code;
                    existingProvince.IsActive = province.IsActive;
                    context.Locations.Update(existingProvince);
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Provinces seeded/updated");
            
            // Seed districts - always check and add missing ones
            var districts = new List<Location>();
            
            // Get the saved provinces with their IDs
            var savedProvinces = await context.Locations.Where(l => l.TypeId == 2).ToListAsync();
            
            // Kabul Province Districts
            var kabulProvince = savedProvinces.FirstOrDefault(p => p.Name == "Kabul");
            if (kabulProvince != null)
            {
                districts.AddRange(new[]
                {
                    new Location { Name = "Kabul Center", Dari = "مرکز کابل", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Paghman", Dari = "پغمان", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Deh Sabz", Dari = "ده سبز", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Bagrami", Dari = "بگرامی", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Chahar Asyab", Dari = "چهار آسیاب", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Farza", Dari = "فرزه", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Kalakan", Dari = "کلکان", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Khaki Jabbar", Dari = "خاکی جبار", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Mir Bacha Kot", Dari = "میر بچه کوت", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Musahi", Dari = "موسهی", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Qarabagh", Dari = "قره باغ", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Shakardara", Dari = "شکردره", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 },
                    new Location { Name = "Surobi", Dari = "سروبی", TypeId = 3, ParentId = kabulProvince.Id, IsActive = 1 }
                });
            }
            
            // Herat Province Districts
            var heratProvince = savedProvinces.FirstOrDefault(p => p.Name == "Herat");
            if (heratProvince != null)
            {
                districts.AddRange(new[]
                {
                    new Location { Name = "Herat Center", Dari = "مرکز هرات", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Adraskan", Dari = "ادرسکن", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Chishti Sharif", Dari = "چشتی شریف", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Farsi", Dari = "فارسی", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Ghoryan", Dari = "غوریان", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Gulran", Dari = "گلران", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Injil", Dari = "انجیل", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Karukh", Dari = "کرخ", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Kushk", Dari = "کشک", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Kushki Kuhna", Dari = "کشک کهنه", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Obe", Dari = "اوبه", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Pashtun Zarghun", Dari = "پشتون زرغون", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Shindand", Dari = "شیندند", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Zinda Jan", Dari = "زنده جان", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 }
                });
            }
            
            // Kandahar Province Districts
            var kandaharProvince = savedProvinces.FirstOrDefault(p => p.Name == "Kandahar");
            if (kandaharProvince != null)
            {
                districts.AddRange(new[]
                {
                    new Location { Name = "Kandahar Center", Dari = "مرکز کندهار", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Arghandab", Dari = "ارغنداب", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Arghistan", Dari = "ارغستان", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Daman", Dari = "دامان", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Ghorak", Dari = "غورک", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Khakrez", Dari = "خاکریز", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Maywand", Dari = "میوند", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Maruf", Dari = "معروف", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Nesh", Dari = "نیش", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Panjwayi", Dari = "پنجوایی", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Reg", Dari = "ریگ", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Shah Wali Kot", Dari = "شاه ولی کوت", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Shorabak", Dari = "شورابک", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Spin Boldak", Dari = "سپین بولدک", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Takht-e Pol", Dari = "تخت پل", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Zhari", Dari = "ژړی", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 }
                });
            }
            
            // Balkh Province Districts
            var balkhProvince = savedProvinces.FirstOrDefault(p => p.Name == "Balkh");
            if (balkhProvince != null)
            {
                districts.AddRange(new[]
                {
                    new Location { Name = "Mazar-i-Sharif", Dari = "مزار شریف", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Balkh Center", Dari = "مرکز بلخ", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Char Bolak", Dari = "چار بولک", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Char Kant", Dari = "چارکنت", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Chemtal", Dari = "چمتال", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Dehdadi", Dari = "دهدادی", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Kaldar", Dari = "کلدار", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Kishindih", Dari = "کشنده", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Marmul", Dari = "مارمل", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Nahri Shahi", Dari = "نهر شاهی", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Sholgara", Dari = "شولگره", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Shortepa", Dari = "شورتپه", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Zari", Dari = "زاری", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 }
                });
            }
            
            // Nangarhar Province Districts
                var nangarharProvince = savedProvinces.FirstOrDefault(p => p.Name == "Nangarhar");
                if (nangarharProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Jalalabad", Dari = "جلال آباد", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Achin", Dari = "اچین", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Bati Kot", Dari = "بتی کوت", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Chaparhar", Dari = "چپرهار", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Deh Bala", Dari = "ده بالا", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Dor Baba", Dari = "در بابا", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Goshta", Dari = "گوشته", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Hisarak", Dari = "حصارک", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Kama", Dari = "کامه", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Khogiani", Dari = "خوگیانی", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Kot", Dari = "کوت", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Kuz Kunar", Dari = "کوز کنر", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Lal Pur", Dari = "لعل پور", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Mohmand Dara", Dari = "محمند دره", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Nazyan", Dari = "نازیان", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Pachir wa Agam", Dari = "پچیر و آگام", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Rodat", Dari = "رودات", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Sherzad", Dari = "شیرزاد", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Shinwar", Dari = "شینوار", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                        new Location { Name = "Surkh Rod", Dari = "سرخ رود", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 }
                    });
                }
                
                // Ghazni Province Districts
                var ghazniProvince = savedProvinces.FirstOrDefault(p => p.Name == "Ghazni");
                if (ghazniProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Ghazni Center", Dari = "مرکز غزنی", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Ajristan", Dari = "اجرستان", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Andar", Dari = "اندر", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Deh Yak", Dari = "ده یک", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Gelan", Dari = "گیلان", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Giro", Dari = "گیرو", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Jaghatu", Dari = "جاغتو", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Jaghori", Dari = "جاغوری", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Khost Wa Fereng", Dari = "خوست و فرنگ", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Muqur", Dari = "موقر", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Nawa", Dari = "ناوه", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Waghjan", Dari = "واغجان", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 },
                        new Location { Name = "Zana Khan", Dari = "زانا خان", TypeId = 3, ParentId = ghazniProvince.Id, IsActive = 1 }
                    });
                }
                
                // Helmand Province Districts
                var helmandProvince = savedProvinces.FirstOrDefault(p => p.Name == "Helmand");
                if (helmandProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Lashkar Gah", Dari = "لشکرگاه", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Bakwa", Dari = "بکوا", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Bust", Dari = "بوست", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Garmser", Dari = "گرمسیر", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Gereshk", Dari = "گریشک", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Herat Rud", Dari = "هرات رود", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Kajaki", Dari = "کاجکی", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Musa Qala", Dari = "موسیٰ قلعه", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Nad Ali", Dari = "ناد علی", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Naw Zad", Dari = "نو زاد", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 },
                        new Location { Name = "Sangin", Dari = "سنگین", TypeId = 3, ParentId = helmandProvince.Id, IsActive = 1 }
                    });
                }
                
                // Badakhshan Province Districts
                var badakhshanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Badakhshan");
                if (badakhshanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Faizabad", Dari = "فیض آباد", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Baharak", Dari = "بهارک", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Daraim", Dari = "درایم", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Darwaz", Dari = "درواز", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Ishkashim", Dari = "اشکاشم", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Jorm", Dari = "جرم", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Jurm", Dari = "جورم", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Keshm", Dari = "کشم", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Khwahan", Dari = "خواهان", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Kuran wa Munjan", Dari = "کوران و منجان", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Maimay", Dari = "میمی", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Ragh", Dari = "راغ", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Shighnan", Dari = "شغنان", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Shuhada", Dari = "شهدا", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 },
                        new Location { Name = "Wakhan", Dari = "واخان", TypeId = 3, ParentId = badakhshanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Takhar Province Districts
                var takharProvince = savedProvinces.FirstOrDefault(p => p.Name == "Takhar");
                if (takharProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Taluqan", Dari = "تالقان", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Bangi", Dari = "بنگی", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Chah Ab", Dari = "چاه آب", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Dasht-e Qala", Dari = "داشت قلعه", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Farkhar", Dari = "فرخار", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Kalafgan", Dari = "کلافگان", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Khwaja Ghar", Dari = "خواجه غار", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Rustaq", Dari = "رستاق", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Taloqan", Dari = "تلوقان", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Warsaj", Dari = "وارسج", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 },
                        new Location { Name = "Yangi Qala", Dari = "ینگی قلعه", TypeId = 3, ParentId = takharProvince.Id, IsActive = 1 }
                    });
                }
                
                // Kunduz Province Districts
                var kunduzProvince = savedProvinces.FirstOrDefault(p => p.Name == "Kunduz");
                if (kunduzProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Kunduz Center", Dari = "مرکز کندز", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Aliabad", Dari = "علی آباد", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Archi", Dari = "ارچی", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Chahab", Dari = "چهاب", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Dasht-e Archi", Dari = "داشت ارچی", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Hazrat Imam", Dari = "حضرت امام", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Khanabad", Dari = "خان آباد", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Qala-e Zal", Dari = "قلعه زال", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 },
                        new Location { Name = "Warsaj", Dari = "وارسج", TypeId = 3, ParentId = kunduzProvince.Id, IsActive = 1 }
                    });
                }
                
                // Baghlan Province Districts
                var baghlanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Baghlan");
                if (baghlanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Baghlan Center", Dari = "مرکز بغلان", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Andarab", Dari = "اندراب", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Baglan-e Jadid", Dari = "بغلان جدید", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Doshi", Dari = "دوشی", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Khost Wa Fereng", Dari = "خوست و فرنگ", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Nahrin", Dari = "نهرین", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Pul-e Khumri", Dari = "پل خمری", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 },
                        new Location { Name = "Tala-wa Barfak", Dari = "تالا و برفک", TypeId = 3, ParentId = baghlanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Bamyan Province Districts
                var bamyanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Bamyan");
                if (bamyanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Bamyan Center", Dari = "مرکز بامیان", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Kahmard", Dari = "کهمرد", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Nili", Dari = "نیلی", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Panjab", Dari = "پنجاب", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Saighan", Dari = "سیغان", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Shibar", Dari = "شیبر", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Waras", Dari = "وارس", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 },
                        new Location { Name = "Yakawlang", Dari = "یکاولنگ", TypeId = 3, ParentId = bamyanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Farah Province Districts
                var farahProvince = savedProvinces.FirstOrDefault(p => p.Name == "Farah");
                if (farahProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Farah Center", Dari = "مرکز فراه", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Anar Darreh", Dari = "انار دره", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Bakwa", Dari = "بکوا", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Bala Bluk", Dari = "بالا بلوک", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Delaram", Dari = "دل ارام", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Gulestan", Dari = "گلستان", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Khaki Safed", Dari = "خاکی سفید", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Purdil", Dari = "پردیل", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 },
                        new Location { Name = "Shindand", Dari = "شیندند", TypeId = 3, ParentId = farahProvince.Id, IsActive = 1 }
                    });
                }
                
                // Faryab Province Districts
                var faryabProvince = savedProvinces.FirstOrDefault(p => p.Name == "Faryab");
                if (faryabProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Maimana", Dari = "میمنه", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Almar", Dari = "الماز", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Andkhoy", Dari = "اندخوی", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Bilchiragh", Dari = "بیل چراغ", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Darzab", Dari = "درزاب", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Khani Chahar", Dari = "خانی چهار", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Pashtun Kot", Dari = "پشتون کوت", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Qaisar", Dari = "قیصر", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 },
                        new Location { Name = "Shirin Tagab", Dari = "شیرین تگاب", TypeId = 3, ParentId = faryabProvince.Id, IsActive = 1 }
                    });
                }
                
                // Ghor Province Districts
                var ghorProvince = savedProvinces.FirstOrDefault(p => p.Name == "Ghor");
                if (ghorProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Chaghcharan", Dari = "چغچران", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Chightan", Dari = "چغتن", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Dolina", Dari = "دولینه", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Lal Wa Sarjangal", Dari = "لعل و سرجنگل", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Pasaband", Dari = "پاسابند", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Saghar", Dari = "ساغر", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Shahrak", Dari = "شهرک", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 },
                        new Location { Name = "Taywara", Dari = "تیواره", TypeId = 3, ParentId = ghorProvince.Id, IsActive = 1 }
                    });
                }
                
                // Jawzjan Province Districts
                var jawzjanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Jawzjan");
                if (jawzjanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Sheberghan", Dari = "شبرغان", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Aqcha", Dari = "اقچه", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Fayzabad", Dari = "فیض آباد", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Khamyab", Dari = "خامیاب", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Khanaqa", Dari = "خانقاه", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Mardyan", Dari = "مردیان", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Mingajik", Dari = "مینگاجک", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 },
                        new Location { Name = "Qaragum", Dari = "قره قم", TypeId = 3, ParentId = jawzjanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Kapisa Province Districts
                var kapisaProvince = savedProvinces.FirstOrDefault(p => p.Name == "Kapisa");
                if (kapisaProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Mahmud-e Raqi", Dari = "محمود رقی", TypeId = 3, ParentId = kapisaProvince.Id, IsActive = 1 },
                        new Location { Name = "Alasay", Dari = "علاسای", TypeId = 3, ParentId = kapisaProvince.Id, IsActive = 1 },
                        new Location { Name = "Kohband", Dari = "کوهبند", TypeId = 3, ParentId = kapisaProvince.Id, IsActive = 1 },
                        new Location { Name = "Nijrab", Dari = "نجراب", TypeId = 3, ParentId = kapisaProvince.Id, IsActive = 1 },
                        new Location { Name = "Panjshir", Dari = "پنجشیر", TypeId = 3, ParentId = kapisaProvince.Id, IsActive = 1 },
                        new Location { Name = "Tagab", Dari = "تگاب", TypeId = 3, ParentId = kapisaProvince.Id, IsActive = 1 }
                    });
                }
                
                // Khost Province Districts
                var khostProvince = savedProvinces.FirstOrDefault(p => p.Name == "Khost");
                if (khostProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Khost Center", Dari = "مرکز خوست", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Bak", Dari = "باک", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Dand Wa Patan", Dari = "دند و پتن", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Garbuz", Dari = "گربز", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Gyan", Dari = "گیان", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Jaji", Dari = "جاجی", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Jani Khel", Dari = "جانی خیل", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Katawaz", Dari = "کتواز", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Mandozai", Dari = "ماندوزی", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Musa Khel", Dari = "موسیٰ خیل", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Sabari", Dari = "صباری", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 },
                        new Location { Name = "Tani", Dari = "تنی", TypeId = 3, ParentId = khostProvince.Id, IsActive = 1 }
                    });
                }
                
                // Kunar Province Districts
                var kunarProvince = savedProvinces.FirstOrDefault(p => p.Name == "Kunar");
                if (kunarProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Asadabad", Dari = "اسد آباد", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Arandu", Dari = "ارندو", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Bar Kunar", Dari = "بر کنر", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Chapa Dara", Dari = "چاپه دره", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Dangam", Dari = "دنگام", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Kamdesh", Dari = "کمدش", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Khas Kunar", Dari = "خاص کنر", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Marawara", Dari = "مراوره", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Nari", Dari = "ناری", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Pech", Dari = "پیچ", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 },
                        new Location { Name = "Shaltan", Dari = "شالتن", TypeId = 3, ParentId = kunarProvince.Id, IsActive = 1 }
                    });
                }
                
                // Laghman Province Districts
                var laghmanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Laghman");
                if (laghmanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Mihtarlam", Dari = "مهتر لم", TypeId = 3, ParentId = laghmanProvince.Id, IsActive = 1 },
                        new Location { Name = "Alingar", Dari = "علنگر", TypeId = 3, ParentId = laghmanProvince.Id, IsActive = 1 },
                        new Location { Name = "Alishang", Dari = "علیشنگ", TypeId = 3, ParentId = laghmanProvince.Id, IsActive = 1 },
                        new Location { Name = "Dawlatshah", Dari = "دولت شاه", TypeId = 3, ParentId = laghmanProvince.Id, IsActive = 1 },
                        new Location { Name = "Qarghayi", Dari = "قرغی", TypeId = 3, ParentId = laghmanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Logar Province Districts
                var logarProvince = savedProvinces.FirstOrDefault(p => p.Name == "Logar");
                if (logarProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Puli Alam", Dari = "پل علم", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 },
                        new Location { Name = "Azra", Dari = "عزره", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 },
                        new Location { Name = "Baraki Barak", Dari = "براکی بارک", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 },
                        new Location { Name = "Charkh", Dari = "چارخ", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 },
                        new Location { Name = "Khoshi", Dari = "خوشی", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 },
                        new Location { Name = "Mohammad Agha", Dari = "محمد آغا", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 },
                        new Location { Name = "Puli Alam", Dari = "پل علم", TypeId = 3, ParentId = logarProvince.Id, IsActive = 1 }
                    });
                }
                
                // Nimroz Province Districts
                var nimrozProvince = savedProvinces.FirstOrDefault(p => p.Name == "Nimroz");
                if (nimrozProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Zaranj", Dari = "زرنج", TypeId = 3, ParentId = nimrozProvince.Id, IsActive = 1 },
                        new Location { Name = "Chahar Burjak", Dari = "چهار برجک", TypeId = 3, ParentId = nimrozProvince.Id, IsActive = 1 },
                        new Location { Name = "Dasht-e Margo", Dari = "داشت مرگو", TypeId = 3, ParentId = nimrozProvince.Id, IsActive = 1 },
                        new Location { Name = "Khanneshin", Dari = "خان نشین", TypeId = 3, ParentId = nimrozProvince.Id, IsActive = 1 },
                        new Location { Name = "Washir", Dari = "واشیر", TypeId = 3, ParentId = nimrozProvince.Id, IsActive = 1 }
                    });
                }
                
                // Nuristan Province Districts
                var nuristanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Nuristan");
                if (nuristanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Bardigal", Dari = "بردیگل", TypeId = 3, ParentId = nuristanProvince.Id, IsActive = 1 },
                        new Location { Name = "Kamdesh", Dari = "کمدش", TypeId = 3, ParentId = nuristanProvince.Id, IsActive = 1 },
                        new Location { Name = "Khas Nuristan", Dari = "خاص نورستان", TypeId = 3, ParentId = nuristanProvince.Id, IsActive = 1 },
                        new Location { Name = "Parun", Dari = "پارون", TypeId = 3, ParentId = nuristanProvince.Id, IsActive = 1 },
                        new Location { Name = "Wama", Dari = "وامه", TypeId = 3, ParentId = nuristanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Paktia Province Districts
                var paktiaProvince = savedProvinces.FirstOrDefault(p => p.Name == "Paktia");
                if (paktiaProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Gardez", Dari = "گردیز", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Ahmadabad", Dari = "احمد آباد", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Chamkani", Dari = "چمکنی", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Dara Michi", Dari = "دره میچی", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Drabalo", Dari = "درابالو", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Gyan", Dari = "گیان", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Jani Khel", Dari = "جانی خیل", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Jaya Manda", Dari = "جیا مندا", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Laja Mangal", Dari = "لاجا منگل", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Sarobi", Dari = "سروبی", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Sharan", Dari = "شاران", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 },
                        new Location { Name = "Zeluk", Dari = "زیلوک", TypeId = 3, ParentId = paktiaProvince.Id, IsActive = 1 }
                    });
                }
                
                // Paktika Province Districts
                var paktikaProvince = savedProvinces.FirstOrDefault(p => p.Name == "Paktika");
                if (paktikaProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Sharan", Dari = "شاران", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Angoor Adha", Dari = "انگور آده", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Barmal", Dari = "برمل", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Dila", Dari = "دیلا", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Giyan", Dari = "گیان", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Gomal", Dari = "گومل", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Gyan", Dari = "گیان", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Hari", Dari = "ہری", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Jani Khel", Dari = "جانی خیل", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Katawaz", Dari = "کتواز", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Naka", Dari = "ناکا", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Sar Hawza", Dari = "سر حوزه", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 },
                        new Location { Name = "Zarmat", Dari = "زرمت", TypeId = 3, ParentId = paktikaProvince.Id, IsActive = 1 }
                    });
                }
                
                // Panjshir Province Districts
                var panjshirProvince = savedProvinces.FirstOrDefault(p => p.Name == "Panjshir");
                if (panjshirProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Bazarak", Dari = "بازارک", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 },
                        new Location { Name = "Ashtan", Dari = "اشتن", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 },
                        new Location { Name = "Darah", Dari = "داره", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 },
                        new Location { Name = "Khenj", Dari = "خنج", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 },
                        new Location { Name = "Paryan", Dari = "پریان", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 },
                        new Location { Name = "Rukha", Dari = "روخه", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 },
                        new Location { Name = "Safed Sang", Dari = "سفید سنگ", TypeId = 3, ParentId = panjshirProvince.Id, IsActive = 1 }
                    });
                }
                
                // Parwan Province Districts
                var parwanProvince = savedProvinces.FirstOrDefault(p => p.Name == "Parwan");
                if (parwanProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Charikar", Dari = "چاریکار", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Bagram", Dari = "باگرام", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Ghorband", Dari = "غوربند", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Jabul Saraj", Dari = "جبل سراج", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Kohdaman", Dari = "کوهدامن", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Panjshir", Dari = "پنجشیر", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Rikha", Dari = "ریخه", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Salang", Dari = "سلنگ", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 },
                        new Location { Name = "Surkh Parsa", Dari = "سرخ پرسا", TypeId = 3, ParentId = parwanProvince.Id, IsActive = 1 }
                    });
                }
                
                // Samangan Province Districts
                var samanganProvince = savedProvinces.FirstOrDefault(p => p.Name == "Samangan");
                if (samanganProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Aybak", Dari = "ایبک", TypeId = 3, ParentId = samanganProvince.Id, IsActive = 1 },
                        new Location { Name = "Dara-e Suf", Dari = "دره سوف", TypeId = 3, ParentId = samanganProvince.Id, IsActive = 1 },
                        new Location { Name = "Khulm", Dari = "خلم", TypeId = 3, ParentId = samanganProvince.Id, IsActive = 1 },
                        new Location { Name = "Ruler", Dari = "رولر", TypeId = 3, ParentId = samanganProvince.Id, IsActive = 1 },
                        new Location { Name = "Samangan", Dari = "سمنگان", TypeId = 3, ParentId = samanganProvince.Id, IsActive = 1 }
                    });
                }
                
                // Sar-e Pol Province Districts
                var sarepolProvince = savedProvinces.FirstOrDefault(p => p.Name == "Sar-e Pol");
                if (sarepolProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Sar-e Pol Center", Dari = "مرکز سرپل", TypeId = 3, ParentId = sarepolProvince.Id, IsActive = 1 },
                        new Location { Name = "Balkhab", Dari = "بلخاب", TypeId = 3, ParentId = sarepolProvince.Id, IsActive = 1 },
                        new Location { Name = "Gosfandi", Dari = "گوسفندی", TypeId = 3, ParentId = sarepolProvince.Id, IsActive = 1 },
                        new Location { Name = "Kohistanat", Dari = "کوهستانات", TypeId = 3, ParentId = sarepolProvince.Id, IsActive = 1 },
                        new Location { Name = "Sangcharak", Dari = "سنگ چراک", TypeId = 3, ParentId = sarepolProvince.Id, IsActive = 1 }
                    });
                }
                
                // Uruzgan Province Districts
                var uruzganProvince = savedProvinces.FirstOrDefault(p => p.Name == "Uruzgan");
                if (uruzganProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Tirin Kot", Dari = "تیرین کوت", TypeId = 3, ParentId = uruzganProvince.Id, IsActive = 1 },
                        new Location { Name = "Charchino", Dari = "چارچینو", TypeId = 3, ParentId = uruzganProvince.Id, IsActive = 1 },
                        new Location { Name = "Deh Rawud", Dari = "ده رود", TypeId = 3, ParentId = uruzganProvince.Id, IsActive = 1 },
                        new Location { Name = "Gizab", Dari = "گیزاب", TypeId = 3, ParentId = uruzganProvince.Id, IsActive = 1 },
                        new Location { Name = "Khas Uruzgan", Dari = "خاص ارزگان", TypeId = 3, ParentId = uruzganProvince.Id, IsActive = 1 },
                        new Location { Name = "Shahidi Hassas", Dari = "شاهدی حصص", TypeId = 3, ParentId = uruzganProvince.Id, IsActive = 1 }
                    });
                }
                
                // Wardak Province Districts
                var wardakProvince = savedProvinces.FirstOrDefault(p => p.Name == "Wardak");
                if (wardakProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Maidan Shahr", Dari = "میدان شهر", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Chak", Dari = "چک", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Daimirdad", Dari = "دیمردد", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Day Mirdad", Dari = "دی میردد", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Hesa Awal", Dari = "حصه اول", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Hesa Duwum", Dari = "حصه دوم", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Jagi Amir", Dari = "جاگی امیر", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Nerkh", Dari = "نرخ", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Saydabad", Dari = "سیدآباد", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 },
                        new Location { Name = "Syed Abad", Dari = "سید آباد", TypeId = 3, ParentId = wardakProvince.Id, IsActive = 1 }
                    });
                }
                
                // Zabul Province Districts
                var zabulProvince = savedProvinces.FirstOrDefault(p => p.Name == "Zabul");
                if (zabulProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Qalat", Dari = "قلات", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Arghandab", Dari = "ارغنداب", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Dai Chopan", Dari = "دای چوپان", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Jaldak", Dari = "جلدک", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Kalat", Dari = "کلات", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Mizan", Dari = "میزان", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Navur", Dari = "ناور", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 },
                        new Location { Name = "Shinkay", Dari = "شنکی", TypeId = 3, ParentId = zabulProvince.Id, IsActive = 1 }
                    });
                }
                
                // Badghis Province Districts
                var badghisProvince = savedProvinces.FirstOrDefault(p => p.Name == "Badghis");
                if (badghisProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Qala-e Naw", Dari = "قلعه نو", TypeId = 3, ParentId = badghisProvince.Id, IsActive = 1 },
                        new Location { Name = "Ab Kamari", Dari = "آب کماری", TypeId = 3, ParentId = badghisProvince.Id, IsActive = 1 },
                        new Location { Name = "Ghormach", Dari = "غورماچ", TypeId = 3, ParentId = badghisProvince.Id, IsActive = 1 },
                        new Location { Name = "Herat Rud", Dari = "هرات رود", TypeId = 3, ParentId = badghisProvince.Id, IsActive = 1 },
                        new Location { Name = "Morghab", Dari = "مرغاب", TypeId = 3, ParentId = badghisProvince.Id, IsActive = 1 }
                    });
                }
                
                // Daykundi Province Districts
                var daykoundiProvince = savedProvinces.FirstOrDefault(p => p.Name == "Daykundi");
                if (daykoundiProvince != null)
                {
                    districts.AddRange(new[]
                    {
                        new Location { Name = "Nili", Dari = "نیلی", TypeId = 3, ParentId = daykoundiProvince.Id, IsActive = 1 },
                        new Location { Name = "Gizab", Dari = "گیزاب", TypeId = 3, ParentId = daykoundiProvince.Id, IsActive = 1 },
                        new Location { Name = "Khadir", Dari = "خادر", TypeId = 3, ParentId = daykoundiProvince.Id, IsActive = 1 },
                        new Location { Name = "Miramor", Dari = "میرامور", TypeId = 3, ParentId = daykoundiProvince.Id, IsActive = 1 },
                        new Location { Name = "Shahristan", Dari = "شهرستان", TypeId = 3, ParentId = daykoundiProvince.Id, IsActive = 1 }
                    });
                }
                
            
            // Add missing districts to database
            if (districts.Any())
            {
                // Check which districts already exist
                var existingDistrictNames = await context.Locations
                    .Where(l => l.TypeId == 3)
                    .Select(l => l.Name)
                    .ToListAsync();
                
                var newDistricts = districts.Where(d => !existingDistrictNames.Contains(d.Name)).ToList();
                
                if (newDistricts.Any())
                {
                    await context.Locations.AddRangeAsync(newDistricts);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"✓ {newDistricts.Count} new districts added");
                }
                else
                {
                    Console.WriteLine("✓ All districts already exist");
                }
            }
            
            Console.WriteLine("✓ Districts seeding complete");

            // Save all changes
            await context.SaveChangesAsync();
            Console.WriteLine("✓ All lookup tables seeded successfully!");
        }
    }

}
