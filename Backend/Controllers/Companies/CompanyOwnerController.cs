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
                var Pro = await _context.CompanyOwners
                    .Where(x => x.CompanyId.Equals(id))
                    .Select(o => new
                    {
                        o.Id,
                        o.FirstName,
                        o.FatherName,
                        o.GrandFatherName,
                        o.EducationLevelId,
                        DateofBirth = o.DateofBirth,
                        o.IdentityCardTypeId,
                        o.IndentityCardNumber,
                        o.Jild,
                        o.Safha,
                        o.CompanyId,
                        o.SabtNumber,
                        o.PothoPath,
                        // Contact Information
                        o.PhoneNumber,
                        o.WhatsAppNumber,
                        // Owner's Own Address Fields (آدرس اصلی مالک)
                        o.OwnerProvinceId,
                        o.OwnerDistrictId,
                        o.OwnerVillage,
                        // Permanent Address Fields (آدرس دایمی) - Current Residence
                        o.PermanentProvinceId,
                        o.PermanentDistrictId,
                        o.PermanentVillage,
                        // Temporary Address Fields (آدرس موقت)
                        o.TemporaryProvinceId,
                        o.TemporaryDistrictId,
                        o.TemporaryVillage,
                        // Location names for display
                        OwnerProvinceName = o.OwnerProvince != null ? o.OwnerProvince.Dari : null,
                        OwnerDistrictName = o.OwnerDistrict != null ? o.OwnerDistrict.Dari : null,
                        PermanentProvinceName = o.PermanentProvince != null ? o.PermanentProvince.Dari : null,
                        PermanentDistrictName = o.PermanentDistrict != null ? o.PermanentDistrict.Dari : null,
                        TemporaryProvinceName = o.TemporaryProvince != null ? o.TemporaryProvince.Dari : null,
                        TemporaryDistrictName = o.TemporaryDistrict != null ? o.TemporaryDistrict.Dari : null,
                    })
                    .ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var result = Pro.Select(item => new
                {
                    item.Id,
                    item.FirstName,
                    item.FatherName,
                    item.GrandFatherName,
                    item.EducationLevelId,
                    DateofBirth = DateConversionHelper.ToCalendarDateOnly(item.DateofBirth, calendar),
                    item.IdentityCardTypeId,
                    item.IndentityCardNumber,
                    item.Jild,
                    item.Safha,
                    item.CompanyId,
                    item.SabtNumber,
                    item.PothoPath,
                    item.PhoneNumber,
                    item.WhatsAppNumber,
                    item.OwnerProvinceId,
                    item.OwnerDistrictId,
                    item.OwnerVillage,
                    item.PermanentProvinceId,
                    item.PermanentDistrictId,
                    item.PermanentVillage,
                    item.TemporaryProvinceId,
                    item.TemporaryDistrictId,
                    item.TemporaryVillage,
                    item.OwnerProvinceName,
                    item.OwnerDistrictName,
                    item.PermanentProvinceName,
                    item.PermanentDistrictName,
                    item.TemporaryProvinceName,
                    item.TemporaryDistrictName,
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// Get address history for a company owner
        /// </summary>
        [HttpGet("addressHistory/{companyId}")]
        public async Task<IActionResult> GetAddressHistory(int companyId)
        {
            try
            {
                var owner = await _context.CompanyOwners
                    .FirstOrDefaultAsync(x => x.CompanyId == companyId);

                if (owner == null)
                {
                    return NotFound("Owner not found");
                }

                var history = await _context.CompanyOwnerAddressHistories
                    .Where(h => h.CompanyOwnerId == owner.Id)
                    .OrderByDescending(h => h.EffectiveFrom)
                    .Select(h => new
                    {
                        h.Id,
                        h.AddressType,
                        h.ProvinceId,
                        h.DistrictId,
                        h.Village,
                        ProvinceName = h.Province != null ? h.Province.Dari : null,
                        DistrictName = h.District != null ? h.District.Dari : null,
                        h.EffectiveFrom,
                        h.EffectiveTo,
                        h.IsActive
                    })
                    .ToListAsync();

                return Ok(history);
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
                IndentityCardNumber = string.IsNullOrEmpty(request.IndentityCardNumber) ? null : double.TryParse(request.IndentityCardNumber, out var cardNum) ? cardNum : null,
                Jild = request.Jild,
                Safha = request.Safha,
                CompanyId = request.CompanyId,
                SabtNumber = request.SabtNumber,
                PothoPath = request.PothoPath,
                // Contact Information
                PhoneNumber = request.PhoneNumber?.Trim(),
                WhatsAppNumber = request.WhatsAppNumber?.Trim(),
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                // Owner's Own Address Fields (آدرس اصلی مالک)
                OwnerProvinceId = request.OwnerProvinceId,
                OwnerDistrictId = request.OwnerDistrictId,
                OwnerVillage = request.OwnerVillage,
                // Permanent Address Fields (آدرس دایمی) - Current Residence
                PermanentProvinceId = request.PermanentProvinceId,
                PermanentDistrictId = request.PermanentDistrictId,
                PermanentVillage = request.PermanentVillage,
                // Temporary Address Fields (آدرس موقت)
                TemporaryProvinceId = request.TemporaryProvinceId,
                TemporaryDistrictId = request.TemporaryDistrictId,
                TemporaryVillage = request.TemporaryVillage,
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

            // Handle Address Change - Archive old address to history if this is an address change
            if (request.IsAddressChange)
            {
                await ArchiveCurrentAddressToHistory(existingProperty, userId);
            }

            // Update the entity with the new values
            existingProperty.FirstName = request.FirstName;
            existingProperty.FatherName = request.FatherName;
            existingProperty.GrandFatherName = request.GrandFatherName;
            existingProperty.EducationLevelId = request.EducationLevelId;
            existingProperty.DateofBirth = pdate;
            existingProperty.IdentityCardTypeId = request.IdentityCardTypeId;
            existingProperty.IndentityCardNumber = string.IsNullOrEmpty(request.IndentityCardNumber) ? null : double.TryParse(request.IndentityCardNumber, out var cardNum) ? cardNum : null;
            existingProperty.Jild = request.Jild;
            existingProperty.Safha = request.Safha;
            existingProperty.CompanyId = request.CompanyId;
            existingProperty.SabtNumber = request.SabtNumber;
            existingProperty.PothoPath = request.PothoPath;
            // Contact Information
            existingProperty.PhoneNumber = request.PhoneNumber?.Trim();
            existingProperty.WhatsAppNumber = request.WhatsAppNumber?.Trim();
            // Owner's Own Address Fields (آدرس اصلی مالک)
            existingProperty.OwnerProvinceId = request.OwnerProvinceId;
            existingProperty.OwnerDistrictId = request.OwnerDistrictId;
            existingProperty.OwnerVillage = request.OwnerVillage;
            // Permanent Address Fields (آدرس دایمی) - Current Residence
            existingProperty.PermanentProvinceId = request.PermanentProvinceId;
            existingProperty.PermanentDistrictId = request.PermanentDistrictId;
            existingProperty.PermanentVillage = request.PermanentVillage;
            // Temporary Address Fields (آدرس موقت)
            existingProperty.TemporaryProvinceId = request.TemporaryProvinceId;
            existingProperty.TemporaryDistrictId = request.TemporaryDistrictId;
            existingProperty.TemporaryVillage = request.TemporaryVillage;

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

        /// <summary>
        /// Archives the current address to history before updating with new address
        /// </summary>
        private async Task ArchiveCurrentAddressToHistory(CompanyOwner owner, string userId)
        {
            var now = DateTime.Now;

            // Archive Owner's Own Address if exists
            if (owner.OwnerProvinceId.HasValue || owner.OwnerDistrictId.HasValue || !string.IsNullOrEmpty(owner.OwnerVillage))
            {
                // Mark any existing active owner address history as inactive
                var existingActiveOwner = await _context.CompanyOwnerAddressHistories
                    .Where(h => h.CompanyOwnerId == owner.Id && h.AddressType == "Owner" && h.IsActive)
                    .ToListAsync();

                foreach (var existing in existingActiveOwner)
                {
                    existing.IsActive = false;
                    existing.EffectiveTo = now;
                }

                // Create new history record for the current owner address
                var ownerHistory = new CompanyOwnerAddressHistory
                {
                    CompanyOwnerId = owner.Id,
                    ProvinceId = owner.OwnerProvinceId,
                    DistrictId = owner.OwnerDistrictId,
                    Village = owner.OwnerVillage,
                    AddressType = "Owner",
                    EffectiveFrom = owner.CreatedAt ?? now,
                    EffectiveTo = now,
                    IsActive = false,
                    CreatedAt = now,
                    CreatedBy = userId
                };
                _context.CompanyOwnerAddressHistories.Add(ownerHistory);
            }

            // Archive Permanent Address if exists
            if (owner.PermanentProvinceId.HasValue || owner.PermanentDistrictId.HasValue || !string.IsNullOrEmpty(owner.PermanentVillage))
            {
                // Mark any existing active permanent address history as inactive
                var existingActivePermanent = await _context.CompanyOwnerAddressHistories
                    .Where(h => h.CompanyOwnerId == owner.Id && h.AddressType == "Permanent" && h.IsActive)
                    .ToListAsync();

                foreach (var existing in existingActivePermanent)
                {
                    existing.IsActive = false;
                    existing.EffectiveTo = now;
                }

                // Create new history record for the current permanent address
                var permanentHistory = new CompanyOwnerAddressHistory
                {
                    CompanyOwnerId = owner.Id,
                    ProvinceId = owner.PermanentProvinceId,
                    DistrictId = owner.PermanentDistrictId,
                    Village = owner.PermanentVillage,
                    AddressType = "Permanent",
                    EffectiveFrom = owner.CreatedAt ?? now,
                    EffectiveTo = now,
                    IsActive = false,
                    CreatedAt = now,
                    CreatedBy = userId
                };
                _context.CompanyOwnerAddressHistories.Add(permanentHistory);
            }

            // Archive Temporary Address if exists
            if (owner.TemporaryProvinceId.HasValue || owner.TemporaryDistrictId.HasValue || !string.IsNullOrEmpty(owner.TemporaryVillage))
            {
                // Mark any existing active temporary address history as inactive
                var existingActiveTemporary = await _context.CompanyOwnerAddressHistories
                    .Where(h => h.CompanyOwnerId == owner.Id && h.AddressType == "Temporary" && h.IsActive)
                    .ToListAsync();

                foreach (var existing in existingActiveTemporary)
                {
                    existing.IsActive = false;
                    existing.EffectiveTo = now;
                }

                // Create new history record for the temporary address
                var temporaryHistory = new CompanyOwnerAddressHistory
                {
                    CompanyOwnerId = owner.Id,
                    ProvinceId = owner.TemporaryProvinceId,
                    DistrictId = owner.TemporaryDistrictId,
                    Village = owner.TemporaryVillage,
                    AddressType = "Temporary",
                    EffectiveFrom = owner.CreatedAt ?? now,
                    EffectiveTo = now,
                    IsActive = false,
                    CreatedAt = now,
                    CreatedBy = userId
                };
                _context.CompanyOwnerAddressHistories.Add(temporaryHistory);
            }
        }
    }
}
