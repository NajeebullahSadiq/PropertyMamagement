using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.LicenseApplication;
using WebAPIBackend.Models.RequestData.LicenseApplication;

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

        // Guarantee Type Constants
        private const int GuaranteeType_Cash = 1;
        private const int GuaranteeType_ShariaDeed = 2;
        private const int GuaranteeType_CustomaryDeed = 3;

        public LicenseApplicationController(AppDbContext context)
        {
            _context = context;
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
                    .Where(x => x.Status == true)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.RequestSerialNumber.Contains(search) ||
                        x.ApplicantName.Contains(search) ||
                        x.ProposedGuideName.Contains(search));
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
                        x.RequestDate,
                        RequestDateFormatted = x.RequestDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.RequestDate, calendar)
                            : "",
                        x.RequestSerialNumber,
                        x.ApplicantName,
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
                    .Include(x => x.PermanentProvince)
                    .Include(x => x.PermanentDistrict)
                    .Include(x => x.CurrentProvince)
                    .Include(x => x.CurrentDistrict)
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

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.RequestDate, request.CalendarType, out var requestDate);

                var entity = new LicenseApplicationEntity
                {
                    RequestDate = requestDate,
                    RequestSerialNumber = request.RequestSerialNumber,
                    ApplicantName = request.ApplicantName,
                    ProposedGuideName = request.ProposedGuideName,
                    PermanentProvinceId = request.PermanentProvinceId,
                    PermanentDistrictId = request.PermanentDistrictId,
                    PermanentVillage = request.PermanentVillage,
                    CurrentProvinceId = request.CurrentProvinceId,
                    CurrentDistrictId = request.CurrentDistrictId,
                    CurrentVillage = request.CurrentVillage,
                    Status = true,
                    IsWithdrawn = false,
                    CreatedAt = DateTime.Now,
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

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.RequestDate, request.CalendarType, out var requestDate);

                entity.RequestDate = requestDate;
                entity.RequestSerialNumber = request.RequestSerialNumber;
                entity.ApplicantName = request.ApplicantName;
                entity.ProposedGuideName = request.ProposedGuideName;
                entity.PermanentProvinceId = request.PermanentProvinceId;
                entity.PermanentDistrictId = request.PermanentDistrictId;
                entity.PermanentVillage = request.PermanentVillage;
                entity.CurrentProvinceId = request.CurrentProvinceId;
                entity.CurrentDistrictId = request.CurrentDistrictId;
                entity.CurrentVillage = request.CurrentVillage;
                entity.UpdatedAt = DateTime.Now;
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
                    .Where(x => x.LicenseApplicationId == applicationId)
                    .Include(x => x.PermanentProvince)
                    .Include(x => x.PermanentDistrict)
                    .Include(x => x.CurrentProvince)
                    .Include(x => x.CurrentDistrict)
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
                    })
                    .ToListAsync();

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
                    PermanentProvinceId = request.PermanentProvinceId,
                    PermanentDistrictId = request.PermanentDistrictId,
                    PermanentVillage = request.PermanentVillage,
                    CurrentProvinceId = request.CurrentProvinceId,
                    CurrentDistrictId = request.CurrentDistrictId,
                    CurrentVillage = request.CurrentVillage,
                    CreatedAt = DateTime.Now,
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

                var withdrawal = await _context.LicenseApplicationWithdrawals
                    .Where(x => x.LicenseApplicationId == applicationId)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseApplicationId,
                        x.WithdrawalReason,
                        x.WithdrawalDate,
                        WithdrawalDateFormatted = x.WithdrawalDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.WithdrawalDate, calendar)
                            : "",
                        x.CreatedAt
                    })
                    .FirstOrDefaultAsync();

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
                        CreatedAt = DateTime.Now,
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
    }
}
