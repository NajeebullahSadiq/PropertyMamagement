using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.ActivityMonitoring;

namespace WebAPIBackend.Controllers.ActivityMonitoring
{
    /// <summary>
    /// Report Controller for Activity Monitoring
    /// گزارش‌گیری نظارت بر فعالیت دفاتر رهنمای معاملات
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityMonitoringReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityMonitoringReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
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
        /// Get all users who have created activity monitoring records (for filter dropdown)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var createdByValues = await _context.ActivityMonitoringRecords
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

                var annualReportRecords = records.Where(x => x.SectionType == "annualReport").ToList();
                var complaintsRecords = records.Where(x => x.SectionType == "complaints").ToList();
                var violationsRecords = records.Where(x => x.SectionType == "violations").ToList();
                var inspectionRecords = records.Where(x => x.SectionType == "inspection").ToList();

                var result = new
                {
                    totalRecords = records.Count,
                    annualReport = BuildAnnualReportSummary(annualReportRecords, calendar),
                    complaints = BuildComplaintsSummary(complaintsRecords, calendar),
                    violations = BuildViolationsSummary(violationsRecords, calendar),
                    inspection = BuildInspectionSummary(inspectionRecords, calendar)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// Get annual report section details
        /// </summary>
        [HttpGet("annual-report")]
        public async Task<IActionResult> GetAnnualReport(
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
                    .Where(x => x.SectionType == "annualReport")
                    .ToListAsync();

                var summary = BuildAnnualReportSummary(records, calendar);
                var details = records.Select(x => new
                {
                    x.Id,
                    x.SerialNumber,
                    x.LicenseNumber,
                    x.LicenseHolderName,
                    x.CompanyTitle,
                    x.District,
                    reportRegistrationDate = x.ReportRegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
                        : "-",
                    saleDeedsCount = x.SaleDeedsCount ?? 0,
                    rentalDeedsCount = x.RentalDeedsCount ?? 0,
                    baiUlWafaDeedsCount = x.BaiUlWafaDeedsCount ?? 0,
                    vehicleTransactionDeedsCount = x.VehicleTransactionDeedsCount ?? 0,
                    totalDeedsCount = (x.SaleDeedsCount ?? 0) + (x.RentalDeedsCount ?? 0) + (x.BaiUlWafaDeedsCount ?? 0) + (x.VehicleTransactionDeedsCount ?? 0),
                    taxAmount = x.TaxAmount ?? 0,
                    createdBy = x.CreatedBy ?? "-"
                }).ToList();

                return Ok(new { summary, details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت گزارش سالانه", error = ex.Message });
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

                var summary = BuildComplaintsSummary(records, calendar);
                var details = records.Select(x => new
                {
                    x.Id,
                    x.SerialNumber,
                    x.LicenseNumber,
                    x.LicenseHolderName,
                    x.CompanyTitle,
                    x.District,
                    complainantName = x.ComplainantName ?? "-",
                    complaintSubject = x.ComplaintSubject ?? "-",
                    complaintActionsTaken = x.ComplaintActionsTaken ?? "-",
                    reportRegistrationDate = x.ReportRegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
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

                var summary = BuildViolationsSummary(records, calendar);
                var details = records.Select(x => new
                {
                    x.Id,
                    x.SerialNumber,
                    x.LicenseNumber,
                    x.LicenseHolderName,
                    x.CompanyTitle,
                    x.District,
                    violationStatus = x.ViolationStatus ?? "-",
                    violationType = x.ViolationType ?? "-",
                    closureReason = x.ClosureReason ?? "-",
                    sealRemovalReason = x.SealRemovalReason ?? "-",
                    violationActionsTaken = x.ViolationActionsTaken ?? "-",
                    reportRegistrationDate = x.ReportRegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
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
        /// Get inspection section details
        /// </summary>
        [HttpGet("inspection")]
        public async Task<IActionResult> GetInspection(
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
                    .Where(x => x.SectionType == "inspection")
                    .ToListAsync();

                var summary = BuildInspectionSummary(records, calendar);
                var details = records.Select(x => new
                {
                    x.Id,
                    x.Year,
                    x.Month,
                    monitoringCount = x.MonitoringCount ?? 0,
                    monitoringRemarks = x.MonitoringRemarks ?? "-",
                    reportRegistrationDate = x.ReportRegistrationDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
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

                // Resolve user names
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
                        x.LicenseNumber,
                        x.LicenseHolderName,
                        x.CompanyTitle,
                        x.District,
                        sectionType = x.SectionType,
                        reportRegistrationDate = x.ReportRegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
                            : "",
                        saleDeedsCount = x.SaleDeedsCount,
                        rentalDeedsCount = x.RentalDeedsCount,
                        baiUlWafaDeedsCount = x.BaiUlWafaDeedsCount,
                        vehicleTransactionDeedsCount = x.VehicleTransactionDeedsCount,
                        taxAmount = x.TaxAmount,
                        complainantName = x.ComplainantName,
                        complaintSubject = x.ComplaintSubject,
                        complaintActionsTaken = x.ComplaintActionsTaken,
                        violationStatus = x.ViolationStatus,
                        violationType = x.ViolationType,
                        closureReason = x.ClosureReason,
                        sealRemovalReason = x.SealRemovalReason,
                        violationActionsTaken = x.ViolationActionsTaken,
                        year = x.Year,
                        month = x.Month,
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

        private IQueryable<ActivityMonitoringRecord> BuildBaseQuery(
            string? startDate, string? endDate, string? createdBy, CalendarType calendar)
        {
            var query = _context.ActivityMonitoringRecords
                .AsNoTracking()
                .Where(x => x.Status == true);

            if (!string.IsNullOrWhiteSpace(startDate))
            {
                if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                    query = query.Where(x => x.ReportRegistrationDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                    query = query.Where(x => x.ReportRegistrationDate <= end);
            }
            if (!string.IsNullOrWhiteSpace(createdBy))
            {
                query = query.Where(x => x.CreatedBy == createdBy);
            }

            return query;
        }

        private static object BuildAnnualReportSummary(List<ActivityMonitoringRecord> records, CalendarType calendar)
        {
            return new
            {
                totalRecords = records.Count,
                saleDeedsCount = records.Sum(x => x.SaleDeedsCount ?? 0),
                rentalDeedsCount = records.Sum(x => x.RentalDeedsCount ?? 0),
                baiUlWafaDeedsCount = records.Sum(x => x.BaiUlWafaDeedsCount ?? 0),
                vehicleTransactionDeedsCount = records.Sum(x => x.VehicleTransactionDeedsCount ?? 0),
                totalDeedsCount = records.Sum(x => (x.SaleDeedsCount ?? 0) + (x.RentalDeedsCount ?? 0) + (x.BaiUlWafaDeedsCount ?? 0) + (x.VehicleTransactionDeedsCount ?? 0)),
                totalTaxAmount = records.Sum(x => x.TaxAmount ?? 0)
            };
        }

        private static object BuildComplaintsSummary(List<ActivityMonitoringRecord> records, CalendarType calendar)
        {
            return new
            {
                totalRecords = records.Count
            };
        }

        private static object BuildViolationsSummary(List<ActivityMonitoringRecord> records, CalendarType calendar)
        {
            return new
            {
                totalRecords = records.Count,
                blockedCount = records.Count(x => x.ViolationStatus == "blocked"),
                normalCount = records.Count(x => x.ViolationStatus == "normal"),
                sealRemovedCount = records.Count(x => x.ViolationStatus == "sealRemoved")
            };
        }

        private static object BuildInspectionSummary(List<ActivityMonitoringRecord> records, CalendarType calendar)
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
