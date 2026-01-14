using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WebAPI.Models;
using WebAPIBackend.Application.Common.Interfaces;

namespace WebAPIBackend.Infrastructure.Services
{
    /// <summary>
    /// Service for accessing current user information
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == "UserID")?.Value;

        public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public async Task<IList<string>> GetRolesAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return new List<string>();

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<string?> GetLicenseTypeAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return null;

            var user = await _userManager.FindByIdAsync(UserId);
            return user?.LicenseType;
        }
    }
}
