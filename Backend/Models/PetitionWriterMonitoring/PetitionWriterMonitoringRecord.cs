using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.PetitionWriterMonitoring
{
    /// <summary>
    /// ثبت نظارت بر فعالیت عریضه نویسان
    /// Registration of Monitoring the Activities of Petition Writers
    /// Single Table Design - All sections in one entity
    /// </summary>
    [Table("PetitionWriterMonitoringRecords", Schema = "org")]
    public class PetitionWriterMonitoringRecord
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        // ============ Common Fields ============
        [MaxLength(50)]
        [Column("SerialNumber")]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        [Column("SectionType")]
        public string? SectionType { get; set; }  // complaints, violations, monitoring

        [Column("RegistrationDate")]
        public DateOnly? RegistrationDate { get; set; }

        // ============ Section 1: Complaints Registration (ثبت شکایات) ============
        [MaxLength(200)]
        [Column("ComplainantName")]
        public string? ComplainantName { get; set; }

        [MaxLength(500)]
        [Column("ComplaintSubject")]
        public string? ComplaintSubject { get; set; }

        [MaxLength(1000)]
        [Column("ComplaintActionsTaken")]
        public string? ComplaintActionsTaken { get; set; }

        [MaxLength(1000)]
        [Column("ComplaintRemarks")]
        public string? ComplaintRemarks { get; set; }

        // ============ Section 2: Violations (تخلفات عریضه نویسان) ============
        [MaxLength(200)]
        [Column("PetitionWriterName")]
        public string? PetitionWriterName { get; set; }

        [MaxLength(50)]
        [Column("PetitionWriterLicenseNumber")]
        public string? PetitionWriterLicenseNumber { get; set; }

        [MaxLength(200)]
        [Column("PetitionWriterDistrict")]
        public string? PetitionWriterDistrict { get; set; }

        [MaxLength(500)]
        [Column("ViolationType")]
        public string? ViolationType { get; set; }

        [MaxLength(1000)]
        [Column("ViolationActionsTaken")]
        public string? ViolationActionsTaken { get; set; }

        [MaxLength(1000)]
        [Column("ViolationRemarks")]
        public string? ViolationRemarks { get; set; }

        [MaxLength(50)]
        [Column("ActivityStatus")]
        public string? ActivityStatus { get; set; }  // activity_prevention, activity_permission

        [MaxLength(500)]
        [Column("ActivityPermissionReason")]
        public string? ActivityPermissionReason { get; set; }

        // ============ Section 3: Monitoring Activities (نظارت فعالیت عریضه نویسان) ============
        [MaxLength(20)]
        [Column("MonitoringYear")]
        public string? MonitoringYear { get; set; }

        [MaxLength(50)]
        [Column("MonitoringMonth")]
        public string? MonitoringMonth { get; set; }  // Hamal, Saur, Jawza, etc.

        [Column("MonitoringCount")]
        public int? MonitoringCount { get; set; }

        [MaxLength(1000)]
        [Column("MonitoringRemarks")]
        public string? MonitoringRemarks { get; set; }

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
