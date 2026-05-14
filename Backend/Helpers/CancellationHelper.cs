using WebAPIBackend.Models;

namespace WebAPIBackend.Helpers
{
    public static class CancellationHelper
    {
        /// <summary>
        /// Check if a company cancellation record represents a valid/requiring cancellation.
        /// 
        /// For فسخ (Faskh): Requires both LicenseCancellationLetterNumber AND RevenueCancellationLetterNumber
        /// For لغوه (Laghwa): Requires both RevocationLetterNumber AND RevocationRevenueLetterNumber
        /// </summary>
        /// <param name="cancellation">The cancellation record to check</param>
        /// <returns>True if the cancellation is valid and requires blocking</returns>
        public static bool IsValidCancellation(CompanyCancellationInfo? cancellation)
        {
            if (cancellation == null || cancellation.Status == false)
                return false;

            // Check for فسخ - requires both license and revenue cancellation letter numbers
            if (cancellation.CancellationType == "فسخ" &&
                !string.IsNullOrWhiteSpace(cancellation.LicenseCancellationLetterNumber) &&
                !string.IsNullOrWhiteSpace(cancellation.RevenueCancellationLetterNumber))
            {
                return true;
            }

            // Check for لغوه - requires both revocation letter numbers
            if (cancellation.CancellationType == "لغوه" &&
                !string.IsNullOrWhiteSpace(cancellation.RevocationLetterNumber) &&
                !string.IsNullOrWhiteSpace(cancellation.RevocationRevenueLetterNumber))
            {
                return true;
            }

            return false;
        }
    }
}
