using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models;

/// <summary>
/// کنټرول ورودی و خروجی اسناد بهادار - Securities Inbound & Outbound Control
/// </summary>
public partial class SecuritiesControl
{
    public int Id { get; set; }

    // Tab 1: معلومات عمومی و ثبت پیشنهاد (General Information & Proposal Registration)
    /// <summary>
    /// شماره مسلسل - Auto-generated unique system identifier
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// نوع سند بهادار - Security Document Type (1=ستههای رهنمای معاملات, 2=کتاب ثبت معاملات, 3=عرایض مطبوع)
    /// </summary>
    [Required]
    public int SecurityDocumentType { get; set; }

    /// <summary>
    /// نمبر پیشنهاد - Proposal Number
    /// </summary>
    [MaxLength(100)]
    public string? ProposalNumber { get; set; }

    /// <summary>
    /// تاریخ پیشنهاد - Proposal Date
    /// </summary>
    public DateOnly? ProposalDate { get; set; }

    /// <summary>
    /// نمبر تکت توزیع - Distribution Ticket Number
    /// </summary>
    [MaxLength(100)]
    public string? DistributionTicketNumber { get; set; }

    /// <summary>
    /// تاریخ تسلیمی - Delivery Date
    /// </summary>
    public DateOnly? DeliveryDate { get; set; }

    // Tab 2: مشخصات اسناد بهادار و تعداد آنها (Securities Details & Quantity Control)
    /// <summary>
    /// انواع اسناد بهادار - Securities Type (1-11 based on dropdown options)
    /// </summary>
    public int? SecuritiesType { get; set; }

    // Single Type Options Fields
    /// <summary>
    /// تعداد ستههای خرید و فروش جایداد
    /// </summary>
    public int? PropertySaleCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر ستههای خرید و فروش جایداد
    /// </summary>
    [MaxLength(100)]
    public string? PropertySaleSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر ستههای خرید و فروش جایداد
    /// </summary>
    [MaxLength(100)]
    public string? PropertySaleSerialEnd { get; set; }

    /// <summary>
    /// تعداد ستههای بیع وفا
    /// </summary>
    public int? BayWafaCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر ستههای بیع وفا
    /// </summary>
    [MaxLength(100)]
    public string? BayWafaSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر ستههای بیع وفا
    /// </summary>
    [MaxLength(100)]
    public string? BayWafaSerialEnd { get; set; }

    /// <summary>
    /// تعداد ستههای کرایی
    /// </summary>
    public int? RentCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر ستههای کرایی
    /// </summary>
    [MaxLength(100)]
    public string? RentSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر ستههای کرایی
    /// </summary>
    [MaxLength(100)]
    public string? RentSerialEnd { get; set; }

    /// <summary>
    /// تعداد ستههای خرید و فروش وسایط نقلیه
    /// </summary>
    public int? VehicleSaleCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر ستههای خرید و فروش وسایط نقلیه
    /// </summary>
    [MaxLength(100)]
    public string? VehicleSaleSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر ستههای خرید و فروش وسایط نقلیه
    /// </summary>
    [MaxLength(100)]
    public string? VehicleSaleSerialEnd { get; set; }

    /// <summary>
    /// تعداد ستههای تبادله
    /// </summary>
    public int? ExchangeCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر ستههای تبادله
    /// </summary>
    [MaxLength(100)]
    public string? ExchangeSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر ستههای تبادله
    /// </summary>
    [MaxLength(100)]
    public string? ExchangeSerialEnd { get; set; }

    /// <summary>
    /// تعداد کتاب ثبت معاملات
    /// </summary>
    public int? RegistrationBookCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر کتاب ثبت معاملات
    /// </summary>
    [MaxLength(100)]
    public string? RegistrationBookSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر کتاب ثبت معاملات
    /// </summary>
    [MaxLength(100)]
    public string? RegistrationBookSerialEnd { get; set; }

    /// <summary>
    /// تعداد عرایض مطبوع
    /// </summary>
    public int? PrintedPetitionCount { get; set; }

    /// <summary>
    /// آغاز سریال نمبر عرایض مطبوع
    /// </summary>
    [MaxLength(100)]
    public string? PrintedPetitionSerialStart { get; set; }

    /// <summary>
    /// ختم سریال نمبر عرایض مطبوع
    /// </summary>
    [MaxLength(100)]
    public string? PrintedPetitionSerialEnd { get; set; }

    // Tab 3: ثبت و کنترول توزیع اسناد (Distribution Registration & Control)
    /// <summary>
    /// نمبر ثبت آغاز توزیع
    /// </summary>
    [MaxLength(100)]
    public string? DistributionStartNumber { get; set; }

    /// <summary>
    /// نمبر ثبت ختم توزیع
    /// </summary>
    [MaxLength(100)]
    public string? DistributionEndNumber { get; set; }

    /// <summary>
    /// تعداد افرادی که اسناد بر آنان توزیع شده است
    /// </summary>
    public int? DistributedPersonsCount { get; set; }

    // Tab 4: ملاحظات و توضیحات (Remarks & Exceptions)
    /// <summary>
    /// ملاحظه - Remarks
    /// </summary>
    [MaxLength(2000)]
    public string? Remarks { get; set; }

    // Audit Fields
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool? Status { get; set; }
}
