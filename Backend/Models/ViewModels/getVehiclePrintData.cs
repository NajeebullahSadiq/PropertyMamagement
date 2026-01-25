namespace WebAPIBackend.Models.ViewModels
{
    public class getVehiclePrintData
    {
        public int Id { get; set; }
        public int PermitNo { get; set; }
        public int PilateNo { get; set; }
        public string? TypeOfVehicle { get; set; }
        public string? Model { get; set; }
        public int EnginNo { get; set; }
        public int ShasiNo { get; set; }
        public string? Color { get; set; }
        public string? Description { get;set; }
        public string? Price { get; set; }
        public string? PriceText { get; set; }
        public string? RoyaltyAmount { get; set; }
        public string? SellerFirstName { get; set; }
        public string? SellerFatherName { get; set; }
        public string? SellerIndentityCardNumber { get; set; }
        public string? SellerVillage { get; set; }
        public string? tSellerVillage { get; set; }
        public string? SellerPhoto { get; set; }
        public string? SellerProvince { get; set; }
        public string? SellerDistrict { get; set; }
        public string? tSellerProvince { get; set; }
        public string? tSellerDistrict { get; set; }
        public string? BuyerFirstName { get; set; }
        public string? BuyerFatherName { get; set; }
        public string? BuyerIndentityCardNumber { get; set; }
        public string? BuyerVillage { get; set; }
        public string? BuyerProvince { get; set; }
        public string? BuyerDistrict { get; set; }
        public string? tBuyerProvince { get; set; }
        public string? tBuyerDistrict { get; set; }
        public string? tBuyerVillage { get; set; }
        public string? BuyerPhoto { get; set; }
        public string? WitnessOneFirstName { get; set; }
        public string? WitnessOneFatherName { get; set; }
        public string? WitnessOneIndentityCardNumber { get; set; }
        public string? WitnessTwoFirstName { get; set; }
        public string? WitnessTwoFatherName { get; set; }
        public string? WitnessTwoIndentityCardNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
