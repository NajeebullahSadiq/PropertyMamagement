using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.ViewModels
{
    public class GetPrintType
    {
        // PropertyDetails
        [Key]
        public int Id { get; set; }
        public string doctype { get; set; }
        public int PNumber { get; set; }
        public int PArea { get; set; }
        public int? NumofRooms { get; set; }
        public string North { get; set; }
        public string South { get; set; }
        public string West { get; set; }
        public string East { get; set; }
        public double Price { get; set; }
        public string PriceText { get; set; }
        public double RoyaltyAmount { get; set; }
        public string PropertypeType { get; set; }
        public DateTime CreatedAt { get; set; }

        // Location - Province and District
        public string Province { get; set; }
        public string District { get; set; }
        public string Village { get; set; }

        // SellerDetails
        public string SellerFirstName { get; set; }
        public string SellerFatherName { get; set; }
        public double SellerIndentityCardNumber { get; set; }
        public string SellerVillage { get; set; }
        public string tSellerVillage { get; set; }
        public string SellerPhoto { get; set; } // Assuming photo is stored as byte[]

        // Location - SellerProvince and SellerDistrict
        public string SellerProvince { get; set; }
        public string SellerDistrict { get; set; }
        public string tSellerProvince { get; set; }
        public string tSellerDistrict { get; set; }

        // BuyerDetails
        public string BuyerFirstName { get; set; }
        public string BuyerFatherName { get; set; }
        public double BuyerIndentityCardNumber { get; set; }
        public string BuyerVillage { get; set; }
        public string BuyerPhoto { get; set; } // Assuming photo is stored as byte[]

        // Location - BuyerProvince and BuyerDistrict
        public string BuyerProvince { get; set; }
        public string BuyerDistrict { get; set; }
        public string tBuyerProvince { get; set; }
        public string tBuyerDistrict { get; set; }
        public string tBuyerVillage { get; set; }

        // WitnessDetails
        public string WitnessOneFirstName { get; set; }
        public string WitnessOneFatherName { get; set; }
        public double WitnessOneIndentityCardNumber { get; set; }

        public string WitnessTwoFirstName { get; set; }
        public string WitnessTwoFatherName { get; set; }
        public double WitnessTwoIndentityCardNumber { get; set; }

        // PropertyUnitType and TransactionType
        public string UnitType { get; set; }
        public string TransactionType { get; set; }
    }
}
