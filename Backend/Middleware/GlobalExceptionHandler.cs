using Microsoft.AspNetCore.Diagnostics;
using WebAPIBackend.Models.Common;

namespace WebAPIBackend.Middleware
{
    /// <summary>
    /// Global exception handler that maps exceptions to appropriate HTTP status codes
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = exception switch
            {
                UnauthorizedAccessException => new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Unauthorized",
                    Details = exception.Message
                },
                ForbiddenException => new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "Forbidden",
                    Details = exception.Message
                },
                NotFoundException => new ErrorResponse
                {
                    StatusCode = 404,
                    Message = "Not Found",
                    Details = exception.Message
                },
                ValidationException => new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Validation Error",
                    Details = exception.Message
                },
                _ => new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                    Details = "An unexpected error occurred"
                }
            };

            response.Timestamp = DateTime.UtcNow;

            httpContext.Response.StatusCode = response.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
