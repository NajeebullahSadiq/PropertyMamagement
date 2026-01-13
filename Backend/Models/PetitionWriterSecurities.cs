using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models;

/// <summary>
/// سند بهادار عریضه‌ نویسان - Securities for Petition Writers
/// </summary>
public partial class PetitionWriterSecurities
{
    public int Id { get; set; }

    // Tab 1: مشخصات عریضه‌نویس
    /// <summary>
    /// نمبر ثبت تعرفه - Registration Number (unique)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// اسم عریضه‌نویس - Petition Writer Name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string PetitionWriterName { get; set; } = string.Empty;

    /// <summary>
    /// اسم پدر عریضه‌نویس - Petition Writer Father Name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string PetitionWriterFatherName { get; set; } = string.Empty;

    /// <summary>
    /// نمبر جواز عریضه‌نویس - Petition Writer License Number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    // Tab 2: مشخصات سند بهادار عریضه
    /// <summary>
    /// تعداد عریضه - Petition Count
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int PetitionCount { get; set; }

    /// <summary>
    /// مبلغ پول - Amount
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    /// <summary>
    /// آویز نمبر بانکی - Bank Receipt Number
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string BankReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// آغاز سریال نمبر عریضه - Serial Number Start
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SerialNumberStart { get; set; } = string.Empty;

    /// <summary>
    /// ختم سریال نمبر عریضه - Serial Number End
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SerialNumberEnd { get; set; } = string.Empty;

    /// <summary>
    /// تاریخ توزیع عریضه - Distribution Date
    /// </summary>
    [Required]
    public DateOnly DistributionDate { get; set; }

    // Audit Fields
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? Status { get; set; }
}
