using System.Security.Claims;
using WebAPIBackend.Configuration;

namespace WebAPIBackend.Middleware
{
    /// <summary>
    /// Middleware that extracts province and role information from JWT token claims
    /// and stores them in HttpContext.Items for use by service layer
    /// </summary>
    public class ProvinceAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public ProvinceAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extract user role from claims
            var roleClaim = context.User.FindFirst("userRole");
            var role = roleClaim?.Value;

            if (!string.IsNullOrEmpty(role))
            {
                // Store role in HttpContext for service layer access
                context.Items["UserRole"] = role;

                // For COMPANY_REGISTRAR users, extract province claim if present
                if (role == UserRoles.CompanyRegistrar)
                {
                    var provinceIdClaim = context.User.FindFirst(CustomClaimTypes.ProvinceId);

                    if (provinceIdClaim != null && !string.IsNullOrEmpty(provinceIdClaim.Value))
                    {
                        // Store province in HttpContext for service layer access
                        context.Items["UserProvinceId"] = provinceIdClaim.Value;
                    }
                    // If province claim is missing, the service layer handles it gracefully
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to register the middleware
    /// </summary>
    public static class ProvinceAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseProvinceAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProvinceAuthorizationMiddleware>();
        }
    }
}
