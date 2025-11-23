using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;


namespace WebAPIBackend.Controllers
{
    [Authorize(Roles="ADMIN")]
    [Route("api/[controller]")] // api/ClaimSetup
    [ApiController]
    public class ClaimSetupController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly ILogger<ClaimSetupController> _logger;

        public ClaimSetupController(
            AppDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ClaimSetupController> logger)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllClaims(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);

        //    var claims = await _userManager.GetClaimsAsync(user);

        //    return Ok(claims);
        //}


        [HttpGet]
        public async Task<IActionResult> GetAllClaims(string email)
        {
            // var roleClaims;
            var user = await _userManager.FindByEmailAsync(email);
            var userRoles = await _userManager.GetRolesAsync(user);
           
            // var roleclaim = await _roleManager.GetClaimsAsync(role);
            // var roleclaim = await _roleManager.GetClaimsAsync((IdentityRole)role);
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
               var roleClaims = await _roleManager.GetClaimsAsync(role);
                return Ok(roleClaims);
            }
            return BadRequest();
        }

        // Add Claim to user
        [HttpPost]
        [Route("AddClaimToUser")]
        public async Task<IActionResult> AddClaimToUser(string email, string claimName, string value)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var userClaim = new Claim(claimName, value);

            if (user != null)
            {
                var result = await _userManager.AddClaimAsync(user, userClaim);

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, $"the claim {claimName} add to the  User {user.Email}");
                    return Ok(new { result = $"the claim {claimName} add to the  User {user.Email}" });
                }
                else
                {
                    _logger.LogInformation(1, $"Error: Unable to add the claim {claimName} to the  User {user.Email}");
                    return BadRequest(new { error = $"Error: Unable to add the claim {claimName} to the  User {user.Email}" });
                }
            }

            // User doesn't exist
            return BadRequest(new { error = "Unable to find user" });
        }
    }
}
