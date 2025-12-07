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

            // Seed EducationLevels
            if (!context.EducationLevels.Any())
            {
                var educationLevels = new[]
                {
                    new EducationLevel { Name = "Illiterate", Sorter = "01" },
                    new EducationLevel { Name = "Primary School", Sorter = "02" },
                    new EducationLevel { Name = "Secondary School", Sorter = "03" },
                    new EducationLevel { Name = "High School", Sorter = "04" },
                    new EducationLevel { Name = "Diploma", Sorter = "05" },
                    new EducationLevel { Name = "Bachelor's Degree", Sorter = "06" },
                    new EducationLevel { Name = "Master's Degree", Sorter = "07" },
                    new EducationLevel { Name = "PhD/Doctorate", Sorter = "08" },
                    new EducationLevel { Name = "Religious Education", Sorter = "09" },
                    new EducationLevel { Name = "Technical/Vocational", Sorter = "10" }
                };
                await context.EducationLevels.AddRangeAsync(educationLevels);
                Console.WriteLine("✓ EducationLevels seeded");
            }

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
            if (!context.Locations.Any())
            {
                var locations = new List<Location>();
                
                // Provinces (TypeId = 2 for provinces based on controller code)
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
                
                locations.AddRange(provinces);
                await context.Locations.AddRangeAsync(provinces);
                await context.SaveChangesAsync(); // Save provinces first to get their IDs
                
                // Now add districts (TypeId = 3 for districts) with ParentId referencing provinces
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
                
                // Add sample districts for a few more provinces to provide good coverage
                // Add districts for other major provinces as needed...
                
                if (districts.Any())
                {
                    await context.Locations.AddRangeAsync(districts);
                    Console.WriteLine($"✓ Districts seeded for major provinces ({districts.Count} districts)");
                }
                
                Console.WriteLine("✓ Locations (Provinces and Districts) seeded");
            }

            // Save all changes
            await context.SaveChangesAsync();
            Console.WriteLine("✓ All lookup tables seeded successfully!");
        }
    }

}
