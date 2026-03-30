using System.Threading.Tasks;

namespace WebAPIBackend.Services.Verification
{
    /// <summary>
    /// Interface for document verification operations
    /// </summary>
    public interface IVerificationService
    {
        /// <summary>
        /// Gets existing verification code or creates a new one for a document
        /// </summary>
        Task<VerificationResult> GetOrCreateVerificationAsync(int documentId, string documentType, string userId);

        /// <summary>
        /// Verifies a document using its verification code
        /// </summary>
        Task<DocumentVerificationDto> VerifyDocumentAsync(string verificationCode, string ipAddress);

        /// <summary>
        /// Revokes a verification code
        /// </summary>
        Task<bool> RevokeVerificationAsync(string verificationCode, string reason, string userId);

        /// <summary>
        /// Gets verification statistics for a document
        /// </summary>
        Task<VerificationStatsDto> GetVerificationStatsAsync(string verificationCode);
    }

    /// <summary>
    /// Result of verification code generation
    /// </summary>
    public class VerificationResult
    {
        public string VerificationCode { get; set; } = string.Empty;
        public string VerificationUrl { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Document verification result DTO
    /// </summary>
    public class DocumentVerificationDto
    {
        public bool IsValid { get; set; }
        public string Status { get; set; } = string.Empty; // Valid, Invalid, Expired, Revoked
        public string VerificationCode { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? HolderName { get; set; }
        public string? HolderPhoto { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? RevokedReason { get; set; }
        public DateTime VerifiedAt { get; set; }
        public string? CompanyTitle { get; set; }
        public string? OfficeAddress { get; set; }
        
        // Property details (for Property documents)
        public string? SerialNumber { get; set; }
        public string? CustomDocumentType { get; set; }
        public string? PropertyType { get; set; }
        public string? PropertyTypeName { get; set; }
        public string? PropertyTypeDari { get; set; }
        public decimal? Area { get; set; }
        public string? UnitType { get; set; }
        public string? UnitTypeDari { get; set; }
        public string? Province { get; set; }
        public string? ProvinceDari { get; set; }
        public string? District { get; set; }
        public string? DistrictDari { get; set; }
        public string? Village { get; set; }
        
        // Boundaries
        public string? North { get; set; }
        public string? South { get; set; }
        public string? East { get; set; }
        public string? West { get; set; }
        
        // Price info
        public decimal? Price { get; set; }
        public string? PriceText { get; set; }
        public decimal? RoyaltyAmount { get; set; }
        public decimal? HalfPrice { get; set; }
        
        // Witnesses
        public WitnessInfoDto? WitnessOne { get; set; }
        public WitnessInfoDto? WitnessTwo { get; set; }
        
        // Seller information (for Property and Vehicle documents)
        public SellerInfoDto? SellerInfo { get; set; }
        
        // Buyer information (for Property and Vehicle documents)
        public BuyerInfoDto? BuyerInfo { get; set; }
        
        // Petition Writer information (for PetitionWriterLicense documents)
        public PetitionWriterInfoDto? PetitionWriterInfo { get; set; }
    }

    /// <summary>
    /// Witness information DTO
    /// </summary>
    public class WitnessInfoDto
    {
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
    }

    /// <summary>
    /// Seller information DTO
    /// </summary>
    public class SellerInfoDto
    {
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Village { get; set; }
    }

    /// <summary>
    /// Buyer information DTO
    /// </summary>
    public class BuyerInfoDto
    {
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? GrandFatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Village { get; set; }
    }

    /// <summary>
    /// Petition Writer information DTO
    /// </summary>
    public class PetitionWriterInfoDto
    {
        public string? ApplicantFatherName { get; set; }
        public string? ApplicantGrandFatherName { get; set; }
        public string? ElectronicNationalIdNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Competency { get; set; }
        public string? District { get; set; }
        public string? LicenseType { get; set; }
        public decimal? LicensePrice { get; set; }
        public string? PermanentProvinceName { get; set; }
        public string? PermanentDistrictName { get; set; }
        public string? PermanentVillage { get; set; }
        public string? CurrentProvinceName { get; set; }
        public string? CurrentDistrictName { get; set; }
        public string? CurrentVillage { get; set; }
        public string? DetailedAddress { get; set; }
        public string? LatestRelocation { get; set; }
    }

    /// <summary>
    /// Verification statistics DTO
    /// </summary>
    public class VerificationStatsDto
    {
        public string VerificationCode { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LastVerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
