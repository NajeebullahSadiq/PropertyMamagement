using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models;

/// <summary>
/// Company Account/Financial Information (مالیه)
/// Stores tax settlement and commission data for companies
/// </summary>
public partial class CompanyAccountInfo
{
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to CompanyDetails
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Account settlement information / نمرمكتوب تصفيه معلومات
    /// </summary>
    [MaxLength(500)]
    public string? SettlementInfo { get; set; }

    /// <summary>
    /// Tax payment amount / تحويل ماليات (مبلغ)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Tax payment amount must be non-negative")]
    public decimal TaxPaymentAmount { get; set; }

    /// <summary>
    /// Settlement year / سال تصفيه مالية
    /// </summary>
    public int? SettlementYear { get; set; }

    /// <summary>
    /// Tax payment date / تاريخ تحويل ماليات
    /// </summary>
    public DateOnly? TaxPaymentDate { get; set; }

    /// <summary>
    /// Transaction count / تعدادی معامله
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Transaction count must be non-negative")]
    public int? TransactionCount { get; set; }

    /// <summary>
    /// Company commission / كمیشن رهنما
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Company commission must be non-negative")]
    public decimal? CompanyCommission { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// User who created the record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Record status (active/inactive)
    /// </summary>
    public bool? Status { get; set; }

    // Navigation property
    public virtual CompanyDetail? Company { get; set; }
}
