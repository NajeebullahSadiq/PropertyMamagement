using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class VehiclesSellerDetail
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string GrandFather { get; set; } = null!;

    public double? IndentityCardNumber { get; set; }

    public string? TazkiraType { get; set; }

    public string? TazkiraVolume { get; set; }

    public string? TazkiraPage { get; set; }

    public string? TazkiraNumber { get; set; }

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

    public string? NationalIdCardPath { get; set; }

    public string? RoleType { get; set; } // "Seller" or "Authorized Agent (Seller)"

    public string? AuthorizationLetter { get; set; } // Path to authorization letter file

    public string? HeirsLetter { get; set; } // Path to heirs letter file
    
    public virtual Location? PaddressDistrict { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual VehiclesPropertyDetail? PropertyDetails { get; set; }

    public virtual Location? TaddressDistrict { get; set; }

    public virtual Location? TaddressProvince { get; set; }

    public virtual ICollection<Vehicleselleraudit> Vehicleselleraudits { get; } = new List<Vehicleselleraudit>();
}
