namespace WebAPIBackend.Models.RequestData
{
    public class LicenseDetailData
    {
        public int Id { get; set; }

        public double LicenseNumber { get; set; }

        public string? IssueDate { get; set; }

        public string? ExpireDate { get; set; }

        public int? AreaId { get; set; }

        public string? OfficeAddress { get; set; }

        public int? CompanyId { get; set; }

        public string? DocPath { get; set; }

        public string? LicenseType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public bool? Status { get; set; }

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// </summary>
        public string? CalendarType { get; set; }
    }
}
