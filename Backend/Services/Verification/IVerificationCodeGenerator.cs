namespace WebAPIBackend.Services.Verification
{
    /// <summary>
    /// Interface for generating unique verification codes
    /// </summary>
    public interface IVerificationCodeGenerator
    {
        /// <summary>
        /// Generates a unique verification code in format: {PREFIX}-{YEAR}-{RANDOM}
        /// Example: LIC-2026-A7X9K2
        /// </summary>
        /// <param name="documentTypePrefix">3-letter prefix for document type (e.g., LIC, PWL, SEC)</param>
        /// <returns>Unique verification code</returns>
        string GenerateCode(string documentTypePrefix);

        /// <summary>
        /// Validates if a verification code matches the expected format
        /// </summary>
        /// <param name="code">Verification code to validate</param>
        /// <returns>True if format is valid</returns>
        bool ValidateCodeFormat(string code);
    }
}
