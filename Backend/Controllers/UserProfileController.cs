using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserProfileController(UserManager<ApplicationUser> userManager, AppDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<Object> GetUserProfile()
        {
            List<string> ClaimValue = new List<string>();
            List<string> ClaimType = new List<string>();
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var userroles = await _userManager.GetRolesAsync(user);
            var primaryRole = userroles.FirstOrDefault() ?? user.UserRole ?? "";
            var permissions = RolePermissions.GetPermissionsForRole(primaryRole);

            foreach (var userRole in userroles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var c in roleClaims)
                {
                    ClaimValue.Add(c.Value);
                    ClaimType.Add(c.Type);
                }

                return new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.UserName,
                    user.PhotoPath,
                    user.IsLocked,
                    user.CompanyId,
                    user.LicenseType,
                    userroles,
                    Role = primaryRole,
                    RoleDari = UserRoles.GetDariName(primaryRole),
                    Permissions = permissions,
                    IsViewOnly = primaryRole == UserRoles.Authority || primaryRole == UserRoles.LicenseReviewer,
                    ClaimValue,
                    ClaimType
                };
            }
            return StatusCode(420);
        }

        [HttpGet]
        [Route("getProfile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized(new { message = "UserID claim not found" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User profile not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault() ?? user.UserRole ?? "";
                var permissions = RolePermissions.GetPermissionsForRole(primaryRole);

                var companyName = string.Empty;
                string? companyPhone = null;
                if (user.CompanyId > 0)
                {
                    var company = await _context.CompanyDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == user.CompanyId);

                    companyName = company?.Title ?? string.Empty;
                    companyPhone = company?.PhoneNumber;
                }

                return Ok(new
                {
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    UserId = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    PhotoPath = user.PhotoPath ?? string.Empty,
                    CompanyId = user.CompanyId,
                    CompanyName = companyName,
                    PhoneNumber = companyPhone ?? user.PhoneNumber ?? string.Empty,
                    LicenseType = user.LicenseType ?? string.Empty,
                    Role = primaryRole,
                    RoleDari = UserRoles.GetDariName(primaryRole),
                    Permissions = permissions,
                    IsViewOnly = primaryRole == UserRoles.Authority || primaryRole == UserRoles.LicenseReviewer,
                    CanAccessCompany = CanAccessModule(primaryRole, user.LicenseType, "company"),
                    CanAccessProperty = CanAccessModule(primaryRole, user.LicenseType, "property"),
                    CanAccessVehicle = CanAccessModule(primaryRole, user.LicenseType, "vehicle"),
                    CanAccessReports = CanAccessModule(primaryRole, user.LicenseType, "reports"),
                    CanAccessDashboard = CanAccessModule(primaryRole, user.LicenseType, "dashboard"),
                    CanAccessUsers = primaryRole == UserRoles.Admin
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving user profile",
                    error = ex.Message
                });
            }
        }

        private bool CanAccessModule(string role, string? licenseType, string module)
        {
            // Admin and Authority can access all modules
            if (role == UserRoles.Admin || role == UserRoles.Authority)
                return true;

            return module.ToLower() switch
            {
                "company" => role == UserRoles.CompanyRegistrar || role == UserRoles.LicenseReviewer,
                "property" => role == UserRoles.PropertyOperator || licenseType == "realEstate",
                "vehicle" => role == UserRoles.VehicleOperator || licenseType == "carSale",
                "reports" => role != UserRoles.LicenseReviewer,
                "dashboard" => role != UserRoles.LicenseReviewer,
                "users" => role == UserRoles.Admin,
                _ => false
            };
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("ForAdmin")]
        public string GetForAdmin()
        {
            return "Web method for Admin";
        }

        [HttpGet]
        [Route("Role")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoles()
        {
            var roles = await _context.Roles.OrderBy(r => r.Id).ToListAsync();
            return roles.Select(r => new
            {
                r.Id,
                r.Name,
                Dari = UserRoles.GetDariName(r.Name ?? "")
            }).ToList();
        }
    }
}