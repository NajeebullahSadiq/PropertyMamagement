using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.ActivityMonitoring;
using WebAPIBackend.Models.RequestData.ActivityMonitoring;

namespace WebAPIBackend.Controllers.ActivityMonitoring
{
    /// <summary>
    /// Controller for Activity Monitoring (نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان)
    /// Single Table Design - All sections in one entity
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityMonitoringController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityMonitoringController(AppDbContext context)
        {
            _context = context;
        }

        #region Main Record CRUD

        /// <summary>
        /// Get all activity monitoring records with pagination and search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sectionType = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var query = _context.ActivityMonitoringRecords
                    .AsNoTracking()
                    .Where(x => x.Status == true)
                    .AsQueryable();

                // Search functionality
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(x =>
                        x.SerialNumber!.Contains(search) ||
                        x.LicenseNumber!.Contains(search) ||
                        x.LicenseHolderName!.Contains(search) ||
                        x.District!.Contains(search));
                }

                // Section type filter
                if (!string.IsNullOrEmpty(sectionType))
                {
                    query = query.Where(x => x.SectionType == sectionType);
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
                        x.SerialNumber,
                        x.LicenseNumber,
                        x.LicenseHolderName,
                        x.CompanyTitle,
                        x.District,
                        x.SectionType,
                        x.ReportRegistrationDate,
                        ReportRegistrationDateFormatted = x.ReportRegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
                            : "",
                        x.SaleDeedsCount,
                        x.RentalDeedsCount,
                        x.BaiUlWafaDeedsCount,
                        x.VehicleTransactionDeedsCount,
                        x.DeedItems,
                        
                        // Complaints
                        x.ComplaintSubject,
                        x.ComplainantName,
                        x.ComplaintActionsTaken,
                        x.ComplaintRemarks,
                        
                        // Violations
                        x.ViolationStatus,
                        x.ViolationType,
                        x.ViolationDate,
                        ViolationDateFormatted = x.ViolationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ViolationDate, calendar)
                            : "",
                        x.ClosureDate,
                        ClosureDateFormatted = x.ClosureDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ClosureDate, calendar)
                            : "",
                        x.ClosureReason,
                        x.ViolationActionsTaken,
                        x.ViolationRemarks,
                        
                        // Inspections
                        x.Year,
                        x.Month,
                        x.MonitoringCount,
                        x.MonitoringRemarks,
                        
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
        /// Get activity monitoring record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var item = await _context.ActivityMonitoringRecords
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.Status == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.SerialNumber,
                        x.LicenseNumber,
                        x.LicenseHolderName,
                        x.CompanyTitle,
                        x.District,
                        x.SectionType,
                        x.ReportRegistrationDate,
                        ReportRegistrationDateFormatted = x.ReportRegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
                            : "",
                        x.SaleDeedsCount,
                        x.RentalDeedsCount,
                        x.BaiUlWafaDeedsCount,
                        x.VehicleTransactionDeedsCount,
                        x.DeedItems,
                        
                        // Complaints
                        x.ComplaintSubject,
                        x.ComplainantName,
                        x.ComplaintActionsTaken,
                        x.ComplaintRemarks,
                        
                        // Violations
                        x.ViolationStatus,
                        x.ViolationType,
                        x.ViolationDate,
                        ViolationDateFormatted = x.ViolationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ViolationDate, calendar)
                            : "",
                        x.ClosureDate,
                        ClosureDateFormatted = x.ClosureDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ClosureDate, calendar)
                            : "",
                        x.ClosureReason,
                        x.ViolationActionsTaken,
                        x.ViolationRemarks,
                        
                        // Inspections
                        x.Year,
                        x.Month,
                        x.MonitoringCount,
                        x.MonitoringRemarks,
                        
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound("رکورد یافت نشد");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new activity monitoring record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ActivityMonitoringData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.ReportRegistrationDate, request.CalendarType, out var reportRegistrationDate);
                DateConversionHelper.TryParseToDateOnly(request.ViolationDate, request.CalendarType, out var violationDate);
                DateConversionHelper.TryParseToDateOnly(request.ClosureDate, request.CalendarType, out var closureDate);

                // Serialize deed items to JSON
                string? deedItemsJson = null;
                if (request.DeedItems != null && request.DeedItems.Any())
                {
                    deedItemsJson = JsonSerializer.Serialize(request.DeedItems);
                }

                var entity = new ActivityMonitoringRecord
                {
                    SerialNumber = request.SerialNumber,
                    LicenseNumber = request.LicenseNumber,
                    LicenseHolderName = request.LicenseHolderName,
                    CompanyTitle = request.CompanyTitle,
                    District = request.District,
                    SectionType = request.SectionType,
                    ReportRegistrationDate = reportRegistrationDate,
                    SaleDeedsCount = request.SaleDeedsCount,
                    RentalDeedsCount = request.RentalDeedsCount,
                    BaiUlWafaDeedsCount = request.BaiUlWafaDeedsCount,
                    VehicleTransactionDeedsCount = request.VehicleTransactionDeedsCount,
                    DeedItems = deedItemsJson,
                    
                    // Complaints
                    ComplaintSubject = request.ComplaintSubject,
                    ComplainantName = request.ComplainantName,
                    ComplaintActionsTaken = request.ComplaintActionsTaken,
                    ComplaintRemarks = request.ComplaintRemarks,
                    
                    // Violations
                    ViolationStatus = request.ViolationStatus,
                    ViolationType = request.ViolationType,
                    ViolationDate = violationDate,
                    ClosureDate = closureDate,
                    ClosureReason = request.ClosureReason,
                    ViolationActionsTaken = request.ViolationActionsTaken,
                    ViolationRemarks = request.ViolationRemarks,
                    
                    // Inspections
                    Year = request.Year,
                    Month = request.Month,
                    MonitoringCount = request.MonitoringCount,
                    MonitoringRemarks = request.MonitoringRemarks,
                    
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.ActivityMonitoringRecords.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "معلومات موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update activity monitoring record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActivityMonitoringData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                var entity = await _context.ActivityMonitoringRecords.FindAsync(id);
                if (entity == null || entity.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.ReportRegistrationDate, request.CalendarType, out var reportRegistrationDate);
                DateConversionHelper.TryParseToDateOnly(request.ViolationDate, request.CalendarType, out var violationDate);
                DateConversionHelper.TryParseToDateOnly(request.ClosureDate, request.CalendarType, out var closureDate);

                // Serialize deed items to JSON
                string? deedItemsJson = null;
                if (request.DeedItems != null && request.DeedItems.Any())
                {
                    deedItemsJson = JsonSerializer.Serialize(request.DeedItems);
                }

                // Update all fields
                entity.LicenseNumber = request.LicenseNumber;
                entity.LicenseHolderName = request.LicenseHolderName;
                entity.CompanyTitle = request.CompanyTitle;
                entity.District = request.District;
                entity.SectionType = request.SectionType;
                entity.ReportRegistrationDate = reportRegistrationDate;
                entity.SaleDeedsCount = request.SaleDeedsCount;
                entity.RentalDeedsCount = request.RentalDeedsCount;
                entity.BaiUlWafaDeedsCount = request.BaiUlWafaDeedsCount;
                entity.VehicleTransactionDeedsCount = request.VehicleTransactionDeedsCount;
                entity.DeedItems = deedItemsJson;
                
                // Complaints
                entity.ComplaintSubject = request.ComplaintSubject;
                entity.ComplainantName = request.ComplainantName;
                entity.ComplaintActionsTaken = request.ComplaintActionsTaken;
                entity.ComplaintRemarks = request.ComplaintRemarks;
                
                // Violations
                entity.ViolationStatus = request.ViolationStatus;
                entity.ViolationType = request.ViolationType;
                entity.ViolationDate = violationDate;
                entity.ClosureDate = closureDate;
                entity.ClosureReason = request.ClosureReason;
                entity.ViolationActionsTaken = request.ViolationActionsTaken;
                entity.ViolationRemarks = request.ViolationRemarks;
                
                // Inspections
                entity.Year = request.Year;
                entity.Month = request.Month;
                entity.MonitoringCount = request.MonitoringCount;
                entity.MonitoringRemarks = request.MonitoringRemarks;
                
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "معلومات موفقانه تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete (soft delete) activity monitoring record
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.ActivityMonitoringRecords.FindAsync(id);
                if (entity == null)
                {
                    return NotFound("رکورد یافت نشد");
                }

                entity.Status = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "رکورد موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Utility

        /// <summary>
        /// Get next serial number for new record
        /// </summary>
        [HttpGet("next-serial-number")]
        public async Task<IActionResult> GetNextSerialNumber([FromQuery] string? sectionType = null)
        {
            try
            {
                var query = _context.ActivityMonitoringRecords
                    .AsNoTracking()
                    .Where(x => x.Status == true);

                // Filter by section type if provided
                if (!string.IsNullOrEmpty(sectionType))
                {
                    query = query.Where(x => x.SectionType == sectionType);
                }

                // Get the max serial number for this section type
                var serialNumbers = await query
                    .Where(x => x.SerialNumber != null)
                    .Select(x => x.SerialNumber!)
                    .ToListAsync();

                int nextNumber = 1;
                var maxSerial = serialNumbers
                    .Select(s => int.TryParse(s, out int n) ? n : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                if (maxSerial > 0)
                {
                    nextNumber = maxSerial + 1;
                }

                return Ok(new { serialNumber = nextNumber.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
