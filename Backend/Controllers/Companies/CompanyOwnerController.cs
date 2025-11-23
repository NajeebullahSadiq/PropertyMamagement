using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPIBackend.Configuration;
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
        public async Task<IActionResult> getOwnerById(int id)
        {
            try
            {
                var Pro = await _context.CompanyOwners.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert the petitionDate to Shamsi
                PersianCalendar persianCalendar = new PersianCalendar();
                foreach (var item in Pro)
                {
                    DateOnly? gregorianDate = item.DateofBirth;

                    if (gregorianDate.HasValue)
                    {
                        DateTime gregorianDateTime = gregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(gregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(gregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(gregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.DateofBirth = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
                    }
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
            DateOnly pdate;
            var userId = userIdClaim.Value;

            var persianCalendar = new System.Globalization.PersianCalendar();
            var persianYear = int.Parse(request.DateofBirth.Substring(0, 4));
            var persianMonth = int.Parse(request.DateofBirth.Substring(5, 2));
            var persianDay = int.Parse(request.DateofBirth.Substring(8, 2));
            var gregorianDate = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0);
            pdate = DateOnly.FromDateTime(gregorianDate);
            var property = new CompanyOwner
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFatherName = request.GrandFatherName,
                EducationLevelId = request.EducationLevelId,
                DateofBirth = pdate, // Convert DateOnly to string
                IdentityCardTypeId = request.IdentityCardTypeId,
                IndentityCardNumber=request.IndentityCardNumber,
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
            var persianCalendar = new System.Globalization.PersianCalendar();
            var persianYear = int.Parse(request.DateofBirth.Substring(0, 4));
            var persianMonth = int.Parse(request.DateofBirth.Substring(5, 2));
            var persianDay = int.Parse(request.DateofBirth.Substring(8, 2));
            var gregorianDate = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0);
            DateOnly pdate;
            pdate = DateOnly.FromDateTime(gregorianDate);
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
                // Only add an entry to the vehicleaudit table if the property has been modified
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
