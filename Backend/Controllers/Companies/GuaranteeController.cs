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
    public class GuaranteeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public GuaranteeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var Pro = await _context.Gaurantees.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                foreach (var item in Pro)
                {
                    item.PropertyDocumentDate = DateConversionHelper.ToCalendarDateOnly(item.PropertyDocumentDate, calendar);
                    item.SenderMaktobDate = DateConversionHelper.ToCalendarDateOnly(item.SenderMaktobDate, calendar);
                    item.AnswerdMaktobDate = DateConversionHelper.ToCalendarDateOnly(item.AnswerdMaktobDate, calendar);
                    item.DateofGuarantee = DateConversionHelper.ToCalendarDateOnly(item.DateofGuarantee, calendar);
                    item.GuaranteeDate = DateConversionHelper.ToCalendarDateOnly(item.GuaranteeDate, calendar);
                }

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] GauranteeData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int propertyCount = _context.Gaurantees.Count(wd => wd.CompanyId == request.CompanyId && wd.CompanyId != null);
            if (propertyCount >= 1)
            {
                return StatusCode(400, "You cannot add more than one");
            }
            if (request.CompanyId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            var userId = userIdClaim.Value;

            // Parse all dates using multi-calendar helper
            DateConversionHelper.TryParseToDateOnly(request.PropertyDocumentDate, request.CalendarType, out var propertyDocumentDate);
            DateConversionHelper.TryParseToDateOnly(request.SenderMaktobDate, request.CalendarType, out var senderMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.AnswerdMaktobDate, request.CalendarType, out var answerdMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.DateofGuarantee, request.CalendarType, out var dateofGuarantee);
            DateConversionHelper.TryParseToDateOnly(request.GuaranteeDate, request.CalendarType, out var guaranteeDate);

            var property = new Gaurantee
            {
                GuaranteeTypeId = request.GuaranteeTypeId,
                PropertyDocumentNumber = request.PropertyDocumentNumber,
                PropertyDocumentDate = propertyDocumentDate,
                SenderMaktobNumber = request.SenderMaktobNumber,
                SenderMaktobDate = senderMaktobDate,
                AnswerdMaktobNumber = request.AnswerdMaktobNumber,
                DateofGuarantee = dateofGuarantee,
                AnswerdMaktobDate = answerdMaktobDate,
                GuaranteeDocNumber = request.GuaranteeDocNumber,
                GuaranteeDate = guaranteeDate,
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
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] GauranteeData request)
        {
            // Parse all dates using multi-calendar helper
            DateConversionHelper.TryParseToDateOnly(request.PropertyDocumentDate, request.CalendarType, out var propertyDocumentDate);
            DateConversionHelper.TryParseToDateOnly(request.SenderMaktobDate, request.CalendarType, out var senderMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.AnswerdMaktobDate, request.CalendarType, out var answerdMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.DateofGuarantee, request.CalendarType, out var dateofGuarantee);
            DateConversionHelper.TryParseToDateOnly(request.GuaranteeDate, request.CalendarType, out var guaranteeDate);

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

            var existingProperty = await _context.Gaurantees.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId = existingProperty.CompanyId;

            // Update the entity with the new values
            existingProperty.GuaranteeTypeId = request.GuaranteeTypeId;
            existingProperty.PropertyDocumentNumber = request.PropertyDocumentNumber;
            existingProperty.PropertyDocumentDate = propertyDocumentDate;
            existingProperty.SenderMaktobNumber = request.SenderMaktobNumber;
            existingProperty.SenderMaktobDate = senderMaktobDate;
            existingProperty.AnswerdMaktobNumber = request.AnswerdMaktobNumber;
            existingProperty.DateofGuarantee = dateofGuarantee;
            existingProperty.AnswerdMaktobDate = answerdMaktobDate;
            existingProperty.GuaranteeDocNumber = request.GuaranteeDocNumber;
            existingProperty.GuaranteeDate = guaranteeDate;
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
                    _context.Graunteeaudits.Add(new Graunteeaudit
                    {
                        GauranteeId = existingProperty.Id,
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
