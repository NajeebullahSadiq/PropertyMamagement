namespace WebAPIBackend.Application.Reports.DTOs
{
    /// <summary>
    /// Generic report request with dynamic filtering
    /// </summary>
    public class ReportRequest
    {
        public string? ReportType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CalendarType { get; set; }
        public Dictionary<string, object> Filters { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

    /// <summary>
    /// Generic report result wrapper
    /// </summary>
    public class ReportResult<T> where T : class
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public ReportSummary? Summary { get; set; }
    }

    /// <summary>
    /// Report summary with aggregated values
    /// </summary>
    public class ReportSummary
    {
        public int TotalRecords { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalRoyalty { get; set; }
        public Dictionary<string, object> CustomSummary { get; set; } = new();
    }

    /// <summary>
    /// Securities report request
    /// </summary>
    public class SecuritiesReportRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CalendarType { get; set; }
        public string? LicenseNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? DocumentType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Securities report result
    /// </summary>
    public class SecuritiesReportResult
    {
        public List<SecuritiesReportItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public SecuritiesReportSummary? Summary { get; set; }
    }

    public class SecuritiesReportItem
    {
        public int Id { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LicenseOwnerName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? DocumentTypeName { get; set; }
        public int TotalDocuments { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? DeliveryDateFormatted { get; set; }
    }

    public class SecuritiesReportSummary
    {
        public int TotalRecords { get; set; }
        public int TotalDocuments { get; set; }
        public decimal TotalAmount { get; set; }
        public int PropertyDocuments { get; set; }
        public int VehicleDocuments { get; set; }
    }

    /// <summary>
    /// Petition writer report request
    /// </summary>
    public class PetitionWriterReportRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CalendarType { get; set; }
        public string? LicenseNumber { get; set; }
        public string? PetitionWriterName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Petition writer report result
    /// </summary>
    public class PetitionWriterReportResult
    {
        public List<PetitionWriterReportItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public PetitionWriterReportSummary? Summary { get; set; }
    }

    public class PetitionWriterReportItem
    {
        public int Id { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? PetitionWriterName { get; set; }
        public string? PetitionWriterFatherName { get; set; }
        public string? LicenseNumber { get; set; }
        public int? DocumentCount { get; set; }
        public string? SerialRange { get; set; }
        public decimal? Amount { get; set; }
        public string? DeliveryDateFormatted { get; set; }
    }

    public class PetitionWriterReportSummary
    {
        public int TotalRecords { get; set; }
        public int TotalDocuments { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
