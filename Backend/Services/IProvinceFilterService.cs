using WebAPIBackend.Models.Common;

namespace WebAPIBackend.Services
{
    /// <summary>
    /// Service for applying province-based filtering and access control
    /// </summary>
    public interface IProvinceFilterService
    {
        /// <summary>
        /// Get the current user's province ID from HttpContext
        /// </summary>
        /// <returns>Province ID or null if not set</returns>
        int? GetUserProvinceId();

        /// <summary>
        /// Check if the current user is an administrator
        /// </summary>
        /// <returns>True if administrator, false otherwise</returns>
        bool IsAdministrator();

        /// <summary>
        /// Apply province filtering to a queryable collection
        /// Filters by user's province for COMPANY_REGISTRAR, no filter for administrators
        /// </summary>
        /// <typeparam name="T">Entity type implementing IProvinceEntity</typeparam>
        /// <param name="query">The queryable to filter</param>
        /// <returns>Filtered queryable</returns>
        IQueryable<T> ApplyProvinceFilter<T>(IQueryable<T> query) where T : IProvinceEntity;

        /// <summary>
        /// Validate that the current user has access to an entity in the specified province
        /// Throws ForbiddenException if access is denied
        /// </summary>
        /// <param name="entityProvinceId">The province ID of the entity being accessed</param>
        void ValidateProvinceAccess(int? entityProvinceId);
    }
}
