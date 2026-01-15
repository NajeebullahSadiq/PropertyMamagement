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
