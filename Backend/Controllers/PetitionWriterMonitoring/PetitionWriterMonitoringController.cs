using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.PetitionWriterMonitoring;
using WebAPIBackend.Models.RequestData.PetitionWriterMonitoring;

namespace WebAPIBackend.Controllers.PetitionWriterMonitoring
{
    /// <summary>
    /// Controller for Petition Writer Monitoring (ثبت نظارت بر فعالیت عریضه نویسان)
    /// Single Table Design - All sections in one entity
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetitionWriterMonitoringController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetitionWriterMonitoringController(AppDbContext context)
        {
            _context = context;
        }

        #region Main Record CRUD

        /// <summary>
        /// Get all petition writer monitoring records with pagination and search
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
                var query = _context.PetitionWriterMonitoringRecords
                    .Where(x => x.Status == true)
                    .AsQueryable();

                // Filter by section type
                if (!string.IsNullOrEmpty(sectionType))
                {
                    query = query.Where(x => x.SectionType == sectionType);
                }

                // Search functionality
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(x =>
                        x.SerialNumber!.Contains(search) ||
                        x.ComplainantName!.Contains(search) ||
                        x.ComplaintSubject!.Contains(search) ||
                        x.PetitionWriterName!.Contains(search) ||
                        x.PetitionWriterLicenseNumber!.Contains(search) ||
                        x.PetitionWriterDistrict!.Contains(search));
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
                        x.SectionType,
                        x.RegistrationDate,
                        RegistrationDateFormatted = x.RegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.RegistrationDate, calendar)
                            : "",
                        
                        // Complaints
                        x.ComplainantName,
                        x.ComplaintSubject,
                        x.ComplaintActionsTaken,
                        x.ComplaintRemarks,
                        
                        // Violations
                        x.PetitionWriterName,
                        x.PetitionWriterLicenseNumber,
                        x.PetitionWriterDistrict,
                        x.ViolationType,
                        x.ViolationActionsTaken,
                        x.ViolationRemarks,
                        
                        // Monitoring
                        x.MonitoringYear,
                        x.MonitoringMonth,
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
        /// Get petition writer monitoring record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var item = await _context.PetitionWriterMonitoringRecords
                    .Where(x => x.Id == id && x.Status == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.SerialNumber,
                        x.SectionType,
                        x.RegistrationDate,
                        RegistrationDateFormatted = x.RegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.RegistrationDate, calendar)
                            : "",
                        
                        // Complaints
                        x.ComplainantName,
                        x.ComplaintSubject,
                        x.ComplaintActionsTaken,
                        x.ComplaintRemarks,
                        
                        // Violations
                        x.PetitionWriterName,
                        x.PetitionWriterLicenseNumber,
                        x.PetitionWriterDistrict,
                        x.ViolationType,
                        x.ViolationActionsTaken,
                        x.ViolationRemarks,
                        
                        // Monitoring
                        x.MonitoringYear,
                        x.MonitoringMonth,
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
        /// Create new petition writer monitoring record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PetitionWriterMonitoringData request)
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
                DateConversionHelper.TryParseToDateOnly(request.RegistrationDate, request.CalendarType, out var registrationDate);

                var entity = new PetitionWriterMonitoringRecord
                {
                    SerialNumber = request.SerialNumber,
                    SectionType = request.SectionType,
                    RegistrationDate = registrationDate,
                    
                    // Complaints
                    ComplainantName = request.ComplainantName,
                    ComplaintSubject = request.ComplaintSubject,
                    ComplaintActionsTaken = request.ComplaintActionsTaken,
                    ComplaintRemarks = request.ComplaintRemarks,
                    
                    // Violations
                    PetitionWriterName = request.PetitionWriterName,
                    PetitionWriterLicenseNumber = request.PetitionWriterLicenseNumber,
                    PetitionWriterDistrict = request.PetitionWriterDistrict,
                    ViolationType = request.ViolationType,
                    ViolationActionsTaken = request.ViolationActionsTaken,
                    ViolationRemarks = request.ViolationRemarks,
                    
                    // Monitoring
                    MonitoringYear = request.MonitoringYear,
                    MonitoringMonth = request.MonitoringMonth,
                    MonitoringCount = request.MonitoringCount,
                    MonitoringRemarks = request.MonitoringRemarks,
                    
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.PetitionWriterMonitoringRecords.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "معلومات موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update petition writer monitoring record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PetitionWriterMonitoringData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                var entity = await _context.PetitionWriterMonitoringRecords.FindAsync(id);
                if (entity == null || entity.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.RegistrationDate, request.CalendarType, out var registrationDate);

                // Update all fields
                entity.SectionType = request.SectionType;
                entity.RegistrationDate = registrationDate;
                
                // Complaints
                entity.ComplainantName = request.ComplainantName;
                entity.ComplaintSubject = request.ComplaintSubject;
                entity.ComplaintActionsTaken = request.ComplaintActionsTaken;
                entity.ComplaintRemarks = request.ComplaintRemarks;
                
                // Violations
                entity.PetitionWriterName = request.PetitionWriterName;
                entity.PetitionWriterLicenseNumber = request.PetitionWriterLicenseNumber;
                entity.PetitionWriterDistrict = request.PetitionWriterDistrict;
                entity.ViolationType = request.ViolationType;
                entity.ViolationActionsTaken = request.ViolationActionsTaken;
                entity.ViolationRemarks = request.ViolationRemarks;
                
                // Monitoring
                entity.MonitoringYear = request.MonitoringYear;
                entity.MonitoringMonth = request.MonitoringMonth;
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
        /// Delete (soft delete) petition writer monitoring record
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.PetitionWriterMonitoringRecords.FindAsync(id);
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
        public async Task<IActionResult> GetNextSerialNumber()
        {
            try
            {
                var lastRecord = await _context.PetitionWriterMonitoringRecords
                    .Where(x => x.Status == true)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;
                if (lastRecord != null && int.TryParse(lastRecord.SerialNumber, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
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
