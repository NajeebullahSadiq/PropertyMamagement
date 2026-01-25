using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ViewModels
{
    public class GetPrintType
    {
        // PropertyDetails
        [Key]
        public int Id { get; set; }
        public string? DocumentType { get; set; }
        public string? IssuanceNumber { get; set; }
        public DateTime? IssuanceDate { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int PNumber { get; set; }
        public int PArea { get; set; }
        public int? NumofRooms { get; set; }

        [Column("north")]
        public string? North { get; set; }

        [Column("south")]
        public string? South { get; set; }

        [Column("west")]
        public string? West { get; set; }

        [Column("east")]
        public string? East { get; set; }
        public string? Price { get; set; }
        public string? PriceText { get; set; }
        public string? RoyaltyAmount { get; set; }
        public string? PropertypeType { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Location - Province and District
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? ProvinceDari { get; set; }
        public string? DistrictDari { get; set; }
        public string? Village { get; set; }

        // SellerDetails
        public string? SellerFirstName { get; set; }
        public string? SellerFatherName { get; set; }
        public string? SellerIndentityCardNumber { get; set; }
        public string? SellerVillage { get; set; }

        [Column("TSellerVillage")]
        public string? tSellerVillage { get; set; }
        public string? SellerPhoto { get; set; } // Assuming photo is stored as byte[]

        // Location - SellerProvince and SellerDistrict
        public string? SellerProvince { get; set; }
        public string? SellerDistrict { get; set; }
        public string? SellerProvinceDari { get; set; }
        public string? SellerDistrictDari { get; set; }

        [Column("TSellerProvince")]
        public string? tSellerProvince { get; set; }

        [Column("TSellerDistrict")]
        public string? tSellerDistrict { get; set; }

        [Column("TSellerProvinceDari")]
        public string? tSellerProvinceDari { get; set; }

        [Column("TSellerDistrictDari")]
        public string? tSellerDistrictDari { get; set; }

        // BuyerDetails
        public string? BuyerFirstName { get; set; }
        public string? BuyerFatherName { get; set; }
        public string? BuyerIndentityCardNumber { get; set; }
        public string? BuyerVillage { get; set; }
        public string? BuyerPhoto { get; set; } // Assuming photo is stored as byte[]

        // Location - BuyerProvince and BuyerDistrict
        public string? BuyerProvince { get; set; }
        public string? BuyerDistrict { get; set; }
        public string? BuyerProvinceDari { get; set; }
        public string? BuyerDistrictDari { get; set; }

        [Column("TBuyerProvince")]
        public string? tBuyerProvince { get; set; }

        [Column("TBuyerDistrict")]
        public string? tBuyerDistrict { get; set; }

        [Column("TBuyerProvinceDari")]
        public string? tBuyerProvinceDari { get; set; }

        [Column("TBuyerDistrictDari")]
        public string? tBuyerDistrictDari { get; set; }

        [Column("TBuyerVillage")]
        public string? tBuyerVillage { get; set; }

        // WitnessDetails
        public string? WitnessOneFirstName { get; set; }
        public string? WitnessOneFatherName { get; set; }
        public string? WitnessOneIndentityCardNumber { get; set; }

        public string? WitnessTwoFirstName { get; set; }
        public string? WitnessTwoFatherName { get; set; }
        public string? WitnessTwoIndentityCardNumber { get; set; }

        // PropertyUnitType and TransactionType
        public string? UnitType { get; set; }
        public string? TransactionType { get; set; }

        // Property Images and Documents
        public string? FilePath { get; set; }
        public string? PreviousDocumentsPath { get; set; }
        public string? ExistingDocumentsPath { get; set; }
    }
}
