using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;

namespace WebAPIBackend.Controllers.Securities;

/// <summary>
/// Report Controller for Securities Distribution
/// گزارش‌گیری اسناد بهادار رهنمای معاملات
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SecuritiesReportController : ControllerBase
{
    private readonly AppDbContext _context;

    public SecuritiesReportController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get report configuration options
    /// </summary>
    [HttpGet("config")]
    public IActionResult GetReportConfig()
    {
        return Ok(new
        {
            metrics = new[]
            {
                new { id = "propertySaleCount",    name = "تعداد سته خرید و فروش جایداد", category = "quantitative" },
                new { id = "bayWafaCount",         name = "تعداد سته بیع وفا",            category = "quantitative" },
                new { id = "rentCount",            name = "تعداد سته کرایی",              category = "quantitative" },
                new { id = "vehicleSaleCount",     name = "تعداد سته وسایط نقلیه",        category = "quantitative" },
                new { id = "vehicleExchangeCount", name = "تعداد سته تبادله",             category = "quantitative" },
                new { id = "registrationBookCount",name = "تعداد کتاب ثبت",              category = "quantitative" },
                new { id = "duplicateBookCount",   name = "تعداد کتاب ثبت مثنی",         category = "quantitative" },
                new { id = "totalDocumentsPrice",  name = "مبلغ سته‌ها",                  category = "financial" },
                new { id = "totalSecuritiesPrice", name = "مبلغ مجموع",                  category = "financial" },
                new { id = "recordCount",          name = "تعداد رکوردها",               category = "quantitative" }
            },
            groupByOptions = new[]
            {
                new { id = "guide",        name = "رهنمای معاملات" },
                new { id = "documentType", name = "نوع سند" },
                new { id = "month",        name = "ماه" },
                new { id = "year",         name = "سال" }
            },
            documentTypes = new[]
            {
                new { id = 1, name = "سته خرید و فروش جایداد" },
                new { id = 2, name = "سته بیع وفا" },
                new { id = 3, name = "سته کرایی" },
                new { id = 4, name = "سته وسایط نقلیه" },
                new { id = 5, name = "کتاب ثبت" },
                new { id = 6, name = "کتاب ثبت مثنی" }
            }
        });
    }

    /// <summary>
    /// Get summary statistics
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            var query = BuildBaseQueryWithItems(startDate, endDate, calendar);
            var distributions = await query.ToListAsync();
            var summary = CalculateSummary(distributions);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت آمار", error = ex.Message });
        }
    }

    /// <summary>
    /// Get report grouped by transaction guide
    /// </summary>
    [HttpGet("by-guide")]
    public async Task<IActionResult> GetByGuide(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            var distributions = await BuildBaseQuery(startDate, endDate, calendar)
                .Include(x => x.Items)
                .ToListAsync();

            var grouped = distributions
                .GroupBy(x => new { x.TransactionGuideName, x.LicenseOwnerName, x.LicenseNumber })
                .Select(g => new
                {
                    transactionGuideName    = g.Key.TransactionGuideName,
                    licenseOwnerName        = g.Key.LicenseOwnerName,
                    licenseNumber           = g.Key.LicenseNumber,
                    recordCount             = g.Count(),
                    propertySaleCount       = g.SelectMany(d => d.Items).Where(i => i.DocumentType == 1).Sum(i => i.Count),
                    bayWafaCount            = g.SelectMany(d => d.Items).Where(i => i.DocumentType == 2).Sum(i => i.Count),
                    rentCount               = g.SelectMany(d => d.Items).Where(i => i.DocumentType == 3).Sum(i => i.Count),
                    vehicleSaleCount        = g.SelectMany(d => d.Items).Where(i => i.DocumentType == 4).Sum(i => i.Count),
                    registrationBookCount   = g.SelectMany(d => d.Items).Where(i => i.DocumentType == 5).Sum(i => i.Count),
                    duplicateBookCount      = g.SelectMany(d => d.Items).Where(i => i.DocumentType == 6).Sum(i => i.Count),
                    totalDocumentsPrice     = g.Sum(d => d.TotalDocumentsPrice ?? 0),
                    totalSecuritiesPrice    = g.Sum(d => d.TotalSecuritiesPrice ?? 0)
                })
                .OrderByDescending(x => x.totalSecuritiesPrice)
                .ToList();

            return Ok(new { data = grouped, summary = CalculateSummary(distributions) });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Get report grouped by document type
    /// </summary>
    [HttpGet("by-document-type")]
    public async Task<IActionResult> GetByDocumentType(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            var distributions = await BuildBaseQuery(startDate, endDate, calendar)
                .Include(x => x.Items)
                .ToListAsync();

            var allItems = distributions.SelectMany(d => d.Items).ToList();

            // Template iterates byDocumentTypeData as: docType.documentTypeName, docType.subTypes[].name/count/amount
            // Group into two categories: سته‌ها (1-4) and کتاب ثبت (5-6)
            var result = new[]
            {
                new
                {
                    documentTypeName = "سته‌های معاملاتی",
                    subTypes = new[]
                    {
                        new { name = "سته خرید و فروش جایداد", count = allItems.Where(i => i.DocumentType == 1).Sum(i => i.Count), amount = allItems.Where(i => i.DocumentType == 1).Sum(i => i.Price * i.Count) },
                        new { name = "سته بیع وفا",             count = allItems.Where(i => i.DocumentType == 2).Sum(i => i.Count), amount = allItems.Where(i => i.DocumentType == 2).Sum(i => i.Price * i.Count) },
                        new { name = "سته کرایی",               count = allItems.Where(i => i.DocumentType == 3).Sum(i => i.Count), amount = allItems.Where(i => i.DocumentType == 3).Sum(i => i.Price * i.Count) },
                        new { name = "سته وسایط نقلیه",         count = allItems.Where(i => i.DocumentType == 4).Sum(i => i.Count), amount = allItems.Where(i => i.DocumentType == 4).Sum(i => i.Price * i.Count) }
                    }
                },
                new
                {
                    documentTypeName = "کتاب ثبت",
                    subTypes = new[]
                    {
                        new { name = "کتاب ثبت",       count = allItems.Where(i => i.DocumentType == 5).Sum(i => i.Count), amount = allItems.Where(i => i.DocumentType == 5).Sum(i => i.Price * i.Count) },
                        new { name = "کتاب ثبت مثنی",  count = allItems.Where(i => i.DocumentType == 6).Sum(i => i.Count), amount = allItems.Where(i => i.DocumentType == 6).Sum(i => i.Price * i.Count) }
                    }
                }
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Get report grouped by book type (registration books)
    /// </summary>
    [HttpGet("by-book-type")]
    public async Task<IActionResult> GetByBookType(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            var distributions = await BuildBaseQuery(startDate, endDate, calendar)
                .Include(x => x.Items)
                .ToListAsync();

            var allItems = distributions.SelectMany(d => d.Items).ToList();

            var regBookCount  = allItems.Where(i => i.DocumentType == 5).Sum(i => i.Count);
            var regBookPrice  = allItems.Where(i => i.DocumentType == 5).Sum(i => i.Price * i.Count);
            var dupBookCount  = allItems.Where(i => i.DocumentType == 6).Sum(i => i.Count);
            var dupBookPrice  = allItems.Where(i => i.DocumentType == 6).Sum(i => i.Price * i.Count);

            return Ok(new
            {
                // Shape matches frontend template: byBookTypeData.newBooks.count / .amount
                newBooks = new
                {
                    name   = "کتاب ثبت",
                    count  = regBookCount,
                    amount = regBookPrice
                },
                duplicateBooks = new
                {
                    name   = "کتاب ثبت مثنی",
                    count  = dupBookCount,
                    amount = dupBookPrice
                },
                totalBooks = new
                {
                    name   = "مجموع کتاب ثبت",
                    count  = regBookCount + dupBookCount,
                    amount = regBookPrice + dupBookPrice
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Get monthly trend report
    /// </summary>
    [HttpGet("monthly-trend")]
    public async Task<IActionResult> GetMonthlyTrend(
        [FromQuery] int? year = null,
        [FromQuery] string? calendarType = null)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            var query = _context.SecuritiesDistributions
                .AsNoTracking()
                .Include(x => x.Items)
                .Where(x => x.Status == true);

            var records = await query.ToListAsync();

            // Group by Hijri Shamsi year/month (convert each record's date)
            var monthly = records
                .Where(x => x.DistributionDate.HasValue)
                .Select(x => new
                {
                    Distribution = x,
                    HijriDate = DateConversionHelper.FromGregorian(
                        x.DistributionDate!.Value.ToDateTime(TimeOnly.MinValue),
                        CalendarType.HijriShamsi)
                })
                .Where(x => !year.HasValue || x.HijriDate.year == year.Value)
                .GroupBy(x => new { x.HijriDate.year, x.HijriDate.month })
                .Select(g => new
                {
                    year           = g.Key.year,
                    month          = g.Key.month,
                    monthName      = GetMonthName(g.Key.month, CalendarType.HijriShamsi),
                    recordCount    = g.Count(),
                    totalDocuments = g.SelectMany(d => d.Distribution.Items).Where(i => i.DocumentType >= 1 && i.DocumentType <= 4).Sum(i => i.Count),
                    totalBooks     = g.SelectMany(d => d.Distribution.Items).Where(i => i.DocumentType == 5 || i.DocumentType == 6).Sum(i => i.Count),
                    totalAmount    = g.Sum(d => d.Distribution.TotalSecuritiesPrice ?? 0)
                })
                .OrderBy(x => x.year).ThenBy(x => x.month)
                .ToList();

            return Ok(monthly);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Get serial number tracking report
    /// </summary>
    [HttpGet("serial-tracking")]
    public async Task<IActionResult> GetSerialTracking(
        [FromQuery] string? licenseNumber = null,
        [FromQuery] string? transactionGuideName = null,
        [FromQuery] int? documentType = null,
        [FromQuery] string? calendarType = null)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(calendarType);
            var query = _context.SecuritiesDistributions
                .AsNoTracking()
                .Include(x => x.Items)
                .Where(x => x.Status == true);

            if (!string.IsNullOrEmpty(licenseNumber))
                query = query.Where(x => x.LicenseNumber.Contains(licenseNumber));
            if (!string.IsNullOrEmpty(transactionGuideName))
                query = query.Where(x => x.TransactionGuideName.Contains(transactionGuideName));

            var records = await query
                .OrderBy(x => x.TransactionGuideName)
                .ThenBy(x => x.DistributionDate)
                .ToListAsync();

            var result = records.SelectMany(d => d.Items
                .Where(i => !documentType.HasValue || i.DocumentType == documentType.Value)
                .Select(i => new
                {
                    distributionId       = d.Id,
                    registrationNumber   = d.RegistrationNumber,
                    transactionGuideName = d.TransactionGuideName,
                    licenseOwnerName     = d.LicenseOwnerName,
                    licenseNumber        = d.LicenseNumber,
                    documentType         = i.DocumentType,
                    documentTypeName     = GetDocumentTypeName(i.DocumentType),
                    serialStart          = i.SerialStart,
                    serialEnd            = i.SerialEnd,
                    count                = i.Count,
                    price                = i.Price,
                    distributionDate     = DateConversionHelper.FormatDateOnly(d.DistributionDate, calendar)
                }))
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Generate report (POST with full filter body)
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport([FromBody] SecuritiesReportRequestBody request)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);
            var query = BuildBaseQuery(request.StartDate, request.EndDate, calendar);

            if (!string.IsNullOrEmpty(request.TransactionGuideName))
                query = query.Where(x => x.TransactionGuideName.Contains(request.TransactionGuideName));
            if (!string.IsNullOrEmpty(request.LicenseNumber))
                query = query.Where(x => x.LicenseNumber.Contains(request.LicenseNumber));
            if (!string.IsNullOrEmpty(request.RegistrationNumber))
                query = query.Where(x => x.RegistrationNumber.Contains(request.RegistrationNumber));

            var distributions = await query.Include(x => x.Items).ToListAsync();

            var data = distributions.Select(d => new
            {
                d.Id,
                d.RegistrationNumber,
                d.LicenseOwnerName,
                d.LicenseOwnerFatherName,
                d.TransactionGuideName,
                d.LicenseNumber,
                d.BankReceiptNumber,
                d.PricePerDocument,
                d.TotalDocumentsPrice,
                d.TotalSecuritiesPrice,
                distributionDate     = DateConversionHelper.FormatDateOnly(d.DistributionDate, calendar),
                deliveryDate         = DateConversionHelper.FormatDateOnly(d.DeliveryDate, calendar),
                propertySaleCount    = d.Items.Where(i => i.DocumentType == 1).Sum(i => i.Count),
                bayWafaCount         = d.Items.Where(i => i.DocumentType == 2).Sum(i => i.Count),
                rentCount            = d.Items.Where(i => i.DocumentType == 3).Sum(i => i.Count),
                vehicleSaleCount     = d.Items.Where(i => i.DocumentType == 4).Sum(i => i.Count),
                registrationBookCount= d.Items.Where(i => i.DocumentType == 5).Sum(i => i.Count),
                duplicateBookCount   = d.Items.Where(i => i.DocumentType == 6).Sum(i => i.Count),
                items                = d.Items.Select(i => new
                {
                    i.DocumentType,
                    documentTypeName = GetDocumentTypeName(i.DocumentType),
                    i.SerialStart,
                    i.SerialEnd,
                    i.Count,
                    i.Price
                })
            }).ToList();

            return Ok(new
            {
                summary  = CalculateSummary(distributions),
                data,
                metadata = new
                {
                    generatedAt    = DateTime.UtcNow,
                    reportTitle    = "گزارش اسناد بهادار رهنمای معاملات",
                    totalRecords   = distributions.Count,
                    appliedFilters = BuildFilterDescription(request),
                    groupedBy      = request.GroupBy ?? new List<string>(),
                    includedMetrics= request.Metrics ?? new List<string>()
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در تولید گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Export report data
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportReport([FromBody] SecuritiesReportRequestBody request)
    {
        // Reuse generate endpoint logic
        return await GenerateReport(request);
    }

    #region Private Helpers

    private IQueryable<SecuritiesDistribution> BuildBaseQuery(
        string? startDate, string? endDate, CalendarType calendar)
    {
        var query = _context.SecuritiesDistributions
            .AsNoTracking()
            .Where(x => x.Status == true);

        if (!string.IsNullOrWhiteSpace(startDate))
        {
            if (DateConversionHelper.TryParseToDateOnly(startDate, calendar, out var start))
                query = query.Where(x => x.DistributionDate >= start);
        }
        if (!string.IsNullOrWhiteSpace(endDate))
        {
            if (DateConversionHelper.TryParseToDateOnly(endDate, calendar, out var end))
                query = query.Where(x => x.DistributionDate <= end);
        }

        return query;
    }

    // Always eager-load Items so SelectMany works correctly after materialization
    private IQueryable<SecuritiesDistribution> BuildBaseQueryWithItems(
        string? startDate, string? endDate, CalendarType calendar)
        => BuildBaseQuery(startDate, endDate, calendar).Include(x => x.Items);

    private static object CalculateSummary(List<SecuritiesDistribution> distributions)
    {
        var allItems = distributions.SelectMany(d => d.Items).ToList();

        var propertySaleCount    = allItems.Where(i => i.DocumentType == 1).Sum(i => i.Count);
        var bayWafaCount         = allItems.Where(i => i.DocumentType == 2).Sum(i => i.Count);
        var rentCount            = allItems.Where(i => i.DocumentType == 3).Sum(i => i.Count);
        var vehicleSaleCount     = allItems.Where(i => i.DocumentType == 4).Sum(i => i.Count);
        var registrationBookCount= allItems.Where(i => i.DocumentType == 5).Sum(i => i.Count);
        var duplicateBookCount   = allItems.Where(i => i.DocumentType == 6).Sum(i => i.Count);

        return new
        {
            totalRecords          = distributions.Count,
            propertySaleCount,
            bayWafaCount,
            rentCount,
            vehicleSaleCount,
            vehicleExchangeCount  = 0,   // kept for frontend compatibility
            registrationBookCount,
            duplicateBookCount,
            totalPropertyDocuments= propertySaleCount + bayWafaCount + rentCount,
            totalVehicleDocuments = vehicleSaleCount,
            totalDocuments        = propertySaleCount + bayWafaCount + rentCount + vehicleSaleCount,
            totalBookCount        = registrationBookCount + duplicateBookCount,
            totalDocumentsPrice   = distributions.Sum(d => d.TotalDocumentsPrice ?? 0),
            totalRegistrationBookPrice = allItems.Where(i => i.DocumentType == 5 || i.DocumentType == 6)
                                                 .Sum(i => i.Price * i.Count),
            totalSecuritiesPrice  = distributions.Sum(d => d.TotalSecuritiesPrice ?? 0)
        };
    }

    private static string GetDocumentTypeName(int documentType) => documentType switch
    {
        1 => "سته خرید و فروش جایداد",
        2 => "سته بیع وفا",
        3 => "سته کرایی",
        4 => "سته وسایط نقلیه",
        5 => "کتاب ثبت",
        6 => "کتاب ثبت مثنی",
        _ => "نامشخص"
    };

    private static string GetMonthName(int month, CalendarType calendar)
    {
        var shamsi    = new[] { "حمل", "ثور", "جوزا", "سرطان", "اسد", "سنبله", "میزان", "عقرب", "قوس", "جدی", "دلو", "حوت" };
        var gregorian = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        if (month < 1 || month > 12) return month.ToString();
        return calendar == CalendarType.Gregorian ? gregorian[month - 1] : shamsi[month - 1];
    }

    private static List<string> BuildFilterDescription(SecuritiesReportRequestBody request)
    {
        var filters = new List<string>();
        if (!string.IsNullOrEmpty(request.StartDate))         filters.Add($"از تاریخ: {request.StartDate}");
        if (!string.IsNullOrEmpty(request.EndDate))           filters.Add($"الی تاریخ: {request.EndDate}");
        if (!string.IsNullOrEmpty(request.TransactionGuideName)) filters.Add($"رهنمای معاملات: {request.TransactionGuideName}");
        if (!string.IsNullOrEmpty(request.LicenseNumber))     filters.Add($"نمبر جواز: {request.LicenseNumber}");
        if (!string.IsNullOrEmpty(request.RegistrationNumber))filters.Add($"نمبر ثبت: {request.RegistrationNumber}");
        return filters;
    }

    #endregion
}

/// <summary>
/// Request body for POST report endpoints
/// </summary>
public class SecuritiesReportRequestBody
{
    public List<string>? Metrics { get; set; }
    public List<string>? GroupBy { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? TransactionGuideName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public int? DocumentType { get; set; }
    public int? RegistrationBookType { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? CalendarType { get; set; }
}
