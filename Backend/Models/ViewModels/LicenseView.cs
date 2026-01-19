using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class LicenseView
{
    public int? CompanyId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsAppNumber { get; set; }

    public string? Title { get; set; }

    public double? Tin { get; set; }

    public string? FirstName { get; set; }

    public string? FatherName { get; set; }

    public string? GrandFatherName { get; set; }

    public DateOnly? DateofBirth { get; set; }

    public double? IndentityCardNumber { get; set; }

    public string? OwnerPhoto { get; set; }

    public double? LicenseNumber { get; set; }

    public string? OfficeAddress { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    // Permanent Address Fields (آدرس دایمی)
    public string? PermanentProvinceName { get; set; }
    public string? PermanentDistrictName { get; set; }
    public string? PermanentVillage { get; set; }

    // Financial and Administrative Fields (جزئیات مالی و اسناد جواز)
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
}
