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
    public class CompanyDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CompanyDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var query = from p in _context.CompanyDetails
                            select new
                            {
                                p.Id,
                                p.Title,
                                p.PhoneNumber,
                                ownerFullName= (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FirstName : null,
                                ownerFatherName= (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FatherName : null,
                                ownerIdNumber = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().IndentityCardNumber : null,
                                licenseNumber= (p.LicenseDetails != null && p.LicenseDetails.Any()) ? p.LicenseDetails.First().LicenseNumber:0,
                                granator= (p.Guarantors != null && p.Guarantors.Any()) ? p.Guarantors.First().FirstName+" "+"فرزند"+" "+ p.Guarantors.First().FatherName : null,
                            };
                return Ok(query);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("getexpired")]
        public IActionResult GetExpiredLicense()
        {
            try
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now); // Current date in DateOnly format

                var query = from p in _context.CompanyDetails
                            where p.LicenseDetails.Any(l => l.ExpireDate < currentDate) // Filter out non-expired licenses
                            select new
                            {
                                p.Id,
                                p.Title,
                                p.PhoneNumber,
                                ownerFullName = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FirstName : null,
                                ownerFatherName = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FatherName : null,
                                ownerIdNumber = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().IndentityCardNumber : null,
                                licenseNumber = (p.LicenseDetails != null && p.LicenseDetails.Any()) ? p.LicenseDetails.First().LicenseNumber : 0,
                                granator = (p.Guarantors != null && p.Guarantors.Any()) ? p.Guarantors.First().FirstName + " " + "فرزند" + " " + p.Guarantors.First().FatherName : null,
                            };
                return Ok(query);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var Pro = await _context.CompanyDetails.Where(x => x.Id.Equals(id)).ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                foreach (var item in Pro)
                {
                    item.PetitionDate = DateConversionHelper.ToCalendarDateOnly(item.PetitionDate, calendar);
                }

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] CompanyDetailData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                if (request == null)
                {
                    return BadRequest("Request body is required.");
                }

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest("Title is required.");
                }

                // Parse calendar type and validate date
                if (string.IsNullOrWhiteSpace(request.PetitionDate))
                {
                    return BadRequest("PetitionDate is required.");
                }

                var calendarType = DateConversionHelper.ParseCalendarType(request.CalendarType);
                if (!DateConversionHelper.TryParseToDateOnly(request.PetitionDate!, calendarType, out var pdate))
                {
                    return BadRequest($"PetitionDate must be a valid date in {calendarType} format (yyyy-MM-dd or yyyy/MM/dd).");
                }

                var userId = userIdClaim.Value;

                var property = new CompanyDetail
                {
                    Title = request.Title,
                    PhoneNumber = request.PhoneNumber,
                    LicenseNumber = request.LicenseNumber,
                    PetitionNumber = request.PetitionNumber,
                    PetitionDate = pdate,
                    Tin = request.Tin,
                    DocPath = request.DocPath,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };

                _context.Add(property);
                await _context.SaveChangesAsync();

                var result = new { Id = property.Id };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] CompanyDetailData request)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            // Parse calendar type and validate date
            var calendarType = DateConversionHelper.ParseCalendarType(request.CalendarType);
            if (string.IsNullOrWhiteSpace(request.PetitionDate) || !DateConversionHelper.TryParseToDateOnly(request.PetitionDate!, calendarType, out var pdate))
            {
                return BadRequest($"PetitionDate is required and must be a valid date in {calendarType} format (yyyy-MM-dd or yyyy/MM/dd).");
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

            var existingProperty = await _context.CompanyDetails.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;

            // Update the entity with the new values
            existingProperty.Title = request.Title;
            existingProperty.PhoneNumber = request.PhoneNumber;
            existingProperty.LicenseNumber = request.LicenseNumber;
            existingProperty.PetitionNumber = request.PetitionNumber;
            existingProperty.PetitionDate = pdate;
            existingProperty.Tin = request.Tin;
            existingProperty.DocPath = request.DocPath;

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
                // Only add an entry to the vehicleaudit table if the property has been modified
                if (change.Value.OldValue != null && !change.Value.OldValue.Equals(change.Value.NewValue))
                {
                    _context.Companydetailsaudits.Add(new Companydetailsaudit
                    {
                        CompanyId = existingProperty.Id,
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

        [HttpGet("getCompanies")]
        public async Task<ActionResult<IEnumerable<Companies>>> GetCompanies()
        {
            try
            {
                var com = await _context.CompanyDetails
                .OrderBy(u => u.Id)
                .Select(u => new Companies { Id = u.Id, Title = u.Title })
                .ToListAsync();

                return com;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        public class Companies
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}
