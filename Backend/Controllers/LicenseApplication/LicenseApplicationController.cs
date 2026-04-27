using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.LicenseApplication;
using WebAPIBackend.Models.RequestData.LicenseApplication;
using WebAPIBackend.Services;

namespace WebAPIBackend.Controllers.LicenseApplication
{
    /// <summary>
    /// Controller for License Applications (ثبت درخواست متقاضیان جواز رهنمای معاملات)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseApplicationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILookupCacheService _cache;

        // Guarantee Type Constants
        private const int GuaranteeType_Cash = 1;
        private const int GuaranteeType_ShariaDeed = 2;
        private const int GuaranteeType_CustomaryDeed = 3;

        public LicenseApplicationController(AppDbContext context, ILookupCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        #region Main Application CRUD

        /// <summary>
        /// Get all license applications with pagination and search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var query = _context.LicenseApplications
                    .AsNoTracking()
                    .Where(x => x.Status == true)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.RequestSerialNumber.Contains(search) ||
                        x.ApplicantName.Contains(search) ||
                        (x.ApplicantFatherName != null && x.ApplicantFatherName.Contains(search)) ||
                        (x.ApplicantGrandfatherName != null && x.ApplicantGrandfatherName.Contains(search)) ||
                        (x.ApplicantElectronicNumber != null && x.ApplicantElectronicNumber.Contains(search)) ||
                        x.ProposedGuideName.Contains(search));
                }

                var totalCount = await query.CountAsync();
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.RequestDate,
                        x.RequestSerialNumber,
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandfatherName,
                        x.ApplicantElectronicNumber,
                        x.ProposedGuideName,
                        x.PermanentProvinceId,
                        PermanentProvinceName = x.PermanentProvince != null ? x.PermanentProvince.Dari : "",
                        x.PermanentDistrictId,
                        PermanentDistrictName = x.PermanentDistrict != null ? x.PermanentDistrict.Dari : "",
                        x.PermanentVillage,
                        x.CurrentProvinceId,
                        CurrentProvinceName = x.CurrentProvince != null ? x.CurrentProvince.Dari : "",
                        x.CurrentDistrictId,
                        CurrentDistrictName = x.CurrentDistrict != null ? x.CurrentDistrict.Dari : "",
                        x.CurrentVillage,
                        x.IsWithdrawn,
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy
                    }).ToListAsync();

                // Get guarantors for each application
                var applicationIds = items.Select(x => x.Id).ToList();
                var guarantors = await _context.LicenseApplicationGuarantors
                    .AsNoTracking()
                    .Where(g => applicationIds.Contains(g.LicenseApplicationId))
                    .Select(g => new
                    {
                        g.Id,
                        g.LicenseApplicationId,
                        g.GuarantorName,
                        g.GuarantorFatherName,
                        g.GuaranteeTypeId,
                        GuaranteeTypeName = g.GuaranteeType != null ? g.GuaranteeType.Name : "",
                        g.GuaranteeLocation
                    })
                    .ToListAsync();

                var result = items.Select(x => new
                {
                    x.Id,
                    x.RequestDate,
                    RequestDateFormatted = x.RequestDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.RequestDate, calendar)
                        : "",
                    x.RequestSerialNumber,
                    x.ApplicantName,
                    x.ApplicantFatherName,
                    x.ApplicantGrandfatherName,
                    x.ApplicantElectronicNumber,
                    x.ProposedGuideName,
                    x.PermanentProvinceId,
                    x.PermanentProvinceName,
                    x.PermanentDistrictId,
                    x.PermanentDistrictName,
                    x.PermanentVillage,
                    x.CurrentProvinceId,
                    x.CurrentProvinceName,
                    x.CurrentDistrictId,
                    x.CurrentDistrictName,
                    x.CurrentVillage,
                    x.IsWithdrawn,
                    x.Status,
                    x.CreatedAt,
                    x.CreatedBy,
                    Guarantors = guarantors
                        .Where(g => g.LicenseApplicationId == x.Id)
                        .Select(g => new
                        {
                            g.Id,
                            g.GuarantorName,
                            g.GuarantorFatherName,
                            g.GuaranteeTypeId,
                            g.GuaranteeTypeName,
                            g.GuaranteeLocation
                        })
                        .ToList()
                }).ToList();

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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Advanced search for license applications
        /// Search fields: نمبر مسلسل، تاریخ درخواست، نام متقاضی، نام پیشنهادی رهنما، نمبر الکترونیکی، نمبر قباله شرعی، سریال نمبر سته قباله عرفی، شهرت تضمین‌کننده
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? serialNumber = null,
            [FromQuery] string? requestDate = null,
            [FromQuery] string? applicantName = null,
            [FromQuery] string? applicantFatherName = null,
            [FromQuery] string? proposedGuideName = null,
            [FromQuery] string? electronicNumber = null,
            [FromQuery] string? shariaDeedNumber = null,
            [FromQuery] string? customaryDeedSerial = null,
            [FromQuery] string? guarantorName = null,
            [FromQuery] string? guarantorFatherName = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                // Start with base query
                var query = _context.LicenseApplications
                    .AsNoTracking()
                    .Where(x => x.Status == true)
                    .AsQueryable();

                // Filter by serial number (نمبر مسلسل)
                if (!string.IsNullOrWhiteSpace(serialNumber))
                {
                    query = query.Where(x => x.RequestSerialNumber.Contains(serialNumber));
                }

                // Filter by request date (تاریخ درخواست)
                if (!string.IsNullOrWhiteSpace(requestDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(requestDate, calendarType, out var parsedDate))
                    {
                        query = query.Where(x => x.RequestDate == parsedDate);
                    }
                }

                // Filter by applicant name (شهرت متقاضی)
                if (!string.IsNullOrWhiteSpace(applicantName))
                {
                    query = query.Where(x => x.ApplicantName.Contains(applicantName));
                }

                // Filter by applicant father name (نام پدر متقاضی)
                if (!string.IsNullOrWhiteSpace(applicantFatherName))
                {
                    query = query.Where(x => x.ApplicantFatherName != null && x.ApplicantFatherName.Contains(applicantFatherName));
                }

                // Filter by proposed guide name (نام پیشنهادی رهنما)
                if (!string.IsNullOrWhiteSpace(proposedGuideName))
                {
                    query = query.Where(x => x.ProposedGuideName.Contains(proposedGuideName));
                }

                // Filter by electronic number (نمبر الکترونیکی)
                if (!string.IsNullOrWhiteSpace(electronicNumber))
                {
                    query = query.Where(x => x.ApplicantElectronicNumber != null && x.ApplicantElectronicNumber.Contains(electronicNumber));
                }

                // Filter by Sharia deed number or Customary deed serial or Guarantor name or Guarantor father name
                // These require joining with guarantors table
                if (!string.IsNullOrWhiteSpace(shariaDeedNumber) || 
                    !string.IsNullOrWhiteSpace(customaryDeedSerial) || 
                    !string.IsNullOrWhiteSpace(guarantorName) ||
                    !string.IsNullOrWhiteSpace(guarantorFatherName))
                {
                    var guarantorQuery = _context.LicenseApplicationGuarantors.AsNoTracking().AsQueryable();

                    // Filter by Sharia deed number (نمبر قباله شرعی)
                    if (!string.IsNullOrWhiteSpace(shariaDeedNumber))
                    {
                        guarantorQuery = guarantorQuery.Where(g => 
                            g.ShariaDeedNumber != null && g.ShariaDeedNumber.Contains(shariaDeedNumber));
                    }

                    // Filter by Customary deed serial (سریال نمبر سته قباله عرفی)
                    if (!string.IsNullOrWhiteSpace(customaryDeedSerial))
                    {
                        guarantorQuery = guarantorQuery.Where(g => 
                            g.CustomaryDeedSerialNumber != null && g.CustomaryDeedSerialNumber.Contains(customaryDeedSerial));
                    }

                    // Filter by Guarantor name (شهرت تضمین‌کننده)
                    if (!string.IsNullOrWhiteSpace(guarantorName))
                    {
                        guarantorQuery = guarantorQuery.Where(g => 
                            g.GuarantorName.Contains(guarantorName));
                    }

                    // Filter by Guarantor father name (نام پدر تضمین‌کننده)
                    if (!string.IsNullOrWhiteSpace(guarantorFatherName))
                    {
                        guarantorQuery = guarantorQuery.Where(g => 
                            g.GuarantorFatherName != null && g.GuarantorFatherName.Contains(guarantorFatherName));
                    }

                    var applicationIdsWithGuarantors = await guarantorQuery
                        .Select(g => g.LicenseApplicationId)
                        .Distinct()
                        .ToListAsync();

                    query = query.Where(x => applicationIdsWithGuarantors.Contains(x.Id));
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.RequestDate,
                        x.RequestSerialNumber,
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandfatherName,
                        x.ApplicantElectronicNumber,
                        x.ProposedGuideName,
                        x.PermanentProvinceId,
                        PermanentProvinceName = x.PermanentProvince != null ? x.PermanentProvince.Dari : "",
                        x.PermanentDistrictId,
                        PermanentDistrictName = x.PermanentDistrict != null ? x.PermanentDistrict.Dari : "",
                        x.PermanentVillage,
                        x.CurrentProvinceId,
                        CurrentProvinceName = x.CurrentProvince != null ? x.CurrentProvince.Dari : "",
                        x.CurrentDistrictId,
                        CurrentDistrictName = x.CurrentDistrict != null ? x.CurrentDistrict.Dari : "",
                        x.CurrentVillage,
                        x.IsWithdrawn,
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy
                    }).ToListAsync();

                // Get guarantors for each application
                var applicationIds = items.Select(x => x.Id).ToList();
                var guarantors = await _context.LicenseApplicationGuarantors
                    .AsNoTracking()
                    .Where(g => applicationIds.Contains(g.LicenseApplicationId))
                    .Select(g => new
                    {
                        g.Id,
                        g.LicenseApplicationId,
                        g.GuarantorName,
                        g.GuarantorFatherName,
                        g.GuaranteeTypeId,
                        GuaranteeTypeName = g.GuaranteeType != null ? g.GuaranteeType.Name : "",
                        g.CashAmount,
                        g.ShariaDeedNumber,
                        g.ShariaDeedDate,
                        g.CustomaryDeedSerialNumber,
                        g.GuaranteeLocation
                    })
                    .ToListAsync();

                var result = items.Select(x => new
                {
                    x.Id,
                    x.RequestDate,
                    RequestDateFormatted = x.RequestDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.RequestDate, calendar)
                        : "",
                    x.RequestSerialNumber,
                    x.ApplicantName,
                    x.ApplicantFatherName,
                    x.ApplicantGrandfatherName,
                    x.ApplicantElectronicNumber,
                    x.ProposedGuideName,
                    x.PermanentProvinceId,
                    x.PermanentProvinceName,
                    x.PermanentDistrictId,
                    x.PermanentDistrictName,
                    x.PermanentVillage,
                    x.CurrentProvinceId,
                    x.CurrentProvinceName,
                    x.CurrentDistrictId,
                    x.CurrentDistrictName,
                    x.CurrentVillage,
                    x.IsWithdrawn,
                    x.Status,
                    x.CreatedAt,
                    x.CreatedBy,
                    Guarantors = guarantors
                        .Where(g => g.LicenseApplicationId == x.Id)
                        .Select(g => new
                        {
                            g.Id,
                            g.GuarantorName,
                            g.GuarantorFatherName,
                            g.GuaranteeTypeId,
                            g.GuaranteeTypeName,
                            g.CashAmount,
                            g.ShariaDeedNumber,
                            g.ShariaDeedDate,
                            ShariaDeedDateFormatted = g.ShariaDeedDate.HasValue
                                ? DateConversionHelper.FormatDateOnly(g.ShariaDeedDate, calendar)
                                : "",
                            g.CustomaryDeedSerialNumber,
                            g.GuaranteeLocation
                        })
                        .ToList()
                }).ToList();

                return Ok(new
                {
                    items = result,
                    totalCount,
                    page,
                    pageSize,
                    searchCriteria = new
                    {
                        serialNumber,
                        requestDate,
                        applicantName,
                        applicantFatherName,
                        proposedGuideName,
                        electronicNumber,
                        shariaDeedNumber,
                        customaryDeedSerial,
                        guarantorName,
                        guarantorFatherName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get license application by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var item = await _context.LicenseApplications
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.Status == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.RequestDate,
                        RequestDateFormatted = x.RequestDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.RequestDate, calendar)
                            : "",
                        x.RequestSerialNumber,
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandfatherName,
                        x.ApplicantElectronicNumber,
                        x.ProposedGuideName,
                        x.PermanentProvinceId,
                        PermanentProvinceName = x.PermanentProvince != null ? x.PermanentProvince.Dari : "",
                        x.PermanentDistrictId,
                        PermanentDistrictName = x.PermanentDistrict != null ? x.PermanentDistrict.Dari : "",
                        x.PermanentVillage,
                        x.CurrentProvinceId,
                        CurrentProvinceName = x.CurrentProvince != null ? x.CurrentProvince.Dari : "",
                        x.CurrentDistrictId,
                        CurrentDistrictName = x.CurrentDistrict != null ? x.CurrentDistrict.Dari : "",
                        x.CurrentVillage,
                        x.IsWithdrawn,
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound("درخواست یافت نشد");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new license application
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> Create([FromBody] LicenseApplicationData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Check for duplicate electronic number
                if (!string.IsNullOrWhiteSpace(request.ApplicantElectronicNumber))
                {
                    var existingWithSameElectronicNumber = await _context.LicenseApplications
                        .AsNoTracking().AnyAsync(x => x.ApplicantElectronicNumber == request.ApplicantElectronicNumber && x.Status == true);
                    
                    if (existingWithSameElectronicNumber)
                    {
                        return BadRequest($"نمبر الکترونیکی {request.ApplicantElectronicNumber} قبلاً ثبت شده است. لطفاً نمبر دیگری را وارد کنید.");
                    }
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.RequestDate, request.CalendarType, out var requestDate);

                var entity = new LicenseApplicationEntity
                {
                    RequestDate = requestDate,
                    RequestSerialNumber = request.RequestSerialNumber,
                    ApplicantName = request.ApplicantName,
                    ApplicantFatherName = request.ApplicantFatherName,
                    ApplicantGrandfatherName = request.ApplicantGrandfatherName,
                    ApplicantElectronicNumber = request.ApplicantElectronicNumber,
                    ProposedGuideName = request.ProposedGuideName,
                    PermanentProvinceId = request.PermanentProvinceId,
                    PermanentDistrictId = request.PermanentDistrictId,
                    PermanentVillage = request.PermanentVillage,
                    CurrentProvinceId = request.CurrentProvinceId,
                    CurrentDistrictId = request.CurrentDistrictId,
                    CurrentVillage = request.CurrentVillage,
                    Status = true,
                    IsWithdrawn = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.LicenseApplications.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "درخواست موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update license application
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> Update(int id, [FromBody] LicenseApplicationData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                var entity = await _context.LicenseApplications.FindAsync(id);
                if (entity == null || entity.Status == false)
                {
                    return NotFound("درخواست یافت نشد");
                }

                // Check for duplicate electronic number (excluding current record)
                if (!string.IsNullOrWhiteSpace(request.ApplicantElectronicNumber))
                {
                    var existingWithSameElectronicNumber = await _context.LicenseApplications
                        .AsNoTracking().AnyAsync(x => x.ApplicantElectronicNumber == request.ApplicantElectronicNumber 
                                    && x.Id != id 
                                    && x.Status == true);
                    
                    if (existingWithSameElectronicNumber)
                    {
                        return BadRequest($"نمبر الکترونیکی {request.ApplicantElectronicNumber} قبلاً ثبت شده است. لطفاً نمبر دیگری را وارد کنید.");
                    }
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.RequestDate, request.CalendarType, out var requestDate);

                entity.RequestDate = requestDate;
                entity.RequestSerialNumber = request.RequestSerialNumber;
                entity.ApplicantName = request.ApplicantName;
                entity.ApplicantFatherName = request.ApplicantFatherName;
                entity.ApplicantGrandfatherName = request.ApplicantGrandfatherName;
                entity.ApplicantElectronicNumber = request.ApplicantElectronicNumber;
                entity.ProposedGuideName = request.ProposedGuideName;
                entity.PermanentProvinceId = request.PermanentProvinceId;
                entity.PermanentDistrictId = request.PermanentDistrictId;
                entity.PermanentVillage = request.PermanentVillage;
                entity.CurrentProvinceId = request.CurrentProvinceId;
                entity.CurrentDistrictId = request.CurrentDistrictId;
                entity.CurrentVillage = request.CurrentVillage;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "درخواست موفقانه تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete (soft delete) license application
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.LicenseApplications.FindAsync(id);
                if (entity == null)
                {
                    return NotFound("درخواست یافت نشد");
                }

                entity.Status = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "درخواست موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Guarantors CRUD

        /// <summary>
        /// Get guarantors for a license application
        /// </summary>
        [HttpGet("{applicationId}/guarantors")]
        public async Task<IActionResult> GetGuarantors(int applicationId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var guarantors = await _context.LicenseApplicationGuarantors
                    .AsNoTracking()
                    .Where(x => x.LicenseApplicationId == applicationId)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseApplicationId,
                        x.GuarantorName,
                        x.GuarantorFatherName,
                        x.GuaranteeTypeId,
                        x.CashAmount,
                        x.ShariaDeedNumber,
                        x.ShariaDeedDate,
                        ShariaDeedDateFormatted = x.ShariaDeedDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ShariaDeedDate, calendar)
                            : "",
                        x.CustomaryDeedSerialNumber,
                        x.GuaranteeLocation,
                        x.PermanentProvinceId,
                        PermanentProvinceName = x.PermanentProvince != null ? x.PermanentProvince.Dari : "",
                        x.PermanentDistrictId,
                        PermanentDistrictName = x.PermanentDistrict != null ? x.PermanentDistrict.Dari : "",
                        x.PermanentVillage,
                        x.CurrentProvinceId,
                        CurrentProvinceName = x.CurrentProvince != null ? x.CurrentProvince.Dari : "",
                        x.CurrentDistrictId,
                        CurrentDistrictName = x.CurrentDistrict != null ? x.CurrentDistrict.Dari : "",
                        x.CurrentVillage,
                        x.CreatedAt
                    }).ToListAsync();

                return Ok(guarantors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add guarantor to license application
        /// </summary>
        [HttpPost("{applicationId}/guarantors")]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> AddGuarantor(int applicationId, [FromBody] LicenseApplicationGuarantorData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Check if application exists
                var application = await _context.LicenseApplications.FindAsync(applicationId);
                if (application == null || application.Status == false)
                {
                    return NotFound("درخواست یافت نشد");
                }

                // Check guarantor count (max 2)
                var guarantorCount = await _context.LicenseApplicationGuarantors
                    .CountAsync(x => x.LicenseApplicationId == applicationId);
                if (guarantorCount >= 2)
                {
                    return BadRequest("شما نمی‌توانید بیشتر از دو تضمین‌کننده ثبت کنید");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ShariaDeedDate, request.CalendarType, out var shariaDeedDate);

                var guarantor = new LicenseApplicationGuarantor
                {
                    LicenseApplicationId = applicationId,
                    GuarantorName = request.GuarantorName,
                    GuarantorFatherName = request.GuarantorFatherName,
                    GuaranteeTypeId = request.GuaranteeTypeId,
                    CashAmount = request.GuaranteeTypeId == GuaranteeType_Cash ? request.CashAmount : null,
                    ShariaDeedNumber = request.GuaranteeTypeId == GuaranteeType_ShariaDeed ? request.ShariaDeedNumber : null,
                    ShariaDeedDate = request.GuaranteeTypeId == GuaranteeType_ShariaDeed ? shariaDeedDate : null,
                    CustomaryDeedSerialNumber = request.GuaranteeTypeId == GuaranteeType_CustomaryDeed ? request.CustomaryDeedSerialNumber : null,
                    GuaranteeLocation = request.GuaranteeTypeId != GuaranteeType_Cash ? request.GuaranteeLocation : null,
                    PermanentProvinceId = request.PermanentProvinceId,
                    PermanentDistrictId = request.PermanentDistrictId,
                    PermanentVillage = request.PermanentVillage,
                    CurrentProvinceId = request.CurrentProvinceId,
                    CurrentDistrictId = request.CurrentDistrictId,
                    CurrentVillage = request.CurrentVillage,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.LicenseApplicationGuarantors.Add(guarantor);
                await _context.SaveChangesAsync();

                return Ok(new { id = guarantor.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update guarantor
        /// </summary>
        [HttpPut("{applicationId}/guarantors/{guarantorId}")]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> UpdateGuarantor(int applicationId, int guarantorId, [FromBody] LicenseApplicationGuarantorData request)
        {
            try
            {
                var guarantor = await _context.LicenseApplicationGuarantors
                    .FirstOrDefaultAsync(x => x.Id == guarantorId && x.LicenseApplicationId == applicationId);

                if (guarantor == null)
                {
                    return NotFound("تضمین‌کننده یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ShariaDeedDate, request.CalendarType, out var shariaDeedDate);

                guarantor.GuarantorName = request.GuarantorName;
                guarantor.GuarantorFatherName = request.GuarantorFatherName;
                guarantor.GuaranteeTypeId = request.GuaranteeTypeId;
                guarantor.CashAmount = request.GuaranteeTypeId == GuaranteeType_Cash ? request.CashAmount : null;
                guarantor.ShariaDeedNumber = request.GuaranteeTypeId == GuaranteeType_ShariaDeed ? request.ShariaDeedNumber : null;
                guarantor.ShariaDeedDate = request.GuaranteeTypeId == GuaranteeType_ShariaDeed ? shariaDeedDate : null;
                guarantor.CustomaryDeedSerialNumber = request.GuaranteeTypeId == GuaranteeType_CustomaryDeed ? request.CustomaryDeedSerialNumber : null;
                guarantor.GuaranteeLocation = request.GuaranteeTypeId != GuaranteeType_Cash ? request.GuaranteeLocation : null;
                guarantor.PermanentProvinceId = request.PermanentProvinceId;
                guarantor.PermanentDistrictId = request.PermanentDistrictId;
                guarantor.PermanentVillage = request.PermanentVillage;
                guarantor.CurrentProvinceId = request.CurrentProvinceId;
                guarantor.CurrentDistrictId = request.CurrentDistrictId;
                guarantor.CurrentVillage = request.CurrentVillage;

                await _context.SaveChangesAsync();

                return Ok(new { id = guarantor.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete guarantor
        /// </summary>
        [HttpDelete("{applicationId}/guarantors/{guarantorId}")]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> DeleteGuarantor(int applicationId, int guarantorId)
        {
            try
            {
                var guarantor = await _context.LicenseApplicationGuarantors
                    .FirstOrDefaultAsync(x => x.Id == guarantorId && x.LicenseApplicationId == applicationId);

                if (guarantor == null)
                {
                    return NotFound("تضمین‌کننده یافت نشد");
                }

                _context.LicenseApplicationGuarantors.Remove(guarantor);
                await _context.SaveChangesAsync();

                return Ok(new { message = "تضمین‌کننده موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Withdrawal CRUD

        /// <summary>
        /// Get withdrawal info for a license application
        /// </summary>
        [HttpGet("{applicationId}/withdrawal")]
        public async Task<IActionResult> GetWithdrawal(int applicationId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var entity = await _context.LicenseApplicationWithdrawals
                    .AsNoTracking()
                    .Where(x => x.LicenseApplicationId == applicationId)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return Ok(null);
                }

                var withdrawal = new
                {
                    entity.Id,
                    entity.LicenseApplicationId,
                    entity.WithdrawalReason,
                    entity.WithdrawalDate,
                    WithdrawalDateFormatted = entity.WithdrawalDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(entity.WithdrawalDate, calendar)
                        : "",
                    entity.CreatedAt
                };

                return Ok(withdrawal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Save withdrawal info (create or update)
        /// </summary>
        [HttpPost("{applicationId}/withdrawal")]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> SaveWithdrawal(int applicationId, [FromBody] LicenseApplicationWithdrawalData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Check if application exists
                var application = await _context.LicenseApplications.FindAsync(applicationId);
                if (application == null || application.Status == false)
                {
                    return NotFound("درخواست یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.WithdrawalDate, request.CalendarType, out var withdrawalDate);

                // Check if withdrawal already exists
                var existingWithdrawal = await _context.LicenseApplicationWithdrawals
                    .FirstOrDefaultAsync(x => x.LicenseApplicationId == applicationId);

                if (existingWithdrawal != null)
                {
                    // Update existing
                    existingWithdrawal.WithdrawalReason = request.WithdrawalReason;
                    existingWithdrawal.WithdrawalDate = withdrawalDate;
                }
                else
                {
                    // Create new
                    var withdrawal = new LicenseApplicationWithdrawal
                    {
                        LicenseApplicationId = applicationId,
                        WithdrawalReason = request.WithdrawalReason,
                        WithdrawalDate = withdrawalDate,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId
                    };
                    _context.LicenseApplicationWithdrawals.Add(withdrawal);
                }

                // Update application status
                application.IsWithdrawn = true;

                await _context.SaveChangesAsync();

                return Ok(new { id = existingWithdrawal?.Id ?? 0, message = "معلومات انصراف موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete withdrawal info
        /// </summary>
        [HttpDelete("{applicationId}/withdrawal")]
        [Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]
        public async Task<IActionResult> DeleteWithdrawal(int applicationId)
        {
            try
            {
                var withdrawal = await _context.LicenseApplicationWithdrawals
                    .FirstOrDefaultAsync(x => x.LicenseApplicationId == applicationId);

                if (withdrawal == null)
                {
                    return NotFound("معلومات انصراف یافت نشد");
                }

                // Update application status
                var application = await _context.LicenseApplications.FindAsync(applicationId);
                if (application != null)
                {
                    application.IsWithdrawn = false;
                }

                _context.LicenseApplicationWithdrawals.Remove(withdrawal);
                await _context.SaveChangesAsync();

                return Ok(new { message = "معلومات انصراف موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Reports

        /// <summary>
        /// Get report: Count of applicants saved in DB within date range
        /// </summary>
        [HttpGet("reports/applicants-count")]
        public async Task<IActionResult> GetApplicantsCountReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = _context.LicenseApplications.Where(x => x.Status == true);

                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendarType, out var start))
                    {
                        parsedStartDate = start;
                        query = query.Where(x => x.RequestDate >= start);
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendarType, out var end))
                    {
                        parsedEndDate = end;
                        query = query.Where(x => x.RequestDate <= end);
                    }
                }

                var totalCount = await query.CountAsync();

                return Ok(new
                {
                    totalApplicants = totalCount,
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
        /// </summary>
        [HttpGet("reports/guarantors-by-type")]
        public async Task<IActionResult> GetGuarantorsByTypeReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                
                // Get applications within date range
                var applicationsQuery = _context.LicenseApplications.Where(x => x.Status == true);

                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendarType, out var start))
                    {
                        parsedStartDate = start;
                        applicationsQuery = applicationsQuery.Where(x => x.RequestDate >= start);
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendarType, out var end))
                    {
                        parsedEndDate = end;
                        applicationsQuery = applicationsQuery.Where(x => x.RequestDate <= end);
                    }
                }

                var applicationIds = await applicationsQuery.Select(x => x.Id).ToListAsync();

                // Get guarantors for these applications
                var guarantorsQuery = _context.LicenseApplicationGuarantors
                    .Where(g => applicationIds.Contains(g.LicenseApplicationId));

                var guarantorsByType = await guarantorsQuery
                    .GroupBy(g => g.GuaranteeTypeId)
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
        /// Get report: Count of withdrawals (انصراف) within date range
        /// </summary>
        [HttpGet("reports/withdrawals-count")]
        public async Task<IActionResult> GetWithdrawalsCountReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = _context.LicenseApplicationWithdrawals.AsQueryable();

                DateOnly? parsedStartDate = null;
                DateOnly? parsedEndDate = null;

                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(startDate, calendarType, out var start))
                    {
                        parsedStartDate = start;
                        query = query.Where(x => x.WithdrawalDate >= start);
                    }
                }

                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (DateConversionHelper.TryParseToDateOnly(endDate, calendarType, out var end))
                    {
                        parsedEndDate = end;
                        query = query.Where(x => x.WithdrawalDate <= end);
                    }
                }

                var totalWithdrawals = await query.CountAsync();

                return Ok(new
                {
                    totalWithdrawals,
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
        /// </summary>
        [HttpGet("reports/comprehensive")]
        public async Task<IActionResult> GetComprehensiveReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                
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

                // Get applicants count
                var applicationsQuery = _context.LicenseApplications.Where(x => x.Status == true);
                if (parsedStartDate.HasValue)
                {
                    applicationsQuery = applicationsQuery.Where(x => x.RequestDate >= parsedStartDate);
                }
                if (parsedEndDate.HasValue)
                {
                    applicationsQuery = applicationsQuery.Where(x => x.RequestDate <= parsedEndDate);
                }
                var totalApplicants = await applicationsQuery.CountAsync();
                var applicationIds = await applicationsQuery.Select(x => x.Id).ToListAsync();

                // Get guarantors by type
                var guarantorsQuery = _context.LicenseApplicationGuarantors
                    .Where(g => applicationIds.Contains(g.LicenseApplicationId));

                var guarantorsByType = await guarantorsQuery
                    .GroupBy(g => g.GuaranteeTypeId)
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

                // Get withdrawals count
                var withdrawalsQuery = _context.LicenseApplicationWithdrawals.AsQueryable();
                if (parsedStartDate.HasValue)
                {
                    withdrawalsQuery = withdrawalsQuery.Where(x => x.WithdrawalDate >= parsedStartDate);
                }
                if (parsedEndDate.HasValue)
                {
                    withdrawalsQuery = withdrawalsQuery.Where(x => x.WithdrawalDate <= parsedEndDate);
                }
                var totalWithdrawals = await withdrawalsQuery.CountAsync();

                return Ok(new
                {
                    totalApplicants,
                    guarantorsByType = guarantorsResult,
                    totalGuarantors,
                    totalWithdrawals,
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
