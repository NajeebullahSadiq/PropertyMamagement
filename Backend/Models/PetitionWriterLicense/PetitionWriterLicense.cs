using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPIBackend.Models;

namespace WebAPIBackend.Models.PetitionWriterLicense
{
    /// <summary>
    /// ثبت جواز عریضه‌نویسان
    /// Petition Writer License Registration
    /// </summary>
    [Table("PetitionWriterLicenses", Schema = "org")]
    public class PetitionWriterLicenseEntity
    {
        [Key]
        public int Id { get; set; }

        // Tab 1: مشخصات عریضه‌نویس
        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ApplicantFatherName { get; set; }

        [MaxLength(200)]
        public string? ApplicantGrandFatherName { get; set; }

        // Identity Card Information
        // Electronic National ID - الیکټرونیکی تذکره
        [Required]
        [MaxLength(50)]
        public string ElectronicIdNumber { get; set; } = string.Empty;

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

        // Activity Location (محل فعالیت)
        [MaxLength(500)]
        public string? ActivityLocation { get; set; }

        // Picture Path (عکس)
        [MaxLength(500)]
        public string? PicturePath { get; set; }

        // Tab 2: ثبت مالیه و مشخصات جواز
        [MaxLength(100)]
        public string? BankReceiptNumber { get; set; }

        public DateOnly? BankReceiptDate { get; set; }

        [MaxLength(50)]
        public string? LicenseType { get; set; }

        public DateOnly? LicenseIssueDate { get; set; }

        public DateOnly? LicenseExpiryDate { get; set; }

        // Tab 3: لغو / انصراف
        // License Status (1 = Active, 2 = Cancelled, 3 = Withdrawn)
        public int LicenseStatus { get; set; } = 1;

        public DateOnly? CancellationDate { get; set; }

        // Soft delete
        public bool Status { get; set; } = true;

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

        public virtual ICollection<PetitionWriterRelocation> Relocations { get; set; } = new List<PetitionWriterRelocation>();
    }

    /// <summary>
    /// Relocation History for Petition Writer License
    /// </summary>
    [Table("PetitionWriterRelocations", Schema = "org")]
    public class PetitionWriterRelocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PetitionWriterLicenseId { get; set; }

        [Required]
        [MaxLength(500)]
        public string NewActivityLocation { get; set; } = string.Empty;

        public DateOnly? RelocationDate { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        // Audit Fields
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        // Navigation Property
        [ForeignKey("PetitionWriterLicenseId")]
        public virtual PetitionWriterLicenseEntity? PetitionWriterLicense { get; set; }
    }
}
