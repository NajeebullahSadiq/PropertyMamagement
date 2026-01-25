namespace WebAPIBackend.Application.Vehicle.DTOs
{
    public class VehicleListItemDto
    {
        public int Id { get; set; }
        public string? PermitNo { get; set; }
        public string? PilateNo { get; set; }
        public string? TypeOfVehicle { get; set; }
        public string? Model { get; set; }
        public string? EnginNo { get; set; }
        public string? ShasiNo { get; set; }
        public string? Color { get; set; }
        public string? Price { get; set; }
        public string? PriceText { get; set; }
        public string? Des { get; set; }
        public string? RoyaltyAmount { get; set; }
        public string? FilePath { get; set; }
        public string? VehicleHand { get; set; }
        public bool? Iscomplete { get; set; }
        public string? SellerName { get; set; }
        public string? BuyerName { get; set; }
    }

    public class VehicleViewDto
    {
        public int Id { get; set; }
        public string? PermitNo { get; set; }
        public string? PilateNo { get; set; }
        public string? TypeOfVehicle { get; set; }
        public string? Model { get; set; }
        public string? EnginNo { get; set; }
        public string? ShasiNo { get; set; }
        public string? Color { get; set; }
        public string? VehicleHand { get; set; }
        public string? TransactionTypeName { get; set; }
        public string? Price { get; set; }
        public string? PriceText { get; set; }
        public string? RoyaltyAmount { get; set; }
        public string? Des { get; set; }
        public string? FilePath { get; set; }
        public bool? Iscomplete { get; set; }
        public bool? Iseditable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<VehicleSellerDto> Sellers { get; set; } = new();
        public List<VehicleBuyerDto> Buyers { get; set; } = new();
        public List<VehicleWitnessDto> Witnesses { get; set; } = new();
    }

    public class VehicleSellerDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFather { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? TazkiraType { get; set; }
        public string? TazkiraVolume { get; set; }
        public string? TazkiraPage { get; set; }
        public string? TazkiraNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? PermanentProvinceName { get; set; }
        public string? PermanentDistrictName { get; set; }
        public string? PaddressVillage { get; set; }
        public string? TemporaryProvinceName { get; set; }
        public string? TemporaryDistrictName { get; set; }
        public string? TaddressVillage { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class VehicleBuyerDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFather { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? TazkiraType { get; set; }
        public string? TazkiraVolume { get; set; }
        public string? TazkiraPage { get; set; }
        public string? TazkiraNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? PermanentProvinceName { get; set; }
        public string? PermanentDistrictName { get; set; }
        public string? PaddressVillage { get; set; }
        public string? TemporaryProvinceName { get; set; }
        public string? TemporaryDistrictName { get; set; }
        public string? TaddressVillage { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class VehicleWitnessDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? TazkiraType { get; set; }
        public string? TazkiraVolume { get; set; }
        public string? TazkiraPage { get; set; }
        public string? TazkiraNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class VehiclePrintDto
    {
        public int Id { get; set; }
        public string? PermitNo { get; set; }
        public string? PilateNo { get; set; }
        public string? TypeOfVehicle { get; set; }
        public string? Model { get; set; }
        public string? EnginNo { get; set; }
        public string? ShasiNo { get; set; }
        public string? Color { get; set; }
        public string? Price { get; set; }
        public string? PriceText { get; set; }
        public string? RoyaltyAmount { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedAtFormatted { get; set; }
        // Seller info
        public string? SellerFirstName { get; set; }
        public string? SellerFatherName { get; set; }
        public string? SellerIndentityCardNumber { get; set; }
        public string? SellerVillage { get; set; }
        public string? TSellerVillage { get; set; }
        public string? SellerPhoto { get; set; }
        public string? SellerProvince { get; set; }
        public string? SellerDistrict { get; set; }
        public string? TSellerProvince { get; set; }
        public string? TSellerDistrict { get; set; }
        // Buyer info
        public string? BuyerFirstName { get; set; }
        public string? BuyerFatherName { get; set; }
        public string? BuyerIndentityCardNumber { get; set; }
        public string? BuyerVillage { get; set; }
        public string? TBuyerVillage { get; set; }
        public string? BuyerPhoto { get; set; }
        public string? BuyerProvince { get; set; }
        public string? BuyerDistrict { get; set; }
        public string? TBuyerProvince { get; set; }
        public string? TBuyerDistrict { get; set; }
        // Witness info
        public string? WitnessOneFirstName { get; set; }
        public string? WitnessOneFatherName { get; set; }
        public string? WitnessOneIndentityCardNumber { get; set; }
        public string? WitnessTwoFirstName { get; set; }
        public string? WitnessTwoFatherName { get; set; }
        public string? WitnessTwoIndentityCardNumber { get; set; }
    }
}
