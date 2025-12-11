using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class GetPrintType
{
    public int? Id { get; set; }

    public string? Doctype { get; set; }

    public int? Pnumber { get; set; }

    public int? Parea { get; set; }

    public int? NumofRooms { get; set; }

    public string? North { get; set; }

    public string? South { get; set; }

    public string? West { get; set; }

    public string? East { get; set; }

    public double? Price { get; set; }

    public string? PriceText { get; set; }

    public double? RoyaltyAmount { get; set; }

    public string? PropertypeType { get; set; }

    public string? Province { get; set; }

    public string? District { get; set; }

    public string? Village { get; set; }

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

    public string? BuyerVillage { get; set; }

    public string? TBuyerVillage { get; set; }

    public string? BuyerPhoto { get; set; }

    public string? BuyerProvince { get; set; }

    public string? BuyerDistrict { get; set; }

    public string? TBuyerProvince { get; set; }

    public string? TBuyerDistrict { get; set; }

    public string? WitnessOneFirstName { get; set; }

    public string? WitnessOneFatherName { get; set; }

    public double? WitnessOneIndentityCardNumber { get; set; }

    public string? WitnessTwoFirstName { get; set; }

    public string? WitnessTwoFatherName { get; set; }

    public double? WitnessTwoIndentityCardNumber { get; set; }

    public string? UnitType { get; set; }

    public string? TransactionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeedDate { get; set; }
}
