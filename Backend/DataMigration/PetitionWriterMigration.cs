using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;

namespace DataMigration
{
    public class PetitionWriterMigration
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MIGRATION_CONNECTION_STRING") 
            ?? "Host=localhost;Port=5432;Database=PRMIS;Username=postgres;Password=Khan@223344";
        
        private static PetitionWriterStats stats = new PetitionWriterStats();
        
        public static async Task RunPetitionWriterMigration()
        {
            Console.WriteLine("\n=================================================================");
            Console.WriteLine("PETITION WRITER LICENSE MIGRATION");
            Console.WriteLine("=================================================================\n");
            
            try
            {
                // Load both JSON files
                var records1403 = await LoadPetitionWriterRecords("petition_1403_records.json", 1403);
                var records1404 = await LoadPetitionWriterRecords("petition_1404_records.json", 1404);
                
                // Combine all records
                var allRecords = new List<PetitionWriterRecord>();
                allRecords.AddRange(records1403);
                allRecords.AddRange(records1404);
                
                stats.TotalRecords = allRecords.Count;
                Console.WriteLine($"Loaded {records1403.Count} records from 1403");
                Console.WriteLine($"Loaded {records1404.Count} records from 1404");
                Console.WriteLine($"Total: {stats.TotalRecords} records\n");
                
                // Start migration
                Console.WriteLine("Starting migration process...\n");
                await MigrateAllPetitionWriterRecords(allRecords);
                
                // Print statistics
                PrintPetitionWriterStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFatal Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private static async Task<List<PetitionWriterRecord>> LoadPetitionWriterRecords(string filename, int sourceYear)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"Warning: File '{filename}' not found. Skipping...");
                    return new List<PetitionWriterRecord>();
                }
                
                string jsonContent = await File.ReadAllTextAsync(filename);
                
                // Replace NaN with null in the JSON string
                jsonContent = jsonContent.Replace(": NaN", ": null");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                
                var records = JsonSerializer.Deserialize<List<PetitionWriterRecord>>(jsonContent, options);
                
                // Set source year for all records
                if (records != null)
                {
                    foreach (var record in records)
                    {
                        record.SourceYear = sourceYear;
                    }
                }
                
                return records ?? new List<PetitionWriterRecord>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {filename}: {ex.Message}");
                return new List<PetitionWriterRecord>();
            }
        }
        
        private static async Task MigrateAllPetitionWriterRecords(List<PetitionWriterRecord> records)
        {
            int processedCount = 0;
            
            foreach (var record in records)
            {
                try
                {
                    await MigratePetitionWriterRecord(record);
                    processedCount++;
                    
                    if (processedCount % 50 == 0)
                    {
                        Console.WriteLine($"Processed {processedCount}/{stats.TotalRecords} records...");
                    }
                }
                catch (Exception ex)
                {
                    stats.Errors.Add(new PetitionWriterError
                    {
                        LicenseNumber = GetLicenseNumberString(record.LicenseNumber),
                        ApplicantName = record.ApplicantName ?? "Unknown",
                        ErrorMessage = ex.Message,
                        SourceYear = record.SourceYear ?? 0
                    });
                    Console.WriteLine($"Error processing record (License: {GetLicenseNumberString(record.LicenseNumber)}): {ex.Message}");
                }
            }
        }
        
        private static async Task MigratePetitionWriterRecord(PetitionWriterRecord record)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // Get license number as string
                        string licenseNumber = GetLicenseNumberString(record.LicenseNumber);
                        
                        if (string.IsNullOrWhiteSpace(licenseNumber))
                        {
                            stats.Skipped.Add(new PetitionWriterSkipped
                            {
                                ApplicantName = record.ApplicantName ?? "Unknown",
                                Reason = "Missing license number",
                                SourceYear = record.SourceYear ?? 0
                            });
                            await transaction.CommitAsync();
                            return;
                        }
                        
                        // Check if license already exists
                        bool licenseExists = await CheckLicenseExists(licenseNumber, conn, transaction);
                        
                        if (licenseExists)
                        {
                            stats.Skipped.Add(new PetitionWriterSkipped
                            {
                                ApplicantName = record.ApplicantName ?? "Unknown",
                                Reason = $"License {licenseNumber} already exists",
                                SourceYear = record.SourceYear ?? 0
                            });
                            await transaction.CommitAsync();
                            return;
                        }
                        
                        // Insert PetitionWriterLicense
                        await InsertPetitionWriterLicense(record, licenseNumber, conn, transaction);
                        stats.LicensesCreated++;
                        
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
        
        private static async Task<bool> CheckLicenseExists(string licenseNumber, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                SELECT COUNT(*) FROM org.""PetitionWriterLicenses"" 
                WHERE ""LicenseNumber"" = @licensenumber";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("licensenumber", licenseNumber);
                var count = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(count) > 0;
            }
        }
        
        private static async Task InsertPetitionWriterLicense(PetitionWriterRecord record, 
            string licenseNumber, NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""PetitionWriterLicenses"" 
                (""LicenseNumber"", ""ProvinceId"", ""ApplicantName"", ""ApplicantFatherName"", 
                 ""ElectronicNationalIdNumber"",
                 ""PermanentProvinceId"", ""PermanentDistrictId"", ""PermanentVillage"",
                 ""CurrentProvinceId"", ""CurrentDistrictId"", ""CurrentVillage"",
                 ""DetailedAddress"", ""LicenseIssueDate"", ""LicenseType"",
                 ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@licensenumber, @provinceid, @applicantname, @fathername, 
                        @electronicnationalidnumber,
                        @permanentprovinceid, @permanentdistrictid, @permanentvillage,
                        @currentprovinceid, @currentdistrictid, @currentvillage,
                        @detailedaddress, @licenseissuedate, @licensetype,
                        @status, @createdat, @createdby)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                // License Number and Province
                cmd.Parameters.AddWithValue("licensenumber", licenseNumber);
                cmd.Parameters.AddWithValue("provinceid", 1); // All licenses are from Kabul
                
                // Basic Information
                cmd.Parameters.AddWithValue("applicantname", record.ApplicantName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("fathername", record.FatherName ?? (object)DBNull.Value);
                
                // Electronic National ID
                string? electronicId = GetStringFromObject(record.ElectronicNationalIdNumber);
                if (!string.IsNullOrEmpty(electronicId) && electronicId.Length > 50)
                    electronicId = electronicId.Substring(0, 50);
                cmd.Parameters.AddWithValue("electronicnationalidnumber", electronicId ?? (object)DBNull.Value);
                
                // Permanent Address
                int? permProvinceId = await GetOrCreateProvinceId(record.PermanentProvince, conn, transaction);
                int? permDistrictId = await GetOrCreateDistrictId(record.PermanentDistrict, permProvinceId, conn, transaction);
                cmd.Parameters.AddWithValue("permanentprovinceid", permProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("permanentdistrictid", permDistrictId ?? (object)DBNull.Value);
                
                // Handle PermanentVillage (could be string or number)
                string? permanentVillage = GetStringFromObject(record.PermanentVillage);
                cmd.Parameters.AddWithValue("permanentvillage", permanentVillage ?? (object)DBNull.Value);
                
                // Current Address
                int? currProvinceId = await GetOrCreateProvinceId(record.CurrentProvince, conn, transaction);
                int? currDistrictId = await GetOrCreateDistrictId(record.CurrentDistrict, currProvinceId, conn, transaction);
                cmd.Parameters.AddWithValue("currentprovinceid", currProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("currentdistrictid", currDistrictId ?? (object)DBNull.Value);
                
                // Handle CurrentVillage_Nahia field (could be string or number)
                string? currentVillage = GetCurrentVillageString(record.CurrentVillageNahia);
                cmd.Parameters.AddWithValue("currentvillage", currentVillage ?? (object)DBNull.Value);
                
                // Detailed Address (use ActivityLocation or DetailedAddress)
                string? detailedAddress = record.ActivityLocation ?? record.DetailedAddress;
                cmd.Parameters.AddWithValue("detailedaddress", detailedAddress ?? (object)DBNull.Value);
                
                // License Issue Date
                DateTime? issueDate = ParsePersianDate(record.LicenseIssueDate);
                cmd.Parameters.AddWithValue("licenseissuedate", issueDate ?? (object)DBNull.Value);
                
                // License Type (New or Renewal)
                string? licenseType = DetermineLicenseType(record.LicenseTypeNew, record.LicenseTypeRenewal);
                cmd.Parameters.AddWithValue("licensetype", licenseType ?? (object)DBNull.Value);
                
                // Status and Audit
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", $"MIGRATION_SCRIPT_{record.SourceYear}");
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        // Helper Methods
        private static string GetLicenseNumberString(object? licenseNumber)
        {
            if (licenseNumber == null) return string.Empty;
            
            if (licenseNumber is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Number)
                {
                    return jsonElement.GetDouble().ToString("0");
                }
                else if (jsonElement.ValueKind == JsonValueKind.String)
                {
                    return jsonElement.GetString() ?? string.Empty;
                }
            }
            
            return licenseNumber.ToString() ?? string.Empty;
        }
        
        private static string? GetMobileNumberString(object? mobileNumber)
        {
            if (mobileNumber == null) return null;
            
            if (mobileNumber is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Number)
                {
                    return jsonElement.GetDouble().ToString("0");
                }
                else if (jsonElement.ValueKind == JsonValueKind.String)
                {
                    return jsonElement.GetString();
                }
            }
            
            return mobileNumber.ToString();
        }
        
        private static string? GetStringFromObject(object? value)
        {
            if (value == null) return null;
            
            if (value is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.String)
                {
                    return jsonElement.GetString();
                }
                else if (jsonElement.ValueKind == JsonValueKind.Number)
                {
                    return jsonElement.GetDouble().ToString("0");
                }
            }
            
            return value.ToString();
        }
        
        private static string? GetCurrentVillageString(object? currentVillageNahia)
        {
            if (currentVillageNahia == null) return null;
            
            string? value = GetStringFromObject(currentVillageNahia);
            if (string.IsNullOrWhiteSpace(value)) return null;
            
            // Try to parse as number
            if (double.TryParse(value, out double nahiaNumber))
            {
                return $"ناحیه {nahiaNumber}";
            }
            
            return value;
        }
        
        private static string? GetDistrictNahiaString(object? districtNahia)
        {
            return GetStringFromObject(districtNahia);
        }
        
        private static DateTime? ParsePersianDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;
            
            try
            {
                // Try to parse Persian date format: "DD/MM/YYYY"
                var parts = dateString.Split('/');
                if (parts.Length == 3)
                {
                    if (int.TryParse(parts[0], out int day) &&
                        int.TryParse(parts[1], out int month) &&
                        int.TryParse(parts[2], out int year))
                    {
                        // Validate ranges
                        if (year >= 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
                        {
                            return new DateTime(year, month, day);
                        }
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            
            return null;
        }
        
        private static string? DetermineLicenseType(object? licenseTypeNew, object? licenseTypeRenewal)
        {
            string? newType = GetStringFromObject(licenseTypeNew);
            string? renewalType = GetStringFromObject(licenseTypeRenewal);
            
            // Check if it's a new license
            if (!string.IsNullOrWhiteSpace(newType) && 
                (newType.Contains("//") || newType.Contains("جدید")))
            {
                return "جدید";
            }
            
            // Check if it's a renewal
            if (!string.IsNullOrWhiteSpace(renewalType) && 
                (renewalType.Contains("//") || renewalType.Contains("تجدید")))
            {
                return "تجدید";
            }
            
            // Check for "مثنی" (duplicate)
            if (!string.IsNullOrWhiteSpace(renewalType) && renewalType.Contains("مثنی"))
            {
                return "مثنی";
            }
            
            return null;
        }
        
        private static async Task<int?> GetOrCreateProvinceId(string? provinceName, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
                return null;
            
            // For Kabul, always return 1
            if (provinceName.Contains("کابل"))
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
        
        private static async Task<int?> GetOrCreateDistrictId(string? districtName, int? provinceId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
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
        
        private static void PrintPetitionWriterStatistics()
        {
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("PETITION WRITER MIGRATION COMPLETED");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"Total records processed: {stats.TotalRecords}");
            Console.WriteLine($"Licenses created: {stats.LicensesCreated}");
            Console.WriteLine($"Records skipped: {stats.Skipped.Count}");
            Console.WriteLine($"Errors encountered: {stats.Errors.Count}");
            
            if (stats.Skipped.Count > 0)
            {
                Console.WriteLine("\nSkipped Records:");
                foreach (var skip in stats.Skipped.Take(10))
                {
                    Console.WriteLine($"  - {skip.ApplicantName} ({skip.SourceYear}): {skip.Reason}");
                }
                if (stats.Skipped.Count > 10)
                    Console.WriteLine($"  ... and {stats.Skipped.Count - 10} more");
            }
            
            if (stats.Errors.Count > 0)
            {
                Console.WriteLine("\nErrors:");
                foreach (var error in stats.Errors.Take(10))
                {
                    Console.WriteLine($"  - {error.ApplicantName} (License: {error.LicenseNumber}, Year: {error.SourceYear}): {error.ErrorMessage}");
                }
                if (stats.Errors.Count > 10)
                    Console.WriteLine($"  ... and {stats.Errors.Count - 10} more");
            }
            
            Console.WriteLine(new string('=', 80));
        }
    }
    
    // Statistics Classes
    public class PetitionWriterStats
    {
        public int TotalRecords { get; set; }
        public int LicensesCreated { get; set; }
        public List<PetitionWriterError> Errors { get; set; } = new List<PetitionWriterError>();
        public List<PetitionWriterSkipped> Skipped { get; set; } = new List<PetitionWriterSkipped>();
    }
    
    public class PetitionWriterError
    {
        public string LicenseNumber { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public int SourceYear { get; set; }
    }
    
    public class PetitionWriterSkipped
    {
        public string ApplicantName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int SourceYear { get; set; }
    }
}
