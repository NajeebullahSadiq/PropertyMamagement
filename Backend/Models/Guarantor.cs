using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class Guarantor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public int? IdentityCardTypeId { get; set; }

    public int? CompanyId { get; set; }

    public double? IndentityCardNumber { get; set; }

    public int? Jild { get; set; }

    public int? Safha { get; set; }

    public int? SabtNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public string? PothoPath { get; set; }

    public int? PaddressProvinceId { get; set; }

    public int? PaddressDistrictId { get; set; }

    public string? PaddressVillage { get; set; }

    public int? TaddressProvinceId { get; set; }

    public int? TaddressDistrictId { get; set; }

    public string? TaddressVillage { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<Guarantorsaudit> Guarantorsaudits { get; } = new List<Guarantorsaudit>();

    public virtual IdentityCardType? IdentityCardType { get; set; }

    public virtual Location? PaddressDistrict { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual Location? TaddressDistrict { get; set; }

    public virtual Location? TaddressProvince { get; set; }
}
