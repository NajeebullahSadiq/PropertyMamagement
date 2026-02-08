using System.Text.Json.Serialization;

namespace DataMigration
{
    // ============================================================
    // SECURITIES MODULE MODELS
    // ============================================================
    
    public class SecuritiesRecord
    {
        [JsonPropertyName("RegistrationNumber")]
        public object? RegistrationNumber { get; set; }
        
        [JsonPropertyName("LicenseOwnerName")]
        public object? LicenseOwnerName { get; set; }
        
        [JsonPropertyName("LicenseOwnerFatherName")]
        public object? LicenseOwnerFatherName { get; set; }
        
        [JsonPropertyName("TransactionGuideName")]
        public object? TransactionGuideName { get; set; }
        
        [JsonPropertyName("LicenseNumber")]
        public object? LicenseNumber { get; set; }
        
        // Securities Serial Numbers (4 types)
        [JsonPropertyName("PropertySale_Serial")]
        public object? PropertySaleSerial { get; set; }
        
        [JsonPropertyName("BayWafa_Serial")]
        public object? BayWafaSerial { get; set; }
        
        [JsonPropertyName("Rent_Serial")]
        public object? RentSerial { get; set; }
        
        [JsonPropertyName("Vehicle_Serial")]
        public object? VehicleSerial { get; set; }
        
        // Quantities
        [JsonPropertyName("PropertySale_Count")]
        public object? PropertySaleCount { get; set; }
        
        [JsonPropertyName("BayWafa_Count")]
        public object? BayWafaCount { get; set; }
        
        [JsonPropertyName("Rent_Count")]
        public object? RentCount { get; set; }
        
        [JsonPropertyName("Vehicle_Count")]
        public object? VehicleCount { get; set; }
        
        [JsonPropertyName("TotalSecuritiesCount")]
        public object? TotalSecuritiesCount { get; set; }
        
        [JsonPropertyName("RegistrationBook_Count")]
        public object? RegistrationBookCount { get; set; }
        
        // Pricing
        [JsonPropertyName("PricePerDocument")]
        public double? PricePerDocument { get; set; }
        
        [JsonPropertyName("TotalSecuritiesPrice")]
        public double? TotalSecuritiesPrice { get; set; }
        
        [JsonPropertyName("RegistrationBookPrice")]
        public object? RegistrationBookPrice { get; set; }
        
        [JsonPropertyName("TotalPrice")]
        public double? TotalPrice { get; set; }
        
        // Receipt and Dates
        [JsonPropertyName("BankReceiptInfo")]
        public object? BankReceiptInfo { get; set; }
        
        [JsonPropertyName("DistributionDate")]
        public object? DistributionDate { get; set; }
    }
}
