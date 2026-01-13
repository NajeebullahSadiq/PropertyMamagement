using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// DTO for creating/updating Petition Writer Securities
/// </summary>
public class PetitionWriterSecuritiesData
{
    public int? Id { get; set; }

    // Tab 1: مشخصات عریضه‌نویس
    [Required(ErrorMessage = "نمبر ثبت تعرفه الزامی است")]
    [MaxLength(50)]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم عریضه‌نویس الزامی است")]
    [MaxLength(200)]
    public string PetitionWriterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم پدر عریضه‌نویس الزامی است")]
    [MaxLength(200)]
    public string PetitionWriterFatherName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نمبر جواز عریضه‌نویس الزامی است")]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    // Tab 2: مشخصات سند بهادار عریضه
    [Required(ErrorMessage = "تعداد عریضه الزامی است")]
    [Range(1, int.MaxValue, ErrorMessage = "تعداد عریضه باید بزرگتر از صفر باشد")]
    public int PetitionCount { get; set; }

    [Required(ErrorMessage = "مبلغ پول الزامی است")]
    [Range(0, double.MaxValue, ErrorMessage = "مبلغ پول باید عدد مثبت باشد")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "آویز نمبر بانکی الزامی است")]
    [MaxLength(100)]
    public string BankReceiptNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "آغاز سریال نمبر عریضه الزامی است")]
    [MaxLength(100)]
    public string SerialNumberStart { get; set; } = string.Empty;

    [Required(ErrorMessage = "ختم سریال نمبر عریضه الزامی است")]
    [MaxLength(100)]
    public string SerialNumberEnd { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاریخ توزیع عریضه الزامی است")]
    public DateOnly? DistributionDate { get; set; }
}
