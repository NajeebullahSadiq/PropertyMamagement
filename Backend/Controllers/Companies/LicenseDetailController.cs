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
    public class LicenseDetailController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LicenseDetailController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var Pro = await _context.LicenseDetails.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert the petitionDate to Shamsi
                PersianCalendar persianCalendar = new PersianCalendar();
                foreach (var item in Pro)
                {
                    DateOnly? gregorianDate = item.IssueDate;
                    DateOnly? EXgregorianDate = item.ExpireDate;
                    if (EXgregorianDate.HasValue)
                    {
                        DateTime ExgregorianDateTime = EXgregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(ExgregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(ExgregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(ExgregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.ExpireDate = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
                    }
                    if (gregorianDate.HasValue)
                    {
                        DateTime gregorianDateTime = gregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(gregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(gregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(gregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.IssueDate = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
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

            DateOnly Issuedate;
            var userId = userIdClaim.Value;

            var persianCalendar = new System.Globalization.PersianCalendar();
            var persianYear = int.Parse(request.IssueDate.Substring(0, 4));
            var persianMonth = int.Parse(request.IssueDate.Substring(5, 2));
            var persianDay = int.Parse(request.IssueDate.Substring(8, 2));
            var gregorianDate = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0);
            Issuedate = DateOnly.FromDateTime(gregorianDate);

            //ExpireDate
            DateOnly Expiredate;
            var persianCalendarExpire = new System.Globalization.PersianCalendar();
            var persianYearExpire = int.Parse(request.ExpireDate.Substring(0, 4));
            var persianMonthExpire = int.Parse(request.ExpireDate.Substring(5, 2));
            var persianDayExpire = int.Parse(request.ExpireDate.Substring(8, 2));
            var gregorianDateExpire = persianCalendarExpire.ToDateTime(persianYearExpire, persianMonthExpire, persianDayExpire, 0, 0, 0, 0);
            Expiredate = DateOnly.FromDateTime(gregorianDateExpire);
            //End
            var property = new LicenseDetail
            {
                LicenseNumber = request.LicenseNumber,
                IssueDate = Issuedate,
                ExpireDate = Expiredate,
                AreaId = request.AreaId,
                OfficeAddress = request.OfficeAddress, // Convert DateOnly to string
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
            DateOnly Issuedate;
          

            var persianCalendar = new System.Globalization.PersianCalendar();
            var persianYear = int.Parse(request.IssueDate.Substring(0, 4));
            var persianMonth = int.Parse(request.IssueDate.Substring(5, 2));
            var persianDay = int.Parse(request.IssueDate.Substring(8, 2));
            var gregorianDate = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0);
            Issuedate = DateOnly.FromDateTime(gregorianDate);

            //ExpireDate
            DateOnly Expiredate;
            var persianCalendarExpire = new System.Globalization.PersianCalendar();
            var persianYearExpire = int.Parse(request.ExpireDate.Substring(0, 4));
            var persianMonthExpire = int.Parse(request.ExpireDate.Substring(5, 2));
            var persianDayExpire = int.Parse(request.ExpireDate.Substring(8, 2));
            var gregorianDateExpire = persianCalendarExpire.ToDateTime(persianYearExpire, persianMonthExpire, persianDayExpire, 0, 0, 0, 0);
            Expiredate = DateOnly.FromDateTime(gregorianDateExpire);
            //End
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

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId=existingProperty.CompanyId;

            // Update the entity with the new values
            existingProperty.LicenseNumber = request.LicenseNumber;
            existingProperty.IssueDate = Issuedate;
            existingProperty.ExpireDate = Expiredate;
            existingProperty.AreaId = request.AreaId;
            existingProperty.OfficeAddress = request.OfficeAddress;
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
        // LicenseDetail
        public async Task<IActionResult> GetLicenseViewById(int id)
        {
            // Call the DbContext to retrieve the data by ID
            var data = await _context.LicenseView
                .FirstOrDefaultAsync(x => x.CompanyId == id);
            if (data == null)
            {
                return NotFound(); // Return 404 if the data with the given ID is not found
            }

            // Assuming 'data.IssueDate' is a DateOnly property
            DateOnly issueDate =(DateOnly)data.IssueDate;
            DateOnly ExpireDate = (DateOnly)data.ExpireDate;
            DateOnly DateofBirth=(DateOnly)data.DateofBirth;
            // Create a DateTime instance with a specific time (midnight)
            DateTime issueDateTime = new DateTime(issueDate.Year, issueDate.Month, issueDate.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime ExpireDateTime = new DateTime(ExpireDate.Year, ExpireDate.Month, ExpireDate.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime DateofBirthDateTime = new DateTime(DateofBirth.Year, DateofBirth.Month, DateofBirth.Day, 0, 0, 0, DateTimeKind.Utc);
            var persianCalendar = new PersianCalendar();
            // Assuming 'data.IssueDate' is a DateOnly property

            string IssueDateShamsi = $"{persianCalendar.GetYear(issueDateTime)}/{persianCalendar.GetMonth(issueDateTime)}/{persianCalendar.GetDayOfMonth(issueDateTime)}";
            string ExpireDateShamsi = $"{persianCalendar.GetYear(ExpireDateTime)}/{persianCalendar.GetMonth(ExpireDateTime)}/{persianCalendar.GetDayOfMonth(ExpireDateTime)}";
            string DateofBirthShamsi = $"{persianCalendar.GetYear(DateofBirthDateTime)}/{persianCalendar.GetMonth(DateofBirthDateTime)}/{persianCalendar.GetDayOfMonth(DateofBirthDateTime)}";
            // Create a custom result object with the desired properties
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
                issueDateShamsi = IssueDateShamsi,
                expireDateShamsi=ExpireDateShamsi,
                dateofBirthShamsi=DateofBirthShamsi,
            };

            return Ok(result); // Return the data as JSON if found
        }
    }
}
