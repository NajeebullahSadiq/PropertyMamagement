using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// 5️⃣ Inspection Records
    /// نظارت وبررسی فعالیت دفاتر رهنمای معاملات و عریضه نویسان
    /// </summary>
    [Table("ActivityMonitoringInspections", Schema = "org")]
    public class Inspection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityMonitoringRecordId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MonitoringType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Month { get; set; } = string.Empty;

        [Required]
        public int MonitoringCount { get; set; }

        // Audit
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        // Navigation
        [ForeignKey("ActivityMonitoringRecordId")]
        public virtual ActivityMonitoringRecord? ActivityMonitoringRecord { get; set; }
    }
}
