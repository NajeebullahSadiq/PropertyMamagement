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
    public class GuaranteeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public GuaranteeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var Pro = await _context.Gaurantees.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert the petitionDate to Shamsi
                PersianCalendar persianCalendar = new PersianCalendar();
                foreach (var item in Pro)
                {
                    DateOnly? gregorianDate = item.PropertyDocumentDate;
                    DateOnly? senderMaktobDategregorianDate = item.SenderMaktobDate;
                    DateOnly? answerMaktobDategregorianDate = item.AnswerdMaktobDate;
                    DateOnly? dateofguaranteeDategregorianDate = item.DateofGuarantee;
                    DateOnly? guaranteeDategregorianDate = item.GuaranteeDate;
                    if (guaranteeDategregorianDate.HasValue)
                    {
                        DateTime guaranteeDategregorianDateTime = guaranteeDategregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(guaranteeDategregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(guaranteeDategregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(guaranteeDategregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.GuaranteeDate = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
                    }
                    if (dateofguaranteeDategregorianDate.HasValue)
                    {
                        DateTime dateofguaranteeDategregorianDateTime = dateofguaranteeDategregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(dateofguaranteeDategregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(dateofguaranteeDategregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(dateofguaranteeDategregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.DateofGuarantee = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
                    }
                    if (answerMaktobDategregorianDate.HasValue)
                    {
                        DateTime answerMaktobDategregorianDateTime = answerMaktobDategregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(answerMaktobDategregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(answerMaktobDategregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(answerMaktobDategregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.AnswerdMaktobDate = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
                    }
                    if (senderMaktobDategregorianDate.HasValue)
                    {
                        DateTime senderMaktobDategregorianDateTime = senderMaktobDategregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(senderMaktobDategregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(senderMaktobDategregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(senderMaktobDategregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.SenderMaktobDate = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
                    }
                    if (gregorianDate.HasValue)
                    {
                        DateTime gregorianDateTime = gregorianDate.Value.ToDateTime(TimeOnly.MinValue);

                        int shamsiYear = persianCalendar.GetYear(gregorianDateTime);
                        int shamsiMonth = persianCalendar.GetMonth(gregorianDateTime);
                        int shamsiDay = persianCalendar.GetDayOfMonth(gregorianDateTime);

                        // Assign the converted Shamsi date back to the original property
                        item.PropertyDocumentDate = new DateOnly(shamsiYear, shamsiMonth, shamsiDay);
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

            DateOnly PropertyDocumentDate;
            var userId = userIdClaim.Value;

            var persianCalendar = new System.Globalization.PersianCalendar();
            var persianYear = int.Parse(request.PropertyDocumentDate.Substring(0, 4));
            var persianMonth = int.Parse(request.PropertyDocumentDate.Substring(5, 2));
            var persianDay = int.Parse(request.PropertyDocumentDate.Substring(8, 2));
            var gregorianDate = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0);
            PropertyDocumentDate = DateOnly.FromDateTime(gregorianDate);

            //SenderMaktobDate
            DateOnly SenderMaktobDate;
            var persianCalendarSenderMaktobDate = new System.Globalization.PersianCalendar();
            var persianYearSenderMaktobDate = int.Parse(request.SenderMaktobDate.Substring(0, 4));
            var persianMonthSenderMaktobDate = int.Parse(request.SenderMaktobDate.Substring(5, 2));
            var persianDaySenderMaktobDate = int.Parse(request.SenderMaktobDate.Substring(8, 2));
            var gregorianDateSenderMaktobDate = persianCalendarSenderMaktobDate.ToDateTime(persianYearSenderMaktobDate, persianMonthSenderMaktobDate, persianDaySenderMaktobDate, 0, 0, 0, 0);
            SenderMaktobDate = DateOnly.FromDateTime(gregorianDateSenderMaktobDate);
            //End

            //DateofGuarantee
            DateOnly DateofGuarantee;
            var persianCalendarDateofGuarantee = new System.Globalization.PersianCalendar();
            var persianYearDateofGuarantee = int.Parse(request.DateofGuarantee.Substring(0, 4));
            var persianMonthDateofGuarantee = int.Parse(request.DateofGuarantee.Substring(5, 2));
            var persianDayDateofGuarantee = int.Parse(request.DateofGuarantee.Substring(8, 2));
            var gregorianDateDateofGuarantee = persianCalendarDateofGuarantee.ToDateTime(persianYearDateofGuarantee, persianMonthDateofGuarantee, persianDayDateofGuarantee, 0, 0, 0, 0);
            DateofGuarantee = DateOnly.FromDateTime(gregorianDateDateofGuarantee);
            //End

            //AnswerdMaktobDate
            DateOnly AnswerdMaktobDate;
            var persianCalendarAnswerdMaktobDate = new System.Globalization.PersianCalendar();
            var persianYearAnswerdMaktobDate = int.Parse(request.AnswerdMaktobDate.Substring(0, 4));
            var persianMonthAnswerdMaktobDate = int.Parse(request.AnswerdMaktobDate.Substring(5, 2));
            var persianDayAnswerdMaktobDate = int.Parse(request.AnswerdMaktobDate.Substring(8, 2));
            var gregorianDateAnswerdMaktobDate = persianCalendarAnswerdMaktobDate.ToDateTime(persianYearAnswerdMaktobDate, persianMonthAnswerdMaktobDate, persianDayAnswerdMaktobDate, 0, 0, 0, 0);
            AnswerdMaktobDate = DateOnly.FromDateTime(gregorianDateAnswerdMaktobDate);
            //End

            //AnswerdMaktobDate
            DateOnly GuaranteeDate;
            var persianCalendarGuaranteeDate = new System.Globalization.PersianCalendar();
            var persianYearGuaranteeDate = int.Parse(request.GuaranteeDate.Substring(0, 4));
            var persianMonthGuaranteeDate = int.Parse(request.GuaranteeDate.Substring(5, 2));
            var persianDayGuaranteeDate = int.Parse(request.GuaranteeDate.Substring(8, 2));
            var gregorianDateGuaranteeDate = persianCalendarGuaranteeDate.ToDateTime(persianYearGuaranteeDate, persianMonthGuaranteeDate, persianDayGuaranteeDate, 0, 0, 0, 0);
            GuaranteeDate = DateOnly.FromDateTime(gregorianDateGuaranteeDate);
            //End
            var property = new Gaurantee
            {
                GuaranteeTypeId = request.GuaranteeTypeId,
                PropertyDocumentNumber = request.PropertyDocumentNumber,
                PropertyDocumentDate = PropertyDocumentDate,
                SenderMaktobNumber = request.SenderMaktobNumber,
                SenderMaktobDate = SenderMaktobDate, // Convert DateOnly to string
                AnswerdMaktobNumber=request.AnswerdMaktobNumber,
                DateofGuarantee= DateofGuarantee,
                AnswerdMaktobDate = AnswerdMaktobDate,
                GuaranteeDocNumber =request.GuaranteeDocNumber,
                GuaranteeDate= GuaranteeDate,
                CompanyId =request.CompanyId,
                DocPath=request.DocPath,
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
            DateOnly PropertyDocumentDate;

            var persianCalendar = new System.Globalization.PersianCalendar();
            var persianYear = int.Parse(request.PropertyDocumentDate.Substring(0, 4));
            var persianMonth = int.Parse(request.PropertyDocumentDate.Substring(5, 2));
            var persianDay = int.Parse(request.PropertyDocumentDate.Substring(8, 2));
            var gregorianDate = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, 0, 0, 0, 0);
            PropertyDocumentDate = DateOnly.FromDateTime(gregorianDate);

            //SenderMaktobDate
            DateOnly SenderMaktobDate;
            var persianCalendarSenderMaktobDate = new System.Globalization.PersianCalendar();
            var persianYearSenderMaktobDate = int.Parse(request.SenderMaktobDate.Substring(0, 4));
            var persianMonthSenderMaktobDate = int.Parse(request.SenderMaktobDate.Substring(5, 2));
            var persianDaySenderMaktobDate = int.Parse(request.SenderMaktobDate.Substring(8, 2));
            var gregorianDateSenderMaktobDate = persianCalendarSenderMaktobDate.ToDateTime(persianYearSenderMaktobDate, persianMonthSenderMaktobDate, persianDaySenderMaktobDate, 0, 0, 0, 0);
            SenderMaktobDate = DateOnly.FromDateTime(gregorianDateSenderMaktobDate);
            //End

            //DateofGuarantee
            DateOnly DateofGuarantee;
            var persianCalendarDateofGuarantee = new System.Globalization.PersianCalendar();
            var persianYearDateofGuarantee = int.Parse(request.DateofGuarantee.Substring(0, 4));
            var persianMonthDateofGuarantee = int.Parse(request.DateofGuarantee.Substring(5, 2));
            var persianDayDateofGuarantee = int.Parse(request.DateofGuarantee.Substring(8, 2));
            var gregorianDateDateofGuarantee = persianCalendarDateofGuarantee.ToDateTime(persianYearDateofGuarantee, persianMonthDateofGuarantee, persianDayDateofGuarantee, 0, 0, 0, 0);
            DateofGuarantee = DateOnly.FromDateTime(gregorianDateDateofGuarantee);
            //End

            //AnswerdMaktobDate
            DateOnly AnswerdMaktobDate;
            var persianCalendarAnswerdMaktobDate = new System.Globalization.PersianCalendar();
            var persianYearAnswerdMaktobDate = int.Parse(request.AnswerdMaktobDate.Substring(0, 4));
            var persianMonthAnswerdMaktobDate = int.Parse(request.AnswerdMaktobDate.Substring(5, 2));
            var persianDayAnswerdMaktobDate = int.Parse(request.AnswerdMaktobDate.Substring(8, 2));
            var gregorianDateAnswerdMaktobDate = persianCalendarAnswerdMaktobDate.ToDateTime(persianYearAnswerdMaktobDate, persianMonthAnswerdMaktobDate, persianDayAnswerdMaktobDate, 0, 0, 0, 0);
            AnswerdMaktobDate = DateOnly.FromDateTime(gregorianDateAnswerdMaktobDate);
            //End

            //AnswerdMaktobDate
            DateOnly GuaranteeDate;
            var persianCalendarGuaranteeDate = new System.Globalization.PersianCalendar();
            var persianYearGuaranteeDate = int.Parse(request.GuaranteeDate.Substring(0, 4));
            var persianMonthGuaranteeDate = int.Parse(request.GuaranteeDate.Substring(5, 2));
            var persianDayGuaranteeDate = int.Parse(request.GuaranteeDate.Substring(8, 2));
            var gregorianDateGuaranteeDate = persianCalendarGuaranteeDate.ToDateTime(persianYearGuaranteeDate, persianMonthGuaranteeDate, persianDayGuaranteeDate, 0, 0, 0, 0);
            GuaranteeDate = DateOnly.FromDateTime(gregorianDateGuaranteeDate);
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

            var existingProperty = await _context.Gaurantees.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId = existingProperty.CompanyId;

            // Update the entity with the new values
                existingProperty.GuaranteeTypeId = request.GuaranteeTypeId;
                existingProperty.PropertyDocumentNumber = request.PropertyDocumentNumber;
                existingProperty.PropertyDocumentDate = PropertyDocumentDate;
                existingProperty.SenderMaktobNumber = request.SenderMaktobNumber;
                existingProperty.SenderMaktobDate = SenderMaktobDate; // Convert DateOnly to string
                existingProperty.AnswerdMaktobNumber = request.AnswerdMaktobNumber;
                existingProperty.DateofGuarantee = DateofGuarantee;
                existingProperty.AnswerdMaktobDate = AnswerdMaktobDate;
                existingProperty.GuaranteeDocNumber = request.GuaranteeDocNumber;
                existingProperty.GuaranteeDate = GuaranteeDate;
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
