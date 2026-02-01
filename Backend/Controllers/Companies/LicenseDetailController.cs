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
    [Authorize(Roles = "ADMIN,AUTHORITY,COMPANY_REGISTRAR,LICENSE_REVIEWER")]
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseDetailController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly WebAPIBackend.Services.ILicenseNumberGenerator _licenseNumberGenerator;
        private readonly WebAPIBackend.Services.IProvinceFilterService _provinceFilter;
        
        public LicenseDetailController(
            AppDbContext context, 
            WebAPIBackend.Services.ILicenseNumberGenerator licenseNumberGenerator,
            WebAPIBackend.Services.IProvinceFilterService provinceFilter)
        {
            _context = context;
            _licenseNumberGenerator = licenseNumberGenerator;
            _provinceFilter = provinceFilter;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                // Get licenses for the company
                var licenses = await _context.LicenseDetails.Where(x => x.CompanyId.Equals(id)).ToListAsync();
                
                // Validate province access for each license
                foreach (var license in licenses)
                {
                    _provinceFilter.ValidateProvinceAccess(license.ProvinceId);
                }

                // DO NOT convert dates - return them as Gregorian
                // The frontend will handle calendar conversion
                
                return Ok(licenses);
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException)
            {
                return NotFound(new { message = "License not found" }); // Return 404 to avoid information leakage
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

            try
            {
                var userId = userIdClaim.Value;

                // Get parent company to inherit province
                var company = await _context.CompanyDetails.FindAsync(request.CompanyId);
                if (company == null)
                {
                    return NotFound("Company not found");
                }

                // Validate province access to parent company
                _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

                // Parse dates using multi-calendar helper
                if (!DateConversionHelper.TryParseToDateOnly(request.IssueDate, request.CalendarType, out var issueDate))
                {
                    return BadRequest("????? ???? ???? ????? ???? / ? ???? ? ???? ???? ??? ?? ??");
                }

                var expectedExpireDate = issueDate.AddYears(3);

                DateOnly expireDate = expectedExpireDate;
                if (!string.IsNullOrWhiteSpace(request.ExpireDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.ExpireDate, request.CalendarType, out var submittedExpireDate))
                    {
                        return BadRequest("????? ??? ???? ????? ???? / ? ???? ? ??? ???? ??? ?? ??");
                    }

                    if (submittedExpireDate != expectedExpireDate)
                    {
                        return BadRequest("????? ??? ???? ???? ?????? ?? ??? ??? ?? ????? ???? ???? ???? / ? ???? ? ??? ???? ???? ?????? ? ???? ?? ???? ??? ???? ?????? ??");
                    }
                }

                // Validate LicenseCategory if provided
                var allowedLicenseCategories = new[] { "جدید", "تجدید", "مثنی" };
                if (!string.IsNullOrEmpty(request.LicenseCategory) && !allowedLicenseCategories.Contains(request.LicenseCategory))
                {
                    return BadRequest("Invalid LicenseCategory. Allowed values: جدید, تجدید, مثنی");
                }

                // Parse HrLetterDate if provided
                DateOnly? hrLetterDate = null;
                if (!string.IsNullOrWhiteSpace(request.HrLetterDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.HrLetterDate, request.CalendarType, out var parsedHrLetterDate))
                    {
                        return BadRequest("????? ????? ???? ???? ????? ????");
                    }
                    hrLetterDate = parsedHrLetterDate;
                }

                // Parse RoyaltyDate if provided
                DateOnly? royaltyDate = null;
                if (!string.IsNullOrWhiteSpace(request.RoyaltyDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.RoyaltyDate, request.CalendarType, out var parsedRoyaltyDate))
                    {
                        return BadRequest("????? ?????? ????? ????? ????");
                    }
                    royaltyDate = parsedRoyaltyDate;
                }

                // Parse PenaltyDate if provided
                DateOnly? penaltyDate = null;
                if (!string.IsNullOrWhiteSpace(request.PenaltyDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.PenaltyDate, request.CalendarType, out var parsedPenaltyDate))
                    {
                        return BadRequest("????? ????? ????? ????");
                    }
                    penaltyDate = parsedPenaltyDate;
                }

                // Validate numeric fields
                if (request.RoyaltyAmount.HasValue && request.RoyaltyAmount < 0)
                {
                    return BadRequest("RoyaltyAmount must be a non-negative number.");
                }
                if (request.PenaltyAmount.HasValue && request.PenaltyAmount < 0)
                {
                    return BadRequest("PenaltyAmount must be a non-negative number.");
                }

                // Inherit province from parent company
                var provinceId = company.ProvinceId;

                // Auto-generate license number if ProvinceId is provided and LicenseNumber is empty
                string licenseNumber = request.LicenseNumber;
                if (provinceId.HasValue && string.IsNullOrWhiteSpace(licenseNumber))
                {
                    try
                    {
                        licenseNumber = await _licenseNumberGenerator.GenerateNextLicenseNumber(provinceId.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest($"??? ?? ????? ????? ????: {ex.Message}");
                    }
                }

                var property = new LicenseDetail
                {
                    LicenseNumber = licenseNumber,
                    ProvinceId = provinceId, // Inherit from company
                    IssueDate = issueDate,
                    ExpireDate = expireDate,
                    AreaId = request.AreaId,
                    OfficeAddress = request.OfficeAddress,
                    CompanyId = request.CompanyId,
                    DocPath = request.DocPath,
                    LicenseType = request.LicenseType,
                    LicenseCategory = request.LicenseCategory,
                    RenewalRound = request.RenewalRound,
                    RoyaltyAmount = request.RoyaltyAmount,
                    RoyaltyDate = royaltyDate,
                    TariffNumber = request.TariffNumber,
                    PenaltyAmount = request.PenaltyAmount,
                    PenaltyDate = penaltyDate,
                    HrLetter = request.HrLetter,
                    HrLetterDate = hrLetterDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                };

                _context.Add(property);
                await _context.SaveChangesAsync();
                var result = new { Id = property.Id };
                return Ok(result);
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] LicenseDetailData request)
        {
            try
            {
                // Parse dates using multi-calendar helper
                if (!DateConversionHelper.TryParseToDateOnly(request.IssueDate, request.CalendarType, out var issueDate))
                {
                    return BadRequest("????? ???? ???? ????? ???? / ? ???? ? ???? ???? ??? ?? ??");
                }

                var expectedExpireDate = issueDate.AddYears(3);

                DateOnly expireDate = expectedExpireDate;
                if (!string.IsNullOrWhiteSpace(request.ExpireDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.ExpireDate, request.CalendarType, out var submittedExpireDate))
                    {
                        return BadRequest("????? ??? ???? ????? ???? / ? ???? ? ??? ???? ??? ?? ??");
                    }

                    if (submittedExpireDate != expectedExpireDate)
                    {
                        return BadRequest("????? ??? ???? ???? ?????? ?? ??? ??? ?? ????? ???? ???? ???? / ? ???? ? ??? ???? ???? ?????? ? ???? ?? ???? ??? ???? ?????? ??");
                    }
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

                // Validate province access
                _provinceFilter.ValidateProvinceAccess(existingProperty.ProvinceId);

                // Validate LicenseCategory if provided
                var allowedLicenseCategories = new[] { "جدید", "تجدید", "مثنی" };
                if (!string.IsNullOrEmpty(request.LicenseCategory) && !allowedLicenseCategories.Contains(request.LicenseCategory))
                {
                    return BadRequest("Invalid LicenseCategory. Allowed values: جدید, تجدید, مثنی");
                }

                // Parse HrLetterDate if provided
                DateOnly? hrLetterDate = null;
                if (!string.IsNullOrWhiteSpace(request.HrLetterDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.HrLetterDate, request.CalendarType, out var parsedHrLetterDate))
                    {
                        return BadRequest("????? ????? ???? ???? ????? ????");
                    }
                    hrLetterDate = parsedHrLetterDate;
                }

                // Parse RoyaltyDate if provided
                DateOnly? royaltyDate = null;
                if (!string.IsNullOrWhiteSpace(request.RoyaltyDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.RoyaltyDate, request.CalendarType, out var parsedRoyaltyDate))
                    {
                        return BadRequest("????? ?????? ????? ????? ????");
                    }
                    royaltyDate = parsedRoyaltyDate;
                }

                // Parse PenaltyDate if provided
                DateOnly? penaltyDate = null;
                if (!string.IsNullOrWhiteSpace(request.PenaltyDate))
                {
                    if (!DateConversionHelper.TryParseToDateOnly(request.PenaltyDate, request.CalendarType, out var parsedPenaltyDate))
                    {
                        return BadRequest("????? ????? ????? ????");
                    }
                    penaltyDate = parsedPenaltyDate;
                }

                // Validate numeric fields
                if (request.RoyaltyAmount.HasValue && request.RoyaltyAmount < 0)
                {
                    return BadRequest("RoyaltyAmount must be a non-negative number.");
                }
                if (request.PenaltyAmount.HasValue && request.PenaltyAmount < 0)
                {
                    return BadRequest("PenaltyAmount must be a non-negative number.");
                }

                // Store the original values
                var createdBy = existingProperty.CreatedBy;
                var createdAt = existingProperty.CreatedAt;
                var companyId = existingProperty.CompanyId;
                var originalProvinceId = existingProperty.ProvinceId; // Preserve province immutability

                // Auto-generate license number if ProvinceId is provided and LicenseNumber is empty
                string licenseNumber = request.LicenseNumber;
                if (originalProvinceId.HasValue && string.IsNullOrWhiteSpace(licenseNumber))
                {
                    try
                    {
                        licenseNumber = await _licenseNumberGenerator.GenerateNextLicenseNumber(originalProvinceId.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest($"??? ?? ????? ????? ????: {ex.Message}");
                    }
                }

                // Update the entity with the new values
                existingProperty.LicenseNumber = licenseNumber;
                existingProperty.IssueDate = issueDate;
                existingProperty.ExpireDate = expireDate;
                existingProperty.AreaId = request.AreaId;
                existingProperty.OfficeAddress = request.OfficeAddress;
                existingProperty.DocPath = request.DocPath;
                existingProperty.LicenseType = request.LicenseType;
                existingProperty.LicenseCategory = request.LicenseCategory;
                existingProperty.RenewalRound = request.RenewalRound;
                existingProperty.RoyaltyAmount = request.RoyaltyAmount;
                existingProperty.RoyaltyDate = royaltyDate;
                existingProperty.TariffNumber = request.TariffNumber;
                existingProperty.PenaltyAmount = request.PenaltyAmount;
                existingProperty.PenaltyDate = penaltyDate;
                existingProperty.HrLetter = request.HrLetter;
                existingProperty.HrLetterDate = hrLetterDate;

                // Restore the original values
                existingProperty.CreatedBy = createdBy;
                existingProperty.CreatedAt = createdAt;
                existingProperty.ProvinceId = originalProvinceId; // Ensure province hasn't changed

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
                            UpdatedAt = DateTime.UtcNow,
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
            catch (WebAPIBackend.Models.Common.ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("GetLicenseView/{id}")]
        public async Task<IActionResult> GetLicenseViewById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                // First check if license exists and validate province access
                var licenseDetail = await _context.LicenseDetails
                    .Where(l => l.CompanyId == id)
                    .OrderByDescending(l => l.IssueDate)
                    .FirstOrDefaultAsync();

                if (licenseDetail == null)
                {
                    return NotFound();
                }

                // Validate province access
                _provinceFilter.ValidateProvinceAccess(licenseDetail.ProvinceId);

                // First, ensure the LicenseView exists by trying to create it
                await EnsureLicenseViewExists();
                
                var data = await _context.LicenseView
                    .FirstOrDefaultAsync(x => x.CompanyId == id);
                if (data == null)
                {
                    return NotFound();
                }

                // Get address data from CompanyOwner and Location tables
                var owner = await _context.CompanyOwners
                    .Where(o => o.CompanyId == id)
                    .FirstOrDefaultAsync();

                string? ownerProvinceName = null;
                string? ownerDistrictName = null;
                string? ownerVillage = null;
                string? permanentProvinceName = null;
                string? permanentDistrictName = null;
                string? permanentVillage = null;

                if (owner != null)
                {
                    ownerVillage = owner.OwnerVillage;
                    permanentVillage = owner.PermanentVillage;

                    // Get owner address location names
                    if (owner.OwnerProvinceId.HasValue)
                    {
                        var ownerProvince = await _context.Locations
                            .Where(l => l.Id == owner.OwnerProvinceId.Value)
                            .FirstOrDefaultAsync();
                        ownerProvinceName = ownerProvince?.Dari;
                    }

                    if (owner.OwnerDistrictId.HasValue)
                    {
                        var ownerDistrict = await _context.Locations
                            .Where(l => l.Id == owner.OwnerDistrictId.Value)
                            .FirstOrDefaultAsync();
                        ownerDistrictName = ownerDistrict?.Dari;
                    }

                    // Get permanent address location names
                    if (owner.PermanentProvinceId.HasValue)
                    {
                        var permanentProvince = await _context.Locations
                            .Where(l => l.Id == owner.PermanentProvinceId.Value)
                            .FirstOrDefaultAsync();
                        permanentProvinceName = permanentProvince?.Dari;
                    }

                    if (owner.PermanentDistrictId.HasValue)
                    {
                        var permanentDistrict = await _context.Locations
                            .Where(l => l.Id == owner.PermanentDistrictId.Value)
                            .FirstOrDefaultAsync();
                        permanentDistrictName = permanentDistrict?.Dari;
                    }
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
                string hrLetterDateFormatted = data.HrLetterDate.HasValue 
                    ? DateConversionHelper.FormatDateOnly(data.HrLetterDate, calendar) 
                    : "";
                string royaltyDateFormatted = data.RoyaltyDate.HasValue 
                    ? DateConversionHelper.FormatDateOnly(data.RoyaltyDate, calendar) 
                    : "";
                string penaltyDateFormatted = data.PenaltyDate.HasValue 
                    ? DateConversionHelper.FormatDateOnly(data.PenaltyDate, calendar) 
                    : "";

                var result = new
                {
                    data.CompanyId,
                    LicenseDetailId = licenseDetail?.Id, // Actual LicenseDetail ID for verification
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
                    // Owner Address Fields
                    ownerProvinceName,
                    ownerDistrictName,
                    ownerVillage,
                    // Permanent Address Fields
                    permanentProvinceName,
                    permanentDistrictName,
                    permanentVillage,
                    // Financial and Administrative Fields
                    data.RoyaltyAmount,
                    data.RoyaltyDate,
                    royaltyDateFormatted,
                    data.TariffNumber,
                    data.PenaltyAmount,
                    data.PenaltyDate,
                    penaltyDateFormatted,
                    data.HrLetter,
                    data.HrLetterDate,
                    hrLetterDateFormatted,
                };

                return Ok(result);
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException)
            {
                return NotFound(new { message = "License not found" }); // Return 404 to avoid information leakage
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading license view: {ex.Message}");
            }
        }

        /// <summary>
        /// Ensures the LicenseView database view exists with all required columns
        /// </summary>
        private async Task EnsureLicenseViewExists()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    DO $$
                    BEGIN
                        -- Add missing columns to CompanyOwner if needed
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentProvinceId') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentProvinceId"" INTEGER NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentDistrictId') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentDistrictId"" INTEGER NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PermanentVillage') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PermanentVillage"" TEXT NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'PhoneNumber') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""PhoneNumber"" VARCHAR(20) NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'CompanyOwner' AND column_name = 'WhatsAppNumber') THEN
                            ALTER TABLE org.""CompanyOwner"" ADD COLUMN ""WhatsAppNumber"" VARCHAR(20) NULL;
                        END IF;
                        
                        -- Add missing columns to LicenseDetails if needed
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RoyaltyAmount') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RoyaltyAmount"" NUMERIC(18,2) NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'RoyaltyDate') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""RoyaltyDate"" DATE NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'TariffNumber') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""TariffNumber"" VARCHAR(100) NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'PenaltyAmount') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""PenaltyAmount"" NUMERIC(18,2) NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'PenaltyDate') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""PenaltyDate"" DATE NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'HrLetter') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""HrLetter"" VARCHAR(255) NULL;
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'org' AND table_name = 'LicenseDetails' AND column_name = 'HrLetterDate') THEN
                            ALTER TABLE org.""LicenseDetails"" ADD COLUMN ""HrLetterDate"" DATE NULL;
                        END IF;
                    END $$;

                    -- Recreate the LicenseView
                    DROP VIEW IF EXISTS public.""LicenseView"";
                    
                    CREATE OR REPLACE VIEW public.""LicenseView"" AS
                    SELECT 
                        cd.""Id"" AS ""CompanyId"",
                        co.""PhoneNumber"",
                        co.""WhatsAppNumber"",
                        cd.""Title"",
                        cd.""TIN"",
                        co.""FirstName"",
                        co.""FatherName"",
                        co.""GrandFatherName"",
                        co.""DateofBirth"",
                        co.""ElectronicNationalIdNumber"" AS ""IndentityCardNumber"",
                        co.""PothoPath"" AS ""OwnerPhoto"",
                        ld.""LicenseNumber"",
                        ld.""OfficeAddress"",
                        ld.""IssueDate"",
                        ld.""ExpireDate"",
                        pp.""Dari"" AS ""PermanentProvinceName"",
                        pd.""Dari"" AS ""PermanentDistrictName"",
                        co.""PermanentVillage"",
                        ld.""RoyaltyAmount"",
                        ld.""RoyaltyDate"",
                        ld.""TariffNumber"",
                        ld.""PenaltyAmount"",
                        ld.""PenaltyDate"",
                        ld.""HrLetter"",
                        ld.""HrLetterDate""
                    FROM org.""CompanyDetails"" cd
                    LEFT JOIN org.""CompanyOwner"" co ON cd.""Id"" = co.""CompanyId""
                    LEFT JOIN org.""LicenseDetails"" ld ON cd.""Id"" = ld.""CompanyId""
                    LEFT JOIN look.""Location"" pp ON co.""PermanentProvinceId"" = pp.""ID""
                    LEFT JOIN look.""Location"" pd ON co.""PermanentDistrictId"" = pd.""ID"";
                ");
            }
            catch (Exception)
            {
                // View creation failed, but we'll try to query anyway
            }
        }
    }
}
