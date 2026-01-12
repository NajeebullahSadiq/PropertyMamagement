using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;

namespace WebAPIBackend.Controllers.Report
{
    [Authorize(Roles = "ADMIN,AUTHORITY")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public DashboardController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private static bool TryParseStartEndDates(string startDate, string endDate, string? calendarType, out DateTime start, out DateTime end)
        {
            start = default;
            end = default;

            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
            {
                return false;
            }

            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            
            if (!DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var startDateOnly) ||
                !DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var endDateOnly))
            {
                return false;
            }

            start = startDateOnly.ToDateTime(TimeOnly.MinValue);
            end = endDateOnly.ToDateTime(TimeOnly.MinValue);

            if (start > end)
            {
                var tmp = start;
                start = end;
                end = tmp;
            }

            return true;
        }


        [HttpGet]
        [Route("GetCompanyDashboardData")]
        public IActionResult GetCompanyDashboardData()
        {


            var query = _context.CompanyDetails;
          //  .Where(b => b.Price.HasValue && b.PropertyTypeId.HasValue);
            var results = new
            {
                TotalCompanyRegisterd = new
                {
                    TotalTransaction = query.Count(),
                }

            };

            return Ok(results);
        }



        [HttpGet]
        [Route("GetExpireLicenseDashboardData")]
        public IActionResult GetExpireLicenseDashboardData()
        {


            var query = _context.LicenseDetails
            .Where(b => b.ExpireDate < DateOnly.FromDateTime(DateTime.Now));
            var results = new
            {
                TotalLicenseExpired = new
                {
                    TotalTransaction = query.Count(),
                }

            };

            return Ok(results);
        }

        [HttpGet]
        [Route("GetVehicleDashboardData")]
        public IActionResult GetVehicleDashboardData()
        {
            
                var query = _context.VehiclesPropertyDetails
                .Where(b=>b.Price.HasValue);
            var results = new
            {
                TotalRecord = new
                {
                    TotalAmount = query
                .Sum(b => b.Price),
                    TotalAmountNotCompleted = query.Where(b => b.iscomplete == false)
                .Sum(b => b.Price),
                    TotalAmountCompleted = query.Where(b => b.iscomplete == true)
                .Sum(b => b.Price),
                    TotalTransactionCompleted = query.Count(b => b.iscomplete == true),
                    TotalTransactionNotCompleted = query.Count(b => b.iscomplete == false),
                    TotalTransaction = query.Count(),
                    TotalRoyaltyAmount = query
                    .Sum(b => b.RoyaltyAmount),
                    TotalRoyaltyAmountNotCompleted = query.Where(b => b.iscomplete == false)
                    .Sum(b => b.RoyaltyAmount),
                    TotalRoyaltyAmountCompleted = query.Where(b => b.iscomplete == true)
                    .Sum(b => b.RoyaltyAmount),
                }

            };

            return Ok(results);
        }

        [HttpGet]
        [Route("GetDashboardDataByDate")]
        public IActionResult GetDashboardDataByDate(string startDate, string endDate, string? calendarType = null)
        {
            if (!TryParseStartEndDates(startDate, endDate, calendarType, out var gregoriansDate, out var gregorianeDate))
            {
                return BadRequest("Invalid date range");
            }

            var vquery = _context.VehiclesPropertyDetails
               .Where(b => b.Price.HasValue &&
                           b.CreatedAt.HasValue &&
                           b.CreatedAt.Value.Date >= gregoriansDate.Date &&
                           b.CreatedAt.Value.Date <= gregorianeDate.Date);

            var pquery = _context.PropertyDetails
                .Where(b => b.Price.HasValue &&
                            b.CreatedAt.HasValue &&
                            b.CreatedAt.Value.Date >= gregoriansDate.Date &&
                            b.CreatedAt.Value.Date <= gregorianeDate.Date);

            var results = new
            {
                TotalRecord = new
                {
                    // VehiclesPropertyDetails Data
                    TotalAmount = vquery.Sum(b => b.Price),
                    TotalAmountNotCompleted = vquery.Where(b => b.iscomplete==false).Sum(b => b.Price),
                    TotalAmountCompleted = vquery.Where(b => b.iscomplete==true).Sum(b => b.Price),
                    TotalTransactionCompleted = vquery.Count(b => b.iscomplete==true),
                    TotalTransactionNotCompleted = vquery.Count(b =>b.iscomplete==false),
                    TotalTransaction = vquery.Count(),
                    TotalRoyaltyAmount = vquery.Sum(b => b.RoyaltyAmount),
                    TotalRoyaltyAmountNotCompleted = vquery.Where(b => b.iscomplete==false).Sum(b => b.RoyaltyAmount),
                    TotalRoyaltyAmountCompleted = vquery.Where(b => b.iscomplete==true).Sum(b => b.RoyaltyAmount),

                    // PropertyDetails Data
                    TotalAmountProperty = pquery.Sum(b => b.Price),
                    TotalAmountPropertyNotCompleted = pquery.Where(b => b.iscomplete==false).Sum(b => b.Price),
                    TotalAmountPropertyCompleted = pquery.Where(b => b.iscomplete==true).Sum(b => b.Price),
                    TotalPropertyTransactionCompleted = pquery.Count(b => b.iscomplete==true),
                    TotalPropertyTransactionNotCompleted = pquery.Count(b => b.iscomplete==false),
                    TotalPropertyTransaction = pquery.Count(),
                    TotalPropertyRoyaltyAmount = pquery.Sum(b => b.RoyaltyAmount),
                    TotalPropertyRoyaltyAmountNotCompleted = pquery.Where(b => b.iscomplete==false).Sum(b => b.RoyaltyAmount),
                    TotalPropertyRoyaltyAmountCompleted = pquery.Where(b => b.iscomplete==true).Sum(b => b.RoyaltyAmount)
                }
            };

            return Ok(results);
        }
      

        [HttpGet]
        [Route("GetEstateDashboardData")]
        public IActionResult GetDashboardData()
        {

            var query = _context.PropertyDetails
                .Where(b => b.PropertyTypeId.HasValue && b.Price.HasValue);

            var results = new
            {
                TransactionDataByTypeTotal = query
                    // .Where(b => b.iscomplete == true)
                    .Join(
                        _context.PropertyTypes,
                        b => b.PropertyTypeId,
                        z => z.Id,
                        (b, z) => new {
                            Name = z.Name,
                            Amount = b.Price ?? 0
                        }
                    )
                    .GroupBy(b => b.Name)
                    .Select(g => new {
                        Name = g.Key,
                        Amount = g.Sum(b => b.Amount)
                    })
                    .ToList(),

                TransactionDataByTypeCompleted = query
                  .Where(b => b.iscomplete == true)
                    .Join(
                        _context.PropertyTypes,
                        b => b.PropertyTypeId,
                        z => z.Id,
                        (b, z) => new {
                            Name = z.Name,
                            Amount = b.Price ?? 0
                        }
                    )
                    .GroupBy(b => b.Name)
                    .Select(g => new {
                        Name = g.Key,
                        Amount = g.Sum(b => b.Amount)
                    })
                    .ToList(),

                TransactionDataByTypeNotCompleted = query
                  .Where(b => b.iscomplete == false)
                    .Join(
                        _context.PropertyTypes,
                        b => b.PropertyTypeId,
                        z => z.Id,
                        (b, z) => new {
                            Name = z.Name,
                            Amount = b.Price ?? 0
                        }
                    )
                    .GroupBy(b => b.Name)
                    .Select(g => new {
                        Name = g.Key,
                        Amount = g.Sum(b => b.Amount)
                    })
                    .ToList(),
                TransactionDataByTransactionTypeTotal = query
                    // .Where(b => b.iscomplete == false)
                    .Join(
                        _context.TransactionTypes,
                        b => b.TransactionTypeId,
                        z => z.Id,
                        (b, z) => new {
                            Name = z.Name,
                            Amount = b.Price ?? 0
                        }
                    )
                    .GroupBy(b => b.Name)
                    .Select(g => new {
                        Name = g.Key,
                        Amount = g.Sum(b => b.Amount)
                    })
                    .ToList(),
                TransactionDataByTransactionTypeNotCompleted = query
                     .Where(b => b.iscomplete == false)
                    .Join(
                        _context.TransactionTypes,
                        b => b.TransactionTypeId,
                        z => z.Id,
                        (b, z) => new {
                            Name = z.Name,
                            Amount = b.Price ?? 0
                        }
                    )
                    .GroupBy(b => b.Name)
                    .Select(g => new {
                        Name = g.Key,
                        Amount = g.Sum(b => b.Amount)
                    })
                    .ToList(),
                TransactionDataByTransactionTypeCompleted = query
                     .Where(b => b.iscomplete == true)
                    .Join(
                        _context.TransactionTypes,
                        b => b.TransactionTypeId,
                        z => z.Id,
                        (b, z) => new {
                            Name = z.Name,
                            Amount = b.Price ?? 0
                        }
                    )
                    .GroupBy(b => b.Name)
                    .Select(g => new {
                        Name = g.Key,
                        Amount = g.Sum(b => b.Amount)
                    })
                    .ToList(),

                TotalRecord = new
                {
                    TotalAmount = query
                    .Sum(b => b.Price),
                    TotalAmountNotCompleted = query.Where(b => b.iscomplete == false)
                    .Sum(b => b.Price),
                    TotalAmountCompleted = query.Where(b => b.iscomplete == true)
                    .Sum(b => b.Price),
                    TotalRoyaltyAmount = query
                    .Sum(b => b.RoyaltyAmount),
                    TotalRoyaltyAmountNotCompleted = query.Where(b => b.iscomplete == false)
                    .Sum(b => b.RoyaltyAmount),
                    TotalRoyaltyAmountCompleted = query.Where(b => b.iscomplete == true)
                    .Sum(b => b.RoyaltyAmount),
                    TotalTransactionCompleted = query.Count(b => b.iscomplete == true),
                    TotalTransactionNotCompleted = query.Count(b => b.iscomplete == false),
                    TotalTransaction = query.Count(),
                }
            };

            return Ok(results);
        }

        [HttpGet]
        [Route("GetTopUsersSummary")]
        public IActionResult GetTopUsersSummary()
        {
            var topUsersSummary = _context.PropertyDetails
                .Where(b => b.PropertyTypeId.HasValue && b.Price.HasValue)
                .GroupBy(b => b.CreatedBy)
                .Select(g => new
                {
                    CreatedBy = g.Key,
                    TotalPropertiesCreated = g.Count(),
                    TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderByDescending(u => u.TotalPriceOfProperties)
               // .Take(10)
                .ToList();

            var topUsersWithNamesAndCompany = topUsersSummary
                .Join(
                    _userManager.Users,
                    topUser => topUser.CreatedBy,
                    user => user.Id,
                    (topUser, user) => new
                    {
                        CreatedBy = $"{user.FirstName} {user.LastName}",
                        topUser.TotalPropertiesCreated,
                        topUser.TotalPriceOfProperties,
                        user.CompanyId // Assuming you have the CompanyId property in ApplicationUser
            }
                )
                .Join(
                    _context.CompanyDetails,
                    user => user.CompanyId,
                    company => company.Id,
                    (user, company) => new
                    {
                        user.CreatedBy,
                        user.TotalPropertiesCreated,
                        user.TotalPriceOfProperties,
                        CompanyTitle = company.Title // Assuming you have the CompanyTitle property in CompanyDetails
            }
                )
                .ToList();

            var totalPriceSummary = new
            {
                TotalProperties = topUsersSummary.Sum(u => u.TotalPropertiesCreated),
                TotalPrice = topUsersSummary.Sum(u => u.TotalPriceOfProperties),
                TopUsers = topUsersWithNamesAndCompany
            };

            return Ok(totalPriceSummary);
        }
        [HttpGet]
        [Route("GetVehicleTopUsersSummary")]
        public IActionResult GetVehicleTopUsersSummary()
        {
            var topUsersSummary = _context.VehiclesPropertyDetails
                .Where( b=> b.Price.HasValue)
                .GroupBy(b => b.CreatedBy)
                .Select(g => new
                {
                    CreatedBy = g.Key,
                    TotalPropertiesCreated = g.Count(),
                    TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderByDescending(u => u.TotalPriceOfProperties)
                // .Take(10)
                .ToList();

            var topUsersWithNamesAndCompany = topUsersSummary
                .Join(
                    _userManager.Users,
                    topUser => topUser.CreatedBy,
                    user => user.Id,
                    (topUser, user) => new
                    {
                        CreatedBy = $"{user.FirstName} {user.LastName}",
                        topUser.TotalPropertiesCreated,
                        topUser.TotalPriceOfProperties,
                        user.CompanyId // Assuming you have the CompanyId property in ApplicationUser
                    }
                )
                .Join(
                    _context.CompanyDetails,
                    user => user.CompanyId,
                    company => company.Id,
                    (user, company) => new
                    {
                        user.CreatedBy,
                        user.TotalPropertiesCreated,
                        user.TotalPriceOfProperties,
                        CompanyTitle = company.Title // Assuming you have the CompanyTitle property in CompanyDetails
                    }
                )
                .ToList();

            var totalPriceSummary = new
            {
                TotalProperties = topUsersSummary.Sum(u => u.TotalPropertiesCreated),
                TotalPrice = topUsersSummary.Sum(u => u.TotalPriceOfProperties),
                TopUsers = topUsersWithNamesAndCompany
            };

            return Ok(totalPriceSummary);
        }

        [HttpGet]
        [Route("GetPropertyTypesByMonth")]
        public IActionResult GetPropertyTypesByMonth()
        {
            var propertyTypesByMonth = _context.PropertyDetails
                .Where(b => b.PropertyTypeId.HasValue && b.Price.HasValue && b.CreatedAt.HasValue) // Ensure CreatedAt has a value
                .GroupBy(b => new
                {
                    PropertyTypeId = b.PropertyTypeId.Value,
                    Month = new DateTime(b.CreatedAt.Value.Year, b.CreatedAt.Value.Month, 1)
                })
                .Select(g => new
                {
                    PropertyTypeId = g.Key.PropertyTypeId,
                    Month = g.Key.Month,
                  //  TotalPropertiesCreated = g.Count(),
                    TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderBy(g => g.Month)
                .ToList();

            var propertyTypes = _context.PropertyTypes.ToDictionary(pt => pt.Id, pt => pt.Name);

            var graphData = propertyTypesByMonth
                .GroupBy(p => p.PropertyTypeId)
                .Select(g => new
                {
                    PropertyType = propertyTypes.ContainsKey(g.Key) ? propertyTypes[g.Key] : "Unknown",
                    Data = g.Select(p => new
                    {
                        Month = p.Month.ToString("MMM yyyy"),
                       // TotalPropertiesCreated = p.TotalPropertiesCreated,
                        TotalPriceOfProperties = p.TotalPriceOfProperties
                    }).ToList()
                })
                .ToList();

            return Ok(graphData);
        }

        [HttpGet]
        [Route("GetTransactionTypesByMonth")]
        public IActionResult GetTransactionTypesByMonth()
        {
            var propertyTypesByMonth = _context.PropertyDetails
                .Where(b => b.TransactionTypeId.HasValue && b.Price.HasValue && b.CreatedAt.HasValue) // Ensure CreatedAt has a value
                .GroupBy(b => new
                {
                    TransactionTypeId = b.TransactionTypeId.Value,
                    Month = new DateTime(b.CreatedAt.Value.Year, b.CreatedAt.Value.Month, 1)
                })
                .Select(g => new
                {
                    TransactionTypeId = g.Key.TransactionTypeId,
                    Month = g.Key.Month,
                   // TotalPropertiesCreated = g.Count(),
                    TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderBy(g => g.Month)
                .ToList();

            var propertyTypes = _context.TransactionTypes.ToDictionary(pt => pt.Id, pt => pt.Name);

            var graphData = propertyTypesByMonth
                .GroupBy(p => p.TransactionTypeId)
                .Select(g => new
                {
                    PropertyType = propertyTypes.ContainsKey(g.Key) ? propertyTypes[g.Key] : "Unknown",
                    Data = g.Select(p => new
                    {
                        Month = p.Month.ToString("MMM yyyy"),
                      //  TotalPropertiesCreated = p.TotalPropertiesCreated,
                        TotalPriceOfProperties = p.TotalPriceOfProperties
                    }).ToList()
                })
                .ToList();

            return Ok(graphData);
        }


        [HttpGet]
        [Route("GetVehicleReportByMonth")]
        public IActionResult GetVehicleReportByMonth()
        {
            var propertyTypesByMonth = _context.VehiclesPropertyDetails
                .Where(b => b.Price.HasValue && b.CreatedAt.HasValue) // Ensure CreatedAt has a value
                .GroupBy(b => new
                {
                    Month = new DateTime(b.CreatedAt.Value.Year, b.CreatedAt.Value.Month,1)
                })
                .Select(g => new
                {
                    Month = g.Key.Month, // Store the DateTime without formatting
            TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderBy(g => g.Month)
                .AsEnumerable() // Switch to client-side evaluation after retrieving the data
                .Select(g => new
                {
                    Month = g.Month.ToString("MMM yyyy"), // Format the DateTime here
            g.TotalPriceOfProperties
                })
                .ToList();

            return Ok(propertyTypesByMonth);
        }

       
        [HttpGet]
        [Route("GetTopUsersSummaryByDate")]
        public IActionResult GetTopUsersSummaryByDate(string startDate, string endDate, string? calendarType = null)
        {
            if (!TryParseStartEndDates(startDate, endDate, calendarType, out var gregoriansDate, out var gregorianeDate))
            {
                return BadRequest("Invalid date range");
            }

            var topUsersSummary = _context.PropertyDetails
                .Where(b => b.PropertyTypeId.HasValue && b.Price.HasValue && b.CreatedAt.Value.Date >= gregoriansDate.Date &&
                           b.CreatedAt.Value.Date <= gregorianeDate.Date)
                .GroupBy(b => b.CreatedBy)
                .Select(g => new
                {
                    CreatedBy = g.Key,
                    TotalPropertiesCreated = g.Count(),
                    TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderByDescending(u => u.TotalPriceOfProperties)
                // .Take(10)
                .ToList();

            var topUsersWithNamesAndCompany = topUsersSummary
                .Join(
                    _userManager.Users,
                    topUser => topUser.CreatedBy,
                    user => user.Id,
                    (topUser, user) => new
                    {
                        CreatedBy = $"{user.FirstName} {user.LastName}",
                        topUser.TotalPropertiesCreated,
                        topUser.TotalPriceOfProperties,
                        user.CompanyId // Assuming you have the CompanyId property in ApplicationUser
                    }
                )
                .Join(
                    _context.CompanyDetails,
                    user => user.CompanyId,
                    company => company.Id,
                    (user, company) => new
                    {
                        user.CreatedBy,
                        user.TotalPropertiesCreated,
                        user.TotalPriceOfProperties,
                        CompanyTitle = company.Title // Assuming you have the CompanyTitle property in CompanyDetails
                    }
                )
                .ToList();

            var totalPriceSummary = new
            {
                TotalProperties = topUsersSummary.Sum(u => u.TotalPropertiesCreated),
                TotalPrice = topUsersSummary.Sum(u => u.TotalPriceOfProperties),
                TopUsers = topUsersWithNamesAndCompany
            };

            return Ok(totalPriceSummary);
        }


        [HttpGet]
        [Route("GetVehicleTopUsersSummaryByDate")]
        public IActionResult GetVehicleTopUsersSummaryByDate(string startDate, string endDate, string? calendarType = null)
        {
            if (!TryParseStartEndDates(startDate, endDate, calendarType, out var gregoriansDate, out var gregorianeDate))
            {
                return BadRequest("Invalid date range");
            }

            var topUsersSummary = _context.VehiclesPropertyDetails
                .Where(b => b.Price.HasValue && b.CreatedAt.Value.Date >= gregoriansDate.Date &&
                           b.CreatedAt.Value.Date <= gregorianeDate.Date)
                .GroupBy(b => b.CreatedBy)
                .Select(g => new
                {
                    CreatedBy = g.Key,
                    TotalPropertiesCreated = g.Count(),
                    TotalPriceOfProperties = g.Sum(b => b.Price ?? 0)
                })
                .OrderByDescending(u => u.TotalPriceOfProperties)
                // .Take(10)
                .ToList();

            var topUsersWithNamesAndCompany = topUsersSummary
                .Join(
                    _userManager.Users,
                    topUser => topUser.CreatedBy,
                    user => user.Id,
                    (topUser, user) => new
                    {
                        CreatedBy = $"{user.FirstName} {user.LastName}",
                        topUser.TotalPropertiesCreated,
                        topUser.TotalPriceOfProperties,
                        user.CompanyId // Assuming you have the CompanyId property in ApplicationUser
                    }
                )
                .Join(
                    _context.CompanyDetails,
                    user => user.CompanyId,
                    company => company.Id,
                    (user, company) => new
                    {
                        user.CreatedBy,
                        user.TotalPropertiesCreated,
                        user.TotalPriceOfProperties,
                        CompanyTitle = company.Title // Assuming you have the CompanyTitle property in CompanyDetails
                    }
                )
                .ToList();

            var totalPriceSummary = new
            {
                TotalProperties = topUsersSummary.Sum(u => u.TotalPropertiesCreated),
                TotalPrice = topUsersSummary.Sum(u => u.TotalPriceOfProperties),
                TopUsers = topUsersWithNamesAndCompany
            };

            return Ok(totalPriceSummary);
        }
    }
}
