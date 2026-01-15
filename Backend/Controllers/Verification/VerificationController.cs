using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIBackend.Services.Verification;
using System.Security.Claims;

namespace WebAPIBackend.Controllers.Verification
{
    /// <summary>
    /// Controller for document verification operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        /// <summary>
        /// Generate or retrieve verification code for a document
        /// POST /api/verification/generate
        /// </summary>
        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> GenerateVerification([FromBody] GenerateVerificationRequest request)
        {
            if (request == null || request.DocumentId <= 0 || string.IsNullOrWhiteSpace(request.DocumentType))
            {
                return BadRequest(new { error = "Invalid request. DocumentId and DocumentType are required." });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var result = await _verificationService.GetOrCreateVerificationAsync(
                request.DocumentId, 
                request.DocumentType, 
                userId);

            if (!result.Success)
            {
                return StatusCode(500, new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                verificationCode = result.VerificationCode,
                verificationUrl = result.VerificationUrl,
                isNew = result.IsNew
            });
        }

        /// <summary>
        /// Verify a document using its verification code (PUBLIC - no auth required)
        /// GET /api/verification/verify/{code}
        /// </summary>
        [HttpGet("verify/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyDocument(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { error = "Verification code is required" });
            }

            var ipAddress = GetClientIpAddress();
            var result = await _verificationService.VerifyDocumentAsync(code, ipAddress);

            return Ok(result);
        }

        /// <summary>
        /// Revoke a verification code (Admin only)
        /// POST /api/verification/revoke
        /// </summary>
        [HttpPost("revoke")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RevokeVerification([FromBody] RevokeVerificationRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.VerificationCode))
            {
                return BadRequest(new { error = "Verification code is required" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var result = await _verificationService.RevokeVerificationAsync(
                request.VerificationCode, 
                request.Reason ?? "No reason provided", 
                userId);

            if (!result)
            {
                return NotFound(new { error = "Verification code not found or already revoked" });
            }

            return Ok(new { success = true, message = "Verification code revoked successfully" });
        }

        /// <summary>
        /// Get verification statistics for a document
        /// GET /api/verification/stats/{code}
        /// </summary>
        [HttpGet("stats/{code}")]
        [Authorize]
        public async Task<IActionResult> GetVerificationStats(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { error = "Verification code is required" });
            }

            var stats = await _verificationService.GetVerificationStatsAsync(code);
            return Ok(stats);
        }

        /// <summary>
        /// Gets the client IP address from the request
        /// </summary>
        private string GetClientIpAddress()
        {
            // Check for forwarded IP (behind proxy/load balancer)
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',').First().Trim();
            }

            // Check for real IP header
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fall back to connection remote IP
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    /// <summary>
    /// Request DTO for generating verification code
    /// </summary>
    public class GenerateVerificationRequest
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for revoking verification code
    /// </summary>
    public class RevokeVerificationRequest
    {
        public string VerificationCode { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}
