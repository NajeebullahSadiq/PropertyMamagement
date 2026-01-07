using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models;

/// <summary>
/// Company License Cancellation/Revocation Information (فسخ / لغوه)
/// Stores cancellation details for company licenses
/// </summary>
public partial class CompanyCancellationInfo
{
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to CompanyDetails
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// License Cancellation Letter Number / نمبر مکتوب فسخ جواز
    /// </summary>
    [MaxLength(100)]
    public string? LicenseCancellationLetterNumber { get; set; }

    /// <summary>
    /// Revenue Cancellation Letter Number / نمبر مکتوب فسخ عواید
    /// </summary>
    [MaxLength(100)]
    public string? RevenueCancellationLetterNumber { get; set; }

    /// <summary>
    /// License Cancellation Letter Date / تاریخ مکتوب فسخ جواز
    /// </summary>
    public DateOnly? LicenseCancellationLetterDate { get; set; }

    /// <summary>
    /// Remarks/Notes / ملاحظات
    /// </summary>
    [MaxLength(1000)]
    public string? Remarks { get; set; }

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
