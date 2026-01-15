namespace WebAPIBackend.Services.Verification
{
    /// <summary>
    /// Interface for digital signature generation and verification
    /// </summary>
    public interface ISignatureService
    {
        /// <summary>
        /// Generates an HMAC-SHA256 signature for document data
        /// </summary>
        /// <param name="data">Document data to sign</param>
        /// <returns>Base64-encoded signature</returns>
        string GenerateSignature(DocumentSignatureData data);

        /// <summary>
        /// Verifies if the stored signature matches the current document data
        /// </summary>
        /// <param name="data">Current document data</param>
        /// <param name="storedSignature">Previously stored signature</param>
        /// <returns>True if signatures match</returns>
        bool VerifySignature(DocumentSignatureData data, string storedSignature);
    }

    /// <summary>
    /// Data structure containing key document fields for signature generation
    /// </summary>
    public class DocumentSignatureData
    {
        public string LicenseNumber { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
    }
}
