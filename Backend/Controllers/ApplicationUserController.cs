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

            // Validate company operators must have company and license type
            if ((model.Role == UserRoles.PropertyOperator || model.Role == UserRoles.VehicleOperator) 
                && model.CompanyId == 0)
            {
                return BadRequest(new { message = "Company operators must be associated with a company" });
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
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            try
            {
                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    
                    // Add permissions to role
                    var role = await _roleManager.FindByNameAsync(model.Role);
                    var permissions = RolePermissions.GetPermissionsForRole(model.Role);
                    foreach (var permission in permissions)
                    {
                        await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
                    }
                }

                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, model.Role);
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
                var user = await _userManager.FindByNameAsync(model.UserName);
               
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (user.IsLocked.Equals(true))
                    {
                        return StatusCode(418, new { message = "حساب کاربری قفل شده است" });
                    }

                    // Get role assigned to the user
                    var roles = await _userManager.GetRolesAsync(user);
                    var primaryRole = roles.FirstOrDefault() ?? user.UserRole ?? "USER";
                    
                    // Get permissions for the role
                    var permissions = RolePermissions.GetPermissionsForRole(primaryRole);
                    
                    var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
                    var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    
                    // Collect all role claims (permissions)
                    var allRoleClaims = new List<Claim>();
                    foreach (var userRole in roles)
                    {
                        var role = await _roleManager.FindByNameAsync(userRole);
                        if (role != null)
                        {
                            var roleClaims = await _roleManager.GetClaimsAsync(role);
                            allRoleClaims.AddRange(roleClaims);
                        }
                    }

                    // Add custom claims for RBAC
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

                    // Add permissions as claims
                    foreach (var permission in permissions)
                    {
                        customClaims.Add(new Claim("permission", permission));
                    }

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(customClaims.Union(userClaims).Union(userRoles).Union(allRoleClaims)),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), 
                            SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    return Ok(new { 
                        token,
                        role = primaryRole,
                        roleDari = UserRoles.GetDariName(primaryRole),
                        permissions = permissions,
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
                var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
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
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("UserInfo")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUserInfos()
        {
            var userInfos = await _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserInfo { UserId = u.Id.ToString(), UserName = u.UserName })
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
        [Authorize(Roles = "ADMIN")]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
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
                    Role = roles.FirstOrDefault() ?? user.UserRole,
                    RoleDari = UserRoles.GetDariName(roles.FirstOrDefault() ?? user.UserRole ?? "")
                });
            }

            return Ok(userList);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("GetRoles")]
        public IActionResult GetRoles()
        {
            var roles = UserRoles.AllRoles.Select(r => new
            {
                Id = r,
                Name = r,
                Dari = UserRoles.GetDariName(r),
                Permissions = RolePermissions.GetPermissionsForRole(r)
            });

            return Ok(roles);
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
                    var permissions = RolePermissions.GetPermissionsForRole(role);
                    foreach (var permission in permissions)
                    {
                        await _roleManager.AddClaimAsync(roleEntity, new Claim(CustomClaimTypes.Permission, permission));
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
                        message = "کاربر شرکت با موفقیت ایجاد شد"
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

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.CompanyId = model.CompanyId;
            user.LicenseType = model.LicenseType;

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
            var user = await _userManager.FindByIdAsync(userId);
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
        }

        public class ToggleUserStatusModel
        {
            public string UserId { get; set; } = "";
            public bool IsLocked { get; set; }
        }
    }
}
