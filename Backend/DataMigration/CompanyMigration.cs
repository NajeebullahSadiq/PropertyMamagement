using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;

namespace DataMigration
{
    public class CompanyMigration
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MIGRATION_CONNECTION_STRING") 
            ?? "Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=SecurePassword@2024";
        
        private static CompanyStats stats = new CompanyStats();
        
        public static async Task RunCompanyMigration()
        {
            Console.WriteLine("\n=================================================================");
            Console.WriteLine("COMPANY/LICENSE MODULE MIGRATION");
            Console.WriteLine("=================================================================\n");
            
            try
            {
                // Ask if user wants to delete existing data
                Console.Write("Delete existing company data before migration? (y/n): ");
                string response = Console.ReadLine()?.ToLower() ?? "n";
                
                if (response == "y" || response == "yes")
                {
                    await DeleteExistingCompanyData();
                }
                
                // Load JSON files
                var mainFormRecords = await LoadMainFormRecords("mainform_full_records.json");
                var guaranteeRecords = await LoadGuaranteeRecords("guarantee_records.json");
                
                stats.TotalMainFormRecords = mainFormRecords.Count;
                stats.TotalGuaranteeRecords = guaranteeRecords.Count;
                
                Console.WriteLine($"Loaded {mainFormRecords.Count} company records");
                Console.WriteLine($"Loaded {guaranteeRecords.Count} guarantee records\n");
                
                // Start migration
                Console.WriteLine("Starting migration process...\n");
                await MigrateAllCompanies(mainFormRecords, guaranteeRecords);
                
                // Print statistics
                PrintCompanyStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFatal Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private static async Task DeleteExistingCompanyData()
        {
            Console.WriteLine("Deleting existing company data...");
            
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // Delete in correct order (respecting foreign keys)
                        await ExecuteDelete(conn, transaction, "org.\"Guarantors\"");
                        await ExecuteDelete(conn, transaction, "org.\"LicenseDetails\"");
                        await ExecuteDelete(conn, transaction, "org.\"CompanyOwner\"");
                        await ExecuteDelete(conn, transaction, "org.\"CompanyDetails\"");
                        
                        await transaction.CommitAsync();
                        Console.WriteLine("Existing data deleted successfully.\n");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error deleting data: {ex.Message}");
                        throw;
                    }
                }
            }
        }
        
        private static async Task ExecuteDelete(NpgsqlConnection conn, NpgsqlTransaction transaction, string tableName)
        {
            string query = $"DELETE FROM {tableName}";
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                int deleted = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"  Deleted {deleted} records from {tableName}");
            }
        }
        
        private static async Task<List<OldRecord>> LoadMainFormRecords(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Warning: File '{filename}' not found.");
                    return new List<OldRecord>();
                }
                
                string jsonContent = await File.ReadAllTextAsync(filename);
                jsonContent = jsonContent.Replace(": NaN", ": null");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                
                var records = JsonSerializer.Deserialize<List<OldRecord>>(jsonContent, options);
                return records ?? new List<OldRecord>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
                return new List<OldRecord>();
            }
        }
        
        private static async Task<List<GuaranteeRecord>> LoadGuaranteeRecords(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Warning: File '{filename}' not found.");
                    return new List<GuaranteeRecord>();
                }
                
                string jsonContent = await File.ReadAllTextAsync(filename);
                jsonContent = jsonContent.Replace(": NaN", ": null");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                
                var records = JsonSerializer.Deserialize<List<GuaranteeRecord>>(jsonContent, options);
                return records ?? new List<GuaranteeRecord>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
                return new List<GuaranteeRecord>();
            }
        }
        
        private static async Task MigrateAllCompanies(List<OldRecord> mainFormRecords, List<GuaranteeRecord> guaranteeRecords)
        {
            int processedCount = 0;
            
            foreach (var record in mainFormRecords)
            {
                try
                {
                    await MigrateCompanyRecord(record, guaranteeRecords);
                    processedCount++;
                    
                    if (processedCount % 50 == 0)
                    {
                        Console.WriteLine($"Processed {processedCount}/{stats.TotalMainFormRecords} records...");
                    }
                }
                catch (Exception ex)
                {
                    stats.Errors.Add(new CompanyError
                    {
                        CompanyName = record.RealEstateName ?? "Unknown",
                        LicenseNumber = record.LicenseNo?.ToString() ?? "N/A",
                        ErrorMessage = ex.Message
                    });
                    Console.WriteLine($"Error processing record (License: {record.LicenseNo}): {ex.Message}");
                }
            }
        }
        
        private static async Task MigrateCompanyRecord(OldRecord record, List<GuaranteeRecord> guaranteeRecords)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // Skip if no license number
                        if (!record.LicenseNo.HasValue || record.LicenseNo.Value == 0)
                        {
                            stats.Skipped.Add(new CompanySkipped
                            {
                                CompanyName = record.RealEstateName ?? "Unknown",
                                Reason = "Missing license number"
                            });
                            await transaction.CommitAsync();
                            return;
                        }
                        
                        // Check if company already exists
                        int? existingCompanyId = await CheckCompanyExists(record.LicenseNo.Value, conn, transaction);
                        
                        if (existingCompanyId.HasValue)
                        {
                            stats.Skipped.Add(new CompanySkipped
                            {
                                CompanyName = record.RealEstateName ?? "Unknown",
                                Reason = $"License {record.LicenseNo} already exists"
                            });
                            await transaction.CommitAsync();
                            return;
                        }
                        
                        // Insert CompanyDetails
                        int companyId = await InsertCompanyDetails(record, conn, transaction);
                        stats.CompaniesCreated++;
                        
                        // Insert CompanyOwner
                        int ownerId = await InsertCompanyOwner(record, companyId, conn, transaction);
                        stats.OwnersCreated++;
                        
                        // Insert LicenseDetails
                        await InsertLicenseDetails(record, companyId, conn, transaction);
                        stats.LicensesCreated++;
                        
                        // Insert Guarantors (find matching guarantee records by FK = RID)
                        var companyGuarantees = guaranteeRecords.Where(g => g.FK.HasValue && (int)g.FK.Value == record.RID).ToList();
                        foreach (var guarantee in companyGuarantees)
                        {
                            await InsertGuarantor(guarantee, companyId, conn, transaction);
                            stats.GuarantorsCreated++;
                        }
                        
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
        
        private static async Task<int?> CheckCompanyExists(int licenseNo, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                SELECT c.""Id"" 
                FROM org.""CompanyDetails"" c
                INNER JOIN org.""LicenseDetails"" l ON c.""Id"" = l.""CompanyId""
                WHERE l.""LicenseNumber"" = @licensenumber
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                // Format license number with KBL prefix for checking
                string formattedLicenseNumber = $"KBL-{licenseNo.ToString().PadLeft(5, '0')}";
                cmd.Parameters.AddWithValue("licensenumber", formattedLicenseNumber);
                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : null;
            }
        }
        
        private static async Task<int> InsertCompanyDetails(OldRecord record, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""CompanyDetails"" 
                (""Title"", ""ProvinceId"", ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@title, @provinceid, @status, @createdat, @createdby)
                RETURNING ""Id""";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("title", record.RealEstateName ?? "Unknown Company");
                cmd.Parameters.AddWithValue("provinceid", 1); // Default to Kabul
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
        
        private static async Task<int> InsertCompanyOwner(OldRecord record, int companyId, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""CompanyOwner"" 
                (""FirstName"", ""FatherName"", ""GrandFatherName"", ""DateofBirth"",
                 ""ElectronicNationalIdNumber"", ""PhoneNumber"", ""CompanyId"",
                 ""OwnerProvinceId"", ""OwnerDistrictId"",
                 ""PermanentProvinceId"", ""PermanentDistrictId"",
                 ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@firstname, @fathername, @grandfathername, @dateofbirth,
                        @electronicnationalidnumber, @phonenumber, @companyid,
                        @ownerprovinceid, @ownerdistrictid,
                        @permanentprovinceid, @permanentdistrictid,
                        @status, @createdat, @createdby)
                RETURNING ""Id""";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("firstname", record.FName ?? "Unknown");
                cmd.Parameters.AddWithValue("fathername", record.FathName ?? "Unknown");
                cmd.Parameters.AddWithValue("grandfathername", record.GFName ?? (object)DBNull.Value);
                
                // Parse date of birth
                DateTime? dob = ParseDate(record.DOB);
                cmd.Parameters.AddWithValue("dateofbirth", dob ?? (object)DBNull.Value);
                
                cmd.Parameters.AddWithValue("electronicnationalidnumber", record.TazkeraNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("phonenumber", record.ContactNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("companyid", companyId);
                
                // Get or create location IDs
                int? permProvinceId = await GetOrCreateProvinceId(record.PerProvince, conn, transaction);
                int? permDistrictId = await GetOrCreateDistrictId(record.PerWoloswaly, permProvinceId, conn, transaction);
                int? tempProvinceId = await GetOrCreateProvinceId(record.TempProvince, conn, transaction);
                int? tempDistrictId = await GetOrCreateDistrictId(record.TempWoloswaly, tempProvinceId, conn, transaction);
                
                cmd.Parameters.AddWithValue("ownerprovinceid", tempProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("ownerdistrictid", tempDistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("permanentprovinceid", permProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("permanentdistrictid", permDistrictId ?? (object)DBNull.Value);
                
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
        
        private static async Task InsertLicenseDetails(OldRecord record, int companyId, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""LicenseDetails"" 
                (""LicenseNumber"", ""ProvinceId"", ""IssueDate"", ""ExpireDate"",
                 ""OfficeAddress"", ""CompanyId"", ""LicenseType"", ""LicenseCategory"",
                 ""RoyaltyAmount"", ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@licensenumber, @provinceid, @issuedate, @expiredate,
                        @officeaddress, @companyid, @licensetype, @licensecategory,
                        @royaltyamount, @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                // Format license number as KBL-00001 (Kabul prefix with 5-digit number)
                string formattedLicenseNumber = $"KBL-{record.LicenseNo.ToString().PadLeft(5, '0')}";
                cmd.Parameters.AddWithValue("licensenumber", formattedLicenseNumber);
                cmd.Parameters.AddWithValue("provinceid", 1); // Default to Kabul
                
                // Parse dates
                DateTime? issueDate = ParseDateFromParts(record.SYear, record.SMonth, record.SDay);
                DateTime? expireDate = ParseDateFromParts(record.EYear, record.EMonth, record.EDay);
                
                cmd.Parameters.AddWithValue("issuedate", issueDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("expiredate", expireDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("officeaddress", record.ExactAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("companyid", companyId);
                cmd.Parameters.AddWithValue("licensetype", record.LicenseType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("licensecategory", DetermineLicenseCategory(record.LicenseType));
                cmd.Parameters.AddWithValue("royaltyamount", record.CreditRightAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        private static async Task InsertGuarantor(GuaranteeRecord record, int companyId, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""Guarantors"" 
                (""FirstName"", ""FatherName"", ""GrandFatherName"", ""CompanyId"",
                 ""ElectronicNationalIdNumber"", ""PhoneNumber"",
                 ""GuaranteeTypeId"", ""PropertyDocumentNumber"",
                 ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@firstname, @fathername, @grandfathername, @companyid,
                        @electronicnationalidnumber, @phonenumber,
                        @guaranteetypeid, @propertydocumentnumber,
                        @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("firstname", record.GName ?? "Unknown");
                cmd.Parameters.AddWithValue("fathername", record.GFName ?? "Unknown");
                cmd.Parameters.AddWithValue("grandfathername", record.GRelation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("companyid", companyId);
                cmd.Parameters.AddWithValue("electronicnationalidnumber", record.GTazkeraNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("phonenumber", record.GContact ?? (object)DBNull.Value);
                
                // Guarantee type (1=Sharia Deed, 2=Customary Deed, 3=Cash)
                cmd.Parameters.AddWithValue("guaranteetypeid", DetermineGuaranteeType(record.GuaranteeType));
                
                // Parse PropertyDocumentNumber to long (bigint)
                long? propertyDocNumber = null;
                if (!string.IsNullOrWhiteSpace(record.GReferenceNo) && long.TryParse(record.GReferenceNo, out long parsedNumber))
                {
                    propertyDocNumber = parsedNumber;
                }
                cmd.Parameters.AddWithValue("propertydocumentnumber", propertyDocNumber ?? (object)DBNull.Value);
                
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        // Helper Methods
        private static async Task<int?> GetOrCreateProvinceId(string provinceName, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
                return null;
            
            // For Kabul, always return 1
            if (provinceName.Contains("کابل") || provinceName.ToLower().Contains("kabul"))
                return 1;
            
            // Try to find existing province
            string query = @"
                SELECT ""ID"" FROM look.""Location"" 
                WHERE ""Name"" = @name AND ""ParentID"" IS NULL
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", provinceName);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
            }
            
            // If not found, create it
            string insertQuery = @"
                INSERT INTO look.""Location"" (""Name"", ""Dari"", ""ParentID"", ""IsActive"")
                VALUES (@name, @dari, NULL, 1)
                RETURNING ""ID""";
            
            using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
            {
                insertCmd.Parameters.AddWithValue("name", provinceName);
                insertCmd.Parameters.AddWithValue("dari", provinceName);
                
                var insertResult = await insertCmd.ExecuteScalarAsync();
                return Convert.ToInt32(insertResult);
            }
        }
        
        private static async Task<int?> GetOrCreateDistrictId(string districtName, int? provinceId, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(districtName) || !provinceId.HasValue)
                return null;
            
            // Try to find existing district
            string query = @"
                SELECT ""ID"" FROM look.""Location"" 
                WHERE ""Name"" = @name AND ""ParentID"" = @parentid
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", districtName);
                cmd.Parameters.AddWithValue("parentid", provinceId.Value);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
            }
            
            // If not found, create it
            string insertQuery = @"
                INSERT INTO look.""Location"" (""Name"", ""Dari"", ""ParentID"", ""IsActive"")
                VALUES (@name, @dari, @parentid, 1)
                RETURNING ""ID""";
            
            using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
            {
                insertCmd.Parameters.AddWithValue("name", districtName);
                insertCmd.Parameters.AddWithValue("dari", districtName);
                insertCmd.Parameters.AddWithValue("parentid", provinceId.Value);
                
                var insertResult = await insertCmd.ExecuteScalarAsync();
                return Convert.ToInt32(insertResult);
            }
        }
        
        private static DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;
            
            try
            {
                // Try to parse various date formats
                if (DateTime.TryParse(dateString, out DateTime result))
                    return result;
            }
            catch
            {
                // Ignore parsing errors
            }
            
            return null;
        }
        
        private static DateTime? ParseDateFromParts(double? year, double? month, double? day)
        {
            if (!year.HasValue || !month.HasValue || !day.HasValue)
                return null;
            
            try
            {
                int y = (int)year.Value;
                int m = (int)month.Value;
                int d = (int)day.Value;
                
                if (y >= 1 && y <= 9999 && m >= 1 && m <= 12 && d >= 1 && d <= 31)
                {
                    return new DateTime(y, m, d);
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            
            return null;
        }
        
        private static string DetermineLicenseCategory(string licenseType)
        {
            if (string.IsNullOrWhiteSpace(licenseType))
                return "جدید";
            
            if (licenseType.Contains("تجدید") || licenseType.ToLower().Contains("renewal"))
                return "تجدید";
            
            if (licenseType.Contains("مثنی") || licenseType.ToLower().Contains("duplicate"))
                return "مثنی";
            
            return "جدید";
        }
        
        private static int DetermineGuaranteeType(string guaranteeType)
        {
            if (string.IsNullOrWhiteSpace(guaranteeType))
                return 1; // Default to Sharia Deed
            
            if (guaranteeType.Contains("عرفی") || guaranteeType.ToLower().Contains("customary"))
                return 2; // Customary Deed
            
            if (guaranteeType.Contains("نقد") || guaranteeType.ToLower().Contains("cash"))
                return 3; // Cash
            
            return 1; // Sharia Deed
        }
        
        private static void PrintCompanyStatistics()
        {
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("COMPANY MODULE MIGRATION COMPLETED");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"Total main form records: {stats.TotalMainFormRecords}");
            Console.WriteLine($"Total guarantee records: {stats.TotalGuaranteeRecords}");
            Console.WriteLine($"Companies created: {stats.CompaniesCreated}");
            Console.WriteLine($"Owners created: {stats.OwnersCreated}");
            Console.WriteLine($"Licenses created: {stats.LicensesCreated}");
            Console.WriteLine($"Guarantors created: {stats.GuarantorsCreated}");
            Console.WriteLine($"Records skipped: {stats.Skipped.Count}");
            Console.WriteLine($"Errors encountered: {stats.Errors.Count}");
            
            if (stats.Skipped.Count > 0)
            {
                Console.WriteLine("\nSkipped Records:");
                foreach (var skip in stats.Skipped.Take(10))
                {
                    Console.WriteLine($"  - {skip.CompanyName}: {skip.Reason}");
                }
                if (stats.Skipped.Count > 10)
                    Console.WriteLine($"  ... and {stats.Skipped.Count - 10} more");
            }
            
            if (stats.Errors.Count > 0)
            {
                Console.WriteLine("\nErrors:");
                foreach (var error in stats.Errors.Take(10))
                {
                    Console.WriteLine($"  - {error.CompanyName} (License: {error.LicenseNumber}): {error.ErrorMessage}");
                }
                if (stats.Errors.Count > 10)
                    Console.WriteLine($"  ... and {stats.Errors.Count - 10} more");
            }
            
            Console.WriteLine(new string('=', 80));
        }
    }
    
    // Statistics Classes
    public class CompanyStats
    {
        public int TotalMainFormRecords { get; set; }
        public int TotalGuaranteeRecords { get; set; }
        public int CompaniesCreated { get; set; }
        public int OwnersCreated { get; set; }
        public int LicensesCreated { get; set; }
        public int GuarantorsCreated { get; set; }
        public List<CompanyError> Errors { get; set; } = new List<CompanyError>();
        public List<CompanySkipped> Skipped { get; set; } = new List<CompanySkipped>();
    }
    
    public class CompanyError
    {
        public string CompanyName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
    
    public class CompanySkipped
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
