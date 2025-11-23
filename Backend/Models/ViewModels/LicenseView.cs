using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class LicenseView
{
    public int? CompanyId { get; set; }

    public string? PhoneNumber { get; set; }

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
}
