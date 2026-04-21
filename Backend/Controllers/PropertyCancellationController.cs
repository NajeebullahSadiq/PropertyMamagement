using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetActiveTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
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
                    propertyQuery = _context.PropertyDetails.AsNoTracking();
                }
                else
                {
                    propertyQuery = _context.PropertyDetails.AsNoTracking().Where(p => p.CreatedBy == userId);
                }

                var activeQuery = propertyQuery
                    .Where(p => !_context.PropertyCancellations.Any(c => c.PropertyDetailsId == p.Id));

                var totalCount = await activeQuery.CountAsync();

                var activeTransactions = await activeQuery
                    .OrderByDescending(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        p.Id,
                        p.CreatedAt,
                        TransactionTypeName = p.TransactionType != null ? p.TransactionType.Name : "",
                        SellerName = p.SellerDetails.FirstOrDefault() != null ? 
                            $"{p.SellerDetails.First().FirstName} {p.SellerDetails.First().FatherName} {p.SellerDetails.First().GrandFather}" : "",
                        BuyerName = p.BuyerDetails.FirstOrDefault() != null ? 
                            $"{p.BuyerDetails.First().FirstName} {p.BuyerDetails.First().FatherName} {p.BuyerDetails.First().GrandFather}" : "",
                        PropertyDetails = new
                        {
                            p.Parea,
                            p.Des
                        }
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items = activeTransactions,
                    totalCount,
                    page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetCancelledTransactions")]
        public async Task<IActionResult> GetCancelledTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
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
                    cancellationQuery = _context.PropertyCancellations.AsNoTracking();
                }
                else
                {
                    cancellationQuery = _context.PropertyCancellations
                        .AsNoTracking()
                        .Where(c => c.PropertyDetails != null && c.PropertyDetails.CreatedBy == userId);
                }

                var totalCount = await cancellationQuery.CountAsync();

                var cancelledTransactions = await cancellationQuery
                    .OrderByDescending(c => c.CancellationDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        c.Id,
                        c.PropertyDetailsId,
                        c.CancellationDate,
                        c.CancellationReason,
                        c.CancelledBy,
                        c.Status,
                        c.CreatedAt,
                        TransactionTypeName = c.PropertyDetails != null && c.PropertyDetails.TransactionType != null ? c.PropertyDetails.TransactionType.Name : "",
                        PropertyNumber = c.PropertyDetails != null ? c.PropertyDetails.Pnumber : null,
                        SellerName = c.PropertyDetails != null && c.PropertyDetails.SellerDetails.FirstOrDefault() != null ? 
                            $"{c.PropertyDetails.SellerDetails.First().FirstName} {c.PropertyDetails.SellerDetails.First().FatherName} {c.PropertyDetails.SellerDetails.First().GrandFather}" : "",
                        BuyerName = c.PropertyDetails != null && c.PropertyDetails.BuyerDetails.FirstOrDefault() != null ? 
                            $"{c.PropertyDetails.BuyerDetails.First().FirstName} {c.PropertyDetails.BuyerDetails.First().FatherName} {c.PropertyDetails.BuyerDetails.First().GrandFather}" : "",
                        Documents = c.PropertyCancellationDocuments
                            .OrderByDescending(d => d.CreatedAt)
                            .Select(d => new
                            {
                                d.Id,
                                FilePath = d.FilePath,
                                FileName = d.OriginalFileName,
                                d.CreatedAt
                            })
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items = cancelledTransactions,
                    totalCount,
                    page,
                    pageSize
                });
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
                    return Unauthorized(new { error = "User ID not found in claims" });
                }

                if (string.IsNullOrWhiteSpace(request.CancellationReason))
                {
                    return BadRequest(new { error = "Cancellation reason is required" });
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
                    CancellationDate = DateTime.UtcNow,
                    CancellationReason = request.CancellationReason,
                    CancelledBy = userId,
                    Status = "Cancelled",
                    CreatedAt = DateTime.UtcNow
                };

                _context.PropertyCancellations.Add(cancellation);
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    return StatusCode(500, new { 
                        error = "Database error while saving cancellation", 
                        details = innerMessage 
                    });
                }

                var documents = request.Documents ?? new List<CancellationDocumentRequest>();
                foreach (var doc in documents)
                {
                    if (doc == null || string.IsNullOrWhiteSpace(doc.FilePath))
                    {
                        continue;
                    }

                    _context.PropertyCancellationDocuments.Add(new PropertyCancellationDocument
                    {
                        PropertyCancellationId = cancellation.Id,
                        FilePath = doc.FilePath,
                        OriginalFileName = doc.OriginalFileName,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    });
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    return StatusCode(500, new { 
                        error = "Database error while saving documents", 
                        details = innerMessage 
                    });
                }

                return Ok(new { id = cancellation.Id, message = "Transaction cancelled successfully" });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { 
                    error = "An error occurred while cancelling the transaction", 
                    details = innerMessage,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("GetCancellationDetails/{propertyDetailsId}")]
        public async Task<IActionResult> GetCancellationDetails(int propertyDetailsId)
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

                IQueryable<PropertyCancellation> cancellationQuery = _context.PropertyCancellations.AsNoTracking();

                if (!roles.Contains("ADMIN"))
                {
                    cancellationQuery = cancellationQuery.Where(c => c.PropertyDetails != null && c.PropertyDetails.CreatedBy == userId);
                }

                var cancellation = await cancellationQuery
                    .Where(c => c.PropertyDetailsId == propertyDetailsId)
                    .Select(c => new
                    {
                        c.Id,
                        c.PropertyDetailsId,
                        c.CancellationDate,
                        c.CancellationReason,
                        c.CancelledBy,
                        c.Status,
                        c.CreatedAt,
                        Documents = c.PropertyCancellationDocuments
                            .OrderByDescending(d => d.CreatedAt)
                            .Select(d => new
                            {
                                d.Id,
                                FilePath = d.FilePath,
                                FileName = d.OriginalFileName,
                                d.CreatedAt
                            })
                    })
                    .FirstOrDefaultAsync();

                if (cancellation == null)
                {
                    return NotFound(new { error = "Cancellation not found" });
                }

                return Ok(cancellation);
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
                    .AsNoTracking()
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

        public List<CancellationDocumentRequest> Documents { get; set; } = new();
    }

    public class CancellationDocumentRequest
    {
        public string FilePath { get; set; } = "";

        public string? OriginalFileName { get; set; }
    }
}
