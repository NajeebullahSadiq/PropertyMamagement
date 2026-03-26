using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using WebAPIBackend.Configuration;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _singInManager;
        private readonly ApplicationSettings _appSettings;
        private AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<ApplicationSettings> appSettings,
           RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _singInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [Route("Register")]
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            // Validate role
            if (!UserRoles.AllRoles.Contains(model.Role))
            {
                return BadRequest(new { message = $"Invalid role: {model.Role}" });
            }

            // Validate province requirement for COMPANY_REGISTRAR and PETITION_WRITER_LICENSE_MANAGER
            if ((model.Role == UserRoles.CompanyRegistrar || model.Role == UserRoles.PetitionWriterLicenseManager) && !model.ProvinceId.HasValue)
            {
                return BadRequest(new { message = "Province is required for this role" });
            }

            // System-level roles don't need company
            var systemLevelRoles = new[] { 
                UserRoles.Admin, 
                UserRoles.Authority, 
                UserRoles.LicenseReviewer,
                UserRoles.LicenseApplicationManager,
                UserRoles.ActivityMonitoringManager,
                UserRoles.SecuritiesManager,
                UserRoles.SecuritiesEntryManager,
                UserRoles.PetitionWriterSecuritiesEntryManager,
                UserRoles.PetitionWriterLicenseManager
            };
            
            if (systemLevelRoles.Contains(model.Role))
            {
                model.CompanyId = 0; // Ensure no company association
                model.LicenseType = null; // No license type needed
            }

            // Validate province exists if provided
            if (model.ProvinceId.HasValue)
            {
                var provinceExists = await _context.Locations.AnyAsync(l => l.Id == model.ProvinceId.Value);
                if (!provinceExists)
                {
                    return BadRequest(new { message = "Invalid province" });
                }
            }

            // Validate company operators must have company and license type
            if ((model.Role == UserRoles.PropertyOperator || model.Role == UserRoles.VehicleOperator) 
                && model.CompanyId == 0)
            {
                return BadRequest(new { message = "Company operators must be associated with a company" });
            }

            // Validate company exists and license type matches
            if (model.CompanyId > 0)
            {
                var company = await _context.CompanyDetails
                    .Include(c => c.LicenseDetails)
                    .FirstOrDefaultAsync(c => c.Id == model.CompanyId);

                if (company == null)
                {
                    return BadRequest(new { message = "رهنما یافت نشد" });
                }

                // Check if company has licenses
                if (company.LicenseDetails == null || !company.LicenseDetails.Any())
                {
                    return BadRequest(new { message = "این رهنما هیچ جوازی ندارد" });
                }

                // Validate license type matches for operators
                if (model.Role == UserRoles.PropertyOperator || model.Role == UserRoles.VehicleOperator)
                {
                    var expectedLicenseType = model.Role == UserRoles.PropertyOperator ? "realEstate" : "carSale";
                    var hasMatchingLicense = company.LicenseDetails.Any(l => l.LicenseType == expectedLicenseType);

                    if (!hasMatchingLicense)
                    {
                        return BadRequest(new { message = $"این رهنما جواز {(expectedLicenseType == "realEstate" ? "املاک" : "موترفروشی")} ندارد" });
                    }

                    // Set license type from company
                    model.LicenseType = expectedLicenseType;
                }

                // Automatically set provinceId from company if not already set
                if (!model.ProvinceId.HasValue && company.ProvinceId.HasValue)
                {
                    model.ProvinceId = company.ProvinceId;
                }
            }

            // Get current user for CreatedBy
            string createdBy = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "system";

            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhotoPath = model.PhotoPath,
                PhoneNumber = model.PhoneNumber?.ToString(),
                CompanyId = model.CompanyId,
                LicenseType = model.LicenseType,
                UserRole = model.Role,
                IsAdmin = model.Role == UserRoles.Admin,
                ProvinceId = model.ProvinceId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            try
            {
                // Ensure role exists
                if (!string.IsNullOrEmpty(model.Role) && !await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    
                    // Add permissions to role
                    var role = await _roleManager.FindByNameAsync(model.Role);
                    if (role != null)
                    {
                        var permissions = RolePermissions.GetPermissionsForRole(model.Role);
                        foreach (var permission in permissions)
                        {
                            await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
                        }
                    }
                }

                var result = await _userManager.CreateAsync(applicationUser, model.Password ?? "");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, model.Role ?? "USER");
                    return Ok(new { 
                        succeeded = true, 
                        userId = applicationUser.Id,
                        message = "User created successfully"
                    });
                }
                return BadRequest(new { succeeded = false, errors = result.Errors });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "Error creating user", error = innerMessage, details = ex.ToString() });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName ?? "");
               
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password ?? ""))
                {
                    if (user.IsLocked.Equals(true))
                    {
                        return StatusCode(418, new { message = "حساب کاربری قفل شده است" });
                    }

                    // Get role assigned to the user
                    var roles = await _userManager.GetRolesAsync(user);
                    var primaryRole = roles.FirstOrDefault() ?? user.UserRole ?? "USER";

                    // Load permissions: prefer live DB claims, fall back to hardcoded defaults
                    string[] permissions;
                    var roleEntity = await _roleManager.FindByNameAsync(primaryRole);
                    if (roleEntity != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
                        var livePermissions = roleClaims
                            .Where(c => c.Type == "permission")
                            .Select(c => c.Value)
                            .Distinct()
                            .ToArray();
                        permissions = livePermissions.Length > 0
                            ? livePermissions
                            : RolePermissions.GetPermissionsForRole(primaryRole);
                    }
                    else
                    {
                        permissions = RolePermissions.GetPermissionsForRole(primaryRole);
                    }

                    var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
                    var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);

                    // Build custom claims — permissions only from our resolved list (no stale role claims)
                    var customClaims = new List<Claim>
                    {
                        new Claim("UserID", user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""),
                        new Claim("companyId", user.CompanyId.ToString()),
                        new Claim("licenseType", user.LicenseType ?? ""),
                        new Claim("userRole", primaryRole),
                        new Claim("isViewOnly", (primaryRole == UserRoles.Authority || primaryRole == UserRoles.LicenseReviewer).ToString().ToLower())
                    };

                    // Add province claim for COMPANY_REGISTRAR users
                    if (primaryRole == UserRoles.CompanyRegistrar && user.ProvinceId.HasValue)
                        customClaims.Add(new Claim(CustomClaimTypes.ProvinceId, user.ProvinceId.Value.ToString()));

                    // Add permissions as lowercase "permission" claims — deduplicated
                    foreach (var permission in permissions.Distinct())
                        customClaims.Add(new Claim("permission", permission));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        // Only use customClaims + userRoles — exclude allRoleClaims to avoid stale "Permission" duplicates
                        Subject = new ClaimsIdentity(customClaims.Union(userClaims).Union(userRoles)),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret ?? "")), 
                            SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    return Ok(new { 
                        token,
                        role = primaryRole,
                        roleDari = UserRoles.GetDariName(primaryRole),
                        permissions,
                        companyId = user.CompanyId,
                        licenseType = user.LicenseType,
                        isViewOnly = primaryRole == UserRoles.Authority || primaryRole == UserRoles.LicenseReviewer
                    });
                }
                else
                    return BadRequest(new { message = "نام کاربری یا رمز عبور اشتباه است" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ورود به سیستم", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(LoginModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.Password ?? "", model.NewPassword ?? "");
                if (result.Succeeded)
                {
                    await _singInManager.RefreshSignInAsync(user);
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { message = "رمز عبور فعلی اشتباه است" });
                }
            }
            else
                return BadRequest(new { message = "کاربر یافت نشد" });
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName ?? "");
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword ?? "");
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("UserInfo")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserInfos()
        {
            var userInfos = await _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserInfo { UserId = u.Id.ToString(), UserName = u.UserName ?? "" })
                .ToListAsync();

            return userInfos;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("LockUser")]
        public async Task<IActionResult> LockUser([FromBody] LockUserModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            user.IsLocked = model.IsLooked;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = model.IsLooked ? "حساب کاربری قفل شد" : "حساب کاربری باز شد" });
        }

        [HttpGet]
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? search = null,
            [FromQuery] string? userType = null,  // "system" | "company"
            [FromQuery] string? role = null,
            [FromQuery] string? status = null,    // "active" | "inactive"
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15)
        {
            var companyRoles = new[] { UserRoles.PropertyOperator, UserRoles.VehicleOperator };

            var query = _userManager.Users.AsQueryable();

            // User type filter
            if (userType == "company")
                query = query.Where(u => u.UserRole == UserRoles.PropertyOperator || u.UserRole == UserRoles.VehicleOperator);
            else if (userType == "system")
                query = query.Where(u => u.UserRole != UserRoles.PropertyOperator && u.UserRole != UserRoles.VehicleOperator);

            // Role filter
            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.UserRole == role);

            // Status filter
            if (status == "active")
                query = query.Where(u => !u.IsLocked);
            else if (status == "inactive")
                query = query.Where(u => u.IsLocked);

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(s)) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(s)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(s)) ||
                    (u.Email != null && u.Email.ToLower().Contains(s)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(s)));
            }

            var total = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userList = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault() ?? user.UserRole ?? "";
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.CompanyId,
                    user.LicenseType,
                    user.IsLocked,
                    user.PhotoPath,
                    user.CreatedAt,
                    Role = primaryRole,
                    RoleDari = UserRoles.GetDariName(primaryRole),
                    IsCompanyUser = companyRoles.Contains(primaryRole)
                });
            }

            return Ok(new { total, page, pageSize, users = userList });
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = new List<object>();
            foreach (var r in UserRoles.AllRoles)
            {
                var role = await _roleManager.FindByNameAsync(r);
                string[] permissions;
                if (role != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);

                    // Only trust claims saved by our UpdateRolePermissions endpoint,
                    // which stores them with lowercase "permission" type.
                    // Old seeded claims used "Permission" (capital P) and are stale.
                    var newStyleClaims = claims
                        .Where(c => c.Type == "permission")
                        .Select(c => c.Value)
                        .Distinct()
                        .ToArray();

                    permissions = newStyleClaims.Length > 0
                        ? newStyleClaims
                        : RolePermissions.GetPermissionsForRole(r);
                }
                else
                {
                    permissions = RolePermissions.GetPermissionsForRole(r);
                }
                result.Add(new { Id = r, Name = r, Dari = UserRoles.GetDariName(r), Permissions = permissions });
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("GetRolePermissions/{roleName}")]
        public async Task<IActionResult> GetRolePermissions(string roleName)
        {
            if (!UserRoles.AllRoles.Contains(roleName))
                return BadRequest(new { message = "Invalid role" });

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                // Role not yet in DB — return defaults
                return Ok(new { role = roleName, permissions = RolePermissions.GetPermissionsForRole(roleName) });
            }

            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .Distinct()
                .ToArray();

            if (permissions.Length == 0)
                permissions = RolePermissions.GetPermissionsForRole(roleName);

            return Ok(new { role = roleName, permissions });
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("UpdateRolePermissions")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionsModel model)
        {
            if (!UserRoles.AllRoles.Contains(model.RoleName))
                return BadRequest(new { message = "Invalid role" });

            // Protect ADMIN role from being modified
            if (model.RoleName == UserRoles.Admin)
                return BadRequest(new { message = "نقش مدیر سیستم قابل تغییر نیست" });

            // Ensure role exists in DB
            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
                return NotFound(new { message = "نقش یافت نشد" });

            // Remove all existing permission claims (both old "Permission" and new "permission" types)
            var existingClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in existingClaims.Where(c =>
                c.Type == "permission" || c.Type == CustomClaimTypes.Permission))
                await _roleManager.RemoveClaimAsync(role, claim);

            // Add new permission claims
            foreach (var permission in model.Permissions.Distinct())
                await _roleManager.AddClaimAsync(role, new Claim("permission", permission));

            return Ok(new { message = "صلاحیت‌های نقش با موفقیت به‌روز شد", role = model.RoleName, permissions = model.Permissions });
        }

        public class UpdateRolePermissionsModel
        {
            public string RoleName { get; set; } = "";
            public List<string> Permissions { get; set; } = new();
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            // Remove existing roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Ensure new role exists
            if (!await _roleManager.RoleExistsAsync(model.NewRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.NewRole));
            }

            // Add new role
            await _userManager.AddToRoleAsync(user, model.NewRole);
            
            // Update user properties
            user.UserRole = model.NewRole;
            user.IsAdmin = model.NewRole == UserRoles.Admin;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "نقش کاربر با موفقیت تغییر کرد" });
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("CreateCompanyUser")]
        public async Task<IActionResult> CreateCompanyUser([FromBody] CreateCompanyUserModel model)
        {
            // Validate license type
            if (model.LicenseType != "realEstate" && model.LicenseType != "carSale")
            {
                return BadRequest(new { message = "نوع جواز نامعتبر است" });
            }

            // Determine role based on license type
            var role = model.LicenseType == "realEstate" ? UserRoles.PropertyOperator : UserRoles.VehicleOperator;

            string createdBy = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "system";

            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                CompanyId = model.CompanyId,
                LicenseType = model.LicenseType,
                UserRole = role,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            try
            {
                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    var roleEntity = await _roleManager.FindByNameAsync(role);
                    if (roleEntity != null)
                    {
                        var permissions = RolePermissions.GetPermissionsForRole(role);
                        foreach (var permission in permissions)
                        {
                            await _roleManager.AddClaimAsync(roleEntity, new Claim(CustomClaimTypes.Permission, permission));
                        }
                    }
                }

                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, role);
                    return Ok(new { 
                        succeeded = true, 
                        userId = applicationUser.Id,
                        role = role,
                        message = "کاربر رهنما با موفقیت ایجاد شد"
                    });
                }
                return BadRequest(new { succeeded = false, errors = result.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ایجاد کاربر", error = ex.Message });
            }
        }

        public class LockUserModel
        {
            public string UserName { get; set; } = "";
            public bool IsLooked { get; set; }
        }

        public class UserInfo
        {
            public string UserId { get; set; } = "";
            public string UserName { get; set; } = "";
        }

        public class UpdateUserRoleModel
        {
            public string UserId { get; set; } = "";
            public string NewRole { get; set; } = "";
        }

        public class CreateCompanyUserModel
        {
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string? PhoneNumber { get; set; }
            public int CompanyId { get; set; }
            public string LicenseType { get; set; } = "";
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            // Validate province requirement for COMPANY_REGISTRAR
            if (!string.IsNullOrEmpty(model.Role) && model.Role == UserRoles.CompanyRegistrar && !model.ProvinceId.HasValue)
            {
                return BadRequest(new { message = "Province is required for COMPANY_REGISTRAR role" });
            }

            // Validate province exists if provided
            if (model.ProvinceId.HasValue)
            {
                var provinceExists = await _context.Locations.AnyAsync(l => l.Id == model.ProvinceId.Value);
                if (!provinceExists)
                {
                    return BadRequest(new { message = "Invalid province" });
                }
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.CompanyId = model.CompanyId;
            user.LicenseType = model.LicenseType;
            user.ProvinceId = model.ProvinceId;

            // Update role if changed
            if (!string.IsNullOrEmpty(model.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var currentRole = currentRoles.FirstOrDefault();
                
                if (currentRole != model.Role)
                {
                    // Remove existing roles
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    // Ensure new role exists
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

                    // Add new role
                    await _userManager.AddToRoleAsync(user, model.Role);
                    user.UserRole = model.Role;
                    user.IsAdmin = model.Role == UserRoles.Admin;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "معلومات کاربر با موفقیت تغییر کرد" });
            }

            return BadRequest(new { message = "خطا در تغییر معلومات کاربر", errors = result.Errors });
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("ToggleUserStatus")]
        public async Task<IActionResult> ToggleUserStatus([FromBody] ToggleUserStatusModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            user.IsLocked = model.IsLocked;
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                return Ok(new { 
                    message = model.IsLocked ? "حساب کاربری غیرفعال شد" : "حساب کاربری فعال شد",
                    isLocked = user.IsLocked
                });
            }

            return BadRequest(new { message = "خطا در تغییر وضعیت کاربر" });
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("GetUser/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Province)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? user.UserRole ?? "";

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.CompanyId,
                user.LicenseType,
                user.IsLocked,
                user.PhotoPath,
                user.CreatedAt,
                user.ProvinceId,
                Province = user.Province != null ? new { user.Province.Id, user.Province.Name, user.Province.Dari } : null,
                Role = role,
                RoleDari = UserRoles.GetDariName(role)
            });
        }

        public class UpdateUserModel
        {
            public string UserId { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public string? PhoneNumber { get; set; }
            public int CompanyId { get; set; }
            public string? LicenseType { get; set; }
            public string? Role { get; set; }
            public int? ProvinceId { get; set; }
        }

        public class ToggleUserStatusModel
        {
            public string UserId { get; set; } = "";
            public bool IsLocked { get; set; }
        }
    }
}
