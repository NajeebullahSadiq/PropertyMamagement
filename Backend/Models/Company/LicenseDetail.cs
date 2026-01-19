using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class LicenseDetail
{
    public int Id { get; set; }

    public double LicenseNumber { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public int? AreaId { get; set; }

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

    public virtual Area? Area { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<Licenseaudit> Licenseaudits { get; } = new List<Licenseaudit>();
}
