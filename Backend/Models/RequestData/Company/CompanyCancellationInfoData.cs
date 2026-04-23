using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// DTO for Company License Cancellation/Revocation (فسخ / لغوه) API requests
/// </summary>
public class CompanyCancellationInfoData
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Company ID is required")]
    public int CompanyId { get; set; }

    /// <summary>
    /// Cancellation Type / نوعیت فسخ یا لغوه (فسخ or لغوه)
    /// </summary>
    [MaxLength(20, ErrorMessage = "Cancellation type cannot exceed 20 characters")]
    public string? CancellationType { get; set; }

    /// <summary>
    /// License Cancellation Letter Number / نمبر مکتوب فسخ جواز
    /// </summary>
    [MaxLength(100, ErrorMessage = "License cancellation letter number cannot exceed 100 characters")]
    public string? LicenseCancellationLetterNumber { get; set; }

    /// <summary>
    /// Revenue Cancellation Letter Number / نمبر مکتوب فسخ عواید
    /// </summary>
    [MaxLength(100, ErrorMessage = "Revenue cancellation letter number cannot exceed 100 characters")]
    public string? RevenueCancellationLetterNumber { get; set; }

    /// <summary>
    /// License Cancellation Letter Date / تاریخ مکتوب فسخ جواز
    /// </summary>
    public DateOnly? LicenseCancellationLetterDate { get; set; }

    /// <summary>
    /// Revocation Letter Number / نمبر مکتوب لغوه جواز
    /// </summary>
    [MaxLength(100, ErrorMessage = "Revocation letter number cannot exceed 100 characters")]
    public string? RevocationLetterNumber { get; set; }

    /// <summary>
    /// Revocation Revenue Letter Number / نمبر مکتوب لغوه عواید
    /// </summary>
    [MaxLength(100, ErrorMessage = "Revocation revenue letter number cannot exceed 100 characters")]
    public string? RevocationRevenueLetterNumber { get; set; }

    /// <summary>
    /// Revocation Letter Date / تاریخ مکتوب لغوه جواز
    /// </summary>
    public DateOnly? RevocationLetterDate { get; set; }

    /// <summary>
    /// Remarks/Notes / ملاحظات
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Remarks cannot exceed 1000 characters")]
    public string? Remarks { get; set; }
}
