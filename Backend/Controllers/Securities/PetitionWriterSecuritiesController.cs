using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers.Securities
{
    /// <summary>
    /// Controller for Petition Writer Securities (سند بهادار عریضه‌ نویسان)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetitionWriterSecuritiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetitionWriterSecuritiesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all petition writer securities with pagination and search
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
                var query = _context.PetitionWriterSecurities
                    .Where(x => x.Status == true)
                    .AsQueryable();

                // Search by registration number, license number, name, or bank receipt
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.RegistrationNumber.Contains(search) ||
                        x.LicenseNumber.Contains(search) ||
                        x.PetitionWriterName.Contains(search) ||
                        x.BankReceiptNumber.Contains(search));
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
                        x.RegistrationNumber,
                        x.PetitionWriterName,
                        x.PetitionWriterFatherName,
                        x.LicenseNumber,
                        x.PetitionCount,
                        x.Amount,
                        x.BankReceiptNumber,
                        x.SerialNumberStart,
                        x.SerialNumberEnd,
                        x.DistributionDate,
                        DistributionDateFormatted = DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar),
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Get petition writer securities by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var item = await _context.PetitionWriterSecurities
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                return Ok(new
                {
                    item.Id,
                    item.RegistrationNumber,
                    item.PetitionWriterName,
                    item.PetitionWriterFatherName,
                    item.LicenseNumber,
                    item.PetitionCount,
                    item.Amount,
                    item.BankReceiptNumber,
                    item.SerialNumberStart,
                    item.SerialNumberEnd,
                    item.DistributionDate,
                    DistributionDateFormatted = DateConversionHelper.FormatDateOnly(item.DistributionDate, calendar),
                    item.CreatedAt,
                    item.CreatedBy,
                    item.UpdatedAt,
                    item.UpdatedBy,
                    item.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new petition writer securities
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR")]
        public async Task<IActionResult> Create([FromBody] PetitionWriterSecuritiesData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for unique registration number
                var exists = await _context.PetitionWriterSecurities
                    .AnyAsync(x => x.RegistrationNumber == data.RegistrationNumber && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر ثبت تعرفه قبلاً استفاده شده است" });
                }

                // Validate serial number logic
                if (!ValidateSerialNumbers(data.SerialNumberStart, data.SerialNumberEnd, out string? errorMessage))
                {
                    return BadRequest(new { message = errorMessage });
                }

                var userName = User.Identity?.Name ?? "System";

                var entity = new PetitionWriterSecurities
                {
                    RegistrationNumber = data.RegistrationNumber,
                    PetitionWriterName = data.PetitionWriterName,
                    PetitionWriterFatherName = data.PetitionWriterFatherName,
                    LicenseNumber = data.LicenseNumber,
                    PetitionCount = data.PetitionCount,
                    Amount = data.Amount,
                    BankReceiptNumber = data.BankReceiptNumber,
                    SerialNumberStart = data.SerialNumberStart,
                    SerialNumberEnd = data.SerialNumberEnd,
                    DistributionDate = data.DistributionDate!.Value,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    Status = true
                };

                _context.PetitionWriterSecurities.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Update petition writer securities
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR")]
        public async Task<IActionResult> Update(int id, [FromBody] PetitionWriterSecuritiesData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = await _context.PetitionWriterSecurities.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                // Check for unique registration number (excluding current record)
                var exists = await _context.PetitionWriterSecurities
                    .AnyAsync(x => x.RegistrationNumber == data.RegistrationNumber && x.Id != id && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر ثبت تعرفه قبلاً استفاده شده است" });
                }

                // Validate serial number logic
                if (!ValidateSerialNumbers(data.SerialNumberStart, data.SerialNumberEnd, out string? errorMessage))
                {
                    return BadRequest(new { message = errorMessage });
                }

                var userName = User.Identity?.Name ?? "System";

                entity.RegistrationNumber = data.RegistrationNumber;
                entity.PetitionWriterName = data.PetitionWriterName;
                entity.PetitionWriterFatherName = data.PetitionWriterFatherName;
                entity.LicenseNumber = data.LicenseNumber;
                entity.PetitionCount = data.PetitionCount;
                entity.Amount = data.Amount;
                entity.BankReceiptNumber = data.BankReceiptNumber;
                entity.SerialNumberStart = data.SerialNumberStart;
                entity.SerialNumberEnd = data.SerialNumberEnd;
                entity.DistributionDate = data.DistributionDate!.Value;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userName;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت بروزرسانی شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بروزرسانی اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete (soft delete) petition writer securities
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.PetitionWriterSecurities.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                entity.Status = false;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();

                return Ok(new { message = "رکورد با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate that serial start is less than or equal to serial end
        /// </summary>
        private bool ValidateSerialNumbers(string start, string end, out string? errorMessage)
        {
            errorMessage = null;

            // Try to parse as numbers for comparison
            if (long.TryParse(start, out long startNum) && long.TryParse(end, out long endNum))
            {
                if (startNum > endNum)
                {
                    errorMessage = "آغاز سریال نمبر باید کمتر یا مساوی ختم سریال نمبر باشد";
                    return false;
                }
            }
            else
            {
                // If not numeric, compare as strings
                if (string.Compare(start, end, StringComparison.Ordinal) > 0)
                {
                    errorMessage = "آغاز سریال نمبر باید کمتر یا مساوی ختم سریال نمبر باشد";
                    return false;
                }
            }

            return true;
        }
    }
}
