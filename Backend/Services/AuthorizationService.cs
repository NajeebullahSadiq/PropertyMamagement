using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;

namespace WebAPIBackend.Services
{
    public interface IAuthorizationService
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
        Task<bool> CanAccessModuleAsync(ClaimsPrincipal user, string module);
        Task<bool> CanEditRecordAsync(ClaimsPrincipal user, string module, string recordCreatedBy);
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user);
        string[] GetUserPermissions(string role);
        bool IsViewOnlyRole(string role);
    }

    public class AuthorizationServiceImpl : IAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationServiceImpl(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var appUser = await GetCurrentUserAsync(user);
            if (appUser == null) return false;

            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in roles)
            {
                var permissions = RolePermissions.GetPermissionsForRole(role);
                if (permissions.Contains(permission))
                    return true;
            }

            return false;
        }

        public async Task<bool> CanAccessModuleAsync(ClaimsPrincipal user, string module)
        {
            var appUser = await GetCurrentUserAsync(user);
            if (appUser == null) return false;

            var roles = await _userManager.GetRolesAsync(appUser);
            var primaryRole = roles.FirstOrDefault() ?? appUser.UserRole;

            if (string.IsNullOrEmpty(primaryRole))
                return false;

            // Admin and Authority can access all modules
            if (primaryRole == UserRoles.Admin || primaryRole == UserRoles.Authority)
                return true;

            return module.ToLower() switch
            {
                "company" => primaryRole == UserRoles.CompanyRegistrar || 
                             primaryRole == UserRoles.LicenseReviewer,
                
                "property" => primaryRole == UserRoles.PropertyOperator ||
                              (primaryRole == UserRoles.CompanyRegistrar && false) || // Company registrar cannot access property
                              appUser.LicenseType == "realEstate",
                
                "vehicle" => primaryRole == UserRoles.VehicleOperator ||
                             appUser.LicenseType == "carSale",
                
                "reports" => primaryRole != UserRoles.LicenseReviewer,
                
                "dashboard" => primaryRole != UserRoles.LicenseReviewer,
                
                "users" => primaryRole == UserRoles.Admin,
                
                _ => false
            };
        }

        public async Task<bool> CanEditRecordAsync(ClaimsPrincipal user, string module, string recordCreatedBy)
        {
            var appUser = await GetCurrentUserAsync(user);
            if (appUser == null) return false;

            var roles = await _userManager.GetRolesAsync(appUser);
            var primaryRole = roles.FirstOrDefault() ?? appUser.UserRole;

            // Admin can edit all records
            if (primaryRole == UserRoles.Admin)
                return true;

            // Authority cannot edit anything
            if (primaryRole == UserRoles.Authority || primaryRole == UserRoles.LicenseReviewer)
                return false;

            // Company registrar can edit company records
            if (primaryRole == UserRoles.CompanyRegistrar && module == "company")
                return true;

            // Property/Vehicle operators can only edit their own records
            if ((primaryRole == UserRoles.PropertyOperator && module == "property") ||
                (primaryRole == UserRoles.VehicleOperator && module == "vehicle"))
            {
                return recordCreatedBy == appUser.Id;
            }

            return false;
        }

        public string[] GetUserPermissions(string role)
        {
            return RolePermissions.GetPermissionsForRole(role);
        }

        public bool IsViewOnlyRole(string role)
        {
            return role == UserRoles.Authority || role == UserRoles.LicenseReviewer;
        }
    }
}
