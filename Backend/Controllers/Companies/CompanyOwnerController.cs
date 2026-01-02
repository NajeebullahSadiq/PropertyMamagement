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
    public class CompanyOwnerController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CompanyOwnerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getOwnerById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var Pro = await _context.CompanyOwners.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                foreach (var item in Pro)
                {
                    item.DateofBirth = DateConversionHelper.ToCalendarDateOnly(item.DateofBirth, calendar);
                }

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] CompanyOwnerData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int propertyCount = _context.CompanyOwners.Count(wd => wd.CompanyId == request.CompanyId && wd.CompanyId != null);
            if (propertyCount >= 1)
            {
                return StatusCode(400, "You cannot add more than one");
            }
            if (request.CompanyId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            var userId = userIdClaim.Value;

            // Parse date using multi-calendar helper
            if (!DateConversionHelper.TryParseToDateOnly(request.DateofBirth, request.CalendarType, out var pdate))
            {
                return BadRequest("DateofBirth must be a valid date (yyyy-MM-dd or yyyy/MM/dd).");
            }

            var property = new CompanyOwner
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFatherName = request.GrandFatherName,
                EducationLevelId = request.EducationLevelId,
                DateofBirth = pdate,
                IdentityCardTypeId = request.IdentityCardTypeId,
                IndentityCardNumber = request.IndentityCardNumber,
                Jild = request.Jild,
                Safha = request.Safha,
                CompanyId = request.CompanyId,
                SabtNumber = request.SabtNumber,
                PothoPath = request.PothoPath,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
            };

            _context.Add(property);
            await _context.SaveChangesAsync();
            var result = new { Id = property.Id };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] CompanyOwnerData request)
        {
            // Parse date using multi-calendar helper
            if (!DateConversionHelper.TryParseToDateOnly(request.DateofBirth, request.CalendarType, out var pdate))
            {
                return BadRequest("DateofBirth must be a valid date (yyyy-MM-dd or yyyy/MM/dd).");
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

            var existingProperty = await _context.CompanyOwners.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;

            // Update the entity with the new values
            existingProperty.FirstName = request.FirstName;
            existingProperty.FatherName = request.FatherName;
            existingProperty.GrandFatherName = request.GrandFatherName;
            existingProperty.EducationLevelId = request.EducationLevelId;
            existingProperty.DateofBirth = pdate;
            existingProperty.IdentityCardTypeId = request.IdentityCardTypeId;
            existingProperty.IndentityCardNumber = request.IndentityCardNumber;
            existingProperty.Jild = request.Jild;
            existingProperty.Safha = request.Safha;
            existingProperty.CompanyId = request.CompanyId;
            existingProperty.SabtNumber = request.SabtNumber;
            existingProperty.PothoPath = request.PothoPath;

            // Restore the original values of the CreatedBy and CreatedAt properties
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
                    _context.Companyowneraudits.Add(new Companyowneraudit
                    {
                        OwnerId = existingProperty.Id,
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
    }
}
