using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;
using WebAPIBackend.Models.Common;

namespace WebAPIBackend.Models;

public partial class LicenseDetail : IProvinceEntity
{
    public int Id { get; set; }

    public string? LicenseNumber { get; set; }

    /// <summary>
    /// Province where the license is issued (for province-specific numbering)
    /// Format: PROVINCE_CODE-SEQUENTIAL_NUMBER (e.g., KBL-0001, KHR-0234)
    /// </summary>
    public int? ProvinceId { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public string? TransferLocation { get; set; }

    public string? ActivityLocation { get; set; }

    public string? OfficeAddress { get; set; }

    public int? CompanyId { get; set; }

    public string? DocPath { get; set; }

    public string? LicenseType { get; set; }

    /// <summary>
    /// License Category (نوعیت جواز): جدید (New), تجدید (Renewal), مثنی (Duplicate)
    /// </summary>
    public string? LicenseCategory { get; set; }

    /// <summary>
    /// دور تجدید - Renewal Round (only applicable when LicenseCategory is تجدید)
    /// </summary>
    public int? RenewalRound { get; set; }

    /// <summary>
    /// مبلغ حق‌الامتیاز - Royalty/License Fee Amount
    /// </summary>
    public decimal? RoyaltyAmount { get; set; }

    /// <summary>
    /// تاریخ حق‌الامتیاز - Royalty/License Fee Date
    /// </summary>
    public DateOnly? RoyaltyDate { get; set; }

    public string? TariffNumber { get; set; }

    /// <summary>
    /// مبلغ جریمه - Penalty Amount
    /// </summary>
    public decimal? PenaltyAmount { get; set; }

    /// <summary>
    /// تاریخ جریمه - Penalty Date
    /// </summary>
    public DateOnly? PenaltyDate { get; set; }

    /// <summary>
    /// مکتوب قوای بشری - HR Letter Reference Number
    /// </summary>
    public string? HrLetter { get; set; }

    /// <summary>
    /// تاریخ مکتوب قوای بشری - HR Letter Date
    /// </summary>
    public DateOnly? HrLetterDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    /// <summary>
    /// Indicates whether all required fields for license completion are filled
    /// Used to control print functionality
    /// </summary>
    public bool IsComplete { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual Location? Province { get; set; }

    public virtual ICollection<Licenseaudit> Licenseaudits { get; } = new List<Licenseaudit>();
}
