using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class BuyerDetail
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string GrandFather { get; set; } = null!;

    public double? IndentityCardNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public int? PaddressProvinceId { get; set; }

    public int? PaddressDistrictId { get; set; }

    public string? PaddressVillage { get; set; }

    public int? TaddressProvinceId { get; set; }

    public int? TaddressDistrictId { get; set; }

    public string? TaddressVillage { get; set; }

    public int? PropertyDetailsId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? Photo { get; set; }

    public int? PropertyTypeId { get; set; }

    public double? Price { get; set; }

    public string? PriceText { get; set; }

    public double? RoyaltyAmount { get; set; }

    public double? HalfPrice { get; set; }

    public virtual Location? PaddressDistrict { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }

    public virtual ICollection<Propertybuyeraudit> Propertybuyeraudits { get; } = new List<Propertybuyeraudit>();

    public virtual Location? TaddressDistrict { get; set; }

    public virtual Location? TaddressProvince { get; set; }
}
