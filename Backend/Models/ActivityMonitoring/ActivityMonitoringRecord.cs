using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان
    /// Monitoring of Real Estate Offices & Petition Writers Activities
    /// </summary>
    [Table("ActivityMonitoringRecords", Schema = "org")]
    public class ActivityMonitoringRecord
    {
        [Key]
        public int Id { get; set; }

        // 1️⃣ Financial Clearance (Tax Compliance)
        [Required]
        [MaxLength(200)]
        public string LicenseHolderName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TaxClearanceStatus { get; set; }

        [MaxLength(100)]
        public string? TaxClearanceLetterNumber { get; set; }

        public DateOnly? TaxClearanceDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidTaxAmount { get; set; }

        // 2️⃣ Annual Activity Report
        public DateOnly? ReportRegistrationDate { get; set; }

        public int? SaleDeedsCount { get; set; }
        public int? RentalDeedsCount { get; set; }
        public int? BaiUlWafaDeedsCount { get; set; }
        public int? VehicleTransactionDeedsCount { get; set; }
        public int? CancelledMixedTransactions { get; set; }
        public int? LostDeedsCount { get; set; }

        [MaxLength(1000)]
        public string? AnnualReportRemarks { get; set; }

        // 6️⃣ Inspection & Supervision Summary
        [Required]
        public DateOnly? InspectionDate { get; set; }

        public int? InspectedRealEstateOfficesCount { get; set; }
        public int? SealedOfficesCount { get; set; }
        public int? InspectedPetitionWritersCount { get; set; }
        public int? ViolatingPetitionWritersCount { get; set; }

        // Audit Fields
        public bool Status { get; set; } = true;
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
    }
}
