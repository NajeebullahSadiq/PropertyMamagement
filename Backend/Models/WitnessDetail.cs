using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class WitnessDetail
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public double? IndentityCardNumber { get; set; }

    public string? TazkiraType { get; set; }

    public string? TazkiraVolume { get; set; }

    public string? TazkiraPage { get; set; }

    public string? TazkiraNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public int? PropertyDetailsId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? NationalIdCard { get; set; }

    public int? PaddressProvinceId { get; set; }

    public int? PaddressDistrictId { get; set; }

    public string? PaddressVillage { get; set; }

    public string? RelationshipToParties { get; set; }

    public string? WitnessType { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual Location? PaddressDistrict { get; set; }
}
