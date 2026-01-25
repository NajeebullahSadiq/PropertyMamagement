using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class VehiclesBuyerDetail
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string GrandFather { get; set; } = null!;

    // Electronic National ID - الیکټرونیکی تذکره
    public string? ElectronicNationalIdNumber { get; set; }

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

    public string? RoleType { get; set; } // "Buyer" or "Authorized Agent (Buyer)"

    public string? AuthorizationLetter { get; set; } // Path to authorization letter file

    public DateTime? RentStartDate { get; set; }

    public DateTime? RentEndDate { get; set; }

    public string? Price { get; set; }

    public string? RoyaltyAmount { get; set; }

    public string? HalfPrice { get; set; }

    public virtual ICollection<Vehiclebuyeraudit> Vehiclebuyeraudits { get; } = new List<Vehiclebuyeraudit>();

    public virtual Location? PaddressDistrict { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual VehiclesPropertyDetail? PropertyDetails { get; set; }

    public virtual Location? TaddressDistrict { get; set; }

    public virtual Location? TaddressProvince { get; set; }
}
