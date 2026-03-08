using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;

namespace WebAPIBackend.Services
{
    /// <summary>
    /// Service for generating province-specific license numbers
    /// Format: PROVINCE_CODE-SEQUENTIAL_NUMBER (e.g., KBL-0001, KHR-0234)
    /// </summary>
    public interface ILicenseNumberGenerator
    {
        Task<string> GenerateNextLicenseNumber(int provinceId);
        Task<string> GenerateNextPetitionWriterLicenseNumber(int provinceId);
        string GetProvinceCode(int provinceId);
    }

    public class LicenseNumberGenerator : ILicenseNumberGenerator
    {
        private readonly AppDbContext _context;

        // Province code mapping based on Afghanistan's 34 provinces
        private static readonly Dictionary<string, string> ProvinceCodeMap = new()
        {
            { "Kabul", "KBL" },
            { "Herat", "HRT" },
            { "Kandahar", "KHR" },
            { "Balkh", "BLK" },
            { "Nangarhar", "NGR" },
            { "Ghazni", "GHZ" },
            { "Helmand", "HLM" },
            { "Badakhshan", "BDK" },
            { "Takhar", "TKR" },
            { "Kunduz", "KDZ" },
            { "Baghlan", "BGL" },
            { "Bamyan", "BMN" },
            { "Farah", "FRH" },
            { "Faryab", "FRB" },
            { "Ghor", "GHR" },
            { "Jawzjan", "JWZ" },
            { "Kapisa", "KPS" },
            { "Khost", "KHT" },
            { "Kunar", "KNR" },
            { "Laghman", "LGM" },
            { "Logar", "LGR" },
            { "Nimroz", "NMZ" },
            { "Nuristan", "NRS" },
            { "Paktia", "PKT" },
            { "Paktika", "PKK" },
            { "Panjshir", "PNJ" },
            { "Parwan", "PRW" },
            { "Samangan", "SMG" },
            { "Sar-e Pol", "SRP" },
            { "Uruzgan", "URZ" },
            { "Wardak", "WRD" },
            { "Zabul", "ZBL" },
            { "Badghis", "BDG" },
            { "Daykundi", "DYK" }
        };

        public LicenseNumberGenerator(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generate the next sequential license number for a specific province
        /// Uses PostgreSQL advisory lock to prevent race conditions
        /// </summary>
        public async Task<string> GenerateNextLicenseNumber(int provinceId)
        {
            // Get province information
            var province = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == provinceId && l.TypeId == 2);

            if (province == null)
            {
                throw new ArgumentException($"Province with ID {provinceId} not found");
            }

            var provinceCode = GetProvinceCode(provinceId);

            // Use PostgreSQL advisory lock to prevent concurrent generation for same province
            // Lock key is based on province ID to allow concurrent generation for different provinces
            var lockKey = 1000000 + provinceId; // Offset to avoid conflicts with other locks
            
            try
            {
                // Acquire advisory lock (will wait if another transaction holds it)
                await _context.Database.ExecuteSqlRawAsync($"SELECT pg_advisory_xact_lock({lockKey})");
                
                // Get all license numbers for this province and find the max numeric value
                var licenses = await _context.LicenseDetails
                    .Where(l => l.LicenseNumber != null && l.LicenseNumber.StartsWith(provinceCode + "-"))
                    .Select(l => l.LicenseNumber)
                    .ToListAsync();

                int nextNumber = 1;

                if (licenses.Any())
                {
                    // Parse all license numbers and find the maximum
                    var maxNumber = licenses
                        .Select(ln => {
                            var parts = ln.Split('-');
                            if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                                return num;
                            return 0;
                        })
                        .Max();
                    
                    nextNumber = maxNumber + 1;
                }

                // Format: PROVINCE_CODE-00000001 (8 digits with leading zeros)
                return $"{provinceCode}-{nextNumber:D8}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating license number for province {provinceId}: {ex.Message}", ex);
            }
            // Advisory lock is automatically released when transaction commits or rolls back
        }

        /// <summary>
        /// Generate the next sequential license number for petition writer license
        /// Uses PostgreSQL advisory lock to prevent race conditions
        /// </summary>
        public async Task<string> GenerateNextPetitionWriterLicenseNumber(int provinceId)
        {
            // Get province information
            var province = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == provinceId && l.TypeId == 2);

            if (province == null)
            {
                throw new ArgumentException($"Province with ID {provinceId} not found");
            }

            var provinceCode = GetProvinceCode(provinceId);

            // Use PostgreSQL advisory lock to prevent concurrent generation for same province
            // Lock key is based on province ID + offset to separate from company licenses
            var lockKey = 2000000 + provinceId; // Different offset for petition writer licenses
            
            try
            {
                // Acquire advisory lock (will wait if another transaction holds it)
                await _context.Database.ExecuteSqlRawAsync($"SELECT pg_advisory_xact_lock({lockKey})");
                
                // Get all license numbers for this province and find the max numeric value
                var licenses = await _context.PetitionWriterLicenses
                    .Where(l => l.LicenseNumber != null && l.LicenseNumber.StartsWith(provinceCode + "-"))
                    .Select(l => l.LicenseNumber)
                    .ToListAsync();

                int nextNumber = 1;

                if (licenses.Any())
                {
                    // Parse all license numbers and find the maximum
                    var maxNumber = licenses
                        .Select(ln => {
                            var parts = ln.Split('-');
                            if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                                return num;
                            return 0;
                        })
                        .Max();
                    
                    nextNumber = maxNumber + 1;
                }

                // Format: PROVINCE_CODE-00000001 (8 digits with leading zeros)
                return $"{provinceCode}-{nextNumber:D8}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating petition writer license number for province {provinceId}: {ex.Message}", ex);
            }
            // Advisory lock is automatically released when transaction commits or rolls back
        }

        /// <summary>
        /// Get the province code for a given province ID
        /// </summary>
        public string GetProvinceCode(int provinceId)
        {
            var province = _context.Locations
                .FirstOrDefault(l => l.Id == provinceId && l.TypeId == 2);

            if (province == null)
            {
                throw new ArgumentException($"Province with ID {provinceId} not found");
            }

            if (ProvinceCodeMap.TryGetValue(province.Name, out var code))
            {
                return code;
            }

            // Fallback: use first 3 letters of province name in uppercase
            return province.Name.Length >= 3 
                ? province.Name.Substring(0, 3).ToUpper() 
                : province.Name.ToUpper();
        }
    }
}
