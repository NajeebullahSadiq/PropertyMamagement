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
      // [Authorize(Policy = PolicyTypes.Users.View)]
        //GET : /api/UserProfile
        public async Task<Object> GetUserProfile()
        {
            
            List<string> ClaimValue = new List<string>();
            List<string> ClaimType = new List<string>();
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var userroles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userroles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach(var c in roleClaims)
                {
                    ClaimValue.Add(c.Value);
                    ClaimType.Add(c.Type);
                }
              //UserClaims Table
              //  var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                //foreach (var c in userClaims)
                //{
                //    ClaimValue.Add(c.Value);
                //    ClaimType.Add(c.Type);
                //}
                return new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.UserName,
                    user.PhotoPath,
                    user.IsLocked,
                    userroles,
                    ClaimValue,
                    ClaimType,
                   // roleClaims

                };
            }
            return StatusCode(420);

        }

        //[HttpGet]
        
        //[Route("getProfile")]
        //public async Task<Object> GetCurrentUserProfile()
        //{

        //    string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //    var user = await _userManager.FindByIdAsync(userId);
        //    var userroles = await _userManager.GetRolesAsync(user);
        //    return new
        //    {

        //        user.Email,
        //        user.UserName,
        //        user.Id,
        //        userroles,
        //        user.FirstName,
        [HttpGet]
        [Route("getProfile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            try
            {
                var userProfile = await _context.UserProfileWithCompany
                                .Where(u => u.UserId == userId)
                                .FirstOrDefaultAsync();

                if (userProfile != null)
                {
                    return Ok(new
                    {
                        userProfile.Email,
                        userProfile.UserName,
                        userProfile.UserId,
                        userProfile.FirstName,
                        userProfile.LastName,
                        userProfile.PhotoPath,
                        CompanyName = userProfile.CompanyName,
                        userProfile.PhoneNumber
                    });
                }

                // If UserProfileWithCompany not found, try to get from ApplicationUser
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    return Ok(new
                    {
                        Email = user.Email ?? string.Empty,
                        UserName = user.UserName ?? string.Empty,
                        UserId = user.Id,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        PhotoPath = user.PhotoPath ?? string.Empty,
                        CompanyName = string.Empty,
                        PhoneNumber = user.PhoneNumber ?? string.Empty
                    });
                }

                // If user not found in either table, return 404
                return NotFound(new
                {
                    message = "User profile not found"
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

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("ForAdmin")]
        public string GetForAdmin()
        {
            return "Web method for Admin";
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("ForCustomer")]
        public string GetCustomer()
        {
            return "Web method for Customer";
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        [Route("ForAdminOrCustomer")]
        public string GetForAdminOrCustomer()
        {
            return "Web method for Admin or Customer";
        }


        [HttpGet]
        [Route("Role")]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
        {
            var roles = await _context.Roles.OrderBy(r => r.Id).ToListAsync();
            return roles;

        }
    }
}