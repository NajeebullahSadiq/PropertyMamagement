using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData.PetitionWriterMonitoring
{
    /// <summary>
    /// DTO for Petition Writer Monitoring Record API requests - Single Table Design
    /// All sections in one DTO
    /// </summary>
    public class PetitionWriterMonitoringData
    {
        public int? Id { get; set; }

        // ============ Common Fields ============
        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? SectionType { get; set; }  // complaints, violations, monitoring

        public string? RegistrationDate { get; set; }

        // ============ Section 1: Complaints Registration ============
        [MaxLength(200)]
        public string? ComplainantName { get; set; }

        [MaxLength(500)]
        public string? ComplaintSubject { get; set; }

        [MaxLength(1000)]
        public string? ComplaintActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? ComplaintRemarks { get; set; }

        // ============ Section 2: Violations ============
        [MaxLength(200)]
        public string? PetitionWriterName { get; set; }

        [MaxLength(50)]
        public string? PetitionWriterLicenseNumber { get; set; }

        [MaxLength(200)]
        public string? PetitionWriterDistrict { get; set; }

        [MaxLength(500)]
        public string? ViolationType { get; set; }

        [MaxLength(1000)]
        public string? ViolationActionsTaken { get; set; }

        [MaxLength(1000)]
        public string? ViolationRemarks { get; set; }

        [MaxLength(50)]
        public string? ActivityStatus { get; set; }  // activity_prevention, activity_permission

        [MaxLength(500)]
        public string? ActivityPermissionReason { get; set; }

        // ============ Section 3: Monitoring Activities ============
        [MaxLength(20)]
        public string? MonitoringYear { get; set; }

        [MaxLength(50)]
        public string? MonitoringMonth { get; set; }

        public int? MonitoringCount { get; set; }

        [MaxLength(1000)]
        public string? MonitoringRemarks { get; set; }

        // ============ Calendar Type for Date Conversion ============
        public string? CalendarType { get; set; }
    }
}
