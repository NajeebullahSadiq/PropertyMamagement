namespace WebAPIBackend.Application.Property.DTOs
{
    public class PropertyListItemDto
    {
        public int Id { get; set; }
        public int? Pnumber { get; set; }
        public double? Parea { get; set; }
        public int? PunitTypeId { get; set; }
        public int? NumofFloor { get; set; }
        public int? NumofRooms { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? TransactionTypeId { get; set; }
        public double? Price { get; set; }
        public string? PriceText { get; set; }
        public string? Des { get; set; }
        public double? RoyaltyAmount { get; set; }
        public string? FilePath { get; set; }
        public string? PropertyTypeText { get; set; }
        public string? UnitTypeText { get; set; }
        public string? TransactionTypeText { get; set; }
        public bool? Iscomplete { get; set; }
        public string? SellerName { get; set; }
        public string? BuyerName { get; set; }
        public string? SellerIndentityCardNumber { get; set; }
        public string? BuyerIndentityCardNumber { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class PropertyViewDto
    {
        public int Id { get; set; }
        public int? Pnumber { get; set; }
        public double? Parea { get; set; }
        public int? PunitTypeId { get; set; }
        public string? UnitTypeName { get; set; }
        public int? NumofFloor { get; set; }
        public int? NumofRooms { get; set; }
        public int? PropertyTypeId { get; set; }
        public string? CustomPropertyType { get; set; }
        public string? PropertyTypeName { get; set; }
        public string? PropertyTypeText { get; set; }
        public int? TransactionTypeId { get; set; }
        public string? TransactionTypeName { get; set; }
        public double? Price { get; set; }
        public string? PriceText { get; set; }
        public double? RoyaltyAmount { get; set; }
        public string? Des { get; set; }
        public bool? Iscomplete { get; set; }
        public bool? Iseditable { get; set; }
        public string? West { get; set; }
        public string? East { get; set; }
        public string? North { get; set; }
        public string? South { get; set; }
        public string? DocumentType { get; set; }
        public string? IssuanceNumber { get; set; }
        public DateTime? IssuanceDate { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? FilePath { get; set; }
        public string? PreviousDocumentsPath { get; set; }
        public string? ExistingDocumentsPath { get; set; }
        public PropertyAddressDto? Address { get; set; }
        public List<SellerDetailDto> Sellers { get; set; } = new();
        public List<BuyerDetailDto> Buyers { get; set; } = new();
        public List<WitnessDetailDto> Witnesses { get; set; } = new();
    }

    public class PropertyAddressDto
    {
        public int Id { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string? Village { get; set; }
        public string? ProvinceDari { get; set; }
        public string? DistrictDari { get; set; }
    }

    public class SellerDetailDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFather { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? TazkiraType { get; set; }
        public string? TazkiraVolume { get; set; }
        public string? TazkiraPage { get; set; }
        public string? TazkiraNumber { get; set; }
        public string? RoleType { get; set; }
        public string? TaxIdentificationNumber { get; set; }
        public string? AdditionalDetails { get; set; }
        public string? Photo { get; set; }
        public string? NationalIdCard { get; set; }
        public string? AuthorizationLetter { get; set; }
        public string? HeirsLetter { get; set; }
        public int? PaddressProvinceId { get; set; }
        public int? PaddressDistrictId { get; set; }
        public string? PaddressVillage { get; set; }
        public string? PaddressProvinceDari { get; set; }
        public string? PaddressDistrictDari { get; set; }
        public int? TaddressProvinceId { get; set; }
        public int? TaddressDistrictId { get; set; }
        public string? TaddressVillage { get; set; }
        public string? TaddressProvinceDari { get; set; }
        public string? TaddressDistrictDari { get; set; }
    }

    public class BuyerDetailDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFather { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? TazkiraType { get; set; }
        public string? TazkiraVolume { get; set; }
        public string? TazkiraPage { get; set; }
        public string? TazkiraNumber { get; set; }
        public string? RoleType { get; set; }
        public string? TaxIdentificationNumber { get; set; }
        public string? AdditionalDetails { get; set; }
        public string? Photo { get; set; }
        public string? NationalIdCard { get; set; }
        public string? AuthorizationLetter { get; set; }
        public double? Price { get; set; }
        public string? PriceText { get; set; }
        public double? RoyaltyAmount { get; set; }
        public double? HalfPrice { get; set; }
        public string? TransactionType { get; set; }
        public string? TransactionTypeDescription { get; set; }
        public DateOnly? RentStartDate { get; set; }
        public DateOnly? RentEndDate { get; set; }
        public int? PaddressProvinceId { get; set; }
        public int? PaddressDistrictId { get; set; }
        public string? PaddressVillage { get; set; }
        public string? PaddressProvinceDari { get; set; }
        public string? PaddressDistrictDari { get; set; }
        public int? TaddressProvinceId { get; set; }
        public int? TaddressDistrictId { get; set; }
        public string? TaddressVillage { get; set; }
        public string? TaddressProvinceDari { get; set; }
        public string? TaddressDistrictDari { get; set; }
    }

    public class WitnessDetailDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? TazkiraType { get; set; }
        public string? TazkiraVolume { get; set; }
        public string? TazkiraPage { get; set; }
        public string? TazkiraNumber { get; set; }
        public string? NationalIdCard { get; set; }
    }

    public class PropertyPrintDto
    {
        public int Id { get; set; }
        public string? DocumentType { get; set; }
        public string? IssuanceNumber { get; set; }
        public DateTime? IssuanceDate { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? PNumber { get; set; }
        public double? PArea { get; set; }
        public int? NumofRooms { get; set; }
        public string? North { get; set; }
        public string? South { get; set; }
        public string? West { get; set; }
        public string? East { get; set; }
        public double? Price { get; set; }
        public string? PriceText { get; set; }
        public double? RoyaltyAmount { get; set; }
        public string? PropertypeType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedAtFormatted { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? ProvinceDari { get; set; }
        public string? DistrictDari { get; set; }
        public string? Village { get; set; }
        public string? UnitType { get; set; }
        public string? TransactionType { get; set; }
        public string? FilePath { get; set; }
        public string? PreviousDocumentsPath { get; set; }
        public string? ExistingDocumentsPath { get; set; }
        // Seller info
        public string? SellerFirstName { get; set; }
        public string? SellerFatherName { get; set; }
        public string? SellerIndentityCardNumber { get; set; }
        public string? SellerVillage { get; set; }
        public string? TSellerVillage { get; set; }
        public string? SellerPhoto { get; set; }
        public string? SellerProvince { get; set; }
        public string? SellerDistrict { get; set; }
        public string? SellerProvinceDari { get; set; }
        public string? SellerDistrictDari { get; set; }
        public string? TSellerProvince { get; set; }
        public string? TSellerDistrict { get; set; }
        public string? TSellerProvinceDari { get; set; }
        public string? TSellerDistrictDari { get; set; }
        // Buyer info
        public string? BuyerFirstName { get; set; }
        public string? BuyerFatherName { get; set; }
        public string? BuyerIndentityCardNumber { get; set; }
        public string? BuyerVillage { get; set; }
        public string? TBuyerVillage { get; set; }
        public string? BuyerPhoto { get; set; }
        public string? BuyerProvince { get; set; }
        public string? BuyerDistrict { get; set; }
        public string? BuyerProvinceDari { get; set; }
        public string? BuyerDistrictDari { get; set; }
        public string? TBuyerProvince { get; set; }
        public string? TBuyerDistrict { get; set; }
        public string? TBuyerProvinceDari { get; set; }
        public string? TBuyerDistrictDari { get; set; }
        // Witness info
        public string? WitnessOneFirstName { get; set; }
        public string? WitnessOneFatherName { get; set; }
        public string? WitnessOneIndentityCardNumber { get; set; }
        public string? WitnessTwoFirstName { get; set; }
        public string? WitnessTwoFatherName { get; set; }
        public string? WitnessTwoIndentityCardNumber { get; set; }
    }
}
