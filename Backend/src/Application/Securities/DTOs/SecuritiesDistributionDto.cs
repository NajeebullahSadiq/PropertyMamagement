namespace WebAPIBackend.Application.Securities.DTOs
{
    public class SecuritiesDistributionDto
    {
        public int Id { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LicenseOwnerName { get; set; }
        public string? LicenseOwnerFatherName { get; set; }
        public string? TransactionGuideName { get; set; }
        public string? LicenseNumber { get; set; }
        public int? DocumentType { get; set; }
        public int? PropertySubType { get; set; }
        public int? VehicleSubType { get; set; }
        public int? PropertySaleCount { get; set; }
        public string? PropertySaleSerialStart { get; set; }
        public string? PropertySaleSerialEnd { get; set; }
        public int? BayWafaCount { get; set; }
        public string? BayWafaSerialStart { get; set; }
        public string? BayWafaSerialEnd { get; set; }
        public int? RentCount { get; set; }
        public string? RentSerialStart { get; set; }
        public string? RentSerialEnd { get; set; }
        public int? VehicleSaleCount { get; set; }
        public string? VehicleSaleSerialStart { get; set; }
        public string? VehicleSaleSerialEnd { get; set; }
        public int? VehicleExchangeCount { get; set; }
        public string? VehicleExchangeSerialStart { get; set; }
        public string? VehicleExchangeSerialEnd { get; set; }
        public int? RegistrationBookType { get; set; }
        public int? RegistrationBookCount { get; set; }
        public int? DuplicateBookCount { get; set; }
        public decimal? PricePerDocument { get; set; }
        public decimal? TotalDocumentsPrice { get; set; }
        public decimal? RegistrationBookPrice { get; set; }
        public decimal? TotalSecuritiesPrice { get; set; }
        public string? BankReceiptNumber { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public string? DeliveryDateFormatted { get; set; }
        public DateOnly? DistributionDate { get; set; }
        public string? DistributionDateFormatted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool Status { get; set; }
    }

    public class SecuritiesDistributionListItemDto
    {
        public int Id { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LicenseOwnerName { get; set; }
        public string? LicenseOwnerFatherName { get; set; }
        public string? TransactionGuideName { get; set; }
        public string? LicenseNumber { get; set; }
        public int? DocumentType { get; set; }
        public decimal? TotalSecuritiesPrice { get; set; }
        public string? BankReceiptNumber { get; set; }
        public string? DeliveryDateFormatted { get; set; }
        public string? DistributionDateFormatted { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateSecuritiesDistributionRequest
    {
        public string? RegistrationNumber { get; set; }
        public string? LicenseOwnerName { get; set; }
        public string? LicenseOwnerFatherName { get; set; }
        public string? TransactionGuideName { get; set; }
        public string? LicenseNumber { get; set; }
        public int? DocumentType { get; set; }
        public int? PropertySubType { get; set; }
        public int? VehicleSubType { get; set; }
        public int? PropertySaleCount { get; set; }
        public string? PropertySaleSerialStart { get; set; }
        public string? PropertySaleSerialEnd { get; set; }
        public int? BayWafaCount { get; set; }
        public string? BayWafaSerialStart { get; set; }
        public string? BayWafaSerialEnd { get; set; }
        public int? RentCount { get; set; }
        public string? RentSerialStart { get; set; }
        public string? RentSerialEnd { get; set; }
        public int? VehicleSaleCount { get; set; }
        public string? VehicleSaleSerialStart { get; set; }
        public string? VehicleSaleSerialEnd { get; set; }
        public int? VehicleExchangeCount { get; set; }
        public string? VehicleExchangeSerialStart { get; set; }
        public string? VehicleExchangeSerialEnd { get; set; }
        public int? RegistrationBookType { get; set; }
        public int? RegistrationBookCount { get; set; }
        public int? DuplicateBookCount { get; set; }
        public decimal? PricePerDocument { get; set; }
        public decimal? TotalDocumentsPrice { get; set; }
        public decimal? RegistrationBookPrice { get; set; }
        public decimal? TotalSecuritiesPrice { get; set; }
        public string? BankReceiptNumber { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public DateOnly? DistributionDate { get; set; }
    }

    public class PetitionWriterSecuritiesDto
    {
        public int Id { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? PetitionWriterName { get; set; }
        public string? PetitionWriterFatherName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? BankReceiptNumber { get; set; }
        public string? SerialNumberStart { get; set; }
        public string? SerialNumberEnd { get; set; }
        public int? DocumentCount { get; set; }
        public decimal? Amount { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public string? DeliveryDateFormatted { get; set; }
        public DateOnly? DistributionDate { get; set; }
        public string? DistributionDateFormatted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public bool Status { get; set; }
    }

    public class SecuritiesPrintDto
    {
        public int Id { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LicenseOwnerName { get; set; }
        public string? LicenseOwnerFatherName { get; set; }
        public string? TransactionGuideName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? DocumentTypeName { get; set; }
        public int? TotalDocumentCount { get; set; }
        public decimal? TotalSecuritiesPrice { get; set; }
        public string? BankReceiptNumber { get; set; }
        public string? DeliveryDateFormatted { get; set; }
        public string? DistributionDateFormatted { get; set; }
        // Property details
        public int? PropertySaleCount { get; set; }
        public string? PropertySaleSerialRange { get; set; }
        public int? BayWafaCount { get; set; }
        public string? BayWafaSerialRange { get; set; }
        public int? RentCount { get; set; }
        public string? RentSerialRange { get; set; }
        // Vehicle details
        public int? VehicleSaleCount { get; set; }
        public string? VehicleSaleSerialRange { get; set; }
        public int? VehicleExchangeCount { get; set; }
        public string? VehicleExchangeSerialRange { get; set; }
        // Registration book
        public string? RegistrationBookTypeName { get; set; }
        public int? RegistrationBookCount { get; set; }
    }
}
