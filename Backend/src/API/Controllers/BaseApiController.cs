using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIBackend.API.Controllers
{
    /// <summary>
    /// Base controller with common functionality for all API controllers
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Get current user ID from claims
        /// </summary>
        protected string? GetCurrentUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
        }

        /// <summary>
        /// Return standardized error response
        /// </summary>
        protected IActionResult ErrorResponse(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new { message });
        }

        /// <summary>
        /// Return standardized success response with data
        /// </summary>
        protected IActionResult SuccessResponse<T>(T data, string? message = null)
        {
            if (message != null)
                return Ok(new { data, message });
            return Ok(data);
        }

        /// <summary>
        /// Return standardized success response with ID
        /// </summary>
        protected IActionResult CreatedResponse(int id, string? message = null)
        {
            return Ok(new { id, message });
        }

        /// <summary>
        /// Return forbidden response with Dari message
        /// </summary>
        protected IActionResult ForbiddenResponse(string message = "شما اجازه دسترسی ندارید")
        {
            return StatusCode(403, new { message });
        }

        /// <summary>
        /// Return not found response with Dari message
        /// </summary>
        protected IActionResult NotFoundResponse(string message = "رکورد یافت نشد")
        {
            return NotFound(new { message });
        }

        /// <summary>
        /// Return internal server error response
        /// </summary>
        protected IActionResult ServerErrorResponse(Exception ex, string? hint = null)
        {
            var response = new
            {
                message = "خطای داخلی سرور",
                error = ex.Message,
                hint
            };
            return StatusCode(500, response);
        }
    }
}
