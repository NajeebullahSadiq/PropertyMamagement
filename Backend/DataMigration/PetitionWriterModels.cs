using System.Text.Json.Serialization;

namespace DataMigration
{
    // ============================================================
    // PETITION WRITER MODULE MODELS
    // ============================================================
    
    public class PetitionWriterRecord
    {
        [JsonPropertyName("ApplicantName")]
        public object? ApplicantName { get; set; }
        
        [JsonPropertyName("ApplicantFatherName")]
        public object? ApplicantFatherName { get; set; }
        
        [JsonPropertyName("ElectronicNationalIdNumber")]
        public object? ElectronicNationalIdNumber { get; set; }
        
        // Current Address
        [JsonPropertyName("CurrentVillage")]
        public object? CurrentVillage { get; set; }
        
        [JsonPropertyName("CurrentDistrictId")]
        public string? CurrentDistrictId { get; set; }
        
        [JsonPropertyName("CurrentProvinceId")]
        public string? CurrentProvinceId { get; set; }
        
        // Permanent Address
        [JsonPropertyName("PermanentVillage")]
        public object? PermanentVillage { get; set; }
        
        [JsonPropertyName("PermanentDistrictId")]
        public string? PermanentDistrictId { get; set; }
        
        [JsonPropertyName("PermanentProvinceId")]
        public string? PermanentProvinceId { get; set; }
        
        // License Info
        [JsonPropertyName("MobileNumber")]
        public object? MobileNumber { get; set; }
        
        [JsonPropertyName("LicenseNumber")]
        public object? LicenseNumber { get; set; }
        
        [JsonPropertyName("LicenseIssueDate")]
        public string? LicenseIssueDate { get; set; }
        
        [JsonPropertyName("LicenseType(new)")]
        public object? LicenseTypeNew { get; set; }
        
        [JsonPropertyName("LicenseType(extend)")]
        public object? LicenseTypeExtend { get; set; }
        
        [JsonPropertyName("LicenseType")]
        public string? LicenseType { get; set; }
        
        // Activity Location
        [JsonPropertyName("DetailedAddress")]
        public string? DetailedAddress { get; set; }
        
        [JsonPropertyName("ActivityNahia")]
        public string? ActivityNahia { get; set; }
        
        // Relocation
        [JsonPropertyName("PetitionWriterRelocations")]
        public object? PetitionWriterRelocations { get; set; }
        
        // Province
        [JsonPropertyName("ProvinceId")]
        public int? ProvinceId { get; set; }
    }
}
