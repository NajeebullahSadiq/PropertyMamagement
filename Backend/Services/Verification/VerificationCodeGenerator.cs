using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WebAPIBackend.Services.Verification
{
    /// <summary>
    /// Generates unique verification codes for document verification
    /// </summary>
    public class VerificationCodeGenerator : IVerificationCodeGenerator
    {
        private const string AlphanumericChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int RandomPartLength = 6;
        
        // Regex pattern: 3 uppercase letters, dash, 4 digits, dash, 6 alphanumeric
        private static readonly Regex CodeFormatRegex = new Regex(
            @"^[A-Z]{3}-\d{4}-[A-Z0-9]{6}$", 
            RegexOptions.Compiled);

        /// <summary>
        /// Generates a unique verification code in format: {PREFIX}-{YEAR}-{RANDOM}
        /// Example: LIC-2026-A7X9K2
        /// </summary>
        public string GenerateCode(string documentTypePrefix)
        {
            if (string.IsNullOrWhiteSpace(documentTypePrefix) || documentTypePrefix.Length != 3)
            {
                throw new ArgumentException("Document type prefix must be exactly 3 characters", nameof(documentTypePrefix));
            }

            var prefix = documentTypePrefix.ToUpperInvariant();
            var year = DateTime.UtcNow.Year;
            var randomPart = GenerateRandomAlphanumeric(RandomPartLength);

            return $"{prefix}-{year}-{randomPart}";
        }

        /// <summary>
        /// Validates if a verification code matches the expected format
        /// </summary>
        public bool ValidateCodeFormat(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            return CodeFormatRegex.IsMatch(code);
        }

        /// <summary>
        /// Generates a cryptographically secure random alphanumeric string
        /// </summary>
        private static string GenerateRandomAlphanumeric(int length)
        {
            var result = new char[length];
            var randomBytes = new byte[length];
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < length; i++)
            {
                result[i] = AlphanumericChars[randomBytes[i] % AlphanumericChars.Length];
            }

            return new string(result);
        }
    }
}
