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
    [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR")]
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranatorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly WebAPIBackend.Services.ICompanyService _companyService;

        // Guarantee Type Constants
        private const int GuaranteeType_Cash = 1;          // ??? ???
        private const int GuaranteeType_ShariaDeed = 2;    // ????? ????
        private const int GuaranteeType_CustomaryDeed = 3; // ????? ????

        public GuaranatorController(AppDbContext context, WebAPIBackend.Services.ICompanyService companyService)
        {
            _context = context;
            _companyService = companyService;
        }

        /// <summary>
        /// Validates guarantee type and its required conditional fields
        /// </summary>
        private (bool IsValid, string ErrorMessage) ValidateGuaranteeFields(GuarantorData request)
        {
            // Guarantee type is required
            if (!request.GuaranteeTypeId.HasValue || request.GuaranteeTypeId == 0)
            {
                return (false, "????? ????? ?????? ???");
            }

            // Validate guarantee type is one of the 3 allowed values
            if (request.GuaranteeTypeId < 1 || request.GuaranteeTypeId > 3)
            {
                return (false, "????? ????? ??????? ???");
            }

            switch (request.GuaranteeTypeId)
            {
                case GuaranteeType_Cash: // ??? ???
                    if (string.IsNullOrWhiteSpace(request.BankName))
                        return (false, "???? ?????? ???");
                    if (string.IsNullOrWhiteSpace(request.DepositNumber))
                        return (false, "???? ???? ?????? ???");
                    if (string.IsNullOrWhiteSpace(request.DepositDate))
                        return (false, "????? ???? ?????? ???");
                    break;

                case GuaranteeType_ShariaDeed: // ????? ????
                    if (string.IsNullOrWhiteSpace(request.CourtName))
                        return (false, "????? ??? ?????? ???");
                    if (string.IsNullOrWhiteSpace(request.CollateralNumber))
                        return (false, "???? ????? ?????? ???");
                    break;

                case GuaranteeType_CustomaryDeed: // قباله عرفی
                    if (string.IsNullOrWhiteSpace(request.SetSerialNumber))
                        return (false, "نمبر سریال سټه الزامی است");
                    // GuaranteeDistrictName is now optional (no validation required)
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
                    guarantor.PropertyDocumentNumber = null;
                    // PropertyDocumentDate is kept - it's always visible
                    // Clear Customary Deed fields
                    guarantor.SetSerialNumber = null;
                    guarantor.GuaranteeDistrictId = null;
                    guarantor.GuaranteeDistrictName = null;
                    break;

                case GuaranteeType_ShariaDeed:
                    // Keep PropertyDocumentNumber and PropertyDocumentDate for Sharia Deed
                    // Clear Cash fields
                    guarantor.BankName = null;
                    guarantor.DepositNumber = null;
                    guarantor.DepositDate = null;
                    // Clear Customary Deed fields
                    guarantor.SetSerialNumber = null;
                    guarantor.GuaranteeDistrictId = null;
                    guarantor.GuaranteeDistrictName = null;
                    break;

                case GuaranteeType_CustomaryDeed:
                    // Clear Cash fields
                    guarantor.BankName = null;
                    guarantor.DepositNumber = null;
                    guarantor.DepositDate = null;
                    // Clear Sharia Deed fields
                    guarantor.CourtName = null;
                    guarantor.CollateralNumber = null;
                    guarantor.PropertyDocumentNumber = null;
                    // PropertyDocumentDate is kept - it's always visible
                    // Keep GuaranteeDistrictName and clear legacy GuaranteeDistrictId
                    guarantor.GuaranteeDistrictId = null;
                    break;

                default:
                    // Clear all conditional fields if no valid type
                    guarantor.CourtName = null;
                    guarantor.CollateralNumber = null;
                    guarantor.SetSerialNumber = null;
                    guarantor.GuaranteeDistrictId = null;
                    guarantor.GuaranteeDistrictName = null;
                    guarantor.BankName = null;
                    guarantor.DepositNumber = null;
                    guarantor.DepositDate = null;
                    guarantor.PropertyDocumentNumber = null;
                    // PropertyDocumentDate is kept - it's always visible
                    break;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getGuaranatorById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var guarantors = await _context.Guarantors
                    .Where(x => x.CompanyId.Equals(id))
                    .OrderByDescending(x => x.IsActive)
                    .ThenByDescending(x => x.CreatedAt)
                    .ToListAsync();

                // DO NOT convert dates - return them as Gregorian
                // The frontend will handle calendar conversion

                return Ok(guarantors);
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Error in getGuaranatorById: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get only the active witness for a company
        /// </summary>
        [HttpGet("active/{companyId}")]
        public async Task<IActionResult> GetActiveGuarantor(int companyId)
        {
            try
            {
                var activeGuarantor = await _context.Guarantors
                    .Where(x => x.CompanyId == companyId && x.IsActive)
                    .FirstOrDefaultAsync();

                if (activeGuarantor == null)
                {
                    return NotFound(new { message = "هیچ شاهد فعالی یافت نشد" });
                }

                return Ok(activeGuarantor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetActiveGuarantor: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get witness history for a company
        /// </summary>
        [HttpGet("history/{companyId}")]
        public async Task<IActionResult> GetGuarantorHistory(int companyId)
        {
            try
            {
                var history = await _context.Guarantors
                    .Where(x => x.CompanyId == companyId)
                    .OrderByDescending(x => x.IsActive)
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(g => new
                    {
                        g.Id,
                        g.FirstName,
                        g.FatherName,
                        g.GrandFatherName,
                        g.ElectronicNationalIdNumber,
                        g.PhoneNumber,
                        g.IsActive,
                        g.CreatedAt,
                        g.ExpiredAt,
                        g.ExpiredBy,
                        g.ReplacedByGuarantorId
                    })
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGuarantorHistory: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

            // Check if company exists
            if (request.CompanyId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            // Check if there's already an active witness
            var existingActiveWitness = await _context.Guarantors
                .FirstOrDefaultAsync(g => g.CompanyId == request.CompanyId && g.IsActive);

            // If there's an active witness, expire it
            if (existingActiveWitness != null)
            {
                existingActiveWitness.IsActive = false;
                existingActiveWitness.ExpiredAt = DateTime.UtcNow;
                existingActiveWitness.ExpiredBy = userId;
                // We'll set ReplacedByGuarantorId after creating the new witness
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
                CompanyId = request.CompanyId,
                ElectronicNationalIdNumber = request.ElectronicNationalIdNumber,
                PhoneNumber = request.PhoneNumber,
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
                GuaranteeDistrictName = request.GuaranteeDistrictName,
                // Conditional fields - Cash
                BankName = request.BankName,
                DepositNumber = request.DepositNumber,
                DepositDate = depositDate,
                // Witness history fields
                IsActive = true, // New witness is always active
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
            };

            // Clear fields that don't belong to the selected guarantee type
            ClearIrrelevantFields(property, request.GuaranteeTypeId);

            _context.Add(property);
            await _context.SaveChangesAsync();

            // Now update the old witness with the new witness ID
            if (existingActiveWitness != null)
            {
                existingActiveWitness.ReplacedByGuarantorId = property.Id;
                await _context.SaveChangesAsync();
            }

            // Update the IsComplete status based on validation
            if (request.CompanyId.HasValue)
            {
                await _companyService.UpdateLicenseCompletionStatusAsync(request.CompanyId.Value);
            }

            var result = new { 
                Id = property.Id,
                Message = existingActiveWitness != null 
                    ? "شاهد جدید اضافه شد و شاهد قبلی منقضی شد" 
                    : "شاهد با موفقیت اضافه شد",
                ReplacedWitnessId = existingActiveWitness?.Id
            };
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

            // CRITICAL: Only allow editing of active witnesses
            if (!existingProperty.IsActive)
            {
                return StatusCode(403, new { 
                    message = "شاهد منقضی شده قابل ویرایش نیست. فقط شاهد فعال قابل ویرایش است.",
                    isActive = false,
                    expiredAt = existingProperty.ExpiredAt
                });
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

            // Store the original values that should not be changed
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId = existingProperty.CompanyId;
            var isActive = existingProperty.IsActive;
            var expiredAt = existingProperty.ExpiredAt;
            var expiredBy = existingProperty.ExpiredBy;
            var replacedByGuarantorId = existingProperty.ReplacedByGuarantorId;

            // Update the entity with the new values
            existingProperty.FirstName = request.FirstName;
            existingProperty.FatherName = request.FatherName;
            existingProperty.GrandFatherName = request.GrandFatherName;
            existingProperty.ElectronicNationalIdNumber = request.ElectronicNationalIdNumber;
            existingProperty.CompanyId = companyId;
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
            existingProperty.GuaranteeDistrictName = request.GuaranteeDistrictName;
            // Conditional fields - Cash
            existingProperty.BankName = request.BankName;
            existingProperty.DepositNumber = request.DepositNumber;
            existingProperty.DepositDate = depositDate;

            // Clear fields that don't belong to the selected guarantee type
            ClearIrrelevantFields(existingProperty, request.GuaranteeTypeId);

            // Restore the original values that should not be changed
            existingProperty.CreatedBy = createdBy;
            existingProperty.CreatedAt = createdAt;
            existingProperty.IsActive = isActive;
            existingProperty.ExpiredAt = expiredAt;
            existingProperty.ExpiredBy = expiredBy;
            existingProperty.ReplacedByGuarantorId = replacedByGuarantorId;

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
                        UpdatedAt = DateTime.UtcNow,
                        PropertyName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Update the IsComplete status based on validation
            if (existingProperty.CompanyId.HasValue)
            {
                await _companyService.UpdateLicenseCompletionStatusAsync(existingProperty.CompanyId.Value);
            }

            var result = new { Id = request.Id };
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuarantor(int id)
        {
            try
            {
                var guarantor = await _context.Guarantors.FindAsync(id);
                if (guarantor == null)
                {
                    return NotFound(new { message = "شاهد یافت نشد" });
                }

                // Only allow deletion of active witnesses
                if (!guarantor.IsActive)
                {
                    return StatusCode(403, new { 
                        message = "شاهد منقضی شده قابل حذف نیست. فقط شاهد فعال قابل حذف است.",
                        isActive = false
                    });
                }

                var companyId = guarantor.CompanyId;

                // Delete audit records first
                var auditRecords = await _context.Guarantorsaudits
                    .Where(a => a.GuarantorsId == id)
                    .ToListAsync();
                
                if (auditRecords.Any())
                {
                    _context.Guarantorsaudits.RemoveRange(auditRecords);
                }

                // If this witness replaced another, clear the ReplacedByGuarantorId reference
                var previousWitness = await _context.Guarantors
                    .FirstOrDefaultAsync(g => g.ReplacedByGuarantorId == id);
                
                if (previousWitness != null)
                {
                    previousWitness.ReplacedByGuarantorId = null;
                }

                // Delete the guarantor
                _context.Guarantors.Remove(guarantor);
                await _context.SaveChangesAsync();

                // Update the IsComplete status
                if (companyId.HasValue)
                {
                    await _companyService.UpdateLicenseCompletionStatusAsync(companyId.Value);
                }

                return Ok(new { message = "شاهد با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting guarantor: {ex.Message}");
                return StatusCode(500, new { 
                    message = "خطا در حذف شاهد",
                    error = ex.Message 
                });
            }
        }
    }
}
