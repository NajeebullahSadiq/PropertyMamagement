using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAPIBackend.Models.Securities;

namespace WebAPIBackend.Models;

/// <summary>
/// اسناد بهادار رهنمای معاملات - Securities Distribution for Transaction Guides
/// </summary>
public partial class SecuritiesDistribution
{
    public int Id { get; set; }

    // Tab 1: مشخصات رهنمای معاملات
    /// <summary>
    /// نمبر ثبت - Registration Number (unique)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// اسم صاحب امتیاز جواز - License Owner Name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string LicenseOwnerName { get; set; } = string.Empty;

    /// <summary>
    /// اسم پدر صاحب امتیاز جواز - License Owner Father Name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string LicenseOwnerFatherName { get; set; } = string.Empty;

    /// <summary>
    /// نام رهنمای معاملات - Transaction Guide Name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string TransactionGuideName { get; set; } = string.Empty;

    /// <summary>
    /// نمبر جواز - License Number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    // Tab 2: مشخصات اسناد توزیعی - Now handled by Items collection

    // Tab 3: قیمت اسناد بهادار
    /// <summary>
    /// قیمت فی جلد سته (for reference - 4000 Afs)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? PricePerDocument { get; set; }

    /// <summary>
    /// قیمت مجموعی سته‌ها (calculated from items)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? TotalDocumentsPrice { get; set; }

    /// <summary>
    /// قیمت مجموعی اسناد بهادار (total of all items)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? TotalSecuritiesPrice { get; set; }

    // Tab 4: مشخصات آویز تحویلی و تاریخ توزیع
    /// <summary>
    /// آویز نمبر بانکی
    /// </summary>
    [MaxLength(100)]
    public string? BankReceiptNumber { get; set; }

    /// <summary>
    /// تاریخ تحویلی
    /// </summary>
    public DateOnly? DeliveryDate { get; set; }

    /// <summary>
    /// تاریخ توزیع اسناد بهادار
    /// </summary>
    public DateOnly? DistributionDate { get; set; }

    // Audit Fields
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? Status { get; set; }

    // Navigation Properties
    /// <summary>
    /// Collection of document items (سټه ها و کتاب های ثبت)
    /// </summary>
    public virtual ICollection<SecuritiesDistributionItem> Items { get; set; } = new List<SecuritiesDistributionItem>();
}
