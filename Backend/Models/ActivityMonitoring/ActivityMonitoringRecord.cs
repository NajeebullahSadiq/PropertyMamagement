using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان
    /// Monitoring of Real Estate Offices & Petition Writers Activities
    /// Single Table Design - All sections in one entity
    /// </summary>
    [Table("ActivityMonitoringRecords", Schema = "org")]
    public class ActivityMonitoringRecord
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        // ============ Common Fields (Section 1: Annual Report) ============
        [MaxLength(50)]
        [Column("SerialNumber")]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        [Column("LicenseNumber")]
        public string? LicenseNumber { get; set; }

        [MaxLength(200)]
        [Column("LicenseHolderName")]
        public string? LicenseHolderName { get; set; }

        [MaxLength(200)]
        [Column("District")]
        public string? District { get; set; }

        [Column("ReportRegistrationDate")]
        public DateOnly? ReportRegistrationDate { get; set; }

        [MaxLength(50)]
        [Column("SectionType")]
        public string? SectionType { get; set; }  // complaints, violations, inspection

        // Deed Counts
        [Column("SaleDeedsCount")]
        public int? SaleDeedsCount { get; set; }
        
        [Column("RentalDeedsCount")]
        public int? RentalDeedsCount { get; set; }
        
        [Column("BaiUlWafaDeedsCount")]
        public int? BaiUlWafaDeedsCount { get; set; }
        
        [Column("VehicleTransactionDeedsCount")]
        public int? VehicleTransactionDeedsCount { get; set; }

        [MaxLength(1000)]
        [Column("AnnualReportRemarks")]
        public string? AnnualReportRemarks { get; set; }

        // Deed Items as JSONB (flexible array storage)
        [Column("DeedItems", TypeName = "jsonb")]
        public string? DeedItems { get; set; }

        // ============ Section 2: Complaints (ثبت شکایات) ============
        [Column("ComplaintRegistrationDate")]
        public DateOnly? ComplaintRegistrationDate { get; set; }

        [MaxLength(500)]
        [Column("ComplaintSubject")]
        public string? ComplaintSubject { get; set; }

        [MaxLength(200)]
        [Column("ComplainantName")]
        public string? ComplainantName { get; set; }

        [MaxLength(1000)]
        [Column("ComplaintActionsTaken")]
        public string? ComplaintActionsTaken { get; set; }

        [MaxLength(1000)]
        [Column("ComplaintRemarks")]
        public string? ComplaintRemarks { get; set; }

        // ============ Section 3: Violations (تخلفات دفاتر رهنمای معاملات) ============
        [MaxLength(100)]
        [Column("ViolationStatus")]
        public string? ViolationStatus { get; set; }

        [MaxLength(500)]
        [Column("ViolationType")]
        public string? ViolationType { get; set; }

        [Column("ViolationDate")]
        public DateOnly? ViolationDate { get; set; }

        [Column("ClosureDate")]
        public DateOnly? ClosureDate { get; set; }

        [MaxLength(500)]
        [Column("ClosureReason")]
        public string? ClosureReason { get; set; }

        [MaxLength(1000)]
        [Column("ViolationActionsTaken")]
        public string? ViolationActionsTaken { get; set; }

        [MaxLength(1000)]
        [Column("ViolationRemarks")]
        public string? ViolationRemarks { get; set; }

        // ============ Section 4: Inspections (نظارت و بازرسی) ============
        [MaxLength(100)]
        [Column("MonitoringType")]
        public string? MonitoringType { get; set; }

        [MaxLength(50)]
        [Column("Month")]
        public string? Month { get; set; }

        [Column("MonitoringCount")]
        public int? MonitoringCount { get; set; }

        // ============ Audit Fields ============
        [Column("Status")]
        public bool Status { get; set; } = true;
        
        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        [Column("CreatedBy")]
        public string? CreatedBy { get; set; }

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(50)]
        [Column("UpdatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
