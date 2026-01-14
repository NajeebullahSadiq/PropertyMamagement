using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;

namespace WebAPIBackend.Configuration
{
    /// <summary>
    /// Database seeder for initial data and views.
    /// NOTE: Schema/table creation is now handled by module-based migrations in Infrastructure/Migrations/
    /// This seeder only handles:
    /// - Role and permission seeding
    /// - Default admin user creation
    /// - Lookup table data seeding
    /// - View creation/updates
    /// - One-time data migrations
    /// </summary>
    public class DatabaseSeeder
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database exists and apply pending migrations
            await context.Database.MigrateAsync();

            // Seed RBAC roles using Identity API (safer than raw SQL)
            await SeedRolesAndPermissions(roleManager);

            // Seed default admin user
            await SeedAdminUser(userManager);

            // Create/Update database views
            await CreateDatabaseViews(context);

            // Seed lookup tables with initial data
            await SeedLookupTables(context);
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
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
                    PhoneNumber = "0700000000",
                    UserRole = UserRoles.Admin,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                }
            }
            else
            {
                // Update existing admin user with new fields if missing
                if (string.IsNullOrEmpty(adminUser.UserRole))
                {
                    adminUser.UserRole = UserRoles.Admin;
                    adminUser.CreatedAt ??= DateTime.UtcNow;
                    adminUser.CreatedBy ??= "system";
                    await userManager.UpdateAsync(adminUser);
                }
            }
        }

        private static async Task SeedRolesAndPermissions(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in UserRoles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);

                    var permissions = RolePermissions.GetPermissionsForRole(roleName);
                    foreach (var permission in permissions)
                    {
                        await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
                    }
                }
                else
                {
                    // Update existing role with any missing permissions
                    var role = await roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var existingClaims = await roleManager.GetClaimsAsync(role);
                        var existingPermissions = existingClaims
                            .Where(c => c.Type == CustomClaimTypes.Permission)
                            .Select(c => c.Value)
                            .ToList();
                        var requiredPermissions = RolePermissions.GetPermissionsForRole(roleName);

                        foreach (var permission in requiredPermissions)
                        {
                            if (!existingPermissions.Contains(permission))
                            {
                                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
                            }
                        }
                    }
                }
            }
        }

        private static async Task CreateDatabaseViews(AppDbContext context)
        {
            // Create UserProfileWithCompany view
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE OR REPLACE VIEW public.""UserProfileWithCompany"" AS
                    SELECT 
                        u.""Id"" AS ""UserId"",
                        u.""Email"",
                        u.""UserName"",
                        u.""FirstName"",
                        u.""LastName"",
                        u.""PhotoPath"",
                        c.""Title"" AS ""CompanyName"",
                        COALESCE(c.""PhoneNumber"", u.""PhoneNumber"") AS ""PhoneNumber""
                    FROM public.""AspNetUsers"" u
                    LEFT JOIN org.""CompanyDetails"" c ON u.""CompanyId"" = c.""Id"";
                ");
            }
            catch (Exception) { /* View may already exist */ }

            // Create LicenseView
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE OR REPLACE VIEW public.""LicenseView"" AS
                    SELECT 
                        cd.""Id"" AS ""CompanyId"",
                        co.""PhoneNumber"",
                        co.""WhatsAppNumber"",
                        cd.""Title"",
                        cd.""TIN"" AS ""Tin"",
                        co.""Name"" AS ""FirstName"",
                        co.""FatherName"",
                        co.""GrandFatherName"",
                        ld.""LicenseNumber"",
                        ld.""LicenseDate"" AS ""IssueDate"",
                        ld.""LicenseExpireDate"" AS ""ExpireDate"",
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
            catch (Exception) { /* View may already exist */ }

            // Create GetPrintType view for property printing
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
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
                        pd.""FilePath"",
                        pd.""PreviousDocumentsPath"",
                        pd.""ExistingDocumentsPath"",
                        pd.""DocumentType"",
                        pd.""IssuanceNumber"",
                        pd.""IssuanceDate"",
                        pd.""SerialNumber"",
                        pd.""TransactionDate"",
                        pd.""PNumber"",
                        pa_prov.""Name"" as ""Province"",
                        pa_dist.""Name"" as ""District"",
                        pa_prov.""Dari"" as ""ProvinceDari"",
                        pa_dist.""Dari"" as ""DistrictDari"",
                        pa.""Village"",
                        sd.""FirstName"" as ""SellerFirstName"",
                        sd.""FatherName"" as ""SellerFatherName"",
                        sd.""IndentityCardNumber"" as ""SellerIndentityCardNumber"",
                        sd.""PaddressVillage"" as ""SellerVillage"",
                        sd.""TaddressVillage"" as ""TSellerVillage"",
                        sd.""photo"" as ""SellerPhoto"",
                        s_perm_prov.""Name"" as ""SellerProvince"",
                        s_perm_dist.""Name"" as ""SellerDistrict"",
                        s_perm_prov.""Dari"" as ""SellerProvinceDari"",
                        s_perm_dist.""Dari"" as ""SellerDistrictDari"",
                        s_temp_prov.""Name"" as ""TSellerProvince"",
                        s_temp_dist.""Name"" as ""TSellerDistrict"",
                        s_temp_prov.""Dari"" as ""TSellerProvinceDari"",
                        s_temp_dist.""Dari"" as ""TSellerDistrictDari"",
                        bd.""FirstName"" as ""BuyerFirstName"",
                        bd.""FatherName"" as ""BuyerFatherName"",
                        bd.""IndentityCardNumber"" as ""BuyerIndentityCardNumber"",
                        bd.""PaddressVillage"" as ""BuyerVillage"",
                        bd.""TaddressVillage"" as ""TBuyerVillage"",
                        bd.""photo"" as ""BuyerPhoto"",
                        b_perm_prov.""Name"" as ""BuyerProvince"",
                        b_perm_dist.""Name"" as ""BuyerDistrict"",
                        b_perm_prov.""Dari"" as ""BuyerProvinceDari"",
                        b_perm_dist.""Dari"" as ""BuyerDistrictDari"",
                        b_temp_prov.""Name"" as ""TBuyerProvince"",
                        b_temp_dist.""Name"" as ""TBuyerDistrict"",
                        b_temp_prov.""Dari"" as ""TBuyerProvinceDari"",
                        b_temp_dist.""Dari"" as ""TBuyerDistrictDari"",
                        wd1.""FirstName"" as ""WitnessOneFirstName"",
                        wd1.""FatherName"" as ""WitnessOneFatherName"",
                        wd1.""IndentityCardNumber"" as ""WitnessOneIndentityCardNumber"",
                        wd2.""FirstName"" as ""WitnessTwoFirstName"",
                        wd2.""FatherName"" as ""WitnessTwoFatherName"",
                        wd2.""IndentityCardNumber"" as ""WitnessTwoIndentityCardNumber"",
                        ut.""Name"" as ""UnitType"",
                        tt.""Name"" as ""TransactionType""
                    FROM tr.""PropertyDetails"" pd
                    LEFT JOIN look.""PropertyType"" pt ON pd.""PropertyTypeId"" = pt.""Id""
                    LEFT JOIN look.""PUnitType"" ut ON pd.""PUnitTypeId"" = ut.""Id""
                    LEFT JOIN look.""TransactionType"" tt ON pd.""TransactionTypeId"" = tt.""Id""
                    LEFT JOIN tr.""PropertyAddress"" pa ON pd.""Id"" = pa.""PropertyDetailsId""
                    LEFT JOIN look.""Location"" pa_prov ON pa.""ProvinceId"" = pa_prov.""ID""
                    LEFT JOIN look.""Location"" pa_dist ON pa.""DistrictId"" = pa_dist.""ID""
                    LEFT JOIN tr.""SellerDetails"" sd ON pd.""Id"" = sd.""PropertyDetailsId""
                    LEFT JOIN look.""Location"" s_perm_prov ON sd.""PaddressProvinceId"" = s_perm_prov.""ID""
                    LEFT JOIN look.""Location"" s_perm_dist ON sd.""PaddressDistrictId"" = s_perm_dist.""ID""
                    LEFT JOIN look.""Location"" s_temp_prov ON sd.""TaddressProvinceId"" = s_temp_prov.""ID""
                    LEFT JOIN look.""Location"" s_temp_dist ON sd.""TaddressDistrictId"" = s_temp_dist.""ID""
                    LEFT JOIN tr.""BuyerDetails"" bd ON pd.""Id"" = bd.""PropertyDetailsId""
                    LEFT JOIN look.""Location"" b_perm_prov ON bd.""PaddressProvinceId"" = b_perm_prov.""ID""
                    LEFT JOIN look.""Location"" b_perm_dist ON bd.""PaddressDistrictId"" = b_perm_dist.""ID""
                    LEFT JOIN look.""Location"" b_temp_prov ON bd.""TaddressProvinceId"" = b_temp_prov.""ID""
                    LEFT JOIN look.""Location"" b_temp_dist ON bd.""TaddressDistrictId"" = b_temp_dist.""ID""
                    LEFT JOIN LATERAL (
                        SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                        FROM tr.""WitnessDetails""
                        WHERE ""PropertyDetailsId"" = pd.""Id""
                        ORDER BY ""Id"" ASC LIMIT 1
                    ) wd1 ON true
                    LEFT JOIN LATERAL (
                        SELECT ""FirstName"", ""FatherName"", ""IndentityCardNumber""
                        FROM tr.""WitnessDetails""
                        WHERE ""PropertyDetailsId"" = pd.""Id""
                        ORDER BY ""Id"" ASC OFFSET 1 LIMIT 1
                    ) wd2 ON true
                    WHERE pd.""iscomplete"" = true;
                ");
            }
            catch (Exception) { /* View may already exist */ }
        }

        private static async Task SeedLookupTables(AppDbContext context)
        {
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
            }

            // Seed Locations (Afghanistan Provinces)
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
            
            // Upsert provinces
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

            // Seed districts for provinces
            var savedProvinces = await context.Locations.Where(l => l.TypeId == 2).ToListAsync();
            var districts = new List<Location>();
            
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
                    new Location { Name = "Injil", Dari = "انجیل", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Guzara", Dari = "گذره", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Pashtun Zarghun", Dari = "پشتون زرغون", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Karukh", Dari = "کرخ", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Shindand", Dari = "شیندند", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 },
                    new Location { Name = "Zinda Jan", Dari = "زنده جان", TypeId = 3, ParentId = heratProvince.Id, IsActive = 1 }
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
                    new Location { Name = "Dehdadi", Dari = "دهدادی", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Nahri Shahi", Dari = "نهر شاهی", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 },
                    new Location { Name = "Sholgara", Dari = "شولگره", TypeId = 3, ParentId = balkhProvince.Id, IsActive = 1 }
                });
            }
            
            // Nangarhar Province Districts
            var nangarharProvince = savedProvinces.FirstOrDefault(p => p.Name == "Nangarhar");
            if (nangarharProvince != null)
            {
                districts.AddRange(new[]
                {
                    new Location { Name = "Jalalabad", Dari = "جلال آباد", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                    new Location { Name = "Behsud", Dari = "بهسود", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                    new Location { Name = "Surkh Rod", Dari = "سرخ رود", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                    new Location { Name = "Kama", Dari = "کامه", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 },
                    new Location { Name = "Rodat", Dari = "رودات", TypeId = 3, ParentId = nangarharProvince.Id, IsActive = 1 }
                });
            }
            
            // Kandahar Province Districts
            var kandaharProvince = savedProvinces.FirstOrDefault(p => p.Name == "Kandahar");
            if (kandaharProvince != null)
            {
                districts.AddRange(new[]
                {
                    new Location { Name = "Kandahar Center", Dari = "مرکز کندهار", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Daman", Dari = "دامان", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Arghandab", Dari = "ارغنداب", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Panjwayi", Dari = "پنجوایی", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 },
                    new Location { Name = "Spin Boldak", Dari = "سپین بولدک", TypeId = 3, ParentId = kandaharProvince.Id, IsActive = 1 }
                });
            }

            // Add missing districts to database
            if (districts.Any())
            {
                var existingDistrictNames = await context.Locations
                    .Where(l => l.TypeId == 3)
                    .Select(l => l.Name)
                    .ToListAsync();
                
                var newDistricts = districts.Where(d => !existingDistrictNames.Contains(d.Name)).ToList();
                
                if (newDistricts.Any())
                {
                    await context.Locations.AddRangeAsync(newDistricts);
                    await context.SaveChangesAsync();
                }
            }

            // Save all changes
            await context.SaveChangesAsync();
        }
    }
}
