using System.Text.Json.Serialization;

namespace DataMigration
{
    public class OldRecord
    {
        [JsonPropertyName("UserDisplay")]
        public double? UserDisplay { get; set; }
        
        [JsonPropertyName("RID")]
        public int RID { get; set; }
        
        [JsonPropertyName("Education")]
        public string Education { get; set; }
        
        [JsonPropertyName("LicenseNo")]
        public int? LicenseNo { get; set; }
        
        [JsonPropertyName("FName")]
        public string FName { get; set; }
        
        [JsonPropertyName("FathName")]
        public string FathName { get; set; }
        
        [JsonPropertyName("GFName")]
        public string GFName { get; set; }
        
        [JsonPropertyName("DOB")]
        public string DOB { get; set; }
        
        [JsonPropertyName("TazkeraNo")]
        public string TazkeraNo { get; set; }
        
        [JsonPropertyName("Page")]
        public string Page { get; set; }
        
        [JsonPropertyName("Jold")]
        public string Jold { get; set; }
        
        [JsonPropertyName("ContactNo")]
        public string ContactNo { get; set; }
        
        [JsonPropertyName("EmailID")]
        public string EmailID { get; set; }
        
        [JsonPropertyName("PerProvince")]
        public string PerProvince { get; set; }
        
        [JsonPropertyName("PerWoloswaly")]
        public string PerWoloswaly { get; set; }
        
        [JsonPropertyName("TempProvince")]
        public string TempProvince { get; set; }
        
        [JsonPropertyName("TempWoloswaly")]
        public string TempWoloswaly { get; set; }
        
        [JsonPropertyName("ExactAddress")]
        public string ExactAddress { get; set; }
        
        [JsonPropertyName("DistLocation")]
        public string DistLocation { get; set; }
        
        [JsonPropertyName("SYear")]
        public double? SYear { get; set; }
        
        [JsonPropertyName("SMonth")]
        public double? SMonth { get; set; }
        
        [JsonPropertyName("SDay")]
        public double? SDay { get; set; }
        
        [JsonPropertyName("EYear")]
        public double? EYear { get; set; }
        
        [JsonPropertyName("EMonth")]
        public double? EMonth { get; set; }
        
        [JsonPropertyName("EDay")]
        public double? EDay { get; set; }
        
        [JsonPropertyName("RealEstateName")]
        public string RealEstateName { get; set; }
        
        [JsonPropertyName("LicenseType")]
        public string LicenseType { get; set; }
        
        [JsonPropertyName("CreditRightNo")]
        public string CreditRightNo { get; set; }
        
        [JsonPropertyName("CreditRightAmount")]
        public double? CreditRightAmount { get; set; }
        
        [JsonPropertyName("LateFine")]
        public double? LateFine { get; set; }
        
        [JsonPropertyName("RecordBook")]
        public double? RecordBook { get; set; }
        
        [JsonPropertyName("CreditRightYear")]
        public double? CreditRightYear { get; set; }
        
        [JsonPropertyName("CreditRightMonth")]
        public double? CreditRightMonth { get; set; }
        
        [JsonPropertyName("CreditRightDay")]
        public double? CreditRightDay { get; set; }
        
        [JsonPropertyName("TIN")]
        public string TIN { get; set; }
        
        [JsonPropertyName("HRNo")]
        public double? HRNo { get; set; }
        
        [JsonPropertyName("Combo211")]
        public double? Combo211 { get; set; }
        
        [JsonPropertyName("HRMonth")]
        public double? HRMonth { get; set; }
        
        [JsonPropertyName("HRDay")]
        public double? HRDay { get; set; }
        
        [JsonPropertyName("Halat")]
        public string Halat { get; set; }
        
        [JsonPropertyName("FK")]
        public int FK { get; set; }
        
        [JsonPropertyName("LicnsCancelNo")]
        public string LicnsCancelNo { get; set; }
        
        [JsonPropertyName("CancelAmount")]
        public string CancelAmount { get; set; }
        
        [JsonPropertyName("CancelYear")]
        public double? CancelYear { get; set; }
        
        [JsonPropertyName("CancelMonth")]
        public double? CancelMonth { get; set; }
        
        [JsonPropertyName("CancelDay")]
        public double? CancelDay { get; set; }
        
        [JsonPropertyName("Remarks")]
        public string Remarks { get; set; }
        
        [JsonPropertyName("SearchFor")]
        public double? SearchFor { get; set; }
        
        [JsonPropertyName("SearchResults")]
        public double? SearchResults { get; set; }
        
        [JsonPropertyName("Text34")]
        public int Text34 { get; set; }
    }
    
    public class MigrationStats
    {
        public int TotalRecords { get; set; }
        public int CompaniesCreated { get; set; }
        public int OwnersCreated { get; set; }
        public int LicensesCreated { get; set; }
        public int CancellationsCreated { get; set; }
        public List<MigrationError> Errors { get; set; } = new List<MigrationError>();
        public List<SkippedRecord> Skipped { get; set; } = new List<SkippedRecord>();
    }
    
    public class MigrationError
    {
        public int RecordId { get; set; }
        public string ErrorMessage { get; set; }
        public OldRecord RecordData { get; set; }
    }
    
    public class SkippedRecord
    {
        public int RecordId { get; set; }
        public string Reason { get; set; }
    }
}
