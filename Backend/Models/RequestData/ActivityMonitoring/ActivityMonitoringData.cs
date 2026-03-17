using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebAPIBackend.Models.RequestData.ActivityMonitoring
{
    /// <summary>
    /// DTO for Activity Monitoring Record API requests - Single Table Design
    /// All sections in one DTO
    /// </summary>
    public class ActivityMonitoringData
    {
        public int? Id { get; set; }

        // ============ Common Fields ============
        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [MaxLength(200)]
        public string? LicenseHolderName { get; set; }

        [MaxLength(200)]
        public string? District { get; set; }

        public string? ReportRegistrationDate { get; set; }

        [MaxLength(50)]
        public string? SectionType { get; set; }  // complaints, violations, inspection

        // ============ Deed Counts ============
        public int? SaleDeedsCount { get; set; }
        public int? RentalDeedsCount { get; set; }
        public int? BaiUlWafaDeedsCount { get; set; }
        public int? VehicleTransactionDeedsCount { get; set; }

        [MaxLength(1000)]
        public string? AnnualReportRemarks { get; set; }

        // Deed items with serial numbers (stored as JSON)
        public List<DeedItemData>? DeedItems { get; set; }

        // ============ Section 2: Complaints ============
        public string? ComplaintRegistrationDate { get; set; }

        [MaxLength(500)]
        public string? ComplaintSubject { get; set; }

        [MaxLength(200)]
        public string? ComplainantName { get; set; }

        [MaxLength(1000)]
        public string? ComplaintActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? ComplaintRemarks { get; set; }

        // ============ Section 3: Violations ============
        [MaxLength(100)]
        public string? ViolationStatus { get; set; }

        [MaxLength(500)]
        public string? ViolationType { get; set; }

        public string? ViolationDate { get; set; }

        public string? ClosureDate { get; set; }

        [MaxLength(500)]
        public string? ClosureReason { get; set; }

        [MaxLength(1000)]
        public string? ViolationActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? ViolationRemarks { get; set; }

        // ============ Section 4: Inspections ============
        [MaxLength(100)]
        public string? MonitoringType { get; set; }

        [MaxLength(50)]
        public string? Month { get; set; }

        public int? MonitoringCount { get; set; }

        // ============ Calendar Type for Date Conversion ============
        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Deed Item with serial numbers
    /// </summary>
    public class DeedItemData
    {
        public int? Id { get; set; }
        public int DeedType { get; set; }
        
        [MaxLength(100)]
        public string? SerialStart { get; set; }
        
        [MaxLength(100)]
        public string? SerialEnd { get; set; }
        
        public int Count { get; set; }
    }
}
