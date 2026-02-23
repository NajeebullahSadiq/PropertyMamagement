using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData.LicenseApplication
{
    /// <summary>
    /// DTO for License Application API requests
    /// </summary>
    public class LicenseApplicationData
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "تاریخ درخواست الزامی است")]
        public string? RequestDate { get; set; }

        [Required(ErrorMessage = "نمبر عریضه الزامی است")]
        [MaxLength(50, ErrorMessage = "نمبر مسلسل نباید بیشتر از ۵۰ حرف باشد")]
        public string RequestSerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام متقاضی الزامی است")]
        [MaxLength(200, ErrorMessage = "نام متقاضی نباید بیشتر از ۲۰۰ حرف باشد")]
        public string ApplicantName { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "نام پدر متقاضی نباید بیشتر از ۲۰۰ حرف باشد")]
        public string? ApplicantFatherName { get; set; }

        [MaxLength(200, ErrorMessage = "نام پدرکلان متقاضی نباید بیشتر از ۲۰۰ حرف باشد")]
        public string? ApplicantGrandfatherName { get; set; }

        [MaxLength(50, ErrorMessage = "نمبر الکترونیکی نباید بیشتر از ۵۰ حرف باشد")]
        public string? ApplicantElectronicNumber { get; set; }

        [Required(ErrorMessage = "نام پیشنهادی رهنما الزامی است")]
        [MaxLength(200, ErrorMessage = "نام پیشنهادی رهنما نباید بیشتر از ۲۰۰ حرف باشد")]
        public string ProposedGuideName { get; set; } = string.Empty;

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

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Guarantor API requests
    /// </summary>
    public class LicenseApplicationGuarantorData
    {
        public int? Id { get; set; }
        public int? LicenseApplicationId { get; set; }

        [Required(ErrorMessage = "شهرت تضمین‌کننده الزامی است")]
        [MaxLength(200)]
        public string GuarantorName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? GuarantorFatherName { get; set; }

        [Required(ErrorMessage = "نوعیت ضمانت الزامی است")]
        public int GuaranteeTypeId { get; set; }

        // Conditional Fields
        public decimal? CashAmount { get; set; }

        [MaxLength(100)]
        public string? ShariaDeedNumber { get; set; }

        public string? ShariaDeedDate { get; set; }

        [MaxLength(100)]
        public string? CustomaryDeedSerialNumber { get; set; }

        // Addresses
        public int? PermanentProvinceId { get; set; }
        public int? PermanentDistrictId { get; set; }

        [MaxLength(500)]
        public string? PermanentVillage { get; set; }

        public int? CurrentProvinceId { get; set; }
        public int? CurrentDistrictId { get; set; }

        [MaxLength(500)]
        public string? CurrentVillage { get; set; }

        public string? CalendarType { get; set; }
    }

    /// <summary>
    /// DTO for Withdrawal API requests
    /// </summary>
    public class LicenseApplicationWithdrawalData
    {
        public int? Id { get; set; }
        public int? LicenseApplicationId { get; set; }

        [Required(ErrorMessage = "علت انصراف الزامی است")]
        [MaxLength(1000)]
        public string WithdrawalReason { get; set; } = string.Empty;

        public string? WithdrawalDate { get; set; }

        public string? CalendarType { get; set; }
    }
}
