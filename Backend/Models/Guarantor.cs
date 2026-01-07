using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class Guarantor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string? GrandFatherName { get; set; }

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

    // Guarantee Information (merged from Gaurantee entity)
    public int? GuaranteeTypeId { get; set; }

    public long? PropertyDocumentNumber { get; set; }

    public DateOnly? PropertyDocumentDate { get; set; }

    public string? SenderMaktobNumber { get; set; }

    public DateOnly? SenderMaktobDate { get; set; }

    public long? AnswerdMaktobNumber { get; set; }

    public DateOnly? AnswerdMaktobDate { get; set; }

    public DateOnly? DateofGuarantee { get; set; }

    public long? GuaranteeDocNumber { get; set; }

    public DateOnly? GuaranteeDate { get; set; }

    public string? GuaranteeDocPath { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<Guarantorsaudit> Guarantorsaudits { get; } = new List<Guarantorsaudit>();

    public virtual IdentityCardType? IdentityCardType { get; set; }

    public virtual GuaranteeType? GuaranteeType { get; set; }

    public virtual Location? PaddressDistrict { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual Location? TaddressDistrict { get; set; }

    public virtual Location? TaddressProvince { get; set; }
}
