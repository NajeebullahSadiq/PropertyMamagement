namespace WebAPIBackend.Application.Company.DTOs
{
    public class CompanyDetailDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public int? LicenseNumber { get; set; }
        public string? PetitionNumber { get; set; }
        public DateOnly? PetitionDate { get; set; }
        public string? Tin { get; set; }
        public string? DocPath { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CompanyListItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerFatherName { get; set; }
        public string? OwnerIdNumber { get; set; }
        public int? LicenseNumber { get; set; }
        public string? Guarantor { get; set; }
    }

    public class CompanyViewDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public int? LicenseNumber { get; set; }
        public DateOnly? PetitionDate { get; set; }
        public string? PetitionNumber { get; set; }
        public string? Tin { get; set; }
        public string? DocPath { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CompanyOwnerViewDto? Owner { get; set; }
        public LicenseDetailViewDto? License { get; set; }
        public List<GuarantorViewDto> Guarantors { get; set; } = new();
        public AccountInfoViewDto? AccountInfo { get; set; }
        public CancellationInfoViewDto? CancellationInfo { get; set; }
    }

    public class CompanyOwnerViewDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFatherName { get; set; }
        public DateOnly? DateofBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? Jild { get; set; }
        public string? Safha { get; set; }
        public string? SabtNumber { get; set; }
        public string? PothoPath { get; set; }
        public string? EducationLevelName { get; set; }
        public string? IdentityCardTypeName { get; set; }
        public string? OwnerProvinceName { get; set; }
        public string? OwnerDistrictName { get; set; }
        public string? OwnerVillage { get; set; }
        public string? PermanentProvinceName { get; set; }
        public string? PermanentDistrictName { get; set; }
        public string? PermanentVillage { get; set; }
        public string? TemporaryProvinceName { get; set; }
        public string? TemporaryDistrictName { get; set; }
        public string? TemporaryVillage { get; set; }
    }

    public class LicenseDetailViewDto
    {
        public int Id { get; set; }
        public int? LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseCategory { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpireDate { get; set; }
        public string? OfficeAddress { get; set; }
        public string? AreaName { get; set; }
        public decimal? RoyaltyAmount { get; set; }
        public DateOnly? RoyaltyDate { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public DateOnly? PenaltyDate { get; set; }
        public string? HrLetter { get; set; }
        public DateOnly? HrLetterDate { get; set; }
        public string? DocPath { get; set; }
    }

    public class GuarantorViewDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFatherName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IndentityCardNumber { get; set; }
        public string? Jild { get; set; }
        public string? Safha { get; set; }
        public string? SabtNumber { get; set; }
        public string? PothoPath { get; set; }
        public string? IdentityCardTypeName { get; set; }
        public string? GuaranteeTypeName { get; set; }
        public int? GuaranteeTypeId { get; set; }
        public string? PropertyDocumentNumber { get; set; }
        public DateOnly? PropertyDocumentDate { get; set; }
        public string? SenderMaktobNumber { get; set; }
        public DateOnly? SenderMaktobDate { get; set; }
        public string? AnswerdMaktobNumber { get; set; }
        public DateOnly? AnswerdMaktobDate { get; set; }
        public DateOnly? DateofGuarantee { get; set; }
        public string? GuaranteeDocNumber { get; set; }
        public DateOnly? GuaranteeDate { get; set; }
        public string? GuaranteeDocPath { get; set; }
        public string? CourtName { get; set; }
        public string? CollateralNumber { get; set; }
        public string? SetSerialNumber { get; set; }
        public string? GuaranteeDistrictName { get; set; }
        public string? BankName { get; set; }
        public string? DepositNumber { get; set; }
        public DateOnly? DepositDate { get; set; }
        public string? PermanentProvinceName { get; set; }
        public string? PermanentDistrictName { get; set; }
        public string? PaddressVillage { get; set; }
        public string? TemporaryProvinceName { get; set; }
        public string? TemporaryDistrictName { get; set; }
        public string? TaddressVillage { get; set; }
    }

    public class AccountInfoViewDto
    {
        public int Id { get; set; }
        public string? SettlementInfo { get; set; }
        public decimal? TaxPaymentAmount { get; set; }
        public int? SettlementYear { get; set; }
        public DateOnly? TaxPaymentDate { get; set; }
        public int? TransactionCount { get; set; }
        public decimal? CompanyCommission { get; set; }
    }

    public class CancellationInfoViewDto
    {
        public int Id { get; set; }
        public string? LicenseCancellationLetterNumber { get; set; }
        public string? RevenueCancellationLetterNumber { get; set; }
        public DateOnly? LicenseCancellationLetterDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class CreateCompanyRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public int? LicenseNumber { get; set; }
        public string? PetitionNumber { get; set; }
        public string? PetitionDate { get; set; }
        public string? Tin { get; set; }
        public string? DocPath { get; set; }
        public string? CalendarType { get; set; }
    }
}
