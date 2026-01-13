using System;
using System.ComponentModel.DataAnnotations;

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

    // Tab 2: مشخصات اسناد توزیعی
    /// <summary>
    /// نوعیت سته - Document Type (1=Property, 2=Vehicle)
    /// </summary>
    public int? DocumentType { get; set; }

    /// <summary>
    /// نوع فرعی سته جایداد - Property Sub Type (1=Sale, 2=BayWafa, 3=Rent, 4=All)
    /// </summary>
    public int? PropertySubType { get; set; }

    /// <summary>
    /// نوع فرعی سته وسایط - Vehicle Sub Type (1=Sale, 2=Exchange)
    /// </summary>
    public int? VehicleSubType { get; set; }

    // Property Document Counts
    /// <summary>
    /// تعداد سته خرید و فروش جایداد
    /// </summary>
    public int? PropertySaleCount { get; set; }

    /// <summary>
    /// سریال نمبر سته خرید و فروش
    /// </summary>
    [MaxLength(100)]
    public string? PropertySaleSerialNumber { get; set; }

    /// <summary>
    /// تعداد سته بیع وفا
    /// </summary>
    public int? BayWafaCount { get; set; }

    /// <summary>
    /// سریال نمبر سته بیع وفا
    /// </summary>
    [MaxLength(100)]
    public string? BayWafaSerialNumber { get; set; }

    /// <summary>
    /// تعداد سته کرایی
    /// </summary>
    public int? RentCount { get; set; }

    /// <summary>
    /// سریال نمبر سته کرایی
    /// </summary>
    [MaxLength(100)]
    public string? RentSerialNumber { get; set; }

    // Vehicle Document Counts
    /// <summary>
    /// تعداد سته خرید و فروش وسایط نقلیه
    /// </summary>
    public int? VehicleSaleCount { get; set; }

    /// <summary>
    /// سریال نمبر سته خرید و فروش وسایط
    /// </summary>
    [MaxLength(100)]
    public string? VehicleSaleSerialNumber { get; set; }

    /// <summary>
    /// تعداد سته تبادله
    /// </summary>
    public int? VehicleExchangeCount { get; set; }

    /// <summary>
    /// سریال نمبر سته تبادله
    /// </summary>
    [MaxLength(100)]
    public string? VehicleExchangeSerialNumber { get; set; }

    // Registration Book
    /// <summary>
    /// نوع کتاب ثبت (1=کتاب ثبت, 2=کتاب ثبت مثنی)
    /// </summary>
    public int? RegistrationBookType { get; set; }

    /// <summary>
    /// تعداد کتاب ثبت
    /// </summary>
    public int? RegistrationBookCount { get; set; }

    /// <summary>
    /// تعداد کتاب ثبت مثنی
    /// </summary>
    public int? DuplicateBookCount { get; set; }

    // Tab 3: قیمت اسناد بهادار
    /// <summary>
    /// قیمت فی جلد سته
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? PricePerDocument { get; set; }

    /// <summary>
    /// قیمت مجموعی سته‌ها
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? TotalDocumentsPrice { get; set; }

    /// <summary>
    /// قیمت کتاب ثبت / کتاب ثبت مثنی
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? RegistrationBookPrice { get; set; }

    /// <summary>
    /// قیمت مجموعی اسناد بهادار
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
}
