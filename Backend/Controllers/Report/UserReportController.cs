using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;

namespace WebAPIBackend.Controllers.Report
{
    [Authorize(Roles = "ADMIN,AUTHORITY")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get user report summary: total, active, inactive (by lock + expired license + cancelled)
        /// </summary>
        [HttpGet]
        [Route("Summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? districtId = null)
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            DateOnly? parsedStartDate = null;
            DateOnly? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var sd))
                parsedStartDate = sd;
            if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var ed))
                parsedEndDate = ed;

            var allUsers = _userManager.Users.AsNoTracking().AsQueryable();

            // Date range filter (by CreatedAt)
            if (parsedStartDate.HasValue && allUsers.Any(u => u.CreatedAt.HasValue))
                allUsers = allUsers.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date >= parsedStartDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEndDate.HasValue && allUsers.Any(u => u.CreatedAt.HasValue))
                allUsers = allUsers.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date <= parsedEndDate.Value.ToDateTime(TimeOnly.MinValue).Date);

            // Province filter
            if (provinceId.HasValue)
                allUsers = allUsers.Where(u => u.ProvinceId == provinceId.Value);

            // District filter - users in companies whose province matches the district's parent
            if (districtId.HasValue)
            {
                var district = await _context.Locations.AsNoTracking().FirstOrDefaultAsync(l => l.Id == districtId.Value);
                if (district != null && district.ParentId.HasValue)
                {
                    var districtProvinceId = district.ParentId.Value;
                    allUsers = allUsers.Where(u => u.ProvinceId == districtProvinceId);
                }
            }

            var userList = await allUsers.ToListAsync();

            // Get all company IDs for company users
            var companyUserIds = userList.Where(u => u.CompanyId > 0).Select(u => u.CompanyId).Distinct().ToList();

            // Get expired license company IDs
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expiredLicenseCompanyIds = await _context.LicenseDetails.AsNoTracking()
                .Where(l => l.ExpireDate.HasValue && l.ExpireDate.Value < today && companyUserIds.Contains(l.CompanyId ?? 0))
                .Select(l => l.CompanyId ?? 0)
                .Distinct()
                .ToListAsync();

            // Get cancelled (فسخ / لغوه) company IDs
            var cancelledCompanyIds = await _context.CompanyCancellationInfos.AsNoTracking()
                .Where(c => c.Status != false && (c.CancellationType == "فسخ" || c.CancellationType == "لغوه"))
                .Where(c => companyUserIds.Contains(c.CompanyId))
                .Select(c => c.CompanyId)
                .Distinct()
                .ToListAsync();

            // Combine: inactive = IsLocked OR (company user with expired license) OR (company user with cancelled license)
            var inactiveByLock = userList.Count(u => u.IsLocked);
            var inactiveByExpiredLicense = userList.Count(u => !u.IsLocked && u.CompanyId > 0 && expiredLicenseCompanyIds.Contains(u.CompanyId));
            var inactiveByCancellation = userList.Count(u => !u.IsLocked && u.CompanyId > 0 && cancelledCompanyIds.Contains(u.CompanyId) && !expiredLicenseCompanyIds.Contains(u.CompanyId));
            var totalInactive = userList.Count(u =>
                u.IsLocked ||
                (u.CompanyId > 0 && expiredLicenseCompanyIds.Contains(u.CompanyId)) ||
                (u.CompanyId > 0 && cancelledCompanyIds.Contains(u.CompanyId)));

            var totalActive = userList.Count - totalInactive;

            // System vs Company users
            var systemUsers = userList.Count(u => u.UserRole != "PROPERTY_OPERATOR" && u.UserRole != "VEHICLE_OPERATOR");
            var companyUsers = userList.Count - systemUsers;

            // By role
            var byRole = userList
                .GroupBy(u => u.UserRole ?? "UNKNOWN")
                .Select(g => new
                {
                    Role = g.Key,
                    RoleDari = UserRoles.GetDariName(g.Key),
                    Count = g.Count(),
                    Active = g.Count(u => !u.IsLocked && !(u.CompanyId > 0 && expiredLicenseCompanyIds.Contains(u.CompanyId)) && !(u.CompanyId > 0 && cancelledCompanyIds.Contains(u.CompanyId))),
                    Inactive = g.Count(u => u.IsLocked || (u.CompanyId > 0 && expiredLicenseCompanyIds.Contains(u.CompanyId)) || (u.CompanyId > 0 && cancelledCompanyIds.Contains(u.CompanyId)))
                })
                .OrderByDescending(r => r.Count)
                .ToList();

            // By license type
            var byLicenseType = userList
                .Where(u => u.CompanyId > 0)
                .GroupBy(u => u.LicenseType ?? "none")
                .Select(g => new
                {
                    LicenseType = g.Key,
                    LicenseTypeDari = g.Key == "realEstate" ? "املاک" : g.Key == "carSale" ? "موتر فروشی" : "—",
                    Count = g.Count(),
                    Active = g.Count(u => !u.IsLocked && !expiredLicenseCompanyIds.Contains(u.CompanyId) && !cancelledCompanyIds.Contains(u.CompanyId)),
                    Inactive = g.Count(u => u.IsLocked || expiredLicenseCompanyIds.Contains(u.CompanyId) || cancelledCompanyIds.Contains(u.CompanyId))
                })
                .ToList();

            return Ok(new
            {
                TotalUsers = userList.Count,
                ActiveUsers = totalActive,
                InactiveUsers = totalInactive,
                InactiveByLock = inactiveByLock,
                InactiveByExpiredLicense = inactiveByExpiredLicense,
                InactiveByCancellation = inactiveByCancellation,
                SystemUsers = systemUsers,
                CompanyUsers = companyUsers,
                ByRole = byRole,
                ByLicenseType = byLicenseType
            });
        }

        /// <summary>
        /// Get users grouped by province
        /// </summary>
        [HttpGet]
        [Route("ByProvince")]
        public async Task<IActionResult> GetByProvince(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            DateOnly? parsedStartDate = null;
            DateOnly? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var sd))
                parsedStartDate = sd;
            if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var ed))
                parsedEndDate = ed;

            var query = _userManager.Users.AsNoTracking().AsQueryable();

            if (parsedStartDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date >= parsedStartDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEndDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date <= parsedEndDate.Value.ToDateTime(TimeOnly.MinValue).Date);

            var users = await query.ToListAsync();
            var companyIds = users.Where(u => u.CompanyId > 0).Select(u => u.CompanyId).Distinct().ToList();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expiredCompanyIds = await _context.LicenseDetails.AsNoTracking()
                .Where(l => l.ExpireDate.HasValue && l.ExpireDate.Value < today && companyIds.Contains(l.CompanyId ?? 0))
                .Select(l => l.CompanyId ?? 0).Distinct().ToListAsync();
            var cancelledCompanyIds = await _context.CompanyCancellationInfos.AsNoTracking()
                .Where(c => c.Status != false && (c.CancellationType == "فسخ" || c.CancellationType == "لغوه") && companyIds.Contains(c.CompanyId))
                .Select(c => c.CompanyId).Distinct().ToListAsync();

            var provinces = await _context.Locations.AsNoTracking()
                .Where(l => l.ParentId == null && l.IsActive == 1)
                .ToListAsync();

            var result = provinces.Select(p =>
            {
                var provinceUsers = users.Where(u => u.ProvinceId == p.Id).ToList();
                var total = provinceUsers.Count;
                var inactive = provinceUsers.Count(u =>
                    u.IsLocked ||
                    (u.CompanyId > 0 && expiredCompanyIds.Contains(u.CompanyId)) ||
                    (u.CompanyId > 0 && cancelledCompanyIds.Contains(u.CompanyId)));

                return new
                {
                    ProvinceId = p.Id,
                    ProvinceName = p.Name ?? p.Dari,
                    ProvinceDari = p.Dari,
                    TotalUsers = total,
                    ActiveUsers = total - inactive,
                    InactiveUsers = inactive
                };
            }).Where(p => p.TotalUsers > 0).OrderByDescending(p => p.TotalUsers).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Get users grouped by district within a province
        /// </summary>
        [HttpGet]
        [Route("ByDistrict/{provinceId}")]
        public async Task<IActionResult> GetByDistrict(int provinceId,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            DateOnly? parsedStartDate = null;
            DateOnly? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var sd))
                parsedStartDate = sd;
            if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var ed))
                parsedEndDate = ed;

            var districts = await _context.Locations.AsNoTracking()
                .Where(l => l.ParentId == provinceId && l.IsActive == 1)
                .ToListAsync();

            // For district-level, we count company users whose company province matches
            var query = _userManager.Users.AsNoTracking().AsQueryable();

            if (parsedStartDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date >= parsedStartDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEndDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date <= parsedEndDate.Value.ToDateTime(TimeOnly.MinValue).Date);

            var users = await query.Where(u => u.ProvinceId == provinceId).ToListAsync();

            // Since districts are sub-levels of province, we show province-level users grouped by district
            // For now, we show the province total and districts as sub-items
            var companyIds = users.Where(u => u.CompanyId > 0).Select(u => u.CompanyId).Distinct().ToList();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expiredCompanyIds = await _context.LicenseDetails.AsNoTracking()
                .Where(l => l.ExpireDate.HasValue && l.ExpireDate.Value < today && companyIds.Contains(l.CompanyId ?? 0))
                .Select(l => l.CompanyId ?? 0).Distinct().ToListAsync();
            var cancelledCompanyIds = await _context.CompanyCancellationInfos.AsNoTracking()
                .Where(c => c.Status != false && (c.CancellationType == "فسخ" || c.CancellationType == "لغوه") && companyIds.Contains(c.CompanyId))
                .Select(c => c.CompanyId).Distinct().ToListAsync();

            var total = users.Count;
            var inactive = users.Count(u =>
                u.IsLocked ||
                (u.CompanyId > 0 && expiredCompanyIds.Contains(u.CompanyId)) ||
                (u.CompanyId > 0 && cancelledCompanyIds.Contains(u.CompanyId)));

            var result = new
            {
                ProvinceId = provinceId,
                TotalUsers = total,
                ActiveUsers = total - inactive,
                InactiveUsers = inactive,
                Districts = districts.Select(d => new
                {
                    DistrictId = d.Id,
                    DistrictName = d.Name ?? d.Dari,
                    DistrictDari = d.Dari
                }).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Get detailed list of inactive users (expired license or cancelled)
        /// </summary>
        [HttpGet]
        [Route("InactiveUsers")]
        public async Task<IActionResult> GetInactiveUsers(
            [FromQuery] string? reason = null, // "expired" | "cancelled" | "locked" | null (all)
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            DateOnly? parsedStartDate = null;
            DateOnly? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var sd))
                parsedStartDate = sd;
            if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var ed))
                parsedEndDate = ed;

            var query = _userManager.Users.AsNoTracking().AsQueryable();

            if (parsedStartDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date >= parsedStartDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEndDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date <= parsedEndDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (provinceId.HasValue)
                query = query.Where(u => u.ProvinceId == provinceId.Value);

            var allUsers = await query.ToListAsync();
            var companyIds = allUsers.Where(u => u.CompanyId > 0).Select(u => u.CompanyId).Distinct().ToList();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Get expired license details per company
            var expiredLicenses = await _context.LicenseDetails.AsNoTracking()
                .Where(l => l.ExpireDate.HasValue && l.ExpireDate.Value < today && companyIds.Contains(l.CompanyId ?? 0))
                .ToListAsync();

            var expiredCompanyIds = expiredLicenses.Select(l => l.CompanyId ?? 0).Distinct().ToList();

            // Get cancelled company details
            var cancellations = await _context.CompanyCancellationInfos.AsNoTracking()
                .Where(c => c.Status != false && (c.CancellationType == "فسخ" || c.CancellationType == "لغوه") && companyIds.Contains(c.CompanyId))
                .ToListAsync();

            var cancelledCompanyIds = cancellations.Select(c => c.CompanyId).Distinct().ToList();

            // Build result list
            var inactiveList = new List<object>();

            foreach (var user in allUsers)
            {
                var reasons = new List<string>();
                var details = new List<object>();

                if (user.IsLocked)
                {
                    reasons.Add("locked");
                    details.Add(new { Reason = "قفل شده", ReasonEn = "locked" });
                }

                if (user.CompanyId > 0 && expiredCompanyIds.Contains(user.CompanyId))
                {
                    reasons.Add("expired");
                    var expLic = expiredLicenses.FirstOrDefault(l => l.CompanyId == user.CompanyId);
                    details.Add(new
                    {
                        Reason = "ختم جواز",
                        ReasonEn = "expired",
                        ExpireDate = expLic?.ExpireDate?.ToString("yyyy-MM-dd"),
                        LicenseNumber = expLic?.LicenseNumber
                    });
                }

                if (user.CompanyId > 0 && cancelledCompanyIds.Contains(user.CompanyId))
                {
                    reasons.Add("cancelled");
                    var canc = cancellations.FirstOrDefault(c => c.CompanyId == user.CompanyId);
                    details.Add(new
                    {
                        Reason = canc?.CancellationType == "فسخ" ? "فسخ" : "لغوه",
                        ReasonEn = "cancelled",
                        CancellationType = canc?.CancellationType,
                        CancellationDate = canc?.CancellationType == "فسخ"
                            ? canc?.LicenseCancellationLetterDate?.ToString("yyyy-MM-dd")
                            : canc?.RevocationLetterDate?.ToString("yyyy-MM-dd")
                    });
                }

                if (reasons.Count == 0) continue;

                // Filter by reason if specified
                if (!string.IsNullOrWhiteSpace(reason) && !reasons.Contains(reason)) continue;

                var province = user.ProvinceId.HasValue
                    ? await _context.Locations.AsNoTracking().Where(l => l.Id == user.ProvinceId.Value).Select(l => new { l.Id, l.Name, l.Dari }).FirstOrDefaultAsync()
                    : null;

                var company = user.CompanyId > 0
                    ? await _context.CompanyDetails.AsNoTracking().Where(c => c.Id == user.CompanyId).Select(c => new { c.Id, c.Title }).FirstOrDefaultAsync()
                    : null;

                inactiveList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.CompanyId,
                    CompanyName = company?.Title,
                    user.LicenseType,
                    LicenseTypeDari = user.LicenseType == "realEstate" ? "املاک" : user.LicenseType == "carSale" ? "موتر فروشی" : "—",
                    user.IsLocked,
                    user.CreatedAt,
                    Province = province,
                    Role = user.UserRole ?? "",
                    RoleDari = UserRoles.GetDariName(user.UserRole ?? ""),
                    Reasons = reasons,
                    Details = details
                });
            }

            var total = inactiveList.Count;
            var paged = inactiveList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new { total, page, pageSize, users = paged });
        }

        /// <summary>
        /// Get users registration trend by month
        /// </summary>
        [HttpGet]
        [Route("RegistrationTrend")]
        public async Task<IActionResult> GetRegistrationTrend(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            DateOnly? parsedStartDate = null;
            DateOnly? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var sd))
                parsedStartDate = sd;
            if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var ed))
                parsedEndDate = ed;

            var query = _userManager.Users.AsNoTracking()
                .Where(u => u.CreatedAt.HasValue);

            if (parsedStartDate.HasValue)
                query = query.Where(u => u.CreatedAt!.Value.Date >= parsedStartDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEndDate.HasValue)
                query = query.Where(u => u.CreatedAt!.Value.Date <= parsedEndDate.Value.ToDateTime(TimeOnly.MinValue).Date);

            var users = await query.ToListAsync();

            var trend = users
                .GroupBy(u => new DateTime(u.CreatedAt!.Value.Year, u.CreatedAt.Value.Month, 1))
                .Select(g => new
                {
                    Month = g.Key.ToString("yyyy-MM"),
                    MonthLabel = g.Key.ToString("MMM yyyy"),
                    TotalRegistrations = g.Count(),
                    SystemUsers = g.Count(u => u.UserRole != "PROPERTY_OPERATOR" && u.UserRole != "VEHICLE_OPERATOR"),
                    CompanyUsers = g.Count(u => u.UserRole == "PROPERTY_OPERATOR" || u.UserRole == "VEHICLE_OPERATOR")
                })
                .OrderBy(t => t.Month)
                .ToList();

            return Ok(trend);
        }

        /// <summary>
        /// Get expiring licenses report (licenses expiring soon)
        /// </summary>
        [HttpGet]
        [Route("ExpiringLicenses")]
        public async Task<IActionResult> GetExpiringLicenses(
            [FromQuery] int daysAhead = 30,
            [FromQuery] int? provinceId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var futureDate = today.AddDays(daysAhead);

            var query = _context.LicenseDetails.AsNoTracking()
                .Where(l => l.ExpireDate.HasValue && l.ExpireDate.Value >= today && l.ExpireDate.Value <= futureDate);

            if (provinceId.HasValue)
                query = query.Where(l => l.ProvinceId == provinceId.Value);

            var total = await query.CountAsync();
            var licenses = await query
                .OrderBy(l => l.ExpireDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<object>();
            foreach (var lic in licenses)
            {
                var company = lic.CompanyId.HasValue
                    ? await _context.CompanyDetails.AsNoTracking().Where(c => c.Id == lic.CompanyId.Value).Select(c => new { c.Id, c.Title }).FirstOrDefaultAsync()
                    : null;

                var companyUsers = lic.CompanyId.HasValue
                    ? (await _userManager.Users.AsNoTracking().Where(u => u.CompanyId == lic.CompanyId.Value).Select(u => new { u.Id, u.UserName, u.FirstName, u.LastName }).ToListAsync()).Cast<object>().ToList()
                    : new List<object>();

                var province = lic.ProvinceId.HasValue
                    ? await _context.Locations.AsNoTracking().Where(l => l.Id == lic.ProvinceId.Value).Select(l => new { l.Id, l.Name, l.Dari }).FirstOrDefaultAsync()
                    : null;

                var daysRemaining = lic.ExpireDate.HasValue ? (lic.ExpireDate.Value.DayNumber - today.DayNumber) : 0;

                result.Add(new
                {
                    lic.Id,
                    lic.LicenseNumber,
                    lic.LicenseType,
                    LicenseTypeDari = lic.LicenseType == "realEstate" ? "املاک" : lic.LicenseType == "carSale" ? "موتر فروشی" : "—",
                    lic.ExpireDate,
                    DaysRemaining = daysRemaining,
                    Company = company,
                    Province = province,
                    Users = companyUsers
                });
            }

            return Ok(new { total, page, pageSize, daysAhead, data = result });
        }

        /// <summary>
        /// Check and auto-deactivate users with expired licenses or cancelled companies
        /// Called during login or on-demand
        /// </summary>
        [HttpPost]
        [Route("AutoDeactivate")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AutoDeactivate()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var deactivatedCount = 0;

            // Get all company users that are NOT locked
            var companyUsers = await _userManager.Users
                .Where(u => !u.IsLocked && (u.UserRole == "PROPERTY_OPERATOR" || u.UserRole == "VEHICLE_OPERATOR"))
                .ToListAsync();

            var companyIds = companyUsers.Select(u => u.CompanyId).Distinct().ToList();

            // Find companies with expired licenses
            var expiredCompanyIds = await _context.LicenseDetails.AsNoTracking()
                .Where(l => l.ExpireDate.HasValue && l.ExpireDate.Value < today && companyIds.Contains(l.CompanyId ?? 0))
                .Select(l => l.CompanyId ?? 0)
                .Distinct()
                .ToListAsync();

            // Find companies with فسخ or لغوه
            var cancelledCompanyIds = await _context.CompanyCancellationInfos.AsNoTracking()
                .Where(c => c.Status != false && (c.CancellationType == "فسخ" || c.CancellationType == "لغوه"))
                .Where(c => companyIds.Contains(c.CompanyId))
                .Select(c => c.CompanyId)
                .Distinct()
                .ToListAsync();

            var toDeactivate = companyIds.Where(id => expiredCompanyIds.Contains(id) || cancelledCompanyIds.Contains(id)).ToList();

            foreach (var user in companyUsers)
            {
                if (toDeactivate.Contains(user.CompanyId))
                {
                    user.IsLocked = true;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded) deactivatedCount++;
                }
            }

            return Ok(new
            {
                message = $"{deactivatedCount} کاربر به دلیل ختم جواز یا فسخ/لغوه غیرفعال شد",
                deactivatedCount,
                expiredLicenseCompanies = expiredCompanyIds.Count,
                cancelledCompanies = cancelledCompanyIds.Count
            });
        }
    }
}
