using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;

namespace DataMigration
{
    class Program
    {
        // Get connection string from environment variable or use default
        // Use 127.0.0.1 instead of localhost to force TCP connection (avoids peer authentication)
        private static string connectionString = Environment.GetEnvironmentVariable("MIGRATION_CONNECTION_STRING") 
            ?? "Host=127.0.0.1;Port=5432;Database=PRMIS;Username=postgres;Password=SecurePassword@2024";
        
        private static MigrationStats stats = new MigrationStats();
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("Data Migration Tool - Access to PostgreSQL");
            Console.WriteLine("=================================================================\n");
            
            try
            {
                // Read JSON file
                Console.WriteLine("Loading data from mainform_records.json...");
                string jsonPath = "mainform_records.json";
                
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Error: File '{jsonPath}' not found!");
                    Console.WriteLine("Please ensure mainform_records.json is in the same directory.");
                    return;
                }
                
                string jsonContent = await File.ReadAllTextAsync(jsonPath);
                var oldRecords = JsonSerializer.Deserialize<List<OldRecord>>(jsonContent);
                
                stats.TotalRecords = oldRecords.Count;
                Console.WriteLine($"Loaded {stats.TotalRecords} records\n");
                
                // Start migration
                Console.WriteLine("Starting migration process...\n");
                await MigrateAllRecords(oldRecords);
                
                // Print statistics
                PrintStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFatal Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static async Task MigrateAllRecords(List<OldRecord> records)
        {
            int processedCount = 0;
            int batchSize = 50;
            
            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    await MigrateRecord(records[i]);
                    processedCount++;
                    
                    if (processedCount % 100 == 0)
                    {
                        Console.WriteLine($"Processed {processedCount}/{stats.TotalRecords} records...");
                    }
                }
                catch (Exception ex)
                {
                    stats.Errors.Add(new MigrationError
                    {
                        RecordId = records[i].RID,
                        ErrorMessage = ex.Message,
                        RecordData = records[i]
                    });
                    Console.WriteLine($"Error processing record {records[i].RID}: {ex.Message}");
                }
            }
        }
        
        static async Task MigrateRecord(OldRecord record)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // Check if company already exists
                        using (var checkCmd = new NpgsqlCommand(
                            "SELECT \"Id\" FROM org.\"CompanyDetails\" WHERE \"Id\" = @id", conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("id", record.RID);
                            var exists = await checkCmd.ExecuteScalarAsync();
                            
                            if (exists != null)
                            {
                                stats.Skipped.Add(new SkippedRecord
                                {
                                    RecordId = record.RID,
                                    Reason = "Company already exists"
                                });
                                await transaction.RollbackAsync();
                                return;
                            }
                        }
                        
                        // Get Kabul Province ID (all licenses are from Kabul)
                        // Use ProvinceId = 1 for all records
                        int kabulProvinceId = 1;
                        
                        // 1. Insert CompanyDetails (registered in Kabul)
                        int companyId = await InsertCompanyDetails(record, kabulProvinceId, conn, transaction);
                        stats.CompaniesCreated++;
                        
                        // 2. Insert CompanyOwner (with their actual address)
                        if (!string.IsNullOrWhiteSpace(record.FName) && !string.IsNullOrWhiteSpace(record.FathName))
                        {
                            await InsertCompanyOwner(record, companyId, conn, transaction);
                            stats.OwnersCreated++;
                        }
                        
                        // 3. Insert LicenseDetails (issued in Kabul)
                        if (record.LicenseNo.HasValue)
                        {
                            await InsertLicenseDetails(record, companyId, kabulProvinceId, conn, transaction);
                            stats.LicensesCreated++;
                        }
                        
                        // 4. Insert CompanyCancellationInfo (if cancelled)
                        if (!string.IsNullOrWhiteSpace(record.LicnsCancelNo))
                        {
                            await InsertCancellationInfo(record, companyId, conn, transaction);
                            stats.CancellationsCreated++;
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
        
        static async Task<int> InsertCompanyDetails(OldRecord record, int kabulProvinceId, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""CompanyDetails"" 
                (""Id"", ""Title"", ""TIN"", ""ProvinceId"", ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@id, @title, @tin, @provinceid, @status, @createdat, @createdby)
                RETURNING ""Id""";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("id", record.RID);
                // Provide default title if missing
                string title = string.IsNullOrWhiteSpace(record.RealEstateName) 
                    ? $"رهنما شماره {record.RID}" 
                    : record.RealEstateName;
                cmd.Parameters.AddWithValue("title", title);
                cmd.Parameters.AddWithValue("tin", record.TIN ?? (object)DBNull.Value);
                // All companies are registered in Kabul (ProvinceId = 1)
                cmd.Parameters.AddWithValue("provinceid", kabulProvinceId);
                cmd.Parameters.AddWithValue("status", GetActiveStatus(record.Halat, record.LicnsCancelNo));
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
        
        static async Task InsertCompanyOwner(OldRecord record, int companyId, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""CompanyOwner"" 
                (""FirstName"", ""FatherName"", ""GrandFatherName"", ""EducationLevelId"", ""DateofBirth"",
                 ""ElectronicNationalIdNumber"", ""PhoneNumber"", ""CompanyId"",
                 ""OwnerProvinceId"", ""OwnerDistrictId"", ""OwnerVillage"",
                 ""PermanentProvinceId"", ""PermanentDistrictId"", ""PermanentVillage"",
                 ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@firstname, @fathername, @grandfathername, @educationlevelid, @dateofbirth,
                        @electronicnationalidnumber, @phonenumber, @companyid,
                        @ownerprovinceid, @ownerdistrictid, @ownervillage,
                        @permanentprovinceid, @permanentdistrictid, @permanentvillage,
                        @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("firstname", record.FName);
                cmd.Parameters.AddWithValue("fathername", record.FathName);
                cmd.Parameters.AddWithValue("grandfathername", record.GFName ?? (object)DBNull.Value);
                
                int? educationLevelId = await GetEducationLevelId(record.Education, conn, transaction);
                cmd.Parameters.AddWithValue("educationlevelid", educationLevelId ?? (object)DBNull.Value);
                
                cmd.Parameters.AddWithValue("dateofbirth", ParseDate(record.DOB));
                // Truncate ElectronicNationalIdNumber to 20 characters if too long
                string? electronicId = record.TazkeraNo;
                if (!string.IsNullOrEmpty(electronicId) && electronicId.Length > 20)
                    electronicId = electronicId.Substring(0, 20);
                cmd.Parameters.AddWithValue("electronicnationalidnumber", electronicId ?? (object)DBNull.Value);
                // Truncate PhoneNumber to 20 characters if too long
                string? phoneNumber = record.ContactNo;
                if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length > 20)
                    phoneNumber = phoneNumber.Substring(0, 20);
                cmd.Parameters.AddWithValue("phonenumber", phoneNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("companyid", companyId);
                
                // Owner's address
                int? ownerProvinceId = await GetOrCreateProvinceId(record.PerProvince, conn, transaction);
                int? ownerDistrictId = await GetOrCreateDistrictId(record.PerWoloswaly, ownerProvinceId, conn, transaction);
                cmd.Parameters.AddWithValue("ownerprovinceid", ownerProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("ownerdistrictid", ownerDistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("ownervillage", record.ExactAddress ?? (object)DBNull.Value);
                
                // Permanent address
                int? permProvinceId = await GetOrCreateProvinceId(record.TempProvince, conn, transaction);
                int? permDistrictId = await GetOrCreateDistrictId(record.TempWoloswaly, permProvinceId, conn, transaction);
                cmd.Parameters.AddWithValue("permanentprovinceid", permProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("permanentdistrictid", permDistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("permanentvillage", record.ExactAddress ?? (object)DBNull.Value);
                
                cmd.Parameters.AddWithValue("status", GetActiveStatus(record.Halat, null));
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        static async Task InsertLicenseDetails(OldRecord record, int companyId, int kabulProvinceId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""LicenseDetails"" 
                (""LicenseNumber"", ""ProvinceId"", ""IssueDate"", ""ExpireDate"", ""OfficeAddress"",
                 ""CompanyId"", ""LicenseType"", ""LicenseCategory"",
                 ""RoyaltyAmount"", ""RoyaltyDate"", ""PenaltyAmount"", ""TariffNumber"",
                 ""HrLetter"", ""HrLetterDate"", ""IsComplete"", ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@licensenumber, @provinceid, @issuedate, @expiredate, @officeaddress,
                        @companyid, @licensetype, @licensecategory,
                        @royaltyamount, @royaltydate, @penaltyamount, @tariffnumber,
                        @hrletter, @hrletterdate, @iscomplete, @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                // Format license number as KBL-00000{number}
                string formattedLicenseNumber = $"KBL-{record.LicenseNo.ToString().PadLeft(5, '0')}";
                cmd.Parameters.AddWithValue("licensenumber", formattedLicenseNumber);
                // All licenses are issued in Kabul (ProvinceId = 1)
                cmd.Parameters.AddWithValue("provinceid", kabulProvinceId);
                
                // Create dates from year/month/day
                object issueDate = CreateDateObject(record.SYear, record.SMonth, record.SDay);
                object expireDate = CreateDateObject(record.EYear, record.EMonth, record.EDay);
                object royaltyDate = CreateDateObject(record.CreditRightYear, record.CreditRightMonth, record.CreditRightDay);
                object hrDate = CreateDateObject(record.Combo211, record.HRMonth, record.HRDay);
                
                cmd.Parameters.AddWithValue("issuedate", issueDate);
                cmd.Parameters.AddWithValue("expiredate", expireDate);
                cmd.Parameters.AddWithValue("officeaddress", record.ExactAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("companyid", companyId);
                cmd.Parameters.AddWithValue("licensetype", record.LicenseType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("licensecategory", record.LicenseType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("royaltyamount", record.CreditRightAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("royaltydate", royaltyDate);
                cmd.Parameters.AddWithValue("penaltyamount", record.LateFine ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("tariffnumber", record.CreditRightNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("hrletter", record.HRNo.HasValue ? record.HRNo.ToString() : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("hrletterdate", hrDate);
                cmd.Parameters.AddWithValue("iscomplete", true);
                cmd.Parameters.AddWithValue("status", GetActiveStatus(record.Halat, record.LicnsCancelNo));
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        static async Task InsertCancellationInfo(OldRecord record, int companyId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""CompanyCancellationInfo"" 
                (""CompanyId"", ""LicenseCancellationLetterNumber"", ""LicenseCancellationLetterDate"",
                 ""Remarks"", ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@companyid, @licensecancellationletternumber, @licensecancellationletterdate,
                        @remarks, @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("companyid", companyId);
                cmd.Parameters.AddWithValue("licensecancellationletternumber", record.LicnsCancelNo);
                
                object cancelDate = CreateDateObject(record.CancelYear, record.CancelMonth, record.CancelDay);
                cmd.Parameters.AddWithValue("licensecancellationletterdate", cancelDate);
                cmd.Parameters.AddWithValue("remarks", record.Remarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        // Helper methods
        static object ParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return DBNull.Value;
            
            // Try to parse the date string
            // Common formats: "YYYY-MM-DD", "YYYY/MM/DD", "DD/MM/YYYY"
            if (DateTime.TryParse(dateString, out DateTime parsedDate))
            {
                return parsedDate;
            }
            
            // If parsing fails, return NULL
            return DBNull.Value;
        }
        
        static object CreateDateObject(double? year, double? month, double? day)
        {
            if (!year.HasValue || !month.HasValue || !day.HasValue)
                return DBNull.Value;
            
            try
            {
                int y = (int)year.Value;
                int m = (int)month.Value;
                int d = (int)day.Value;
                
                // Validate ranges
                if (y < 1 || y > 9999 || m < 1 || m > 12 || d < 1 || d > 31)
                    return DBNull.Value;
                
                // Create DateTime (Jalali dates stored as-is, no conversion)
                // Note: This assumes the dates are already in a valid format
                // For Jalali calendar, you might need a conversion library
                return new DateTime(y, m, d);
            }
            catch
            {
                return DBNull.Value;
            }
        }
        
        static string? CreateDateString(double? year, double? month, double? day)
        {
            if (!year.HasValue || !month.HasValue || !day.HasValue)
                return null;
            
            int y = (int)year.Value;
            int m = (int)month.Value;
            int d = (int)day.Value;
            
            // Return Jalali date string (you may need to convert to Gregorian)
            return $"{y:0000}-{m:00}-{d:00}";
        }
        
        static bool GetActiveStatus(string? halatText, string? cancelText)
        {
            // Check if cancelled - فسخ شده means cancelled
            if (!string.IsNullOrWhiteSpace(cancelText) && 
                (cancelText.Contains("فسخ") || cancelText != ""))
                return false;
            
            // Check halat field
            if (!string.IsNullOrWhiteSpace(halatText) && halatText.Contains("فسخ"))
                return false;
                
            return true;
        }
        
        static bool GetActiveStatus(string? halatText)
        {
            return GetActiveStatus(halatText, null);
        }
        
        static async Task<int?> GetEducationLevelId(string? educationText, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(educationText))
                return null;
            
            string query = @"
                SELECT ""ID"" FROM look.""EducationLevel"" 
                WHERE ""Name"" = @name 
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", educationText);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
                
                // If not found, create it
                string insertQuery = @"
                    INSERT INTO look.""EducationLevel"" (""Name"", ""Dari"")
                    VALUES (@name, @dari)
                    RETURNING ""ID""";
                
                using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("name", educationText);
                    insertCmd.Parameters.AddWithValue("dari", educationText);
                    
                    var insertResult = await insertCmd.ExecuteScalarAsync();
                    return Convert.ToInt32(insertResult);
                }
            }
        }
        
        static async Task<int?> GetOrCreateProvinceId(string? provinceName, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
                return null;
            
            string query = @"
                SELECT ""ID"" FROM look.""Location"" 
                WHERE ""Name"" = @name AND ""Type"" = 'province'
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", provinceName);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
                
                // If not found, create it
                string insertQuery = @"
                    INSERT INTO look.""Location"" (""Name"", ""Dari"", ""Type"", ""IsActive"")
                    VALUES (@name, @dari, 'province', 1)
                    RETURNING ""ID""";
                
                using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("name", provinceName);
                    insertCmd.Parameters.AddWithValue("dari", provinceName);
                    
                    var insertResult = await insertCmd.ExecuteScalarAsync();
                    return Convert.ToInt32(insertResult);
                }
            }
        }
        
        static async Task<int?> GetOrCreateDistrictId(string? districtName, int? provinceId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(districtName))
                return null;
            
            string query = @"
                SELECT ""ID"" FROM look.""Location"" 
                WHERE ""Name"" = @name AND ""Type"" = 'district'
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", districtName);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
                
                // If not found, create it
                string insertQuery = @"
                    INSERT INTO look.""Location"" (""Name"", ""Dari"", ""Type"", ""Parent_ID"", ""IsActive"")
                    VALUES (@name, @dari, 'district', @parent_id, 1)
                    RETURNING ""ID""";
                
                using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("name", districtName);
                    insertCmd.Parameters.AddWithValue("dari", districtName);
                    insertCmd.Parameters.AddWithValue("parent_id", provinceId ?? (object)DBNull.Value);
                    
                    var insertResult = await insertCmd.ExecuteScalarAsync();
                    return Convert.ToInt32(insertResult);
                }
            }
        }
        
        static void PrintStatistics()
        {
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("MIGRATION COMPLETED");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"Total records processed: {stats.TotalRecords}");
            Console.WriteLine($"Companies created: {stats.CompaniesCreated}");
            Console.WriteLine($"Owners created: {stats.OwnersCreated}");
            Console.WriteLine($"Licenses created: {stats.LicensesCreated}");
            Console.WriteLine($"Cancellations created: {stats.CancellationsCreated}");
            Console.WriteLine($"Records skipped: {stats.Skipped.Count}");
            Console.WriteLine($"Errors encountered: {stats.Errors.Count}");
            
            if (stats.Errors.Count > 0)
            {
                Console.WriteLine("\nFirst 10 Errors:");
                foreach (var error in stats.Errors.Take(10))
                {
                    Console.WriteLine($"  Record {error.RecordId}: {error.ErrorMessage}");
                }
                if (stats.Errors.Count > 10)
                {
                    Console.WriteLine($"  ... and {stats.Errors.Count - 10} more errors");
                }
            }
            
            if (stats.Skipped.Count > 0)
            {
                Console.WriteLine("\nFirst 10 Skipped Records:");
                foreach (var skip in stats.Skipped.Take(10))
                {
                    Console.WriteLine($"  Record {skip.RecordId}: {skip.Reason}");
                }
                if (stats.Skipped.Count > 10)
                {
                    Console.WriteLine($"  ... and {stats.Skipped.Count - 10} more skipped");
                }
            }
            
            Console.WriteLine("\n" + new string('=', 80));
        }
    }
}
