using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.LicenseApplication
{
    /// <summary>
    /// تضمین‌کنندگان - Guarantors for License Application
    /// </summary>
    [Table("LicenseApplicationGuarantors", Schema = "org")]
    public class LicenseApplicationGuarantor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LicenseApplicationId { get; set; }

        // Guarantor Identity
        [Required]
        [MaxLength(200)]
        public string GuarantorName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? GuarantorFatherName { get; set; }

        // Guarantee Type (1=Cash, 2=ShariaDeed, 3=CustomaryDeed)
        [Required]
        public int GuaranteeTypeId { get; set; }

        // Conditional Fields - Cash (پول نقد)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CashAmount { get; set; }

        // Conditional Fields - Sharia Deed (قباله شرعی)
        [MaxLength(100)]
        public string? ShariaDeedNumber { get; set; }

        public DateOnly? ShariaDeedDate { get; set; }

        // Conditional Fields - Customary Deed (قباله عرفی)
        [MaxLength(100)]
        public string? CustomaryDeedSerialNumber { get; set; }

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

        // Audit Fields
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        // Navigation Properties
        [ForeignKey("LicenseApplicationId")]
        public virtual LicenseApplicationEntity? LicenseApplication { get; set; }

        [ForeignKey("PermanentProvinceId")]
        public virtual Location? PermanentProvince { get; set; }

        [ForeignKey("PermanentDistrictId")]
        public virtual Location? PermanentDistrict { get; set; }

        [ForeignKey("CurrentProvinceId")]
        public virtual Location? CurrentProvince { get; set; }

        [ForeignKey("CurrentDistrictId")]
        public virtual Location? CurrentDistrict { get; set; }

        [ForeignKey("GuaranteeTypeId")]
        public virtual GuaranteeType? GuaranteeType { get; set; }
    }
}
