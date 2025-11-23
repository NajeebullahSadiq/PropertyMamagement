using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;


namespace UploadFilesServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context) => _context = context;

        //[HttpGet]
        //public ActionResult<IEnumerable<object>> GetItems()
        //{
        //    var items = new[]
        //    {
        //    new { id = "1", text = "Item 1" },
        //    new { id = "2", text = "Item 2" },
        //    new { id = "3", text = "Item 3" }
        //};

        //    return Ok(items);
        //}
        //  [Authorize(Policy = PolicyTypes.Users.EditRole)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user is null)
                {
                    return BadRequest("User object is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }

                user.Id = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
