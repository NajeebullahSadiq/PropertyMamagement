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
    /// Controller for Company License Cancellation/Revocation (فسخ / لغوه)
    /// </summary>
    [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyCancellationInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompanyCancellationInfoController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get cancellation info by company ID
        /// </summary>
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetByCompanyId(int companyId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var companyExists = await _context.CompanyDetails.AnyAsync(c => c.Id == companyId);
                if (!companyExists)
                {
                    return NotFound(new { message = "Company not found" });
                }

                CompanyCancellationInfo? cancellationInfo = null;
                try
                {
                    cancellationInfo = await _context.CompanyCancellationInfos
                        .Where(x => x.CompanyId == companyId)
                        .FirstOrDefaultAsync();
                }
                catch (Exception)
                {
                    // Table might not exist yet, return null
                    return Ok(null);
                }

                if (cancellationInfo == null)
                {
                    return Ok(null);
                }

                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var licenseCancellationLetterDateFormatted = cancellationInfo.LicenseCancellationLetterDate.HasValue
                    ? DateConversionHelper.FormatDateOnly(cancellationInfo.LicenseCancellationLetterDate, calendar)
                    : "";

                var result = new
                {
                    cancellationInfo.Id,
                    cancellationInfo.CompanyId,
                    cancellationInfo.LicenseCancellationLetterNumber,
                    cancellationInfo.RevenueCancellationLetterNumber,
                    cancellationInfo.LicenseCancellationLetterDate,
                    licenseCancellationLetterDateFormatted,
                    cancellationInfo.Remarks,
                    cancellationInfo.CreatedAt,
                    cancellationInfo.CreatedBy,
                    cancellationInfo.Status
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Create new cancellation info record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CompanyCancellationInfoData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var companyExists = await _context.CompanyDetails.AnyAsync(c => c.Id == request.CompanyId);
            if (!companyExists)
            {
                return NotFound(new { message = "Company not found" });
            }

            var existingInfo = await _context.CompanyCancellationInfos
                .AnyAsync(x => x.CompanyId == request.CompanyId);
            if (existingInfo)
            {
                return BadRequest(new { message = "Cancellation info already exists for this company. Use PUT to update." });
            }

            if (!string.IsNullOrEmpty(request.LicenseCancellationLetterNumber) && request.LicenseCancellationLetterNumber.Length > 100)
            {
                return BadRequest(new { errors = new { licenseCancellationLetterNumber = new[] { "License cancellation letter number cannot exceed 100 characters" } } });
            }
            if (!string.IsNullOrEmpty(request.RevenueCancellationLetterNumber) && request.RevenueCancellationLetterNumber.Length > 100)
            {
                return BadRequest(new { errors = new { revenueCancellationLetterNumber = new[] { "Revenue cancellation letter number cannot exceed 100 characters" } } });
            }
            if (!string.IsNullOrEmpty(request.Remarks) && request.Remarks.Length > 1000)
            {
                return BadRequest(new { errors = new { remarks = new[] { "Remarks cannot exceed 1000 characters" } } });
            }

            var userId = userIdClaim.Value;

            var cancellationInfo = new CompanyCancellationInfo
            {
                CompanyId = request.CompanyId,
                LicenseCancellationLetterNumber = request.LicenseCancellationLetterNumber,
                RevenueCancellationLetterNumber = request.RevenueCancellationLetterNumber,
                LicenseCancellationLetterDate = request.LicenseCancellationLetterDate,
                Remarks = request.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                Status = true
            };

            _context.CompanyCancellationInfos.Add(cancellationInfo);
            await _context.SaveChangesAsync();

            return Ok(new { Id = cancellationInfo.Id });
        }

        /// <summary>
        /// Update existing cancellation info record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyCancellationInfoData request)
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

            var existingInfo = await _context.CompanyCancellationInfos.FindAsync(id);
            if (existingInfo == null)
            {
                return NotFound(new { message = "Cancellation info not found" });
            }

            if (!string.IsNullOrEmpty(request.LicenseCancellationLetterNumber) && request.LicenseCancellationLetterNumber.Length > 100)
            {
                return BadRequest(new { errors = new { licenseCancellationLetterNumber = new[] { "License cancellation letter number cannot exceed 100 characters" } } });
            }
            if (!string.IsNullOrEmpty(request.RevenueCancellationLetterNumber) && request.RevenueCancellationLetterNumber.Length > 100)
            {
                return BadRequest(new { errors = new { revenueCancellationLetterNumber = new[] { "Revenue cancellation letter number cannot exceed 100 characters" } } });
            }
            if (!string.IsNullOrEmpty(request.Remarks) && request.Remarks.Length > 1000)
            {
                return BadRequest(new { errors = new { remarks = new[] { "Remarks cannot exceed 1000 characters" } } });
            }

            var createdBy = existingInfo.CreatedBy;
            var createdAt = existingInfo.CreatedAt;

            existingInfo.LicenseCancellationLetterNumber = request.LicenseCancellationLetterNumber;
            existingInfo.RevenueCancellationLetterNumber = request.RevenueCancellationLetterNumber;
            existingInfo.LicenseCancellationLetterDate = request.LicenseCancellationLetterDate;
            existingInfo.Remarks = request.Remarks;

            existingInfo.CreatedBy = createdBy;
            existingInfo.CreatedAt = createdAt;

            _context.Entry(existingInfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Id = existingInfo.Id });
        }
    }
}
