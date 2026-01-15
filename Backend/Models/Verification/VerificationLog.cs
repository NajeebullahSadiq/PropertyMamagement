using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.Verification
{
    /// <summary>
    /// Verification Attempt Log - Records all verification attempts for audit purposes
    /// سجل تلاش‌های تایید - ثبت تمام تلاش‌های تایید برای اهداف حسابرسی
    /// </summary>
    [Table("VerificationLogs", Schema = "org")]
    public class VerificationLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The verification code that was attempted
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the verification attempt
        /// </summary>
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// IP address of the requester
        /// </summary>
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// Whether the verification was successful
        /// </summary>
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Reason for failure if verification failed
        /// </summary>
        [MaxLength(50)]
        public string? FailureReason { get; set; }
    }
}
