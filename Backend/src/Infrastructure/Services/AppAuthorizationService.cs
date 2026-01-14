using WebAPIBackend.Application.Common.Interfaces;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;

namespace WebAPIBackend.Infrastructure.Services
{
    /// <summary>
    /// Service for authorization operations
    /// </summary>
    public class AppAuthorizationService : IAppAuthorizationService
    {
        private readonly ICurrentUserService _currentUserService;

        public AppAuthorizationService(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            var roles = await _currentUserService.GetRolesAsync();
            foreach (var role in roles)
            {
                var permissions = RolePermissions.GetPermissionsForRole(role);
                if (permissions.Contains(permission))
                    return true;
            }
            return false;
        }

        public async Task<bool> CanAccessModuleAsync(string module)
        {
            var roles = await _currentUserService.GetRolesAsync();
            var licenseType = await _currentUserService.GetLicenseTypeAsync();
            return RbacHelper.CanAccessModule(roles, licenseType, module);
        }

        public async Task<bool> CanEditRecordAsync(string module, string recordCreatedBy)
        {
            var roles = await _currentUserService.GetRolesAsync();
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                return false;
            return RbacHelper.CanEditRecord(roles, module, recordCreatedBy, userId);
        }

        public async Task<bool> CanCreateRecordAsync(string module)
        {
            var roles = await _currentUserService.GetRolesAsync();
            return RbacHelper.CanCreateRecords(roles, module);
        }

        public async Task<bool> CanDeleteRecordAsync(string module)
        {
            var roles = await _currentUserService.GetRolesAsync();
            return RbacHelper.CanDeleteRecords(roles, module);
        }

        public async Task<bool> CanViewAllRecordsAsync(string module)
        {
            var roles = await _currentUserService.GetRolesAsync();
            return RbacHelper.CanViewAllRecords(roles, module);
        }

        public bool IsViewOnlyRole(string role)
        {
            return role == UserRoles.Authority || role == UserRoles.LicenseReviewer;
        }

        public string[] GetUserPermissions(string role)
        {
            return RolePermissions.GetPermissionsForRole(role);
        }
    }
}
