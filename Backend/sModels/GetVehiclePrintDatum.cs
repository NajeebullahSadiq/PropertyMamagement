using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class GetVehiclePrintDatum
{
    public int? Id { get; set; }

    public int? PermitNo { get; set; }

    public int? PilateNo { get; set; }

    public string? TypeOfVehicle { get; set; }

    public string? Model { get; set; }

    public int? EnginNo { get; set; }

    public int? ShasiNo { get; set; }

    public string? Color { get; set; }

    public double? Price { get; set; }

    public string? PriceText { get; set; }

    public double? RoyaltyAmount { get; set; }

    public string? Description { get; set; }

    public string? SellerFirstName { get; set; }

    public string? SellerFatherName { get; set; }

    public double? SellerIndentityCardNumber { get; set; }

    public string? SellerVillage { get; set; }

    public string? TSellerVillage { get; set; }

    public string? SellerPhoto { get; set; }

    public string? SellerProvince { get; set; }

    public string? SellerDistrict { get; set; }

    public string? TSellerProvince { get; set; }

    public string? TSellerDistrict { get; set; }

    public string? BuyerFirstName { get; set; }

    public string? BuyerFatherName { get; set; }

    public double? BuyerIndentityCardNumber { get; set; }

    public string? BuyerProvince { get; set; }

    public string? BuyerDistrict { get; set; }

    public string? TBuyerProvince { get; set; }

    public string? TBuyerDistrict { get; set; }

    public string? BuyerVillage { get; set; }

    public string? TBuyerVillage { get; set; }

    public string? BuyerPhoto { get; set; }

    public string? WitnessOneFirstName { get; set; }

    public string? WitnessOneFatherName { get; set; }

    public double? WitnessOneIndentityCardNumber { get; set; }

    public string? WitnessTwoFirstName { get; set; }

    public string? WitnessTwoFatherName { get; set; }

    public double? WitnessTwoIndentityCardNumber { get; set; }

    public DateTime? CreatedAt { get; set; }
}
