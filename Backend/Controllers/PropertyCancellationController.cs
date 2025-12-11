using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;

namespace WebAPIBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyCancellationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PropertyCancellationController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("GetActiveTransactions")]
        public async Task<IActionResult> GetActiveTransactions()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                IQueryable<PropertyDetail> propertyQuery;

                if (roles.Contains("ADMIN"))
                {
                    propertyQuery = _context.PropertyDetails;
                }
                else
                {
                    propertyQuery = _context.PropertyDetails.Where(p => p.CreatedBy == userId);
                }

                var activeTransactions = await propertyQuery
                    .Where(p => !_context.PropertyCancellations.Any(c => c.PropertyDetailsId == p.Id))
                    .Include(p => p.SellerDetails)
                    .Include(p => p.BuyerDetails)
                    .Include(p => p.TransactionType)
                    .Select(p => new
                    {
                        p.Id,
                        p.Pnumber,
                        p.CreatedAt,
                        TransactionTypeName = p.TransactionType.Name,
                        SellerName = p.SellerDetails.FirstOrDefault() != null ? 
                            $"{p.SellerDetails.First().FirstName} {p.SellerDetails.First().FatherName} {p.SellerDetails.First().GrandFather}" : "",
                        BuyerName = p.BuyerDetails.FirstOrDefault() != null ? 
                            $"{p.BuyerDetails.First().FirstName} {p.BuyerDetails.First().FatherName} {p.BuyerDetails.First().GrandFather}" : "",
                        PropertyDetails = new
                        {
                            p.Pnumber,
                            p.Parea,
                            p.Des
                        }
                    })
                    .ToListAsync();

                return Ok(activeTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetCancelledTransactions")]
        public async Task<IActionResult> GetCancelledTransactions()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                IQueryable<PropertyCancellation> cancellationQuery;

                if (roles.Contains("ADMIN"))
                {
                    cancellationQuery = _context.PropertyCancellations;
                }
                else
                {
                    cancellationQuery = _context.PropertyCancellations
                        .Where(c => c.PropertyDetails.CreatedBy == userId);
                }

                var cancelledTransactions = await cancellationQuery
                    .Include(c => c.PropertyDetails)
                    .ThenInclude(p => p.SellerDetails)
                    .Include(c => c.PropertyDetails)
                    .ThenInclude(p => p.BuyerDetails)
                    .Include(c => c.PropertyDetails)
                    .ThenInclude(p => p.TransactionType)
                    .Select(c => new
                    {
                        c.Id,
                        c.PropertyDetailsId,
                        c.CancellationDate,
                        c.CancellationReason,
                        c.CancelledBy,
                        c.Status,
                        c.CreatedAt,
                        PropertyNumber = c.PropertyDetails.Pnumber,
                        TransactionTypeName = c.PropertyDetails.TransactionType.Name,
                        SellerName = c.PropertyDetails.SellerDetails.FirstOrDefault() != null ? 
                            $"{c.PropertyDetails.SellerDetails.First().FirstName} {c.PropertyDetails.SellerDetails.First().FatherName} {c.PropertyDetails.SellerDetails.First().GrandFather}" : "",
                        BuyerName = c.PropertyDetails.BuyerDetails.FirstOrDefault() != null ? 
                            $"{c.PropertyDetails.BuyerDetails.First().FirstName} {c.PropertyDetails.BuyerDetails.First().FatherName} {c.PropertyDetails.BuyerDetails.First().GrandFather}" : ""
                    })
                    .OrderByDescending(c => c.CancellationDate)
                    .ToListAsync();

                return Ok(cancelledTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CancelTransaction")]
        public async Task<IActionResult> CancelTransaction([FromBody] CancelTransactionRequest request)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var property = await _context.PropertyDetails.FindAsync(request.PropertyDetailsId);
                if (property == null)
                {
                    return NotFound(new { error = "Property not found" });
                }

                var existingCancellation = await _context.PropertyCancellations
                    .FirstOrDefaultAsync(c => c.PropertyDetailsId == request.PropertyDetailsId);

                if (existingCancellation != null)
                {
                    return BadRequest(new { error = "This transaction has already been cancelled" });
                }

                var cancellation = new PropertyCancellation
                {
                    PropertyDetailsId = request.PropertyDetailsId,
                    CancellationDate = DateTime.Now,
                    CancellationReason = request.CancellationReason,
                    CancelledBy = userId,
                    Status = "Cancelled",
                    CreatedAt = DateTime.Now
                };

                _context.PropertyCancellations.Add(cancellation);
                await _context.SaveChangesAsync();

                return Ok(new { id = cancellation.Id, message = "Transaction cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("IsCancelled/{propertyDetailsId}")]
        public async Task<IActionResult> IsCancelled(int propertyDetailsId)
        {
            try
            {
                var cancellation = await _context.PropertyCancellations
                    .FirstOrDefaultAsync(c => c.PropertyDetailsId == propertyDetailsId);

                return Ok(new { isCancelled = cancellation != null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class CancelTransactionRequest
    {
        public int PropertyDetailsId { get; set; }
        public string? CancellationReason { get; set; }
    }
}
