using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace WebAPIBackend.Services.Verification
{
    /// <summary>
    /// Service for generating and verifying HMAC-SHA256 digital signatures
    /// </summary>
    public class SignatureService : ISignatureService
    {
        private readonly byte[] _secretKey;

        public SignatureService(IConfiguration configuration)
        {
            var secretKeyString = configuration["Verification:SignatureSecretKey"] 
                ?? throw new InvalidOperationException("Verification:SignatureSecretKey is not configured");
            
            _secretKey = Encoding.UTF8.GetBytes(secretKeyString);
        }

        /// <summary>
        /// Constructor for testing with explicit secret key
        /// </summary>
        public SignatureService(string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentException("Secret key cannot be empty", nameof(secretKey));
            }
            _secretKey = Encoding.UTF8.GetBytes(secretKey);
        }

        /// <summary>
        /// Generates an HMAC-SHA256 signature for document data
        /// </summary>
        public string GenerateSignature(DocumentSignatureData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var dataToSign = BuildSignatureString(data);
            
            using var hmac = new HMACSHA256(_secretKey);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifies if the stored signature matches the current document data
        /// </summary>
        public bool VerifySignature(DocumentSignatureData data, string storedSignature)
        {
            if (data == null || string.IsNullOrWhiteSpace(storedSignature))
            {
                return false;
            }

            var currentSignature = GenerateSignature(data);
            
            // Use constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(currentSignature),
                Convert.FromBase64String(storedSignature));
        }

        /// <summary>
        /// Builds a canonical string representation of document data for signing
        /// </summary>
        private static string BuildSignatureString(DocumentSignatureData data)
        {
            var sb = new StringBuilder();
            
            sb.Append("DocumentId:");
            sb.Append(data.DocumentId);
            sb.Append("|DocumentType:");
            sb.Append(data.DocumentType ?? string.Empty);
            sb.Append("|LicenseNumber:");
            sb.Append(data.LicenseNumber ?? string.Empty);
            sb.Append("|HolderName:");
            sb.Append(data.HolderName ?? string.Empty);
            sb.Append("|IssueDate:");
            sb.Append(data.IssueDate?.ToString("yyyy-MM-dd") ?? string.Empty);
            sb.Append("|ExpiryDate:");
            sb.Append(data.ExpiryDate?.ToString("yyyy-MM-dd") ?? string.Empty);
            
            return sb.ToString();
        }
    }
}
