using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.LicenseApplication
{
    /// <summary>
    /// ثبت درخواست متقاضیان جواز رهنمای معاملات
    /// License Application for Real Estate Guides
    /// </summary>
    [Table("LicenseApplications", Schema = "org")]
    public class LicenseApplicationEntity
    {
        [Key]
        public int Id { get; set; }

        // Tab 1: مشخصات درخواست متقاضی
        public DateOnly? RequestDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string RequestSerialNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ApplicantFatherName { get; set; }

        [MaxLength(200)]
        public string? ApplicantGrandfatherName { get; set; }

        [MaxLength(50)]
        public string? ApplicantElectronicNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProposedGuideName { get; set; } = string.Empty;

        // Permanent Address (سکونت اصلی)
        public int? PermanentProvinceId { get; set; }
        public int? PermanentDistrictId { get; set; }

        [MaxLength(500)]
        public string? PermanentVillage { get; set; }

        // Current Address (سکونت فعلی)
        public int? CurrentProvinceId { get; set; }
        public int? CurrentDistrictId { get; set; }

        [MaxLength(500)]
        public string? CurrentVillage { get; set; }

        // Status
        public bool Status { get; set; } = true;
        public bool IsWithdrawn { get; set; } = false;

        // Audit Fields
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(50)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        [ForeignKey("PermanentProvinceId")]
        public virtual Location? PermanentProvince { get; set; }

        [ForeignKey("PermanentDistrictId")]
        public virtual Location? PermanentDistrict { get; set; }

        [ForeignKey("CurrentProvinceId")]
        public virtual Location? CurrentProvince { get; set; }

        [ForeignKey("CurrentDistrictId")]
        public virtual Location? CurrentDistrict { get; set; }

        public virtual ICollection<LicenseApplicationGuarantor> Guarantors { get; set; } = new List<LicenseApplicationGuarantor>();
        public virtual LicenseApplicationWithdrawal? Withdrawal { get; set; }
    }
}
