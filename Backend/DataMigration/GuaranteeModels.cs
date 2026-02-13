using System.Text.Json.Serialization;

namespace DataMigration
{
    public class GuaranteeRecord
    {
        [JsonPropertyName("FK")]
        public double? FK { get; set; }
        
        [JsonPropertyName("GuaranteeType")]
        public string? GuaranteeType { get; set; }
        
        [JsonPropertyName("GName")]
        public string? GName { get; set; }
        
        [JsonPropertyName("GFName")]
        public string? GFName { get; set; }
        
        [JsonPropertyName("GRelation")]
        public string? GRelation { get; set; }
        
        [JsonPropertyName("GContact")]
        public string? GContact { get; set; }
        
        [JsonPropertyName("GTazkeraNo")]
        public string? GTazkeraNo { get; set; }
        
        [JsonPropertyName("GJold")]
        public string? GJold { get; set; }
        
        [JsonPropertyName("GPage")]
        public string? GPage { get; set; }
        
        [JsonPropertyName("GReferenceNo")]
        public string? GReferenceNo { get; set; }
        
        [JsonPropertyName("GBank")]
        public object? GBank { get; set; }
        
        [JsonPropertyName("GAddress")]
        public string? GAddress { get; set; }
        
        [JsonPropertyName("Remarks")]
        public string? Remarks { get; set; }
    }
}
