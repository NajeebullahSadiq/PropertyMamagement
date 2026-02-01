using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers.Companies
{
    /// <summary>
    /// Controller for Company Account/Financial Information (مالیه)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyAccountInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompanyAccountInfoController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get account info by company ID
        /// Accessible by: ADMIN, AUTHORITY, COMPANY_REGISTRAR, LICENSE_REVIEWER
        /// </summary>
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetByCompanyId(int companyId, [FromQuery] string? calendarType = null)
        {
            try
            {
                // Get user info from claims
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var rolesClaim = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
                var roles = rolesClaim.Split(',').Select(r => r.Trim()).ToList();
                var licenseType = HttpContext.User.FindFirst("licenseType")?.Value ?? "";

                // Check if user can access company module
                if (!RbacHelper.CanAccessModule(roles, licenseType, "company"))
                {
                    return Forbid();
                }

                // Check if company exists
                var companyExists = await _context.CompanyDetails.AnyAsync(c => c.Id == companyId);
                if (!companyExists)
                {
                    return NotFound(new { message = "Company not found" });
                }

                CompanyAccountInfo? accountInfo = null;
                try
                {
                    accountInfo = await _context.CompanyAccountInfos
                        .Where(x => x.CompanyId == companyId)
                        .FirstOrDefaultAsync();
                }
                catch (Exception)
                {
                    // Table might not exist yet, return null
                    return Ok(null);
                }

                if (accountInfo == null)
                {
                    return Ok(null);
                }

                // Convert date to the requested calendar type
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var taxPaymentDateFormatted = accountInfo.TaxPaymentDate.HasValue
                    ? DateConversionHelper.FormatDateOnly(accountInfo.TaxPaymentDate, calendar)
                    : "";

                var result = new
                {
                    accountInfo.Id,
                    accountInfo.CompanyId,
                    accountInfo.SettlementInfo,
                    accountInfo.TaxPaymentAmount,
                    accountInfo.SettlementYear,
                    accountInfo.TaxPaymentDate,
                    taxPaymentDateFormatted,
                    accountInfo.TransactionCount,
                    accountInfo.CompanyCommission,
                    accountInfo.CreatedAt,
                    accountInfo.CreatedBy,
                    accountInfo.Status
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        /// <summary>
        /// Create new account info record
        /// Accessible by: ADMIN, COMPANY_REGISTRAR
        /// </summary>
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR")]
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CompanyAccountInfoData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            // Check if company exists
            var companyExists = await _context.CompanyDetails.AnyAsync(c => c.Id == request.CompanyId);
            if (!companyExists)
            {
                return NotFound(new { message = "Company not found" });
            }

            // Check if account info already exists for this company
            var existingInfo = await _context.CompanyAccountInfos
                .AnyAsync(x => x.CompanyId == request.CompanyId);
            if (existingInfo)
            {
                return BadRequest(new { message = "Account info already exists for this company. Use PUT to update." });
            }

            // Validate numeric fields
            if (request.TaxPaymentAmount < 0)
            {
                return BadRequest(new { errors = new { taxPaymentAmount = new[] { "Tax payment amount must be non-negative" } } });
            }
            if (request.TransactionCount.HasValue && request.TransactionCount < 0)
            {
                return BadRequest(new { errors = new { transactionCount = new[] { "Transaction count must be non-negative" } } });
            }
            if (request.CompanyCommission.HasValue && request.CompanyCommission < 0)
            {
                return BadRequest(new { errors = new { companyCommission = new[] { "Company commission must be non-negative" } } });
            }

            // Validate settlement info length
            if (!string.IsNullOrEmpty(request.SettlementInfo) && request.SettlementInfo.Length > 500)
            {
                return BadRequest(new { errors = new { settlementInfo = new[] { "Settlement info cannot exceed 500 characters" } } });
            }

            var userId = userIdClaim.Value;

            var accountInfo = new CompanyAccountInfo
            {
                CompanyId = request.CompanyId,
                SettlementInfo = request.SettlementInfo,
                TaxPaymentAmount = request.TaxPaymentAmount,
                SettlementYear = request.SettlementYear,
                TaxPaymentDate = request.TaxPaymentDate,
                TransactionCount = request.TransactionCount,
                CompanyCommission = request.CompanyCommission,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                Status = true
            };

            _context.CompanyAccountInfos.Add(accountInfo);
            await _context.SaveChangesAsync();

            return Ok(new { Id = accountInfo.Id });
        }

        /// <summary>
        /// Update existing account info record
        /// Accessible by: ADMIN, COMPANY_REGISTRAR
        /// </summary>
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyAccountInfoData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (id != request.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var existingInfo = await _context.CompanyAccountInfos.FindAsync(id);
            if (existingInfo == null)
            {
                return NotFound(new { message = "Account info not found" });
            }

            // Validate numeric fields
            if (request.TaxPaymentAmount < 0)
            {
                return BadRequest(new { errors = new { taxPaymentAmount = new[] { "Tax payment amount must be non-negative" } } });
            }
            if (request.TransactionCount.HasValue && request.TransactionCount < 0)
            {
                return BadRequest(new { errors = new { transactionCount = new[] { "Transaction count must be non-negative" } } });
            }
            if (request.CompanyCommission.HasValue && request.CompanyCommission < 0)
            {
                return BadRequest(new { errors = new { companyCommission = new[] { "Company commission must be non-negative" } } });
            }

            // Validate settlement info length
            if (!string.IsNullOrEmpty(request.SettlementInfo) && request.SettlementInfo.Length > 500)
            {
                return BadRequest(new { errors = new { settlementInfo = new[] { "Settlement info cannot exceed 500 characters" } } });
            }

            // Preserve original audit fields
            var createdBy = existingInfo.CreatedBy;
            var createdAt = existingInfo.CreatedAt;

            // Update fields
            existingInfo.SettlementInfo = request.SettlementInfo;
            existingInfo.TaxPaymentAmount = request.TaxPaymentAmount;
            existingInfo.SettlementYear = request.SettlementYear;
            existingInfo.TaxPaymentDate = request.TaxPaymentDate;
            existingInfo.TransactionCount = request.TransactionCount;
            existingInfo.CompanyCommission = request.CompanyCommission;

            // Restore original audit fields
            existingInfo.CreatedBy = createdBy;
            existingInfo.CreatedAt = createdAt;

            _context.Entry(existingInfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Id = existingInfo.Id });
        }
    }
}
