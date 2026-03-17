using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Npgsql;

namespace DataMigration
{
    public class PetitionWriterMigration
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MIGRATION_CONNECTION_STRING") 
            ?? "Host=localhost;Port=5432;Database=PRMIS;Username=postgres;Password=Khan@223344";
        
        public static async Task RunPetitionWriterMigration()
        {
            Console.WriteLine("\n=================================================================");
            Console.WriteLine("PETITION WRITER LICENSE MIGRATION");
            Console.WriteLine("=================================================================\n");
            
            try
            {
                // Load JSON file
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "petition_writer_records.json");
                
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Error: File not found: {jsonPath}");
                    return;
                }
                
                var jsonString = await File.ReadAllTextAsync(jsonPath);
                
                // Replace NaN values with null (NaN is not valid JSON)
                jsonString = jsonString.Replace(": NaN", ": null");
                jsonString = jsonString.Replace(":NaN", ":null");
                
                // Configure JsonSerializer to handle mixed types
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
                };
                
                var records = JsonSerializer.Deserialize<List<PetitionWriterRecord>>(jsonString, options);
                
                Console.WriteLine($"Loaded {records?.Count ?? 0} records from JSON");
                
                if (records == null || records.Count == 0)
                {
                    Console.WriteLine("No records to migrate.");
                    return;
                }
                
                Console.WriteLine("\nStarting migration process...\n");
                
                int licensesCreated = 0;
                int relocationsCreated = 0;
                int errors = 0;
                
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                
                foreach (var record in records)
                {
                    try
                    {
                        // Skip records without required fields
                        var applicantName = record.ApplicantName?.ToString();
                        var electronicId = record.ElectronicNationalIdNumber?.ToString();
                        
                        if (string.IsNullOrWhiteSpace(applicantName) || 
                            string.IsNullOrWhiteSpace(electronicId))
                        {
                            errors++;
                            continue;
                        }
                        
                        // Convert LicenseNumber from object to string and format it
                        string licenseNumber = record.LicenseNumber?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(licenseNumber))
                        {
                            licenseNumber = $"KBL-{(record.ProvinceId ?? 1):D8}";
                        }
                        else
                        {
                            // Handle decimal format like "1560.0" or pure numbers
                            if (double.TryParse(licenseNumber, out double numValue))
                            {
                                licenseNumber = $"KBL-{(int)numValue:D8}";
                            }
                        }
                        
                        // Convert MobileNumber from object to string
                        string mobileNumber = record.MobileNumber?.ToString() ?? "";
                        
                        // Convert village fields
                        string permanentVillage = record.PermanentVillage?.ToString() ?? "";
                        string currentVillage = record.CurrentVillage?.ToString() ?? "";
                        
                        // Get location IDs from province/district names
                        int? permanentProvinceId = await GetLocationId(conn, record.PermanentProvinceId);
                        int? permanentDistrictId = await GetLocationId(conn, record.PermanentDistrictId);
                        int? currentProvinceId = await GetLocationId(conn, record.CurrentProvinceId);
                        int? currentDistrictId = await GetLocationId(conn, record.CurrentDistrictId);
                        
                        // Determine license type
                        string licenseType = record.LicenseType ?? "new";
                        if (licenseType == "renewal" || !string.IsNullOrEmpty(record.LicenseTypeExtend?.ToString()))
                        {
                            licenseType = "تجدید";
                        }
                        else
                        {
                            licenseType = "جدید";
                        }
                        
                        // Parse license issue date
                        DateOnly? licenseIssueDate = ParsePersianDate(record.LicenseIssueDate);
                        
                        // Insert license
                        var licenseId = await InsertLicense(conn, new Dictionary<string, object?>
                        {
                            ["LicenseNumber"] = licenseNumber,
                            ["ProvinceId"] = record.ProvinceId,
                            ["ApplicantName"] = applicantName,
                            ["ApplicantFatherName"] = record.ApplicantFatherName?.ToString(),
                            ["ElectronicNationalIdNumber"] = electronicId,
                            ["MobileNumber"] = mobileNumber,
                            ["PermanentProvinceId"] = permanentProvinceId,
                            ["PermanentDistrictId"] = permanentDistrictId,
                            ["PermanentVillage"] = permanentVillage,
                            ["CurrentProvinceId"] = currentProvinceId,
                            ["CurrentDistrictId"] = currentDistrictId,
                            ["CurrentVillage"] = currentVillage,
                            ["DetailedAddress"] = record.DetailedAddress,
                            ["ActivityNahia"] = record.ActivityNahia,
                            ["LicenseType"] = licenseType,
                            ["LicenseIssueDate"] = licenseIssueDate,
                            ["LicenseStatus"] = 1,
                            ["Status"] = true,
                            ["CreatedAt"] = DateTime.Now,
                            ["CreatedBy"] = "migration"
                        });
                        
                        if (licenseId > 0)
                        {
                            licensesCreated++;
                            
                            // Handle relocations if present
                            var relocationInfo = record.PetitionWriterRelocations?.ToString();
                            if (!string.IsNullOrWhiteSpace(relocationInfo) && 
                                relocationInfo != "NaN" && 
                                relocationInfo != "null")
                            {
                                await InsertRelocation(conn, licenseId, relocationInfo);
                                relocationsCreated++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing record: {ex.Message}");
                        errors++;
                    }
                }
                
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("PETITION WRITER MIGRATION COMPLETED");
                Console.WriteLine("================================================================================");
                Console.WriteLine($"Total records processed: {records.Count}");
                Console.WriteLine($"Licenses created: {licensesCreated}");
                Console.WriteLine($"Relocations created: {relocationsCreated}");
                Console.WriteLine($"Errors encountered: {errors}");
                Console.WriteLine("================================================================================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex.Message}");
            }
        }
        
        private static async Task<int?> GetLocationId(NpgsqlConnection conn, string? locationName)
        {
            if (string.IsNullOrWhiteSpace(locationName))
                return null;
            
            await using var cmd = new NpgsqlCommand(
                "SELECT \"ID\" FROM look.\"Location\" WHERE \"Dari\" = @name LIMIT 1", conn);
            cmd.Parameters.AddWithValue("name", locationName);
            
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : null;
        }
        
        private static DateOnly? ParsePersianDate(string? dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return null;
            
            try
            {
                // Format: "17/1/1404" (day/month/year)
                var parts = dateStr.Split('/');
                if (parts.Length == 3)
                {
                    // For now, just return null - proper Persian date conversion would be needed
                    return null;
                }
            }
            catch { }
            
            return null;
        }
        
        private static async Task<int> InsertLicense(NpgsqlConnection conn, Dictionary<string, object?> values)
        {
            var columns = string.Join(", ", values.Keys.Select(k => $"\"{k}\""));
            var paramNames = string.Join(", ", values.Keys.Select((_, i) => $"@p{i}"));
            
            var sql = $@"
                INSERT INTO org.""PetitionWriterLicenses"" ({columns})
                VALUES ({paramNames})
                RETURNING ""Id""";
            
            await using var cmd = new NpgsqlCommand(sql, conn);
            
            int i = 0;
            foreach (var value in values.Values)
            {
                cmd.Parameters.AddWithValue($"@p{i}", value ?? DBNull.Value);
                i++;
            }
            
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        
        private static async Task InsertRelocation(NpgsqlConnection conn, int licenseId, string relocationInfo)
        {
            var sql = @"
                INSERT INTO org.""PetitionWriterRelocations"" 
                (""PetitionWriterLicenseId"", ""NewActivityLocation"", ""CreatedAt"")
                VALUES (@licenseId, @location, @createdAt)";
            
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("licenseId", licenseId);
            cmd.Parameters.AddWithValue("location", relocationInfo);
            cmd.Parameters.AddWithValue("createdAt", DateTime.Now);
            
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
