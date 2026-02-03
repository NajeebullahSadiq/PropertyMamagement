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
        private static string connectionString = "Host=localhost;Port=5432;Database=your_database_name;Username=your_username;Password=your_password";
        
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
                            "SELECT id FROM org.companydetails WHERE id = @id", conn, transaction))
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
                        int? kabulProvinceId = await GetOrCreateProvinceId("کابل", conn, transaction);
                        
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
        
        static async Task<int> InsertCompanyDetails(OldRecord record, int? kabulProvinceId, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.companydetails 
                (id, title, tin, provinceid, status, createdat, createdby)
                VALUES (@id, @title, @tin, @provinceid, @status, @createdat, @createdby)
                RETURNING id";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("id", record.RID);
                cmd.Parameters.AddWithValue("title", record.RealEstateName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("tin", record.TIN ?? (object)DBNull.Value);
                // All companies are registered in Kabul
                cmd.Parameters.AddWithValue("provinceid", kabulProvinceId ?? (object)DBNull.Value);
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
                INSERT INTO org.companyowners 
                (firstname, fathername, grandfathername, educationlevelid, dateofbirth,
                 electronicnationalidnumber, phonenumber, companyid,
                 ownerprovinceid, ownerdistrictid, ownervillage,
                 permanentprovinceid, permanentdistrictid, permanentvillage,
                 status, createdat, createdby)
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
                
                cmd.Parameters.AddWithValue("dateofbirth", record.DOB ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("electronicnationalidnumber", record.TazkeraNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("phonenumber", record.ContactNo ?? (object)DBNull.Value);
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
                
                cmd.Parameters.AddWithValue("status", GetActiveStatus(record.Halat));
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        static async Task InsertLicenseDetails(OldRecord record, int companyId, int? kabulProvinceId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.licensedetails 
                (licensenumber, provinceid, issuedate, expiredate, officeaddress,
                 companyid, licensetype, licensecategory,
                 royaltyamount, royaltydate, penaltyamount, tariffnumber,
                 hrletter, hrletterdate, iscomplete, status, createdat, createdby)
                VALUES (@licensenumber, @provinceid, @issuedate, @expiredate, @officeaddress,
                        @companyid, @licensetype, @licensecategory,
                        @royaltyamount, @royaltydate, @penaltyamount, @tariffnumber,
                        @hrletter, @hrletterdate, @iscomplete, @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("licensenumber", record.LicenseNo.ToString());
                // All licenses are issued in Kabul
                cmd.Parameters.AddWithValue("provinceid", kabulProvinceId ?? (object)DBNull.Value);
                
                // Create dates from year/month/day
                string issueDate = CreateDateString(record.SYear, record.SMonth, record.SDay);
                string expireDate = CreateDateString(record.EYear, record.EMonth, record.EDay);
                string royaltyDate = CreateDateString(record.CreditRightYear, record.CreditRightMonth, record.CreditRightDay);
                string hrDate = CreateDateString(record.Combo211, record.HRMonth, record.HRDay);
                
                cmd.Parameters.AddWithValue("issuedate", issueDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("expiredate", expireDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("officeaddress", record.ExactAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("companyid", companyId);
                cmd.Parameters.AddWithValue("licensetype", record.LicenseType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("licensecategory", record.LicenseType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("royaltyamount", record.CreditRightAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("royaltydate", royaltyDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("penaltyamount", record.LateFine ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("tariffnumber", record.CreditRightNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("hrletter", record.HRNo.HasValue ? record.HRNo.ToString() : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("hrletterdate", hrDate ?? (object)DBNull.Value);
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
                INSERT INTO org.companycancellationinfo 
                (companyid, licensecancellationletternumber, licensecancellationletterdate,
                 remarks, status, createdat, createdby)
                VALUES (@companyid, @licensecancellationletternumber, @licensecancellationletterdate,
                        @remarks, @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("companyid", companyId);
                cmd.Parameters.AddWithValue("licensecancellationletternumber", record.LicnsCancelNo);
                
                string cancelDate = CreateDateString(record.CancelYear, record.CancelMonth, record.CancelDay);
                cmd.Parameters.AddWithValue("licensecancellationletterdate", cancelDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("remarks", record.Remarks ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        // Helper methods
        static string CreateDateString(double? year, double? month, double? day)
        {
            if (!year.HasValue || !month.HasValue || !day.HasValue)
                return null;
            
            int y = (int)year.Value;
            int m = (int)month.Value;
            int d = (int)day.Value;
            
            // Return Jalali date string (you may need to convert to Gregorian)
            return $"{y:0000}-{m:00}-{d:00}";
        }
        
        static bool GetActiveStatus(string halatText, string cancelText)
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
        
        static async Task<int?> GetEducationLevelId(string educationText, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(educationText))
                return null;
            
            string query = @"
                SELECT id FROM look.educationlevel 
                WHERE name = @name 
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", educationText);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
                
                // If not found, create it
                string insertQuery = @"
                    INSERT INTO look.educationlevel (name, status, createdat, createdby)
                    VALUES (@name, true, @createdat, @createdby)
                    RETURNING id";
                
                using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("name", educationText);
                    insertCmd.Parameters.AddWithValue("createdat", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                    
                    var insertResult = await insertCmd.ExecuteScalarAsync();
                    return Convert.ToInt32(insertResult);
                }
            }
        }
        
        static async Task<int?> GetOrCreateProvinceId(string provinceName, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
                return null;
            
            string query = @"
                SELECT id FROM look.location 
                WHERE name = @name AND type = 'province'
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", provinceName);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
                
                // If not found, create it
                string insertQuery = @"
                    INSERT INTO look.location (name, type, status, createdat, createdby)
                    VALUES (@name, 'province', true, @createdat, @createdby)
                    RETURNING id";
                
                using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("name", provinceName);
                    insertCmd.Parameters.AddWithValue("createdat", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                    
                    var insertResult = await insertCmd.ExecuteScalarAsync();
                    return Convert.ToInt32(insertResult);
                }
            }
        }
        
        static async Task<int?> GetOrCreateDistrictId(string districtName, int? provinceId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(districtName))
                return null;
            
            string query = @"
                SELECT id FROM look.location 
                WHERE name = @name AND type = 'district'
                LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("name", districtName);
                var result = await cmd.ExecuteScalarAsync();
                
                if (result != null)
                    return Convert.ToInt32(result);
                
                // If not found, create it
                string insertQuery = @"
                    INSERT INTO look.location (name, type, parent_id, status, createdat, createdby)
                    VALUES (@name, 'district', @parent_id, true, @createdat, @createdby)
                    RETURNING id";
                
                using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("name", districtName);
                    insertCmd.Parameters.AddWithValue("parent_id", provinceId ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("createdat", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
                    
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
