using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.ActivityMonitoring;
using WebAPIBackend.Models.RequestData.ActivityMonitoring;

namespace WebAPIBackend.Controllers.ActivityMonitoring
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityMonitoringController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityMonitoringController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Resolves a display name from a user ID or returns the value as-is if it's already a name.
        /// </summary>
        private async Task<string> ResolveDisplayName(string? userIdOrName)
        {
            if (string.IsNullOrEmpty(userIdOrName)) return "-";
            // If it looks like a GUID, look up the actual user
            if (Guid.TryParse(userIdOrName, out _))
            {
                var user = await _userManager.FindByIdAsync(userIdOrName);
                if (user != null)
                {
                    var fullName = $"{user.FirstName} {user.LastName}".Trim();
                    return string.IsNullOrEmpty(fullName) ? (user.UserName ?? userIdOrName) : fullName;
                }
            }
            return userIdOrName;
        }

        /// <summary>
        /// Gets the display name of the currently authenticated user.
        /// </summary>
        private async Task<string> GetCurrentUserDisplayName()
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null) return User.Identity?.Name ?? "Unknown";
            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null) return User.Identity?.Name ?? userIdClaim.Value;
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrEmpty(fullName) ? (user.UserName ?? userIdClaim.Value) : fullName;
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

                var rawItems = await query
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

                // Resolve any GUID-based CreatedBy values to display names
                // Done sequentially to avoid concurrent DbContext access
                var userNameCache = new Dictionary<string, string>();
                var resolvedItems = new List<object>();
                foreach (var x in rawItems)
                {
                    string resolvedName;
                    if (string.IsNullOrEmpty(x.CreatedBy))
                    {
                        resolvedName = "-";
                    }
                    else if (userNameCache.TryGetValue(x.CreatedBy, out var cached))
                    {
                        resolvedName = cached;
                    }
                    else
                    {
                        resolvedName = await ResolveDisplayName(x.CreatedBy);
                        userNameCache[x.CreatedBy] = resolvedName;
                    }

                    resolvedItems.Add(new
                    {
                        x.Id,
                        x.SerialNumber,
                        x.LicenseNumber,
                        x.LicenseHolderName,
                        x.CompanyTitle,
                        x.District,
                        x.SectionType,
                        x.ReportRegistrationDate,
                        x.ReportRegistrationDateFormatted,
                        x.SaleDeedsCount,
                        x.RentalDeedsCount,
                        x.BaiUlWafaDeedsCount,
                        x.VehicleTransactionDeedsCount,
                        x.DeedItems,
                        x.ComplaintSubject,
                        x.ComplainantName,
                        x.ComplaintActionsTaken,
                        x.ComplaintRemarks,
                        x.ViolationStatus,
                        x.ViolationType,
                        x.ClosureReason,
                        x.ViolationActionsTaken,
                        x.ViolationRemarks,
                        x.Year,
                        x.Month,
                        x.MonitoringCount,
                        x.MonitoringRemarks,
                        x.Status,
                        x.CreatedAt,
                        CreatedBy = resolvedName
                    });
                }

                var items = resolvedItems;

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

                return Ok(new
                {
                    item.Id,
                    item.SerialNumber,
                    item.LicenseNumber,
                    item.LicenseHolderName,
                    item.CompanyTitle,
                    item.District,
                    item.SectionType,
                    item.ReportRegistrationDate,
                    item.ReportRegistrationDateFormatted,
                    item.SaleDeedsCount,
                    item.RentalDeedsCount,
                    item.BaiUlWafaDeedsCount,
                    item.VehicleTransactionDeedsCount,
                    item.DeedItems,
                    item.ComplaintSubject,
                    item.ComplainantName,
                    item.ComplaintActionsTaken,
                    item.ComplaintRemarks,
                    item.ViolationStatus,
                    item.ViolationType,
                    item.ClosureReason,
                    item.ViolationActionsTaken,
                    item.ViolationRemarks,
                    item.Year,
                    item.Month,
                    item.MonitoringCount,
                    item.MonitoringRemarks,
                    item.Status,
                    item.CreatedAt,
                    CreatedBy = await ResolveDisplayName(item.CreatedBy)
                });
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

                var userName = await GetCurrentUserDisplayName();

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.ReportRegistrationDate, request.CalendarType, out var reportRegistrationDate);

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
                    CreatedBy = userName
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

                var userName = await GetCurrentUserDisplayName();

                var entity = await _context.ActivityMonitoringRecords.FindAsync(id);
                if (entity == null || entity.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.ReportRegistrationDate, request.CalendarType, out var reportRegistrationDate);

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
                entity.ClosureReason = request.ClosureReason;
                entity.ViolationActionsTaken = request.ViolationActionsTaken;
                entity.ViolationRemarks = request.ViolationRemarks;
                
                // Inspections
                entity.Year = request.Year;
                entity.Month = request.Month;
                entity.MonitoringCount = request.MonitoringCount;
                entity.MonitoringRemarks = request.MonitoringRemarks;
                
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userName;

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
