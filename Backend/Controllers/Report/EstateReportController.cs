using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;

namespace WebAPIBackend.Controllers.Report
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EstateReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EstateReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private sealed class ReportAuditUserCandidate
        {
            public int RecordId { get; set; }
            public string? UpdatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        private async Task<string> ResolveDisplayName(string? userIdOrName)
        {
            if (string.IsNullOrWhiteSpace(userIdOrName))
            {
                return "-";
            }

            if (Guid.TryParse(userIdOrName, out _))
            {
                var user = await _userManager.FindByIdAsync(userIdOrName);
                if (user != null)
                {
                    var fullName = $"{user.FirstName} {user.LastName}".Trim();
                    return string.IsNullOrWhiteSpace(fullName) ? (user.UserName ?? userIdOrName) : fullName;
                }
            }

            return userIdOrName;
        }

        private async Task<Dictionary<string, string>> BuildUserDisplayLookup(IEnumerable<string?> userValues)
        {
            var lookup = new Dictionary<string, string>();
            var distinctValues = userValues
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .ToList();

            foreach (var userValue in distinctValues)
            {
                lookup[userValue] = await ResolveDisplayName(userValue);
            }

            return lookup;
        }

        private async Task<Dictionary<int, string?>> GetLatestUpdatedByByProperty(List<int> propertyIds)
        {
            var auditCandidates = new List<ReportAuditUserCandidate>();

            auditCandidates.AddRange(await _context.Propertyaudits
                .AsNoTracking()
                .Where(a => propertyIds.Contains(a.PropertyId))
                .Select(a => new ReportAuditUserCandidate
                {
                    RecordId = a.PropertyId,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync());

            auditCandidates.AddRange(await _context.Propertyselleraudits
                .AsNoTracking()
                .Where(a => a.Seller.PropertyDetailsId.HasValue && propertyIds.Contains(a.Seller.PropertyDetailsId.Value))
                .Select(a => new ReportAuditUserCandidate
                {
                    RecordId = a.Seller.PropertyDetailsId!.Value,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync());

            auditCandidates.AddRange(await _context.Propertybuyeraudits
                .AsNoTracking()
                .Where(a => a.Buyer.PropertyDetailsId.HasValue && propertyIds.Contains(a.Buyer.PropertyDetailsId.Value))
                .Select(a => new ReportAuditUserCandidate
                {
                    RecordId = a.Buyer.PropertyDetailsId!.Value,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync());

            return auditCandidates
                .Where(a => a.RecordId != 0 && !string.IsNullOrWhiteSpace(a.UpdatedBy))
                .GroupBy(a => a.RecordId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(a => a.UpdatedAt ?? DateTime.MinValue).First().UpdatedBy);
        }

        /// <summary>
        /// Transaction type Dari names mapping
        /// </summary>
        private static readonly Dictionary<string, string> TransactionTypeDariMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Sale"] = "خرید و فروش",
            ["Purchase"] = "خرید و فروش",
            ["Rent"] = "کرایه",
            ["Lease"] = "اجاره",
            ["Mortgage"] = "رهن",
            ["Exchange"] = "تبادله",
            ["Gift"] = "هبه",
            ["Inheritance"] = "ارث",
            ["Revocable Sale"] = "بیع جایزی",
            ["Other"] = "سایر"
        };

        private static string GetTransactionTypeDari(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "نامشخص";
            return TransactionTypeDariMap.TryGetValue(name, out var dari) ? dari : name;
        }

        /// <summary>
        /// Get RBAC-filtered base query for property details
        /// </summary>
        private async Task<IQueryable<PropertyDetail>> GetFilteredQuery()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new UnauthorizedAccessException();

            var roles = await _userManager.GetRolesAsync(user);

            IQueryable<PropertyDetail> query;

            if (RbacHelper.CanViewAllRecords(roles, "property"))
            {
                query = _context.PropertyDetails.AsNoTracking();
            }
            else if (RbacHelper.ShouldFilterByCompany(roles, "property"))
            {
                query = _context.PropertyDetails.AsNoTracking()
                    .Where(p => p.CompanyId == user.CompanyId);
            }
            else
            {
                query = _context.PropertyDetails.AsNoTracking()
                    .Where(p => p.CreatedBy == userId);
            }

            return query;
        }

        /// <summary>
        /// Parse date range from request
        /// </summary>
        private static bool TryParseDates(string? startDate, string? endDate, string? calendarType, out DateOnly? sd, out DateOnly? ed)
        {
            sd = null; ed = null;
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);

            if (!string.IsNullOrWhiteSpace(startDate) && DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var s))
                sd = s;
            if (!string.IsNullOrWhiteSpace(endDate) && DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var e))
                ed = e;

            return true;
        }

        /// <summary>
        /// Summary report: count and sums by transaction type
        /// </summary>
        [HttpGet]
        [Route("Summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int? companyId = null)
        {
            TryParseDates(startDate, endDate, calendarType, out var parsedStart, out var parsedEnd);

            var query = await GetFilteredQuery();

            // Join with PropertyAddress for province/district filtering
            if (provinceId.HasValue || districtId.HasValue)
            {
                var addressQuery = _context.PropertyAddresses.AsNoTracking();
                if (provinceId.HasValue)
                    addressQuery = addressQuery.Where(a => a.ProvinceId == provinceId.Value);
                if (districtId.HasValue)
                    addressQuery = addressQuery.Where(a => a.DistrictId == districtId.Value);

                var propertyIds = await addressQuery.Select(a => a.PropertyDetailsId).Distinct().ToListAsync();
                query = query.Where(p => propertyIds.Contains(p.Id));
            }

            // Company filter
            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);

            // Date range filter (by CreatedAt)
            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var properties = await query
                .Include(p => p.TransactionType)
                .Include(p => p.Company)
                .ToListAsync();

            var totalRecords = properties.Count;
            var totalRecordsComplete = properties.Count(p => p.iscomplete == true);

            // Group by transaction type
            var byTransactionType = properties
                .GroupBy(p => new { Id = p.TransactionTypeId ?? 0, Name = p.TransactionType?.Name ?? "نامشخص" })
                .Select(g =>
                {
                    var items = g.ToList();
                    var totalPrice = items.Sum(p => TryParseDecimal(p.Price));
                    var totalRoyalty = items.Sum(p => TryParseDecimal(p.RoyaltyAmount));

                    return new
                    {
                        TransactionTypeId = g.Key.Id,
                        TransactionTypeName = g.Key.Name,
                        TransactionTypeDari = GetTransactionTypeDari(g.Key.Name),
                        Count = items.Count,
                        CompleteCount = items.Count(p => p.iscomplete == true),
                        TotalPrice = totalPrice,
                        TotalRoyaltyAmount = totalRoyalty
                    };
                })
                .OrderByDescending(t => t.Count)
                .ToList();

            // Overall totals
            var grandTotalPrice = properties.Sum(p => TryParseDecimal(p.Price));
            var grandTotalRoyalty = properties.Sum(p => TryParseDecimal(p.RoyaltyAmount));

            // By property type
            var byPropertyType = properties
                .Where(p => p.PropertyTypeId.HasValue)
                .GroupBy(p => new { Id = p.PropertyTypeId!.Value, Name = p.PropertyType != null && p.PropertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase) && p.CustomPropertyType != null ? p.CustomPropertyType : (p.PropertyType?.Name ?? "نامشخص") })
                .Select(g => new
                {
                    PropertyTypeId = g.Key.Id,
                    PropertyTypeName = g.Key.Name,
                    Count = g.Count(),
                    TotalPrice = g.ToList().Sum(p => TryParseDecimal(p.Price)),
                    TotalRoyaltyAmount = g.ToList().Sum(p => TryParseDecimal(p.RoyaltyAmount))
                })
                .OrderByDescending(t => t.Count)
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                CompleteRecords = totalRecordsComplete,
                GrandTotalPrice = grandTotalPrice,
                GrandTotalRoyaltyAmount = grandTotalRoyalty,
                ByTransactionType = byTransactionType,
                ByPropertyType = byPropertyType
            });
        }

        /// <summary>
        /// Report grouped by company
        /// </summary>
        [HttpGet]
        [Route("ByCompany")]
        public async Task<IActionResult> GetByCompany(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? districtId = null)
        {
            TryParseDates(startDate, endDate, calendarType, out var parsedStart, out var parsedEnd);

            var query = await GetFilteredQuery();

            if (provinceId.HasValue || districtId.HasValue)
            {
                var addressQuery = _context.PropertyAddresses.AsNoTracking();
                if (provinceId.HasValue) addressQuery = addressQuery.Where(a => a.ProvinceId == provinceId.Value);
                if (districtId.HasValue) addressQuery = addressQuery.Where(a => a.DistrictId == districtId.Value);
                var propertyIds = await addressQuery.Select(a => a.PropertyDetailsId).Distinct().ToListAsync();
                query = query.Where(p => propertyIds.Contains(p.Id));
            }

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var properties = await query
                .Include(p => p.TransactionType)
                .Include(p => p.Company)
                .ToListAsync();

            var byCompany = properties
                .Where(p => p.CompanyId.HasValue)
                .GroupBy(p => new { p.CompanyId, CompanyTitle = p.Company?.Title ?? "نامشخص" })
                .Select(g =>
                {
                    var items = g.ToList();
                    var byType = items
                        .GroupBy(p => new { Id = p.TransactionTypeId ?? 0, Name = p.TransactionType?.Name ?? "نامشخص" })
                        .Select(tg => new
                        {
                            TransactionTypeId = tg.Key.Id,
                            TransactionTypeName = tg.Key.Name,
                            TransactionTypeDari = GetTransactionTypeDari(tg.Key.Name),
                            Count = tg.Count(),
                            TotalPrice = tg.ToList().Sum(p => TryParseDecimal(p.Price)),
                            TotalRoyaltyAmount = tg.ToList().Sum(p => TryParseDecimal(p.RoyaltyAmount))
                        })
                        .OrderByDescending(t => t.Count)
                        .ToList();

                    return new
                    {
                        CompanyId = g.Key.CompanyId,
                        CompanyTitle = g.Key.CompanyTitle,
                        TotalRecords = items.Count,
                        TotalPrice = items.Sum(p => TryParseDecimal(p.Price)),
                        TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(p.RoyaltyAmount)),
                        ByTransactionType = byType
                    };
                })
                .OrderByDescending(c => c.TotalRecords)
                .ToList();

            return Ok(byCompany);
        }

        /// <summary>
        /// Report grouped by province
        /// </summary>
        [HttpGet]
        [Route("ByProvince")]
        public async Task<IActionResult> GetByProvince(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null)
        {
            TryParseDates(startDate, endDate, calendarType, out var parsedStart, out var parsedEnd);

            var query = await GetFilteredQuery();

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var properties = await query.ToListAsync();

            // Get addresses for all these properties
            var propertyIds = properties.Select(p => p.Id).ToList();
            var addresses = await _context.PropertyAddresses.AsNoTracking()
                .Where(a => propertyIds.Contains(a.PropertyDetailsId ?? 0))
                .ToListAsync();

            var provinces = await _context.Locations.AsNoTracking()
                .Where(l => l.ParentId == null && l.IsActive == 1)
                .ToListAsync();

            var result = provinces.Select(prov =>
            {
                var propIdsInProvince = addresses.Where(a => a.ProvinceId == prov.Id).Select(a => a.PropertyDetailsId ?? 0).ToHashSet();
                var propsInProvince = properties.Where(p => propIdsInProvince.Contains(p.Id)).ToList();

                if (propsInProvince.Count == 0) return null;

                return new
                {
                    ProvinceId = prov.Id,
                    ProvinceName = prov.Name ?? prov.Dari,
                    ProvinceDari = prov.Dari,
                    TotalRecords = propsInProvince.Count,
                    TotalPrice = propsInProvince.Sum(p => TryParseDecimal(p.Price)),
                    TotalRoyaltyAmount = propsInProvince.Sum(p => TryParseDecimal(p.RoyaltyAmount))
                };
            })
            .Where(r => r != null)
            .OrderByDescending(r => r!.TotalRecords)
            .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Monthly trend of property registrations
        /// </summary>
        [HttpGet]
        [Route("MonthlyTrend")]
        public async Task<IActionResult> GetMonthlyTrend(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? companyId = null)
        {
            TryParseDates(startDate, endDate, calendarType, out var parsedStart, out var parsedEnd);

            var query = await GetFilteredQuery();

            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var properties = await query.Include(p => p.TransactionType).ToListAsync();

            var trend = properties
                .Where(p => p.CreatedAt.HasValue)
                .GroupBy(p => new DateTime(p.CreatedAt!.Value.Year, p.CreatedAt.Value.Month, 1))
                .Select(g =>
                {
                    var items = g.ToList();
                    var byType = items
                        .GroupBy(p => p.TransactionType?.Name ?? "نامشخص")
                        .Select(tg => new
                        {
                            TransactionType = tg.Key,
                            TransactionTypeDari = GetTransactionTypeDari(tg.Key),
                            Count = tg.Count()
                        })
                        .OrderByDescending(t => t.Count)
                        .ToList();

                    return new
                    {
                        Month = g.Key.ToString("yyyy-MM"),
                        MonthLabel = g.Key.ToString("MMM yyyy"),
                        TotalRecords = items.Count,
                        TotalPrice = items.Sum(p => TryParseDecimal(p.Price)),
                        TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(p.RoyaltyAmount)),
                        ByTransactionType = byType
                    };
                })
                .OrderBy(t => t.Month)
                .ToList();

            return Ok(trend);
        }

        /// <summary>
        /// Detailed list for a specific transaction type
        /// </summary>
        [HttpGet]
        [Route("ByTransactionType/{transactionTypeId}")]
        public async Task<IActionResult> GetByTransactionType(int transactionTypeId,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? calendarType = null,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            TryParseDates(startDate, endDate, calendarType, out var parsedStart, out var parsedEnd);

            var query = await GetFilteredQuery();

            query = query.Where(p => p.TransactionTypeId == transactionTypeId);

            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);

            if (provinceId.HasValue || districtId.HasValue)
            {
                var addressQuery = _context.PropertyAddresses.AsNoTracking();
                if (provinceId.HasValue) addressQuery = addressQuery.Where(a => a.ProvinceId == provinceId.Value);
                if (districtId.HasValue) addressQuery = addressQuery.Where(a => a.DistrictId == districtId.Value);
                var propertyIds = await addressQuery.Select(a => a.PropertyDetailsId).Distinct().ToListAsync();
                query = query.Where(p => propertyIds.Contains(p.Id));
            }

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var total = await query.CountAsync();

            var items = await query
                .Include(p => p.Company)
                .Include(p => p.TransactionType)
                .Include(p => p.SellerDetails)
                .Include(p => p.BuyerDetails)
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var latestUpdatedByByProperty = await GetLatestUpdatedByByProperty(items.Select(p => p.Id).ToList());
            var userLookup = await BuildUserDisplayLookup(
                items.Select(p => latestUpdatedByByProperty.TryGetValue(p.Id, out var updatedBy) ? updatedBy : p.CreatedBy));

            var result = items.Select(p => new
            {
                p.Id,
                p.Pnumber,
                TransactionType = p.TransactionType?.Name,
                TransactionTypeDari = GetTransactionTypeDari(p.TransactionType?.Name),
                p.Price,
                p.RoyaltyAmount,
                CompanyTitle = p.Company?.Title,
                p.CompanyId,
                p.CreatedAt,
                SellerName = p.SellerDetails.FirstOrDefault()?.FirstName,
                BuyerName = p.BuyerDetails.FirstOrDefault()?.FirstName,
                CreatedBy = latestUpdatedByByProperty.TryGetValue(p.Id, out var updatedByValue)
                    && !string.IsNullOrWhiteSpace(updatedByValue)
                    && userLookup.TryGetValue(updatedByValue, out var updatedByName)
                        ? updatedByName
                        : (!string.IsNullOrWhiteSpace(p.CreatedBy) && userLookup.TryGetValue(p.CreatedBy, out var createdByName)
                            ? createdByName
                            : "-"),
                p.iscomplete
            }).ToList();

            var transactionType = await _context.TransactionTypes.FindAsync(transactionTypeId);

            return Ok(new
            {
                TransactionTypeId = transactionTypeId,
                TransactionTypeName = transactionType?.Name,
                TransactionTypeDari = GetTransactionTypeDari(transactionType?.Name),
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPrice = items.Sum(p => TryParseDecimal(p.Price)),
                TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(p.RoyaltyAmount)),
                Records = result
            });
        }

        /// <summary>
        /// Helper: safely parse decimal from string
        /// </summary>
        private static decimal TryParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            // Remove commas and spaces
            var cleaned = value.Replace(",", "").Replace(" ", "").Trim();
            return decimal.TryParse(cleaned, out var result) ? result : 0;
        }
    }
}
