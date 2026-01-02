using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.Audit;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers.Companies
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseDetailController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LicenseDetailController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var Pro = await _context.LicenseDetails.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                foreach (var item in Pro)
                {
                    item.IssueDate = DateConversionHelper.ToCalendarDateOnly(item.IssueDate, calendar);
                    item.ExpireDate = DateConversionHelper.ToCalendarDateOnly(item.ExpireDate, calendar);
                }

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] LicenseDetailData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int propertyCount = _context.LicenseDetails.Count(wd => wd.CompanyId == request.CompanyId && wd.CompanyId != null);
            if (propertyCount >= 1)
            {
                return StatusCode(400, "You cannot add more than one");
            }
            if (request.CompanyId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            var userId = userIdClaim.Value;

            // Parse dates using multi-calendar helper
            if (!DateConversionHelper.TryParseToDateOnly(request.IssueDate, request.CalendarType, out var issueDate))
            {
                return BadRequest("IssueDate must be a valid date (yyyy-MM-dd or yyyy/MM/dd).");
            }

            if (!DateConversionHelper.TryParseToDateOnly(request.ExpireDate, request.CalendarType, out var expireDate))
            {
                return BadRequest("ExpireDate must be a valid date (yyyy-MM-dd or yyyy/MM/dd).");
            }

            var property = new LicenseDetail
            {
                LicenseNumber = request.LicenseNumber,
                IssueDate = issueDate,
                ExpireDate = expireDate,
                AreaId = request.AreaId,
                OfficeAddress = request.OfficeAddress,
                CompanyId = request.CompanyId,
                DocPath = request.DocPath,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
            };

            _context.Add(property);
            await _context.SaveChangesAsync();
            var result = new { Id = property.Id };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] LicenseDetailData request)
        {
            // Parse dates using multi-calendar helper
            if (!DateConversionHelper.TryParseToDateOnly(request.IssueDate, request.CalendarType, out var issueDate))
            {
                return BadRequest("IssueDate must be a valid date (yyyy-MM-dd or yyyy/MM/dd).");
            }

            if (!DateConversionHelper.TryParseToDateOnly(request.ExpireDate, request.CalendarType, out var expireDate))
            {
                return BadRequest("ExpireDate must be a valid date (yyyy-MM-dd or yyyy/MM/dd).");
            }

            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingProperty = await _context.LicenseDetails.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId = existingProperty.CompanyId;

            // Update the entity with the new values
            existingProperty.LicenseNumber = request.LicenseNumber;
            existingProperty.IssueDate = issueDate;
            existingProperty.ExpireDate = expireDate;
            existingProperty.AreaId = request.AreaId;
            existingProperty.OfficeAddress = request.OfficeAddress;
            existingProperty.DocPath = request.DocPath;

            // Restore the original values
            existingProperty.CreatedBy = createdBy;
            existingProperty.CreatedAt = createdAt;

            var entry = _context.Entry(existingProperty);
            entry.State = EntityState.Modified;

            var changes = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .SelectMany(e => e.Properties)
                .Where(p => p.IsModified)
                .ToDictionary(p => p.Metadata.Name, p => new
                {
                    OldValue = p.OriginalValue,
                    NewValue = p.CurrentValue
                });

            foreach (var change in changes)
            {
                if (change.Value.OldValue != null && !change.Value.OldValue.Equals(change.Value.NewValue))
                {
                    _context.Licenseaudits.Add(new Licenseaudit
                    {
                        LicenseId = existingProperty.Id,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.Now,
                        PropertyName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            var result = new { Id = request.Id };
            return Ok(result);
        }

        [HttpGet("GetLicenseView/{id}")]
        public async Task<IActionResult> GetLicenseViewById(int id, [FromQuery] string? calendarType = null)
        {
            var data = await _context.LicenseView
                .FirstOrDefaultAsync(x => x.CompanyId == id);
            if (data == null)
            {
                return NotFound();
            }

            var calendar = DateConversionHelper.ParseCalendarType(calendarType);

            // Format dates for the requested calendar
            string issueDateFormatted = data.IssueDate.HasValue 
                ? DateConversionHelper.FormatDateOnly(data.IssueDate, calendar) 
                : "";
            string expireDateFormatted = data.ExpireDate.HasValue 
                ? DateConversionHelper.FormatDateOnly(data.ExpireDate, calendar) 
                : "";
            string dateOfBirthFormatted = data.DateofBirth.HasValue 
                ? DateConversionHelper.FormatDateOnly(data.DateofBirth, calendar) 
                : "";

            var result = new
            {
                data.CompanyId,
                data.Title,
                data.PhoneNumber,
                data.Tin,
                data.FirstName,
                data.FatherName,
                data.GrandFatherName,
                data.DateofBirth,
                data.IndentityCardNumber,
                data.OwnerPhoto,
                data.LicenseNumber,
                data.OfficeAddress,
                data.IssueDate,
                data.ExpireDate,
                issueDateFormatted,
                expireDateFormatted,
                dateOfBirthFormatted,
            };

            return Ok(result);
        }
    }
}
