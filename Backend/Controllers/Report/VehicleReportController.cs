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
    public class VehicleReportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehicleReportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

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

        private async Task<IQueryable<VehiclesPropertyDetail>> GetFilteredQuery()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new UnauthorizedAccessException();

            var roles = await _userManager.GetRolesAsync(user);

            if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "vehicle"))
                throw new UnauthorizedAccessException();

            IQueryable<VehiclesPropertyDetail> query;

            if (RbacHelper.CanViewAllRecords(roles, "vehicle"))
            {
                query = _context.VehiclesPropertyDetails.AsNoTracking();
            }
            else if (RbacHelper.ShouldFilterByCompany(roles, "vehicle"))
            {
                query = _context.VehiclesPropertyDetails.AsNoTracking()
                    .Where(p => p.CompanyId == user.CompanyId);
            }
            else
            {
                query = _context.VehiclesPropertyDetails.AsNoTracking()
                    .Where(p => p.CreatedBy == userId);
            }

            return query;
        }

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

        private static decimal TryParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            var cleaned = value.Replace(",", "").Replace(" ", "").Trim();
            return decimal.TryParse(cleaned, out var result) ? result : 0;
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

            // Province/district filter via seller address
            if (provinceId.HasValue || districtId.HasValue)
            {
                var sellerQuery = _context.VehiclesSellerDetails.AsNoTracking();
                if (provinceId.HasValue)
                    sellerQuery = sellerQuery.Where(s => s.PaddressProvinceId == provinceId.Value);
                if (districtId.HasValue)
                    sellerQuery = sellerQuery.Where(s => s.PaddressDistrictId == districtId.Value);

                var vehicleIds = await sellerQuery.Select(s => s.PropertyDetailsId).Distinct().ToListAsync();
                query = query.Where(p => vehicleIds.Contains(p.Id));
            }

            if (companyId.HasValue)
                query = query.Where(p => p.CompanyId == companyId.Value);

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var vehicles = await query
                .Include(p => p.TransactionType)
                .ToListAsync();

            var totalRecords = vehicles.Count;
            var totalRecordsComplete = vehicles.Count(p => p.iscomplete == true);

            // Get company titles for vehicles that have CompanyId
            var companyIds = vehicles.Where(p => p.CompanyId.HasValue).Select(p => p.CompanyId!.Value).Distinct().ToList();
            var companies = await _context.CompanyDetails.AsNoTracking()
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Title ?? "نامشخص");

            // Group by transaction type
            var byTransactionType = vehicles
                .GroupBy(p => new { Id = p.TransactionTypeId ?? 0, Name = p.TransactionType?.Name ?? "نامشخص" })
                .Select(g =>
                {
                    var items = g.ToList();
                    return new
                    {
                        TransactionTypeId = g.Key.Id,
                        TransactionTypeName = g.Key.Name,
                        TransactionTypeDari = GetTransactionTypeDari(g.Key.Name),
                        Count = items.Count,
                        CompleteCount = items.Count(p => p.iscomplete == true),
                        TotalPrice = items.Sum(p => TryParseDecimal(p.Price)),
                        TotalRoyaltyAmount = items.Sum(p => TryParseDecimal(p.RoyaltyAmount))
                    };
                })
                .OrderByDescending(t => t.Count)
                .ToList();

            var grandTotalPrice = vehicles.Sum(p => TryParseDecimal(p.Price));
            var grandTotalRoyalty = vehicles.Sum(p => TryParseDecimal(p.RoyaltyAmount));

            // By vehicle type
            var byVehicleType = vehicles
                .Where(p => !string.IsNullOrWhiteSpace(p.TypeOfVehicle))
                .GroupBy(p => p.TypeOfVehicle!)
                .Select(g => new
                {
                    VehicleType = g.Key,
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
                ByVehicleType = byVehicleType
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
                var sellerQuery = _context.VehiclesSellerDetails.AsNoTracking();
                if (provinceId.HasValue) sellerQuery = sellerQuery.Where(s => s.PaddressProvinceId == provinceId.Value);
                if (districtId.HasValue) sellerQuery = sellerQuery.Where(s => s.PaddressDistrictId == districtId.Value);
                var vehicleIds = await sellerQuery.Select(s => s.PropertyDetailsId).Distinct().ToListAsync();
                query = query.Where(p => vehicleIds.Contains(p.Id));
            }

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var vehicles = await query
                .Include(p => p.TransactionType)
                .ToListAsync();

            var companyIds = vehicles.Where(p => p.CompanyId.HasValue).Select(p => p.CompanyId!.Value).Distinct().ToList();
            var companies = await _context.CompanyDetails.AsNoTracking()
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Title ?? "نامشخص");

            var byCompany = vehicles
                .Where(p => p.CompanyId.HasValue)
                .GroupBy(p => p.CompanyId!.Value)
                .Select(g =>
                {
                    var items = g.ToList();
                    var companyTitle = companies.GetValueOrDefault(g.Key, "نامشخص");
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
                        CompanyId = g.Key,
                        CompanyTitle = companyTitle,
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
        /// Report grouped by province (from seller address)
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

            var vehicles = await query.ToListAsync();

            // Get seller details with province info
            var vehicleIds = vehicles.Select(p => p.Id).ToList();
            var sellers = await _context.VehiclesSellerDetails.AsNoTracking()
                .Where(s => s.PropertyDetailsId.HasValue && vehicleIds.Contains(s.PropertyDetailsId.Value))
                .ToListAsync();

            var provinces = await _context.Locations.AsNoTracking()
                .Where(l => l.ParentId == null && l.IsActive == 1)
                .ToListAsync();

            var result = provinces.Select(prov =>
            {
                var vehicleIdsInProvince = sellers.Where(s => s.PaddressProvinceId == prov.Id && s.PropertyDetailsId.HasValue).Select(s => s.PropertyDetailsId!.Value).ToHashSet();
                var vehiclesInProvince = vehicles.Where(p => vehicleIdsInProvince.Contains(p.Id)).ToList();

                if (vehiclesInProvince.Count == 0) return null;

                return new
                {
                    ProvinceId = prov.Id,
                    ProvinceName = prov.Name ?? prov.Dari,
                    ProvinceDari = prov.Dari,
                    TotalRecords = vehiclesInProvince.Count,
                    TotalPrice = vehiclesInProvince.Sum(p => TryParseDecimal(p.Price)),
                    TotalRoyaltyAmount = vehiclesInProvince.Sum(p => TryParseDecimal(p.RoyaltyAmount))
                };
            })
            .Where(r => r != null)
            .OrderByDescending(r => r!.TotalRecords)
            .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Monthly trend of vehicle registrations
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

            var vehicles = await query.Include(p => p.TransactionType).ToListAsync();

            var trend = vehicles
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
                var sellerQuery = _context.VehiclesSellerDetails.AsNoTracking();
                if (provinceId.HasValue) sellerQuery = sellerQuery.Where(s => s.PaddressProvinceId == provinceId.Value);
                if (districtId.HasValue) sellerQuery = sellerQuery.Where(s => s.PaddressDistrictId == districtId.Value);
                var vehicleIds = await sellerQuery.Select(s => s.PropertyDetailsId).Distinct().ToListAsync();
                query = query.Where(p => vehicleIds.Contains(p.Id));
            }

            if (parsedStart.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= parsedStart.Value.ToDateTime(TimeOnly.MinValue).Date);
            if (parsedEnd.HasValue)
                query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= parsedEnd.Value.ToDateTime(TimeOnly.MinValue).Date);

            var total = await query.CountAsync();

            var companyIds = await query.Where(p => p.CompanyId.HasValue).Select(p => p.CompanyId!.Value).Distinct().ToListAsync();
            var companies = await _context.CompanyDetails.AsNoTracking()
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Title ?? "نامشخص");

            var items = await query
                .Include(p => p.TransactionType)
                .Include(p => p.VehiclesSellerDetails)
                .Include(p => p.VehiclesBuyerDetails)
                .AsSplitQuery()
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = items.Select(p => new
            {
                p.Id,
                p.PermitNo,
                p.PilateNo,
                p.TypeOfVehicle,
                p.Model,
                TransactionType = p.TransactionType?.Name,
                TransactionTypeDari = GetTransactionTypeDari(p.TransactionType?.Name),
                p.Price,
                p.RoyaltyAmount,
                CompanyTitle = p.CompanyId.HasValue ? companies.GetValueOrDefault(p.CompanyId.Value, "نامشخص") : null,
                p.CompanyId,
                p.CreatedAt,
                SellerName = p.VehiclesSellerDetails.FirstOrDefault()?.FirstName,
                BuyerName = p.VehiclesBuyerDetails.FirstOrDefault()?.FirstName,
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
    }
}
