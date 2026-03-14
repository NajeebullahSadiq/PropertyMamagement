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
        /// Check if the current user is a system-level role that doesn't require province filtering
        /// </summary>
        private bool IsSystemLevelRole()
        {
            var role = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            return role == UserRoles.Admin || 
                   role == UserRoles.Authority || 
                   role == UserRoles.LicenseReviewer ||
                   role == UserRoles.LicenseApplicationManager ||
                   role == UserRoles.ActivityMonitoringManager ||
                   role == UserRoles.SecuritiesManager ||
                   role == UserRoles.SecuritiesEntryManager ||
                   role == UserRoles.PetitionWriterSecuritiesEntryManager ||
                   role == UserRoles.PetitionWriterLicenseManager;
        }

        /// <summary>
        /// Apply province filtering to a queryable collection
        /// Filters by user's province for COMPANY_REGISTRAR, no filter for administrators and system-level roles
        /// </summary>
        public IQueryable<T> ApplyProvinceFilter<T>(IQueryable<T> query) where T : IProvinceEntity
        {
            // System-level roles (Admin, Authority, LicenseReviewer, LicenseApplicationManager) see all data
            if (IsSystemLevelRole())
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
            // System-level roles have access to all provinces
            if (IsSystemLevelRole())
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
