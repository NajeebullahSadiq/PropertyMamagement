using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// 3️⃣ Complaints Registration
    /// ثبت شکایات
    /// </summary>
    [Table("ActivityMonitoringComplaints", Schema = "org")]
    public class Complaint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityMonitoringRecordId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ComplaintSerialNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ComplainantName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ComplaintSubject { get; set; } = string.Empty;

        [Required]
        public DateOnly? ComplaintRegistrationDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string AccusedPartyName { get; set; } = string.Empty;

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
