using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData.ActivityMonitoring
{
    /// <summary>
    /// DTO for Activity Monitoring Record API requests
    /// </summary>
    public class ActivityMonitoringData
    {
        public int? Id { get; set; }

        // 1️⃣ Financial Clearance
        [Required(ErrorMessage = "شهرت دارنده جواز الزامی است")]
        [MaxLength(200)]
        public string LicenseHolderName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TaxClearanceStatus { get; set; }

        [MaxLength(100)]
        public string? TaxClearanceLetterNumber { get; set; }

        [Required(ErrorMessage = "تاریخ تصفیه مالیاتی الزامی است")]
        public string? TaxClearanceDate { get; set; }

        public decimal? PaidTaxAmount { get; set; }

        // 2️⃣ Annual Activity Report
        public string? ReportRegistrationDate { get; set; }
        public int? SaleDeedsCount { get; set; }
        public int? RentalDeedsCount { get; set; }
        public int? BaiUlWafaDeedsCount { get; set; }
        public int? VehicleTransactionDeedsCount { get; set; }
        public int? CancelledMixedTransactions { get; set; }
        public int? LostDeedsCount { get; set; }

        [MaxLength(1000)]
        public string? AnnualReportRemarks { get; set; }

        // 6️⃣ Inspection & Supervision
        [Required(ErrorMessage = "تاریخ نظارت الزامی است")]
        public string? InspectionDate { get; set; }

        public int? InspectedRealEstateOfficesCount { get; set; }
        public int? SealedOfficesCount { get; set; }
        public int? InspectedPetitionWritersCount { get; set; }
        public int? ViolatingPetitionWritersCount { get; set; }

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Complaint API requests
    /// </summary>
    public class ComplaintData
    {
        public int? Id { get; set; }
        public int? ActivityMonitoringRecordId { get; set; }

        [Required(ErrorMessage = "نمبر مسلسل الزامی است")]
        [MaxLength(50)]
        public string ComplaintSerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "شهرت عارض الزامی است")]
        [MaxLength(200)]
        public string ComplainantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "موضوع شکایت الزامی است")]
        [MaxLength(500)]
        public string ComplaintSubject { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاریخ ثبت شکایت الزامی است")]
        public string? ComplaintRegistrationDate { get; set; }

        [Required(ErrorMessage = "شهرت معروض علیه الزامی است")]
        [MaxLength(200)]
        public string AccusedPartyName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? ActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Real Estate Violation API requests
    /// </summary>
    public class RealEstateViolationData
    {
        public int? Id { get; set; }
        public int? ActivityMonitoringRecordId { get; set; }

        [Required(ErrorMessage = "نمبر مسلسل الزامی است")]
        [MaxLength(50)]
        public string ViolationSerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "شهرت دارنده جواز الزامی است")]
        [MaxLength(200)]
        public string LicenseHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوعیت تخلف الزامی است")]
        [MaxLength(500)]
        public string ViolationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاریخ ثبت تخلف الزامی است")]
        public string? ViolationDate { get; set; }

        [MaxLength(1000)]
        public string? ActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Petition Writer Violation API requests
    /// </summary>
    public class PetitionWriterViolationData
    {
        public int? Id { get; set; }
        public int? ActivityMonitoringRecordId { get; set; }

        [Required(ErrorMessage = "نمبر مسلسل الزامی است")]
        [MaxLength(50)]
        public string ViolationSerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "شهرت عریضه نویس الزامی است")]
        [MaxLength(200)]
        public string PetitionWriterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوعیت تخلف الزامی است")]
        [MaxLength(500)]
        public string ViolationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاریخ ثبت تخلف الزامی است")]
        public string? ViolationDate { get; set; }

        [MaxLength(1000)]
        public string? ActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public string? CalendarType { get; set; }
    }
}
