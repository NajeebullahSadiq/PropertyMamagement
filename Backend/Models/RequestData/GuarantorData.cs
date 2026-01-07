namespace WebAPIBackend.Models.RequestData
{
    public class GuarantorData
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string FatherName { get; set; } = null!;

        public string? GrandFatherName { get; set; }

        public int? IdentityCardTypeId { get; set; }

        public int? CompanyId { get; set; }

        public double? IndentityCardNumber { get; set; }

        public int? Jild { get; set; }

        public int? Safha { get; set; }

        public int? SabtNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public string? PothoPath { get; set; }

        public int? PaddressProvinceId { get; set; }

        public int? PaddressDistrictId { get; set; }

        public string? PaddressVillage { get; set; }

        public int? TaddressProvinceId { get; set; }

        public int? TaddressDistrictId { get; set; }

        public string? TaddressVillage { get; set; }

        // Guarantee Information
        public int? GuaranteeTypeId { get; set; }

        public long? PropertyDocumentNumber { get; set; }

        public string? PropertyDocumentDate { get; set; }

        public string? SenderMaktobNumber { get; set; }

        public string? SenderMaktobDate { get; set; }

        public long? AnswerdMaktobNumber { get; set; }

        public string? AnswerdMaktobDate { get; set; }

        public string? DateofGuarantee { get; set; }

        public long? GuaranteeDocNumber { get; set; }

        public string? GuaranteeDate { get; set; }

        public string? GuaranteeDocPath { get; set; }

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// </summary>
        public string? CalendarType { get; set; }
    }
}
