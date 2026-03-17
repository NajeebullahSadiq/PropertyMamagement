using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// 4️⃣ Violations – Real Estate Offices
    /// تخلفات دفاتر رهنمای معاملات
    /// </summary>
    [Table("ActivityMonitoringRealEstateViolations", Schema = "org")]
    public class RealEstateViolation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityMonitoringRecordId { get; set; }

        [MaxLength(50)]
        public string? ViolationStatus { get; set; }

        // For عادی (Normal)
        [MaxLength(500)]
        public string? ViolationType { get; set; }

        public DateOnly? ViolationDate { get; set; }

        // For منجر به مسدودی (Leading to Closure)
        public DateOnly? ClosureDate { get; set; }

        [MaxLength(1000)]
        public string? ClosureReason { get; set; }

        [MaxLength(1000)]
        public string? ActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        // Audit
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        // Navigation
        [ForeignKey("ActivityMonitoringRecordId")]
        public virtual ActivityMonitoringRecord? ActivityMonitoringRecord { get; set; }
    }
}
