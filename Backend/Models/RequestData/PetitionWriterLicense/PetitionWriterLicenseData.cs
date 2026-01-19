using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData.PetitionWriterLicense
{
    /// <summary>
    /// DTO for Petition Writer License API requests
    /// </summary>
    public class PetitionWriterLicenseData
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "نمبر جواز الزامی است")]
        [MaxLength(50, ErrorMessage = "نمبر جواز نباید بیشتر از ۵۰ حرف باشد")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام متقاضی الزامی است")]
        [MaxLength(200, ErrorMessage = "نام متقاضی نباید بیشتر از ۲۰۰ حرف باشد")]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ApplicantFatherName { get; set; }

        [MaxLength(200)]
        public string? ApplicantGrandFatherName { get; set; }

        [Required(ErrorMessage = "نمبر تذکره الکترونیکی الزامی است")]
        [MaxLength(50)]
        public string ElectronicIdNumber { get; set; } = string.Empty;

        // Permanent Address
        public int? PermanentProvinceId { get; set; }
        public int? PermanentDistrictId { get; set; }

        [MaxLength(500)]
        public string? PermanentVillage { get; set; }

        // Current Address
        public int? CurrentProvinceId { get; set; }
        public int? CurrentDistrictId { get; set; }

        [MaxLength(500)]
        public string? CurrentVillage { get; set; }

        [MaxLength(500)]
        public string? ActivityLocation { get; set; }

        [MaxLength(500)]
        public string? PicturePath { get; set; }

        // Financial Info
        [MaxLength(100)]
        public string? BankReceiptNumber { get; set; }

        public string? BankReceiptDate { get; set; }

        [MaxLength(50)]
        public string? LicenseType { get; set; }

        public string? LicenseIssueDate { get; set; }

        public string? LicenseExpiryDate { get; set; }

        // Status
        public int? LicenseStatus { get; set; }

        public string? CancellationDate { get; set; }

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Relocation API requests
    /// </summary>
    public class PetitionWriterRelocationData
    {
        public int? Id { get; set; }
        public int? PetitionWriterLicenseId { get; set; }

        [Required(ErrorMessage = "محل فعالیت جدید الزامی است")]
        [MaxLength(500)]
        public string NewActivityLocation { get; set; } = string.Empty;

        public string? RelocationDate { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Status Update
    /// </summary>
    public class PetitionWriterLicenseStatusData
    {
        [Required]
        public int LicenseStatus { get; set; }

        public string? CancellationDate { get; set; }

        public string? CalendarType { get; set; }
    }
}
