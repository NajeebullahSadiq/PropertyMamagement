using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.Audit;
using WebAPIBackend.Models.RequestData;
using WebAPIBackend.Models.ViewModels;
using WebAPIBackend.Services;

namespace WebAPIBackend.Controllers.Companies
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WebAPIBackend.Services.IProvinceFilterService _provinceFilter;
        private readonly WebAPIBackend.Services.IComprehensiveAuditService _auditService;
        private readonly WebAPIBackend.Services.ICompanyService _companyService;
        private readonly ILookupCacheService _cache;

        public CompanyDetailsController(
            AppDbContext context, 
            UserManager<ApplicationUser> userManager,
            WebAPIBackend.Services.IProvinceFilterService provinceFilter,
            WebAPIBackend.Services.IComprehensiveAuditService auditService,
            WebAPIBackend.Services.ICompanyService companyService,
            ILookupCacheService cache)
        {
            _context = context;
            _userManager = userManager;
            _provinceFilter = provinceFilter;
            _auditService = auditService;
            _companyService = companyService;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Check if user can access company module
                if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "company"))
                {
                    return StatusCode(403, new { message = "??? ????? ?????? ?? ????? ???? ?? ?? ??????" });
                }

                // Apply province filtering
                var query = _context.CompanyDetails.AsNoTracking().AsQueryable();
                query = _provinceFilter.ApplyProvinceFilter(query);

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower().Trim();
                    query = query.Where(p =>
                        p.Title.ToLower().Contains(searchLower) ||
                        (p.CompanyOwners != null && p.CompanyOwners.Any(o =>
                            o.FirstName.ToLower().Contains(searchLower) ||
                            (o.ElectronicNationalIdNumber != null && o.ElectronicNationalIdNumber.ToLower().Contains(searchLower))
                        )) ||
                        (p.LicenseDetails != null && p.LicenseDetails.Any(l =>
                            l.LicenseNumber != null && l.LicenseNumber.ToLower().Contains(searchLower)
                        ))
                    );
                }

                // Order by CreatedAt descending - most recent first
                var orderedQuery = query.OrderByDescending(p => p.CreatedAt);

                var totalCount = await orderedQuery.CountAsync();

                var result = await orderedQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new CompanyListDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        OwnerFullName = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.FirstName).FirstOrDefault(),
                        OwnerFatherName = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.FatherName).FirstOrDefault(),
                        OwnerElectronicNationalIdNumber = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.ElectronicNationalIdNumber).FirstOrDefault(),
                        LicenseNumber = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.LicenseNumber).FirstOrDefault(),
                        LicenseCategory = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.LicenseCategory).FirstOrDefault(),
                        LicenseIssueDate = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.IssueDate).FirstOrDefault(),
                        LicenseExpiryDate = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.ExpireDate).FirstOrDefault(),
                        Granator = p.Guarantors.OrderBy(g => g.Id).Select(g => 
                            (g.FirstName ?? "") + 
                            (string.IsNullOrWhiteSpace(g.GrandFatherName) ? "" : " " + g.GrandFatherName) + 
                            " " + (g.FatherName ?? "")
                        ).FirstOrDefault(),
                        IsComplete = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.IsComplete).FirstOrDefault(),
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items = result,
                    totalCount,
                    page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("getexpired")]
        public async Task<IActionResult> GetExpiredLicense()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Check if user can access company module
                if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "company"))
                {
                    return StatusCode(403, new { message = "??? ????? ?????? ?? ????? ???? ?? ?? ??????" });
                }

                var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

                // Apply province filtering
                var query = _context.CompanyDetails
                    .AsNoTracking()
                    .Where(p => p.LicenseDetails.Any(l => l.ExpireDate < currentDate))
                    .AsQueryable();
                query = _provinceFilter.ApplyProvinceFilter(query);

                // Order by CreatedAt descending - most recent first
                var result = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        ownerFullName = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.FirstName).FirstOrDefault(),
                        ownerFatherName = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.FatherName).FirstOrDefault(),
                        ownerElectronicNationalIdNumber = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.ElectronicNationalIdNumber).FirstOrDefault(),
                        licenseNumber = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.LicenseNumber).FirstOrDefault(),
                        licenseCategory = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.LicenseCategory).FirstOrDefault(),
                        licenseIssueDate = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.IssueDate).FirstOrDefault(),
                        licenseExpiryDate = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.ExpireDate).FirstOrDefault(),
                        granator = p.Guarantors.OrderBy(g => g.Id).Select(g => 
                            (g.FirstName ?? "") + 
                            (string.IsNullOrWhiteSpace(g.GrandFatherName) ? "" : " " + g.GrandFatherName) + 
                            " " + (g.FatherName ?? "")
                        ).FirstOrDefault(),
                        isComplete = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.IsComplete).FirstOrDefault(),
                    })
                    .ToListAsync();

                return Ok(result);
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
                var company = await _context.CompanyDetails.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new { message = "???? ???? ???" });
                }

                // Validate province access
                _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

                var result = await _context.CompanyDetails.AsNoTracking().Where(x => x.Id.Equals(id)).ToListAsync();
                return Ok(result);
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException)
            {
                return NotFound(new { message = "???? ???? ???" }); // Return 404 to avoid information leakage
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// Get comprehensive company view data for read-only display
        /// </summary>
        [HttpGet("GetView/{id}")]
        public async Task<IActionResult> GetCompanyViewById(int id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Check if user can access company module
                if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "company"))
                {
                    return StatusCode(403, new { message = "??? ????? ?????? ?? ????? ???? ?? ?? ??????" });
                }

                // First check if company exists and validate province access
                var companyCheck = await _context.CompanyDetails.FindAsync(id);
                if (companyCheck == null)
                {
                    return NotFound(new { message = "???? ???? ???" });
                }

                // Validate province access
                _provinceFilter.ValidateProvinceAccess(companyCheck.ProvinceId);

                var data = await _context.CompanyDetails
                    .AsNoTracking()
                    .Where(c => c.Id == id)
                    .Select(c => new
                    {
                        // Basic Company Information
                        c.Id,
                        c.Title,
                        c.Tin,
                        c.DocPath,
                        c.Status,
                        c.CreatedAt,

                        // Company Owner Information
                        Owner = c.CompanyOwners.Select(o => new
                        {
                            o.Id,
                            o.FirstName,
                            o.FatherName,
                            o.GrandFatherName,
                            o.DateofBirth,
                            o.PhoneNumber,
                            o.WhatsAppNumber,
                            o.ElectronicNationalIdNumber,
                            o.PothoPath,
                            EducationLevelName = o.EducationLevel != null ? o.EducationLevel.Dari : null,
                            // Owner's Own Address
                            OwnerProvinceName = o.OwnerProvince != null ? o.OwnerProvince.Dari : null,
                            OwnerDistrictName = o.OwnerDistrict != null ? o.OwnerDistrict.Dari : null,
                            o.OwnerVillage,
                            // Permanent Address
                            PermanentProvinceName = o.PermanentProvince != null ? o.PermanentProvince.Dari : null,
                            PermanentDistrictName = o.PermanentDistrict != null ? o.PermanentDistrict.Dari : null,
                            o.PermanentVillage
                        }).FirstOrDefault(),

                        // License Details
                        License = c.LicenseDetails.Select(l => new
                        {
                            l.Id,
                            l.LicenseNumber,
                            l.ProvinceId,
                            ProvinceName = l.Province != null ? l.Province.Dari : null,
                            l.LicenseType,
                            l.LicenseCategory,
                            l.RenewalRound,
                            l.DuplicateIssueDate,
                            l.IssueDate,
                            l.ExpireDate,
                            l.OfficeAddress,
                            l.TransferLocation,
                            l.ActivityLocation,
                            l.RoyaltyAmount,
                            l.RoyaltyDate,
                            l.TariffNumber,
                            l.PenaltyAmount,
                            l.PenaltyDate,
                            l.HrLetter,
                            l.HrLetterDate,
                            l.DocPath
                        }).FirstOrDefault(),

                        // Guarantors
                        Guarantors = c.Guarantors.Select(g => new
                        {
                            g.Id,
                            g.FirstName,
                            g.FatherName,
                            g.GrandFatherName,
                            g.PhoneNumber,
                            g.ElectronicNationalIdNumber,
                            GuaranteeTypeName = g.GuaranteeType != null ? g.GuaranteeType.Name : null,
                            g.GuaranteeTypeId,
                            g.PropertyDocumentNumber,
                            g.PropertyDocumentDate,
                            g.SenderMaktobNumber,
                            g.SenderMaktobDate,
                            g.AnswerdMaktobNumber,
                            g.AnswerdMaktobDate,
                            g.DateofGuarantee,
                            g.GuaranteeDocNumber,
                            g.GuaranteeDate,
                            g.GuaranteeDocPath,
                            // Conditional fields
                            g.CourtName,
                            g.CollateralNumber,
                            g.SetSerialNumber,
                            GuaranteeDistrictName = g.GuaranteeDistrict != null ? g.GuaranteeDistrict.Dari : null,
                            g.BankName,
                            g.DepositNumber,
                            g.DepositDate,
                            // Addresses
                            PermanentProvinceName = g.PaddressProvince != null ? g.PaddressProvince.Dari : null,
                            PermanentDistrictName = g.PaddressDistrict != null ? g.PaddressDistrict.Dari : null,
                            g.PaddressVillage,
                            TemporaryProvinceName = g.TaddressProvince != null ? g.TaddressProvince.Dari : null,
                            TemporaryDistrictName = g.TaddressDistrict != null ? g.TaddressDistrict.Dari : null,
                            g.TaddressVillage
                        }).ToList(),

                        // Account Info (Financial/Tax)
                        AccountInfo = c.CompanyAccountInfos.Select(a => new
                        {
                            a.Id,
                            a.SettlementInfo,
                            a.TaxPaymentAmount,
                            a.SettlementYear,
                            a.TaxPaymentDate,
                            a.TransactionCount,
                            a.CompanyCommission
                        }).FirstOrDefault(),

                        // Cancellation Info
                        CancellationInfo = c.CompanyCancellationInfos.Select(ci => new
                        {
                            ci.Id,
                            ci.LicenseCancellationLetterNumber,
                            ci.RevenueCancellationLetterNumber,
                            ci.LicenseCancellationLetterDate,
                            ci.Remarks
                        }).FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    return NotFound(new { message = "???? ???? ???" });
                }

                return Ok(data);
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException)
            {
                return NotFound(new { message = "???? ???? ???" }); // Return 404 to avoid information leakage
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

                var userId = userIdClaim.Value;

                // Check for duplicate company name
                // Trim and normalize the title for comparison
                var normalizedTitle = request.Title.Trim().ToLower();
                var existingCompany = await _context.CompanyDetails
                    .FirstOrDefaultAsync(c => c.Title.Trim().ToLower() == normalizedTitle);
                
                if (existingCompany != null)
                {
                    // Check if the existing company has cancellation info (فسخ)
                    var cancellationInfo = await _context.CompanyCancellationInfos
                        .FirstOrDefaultAsync(c => c.CompanyId == existingCompany.Id);
                    
                    // If cancellation info exists and all three fields are filled
                    bool isRevoked = cancellationInfo != null && 
                        !string.IsNullOrWhiteSpace(cancellationInfo.LicenseCancellationLetterNumber) &&
                        !string.IsNullOrWhiteSpace(cancellationInfo.RevenueCancellationLetterNumber) &&
                        cancellationInfo.LicenseCancellationLetterDate.HasValue;
                    
                    if (isRevoked)
                    {
                        // If user hasn't confirmed, return a warning response
                        if (request.ConfirmRevokedDuplicate != true)
                        {
                            return BadRequest(new { 
                                requiresConfirmation = true,
                                message = "این عنوان رهنمایی معاملات قبلاً ثبت شده است اما جواز آن فسخ شده است. آیا میخواهید ادامه دهید؟" 
                            });
                        }
                        // If confirmed, allow creation (continue with the rest of the code)
                    }
                    else
                    {
                        // License is not revoked, block the creation
                        return BadRequest(new { message = "این عنوان رهنمایی معاملات قبلاً ثبت شده است" });
                    }
                }

                // Auto-populate province for COMPANY_REGISTRAR, use provided for administrators
                // Province can be null initially - it will be set when license is created
                var provinceId = _provinceFilter.IsAdministrator() 
                    ? request.ProvinceId 
                    : _provinceFilter.GetUserProvinceId();

                // Validate province access only if province is provided
                if (provinceId.HasValue)
                {
                    _provinceFilter.ValidateProvinceAccess(provinceId.Value);
                }

                var property = new CompanyDetail
                {
                    Title = request.Title,
                    Tin = request.Tin,
                    DocPath = request.DocPath,
                    ProvinceId = provinceId, // Can be null initially
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                };

                _context.Add(property);
                await _context.SaveChangesAsync();

                // Log the creation to comprehensive audit
                await _auditService.LogCreateAsync(
                    WebAPIBackend.Models.Audit.AuditModules.Company,
                    "Company",
                    property.Id.ToString(),
                    newValues: new { property.Id, property.Title, property.Tin, property.ProvinceId },
                    descriptionDari: $"ثبت رهنما {property.Title} با شماره {property.Id}");

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
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] CompanyDetailData request)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
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

            try
            {
                // Validate province access
                _provinceFilter.ValidateProvinceAccess(existingProperty.ProvinceId);

                // Check for duplicate company name (excluding current company)
                // Trim and normalize the title for comparison
                var normalizedTitle = request.Title.Trim().ToLower();
                var duplicateCompany = await _context.CompanyDetails
                    .Where(c => c.Id != id)
                    .FirstOrDefaultAsync(c => c.Title.Trim().ToLower() == normalizedTitle);
                
                if (duplicateCompany != null)
                {
                    return BadRequest(new { message = "این عنوان رهنمایی معاملات قبلاً ثبت شده است" });
                }

                // Store the original values of the CreatedBy, CreatedAt, and ProvinceId properties
                var createdBy = existingProperty.CreatedBy;
                var createdAt = existingProperty.CreatedAt;
                var originalProvinceId = existingProperty.ProvinceId; // Preserve province immutability

                // Update the entity with the new values
                existingProperty.Title = request.Title;
                existingProperty.Tin = request.Tin;
                existingProperty.DocPath = request.DocPath;

                // Restore the original values of the CreatedBy, CreatedAt, and ProvinceId properties
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
                    // Only add an entry to the audit table if the property has been modified
                    if (change.Value.OldValue != null && !change.Value.OldValue.Equals(change.Value.NewValue))
                    {
                        _context.Companydetailsaudits.Add(new Companydetailsaudit
                        {
                            CompanyId = existingProperty.Id,
                            UpdatedBy = userId,
                            UpdatedAt = DateTime.UtcNow,
                            PropertyName = change.Key,
                            OldValue = change.Value.OldValue?.ToString(),
                            NewValue = change.Value.NewValue?.ToString()
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Log the update to comprehensive audit
                await _auditService.LogUpdateAsync(
                    WebAPIBackend.Models.Audit.AuditModules.Company,
                    "Company",
                    id.ToString(),
                    oldValues: changes.Where(c => c.Value.OldValue != null && !c.Value.OldValue.Equals(c.Value.NewValue))
                        .ToDictionary(c => c.Key, c => c.Value.OldValue),
                    newValues: changes.Where(c => c.Value.OldValue != null && !c.Value.OldValue.Equals(c.Value.NewValue))
                        .ToDictionary(c => c.Key, c => c.Value.NewValue),
                    descriptionDari: $"تغییر رهنما {existingProperty.Title} با شماره {id}");

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

        [HttpGet("getCompanies")]
        public async Task<ActionResult<IEnumerable<Companies>>> GetCompanies()
        {
            try
            {
                // Apply province filtering
                var query = _context.CompanyDetails.AsNoTracking().AsQueryable();
                query = _provinceFilter.ApplyProvinceFilter(query);

                var com = await query
                    .OrderBy(u => u.Id)
                    .Select(u => new Companies { Id = u.Id, Title = u.Title, ProvinceId = u.ProvinceId })
                    .ToListAsync();

                return com;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// Search for company by license number and province
        /// </summary>
        [HttpGet("searchByLicense")]
        [Authorize]
        public async Task<IActionResult> SearchCompanyByLicense([FromQuery] string licenseNumber, [FromQuery] int? provinceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(licenseNumber))
                {
                    return BadRequest(new { message = "شماره جواز الزامی است" });
                }

                var query = _context.LicenseDetails
                    .Where(l => l.LicenseNumber == licenseNumber);

                if (provinceId.HasValue && provinceId.Value > 0)
                {
                    query = query.Where(l => l.ProvinceId == provinceId.Value);
                }

                var licenses = await query
                    .Select(l => new
                    {
                        CompanyId = l.CompanyId,
                        CompanyTitle = l.Company != null ? l.Company.Title : null,
                        CompanyName = l.Company != null ? l.Company.Title : null,
                        OwnerName = l.Company != null && l.Company.CompanyOwners.Any() 
                            ? l.Company.CompanyOwners.FirstOrDefault()!.FirstName 
                            : null,
                        OwnerFatherName = l.Company != null && l.Company.CompanyOwners.Any() 
                            ? l.Company.CompanyOwners.FirstOrDefault()!.FatherName 
                            : null,
                        LicenseNumber = l.LicenseNumber,
                        LicenseType = l.LicenseType,
                        ProvinceId = l.ProvinceId,
                        ProvinceName = l.Province != null ? l.Province.Dari : null,
                        ActivityLocation = l.ActivityLocation,
                        IssueDate = l.IssueDate,
                        ExpireDate = l.ExpireDate,
                        Status = l.Status
                    })
                    .ToListAsync();

                if (!licenses.Any())
                {
                    return NotFound(new { message = "هیچ رهنمای با این شماره جواز یافت نشد" });
                }

                return Ok(licenses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در جستجوی رهنما", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var company = await _context.CompanyDetails
                    .Include(c => c.CompanyOwners)
                    .Include(c => c.Guarantors)
                    .Include(c => c.Gaurantees)
                    .Include(c => c.CompanyAccountInfos)
                    .Include(c => c.CompanyCancellationInfos)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return NotFound(new { message = "???? ???? ???" });
                }

                // Validate province access
                _provinceFilter.ValidateProvinceAccess(company.ProvinceId);

                // Delete LicenseDetails and their audit records first
                var licenseDetails = await _context.LicenseDetails
                    .Where(l => l.CompanyId == id)
                    .ToListAsync();
                if (licenseDetails.Any())
                {
                    var licenseIds = licenseDetails.Select(l => l.Id).ToList();
                    var licenseAuditRecords = await _context.Licenseaudits
                        .Where(a => licenseIds.Contains(a.LicenseId))
                        .ToListAsync();
                    if (licenseAuditRecords.Any())
                    {
                        _context.Licenseaudits.RemoveRange(licenseAuditRecords);
                    }
                    _context.LicenseDetails.RemoveRange(licenseDetails);
                }

                // Delete audit records first
                var auditRecords = await _context.Companydetailsaudits
                    .Where(a => a.CompanyId == id)
                    .ToListAsync();
                if (auditRecords.Any())
                {
                    _context.Companydetailsaudits.RemoveRange(auditRecords);
                }

                // Delete CompanyOwnerAddress records (must be deleted before CompanyOwner)
                if (company.CompanyOwners != null && company.CompanyOwners.Any())
                {
                    var ownerIds = company.CompanyOwners.Select(o => o.Id).ToList();
                    
                    var ownerAddresses = await _context.CompanyOwnerAddresses
                        .Where(a => a.CompanyOwnerId.HasValue && ownerIds.Contains(a.CompanyOwnerId.Value))
                        .ToListAsync();
                    if (ownerAddresses.Any())
                    {
                        _context.CompanyOwnerAddresses.RemoveRange(ownerAddresses);
                    }

                    // Delete CompanyOwner audit records (must be deleted before CompanyOwner)
                    var ownerAuditRecords = await _context.Companyowneraudits
                        .Where(a => ownerIds.Contains(a.OwnerId))
                        .ToListAsync();
                    if (ownerAuditRecords.Any())
                    {
                        _context.Companyowneraudits.RemoveRange(ownerAuditRecords);
                    }
                }

                // Remove cancellation info
                if (company.CompanyCancellationInfos != null && company.CompanyCancellationInfos.Any())
                {
                    _context.CompanyCancellationInfos.RemoveRange(company.CompanyCancellationInfos);
                }

                // Remove account info
                if (company.CompanyAccountInfos != null && company.CompanyAccountInfos.Any())
                {
                    _context.CompanyAccountInfos.RemoveRange(company.CompanyAccountInfos);
                }

                // Remove guarantors
                if (company.Guarantors != null && company.Guarantors.Any())
                {
                    var guarantorIds = company.Guarantors.Select(g => g.Id).ToList();
                    
                    // Delete guarantor audit records first
                    var guarantorAuditRecords = await _context.Guarantorsaudits
                        .Where(a => guarantorIds.Contains(a.GuarantorsId))
                        .ToListAsync();
                    if (guarantorAuditRecords.Any())
                    {
                        _context.Guarantorsaudits.RemoveRange(guarantorAuditRecords);
                    }
                    
                    _context.Guarantors.RemoveRange(company.Guarantors);
                }

                // Remove gaurantees (note: different from Guarantors)
                if (company.Gaurantees != null && company.Gaurantees.Any())
                {
                    var gauranteeIds = company.Gaurantees.Select(g => g.Id).ToList();
                    
                    // Delete gaurantee audit records first
                    var gauranteeAuditRecords = await _context.Graunteeaudits
                        .Where(a => gauranteeIds.Contains(a.GauranteeId))
                        .ToListAsync();
                    if (gauranteeAuditRecords.Any())
                    {
                        _context.Graunteeaudits.RemoveRange(gauranteeAuditRecords);
                    }
                    
                    _context.Gaurantees.RemoveRange(company.Gaurantees);
                }

                // Remove license details
                if (company.LicenseDetails != null && company.LicenseDetails.Any())
                {
                    var licenseIds = company.LicenseDetails.Select(l => l.Id).ToList();
                    
                    // Delete license audit records first
                    var licenseAuditRecords = await _context.Licenseaudits
                        .Where(a => licenseIds.Contains(a.LicenseId))
                        .ToListAsync();
                    if (licenseAuditRecords.Any())
                    {
                        _context.Licenseaudits.RemoveRange(licenseAuditRecords);
                    }
                    
                    _context.LicenseDetails.RemoveRange(company.LicenseDetails);
                }

                // Remove company owners (after addresses are deleted)
                if (company.CompanyOwners != null && company.CompanyOwners.Any())
                {
                    _context.CompanyOwners.RemoveRange(company.CompanyOwners);
                }

                // Store company info for audit before deletion
                var companyInfo = new
                {
                    company.Id,
                    company.Title,
                    company.Tin,
                    company.ProvinceId
                };

                // Remove the company itself
                _context.CompanyDetails.Remove(company);

                await _context.SaveChangesAsync();

                // Log the deletion to comprehensive audit
                await _auditService.LogDeleteAsync(
                    WebAPIBackend.Models.Audit.AuditModules.Company,
                    "Company",
                    id.ToString(),
                    oldValues: companyInfo,
                    descriptionDari: $"حذف رهنما {company.Title} با شماره {id}");

                return Ok(new { message = "???? ?? ?????? ??? ??" });
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = $"??? ?? ??? ????: {ex.Message}",
                    details = ex.InnerException?.Message 
                });
            }
        }

        public class Companies
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public int? ProvinceId { get; set; }
        }

        #region Reports

        /// <summary>
        /// Get report: Count of cancellations (فسخ/لغوه) within date range
        /// <summary>
        /// Get report: Count of cancellations within date range
        /// Company module ALWAYS uses Hijri Shamsi calendar
        /// </summary>
        [HttpGet("reports/cancellations-count")]
        public async Task<IActionResult> GetCancellationsCountReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                // Auto-update license Status based on ExpireDate
                await _companyService.UpdateLicenseStatusByExpiryAsync();

                // Company module ALWAYS uses Hijri Shamsi - ignore calendarType parameter
                var calendar = CalendarType.HijriShamsi;
                var query = _context.CompanyCancellationInfos.Where(x => x.Status != false);

                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    {
                        parsedStartDate = start;
                        query = query.Where(x => x.LicenseCancellationLetterDate >= start);
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    {
                        parsedEndDate = end;
                        query = query.Where(x => x.LicenseCancellationLetterDate <= end);
                    }
                }

                var totalCancellations = await query.CountAsync();

                return Ok(new
                {
                    totalCancellations,
                    startDate = parsedStartDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedStartDate, calendar) : "",
                    endDate = parsedEndDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedEndDate, calendar) : "",
                    reportGeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get report: Count of active and inactive companies
        /// Active = license expiry date (تاریخ ختم جواز) is >= today
        /// Inactive = license expiry date is < today or no license
        /// Note: Checks ALL companies regardless of date range
        /// Company module ALWAYS uses Hijri Shamsi calendar
        /// </summary>
        [HttpGet("reports/companies-status")]
        public async Task<IActionResult> GetCompaniesStatusReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                // Company module ALWAYS uses Hijri Shamsi - ignore calendarType parameter
                var calendar = CalendarType.HijriShamsi;
                
                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    {
                        parsedStartDate = start;
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    {
                        parsedEndDate = end;
                    }
                }

                // Get ALL companies (not filtered by date)
                var allCompaniesQuery = _context.CompanyDetails.AsNoTracking().AsQueryable();
                allCompaniesQuery = _provinceFilter.ApplyProvinceFilter(allCompaniesQuery);
                var allCompanyIds = await allCompaniesQuery.Select(c => c.Id).ToListAsync();
                
                // Get licenses for these companies to check expiry dates
                var today = DateOnly.FromDateTime(DateTime.Today);
                var licensesForStatus = await _context.LicenseDetails
                    .Where(l => l.Status == true && l.CompanyId.HasValue && allCompanyIds.Contains(l.CompanyId.Value))
                    .Select(l => new { l.CompanyId, l.ExpireDate })
                    .ToListAsync();
                
                // Group by company and get the latest expiry date for each company
                var companyExpiryStatus = licensesForStatus
                    .GroupBy(l => l.CompanyId)
                    .Select(g => new
                    {
                        CompanyId = g.Key,
                        LatestExpireDate = g.Max(l => l.ExpireDate),
                        IsActive = g.Max(l => l.ExpireDate).HasValue && g.Max(l => l.ExpireDate) >= today
                    })
                    .ToList();
                
                // Count active and inactive companies based on license expiry
                var activeCount = companyExpiryStatus.Count(c => c.IsActive);
                var inactiveCount = companyExpiryStatus.Count(c => !c.IsActive);
                
                // Add companies without licenses as inactive
                var companiesWithoutLicenses = allCompanyIds.Count - companyExpiryStatus.Count;
                inactiveCount += companiesWithoutLicenses;
                
                var totalCompanies = activeCount + inactiveCount;

                return Ok(new
                {
                    activeCompanies = activeCount,
                    inactiveCompanies = inactiveCount,
                    totalCompanies,
                    startDate = parsedStartDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedStartDate, calendar) : "",
                    endDate = parsedEndDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedEndDate, calendar) : "",
                    reportGeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get report: Count of licenses by category (نوعیت جواز) within date range
        /// Categories: جدید (New), تجدید (Renewal), مثنی (Duplicate)
        /// Company module ALWAYS uses Hijri Shamsi calendar
        /// </summary>
        [HttpGet("reports/licenses-by-category")]
        public async Task<IActionResult> GetLicensesByCategoryReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                // Auto-update license Status based on ExpireDate
                await _companyService.UpdateLicenseStatusByExpiryAsync();

                // Company module ALWAYS uses Hijri Shamsi - ignore calendarType parameter
                var calendar = CalendarType.HijriShamsi;
                
                // Get licenses within date range
                var licensesQuery = _context.LicenseDetails.AsQueryable();
                
                // Apply province filter through company
                var allowedCompanyIds = _provinceFilter.ApplyProvinceFilter(_context.CompanyDetails)
                    .Select(c => c.Id)
                    .ToList();
                licensesQuery = licensesQuery.Where(l => l.CompanyId.HasValue && allowedCompanyIds.Contains(l.CompanyId.Value));

                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    {
                        parsedStartDate = start;
                        // For مثنی licenses use DuplicateIssueDate, for others use IssueDate
                        licensesQuery = licensesQuery.Where(x =>
                            (x.LicenseCategory == "مثنی" ? x.DuplicateIssueDate >= start : x.IssueDate >= start));
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    {
                        parsedEndDate = end;
                        // For مثنی licenses use DuplicateIssueDate, for others use IssueDate
                        licensesQuery = licensesQuery.Where(x =>
                            (x.LicenseCategory == "مثنی" ? x.DuplicateIssueDate <= end : x.IssueDate <= end));
                    }
                }

                var licensesByCategory = await licensesQuery
                    .GroupBy(l => l.LicenseCategory ?? "نامشخص")
                    .Select(g => new
                    {
                        category = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                var totalLicenses = licensesByCategory.Sum(l => l.count);

                return Ok(new
                {
                    licensesByCategory,
                    totalLicenses,
                    startDate = parsedStartDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedStartDate, calendar) : "",
                    endDate = parsedEndDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedEndDate, calendar) : "",
                    reportGeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get report: Count of guarantors by type within date range
        /// Company module ALWAYS uses Hijri Shamsi calendar
        /// </summary>
        [HttpGet("reports/guarantors-by-type")]
        public async Task<IActionResult> GetGuarantorsByTypeReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                // Auto-update license Status based on ExpireDate
                await _companyService.UpdateLicenseStatusByExpiryAsync();

                // Company module ALWAYS uses Hijri Shamsi - ignore calendarType parameter
                var calendar = CalendarType.HijriShamsi;
                
                // Get guarantors within date range
                var guarantorsQuery = _context.Guarantors.Where(x => x.Status != false && x.IsActive == true);
                
                // Apply province filter through company
                var allowedCompanyIds = _provinceFilter.ApplyProvinceFilter(_context.CompanyDetails)
                    .Select(c => c.Id)
                    .ToList();
                guarantorsQuery = guarantorsQuery.Where(g => g.CompanyId.HasValue && allowedCompanyIds.Contains(g.CompanyId.Value));

                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    {
                        parsedStartDate = start;
                        var startDateTime = start.ToDateTime(TimeOnly.MinValue);
                        guarantorsQuery = guarantorsQuery.Where(x => x.CreatedAt >= startDateTime);
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    {
                        parsedEndDate = end;
                        var endDateTime = end.ToDateTime(TimeOnly.MaxValue);
                        guarantorsQuery = guarantorsQuery.Where(x => x.CreatedAt <= endDateTime);
                    }
                }

                var guarantorsByType = await guarantorsQuery
                    .Where(g => g.GuaranteeTypeId.HasValue)
                    .GroupBy(g => g.GuaranteeTypeId!.Value)
                    .Select(g => new
                    {
                        guaranteeTypeId = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                // Get guarantee type names
                var guaranteeTypes = await _cache.GetGuaranteeTypesAsync();

                var result = guarantorsByType.Select(g => new
                {
                    g.guaranteeTypeId,
                    guaranteeTypeName = guaranteeTypes.FirstOrDefault(gt => gt.Id == g.guaranteeTypeId)?.Name ?? "Unknown",
                    g.count
                }).ToList();

                var totalGuarantors = result.Sum(r => r.count);

                return Ok(new
                {
                    guarantorsByType = result,
                    totalGuarantors,
                    startDate = parsedStartDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedStartDate, calendar) : "",
                    endDate = parsedEndDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedEndDate, calendar) : "",
                    reportGeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get comprehensive report with all statistics
        /// Company module ALWAYS uses Hijri Shamsi calendar
        /// </summary>
        [HttpGet("reports/comprehensive")]
        public async Task<IActionResult> GetComprehensiveReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? districtId = null)
        {
            try
            {
                // Auto-update license Status based on ExpireDate (تاریخ ختم جواز)
                // If ExpireDate has passed: Status = false (inactive); otherwise Status = true (active)
                await _companyService.UpdateLicenseStatusByExpiryAsync();

                // Company module ALWAYS uses Hijri Shamsi - ignore calendarType parameter
                var calendar = CalendarType.HijriShamsi;
                
                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    {
                        parsedStartDate = start;
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    {
                        parsedEndDate = end;
                    }
                }

                // Get cancellations count (date range)
                var cancellationsQuery = _context.CompanyCancellationInfos.Where(x => x.Status != false);
                if (parsedStartDate.HasValue)
                {
                    cancellationsQuery = cancellationsQuery.Where(x => x.LicenseCancellationLetterDate >= parsedStartDate);
                }
                if (parsedEndDate.HasValue)
                {
                    cancellationsQuery = cancellationsQuery.Where(x => x.LicenseCancellationLetterDate <= parsedEndDate);
                }
                var totalCancellations = await cancellationsQuery.CountAsync();

                // Get all-time cancellations count (no date filter)
                var totalCancellationsAllTime = await _context.CompanyCancellationInfos.AsNoTracking().Where(x => x.Status != false).CountAsync();

                // Get companies status based on license expiry date
                // Note: We check ALL companies' license status, not just those created in date range
                var allCompaniesQuery = _context.CompanyDetails.AsNoTracking().AsQueryable();
                allCompaniesQuery = _provinceFilter.ApplyProvinceFilter(allCompaniesQuery);
                
                // Apply province filter if specified
                if (provinceId.HasValue && provinceId.Value > 0)
                {
                    allCompaniesQuery = allCompaniesQuery.Where(c => c.ProvinceId == provinceId.Value);
                }
                
                // Get all company IDs (not filtered by date, but filtered by province if specified)
                var allCompanyIds = await allCompaniesQuery.Select(c => c.Id).ToListAsync();
                
                // Get licenses for these companies to check expiry dates and apply district filter
                var today = DateOnly.FromDateTime(DateTime.Today);
                var licensesForStatusQuery = _context.LicenseDetails
                    .Where(l => l.CompanyId.HasValue && allCompanyIds.Contains(l.CompanyId.Value));
                
                // Apply district filter if specified
                if (districtId.HasValue && districtId.Value > 0)
                {
                    // Filter by license province (which represents district in the license context)
                    licensesForStatusQuery = licensesForStatusQuery.Where(l => l.ProvinceId == districtId.Value);
                }
                
                var licensesForStatus = await licensesForStatusQuery
                    .Select(l => new { l.CompanyId, l.ExpireDate })
                    .ToListAsync();
                
                // Group by company and get the latest expiry date for each company
                var companyExpiryStatus = licensesForStatus
                    .GroupBy(l => l.CompanyId)
                    .Select(g => new
                    {
                        CompanyId = g.Key,
                        LatestExpireDate = g.Max(l => l.ExpireDate),
                        IsActive = g.Max(l => l.ExpireDate).HasValue && g.Max(l => l.ExpireDate) >= today
                    })
                    .ToList();
                
                // Count active and inactive companies based on license expiry
                var activeCompanies = companyExpiryStatus.Count(c => c.IsActive);
                var inactiveCompanies = companyExpiryStatus.Count(c => !c.IsActive);
                
                // Add companies without licenses as inactive
                var companiesWithoutLicenses = allCompanyIds.Count - companyExpiryStatus.Count;
                inactiveCompanies += companiesWithoutLicenses;
                
                var totalCompanies = activeCompanies + inactiveCompanies;

                // Get licenses by category
                var allowedCompanyIds = await _provinceFilter.ApplyProvinceFilter(_context.CompanyDetails)
                    .Select(c => c.Id)
                    .ToListAsync();
                
                // Apply province filter to allowed companies
                if (provinceId.HasValue && provinceId.Value > 0)
                {
                    var provinceFilteredCompanyIds = await _context.CompanyDetails
                        .Where(c => c.ProvinceId == provinceId.Value && allowedCompanyIds.Contains(c.Id))
                        .Select(c => c.Id)
                        .ToListAsync();
                    allowedCompanyIds = provinceFilteredCompanyIds;
                }
                
                var licensesQuery = _context.LicenseDetails
                    .Where(x => x.CompanyId.HasValue && allowedCompanyIds.Contains(x.CompanyId.Value));
                
                // Apply district filter
                if (districtId.HasValue && districtId.Value > 0)
                {
                    licensesQuery = licensesQuery.Where(x => x.ProvinceId == districtId.Value);
                }
                
                if (parsedStartDate.HasValue)
                {
                    // For مثنی licenses use DuplicateIssueDate, for others use IssueDate
                    licensesQuery = licensesQuery.Where(x =>
                        (x.LicenseCategory == "مثنی" ? x.DuplicateIssueDate >= parsedStartDate : x.IssueDate >= parsedStartDate));
                }
                if (parsedEndDate.HasValue)
                {
                    // For مثنی licenses use DuplicateIssueDate, for others use IssueDate
                    licensesQuery = licensesQuery.Where(x =>
                        (x.LicenseCategory == "مثنی" ? x.DuplicateIssueDate <= parsedEndDate : x.IssueDate <= parsedEndDate));
                }
                
                // Get all licenses that match the criteria
                var allLicenses = await licensesQuery.ToListAsync();

                
                // Group by LicenseCategory with proper handling of NULL/empty values
                var licensesByCategory = allLicenses
                    .GroupBy(l => string.IsNullOrWhiteSpace(l.LicenseCategory) ? "نامشخص" : l.LicenseCategory)
                    .Select(g => new
                    {
                        category = g.Key,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .ToList();
                    
                var totalLicenses = licensesByCategory.Sum(l => l.count);

                // Get guarantors by type
                var guarantorsQuery = _context.Guarantors
                    .Where(x => x.Status != false && x.IsActive == true && x.CompanyId.HasValue && allowedCompanyIds.Contains(x.CompanyId.Value));
                if (parsedStartDate.HasValue)
                {
                    var startDateTime = parsedStartDate.Value.ToDateTime(TimeOnly.MinValue);
                    guarantorsQuery = guarantorsQuery.Where(x => x.CreatedAt >= startDateTime);
                }
                if (parsedEndDate.HasValue)
                {
                    var endDateTime = parsedEndDate.Value.ToDateTime(TimeOnly.MaxValue);
                    guarantorsQuery = guarantorsQuery.Where(x => x.CreatedAt <= endDateTime);
                }
                var guarantorsByType = await guarantorsQuery
                    .Where(g => g.GuaranteeTypeId.HasValue)
                    .GroupBy(g => g.GuaranteeTypeId!.Value)
                    .Select(g => new
                    {
                        guaranteeTypeId = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                var guaranteeTypes = await _cache.GetGuaranteeTypesAsync();
                var guarantorsResult = guarantorsByType.Select(g => new
                {
                    g.guaranteeTypeId,
                    guaranteeTypeName = guaranteeTypes.FirstOrDefault(gt => gt.Id == g.guaranteeTypeId)?.Name ?? "Unknown",
                    g.count
                }).ToList();
                var totalGuarantors = guarantorsResult.Sum(r => r.count);

                // Get licenses by type (املاک vs موټر فروشی) with revenue calculation
                // Use the already filtered allLicenses list instead of querying again
                var licensesByType = allLicenses
                    .GroupBy(l => string.IsNullOrWhiteSpace(l.LicenseType) ? "realEstate" : l.LicenseType)
                    .Select(g => new
                    {
                        licenseType = g.Key,
                        count = g.Count()
                    })
                    .ToList();

                // Calculate revenue based on license type
                // املاک (realEstate) = 20,000 AFN per license
                // موټر فروشی (carSale) = 25,000 AFN per license
                // The database stores English values: 'realEstate' and 'carSale'
                var amlakCount = licensesByType.FirstOrDefault(l => 
                    l.licenseType != null && 
                    (l.licenseType.Equals("realEstate", StringComparison.OrdinalIgnoreCase) || 
                     l.licenseType == "املاک"))?.count ?? 0;
                var motorCount = licensesByType.FirstOrDefault(l => 
                    l.licenseType != null && 
                    (l.licenseType.Equals("carSale", StringComparison.OrdinalIgnoreCase) || 
                     l.licenseType == "موټر فروشی"))?.count ?? 0;
                
                var amlakRevenue = amlakCount * 20000;
                var motorRevenue = motorCount * 25000;
                var totalRevenue = amlakRevenue + motorRevenue;

                var licenseRevenueByType = new[]
                {
                    new
                    {
                        licenseType = "املاک",
                        count = amlakCount,
                        pricePerLicense = 20000,
                        totalRevenue = amlakRevenue
                    },
                    new
                    {
                        licenseType = "موټر فروشی",
                        count = motorCount,
                        pricePerLicense = 25000,
                        totalRevenue = motorRevenue
                    }
                };

                return Ok(new
                {
                    totalCancellations,
                    totalCancellationsAllTime,
                    activeCompanies,
                    inactiveCompanies,
                    totalCompanies,
                    licensesByCategory,
                    totalLicenses,
                    guarantorsByType = guarantorsResult,
                    totalGuarantors,
                    licenseRevenueByType,
                    totalRevenue,
                    startDate = parsedStartDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedStartDate, calendar) : "",
                    endDate = parsedEndDate.HasValue ? DateConversionHelper.FormatDateOnly(parsedEndDate, calendar) : "",
                    reportGeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}


