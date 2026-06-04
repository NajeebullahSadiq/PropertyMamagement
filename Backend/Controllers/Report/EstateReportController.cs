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

        private static readonly string[] HijriShamsiMonthNames =
        {
            "\u062d\u0645\u0644",
            "\u062b\u0648\u0631",
            "\u062c\u0648\u0632\u0627",
            "\u0633\u0631\u0637\u0627\u0646",
            "\u0627\u0633\u062f",
            "\u0633\u0646\u0628\u0644\u0647",
            "\u0645\u06cc\u0632\u0627\u0646",
            "\u0639\u0642\u0631\u0628",
            "\u0642\u0648\u0633",
            "\u062c\u062f\u06cc",
            "\u062f\u0644\u0648",
            "\u062d\u0648\u062a"
        };

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

        private static BuyerDetail? GetPrimaryBuyer(PropertyDetail property)
        {
            return property.BuyerDetails.FirstOrDefault();
        }

        private static string? GetEffectiveTransactionTypeName(PropertyDetail property)
        {
            var buyerTransactionType = GetPrimaryBuyer(property)?.TransactionType;
            return !string.IsNullOrWhiteSpace(buyerTransactionType)
                ? buyerTransactionType
                : property.TransactionType?.Name;
        }

        private static int GetEffectiveTransactionTypeId(PropertyDetail property, Dictionary<string, int> transactionTypeIdsByName)
        {
            if (property.TransactionTypeId.HasValue)
            {
                return property.TransactionTypeId.Value;
            }

            var transactionTypeName = GetEffectiveTransactionTypeName(property);
            return !string.IsNullOrWhiteSpace(transactionTypeName)
                && transactionTypeIdsByName.TryGetValue(transactionTypeName, out var transactionTypeId)
                    ? transactionTypeId
                    : 0;
        }

        private static string? GetEffectivePrice(PropertyDetail property)
        {
            var buyerPrice = GetPrimaryBuyer(property)?.Price;
            return !string.IsNullOrWhiteSpace(buyerPrice) ? buyerPrice : property.Price;
        }

        private static string? GetEffectiveRoyaltyAmount(PropertyDetail property)
        {
            var buyerRoyaltyAmount = GetPrimaryBuyer(property)?.RoyaltyAmount;
            return !string.IsNullOrWhiteSpace(buyerRoyaltyAmount) ? buyerRoyaltyAmount : property.RoyaltyAmount;
        }

        private static string GetEffectivePropertyTypeName(PropertyDetail property)
        {
            if (property.PropertyType != null
                && property.PropertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(property.CustomPropertyType))
            {
                return property.CustomPropertyType;
            }

            return property.PropertyType?.Dari
                ?? property.PropertyType?.Name
                ?? "Ù†Ø§Ù…Ø´Ø®Øµ";
        }

        private static (int Year, int Month, string Key, string Label) GetHijriShamsiMonth(DateTime gregorianDate)
        {
            var (year, month, _) = DateConversionHelper.FromGregorian(gregorianDate, CalendarType.HijriShamsi);
            var monthName = month >= 1 && month <= 12 ? HijriShamsiMonthNames[month - 1] : month.ToString("D2");
            return (year, month, $"{year:D4}-{month:D2}", $"{monthName} {year:D4}");
        }

        private async Task<Dictionary<string, int>> GetTransactionTypeIdsByName()
        {
            return await _context.TransactionTypes
                .AsNoTracking()
                .ToDictionaryAsync(t => t.Name, t => t.Id, StringComparer.OrdinalIgnoreCase);
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

            if (RbacHelper.HasAnyRole(roles, UserRoles.Admin, UserRoles.Authority))
            {
                query = _context.PropertyDetails.AsNoTracking();
            }
            else if (user.CompanyId > 0)
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
                .Include(p => p.PropertyType)
                .Include(p => p.BuyerDetails)
                .Include(p => p.Company)
                .AsSplitQuery()
                .ToListAsync();

            var totalRecords = properties.Count;
            var totalRecordsComplete = properties.Count(p => p.iscomplete == true);
            var transactionTypeIdsByName = await GetTransactionTypeIdsByName();

            // Group by transaction type
            var byTransactionType = properties
                .GroupBy(p => new
                {
                    Id = GetEffectiveTransactionTypeId(p, transactionTypeIdsByName),
                    Name = GetEffectiveTransactionTypeName(p) ?? "نامشخص"
                })
                .Select(g =>
                {
                    var items = g.ToList();
                    var totalPrice = items.Sum(p => TryParseDecimal(GetEffectivePrice(p)));
                    var totalRoyalty = items.Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p)));

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
            var grandTotalPrice = properties.Sum(p => TryParseDecimal(GetEffectivePrice(p)));
            var grandTotalRoyalty = properties.Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p)));

            // By property type
            var byPropertyType = properties
                .Where(p => p.PropertyTypeId.HasValue)
                .GroupBy(p => new { Id = p.PropertyTypeId!.Value, Name = GetEffectivePropertyTypeName(p) })
                .Select(g => new
                {
                    PropertyTypeId = g.Key.Id,
                    PropertyTypeName = g.Key.Name,
                    Count = g.Count(),
                    TotalPrice = g.ToList().Sum(p => TryParseDecimal(GetEffectivePrice(p))),
                    TotalRoyaltyAmount = g.ToList().Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p)))
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
                .Include(p => p.BuyerDetails)
                .Include(p => p.Company)
                .AsSplitQuery()
                .ToListAsync();
            var transactionTypeIdsByName = await GetTransactionTypeIdsByName();

            var byCompany = properties
                .Where(p => p.CompanyId.HasValue)
                .GroupBy(p => new { p.CompanyId, CompanyTitle = p.Company?.Title ?? "نامشخص" })
                .Select(g =>
                {
                    var items = g.ToList();
                    var byType = items
                        .GroupBy(p => new
                        {
                            Id = GetEffectiveTransactionTypeId(p, transactionTypeIdsByName),
                            Name = GetEffectiveTransactionTypeName(p) ?? "نامشخص"
                        })
                        .Select(tg => new
                        {
                            TransactionTypeId = tg.Key.Id,
                            TransactionTypeName = tg.Key.Name,
                            TransactionTypeDari = GetTransactionTypeDari(tg.Key.Name),
                            Count = tg.Count(),
                            TotalPrice = tg.ToList().Sum(p => TryParseDecimal(GetEffectivePrice(p))),
                            TotalRoyaltyAmount = tg.ToList().Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p)))
                        })
                        .OrderByDescending(t => t.Count)
                        .ToList();

                    return new
                    {
                        CompanyId = g.Key.CompanyId,
                        CompanyTitle = g.Key.CompanyTitle,
                        TotalRecords = items.Count,
                        TotalPrice = items.Sum(p => TryParseDecimal(GetEffectivePrice(p))),
                        TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p))),
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

            var properties = await query
                .Include(p => p.BuyerDetails)
                .AsSplitQuery()
                .ToListAsync();

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
                    TotalPrice = propsInProvince.Sum(p => TryParseDecimal(GetEffectivePrice(p))),
                    TotalRoyaltyAmount = propsInProvince.Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p)))
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

            var properties = await query
                .Include(p => p.TransactionType)
                .Include(p => p.BuyerDetails)
                .AsSplitQuery()
                .ToListAsync();

            var trend = properties
                .Where(p => p.CreatedAt.HasValue)
                .GroupBy(p => GetHijriShamsiMonth(p.CreatedAt!.Value))
                .Select(g =>
                {
                    var items = g.ToList();
                    var byType = items
                        .GroupBy(p => GetEffectiveTransactionTypeName(p) ?? "نامشخص")
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
                        Month = g.Key.Key,
                        MonthLabel = g.Key.Label,
                        TotalRecords = items.Count,
                        TotalPrice = items.Sum(p => TryParseDecimal(GetEffectivePrice(p))),
                        TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p))),
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
            var transactionType = await _context.TransactionTypes.FindAsync(transactionTypeId);

            if (transactionType != null)
            {
                query = query.Where(p =>
                    p.BuyerDetails.Any(b => b.TransactionType == transactionType.Name)
                    || (!p.BuyerDetails.Any(b => b.TransactionType != null && b.TransactionType != "")
                        && p.TransactionTypeId == transactionTypeId));
            }
            else
            {
                query = query.Where(p => p.TransactionTypeId == transactionTypeId);
            }

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
                .AsSplitQuery()
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
                TransactionType = GetEffectiveTransactionTypeName(p),
                TransactionTypeDari = GetTransactionTypeDari(GetEffectiveTransactionTypeName(p)),
                Price = GetEffectivePrice(p),
                RoyaltyAmount = GetEffectiveRoyaltyAmount(p),
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

            return Ok(new
            {
                TransactionTypeId = transactionTypeId,
                TransactionTypeName = transactionType?.Name,
                TransactionTypeDari = GetTransactionTypeDari(transactionType?.Name),
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPrice = items.Sum(p => TryParseDecimal(GetEffectivePrice(p))),
                TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(GetEffectiveRoyaltyAmount(p))),
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
