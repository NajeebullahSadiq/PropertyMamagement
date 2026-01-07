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
    public class GuaranatorController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Guarantee Type Constants
        private const int GuaranteeType_Cash = 1;          // پول نقد
        private const int GuaranteeType_ShariaDeed = 2;    // قباله شرعی
        private const int GuaranteeType_CustomaryDeed = 3; // قباله عرفی

        public GuaranatorController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Validates guarantee type and its required conditional fields
        /// </summary>
        private (bool IsValid, string ErrorMessage) ValidateGuaranteeFields(GuarantorData request)
        {
            // Guarantee type is required
            if (!request.GuaranteeTypeId.HasValue || request.GuaranteeTypeId == 0)
            {
                return (false, "نوعیت تضمین الزامی است");
            }

            // Validate guarantee type is one of the 3 allowed values
            if (request.GuaranteeTypeId < 1 || request.GuaranteeTypeId > 3)
            {
                return (false, "نوعیت تضمین نامعتبر است");
            }

            switch (request.GuaranteeTypeId)
            {
                case GuaranteeType_Cash: // پول نقد
                    if (string.IsNullOrWhiteSpace(request.BankName))
                        return (false, "بانک الزامی است");
                    if (string.IsNullOrWhiteSpace(request.DepositNumber))
                        return (false, "نمبر اویز الزامی است");
                    if (string.IsNullOrWhiteSpace(request.DepositDate))
                        return (false, "تاریخ اویز الزامی است");
                    break;

                case GuaranteeType_ShariaDeed: // قباله شرعی
                    if (string.IsNullOrWhiteSpace(request.CourtName))
                        return (false, "محکمه نوم الزامی است");
                    if (string.IsNullOrWhiteSpace(request.CollateralNumber))
                        return (false, "نمبر وثیقه الزامی است");
                    break;

                case GuaranteeType_CustomaryDeed: // قباله عرفی
                    if (string.IsNullOrWhiteSpace(request.SetSerialNumber))
                        return (false, "نمبر سریال سټه الزامی است");
                    if (!request.GuaranteeDistrictId.HasValue || request.GuaranteeDistrictId == 0)
                        return (false, "ناحیه الزامی است");
                    break;
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Clears conditional fields that don't belong to the selected guarantee type
        /// </summary>
        private void ClearIrrelevantFields(Guarantor guarantor, int? guaranteeTypeId)
        {
            switch (guaranteeTypeId)
            {
                case GuaranteeType_Cash:
                    // Clear Sharia Deed fields
                    guarantor.CourtName = null;
                    guarantor.CollateralNumber = null;
                    // Clear Customary Deed fields
                    guarantor.SetSerialNumber = null;
                    guarantor.GuaranteeDistrictId = null;
                    break;

                case GuaranteeType_ShariaDeed:
                    // Clear Cash fields
                    guarantor.BankName = null;
                    guarantor.DepositNumber = null;
                    guarantor.DepositDate = null;
                    // Clear Customary Deed fields
                    guarantor.SetSerialNumber = null;
                    guarantor.GuaranteeDistrictId = null;
                    break;

                case GuaranteeType_CustomaryDeed:
                    // Clear Cash fields
                    guarantor.BankName = null;
                    guarantor.DepositNumber = null;
                    guarantor.DepositDate = null;
                    // Clear Sharia Deed fields
                    guarantor.CourtName = null;
                    guarantor.CollateralNumber = null;
                    break;

                default:
                    // Clear all conditional fields if no valid type
                    guarantor.CourtName = null;
                    guarantor.CollateralNumber = null;
                    guarantor.SetSerialNumber = null;
                    guarantor.GuaranteeDistrictId = null;
                    guarantor.BankName = null;
                    guarantor.DepositNumber = null;
                    guarantor.DepositDate = null;
                    break;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getGuaranatorById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var Pro = await _context.Guarantors.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                foreach (var item in Pro)
                {
                    item.PropertyDocumentDate = DateConversionHelper.ToCalendarDateOnly(item.PropertyDocumentDate, calendar);
                    item.SenderMaktobDate = DateConversionHelper.ToCalendarDateOnly(item.SenderMaktobDate, calendar);
                    item.AnswerdMaktobDate = DateConversionHelper.ToCalendarDateOnly(item.AnswerdMaktobDate, calendar);
                    item.DateofGuarantee = DateConversionHelper.ToCalendarDateOnly(item.DateofGuarantee, calendar);
                    item.GuaranteeDate = DateConversionHelper.ToCalendarDateOnly(item.GuaranteeDate, calendar);
                    // Convert Cash deposit date
                    item.DepositDate = DateConversionHelper.ToCalendarDateOnly(item.DepositDate, calendar);
                }

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] GuarantorData request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            int propertyCount = _context.Guarantors.Count(wd => wd.CompanyId == request.CompanyId && wd.CompanyId != null);
            if (propertyCount >= 2)
            {
                return StatusCode(400, "You cannot add more than Two");
            }
            if (request.CompanyId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            // Parse all guarantee dates using multi-calendar helper
            DateConversionHelper.TryParseToDateOnly(request.PropertyDocumentDate, request.CalendarType, out var propertyDocumentDate);
            DateConversionHelper.TryParseToDateOnly(request.SenderMaktobDate, request.CalendarType, out var senderMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.AnswerdMaktobDate, request.CalendarType, out var answerdMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.DateofGuarantee, request.CalendarType, out var dateofGuarantee);
            DateConversionHelper.TryParseToDateOnly(request.GuaranteeDate, request.CalendarType, out var guaranteeDate);
            // Parse Cash deposit date
            DateConversionHelper.TryParseToDateOnly(request.DepositDate, request.CalendarType, out var depositDate);

            // Validate guarantee type and conditional fields
            var validation = ValidateGuaranteeFields(request);
            if (!validation.IsValid)
            {
                return BadRequest(validation.ErrorMessage);
            }

            var property = new Guarantor
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFatherName = request.GrandFatherName,
                IdentityCardTypeId = request.IdentityCardTypeId,
                CompanyId = request.CompanyId,
                IndentityCardNumber = request.IndentityCardNumber,
                Jild = request.Jild,
                Safha = request.Safha,
                SabtNumber = request.SabtNumber,
                PhoneNumber = request.PhoneNumber,
                PothoPath = request.PothoPath,
                PaddressProvinceId = request.PaddressProvinceId,
                PaddressDistrictId = request.PaddressDistrictId,
                PaddressVillage = request.PaddressVillage,
                TaddressProvinceId = request.TaddressProvinceId,
                TaddressDistrictId = request.TaddressDistrictId,
                TaddressVillage = request.TaddressVillage,
                // Guarantee fields
                GuaranteeTypeId = request.GuaranteeTypeId,
                PropertyDocumentNumber = request.PropertyDocumentNumber,
                PropertyDocumentDate = propertyDocumentDate,
                SenderMaktobNumber = request.SenderMaktobNumber,
                SenderMaktobDate = senderMaktobDate,
                AnswerdMaktobNumber = request.AnswerdMaktobNumber,
                AnswerdMaktobDate = answerdMaktobDate,
                DateofGuarantee = dateofGuarantee,
                GuaranteeDocNumber = request.GuaranteeDocNumber,
                GuaranteeDate = guaranteeDate,
                GuaranteeDocPath = request.GuaranteeDocPath,
                // Conditional fields - Sharia Deed
                CourtName = request.CourtName,
                CollateralNumber = request.CollateralNumber,
                // Conditional fields - Customary Deed
                SetSerialNumber = request.SetSerialNumber,
                GuaranteeDistrictId = request.GuaranteeDistrictId,
                // Conditional fields - Cash
                BankName = request.BankName,
                DepositNumber = request.DepositNumber,
                DepositDate = depositDate,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
            };

            // Clear fields that don't belong to the selected guarantee type
            ClearIrrelevantFields(property, request.GuaranteeTypeId);

            _context.Add(property);
            await _context.SaveChangesAsync();
            var result = new { Id = property.Id };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] GuarantorData request)
        {
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

            var existingProperty = await _context.Guarantors.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Validate guarantee type and conditional fields
            var validation = ValidateGuaranteeFields(request);
            if (!validation.IsValid)
            {
                return BadRequest(validation.ErrorMessage);
            }

            // Parse all guarantee dates using multi-calendar helper
            DateConversionHelper.TryParseToDateOnly(request.PropertyDocumentDate, request.CalendarType, out var propertyDocumentDate);
            DateConversionHelper.TryParseToDateOnly(request.SenderMaktobDate, request.CalendarType, out var senderMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.AnswerdMaktobDate, request.CalendarType, out var answerdMaktobDate);
            DateConversionHelper.TryParseToDateOnly(request.DateofGuarantee, request.CalendarType, out var dateofGuarantee);
            DateConversionHelper.TryParseToDateOnly(request.GuaranteeDate, request.CalendarType, out var guaranteeDate);
            // Parse Cash deposit date
            DateConversionHelper.TryParseToDateOnly(request.DepositDate, request.CalendarType, out var depositDate);

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId = existingProperty.CompanyId;

            // Update the entity with the new values
            existingProperty.FirstName = request.FirstName;
            existingProperty.FatherName = request.FatherName;
            existingProperty.GrandFatherName = request.GrandFatherName;
            existingProperty.IdentityCardTypeId = request.IdentityCardTypeId;
            existingProperty.IndentityCardNumber = request.IndentityCardNumber;
            existingProperty.Jild = request.Jild;
            existingProperty.Safha = request.Safha;
            existingProperty.SabtNumber = request.SabtNumber;
            existingProperty.CompanyId = companyId;
            existingProperty.PothoPath = request.PothoPath;
            existingProperty.PhoneNumber = request.PhoneNumber;
            existingProperty.PaddressProvinceId = request.PaddressProvinceId;
            existingProperty.PaddressDistrictId = request.PaddressDistrictId;
            existingProperty.PaddressVillage = request.PaddressVillage;
            existingProperty.TaddressProvinceId = request.TaddressProvinceId;
            existingProperty.TaddressDistrictId = request.TaddressDistrictId;
            existingProperty.TaddressVillage = request.TaddressVillage;
            // Guarantee fields
            existingProperty.GuaranteeTypeId = request.GuaranteeTypeId;
            existingProperty.PropertyDocumentNumber = request.PropertyDocumentNumber;
            existingProperty.PropertyDocumentDate = propertyDocumentDate;
            existingProperty.SenderMaktobNumber = request.SenderMaktobNumber;
            existingProperty.SenderMaktobDate = senderMaktobDate;
            existingProperty.AnswerdMaktobNumber = request.AnswerdMaktobNumber;
            existingProperty.AnswerdMaktobDate = answerdMaktobDate;
            existingProperty.DateofGuarantee = dateofGuarantee;
            existingProperty.GuaranteeDocNumber = request.GuaranteeDocNumber;
            existingProperty.GuaranteeDate = guaranteeDate;
            existingProperty.GuaranteeDocPath = request.GuaranteeDocPath;
            // Conditional fields - Sharia Deed
            existingProperty.CourtName = request.CourtName;
            existingProperty.CollateralNumber = request.CollateralNumber;
            // Conditional fields - Customary Deed
            existingProperty.SetSerialNumber = request.SetSerialNumber;
            existingProperty.GuaranteeDistrictId = request.GuaranteeDistrictId;
            // Conditional fields - Cash
            existingProperty.BankName = request.BankName;
            existingProperty.DepositNumber = request.DepositNumber;
            existingProperty.DepositDate = depositDate;

            // Clear fields that don't belong to the selected guarantee type
            ClearIrrelevantFields(existingProperty, request.GuaranteeTypeId);

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
                // Only add an entry to the audit table if the property has been modified
                if (change.Value.OldValue != null && !change.Value.OldValue.Equals(change.Value.NewValue))
                {
                    _context.Guarantorsaudits.Add(new Guarantorsaudit
                    {
                        GuarantorsId = existingProperty.Id,
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
