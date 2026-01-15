using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.PetitionWriterLicense;
using WebAPIBackend.Models.RequestData.PetitionWriterLicense;

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

        public PetitionWriterLicenseController(AppDbContext context)
        {
            _context = context;
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
                    .Where(x => x.Status == true)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.LicenseNumber.Contains(search) ||
                        x.ApplicantName.Contains(search) ||
                        (x.ActivityLocation != null && x.ActivityLocation.Contains(search)));
                }

                var totalCount = await query.CountAsync();
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.PermanentProvince)
                    .Include(x => x.PermanentDistrict)
                    .Include(x => x.CurrentProvince)
                    .Include(x => x.CurrentDistrict)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseNumber,
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandFatherName,
                        x.IdentityCardType,
                        x.ElectronicIdNumber,
                        x.PaperIdNumber,
                        x.PaperIdVolume,
                        x.PaperIdPage,
                        x.PaperIdRegNumber,
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
                        x.BankReceiptNumber,
                        x.BankReceiptDate,
                        BankReceiptDateFormatted = x.BankReceiptDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.BankReceiptDate, calendar)
                            : "",
                        x.LicenseType,
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

                var item = await _context.PetitionWriterLicenses
                    .Where(x => x.Id == id && x.Status == true)
                    .Include(x => x.PermanentProvince)
                    .Include(x => x.PermanentDistrict)
                    .Include(x => x.CurrentProvince)
                    .Include(x => x.CurrentDistrict)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseNumber,
                        x.ApplicantName,
                        x.ApplicantFatherName,
                        x.ApplicantGrandFatherName,
                        x.IdentityCardType,
                        x.ElectronicIdNumber,
                        x.PaperIdNumber,
                        x.PaperIdVolume,
                        x.PaperIdPage,
                        x.PaperIdRegNumber,
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
                        x.BankReceiptNumber,
                        x.BankReceiptDate,
                        BankReceiptDateFormatted = x.BankReceiptDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.BankReceiptDate, calendar)
                            : "",
                        x.LicenseType,
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
                        x.CreatedBy,
                        x.UpdatedAt,
                        x.UpdatedBy
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
        public async Task<IActionResult> Create([FromBody] PetitionWriterLicenseData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for duplicate license number
                var exists = await _context.PetitionWriterLicenses
                    .AnyAsync(x => x.LicenseNumber == data.LicenseNumber && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر جواز قبلاً ثبت شده است" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
                var username = User.Identity?.Name ?? "system";

                var entity = new PetitionWriterLicenseEntity
                {
                    LicenseNumber = data.LicenseNumber,
                    ApplicantName = data.ApplicantName,
                    ApplicantFatherName = data.ApplicantFatherName,
                    ApplicantGrandFatherName = data.ApplicantGrandFatherName,
                    IdentityCardType = data.IdentityCardType,
                    ElectronicIdNumber = data.ElectronicIdNumber,
                    PaperIdNumber = data.PaperIdNumber,
                    PaperIdVolume = data.PaperIdVolume,
                    PaperIdPage = data.PaperIdPage,
                    PaperIdRegNumber = data.PaperIdRegNumber,
                    PermanentProvinceId = data.PermanentProvinceId,
                    PermanentDistrictId = data.PermanentDistrictId,
                    PermanentVillage = data.PermanentVillage,
                    CurrentProvinceId = data.CurrentProvinceId,
                    CurrentDistrictId = data.CurrentDistrictId,
                    CurrentVillage = data.CurrentVillage,
                    ActivityLocation = data.ActivityLocation,
                    BankReceiptNumber = data.BankReceiptNumber,
                    BankReceiptDate = DateConversionHelper.ParseDateOnly(data.BankReceiptDate, calendar),
                    LicenseType = data.LicenseType,
                    LicenseIssueDate = DateConversionHelper.ParseDateOnly(data.LicenseIssueDate, calendar),
                    LicenseExpiryDate = DateConversionHelper.ParseDateOnly(data.LicenseExpiryDate, calendar),
                    LicenseStatus = data.LicenseStatus ?? 1,
                    CancellationDate = DateConversionHelper.ParseDateOnly(data.CancellationDate, calendar),
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = username
                };

                _context.PetitionWriterLicenses.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "جواز با موفقیت ثبت شد" });
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

                // Check for duplicate license number (excluding current)
                var exists = await _context.PetitionWriterLicenses
                    .AnyAsync(x => x.LicenseNumber == data.LicenseNumber && x.Id != id && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر جواز قبلاً ثبت شده است" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
                var username = User.Identity?.Name ?? "system";

                entity.LicenseNumber = data.LicenseNumber;
                entity.ApplicantName = data.ApplicantName;
                entity.ApplicantFatherName = data.ApplicantFatherName;
                entity.ApplicantGrandFatherName = data.ApplicantGrandFatherName;
                entity.IdentityCardType = data.IdentityCardType;
                entity.ElectronicIdNumber = data.ElectronicIdNumber;
                entity.PaperIdNumber = data.PaperIdNumber;
                entity.PaperIdVolume = data.PaperIdVolume;
                entity.PaperIdPage = data.PaperIdPage;
                entity.PaperIdRegNumber = data.PaperIdRegNumber;
                entity.PermanentProvinceId = data.PermanentProvinceId;
                entity.PermanentDistrictId = data.PermanentDistrictId;
                entity.PermanentVillage = data.PermanentVillage;
                entity.CurrentProvinceId = data.CurrentProvinceId;
                entity.CurrentDistrictId = data.CurrentDistrictId;
                entity.CurrentVillage = data.CurrentVillage;
                entity.ActivityLocation = data.ActivityLocation;
                entity.BankReceiptNumber = data.BankReceiptNumber;
                entity.BankReceiptDate = DateConversionHelper.ParseDateOnly(data.BankReceiptDate, calendar);
                entity.LicenseType = data.LicenseType;
                entity.LicenseIssueDate = DateConversionHelper.ParseDateOnly(data.LicenseIssueDate, calendar);
                entity.LicenseExpiryDate = DateConversionHelper.ParseDateOnly(data.LicenseExpiryDate, calendar);
                entity.LicenseStatus = data.LicenseStatus ?? entity.LicenseStatus;
                entity.CancellationDate = DateConversionHelper.ParseDateOnly(data.CancellationDate, calendar);
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = username;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "جواز با موفقیت تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در تغییر جواز", error = ex.Message });
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
                    .AnyAsync(x => x.Id == licenseId && x.Status == true);

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
    }
}
