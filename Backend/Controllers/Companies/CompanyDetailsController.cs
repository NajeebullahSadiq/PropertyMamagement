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

        public CompanyDetailsController(
            AppDbContext context, 
            UserManager<ApplicationUser> userManager,
            WebAPIBackend.Services.IProvinceFilterService provinceFilter)
        {
            _context = context;
            _userManager = userManager;
            _provinceFilter = provinceFilter;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
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
                var query = _context.CompanyDetails.AsQueryable();
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
                
                // Limit to 100 records if no search is provided
                if (string.IsNullOrWhiteSpace(search))
                {
                    orderedQuery = (IOrderedQueryable<CompanyDetail>)orderedQuery.Take(100);
                }

                var result = await orderedQuery
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        ownerFullName = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.FirstName).FirstOrDefault(),
                        ownerFatherName = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.FatherName).FirstOrDefault(),
                        ownerElectronicNationalIdNumber = p.CompanyOwners.OrderBy(o => o.Id).Select(o => o.ElectronicNationalIdNumber).FirstOrDefault(),
                        licenseNumber = p.LicenseDetails.OrderBy(l => l.Id).Select(l => l.LicenseNumber).FirstOrDefault(),
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

                var result = await _context.CompanyDetails.Where(x => x.Id.Equals(id)).ToListAsync();
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
                            l.IssueDate,
                            l.ExpireDate,
                            l.OfficeAddress,
                            l.TransferLocation,
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
                    return BadRequest(new { message = "این عنوان رهنمایی معاملات قبلاً ثبت شده است" });
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
                var query = _context.CompanyDetails.AsQueryable();
                query = _provinceFilter.ApplyProvinceFilter(query);

                var com = await query
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
                    .Include(l => l.Company)
                        .ThenInclude(c => c.CompanyOwners)
                    .Include(l => l.Province)
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

                // Remove the company itself
                _context.CompanyDetails.Remove(company);

                await _context.SaveChangesAsync();

                return Ok(new { message = "???? ?? ?????? ??? ??" });
            }
            catch (WebAPIBackend.Models.Common.ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the detailed error for debugging
                Console.WriteLine($"Error deleting company: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
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
        }
    }
}


