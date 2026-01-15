using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.LicenseApplication
{
    /// <summary>
    /// انصراف - Withdrawal Information for License Application
    /// </summary>
    [Table("LicenseApplicationWithdrawals", Schema = "org")]
    public class LicenseApplicationWithdrawal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LicenseApplicationId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string WithdrawalReason { get; set; } = string.Empty;

        public DateOnly? WithdrawalDate { get; set; }

        // Audit Fields
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        // Navigation Property
        [ForeignKey("LicenseApplicationId")]
        public virtual LicenseApplicationEntity? LicenseApplication { get; set; }
    }
}
