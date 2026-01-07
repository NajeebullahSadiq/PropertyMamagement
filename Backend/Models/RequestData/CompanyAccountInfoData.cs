using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// DTO for Company Account/Financial Information (مالیه) API requests
/// </summary>
public class CompanyAccountInfoData
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Company ID is required")]
    public int CompanyId { get; set; }

    /// <summary>
    /// Account settlement information / نمرمكتوب تصفيه معلومات
    /// </summary>
    [MaxLength(500, ErrorMessage = "Settlement info cannot exceed 500 characters")]
    public string? SettlementInfo { get; set; }

    /// <summary>
    /// Tax payment amount / تحويل ماليات (مبلغ)
    /// </summary>
    [Required(ErrorMessage = "Tax payment amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Tax payment amount must be non-negative")]
    public decimal TaxPaymentAmount { get; set; }

    /// <summary>
    /// Settlement year / سال تصفيه مالية
    /// </summary>
    [Range(1300, 2100, ErrorMessage = "Settlement year must be between 1300 and 2100")]
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
}
