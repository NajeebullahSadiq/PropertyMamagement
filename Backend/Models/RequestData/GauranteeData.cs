namespace WebAPIBackend.Models.RequestData
{
    public class GauranteeData
    {
        public int Id { get; set; }

        public int? GuaranteeTypeId { get; set; }

        public int? PropertyDocumentNumber { get; set; }

        public string? PropertyDocumentDate { get; set; }

        public string? SenderMaktobNumber { get; set; }

        public string? SenderMaktobDate { get; set; }

        public int? AnswerdMaktobNumber { get; set; }

        public string? AnswerdMaktobDate { get; set; }

        public string? DateofGuarantee { get; set; }

        public int? GuaranteeDocNumber { get; set; }

        public string? GuaranteeDate { get; set; }

        public int? CompanyId { get; set; }

        public string? DocPath { get; set; }
    }
}
