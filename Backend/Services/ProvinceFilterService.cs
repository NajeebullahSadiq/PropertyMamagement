using WebAPIBackend.Configuration;
using WebAPIBackend.Models.Common;

namespace WebAPIBackend.Services
{
    /// <summary>
    /// Implementation of province-based filtering and access control service
    /// </summary>
    public class ProvinceFilterService : IProvinceFilterService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecurityAuditLogger _auditLogger;

        public ProvinceFilterService(
            IHttpContextAccessor httpContextAccessor,
            ISecurityAuditLogger auditLogger)
        {
            _httpContextAccessor = httpContextAccessor;
            _auditLogger = auditLogger;
        }

        /// <summary>
        /// Get the current user's province ID from HttpContext
        /// </summary>
        public int? GetUserProvinceId()
        {
            var provinceIdStr = _httpContextAccessor.HttpContext?.Items["UserProvinceId"]?.ToString();
            if (int.TryParse(provinceIdStr, out int provinceId))
            {
                return provinceId;
            }
            return null;
        }

        /// <summary>
        /// Check if the current user is an administrator
        /// </summary>
        public bool IsAdministrator()
        {
            var role = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            return role == UserRoles.Admin;
        }

        /// <summary>
        /// Apply province filtering to a queryable collection
        /// Filters by user's province for COMPANY_REGISTRAR, no filter for administrators
        /// </summary>
        public IQueryable<T> ApplyProvinceFilter<T>(IQueryable<T> query) where T : IProvinceEntity
        {
            // Administrators see all data
            if (IsAdministrator())
            {
                return query;
            }

            // Get user's province
            var provinceId = GetUserProvinceId();
            if (!provinceId.HasValue)
            {
                throw new UnauthorizedAccessException("Province not found");
            }

            // Filter by province
            return query.Where(e => e.ProvinceId == provinceId.Value);
        }

        /// <summary>
        /// Validate that the current user has access to an entity in the specified province
        /// Throws ForbiddenException if access is denied
        /// </summary>
        public void ValidateProvinceAccess(int? entityProvinceId)
        {
            // Administrators have access to all provinces
            if (IsAdministrator())
            {
                return;
            }

            var userProvinceId = GetUserProvinceId();
            if (userProvinceId != entityProvinceId)
            {
                // Log the access violation
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "unknown";
                _auditLogger.LogProvinceAccessViolation(
                    userId,
                    userProvinceId?.ToString() ?? "null",
                    entityProvinceId?.ToString() ?? "null",
                    "Entity",
                    "unknown",
                    "Access");

                throw new ForbiddenException("Access denied to this province");
            }
        }
    }
}
