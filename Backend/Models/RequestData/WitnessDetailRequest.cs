using System;

namespace WebAPIBackend.Models.RequestData
{
    public class WitnessDetailRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string FatherName { get; set; } = null!;
        public string? GrandFatherName { get; set; }
        // Electronic National ID - الیکټرونیکی تذکره
        public string? ElectronicNationalIdNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public int? PropertyDetailsId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? NationalIdCard { get; set; }
        public int? PaddressProvinceId { get; set; }
        public int? PaddressDistrictId { get; set; }
        public string? PaddressVillage { get; set; }
        public string? RelationshipToParties { get; set; }
        public string? WitnessType { get; set; }
        public string? WitnessSide { get; set; }
        public string? Des { get; set; }
    }
}
