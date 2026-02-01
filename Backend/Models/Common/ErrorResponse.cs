using System;

namespace WebAPIBackend.Models.Common
{
    /// <summary>
    /// Standard error response model for API errors
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
