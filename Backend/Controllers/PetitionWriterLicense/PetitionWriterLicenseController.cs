using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.PetitionWriterLicense;
using WebAPIBackend.Models.RequestData.PetitionWriterLicense;
using WebAPIBackend.Services;

namespace WebAPIBackend.Controllers.PetitionWriterLicense
{
    /// <summary>
    /// Controller for Petition Writer Licenses (ثبت جواز عریضه‌نویسان)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetitionWriterLicenseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILicenseNumberGenerator _licenseNumberGenerator;
        private readonly ILookupCacheService _cache;

        public PetitionWriterLicenseController(AppDbContext context, ILicenseNumberGenerator licenseNumberGenerator, ILookupCacheService cache)
        {
            _context = context;
            _licenseNumberGenerator = licenseNumberGenerator;
            _cache = cache;
        }

        #region Main License CRUD

        /// <summary>
        /// Get all petition writer licenses with pagination and search
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
                var query = _context.PetitionWriterLicenses
                    .AsNoTracking()
                    .Where(x => x.Status == true)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.LicenseNumber.Contains(search) ||
                        x.ApplicantName.Contains(search));
                }

                var totalCount = await query.CountAsync();
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var items = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseNumber,
                        x.ProvinceId,
                        ProvinceName = x.Province != null ? x.Province.Dari : "",
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandFatherName,
                        x.MobileNumber,
                        x.Competency,
                        x.ElectronicNationalIdNumber,
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
                        x.ActivityLocation,
                        x.ActivityNahia,
                        x.DetailedAddress,
                        x.PicturePath,
                        x.BankReceiptNumber,
                        x.BankReceiptDate,
                        BankReceiptDateFormatted = x.BankReceiptDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.BankReceiptDate, calendar)
                            : "",
                        x.District,
                        x.LicenseType,
                        x.LicensePrice,
                        x.LicenseCost,
                        x.LicenseIssueDate,
                        LicenseIssueDateFormatted = x.LicenseIssueDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.LicenseIssueDate, calendar)
                            : "",
                        x.LicenseExpiryDate,
                        LicenseExpiryDateFormatted = x.LicenseExpiryDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.LicenseExpiryDate, calendar)
                            : "",
                        x.LicenseStatus,
                        x.CancellationDate,
                        CancellationDateFormatted = x.CancellationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.CancellationDate, calendar)
                            : "",
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items,
                    totalCount,
                    page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a single petition writer license by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                // ALWAYS use Hijri Shamsi (Solar Hijri) calendar for print license dates
                var printCalendar = CalendarType.HijriShamsi;

                var item = await _context.PetitionWriterLicenses
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.Status == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseNumber,
                        x.ProvinceId,
                        ProvinceName = x.Province != null ? x.Province.Dari : "",
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandFatherName,
                        x.MobileNumber,
                        x.Competency,
                        x.ElectronicNationalIdNumber,
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
                        x.ActivityLocation,
                        x.ActivityNahia,
                        x.DetailedAddress,
                        x.PicturePath,
                        x.BankReceiptNumber,
                        x.BankReceiptDate,
                        BankReceiptDateFormatted = x.BankReceiptDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.BankReceiptDate, printCalendar)
                            : "",
                        x.District,
                        x.LicenseType,
                        x.LicensePrice,
                        x.LicenseCost,
                        x.LicenseIssueDate,
                        LicenseIssueDateFormatted = x.LicenseIssueDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.LicenseIssueDate, printCalendar)
                            : "",
                        x.LicenseExpiryDate,
                        LicenseExpiryDateFormatted = x.LicenseExpiryDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.LicenseExpiryDate, printCalendar)
                            : "",
                        x.LicenseStatus,
                        x.CancellationDate,
                        CancellationDateFormatted = x.CancellationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.CancellationDate, printCalendar)
                            : "",
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy,
                        x.UpdatedAt,
                        x.UpdatedBy,
                        Relocations = x.Relocations.OrderByDescending(r => r.RelocationDate).Select(r => new
                        {
                            r.Id,
                            r.NewActivityLocation,
                            r.RelocationDate,
                            RelocationDateFormatted = r.RelocationDate.HasValue
                                ? DateConversionHelper.FormatDateOnly(r.RelocationDate, printCalendar)
                                : "",
                            r.Remarks
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound(new { message = "جواز یافت نشد" });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new petition writer license
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,AUTHORITY,PETITION_WRITER_LICENSE_MANAGER")]
        public async Task<IActionResult> Create([FromBody] PetitionWriterLicenseData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Auto-generate license number if provinceId is provided
                string? licenseNumber = data.LicenseNumber;
                if (data.ProvinceId.HasValue && string.IsNullOrWhiteSpace(data.LicenseNumber))
                {
                    licenseNumber = await _licenseNumberGenerator.GenerateNextPetitionWriterLicenseNumber(data.ProvinceId.Value);
                }

                // Check for duplicate license number
                var exists = await _context.PetitionWriterLicenses
                    .AsNoTracking().AnyAsync(x => x.LicenseNumber == licenseNumber && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر جواز قبلاً ثبت شده است" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
                var username = User.Identity?.Name ?? "system";

                var entity = new PetitionWriterLicenseEntity
                {
                    LicenseNumber = licenseNumber ?? string.Empty,
                    ProvinceId = data.ProvinceId,
                    ApplicantName = data.ApplicantName,
                    ApplicantFatherName = data.ApplicantFatherName,
                    ApplicantGrandFatherName = data.ApplicantGrandFatherName,
                    MobileNumber = data.MobileNumber,
                    Competency = data.Competency,
                    ElectronicNationalIdNumber = data.ElectronicNationalIdNumber,
                    PermanentProvinceId = data.PermanentProvinceId,
                    PermanentDistrictId = data.PermanentDistrictId,
                    PermanentVillage = data.PermanentVillage,
                    CurrentProvinceId = data.CurrentProvinceId,
                    CurrentDistrictId = data.CurrentDistrictId,
                    CurrentVillage = data.CurrentVillage,
                    DetailedAddress = data.DetailedAddress,
                    PicturePath = data.PicturePath,
                    BankReceiptNumber = data.BankReceiptNumber,
                    BankReceiptDate = DateConversionHelper.ParseDateOnly(data.BankReceiptDate, calendar),
                    District = data.District,
                    LicenseType = data.LicenseType,
                    LicensePrice = data.LicensePrice,
                    LicenseCost = data.LicenseCost,
                    LicenseIssueDate = DateConversionHelper.ParseDateOnly(data.LicenseIssueDate, calendar),
                    LicenseExpiryDate = DateConversionHelper.ParseDateOnly(data.LicenseExpiryDate, calendar),
                    LicenseStatus = data.LicenseStatus ?? 1,
                    CancellationDate = DateConversionHelper.ParseDateOnly(data.CancellationDate, calendar),
                    ActivityLocation = data.ActivityLocation,
                    ActivityNahia = data.ActivityNahia,
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = username
                };

                _context.PetitionWriterLicenses.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, licenseNumber = entity.LicenseNumber, message = "جواز با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت جواز", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing petition writer license
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,AUTHORITY,PETITION_WRITER_LICENSE_MANAGER")]
        public async Task<IActionResult> Update(int id, [FromBody] PetitionWriterLicenseData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = await _context.PetitionWriterLicenses
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status == true);

                if (entity == null)
                {
                    return NotFound(new { message = "جواز یافت نشد" });
                }

                // Use existing license number if not provided in request
                string licenseNumber = !string.IsNullOrWhiteSpace(data.LicenseNumber) 
                    ? data.LicenseNumber 
                    : entity.LicenseNumber;

                // Check for duplicate license number (excluding current) only if license number changed
                if (licenseNumber != entity.LicenseNumber)
                {
                    var exists = await _context.PetitionWriterLicenses
                        .AsNoTracking().AnyAsync(x => x.LicenseNumber == licenseNumber && x.Id != id && x.Status == true);

                    if (exists)
                    {
                        return BadRequest(new { message = "نمبر جواز قبلاً ثبت شده است" });
                    }
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
                var username = User.Identity?.Name ?? "system";

                entity.LicenseNumber = licenseNumber;
                entity.ProvinceId = data.ProvinceId;
                entity.ApplicantName = data.ApplicantName;
                entity.ApplicantFatherName = data.ApplicantFatherName;
                entity.ApplicantGrandFatherName = data.ApplicantGrandFatherName;
                entity.MobileNumber = data.MobileNumber;
                entity.Competency = data.Competency;
                entity.ElectronicNationalIdNumber = data.ElectronicNationalIdNumber;
                entity.PermanentProvinceId = data.PermanentProvinceId;
                entity.PermanentDistrictId = data.PermanentDistrictId;
                entity.PermanentVillage = data.PermanentVillage;
                entity.CurrentProvinceId = data.CurrentProvinceId;
                entity.CurrentDistrictId = data.CurrentDistrictId;
                entity.CurrentVillage = data.CurrentVillage;
                entity.DetailedAddress = data.DetailedAddress;
                entity.PicturePath = data.PicturePath;
                entity.BankReceiptNumber = data.BankReceiptNumber;
                entity.BankReceiptDate = DateConversionHelper.ParseDateOnly(data.BankReceiptDate, calendar);
                entity.District = data.District;
                entity.LicenseType = data.LicenseType;
                entity.LicensePrice = data.LicensePrice;
                entity.LicenseCost = data.LicenseCost;
                entity.LicenseIssueDate = DateConversionHelper.ParseDateOnly(data.LicenseIssueDate, calendar);
                entity.LicenseExpiryDate = DateConversionHelper.ParseDateOnly(data.LicenseExpiryDate, calendar);
                entity.LicenseStatus = data.LicenseStatus ?? entity.LicenseStatus;
                entity.CancellationDate = DateConversionHelper.ParseDateOnly(data.CancellationDate, calendar);
                entity.ActivityLocation = data.ActivityLocation;
                entity.ActivityNahia = data.ActivityNahia;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = username;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "جواز با موفقیت تغییر یافت" });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "خطا در تغییر جواز", error = innerMessage });
            }
        }

        /// <summary>
        /// Delete (soft delete) a petition writer license
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.PetitionWriterLicenses
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status == true);

                if (entity == null)
                {
                    return NotFound(new { message = "جواز یافت نشد" });
                }

                var username = User.Identity?.Name ?? "system";

                entity.Status = false;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = username;

                await _context.SaveChangesAsync();

                return Ok(new { message = "جواز با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف جواز", error = ex.Message });
            }
        }

        /// <summary>
        /// Update license status (cancel/withdraw)
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] PetitionWriterLicenseStatusData data)
        {
            try
            {
                var entity = await _context.PetitionWriterLicenses
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status == true);

                if (entity == null)
                {
                    return NotFound(new { message = "جواز یافت نشد" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
                var username = User.Identity?.Name ?? "system";

                entity.LicenseStatus = data.LicenseStatus;
                entity.CancellationDate = DateConversionHelper.ParseDateOnly(data.CancellationDate, calendar);
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = username;

                await _context.SaveChangesAsync();

                return Ok(new { message = "وضعیت جواز با موفقیت تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در تغییر وضعیت", error = ex.Message });
            }
        }

        #endregion

        #region Provinces

        /// <summary>
        /// Get all provinces for dropdown
        /// </summary>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provinces = await _cache.GetProvincesAsync();
                var result = provinces.Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.Dari
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری ولایات", error = ex.Message });
            }
        }

        /// <summary>
        /// Get districts for a specific province (for dropdown - ناحیه)
        /// </summary>
        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistrictsByProvince(int provinceId)
        {
            try
            {
                var districts = await _cache.GetDistrictsAsync(provinceId);
                var result = districts.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Dari
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری ناحیه‌ها", error = ex.Message });
            }
        }

        /// <summary>
        /// Get activity locations for dropdown (محل فعالیت عریضه‌نویس)
        /// </summary>
        [HttpGet("activity-locations")]
        public async Task<IActionResult> GetActivityLocations()
        {
            try
            {
                var cachedItems = await _cache.GetActiveActivityLocationsAsync();
                var items = cachedItems.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.DariName
                }).ToList();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری محل فعالیت", error = ex.Message });
            }
        }

        #endregion

        #region Relocation CRUD

        /// <summary>
        /// Get all relocations for a license
        /// </summary>
        [HttpGet("{licenseId}/relocations")]
        public async Task<IActionResult> GetRelocations(int licenseId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var items = await _context.PetitionWriterRelocations
                    .AsNoTracking()
                    .Where(x => x.PetitionWriterLicenseId == licenseId)
                    .OrderByDescending(x => x.RelocationDate)
                    .Select(x => new
                    {
                        x.Id,
                        x.PetitionWriterLicenseId,
                        x.NewActivityLocation,
                        x.RelocationDate,
                        RelocationDateFormatted = x.RelocationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.RelocationDate, calendar)
                            : "",
                        x.Remarks,
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری نقل مکان‌ها", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new relocation
        /// </summary>
        [HttpPost("{licenseId}/relocations")]
        public async Task<IActionResult> CreateRelocation(int licenseId, [FromBody] PetitionWriterRelocationData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var licenseExists = await _context.PetitionWriterLicenses
                    .AsNoTracking().AnyAsync(x => x.Id == licenseId && x.Status == true);

                if (!licenseExists)
                {
                    return NotFound(new { message = "جواز یافت نشد" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
                var username = User.Identity?.Name ?? "system";

                var entity = new Models.PetitionWriterLicense.PetitionWriterRelocation
                {
                    PetitionWriterLicenseId = licenseId,
                    NewActivityLocation = data.NewActivityLocation,
                    RelocationDate = DateConversionHelper.ParseDateOnly(data.RelocationDate, calendar),
                    Remarks = data.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = username
                };

                _context.PetitionWriterRelocations.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "نقل مکان با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت نقل مکان", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a relocation
        /// </summary>
        [HttpPut("{licenseId}/relocations/{relocationId}")]
        public async Task<IActionResult> UpdateRelocation(int licenseId, int relocationId, [FromBody] PetitionWriterRelocationData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = await _context.PetitionWriterRelocations
                    .FirstOrDefaultAsync(x => x.Id == relocationId && x.PetitionWriterLicenseId == licenseId);

                if (entity == null)
                {
                    return NotFound(new { message = "نقل مکان یافت نشد" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);

                entity.NewActivityLocation = data.NewActivityLocation;
                entity.RelocationDate = DateConversionHelper.ParseDateOnly(data.RelocationDate, calendar);
                entity.Remarks = data.Remarks;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "نقل مکان با موفقیت تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در تغییر نقل مکان", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a relocation
        /// </summary>
        [HttpDelete("{licenseId}/relocations/{relocationId}")]
        public async Task<IActionResult> DeleteRelocation(int licenseId, int relocationId)
        {
            try
            {
                var entity = await _context.PetitionWriterRelocations
                    .FirstOrDefaultAsync(x => x.Id == relocationId && x.PetitionWriterLicenseId == licenseId);

                if (entity == null)
                {
                    return NotFound(new { message = "نقل مکان یافت نشد" });
                }

                _context.PetitionWriterRelocations.Remove(entity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "نقل مکان با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف نقل مکان", error = ex.Message });
            }
        }

        #endregion

        #region Report

        /// <summary>
        /// Get comprehensive report for petition writer licenses
        /// گزارش جامع جوازهای عریضه‌نویسان
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] string? activityLocation = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? districtId = null)
        {
            try
            {
                var calendar = CalendarType.HijriShamsi;

                DateOnly? parsedStart = null;
                DateOnly? parsedEnd = null;

                if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    parsedStart = start;

                if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    parsedEnd = end;

                // Base query - all active licenses
                var baseQuery = _context.PetitionWriterLicenses
                    .AsNoTracking()
                    .Where(x => x.Status == true);

                // Apply province filter (ProvinceId = license province)
                if (provinceId.HasValue && provinceId.Value > 0)
                    baseQuery = baseQuery.Where(x => x.ProvinceId == provinceId.Value);

                // Apply district filter (CurrentDistrictId = current address district)
                if (districtId.HasValue && districtId.Value > 0)
                    baseQuery = baseQuery.Where(x => x.CurrentDistrictId == districtId.Value);

                // Apply activity location filter
                if (!string.IsNullOrWhiteSpace(activityLocation))
                    baseQuery = baseQuery.Where(x => x.ActivityLocation == activityLocation);

                // Fetch all matching for in-memory grouping
                var allLicenses = await baseQuery
                    .Include(x => x.Province)
                    .Include(x => x.CurrentDistrict)
                    .ToListAsync();

                // Date-filtered licenses (by IssueDate)
                var dateFiltered = allLicenses.AsEnumerable();
                if (parsedStart.HasValue)
                    dateFiltered = dateFiltered.Where(x => x.LicenseIssueDate.HasValue && x.LicenseIssueDate >= parsedStart);
                if (parsedEnd.HasValue)
                    dateFiltered = dateFiltered.Where(x => x.LicenseIssueDate.HasValue && x.LicenseIssueDate <= parsedEnd);

                var filteredList = dateFiltered.ToList();

                // Helper: map LicenseType code to display name
                static string MapLicenseType(string? t) => t switch
                {
                    "new" => "جدید",
                    "renewal" => "تجدید",
                    "duplicate" => "مثنی",
                    null or "" => "نامشخص",
                    _ => t
                };

                // --- Filtered stats ---
                var byLicenseType = filteredList
                    .GroupBy(x => MapLicenseType(x.LicenseType))
                    .Select(g => new {
                        licenseType = g.Key,
                        count = g.Count(),
                        totalCost = g.Sum(x => x.LicenseCost ?? 0),
                        totalPrice = g.Sum(x => x.LicensePrice ?? 0)
                    })
                    .OrderByDescending(x => x.count).ToList();

                var totalFilteredCost = filteredList.Sum(x => x.LicenseCost ?? 0);

                var activeCount   = filteredList.Count(x => x.LicenseStatus == 1);
                var cancelledCount = filteredList.Count(x => x.LicenseStatus == 2);
                var withdrawnCount = filteredList.Count(x => x.LicenseStatus == 3);

                // Relocations in date range (scoped to filtered license IDs)
                var filteredIds = filteredList.Select(x => x.Id).ToHashSet();
                var relocQuery = _context.PetitionWriterRelocations.AsNoTracking()
                    .Where(x => filteredIds.Contains(x.PetitionWriterLicenseId));
                if (parsedStart.HasValue)
                    relocQuery = relocQuery.Where(x => x.RelocationDate.HasValue && x.RelocationDate >= parsedStart);
                if (parsedEnd.HasValue)
                    relocQuery = relocQuery.Where(x => x.RelocationDate.HasValue && x.RelocationDate <= parsedEnd);
                var relocationCount = await relocQuery.CountAsync();

                var byActivityLocation = filteredList
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.ActivityLocation) ? "نامشخص" : x.ActivityLocation)
                    .Select(g => new { activityLocation = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count).ToList();

                var byProvince = filteredList
                    .GroupBy(x => x.Province?.Dari ?? "نامشخص")
                    .Select(g => new { province = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count).ToList();

                var byDistrict = filteredList
                    .GroupBy(x => x.CurrentDistrict?.Dari ?? "نامشخص")
                    .Select(g => new { district = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count).ToList();

                // --- Overall (all-time, same province/district/location filters) ---
                var overallAll = allLicenses; // already filtered by province/district/location
                var overallTotal     = overallAll.Count;
                var overallActive    = overallAll.Count(x => x.LicenseStatus == 1);
                var overallCancelled = overallAll.Count(x => x.LicenseStatus == 2);
                var overallWithdrawn = overallAll.Count(x => x.LicenseStatus == 3);

                var overallIds = overallAll.Select(x => x.Id).ToHashSet();
                var overallRelocations = await _context.PetitionWriterRelocations.AsNoTracking()
                    .Where(x => overallIds.Contains(x.PetitionWriterLicenseId))
                    .CountAsync();

                var overallByType = overallAll
                    .GroupBy(x => MapLicenseType(x.LicenseType))
                    .Select(g => new {
                        licenseType = g.Key,
                        count = g.Count(),
                        totalCost = g.Sum(x => x.LicenseCost ?? 0),
                        totalPrice = g.Sum(x => x.LicensePrice ?? 0)
                    })
                    .OrderByDescending(x => x.count).ToList();

                var totalOverallCost = overallAll.Sum(x => x.LicenseCost ?? 0);

                var overallByLocation = overallAll
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.ActivityLocation) ? "نامشخص" : x.ActivityLocation)
                    .Select(g => new { activityLocation = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count).ToList();

                var overallByProvince = overallAll
                    .GroupBy(x => x.Province?.Dari ?? "نامشخص")
                    .Select(g => new { province = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count).ToList();

                var overallByDistrict = overallAll
                    .GroupBy(x => x.CurrentDistrict?.Dari ?? "نامشخص")
                    .Select(g => new { district = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count).ToList();

                // --- Dropdown data (always from full unfiltered set) ---
                var allForDropdowns = await _context.PetitionWriterLicenses
                    .AsNoTracking()
                    .Where(x => x.Status == true)
                    .Include(x => x.Province)
                    .Include(x => x.CurrentDistrict)
                    .ToListAsync();

                var activityLocations = allForDropdowns
                    .Where(x => !string.IsNullOrWhiteSpace(x.ActivityLocation))
                    .Select(x => x.ActivityLocation!).Distinct().OrderBy(x => x).ToList();

                var provinces = await _cache.GetProvincesAsync();
                var provinceList = provinces.Select(p => new { p.Id, name = p.Dari }).ToList();

                // Districts: if a province is selected return its districts, else return all districts
                // that appear in the data
                List<object> districtList;
                if (provinceId.HasValue && provinceId.Value > 0)
                {
                    var districts = await _cache.GetDistrictsAsync(provinceId.Value);
                    districtList = districts.Select(d => (object)new { d.Id, name = d.Dari }).ToList();
                }
                else
                {
                    districtList = new List<object>();
                }

                return Ok(new
                {
                    startDate        = parsedStart.HasValue ? DateConversionHelper.FormatDateOnly(parsedStart, calendar) : null,
                    endDate          = parsedEnd.HasValue   ? DateConversionHelper.FormatDateOnly(parsedEnd,   calendar) : null,
                    reportGeneratedAt = DateConversionHelper.FormatDateOnly(DateOnly.FromDateTime(DateTime.Today), calendar),

                    filtered = new
                    {
                        totalLicenses  = filteredList.Count,
                        activeCount,
                        cancelledCount,
                        withdrawnCount,
                        relocationCount,
                        totalCost = totalFilteredCost,
                        byLicenseType,
                        byActivityLocation,
                        byProvince,
                        byDistrict
                    },

                    overall = new
                    {
                        totalLicenses  = overallTotal,
                        activeCount    = overallActive,
                        cancelledCount = overallCancelled,
                        withdrawnCount = overallWithdrawn,
                        relocationCount = overallRelocations,
                        totalCost = totalOverallCost,
                        byLicenseType  = overallByType,
                        byActivityLocation = overallByLocation,
                        byProvince     = overallByProvince,
                        byDistrict     = overallByDistrict
                    },

                    // Dropdown data
                    activityLocations,
                    provinces = provinceList,
                    districts = districtList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در تولید گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// Get districts for a province (used by report province dropdown)
        /// </summary>
        [HttpGet("report/districts/{provinceId}")]
        public async Task<IActionResult> GetReportDistricts(int provinceId)
        {
            try
            {
                var districts = await _cache.GetDistrictsAsync(provinceId);
                return Ok(districts.Select(d => new { d.Id, name = d.Dari }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری ولسوالی‌ها", error = ex.Message });
            }
        }

        #endregion
    }
}
