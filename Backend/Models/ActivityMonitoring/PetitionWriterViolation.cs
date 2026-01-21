using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// 5️⃣ Violations – Petition Writers
    /// تخلفات عریضه نویسان
    /// </summary>
    [Table("ActivityMonitoringPetitionWriterViolations", Schema = "org")]
    public class PetitionWriterViolation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityMonitoringRecordId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ViolationSerialNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PetitionWriterName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ViolationType { get; set; } = string.Empty;

        [Required]
        public DateOnly? ViolationDate { get; set; }

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
