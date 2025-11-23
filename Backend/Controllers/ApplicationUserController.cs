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
        //POST : /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhotoPath = model.PhotoPath,
                PhoneNumber=model.PhoneNumber.ToString(),
                CompanyId=model.CompanyId,

            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                await _userManager.AddToRoleAsync(applicationUser, model.Role);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            
        }


        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {

            var user = await _userManager.FindByNameAsync(model.UserName);
           
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.IsLocked.Equals(true))
                {
                    return StatusCode(418);
                }
                else
                {
                    //Get role assigned to the user
                    var roles = await _userManager.GetRolesAsync(user);
                    var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
                    var userRoles2 = await _userManager.GetRolesAsync(user);
                    var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    foreach (var userRole in userRoles2)
                    {
                        var role = await _roleManager.FindByNameAsync(userRole);
                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        //return Ok(roleClaims);

                        IdentityOptions _options = new IdentityOptions();
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.UserName)
                            }.Union(userClaims).Union(userRoles).Union(roleClaims)),
                            Expires = DateTime.UtcNow.AddDays(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                        var token = tokenHandler.WriteToken(securityToken);

                        return Ok(new { token });
                    }
                     return StatusCode(420);
                }
               
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }

        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        //POST : /api/ApplicationUser/ChangePassword
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
                    return BadRequest(new { message = "Username or password is incorrect." });
                }
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });

        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                return Ok(result);
            }
            return Ok();
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
                return NotFound();
            }

            user.IsLocked = model.IsLooked;

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }
        public class LockUserModel
        {
            public string UserName { get; set; }
            public bool IsLooked { get; set; }
        }
        public class UserInfo
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
        }
    }
}