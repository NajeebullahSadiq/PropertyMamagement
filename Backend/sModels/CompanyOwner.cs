using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class CompanyOwner
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string? GrandFatherName { get; set; }

    public short? EducationLevelId { get; set; }

    public DateOnly? DateofBirth { get; set; }

    public int? IdentityCardTypeId { get; set; }

    public double? IndentityCardNumber { get; set; }

    public string? Jild { get; set; }

    public string? Safha { get; set; }

    public int? CompanyId { get; set; }

    public string? SabtNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public string? PothoPath { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<CompanyOwnerAddress> CompanyOwnerAddresses { get; } = new List<CompanyOwnerAddress>();

    public virtual ICollection<Companyowneraudit> Companyowneraudits { get; } = new List<Companyowneraudit>();

    public virtual EducationLevel? EducationLevel { get; set; }

    public virtual IdentityCardType? IdentityCardType { get; set; }
}
