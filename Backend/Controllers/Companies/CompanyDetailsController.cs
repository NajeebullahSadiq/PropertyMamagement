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

        public CompanyDetailsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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
                    return StatusCode(403, new { message = "شما اجازه دسترسی به ماژول شرکت ها را ندارید" });
                }

                var query = from p in _context.CompanyDetails
                            select new
                            {
                                p.Id,
                                p.Title,
                                p.PhoneNumber,
                                ownerFullName= (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FirstName : null,
                                ownerFatherName= (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FatherName : null,
                                ownerIdNumber = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().IndentityCardNumber : null,
                                licenseNumber= (p.LicenseDetails != null && p.LicenseDetails.Any()) ? p.LicenseDetails.First().LicenseNumber:0,
                                granator= (p.Guarantors != null && p.Guarantors.Any()) ? p.Guarantors.First().FirstName+" "+"فرزند"+" "+ p.Guarantors.First().FatherName : null,
                            };
                return Ok(query);
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
                    return StatusCode(403, new { message = "شما اجازه دسترسی به ماژول شرکت ها را ندارید" });
                }

                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                var query = from p in _context.CompanyDetails
                            where p.LicenseDetails.Any(l => l.ExpireDate < currentDate)
                            select new
                            {
                                p.Id,
                                p.Title,
                                p.PhoneNumber,
                                ownerFullName = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FirstName : null,
                                ownerFatherName = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().FatherName : null,
                                ownerIdNumber = (p.CompanyOwners != null && p.CompanyOwners.Any()) ? p.CompanyOwners.First().IndentityCardNumber : null,
                                licenseNumber = (p.LicenseDetails != null && p.LicenseDetails.Any()) ? p.LicenseDetails.First().LicenseNumber : 0,
                                granator = (p.Guarantors != null && p.Guarantors.Any()) ? p.Guarantors.First().FirstName + " " + "فرزند" + " " + p.Guarantors.First().FatherName : null,
                            };
                return Ok(query);
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
                var Pro = await _context.CompanyDetails.Where(x => x.Id.Equals(id)).ToListAsync();

                // Convert dates to the requested calendar type (defaults to HijriShamsi)
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                foreach (var item in Pro)
                {
                    item.PetitionDate = DateConversionHelper.ToCalendarDateOnly(item.PetitionDate, calendar);
                }

                return Ok(Pro);
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
                    return StatusCode(403, new { message = "شما اجازه دسترسی به ماژول شرکت ها را ندارید" });
                }

                var data = await _context.CompanyDetails
                    .AsNoTracking()
                    .Where(c => c.Id == id)
                    .Select(c => new
                    {
                        // Basic Company Information
                        c.Id,
                        c.Title,
                        c.PhoneNumber,
                        c.LicenseNumber,
                        c.PetitionDate,
                        c.PetitionNumber,
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
                            o.IndentityCardNumber,
                            o.Jild,
                            o.Safha,
                            o.SabtNumber,
                            o.PothoPath,
                            EducationLevelName = o.EducationLevel != null ? o.EducationLevel.Dari : null,
                            IdentityCardTypeName = o.IdentityCardType != null ? o.IdentityCardType.Name : null,
                            // Owner's Own Address
                            OwnerProvinceName = o.OwnerProvince != null ? o.OwnerProvince.Dari : null,
                            OwnerDistrictName = o.OwnerDistrict != null ? o.OwnerDistrict.Dari : null,
                            o.OwnerVillage,
                            // Permanent Address
                            PermanentProvinceName = o.PermanentProvince != null ? o.PermanentProvince.Dari : null,
                            PermanentDistrictName = o.PermanentDistrict != null ? o.PermanentDistrict.Dari : null,
                            o.PermanentVillage,
                            // Temporary Address
                            TemporaryProvinceName = o.TemporaryProvince != null ? o.TemporaryProvince.Dari : null,
                            TemporaryDistrictName = o.TemporaryDistrict != null ? o.TemporaryDistrict.Dari : null,
                            o.TemporaryVillage
                        }).FirstOrDefault(),

                        // License Details
                        License = c.LicenseDetails.Select(l => new
                        {
                            l.Id,
                            l.LicenseNumber,
                            l.LicenseType,
                            l.LicenseCategory,
                            l.IssueDate,
                            l.ExpireDate,
                            l.OfficeAddress,
                            AreaName = l.Area != null ? l.Area.Name : null,
                            l.RoyaltyAmount,
                            l.RoyaltyDate,
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
                            g.IndentityCardNumber,
                            g.Jild,
                            g.Safha,
                            g.SabtNumber,
                            g.PothoPath,
                            IdentityCardTypeName = g.IdentityCardType != null ? g.IdentityCardType.Name : null,
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
                    return NotFound(new { message = "شرکت یافت نشد" });
                }

                return Ok(data);
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

                // Parse calendar type and validate date
                if (string.IsNullOrWhiteSpace(request.PetitionDate))
                {
                    return BadRequest("PetitionDate is required.");
                }

                var calendarType = DateConversionHelper.ParseCalendarType(request.CalendarType);
                if (!DateConversionHelper.TryParseToDateOnly(request.PetitionDate!, calendarType, out var pdate))
                {
                    return BadRequest($"PetitionDate must be a valid date in {calendarType} format (yyyy-MM-dd or yyyy/MM/dd).");
                }

                var userId = userIdClaim.Value;

                var property = new CompanyDetail
                {
                    Title = request.Title,
                    PhoneNumber = request.PhoneNumber,
                    LicenseNumber = request.LicenseNumber,
                    PetitionNumber = request.PetitionNumber,
                    PetitionDate = pdate,
                    Tin = request.Tin,
                    DocPath = request.DocPath,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };

                _context.Add(property);
                await _context.SaveChangesAsync();

                var result = new { Id = property.Id };
                return Ok(result);
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

            // Parse calendar type and validate date
            var calendarType = DateConversionHelper.ParseCalendarType(request.CalendarType);
            if (string.IsNullOrWhiteSpace(request.PetitionDate) || !DateConversionHelper.TryParseToDateOnly(request.PetitionDate!, calendarType, out var pdate))
            {
                return BadRequest($"PetitionDate is required and must be a valid date in {calendarType} format (yyyy-MM-dd or yyyy/MM/dd).");
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

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;

            // Update the entity with the new values
            existingProperty.Title = request.Title;
            existingProperty.PhoneNumber = request.PhoneNumber;
            existingProperty.LicenseNumber = request.LicenseNumber;
            existingProperty.PetitionNumber = request.PetitionNumber;
            existingProperty.PetitionDate = pdate;
            existingProperty.Tin = request.Tin;
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
                    _context.Companydetailsaudits.Add(new Companydetailsaudit
                    {
                        CompanyId = existingProperty.Id,
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

        [HttpGet("getCompanies")]
        public async Task<ActionResult<IEnumerable<Companies>>> GetCompanies()
        {
            try
            {
                var com = await _context.CompanyDetails
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
        public class Companies
        {
            public int Id { get; set; }
            public string? Title { get; set; }
        }
    }
}
