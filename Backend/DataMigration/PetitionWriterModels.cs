using System.Text.Json.Serialization;

namespace DataMigration
{
    // ============================================================
    // PETITION WRITER MODULE MODELS
    // ============================================================
    
    public class PetitionWriterRecord
    {
        [JsonPropertyName("SequenceNumber")]
        public double? SequenceNumber { get; set; }
        
        [JsonPropertyName("ApplicantName")]
        public string? ApplicantName { get; set; }
        
        [JsonPropertyName("FatherName")]
        public string? FatherName { get; set; }
        
        [JsonPropertyName("Tazkera_Jold")]
        public object? TazkeraJold { get; set; }
        
        [JsonPropertyName("Tazkera_Page")]
        public object? TazkeraPage { get; set; }
        
        [JsonPropertyName("ElectronicNationalIdNumber")]
        public object? ElectronicNationalIdNumber { get; set; }
        
        // Permanent Address
        [JsonPropertyName("PermanentVillage")]
        public object? PermanentVillage { get; set; }
        
        [JsonPropertyName("PermanentDistrict")]
        public string? PermanentDistrict { get; set; }
        
        [JsonPropertyName("PermanentProvince")]
        public string? PermanentProvince { get; set; }
        
        // Current Address
        [JsonPropertyName("CurrentVillage_Nahia")]
        public object? CurrentVillageNahia { get; set; }
        
        [JsonPropertyName("CurrentDistrict")]
        public string? CurrentDistrict { get; set; }
        
        [JsonPropertyName("CurrentProvince")]
        public string? CurrentProvince { get; set; }
        
        // License Info
        [JsonPropertyName("MobileNumber")]
        public object? MobileNumber { get; set; }
        
        [JsonPropertyName("LicenseNumber")]
        public object? LicenseNumber { get; set; }
        
        [JsonPropertyName("LicenseIssueDate")]
        public string? LicenseIssueDate { get; set; }
        
        [JsonPropertyName("LicenseType_New")]
        public object? LicenseTypeNew { get; set; }
        
        [JsonPropertyName("LicenseType_Renewal")]
        public object? LicenseTypeRenewal { get; set; }
        
        // Activity Location (different field names in different years)
        [JsonPropertyName("DetailedAddress")]
        public string? DetailedAddress { get; set; }
        
        [JsonPropertyName("ActivityLocation")]
        public string? ActivityLocation { get; set; }
        
        [JsonPropertyName("District_Nahia")]
        public object? DistrictNahia { get; set; }
        
        // Relocation
        [JsonPropertyName("RelocationInfo")]
        public object? RelocationInfo { get; set; }
        
        // Source tracking
        [JsonPropertyName("SourceYear")]
        public int? SourceYear { get; set; }
    }
}
