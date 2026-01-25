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

            // Get the last license number for this province
            var lastLicense = await _context.LicenseDetails
                .Where(l => l.LicenseNumber != null && l.LicenseNumber.StartsWith(provinceCode + "-"))
                .OrderByDescending(l => l.Id)
                .Select(l => l.LicenseNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastLicense != null)
            {
                // Extract the number part after the dash
                var parts = lastLicense.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            // Format: PROVINCE_CODE-0001 (4 digits with leading zeros)
            return $"{provinceCode}-{nextNumber:D4}";
        }

        /// <summary>
        /// Generate the next sequential license number for petition writer license
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

            // Get the last license number for this province
            var lastLicense = await _context.PetitionWriterLicenses
                .Where(l => l.LicenseNumber != null && l.LicenseNumber.StartsWith(provinceCode + "-"))
                .OrderByDescending(l => l.Id)
                .Select(l => l.LicenseNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastLicense != null)
            {
                // Extract the number part after the dash
                var parts = lastLicense.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            // Format: PROVINCE_CODE-0001 (4 digits with leading zeros)
            return $"{provinceCode}-{nextNumber:D4}";
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
