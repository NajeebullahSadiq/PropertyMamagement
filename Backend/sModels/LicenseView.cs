using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

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

    // Office Address Fields (آدرس دفتر)
    public string? OfficeProvinceName { get; set; }
    public string? OfficeDistrictName { get; set; }
    public string? OfficeVillage { get; set; }

    // Owner Personal Address Fields (آدرس شخصی مالک)
    public string? PersonalProvinceName { get; set; }
    public string? PersonalDistrictName { get; set; }
    public string? PersonalVillage { get; set; }
}
