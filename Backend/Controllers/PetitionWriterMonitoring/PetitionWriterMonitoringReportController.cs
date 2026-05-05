using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.PetitionWriterMonitoring;

namespace WebAPIBackend.Controllers.PetitionWriterMonitoring
{
    /// <summary>
    /// Report Controller for Petition Writer Monitoring
    /// گزارش‌گیری نظارت بر فعالیت عریضه نویسان
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetitionWriterMonitoringReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetitionWriterMonitoringReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<string> ResolveDisplayName(string? userIdOrName)
        {
            if (string.IsNullOrEmpty(userIdOrName)) return "-";
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
        /// Get all users who have created petition writer monitoring records
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var createdByValues = await _context.PetitionWriterMonitoringRecords
                    .AsNoTracking()
                    .Where(x => x.Status == true && x.CreatedBy != null)
                    .Select(x => x.CreatedBy!)
                    .Distinct()
                    .ToListAsync();

                var users = new List<object>();
                foreach (var userId in createdByValues)
                {
                    var displayName = await ResolveDisplayName(userId);
                    users.Add(new { id = userId, name = displayName });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت لیست کاربران", error = ex.Message });
            }
        }

        /// <summary>
        /// Get comprehensive report for all sections
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? createdBy = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = BuildBaseQuery(startDate, endDate, createdBy, calendar);
                var records = await query.ToListAsync();

                var complaintsRecords = records.Where(x => x.SectionType == "complaints").ToList();
                var violationsRecords = records.Where(x => x.SectionType == "violations").ToList();
                var monitoringRecords = records.Where(x => x.SectionType == "monitoring").ToList();

                var result = new
                {
                    totalRecords = records.Count,
                    complaints = BuildComplaintsSummary(complaintsRecords),
                    violations = BuildViolationsSummary(violationsRecords),
                    monitoring = BuildMonitoringSummary(monitoringRecords)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// Get complaints section details
        /// </summary>
        [HttpGet("complaints")]
        public async Task<IActionResult> GetComplaints(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? createdBy = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = BuildBaseQuery(startDate, endDate, createdBy, calendar);
                var records = await query
                    .Where(x => x.SectionType == "complaints")
                    .ToListAsync();

                var summary = BuildComplaintsSummary(records);
                var details = records.Select(x => new
                {
                    x.Id,
                    x.SerialNumber,
                    petitionWriterLicenseNumber = x.PetitionWriterLicenseNumber ?? "-",
                    petitionWriterDistrict = x.PetitionWriterDistrict ?? "-",
                    petitionWriterName = x.PetitionWriterName ?? "-",
                    complainantName = x.ComplainantName ?? "-",
                    complaintSubject = x.ComplaintSubject ?? "-",
                    complaintActionsTaken = x.ComplaintActionsTaken ?? "-",
                    registrationDate = x.RegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.RegistrationDate, calendar)
                        : "-",
                    createdBy = x.CreatedBy ?? "-"
                }).ToList();

                return Ok(new { summary, details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت گزارش شکایات", error = ex.Message });
            }
        }

        /// <summary>
        /// Get violations section details
        /// </summary>
        [HttpGet("violations")]
        public async Task<IActionResult> GetViolations(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? createdBy = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = BuildBaseQuery(startDate, endDate, createdBy, calendar);
                var records = await query
                    .Where(x => x.SectionType == "violations")
                    .ToListAsync();

                var summary = BuildViolationsSummary(records);
                var details = records.Select(x => new
                {
                    x.Id,
                    x.SerialNumber,
                    petitionWriterName = x.PetitionWriterName ?? "-",
                    petitionWriterLicenseNumber = x.PetitionWriterLicenseNumber ?? "-",
                    petitionWriterDistrict = x.PetitionWriterDistrict ?? "-",
                    violationType = x.ViolationType ?? "-",
                    activityStatus = x.ActivityStatus ?? "-",
                    activityPermissionReason = x.ActivityPermissionReason ?? "-",
                    violationActionsTaken = x.ViolationActionsTaken ?? "-",
                    registrationDate = x.RegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.RegistrationDate, calendar)
                        : "-",
                    createdBy = x.CreatedBy ?? "-"
                }).ToList();

                return Ok(new { summary, details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت گزارش تخلفات", error = ex.Message });
            }
        }

        /// <summary>
        /// Get monitoring section details
        /// </summary>
        [HttpGet("monitoring")]
        public async Task<IActionResult> GetMonitoring(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? createdBy = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = BuildBaseQuery(startDate, endDate, createdBy, calendar);
                var records = await query
                    .Where(x => x.SectionType == "monitoring")
                    .ToListAsync();

                var summary = BuildMonitoringSummary(records);
                var details = records.Select(x => new
                {
                    x.Id,
                    monitoringYear = x.MonitoringYear ?? "-",
                    monitoringMonth = x.MonitoringMonth ?? "-",
                    monitoringCount = x.MonitoringCount ?? 0,
                    monitoringRemarks = x.MonitoringRemarks ?? "-",
                    registrationDate = x.RegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.RegistrationDate, calendar)
                        : "-",
                    createdBy = x.CreatedBy ?? "-"
                }).ToList();

                return Ok(new { summary, details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت گزارش نظارت", error = ex.Message });
            }
        }

        /// <summary>
        /// Export report data as CSV for Excel
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? createdBy = null,
            [FromQuery] string? sectionType = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);
                var query = BuildBaseQuery(startDate, endDate, createdBy, calendar);

                if (!string.IsNullOrEmpty(sectionType))
                {
                    query = query.Where(x => x.SectionType == sectionType);
                }

                var records = await query.ToListAsync();

                var userNameCache = new Dictionary<string, string>();
                var resolvedRecords = new List<object>();
                foreach (var x in records)
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

                    resolvedRecords.Add(new
                    {
                        x.Id,
                        x.SerialNumber,
                        sectionType = x.SectionType,
                        registrationDate = x.RegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.RegistrationDate, calendar)
                            : "",
                        petitionWriterName = x.PetitionWriterName,
                        petitionWriterLicenseNumber = x.PetitionWriterLicenseNumber,
                        petitionWriterDistrict = x.PetitionWriterDistrict,
                        complainantName = x.ComplainantName,
                        complaintSubject = x.ComplaintSubject,
                        complaintActionsTaken = x.ComplaintActionsTaken,
                        violationType = x.ViolationType,
                        activityStatus = x.ActivityStatus,
                        activityPermissionReason = x.ActivityPermissionReason,
                        violationActionsTaken = x.ViolationActionsTaken,
                        monitoringYear = x.MonitoringYear,
                        monitoringMonth = x.MonitoringMonth,
                        monitoringCount = x.MonitoringCount,
                        monitoringRemarks = x.MonitoringRemarks,
                        createdBy = resolvedName
                    });
                }

                return Ok(resolvedRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در صادرات گزارش", error = ex.Message });
            }
        }

        #region Private Helpers

        private IQueryable<PetitionWriterMonitoringRecord> BuildBaseQuery(
            string? startDate, string? endDate, string? createdBy, CalendarType calendar)
        {
            var query = _context.PetitionWriterMonitoringRecords
                .AsNoTracking()
                .Where(x => x.Status == true);

            if (!string.IsNullOrWhiteSpace(startDate))
            {
                if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    query = query.Where(x => x.RegistrationDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    query = query.Where(x => x.RegistrationDate <= end);
            }
            if (!string.IsNullOrWhiteSpace(createdBy))
            {
                query = query.Where(x => x.CreatedBy == createdBy);
            }

            return query;
        }

        private static object BuildComplaintsSummary(List<PetitionWriterMonitoringRecord> records)
        {
            return new
            {
                totalRecords = records.Count
            };
        }

        private static object BuildViolationsSummary(List<PetitionWriterMonitoringRecord> records)
        {
            return new
            {
                totalRecords = records.Count,
                activityPreventionCount = records.Count(x => x.ActivityStatus == "activity_prevention"),
                activityPermissionCount = records.Count(x => x.ActivityStatus == "activity_permission")
            };
        }

        private static object BuildMonitoringSummary(List<PetitionWriterMonitoringRecord> records)
        {
            return new
            {
                totalRecords = records.Count,
                totalMonitoringCount = records.Sum(x => x.MonitoringCount ?? 0)
            };
        }

        #endregion
    }
}
