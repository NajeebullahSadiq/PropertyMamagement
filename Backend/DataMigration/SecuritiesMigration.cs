using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql;

namespace DataMigration
{
    class SecuritiesMigration
    {
               // Connection string for database
        private static string connectionString = Environment.GetEnvironmentVariable("MIGRATION_CONNECTION_STRING") 
            ?? "Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=SecurePassword@2024";
        
        private static MigrationStats stats = new MigrationStats();

        private static SecuritiesMigrationStats stats = new SecuritiesMigrationStats();
        
        public static async Task RunSecuritiesMigration()
        {
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("SECURITIES MODULE MIGRATION");
            Console.WriteLine(new string('=', 80) + "\n");
            
            try
            {
                // Read JSON file
                Console.WriteLine("Loading data from securities_records_clean.json...");
                string jsonPath = "securities_records_clean_fixed.json";
                
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Error: File '{jsonPath}' not found!");
                    Console.WriteLine("Please ensure securities_records_clean_fixed.json is in the same directory.");
                    return;
                }
                
                string jsonContent = await File.ReadAllTextAsync(jsonPath);
                var securitiesRecords = JsonSerializer.Deserialize<List<SecuritiesRecord>>(jsonContent);
                
                stats.TotalRecords = securitiesRecords.Count;
                Console.WriteLine($"Loaded {stats.TotalRecords} records\n");
                
                // Start migration
                Console.WriteLine("Starting securities migration process...\n");
                await MigrateAllSecurities(securitiesRecords);
                
                // Print statistics
                PrintSecuritiesStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFatal Error in Securities Migration: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static async Task MigrateAllSecurities(List<SecuritiesRecord> records)
        {
            int processedCount = 0;
            
            for (int i = 0; i < records.Count; i++)
            {
                try
                {
                    await MigrateSecuritiesRecord(records[i], i + 1);
                    processedCount++;
                    
                    if (processedCount % 100 == 0)
                    {
                        Console.WriteLine($"Processed {processedCount}/{stats.TotalRecords} securities records...");
                    }
                }
                catch (Exception ex)
                {
                    stats.Errors.Add(new SecuritiesError
                    {
                        RecordIndex = i + 1,
                        RegistrationNumber = GetStringValue(records[i].RegistrationNumber),
                        ErrorMessage = ex.Message
                    });
                    Console.WriteLine($"Error processing securities record {i + 1}: {ex.Message}");
                }
            }
        }
        
        static async Task MigrateSecuritiesRecord(SecuritiesRecord record, int recordIndex)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        // Skip if no registration number
                        string regNumber = GetStringValue(record.RegistrationNumber);
                        if (string.IsNullOrWhiteSpace(regNumber))
                        {
                            stats.Skipped.Add(new SecuritiesSkipped
                            {
                                RecordIndex = recordIndex,
                                Reason = "No registration number"
                            });
                            await transaction.RollbackAsync();
                            return;
                        }
                        
                        // Check if already exists
                        using (var checkCmd = new NpgsqlCommand(
                            @"SELECT ""Id"" FROM org.""SecuritiesDistribution"" 
                              WHERE ""RegistrationNumber"" = @regnum", conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("regnum", regNumber);
                            var exists = await checkCmd.ExecuteScalarAsync();
                            
                            if (exists != null)
                            {
                                stats.Skipped.Add(new SecuritiesSkipped
                                {
                                    RecordIndex = recordIndex,
                                    Reason = $"Registration number {regNumber} already exists"
                                });
                                await transaction.RollbackAsync();
                                return;
                            }
                        }
                        
                        // 1. Insert SecuritiesDistribution (main record)
                        int distributionId = await InsertSecuritiesDistribution(record, conn, transaction);
                        stats.DistributionsCreated++;
                        
                        // 2. Insert SecuritiesDistributionItems (document items)
                        int itemsCreated = await InsertDistributionItems(record, distributionId, conn, transaction);
                        stats.ItemsCreated += itemsCreated;
                        
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
        
        static async Task<int> InsertSecuritiesDistribution(SecuritiesRecord record, 
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""SecuritiesDistribution"" 
                (""RegistrationNumber"", ""LicenseOwnerName"", ""LicenseOwnerFatherName"",
                 ""TransactionGuideName"", ""LicenseNumber"",
                 ""PricePerDocument"", ""TotalDocumentsPrice"", ""TotalSecuritiesPrice"",
                 ""BankReceiptNumber"", ""DeliveryDate"", ""DistributionDate"",
                 ""Status"", ""CreatedAt"", ""CreatedBy"")
                VALUES (@regnumber, @ownername, @ownerfathername,
                        @guidename, @licensenumber,
                        @priceperdoc, @totaldocprice, @totalsecprice,
                        @bankreceipt, @deliverydate, @distdate,
                        @status, @createdat, @createdby)
                RETURNING ""Id""";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("regnumber", GetStringValue(record.RegistrationNumber) ?? "");
                cmd.Parameters.AddWithValue("ownername", record.LicenseOwnerName?.ToString() ?? "");
                cmd.Parameters.AddWithValue("ownerfathername", record.LicenseOwnerFatherName?.ToString() ?? "");
                cmd.Parameters.AddWithValue("guidename", record.TransactionGuideName?.ToString() ?? "");
                cmd.Parameters.AddWithValue("licensenumber", GetStringValue(record.LicenseNumber) ?? "");
                
                cmd.Parameters.AddWithValue("priceperdoc", record.PricePerDocument ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("totaldocprice", record.TotalSecuritiesPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("totalsecprice", record.TotalPrice ?? (object)DBNull.Value);
                
                // Parse bank receipt (format: Icps56949631/1403/7/1)
                var (receiptNumber, receiptDate) = ParseBankReceiptInfo(record.BankReceiptInfo?.ToString());
                cmd.Parameters.AddWithValue("bankreceipt", receiptNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("deliverydate", receiptDate ?? (object)DBNull.Value);
                
                // Parse distribution date
                object distDate = ParseJalaliDate(record.DistributionDate?.ToString());
                cmd.Parameters.AddWithValue("distdate", distDate);
                
                cmd.Parameters.AddWithValue("status", true);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                cmd.Parameters.AddWithValue("createdby", "SECURITIES_MIGRATION");
                
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
        
        static async Task<int> InsertDistributionItems(SecuritiesRecord record, int distributionId,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            int itemsCreated = 0;
            
            // Type 1: Property Sale (سته خرید و فروش)
            string? propertySaleSerial = record.PropertySaleSerial?.ToString();
            if (!string.IsNullOrWhiteSpace(propertySaleSerial))
            {
                var (start, end) = ParseSerialRange(propertySaleSerial);
                int count = GetIntValue(record.PropertySaleCount) ?? CalculateCount(start, end);
                if (count > 0)
                {
                    await InsertDistributionItem(distributionId, 1, start, end, count, 
                        4000m, conn, transaction);
                    itemsCreated++;
                }
            }
            
            // Type 2: Bay Wafa (سته بیع وفا)
            string? bayWafaSerial = record.BayWafaSerial?.ToString();
            if (!string.IsNullOrWhiteSpace(bayWafaSerial))
            {
                var (start, end) = ParseSerialRange(bayWafaSerial);
                int count = GetIntValue(record.BayWafaCount) ?? CalculateCount(start, end);
                if (count > 0)
                {
                    await InsertDistributionItem(distributionId, 2, start, end, count, 
                        4000m, conn, transaction);
                    itemsCreated++;
                }
            }
            
            // Type 3: Rent (سته کرایی)
            string? rentSerial = record.RentSerial?.ToString();
            if (!string.IsNullOrWhiteSpace(rentSerial))
            {
                var (start, end) = ParseSerialRange(rentSerial);
                int count = GetIntValue(record.RentCount) ?? CalculateCount(start, end);
                if (count > 0)
                {
                    await InsertDistributionItem(distributionId, 3, start, end, count, 
                        4000m, conn, transaction);
                    itemsCreated++;
                }
            }
            
            // Type 4: Vehicle (سته موتر)
            string? vehicleSerial = record.VehicleSerial?.ToString();
            if (!string.IsNullOrWhiteSpace(vehicleSerial))
            {
                var (start, end) = ParseSerialRange(vehicleSerial);
                int count = GetIntValue(record.VehicleCount) ?? CalculateCount(start, end);
                if (count > 0)
                {
                    await InsertDistributionItem(distributionId, 4, start, end, count, 
                        4000m, conn, transaction);
                    itemsCreated++;
                }
            }
            
            // Type 5: Registration Book (کتاب ثبت)
            int regBookCount = GetIntValue(record.RegistrationBookCount) ?? 0;
            if (regBookCount > 0)
            {
                decimal regBookPrice = GetDecimalValue(record.RegistrationBookPrice) ?? 1000m;
                await InsertDistributionItem(distributionId, 5, null, null, regBookCount, 
                    regBookPrice, conn, transaction);
                itemsCreated++;
            }
            
            return itemsCreated;
        }
        
        static async Task InsertDistributionItem(int distributionId, int documentType,
            string? serialStart, string? serialEnd, int count, decimal price,
            NpgsqlConnection conn, NpgsqlTransaction transaction)
        {
            string query = @"
                INSERT INTO org.""SecuritiesDistributionItem"" 
                (""SecuritiesDistributionId"", ""DocumentType"", ""SerialStart"", ""SerialEnd"",
                 ""Count"", ""Price"", ""CreatedAt"")
                VALUES (@distid, @doctype, @serialstart, @serialend, @count, @price, @createdat)";
            
            using (var cmd = new NpgsqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("distid", distributionId);
                cmd.Parameters.AddWithValue("doctype", documentType);
                cmd.Parameters.AddWithValue("serialstart", serialStart ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("serialend", serialEnd ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("count", count);
                cmd.Parameters.AddWithValue("price", price * count);
                cmd.Parameters.AddWithValue("createdat", DateTime.Now);
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        // Helper methods
        static string? GetStringValue(object? value)
        {
            if (value == null) return null;
            
            string str = value.ToString()?.Trim();
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }
        
        static int? GetIntValue(object? value)
        {
            if (value == null) return null;
            
            string str = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(str)) return null;
            
            if (int.TryParse(str, out int result))
                return result;
            
            if (double.TryParse(str, out double dResult))
                return (int)dResult;
            
            return null;
        }
        
        static decimal? GetDecimalValue(object? value)
        {
            if (value == null) return null;
            
            string str = value.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(str)) return null;
            
            if (decimal.TryParse(str, out decimal result))
                return result;
            
            if (double.TryParse(str, out double dResult))
                return (decimal)dResult;
            
            return null;
        }
        
        static (string? start, string? end) ParseSerialRange(string? serialInfo)
        {
            if (string.IsNullOrWhiteSpace(serialInfo))
                return (null, null);
            
            // Format: "9840/9811" or "74760/74731"
            var parts = serialInfo.Split('/');
            if (parts.Length == 2)
            {
                return (parts[1].Trim(), parts[0].Trim()); // Start is second, End is first
            }
            
            return (serialInfo.Trim(), serialInfo.Trim());
        }
        
        static int CalculateCount(string? start, string? end)
        {
            if (string.IsNullOrWhiteSpace(start) || string.IsNullOrWhiteSpace(end))
                return 1;
            
            if (int.TryParse(start, out int startNum) && int.TryParse(end, out int endNum))
            {
                return Math.Abs(endNum - startNum) + 1;
            }
            
            return 1;
        }
        
        static (string? receiptNumber, object? receiptDate) ParseBankReceiptInfo(string? receiptInfo)
        {
            if (string.IsNullOrWhiteSpace(receiptInfo))
                return (null, null);
            
            // Format: "Icps56949631/1403/7/1" or just "Icps56949631"
            var parts = receiptInfo.Split('/');
            if (parts.Length >= 1)
            {
                string receiptNumber = parts[0].Trim();
                
                if (parts.Length >= 4)
                {
                    // Has date: 1403/7/1
                    string dateStr = $"{parts[1]}/{parts[2]}/{parts[3]}";
                    object date = ParseJalaliDate(dateStr);
                    return (receiptNumber, date);
                }
                
                return (receiptNumber, null);
            }
            
            return (receiptInfo, null);
        }
        
        static object ParseJalaliDate(string? dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return DBNull.Value;
            
            // Format: "1403/7/1" or "1403-7-1"
            dateStr = dateStr.Replace('/', '-').Trim();
            var parts = dateStr.Split('-');
            
            if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month) &&
                    int.TryParse(parts[2], out int day))
                {
                    try
                    {
                        // Validate ranges
                        if (year >= 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
                        {
                            return new DateTime(year, month, day);
                        }
                    }
                    catch
                    {
                        return DBNull.Value;
                    }
                }
            }
            
            return DBNull.Value;
        }
        
        static void PrintSecuritiesStatistics()
        {
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("SECURITIES MIGRATION COMPLETED");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"Total records processed: {stats.TotalRecords}");
            Console.WriteLine($"Distributions created: {stats.DistributionsCreated}");
            Console.WriteLine($"Distribution items created: {stats.ItemsCreated}");
            Console.WriteLine($"Records skipped: {stats.Skipped.Count}");
            Console.WriteLine($"Errors encountered: {stats.Errors.Count}");
            
            if (stats.Errors.Count > 0)
            {
                Console.WriteLine("\nFirst 10 Errors:");
                foreach (var error in stats.Errors.Take(10))
                {
                    Console.WriteLine($"  Record {error.RecordIndex} (Reg: {error.RegistrationNumber}): {error.ErrorMessage}");
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
                    Console.WriteLine($"  Record {skip.RecordIndex}: {skip.Reason}");
                }
                if (stats.Skipped.Count > 10)
                {
                    Console.WriteLine($"  ... and {stats.Skipped.Count - 10} more skipped");
                }
            }
            
            Console.WriteLine("\n" + new string('=', 80));
        }
    }
    
    public class SecuritiesMigrationStats
    {
        public int TotalRecords { get; set; }
        public int DistributionsCreated { get; set; }
        public int ItemsCreated { get; set; }
        public List<SecuritiesError> Errors { get; set; } = new List<SecuritiesError>();
        public List<SecuritiesSkipped> Skipped { get; set; } = new List<SecuritiesSkipped>();
    }
    
    public class SecuritiesError
    {
        public int RecordIndex { get; set; }
        public string? RegistrationNumber { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
    
    public class SecuritiesSkipped
    {
        public int RecordIndex { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
