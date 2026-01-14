using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers.Securities;

/// <summary>
/// Dynamic Report Controller for Petition Writer Securities
/// گزارش‌گیری سند بهادار عریضه‌نویسان
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PetitionWriterReportController : ControllerBase
{
    private readonly AppDbContext _context;

    public PetitionWriterReportController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get available report configuration options
    /// دریافت تنظیمات گزارش
    /// </summary>
    [HttpGet("config")]
    public IActionResult GetReportConfig()
    {
        var config = new
        {
            metrics = GetAvailableMetrics(),
            groupByOptions = GetGroupByOptions()
        };
        return Ok(config);
    }

    /// <summary>
    /// Generate dynamic report based on request parameters
    /// تولید گزارش پویا
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport([FromBody] PetitionWriterReportRequest request)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);
            var query = _context.PetitionWriterSecurities
                .Where(x => x.Status == true)
                .AsQueryable();

            // Apply filters
            query = ApplyFilters(query, request);

            // Get all matching records
            var records = await query.ToListAsync();

            // Generate response based on grouping
            var response = new PetitionWriterReportResponse
            {
                Summary = CalculateSummary(records),
                Data = GenerateReportData(records, request, calendar),
                Metadata = GenerateMetadata(request)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "خطا در تولید گزارش", error = ex.Message });
        }
    }

    /// <summary>
    /// Get summary statistics without grouping
    /// خلاصه آماری
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        // Apply date filters
        query = ApplyDateFilter(query, startDate, endDate, calendar);

        var records = await query.ToListAsync();
        var summary = CalculateSummary(records);

        return Ok(summary);
    }

    /// <summary>
    /// Get report grouped by petition writer
    /// گزارش به تفکیک عریضه‌نویس
    /// </summary>
    [HttpGet("by-writer")]
    public async Task<IActionResult> GetByWriter(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        query = ApplyDateFilter(query, startDate, endDate, calendar);

        var grouped = await query
            .GroupBy(x => new { x.PetitionWriterName, x.PetitionWriterFatherName, x.LicenseNumber })
            .Select(g => new PetitionWriterReportRow
            {
                PetitionWriterName = g.Key.PetitionWriterName,
                PetitionWriterFatherName = g.Key.PetitionWriterFatherName,
                LicenseNumber = g.Key.LicenseNumber,
                RecordCount = g.Count(),
                TotalPetitionCount = g.Sum(x => x.PetitionCount),
                TotalAmount = g.Sum(x => x.Amount),
                AverageAmount = g.Average(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();

        return Ok(new { data = grouped, summary = CalculateSummaryFromRows(grouped) });
    }

    /// <summary>
    /// Get report grouped by license number
    /// گزارش به تفکیک نمبر جواز
    /// </summary>
    [HttpGet("by-license")]
    public async Task<IActionResult> GetByLicense(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        query = ApplyDateFilter(query, startDate, endDate, calendar);

        var grouped = await query
            .GroupBy(x => x.LicenseNumber)
            .Select(g => new PetitionWriterReportRow
            {
                GroupKey = g.Key,
                LicenseNumber = g.Key,
                PetitionWriterName = g.First().PetitionWriterName,
                PetitionWriterFatherName = g.First().PetitionWriterFatherName,
                RecordCount = g.Count(),
                TotalPetitionCount = g.Sum(x => x.PetitionCount),
                TotalAmount = g.Sum(x => x.Amount),
                AverageAmount = g.Average(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();

        return Ok(new { data = grouped, summary = CalculateSummaryFromRows(grouped) });
    }

    /// <summary>
    /// Get monthly trend report
    /// روند ماهانه
    /// </summary>
    [HttpGet("monthly-trend")]
    public async Task<IActionResult> GetMonthlyTrend(
        [FromQuery] int? year = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        if (year.HasValue)
        {
            query = query.Where(x => x.DistributionDate.Year == year.Value);
        }

        var records = await query.ToListAsync();

        var monthlyData = records
            .GroupBy(x => new { x.DistributionDate.Year, x.DistributionDate.Month })
            .Select(g => new
            {
                year = g.Key.Year,
                month = g.Key.Month,
                monthName = GetMonthName(g.Key.Month, calendar),
                recordCount = g.Count(),
                totalPetitionCount = g.Sum(x => x.PetitionCount),
                totalAmount = g.Sum(x => x.Amount),
                averageAmount = g.Average(x => x.Amount)
            })
            .OrderBy(x => x.year)
            .ThenBy(x => x.month)
            .ToList();

        return Ok(monthlyData);
    }

    /// <summary>
    /// Get yearly trend report
    /// روند سالانه
    /// </summary>
    [HttpGet("yearly-trend")]
    public async Task<IActionResult> GetYearlyTrend([FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        var records = await query.ToListAsync();

        var yearlyData = records
            .GroupBy(x => x.DistributionDate.Year)
            .Select(g => new
            {
                year = g.Key,
                recordCount = g.Count(),
                totalPetitionCount = g.Sum(x => x.PetitionCount),
                totalAmount = g.Sum(x => x.Amount),
                uniqueWriters = g.Select(x => x.PetitionWriterName).Distinct().Count(),
                uniqueLicenses = g.Select(x => x.LicenseNumber).Distinct().Count()
            })
            .OrderBy(x => x.year)
            .ToList();

        return Ok(yearlyData);
    }

    /// <summary>
    /// Get serial number tracking report
    /// ردیابی سریال نمبرها
    /// </summary>
    [HttpGet("serial-tracking")]
    public async Task<IActionResult> GetSerialTracking(
        [FromQuery] string? licenseNumber = null,
        [FromQuery] string? petitionWriterName = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        if (!string.IsNullOrEmpty(licenseNumber))
            query = query.Where(x => x.LicenseNumber.Contains(licenseNumber));
        if (!string.IsNullOrEmpty(petitionWriterName))
            query = query.Where(x => x.PetitionWriterName.Contains(petitionWriterName));

        var records = await query
            .OrderBy(x => x.PetitionWriterName)
            .ThenBy(x => x.DistributionDate)
            .Select(x => new
            {
                x.Id,
                x.RegistrationNumber,
                x.PetitionWriterName,
                x.PetitionWriterFatherName,
                x.LicenseNumber,
                x.PetitionCount,
                x.Amount,
                x.BankReceiptNumber,
                x.SerialNumberStart,
                x.SerialNumberEnd,
                serialRange = FormatSerialRange(x.SerialNumberStart, x.SerialNumberEnd),
                distributionDate = DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar)
            })
            .ToListAsync();

        return Ok(records);
    }

    /// <summary>
    /// Get detailed list with all fields
    /// لیست تفصیلی
    /// </summary>
    [HttpGet("detailed-list")]
    public async Task<IActionResult> GetDetailedList(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? petitionWriterName = null,
        [FromQuery] string? licenseNumber = null,
        [FromQuery] string? registrationNumber = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.PetitionWriterSecurities
            .Where(x => x.Status == true)
            .AsQueryable();

        // Apply filters
        query = ApplyDateFilter(query, startDate, endDate, calendar);
        
        if (!string.IsNullOrEmpty(petitionWriterName))
            query = query.Where(x => x.PetitionWriterName.Contains(petitionWriterName));
        if (!string.IsNullOrEmpty(licenseNumber))
            query = query.Where(x => x.LicenseNumber.Contains(licenseNumber));
        if (!string.IsNullOrEmpty(registrationNumber))
            query = query.Where(x => x.RegistrationNumber.Contains(registrationNumber));

        var totalCount = await query.CountAsync();
        
        var records = await query
            .OrderByDescending(x => x.DistributionDate)
            .ThenBy(x => x.PetitionWriterName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.RegistrationNumber,
                x.PetitionWriterName,
                x.PetitionWriterFatherName,
                x.LicenseNumber,
                x.PetitionCount,
                x.Amount,
                x.BankReceiptNumber,
                x.SerialNumberStart,
                x.SerialNumberEnd,
                serialRange = FormatSerialRange(x.SerialNumberStart, x.SerialNumberEnd),
                distributionDate = DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar)
            })
            .ToListAsync();

        return Ok(new
        {
            data = records,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    /// <summary>
    /// Export report data for PDF/Excel generation
    /// صادرات گزارش
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportReport([FromBody] PetitionWriterReportRequest request)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);
            var query = _context.PetitionWriterSecurities
                .Where(x => x.Status == true)
                .AsQueryable();

            query = ApplyFilters(query, request);

            var records = await query
                .OrderBy(x => x.PetitionWriterName)
                .ThenByDescending(x => x.DistributionDate)
                .Select(x => new
                {
                    x.RegistrationNumber,
                    x.PetitionWriterName,
                    x.PetitionWriterFatherName,
                    x.LicenseNumber,
                    x.PetitionCount,
                    x.Amount,
                    x.BankReceiptNumber,
                    x.SerialNumberStart,
                    x.SerialNumberEnd,
                    serialRange = FormatSerialRange(x.SerialNumberStart, x.SerialNumberEnd),
                    distributionDate = DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar)
                })
                .ToListAsync();

            var allRecords = await query.ToListAsync();

            return Ok(new
            {
                data = records,
                summary = CalculateSummary(allRecords),
                metadata = GenerateMetadata(request)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "خطا در صادرات گزارش", error = ex.Message });
        }
    }

    #region Private Helper Methods

    private IQueryable<PetitionWriterSecurities> ApplyFilters(
        IQueryable<PetitionWriterSecurities> query, 
        PetitionWriterReportRequest request)
    {
        var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);

        // Date filters
        query = ApplyDateFilter(query, request.StartDate, request.EndDate, calendar);

        // Identity filters
        if (!string.IsNullOrEmpty(request.PetitionWriterName))
            query = query.Where(x => x.PetitionWriterName.Contains(request.PetitionWriterName));
        if (!string.IsNullOrEmpty(request.LicenseNumber))
            query = query.Where(x => x.LicenseNumber.Contains(request.LicenseNumber));
        if (!string.IsNullOrEmpty(request.RegistrationNumber))
            query = query.Where(x => x.RegistrationNumber.Contains(request.RegistrationNumber));

        // Value filters
        if (request.MinAmount.HasValue)
            query = query.Where(x => x.Amount >= request.MinAmount.Value);
        if (request.MaxAmount.HasValue)
            query = query.Where(x => x.Amount <= request.MaxAmount.Value);
        if (request.MinCount.HasValue)
            query = query.Where(x => x.PetitionCount >= request.MinCount.Value);
        if (request.MaxCount.HasValue)
            query = query.Where(x => x.PetitionCount <= request.MaxCount.Value);

        return query;
    }

    private IQueryable<PetitionWriterSecurities> ApplyDateFilter(
        IQueryable<PetitionWriterSecurities> query,
        string? startDate, string? endDate,
        CalendarType calendar)
    {
        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateConversionHelper.ParseDateOnly(startDate, calendar);
            if (start.HasValue)
                query = query.Where(x => x.DistributionDate >= start.Value);
        }
        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateConversionHelper.ParseDateOnly(endDate, calendar);
            if (end.HasValue)
                query = query.Where(x => x.DistributionDate <= end.Value);
        }
        return query;
    }

    private PetitionWriterReportSummary CalculateSummary(List<PetitionWriterSecurities> records)
    {
        if (records.Count == 0)
        {
            return new PetitionWriterReportSummary();
        }

        return new PetitionWriterReportSummary
        {
            TotalRecords = records.Count,
            TotalPetitionCount = records.Sum(x => x.PetitionCount),
            TotalAmount = records.Sum(x => x.Amount),
            UniquePetitionWriters = records.Select(x => x.PetitionWriterName).Distinct().Count(),
            UniqueLicenses = records.Select(x => x.LicenseNumber).Distinct().Count(),
            AverageAmountPerDistribution = records.Average(x => x.Amount),
            AveragePetitionsPerDistribution = (decimal)records.Average(x => x.PetitionCount),
            MinSerialNumber = records.OrderBy(x => x.SerialNumberStart).FirstOrDefault()?.SerialNumberStart,
            MaxSerialNumber = records.OrderByDescending(x => x.SerialNumberEnd).FirstOrDefault()?.SerialNumberEnd
        };
    }

    private PetitionWriterReportSummary CalculateSummaryFromRows(List<PetitionWriterReportRow> rows)
    {
        if (rows.Count == 0)
        {
            return new PetitionWriterReportSummary();
        }

        return new PetitionWriterReportSummary
        {
            TotalRecords = rows.Sum(x => x.RecordCount),
            TotalPetitionCount = rows.Sum(x => x.TotalPetitionCount),
            TotalAmount = rows.Sum(x => x.TotalAmount),
            UniquePetitionWriters = rows.Select(x => x.PetitionWriterName).Distinct().Count(),
            UniqueLicenses = rows.Select(x => x.LicenseNumber).Distinct().Count(),
            AverageAmountPerDistribution = rows.Sum(x => x.RecordCount) > 0 
                ? rows.Sum(x => x.TotalAmount) / rows.Sum(x => x.RecordCount) 
                : 0,
            AveragePetitionsPerDistribution = rows.Sum(x => x.RecordCount) > 0 
                ? (decimal)rows.Sum(x => x.TotalPetitionCount) / rows.Sum(x => x.RecordCount) 
                : 0
        };
    }

    private List<PetitionWriterReportRow> GenerateReportData(
        List<PetitionWriterSecurities> records,
        PetitionWriterReportRequest request,
        CalendarType calendar)
    {
        if (request.GroupBy == null || request.GroupBy.Count == 0)
        {
            // No grouping - return individual records
            return records.Select(x => new PetitionWriterReportRow
            {
                RegistrationNumber = x.RegistrationNumber,
                PetitionWriterName = x.PetitionWriterName,
                PetitionWriterFatherName = x.PetitionWriterFatherName,
                LicenseNumber = x.LicenseNumber,
                RecordCount = 1,
                TotalPetitionCount = x.PetitionCount,
                TotalAmount = x.Amount,
                AverageAmount = x.Amount,
                SerialNumberStart = x.SerialNumberStart,
                SerialNumberEnd = x.SerialNumberEnd,
                SerialRange = FormatSerialRange(x.SerialNumberStart, x.SerialNumberEnd),
                BankReceiptNumber = x.BankReceiptNumber,
                DistributionDate = DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar)
            }).ToList();
        }

        // Group by first grouping option
        var groupBy = request.GroupBy[0];
        return groupBy switch
        {
            PetitionWriterReportGroupBy.PetitionWriter => GroupByPetitionWriter(records),
            PetitionWriterReportGroupBy.LicenseNumber => GroupByLicenseNumber(records),
            PetitionWriterReportGroupBy.Month => GroupByMonth(records, calendar),
            PetitionWriterReportGroupBy.Year => GroupByYear(records),
            _ => records.Select(x => MapToReportRow(x, calendar)).ToList()
        };
    }

    private List<PetitionWriterReportRow> GroupByPetitionWriter(List<PetitionWriterSecurities> records)
    {
        return records
            .GroupBy(x => new { x.PetitionWriterName, x.PetitionWriterFatherName, x.LicenseNumber })
            .Select(g => new PetitionWriterReportRow
            {
                GroupKey = g.Key.PetitionWriterName,
                PetitionWriterName = g.Key.PetitionWriterName,
                PetitionWriterFatherName = g.Key.PetitionWriterFatherName,
                LicenseNumber = g.Key.LicenseNumber,
                RecordCount = g.Count(),
                TotalPetitionCount = g.Sum(x => x.PetitionCount),
                TotalAmount = g.Sum(x => x.Amount),
                AverageAmount = g.Average(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    private List<PetitionWriterReportRow> GroupByLicenseNumber(List<PetitionWriterSecurities> records)
    {
        return records
            .GroupBy(x => x.LicenseNumber)
            .Select(g => new PetitionWriterReportRow
            {
                GroupKey = g.Key,
                LicenseNumber = g.Key,
                PetitionWriterName = g.First().PetitionWriterName,
                PetitionWriterFatherName = g.First().PetitionWriterFatherName,
                RecordCount = g.Count(),
                TotalPetitionCount = g.Sum(x => x.PetitionCount),
                TotalAmount = g.Sum(x => x.Amount),
                AverageAmount = g.Average(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    private List<PetitionWriterReportRow> GroupByMonth(List<PetitionWriterSecurities> records, CalendarType calendar)
    {
        return records
            .GroupBy(x => new { x.DistributionDate.Year, x.DistributionDate.Month })
            .Select(g => new PetitionWriterReportRow
            {
                GroupKey = $"{g.Key.Year}-{g.Key.Month:D2}",
                YearGroup = g.Key.Year,
                MonthGroup = g.Key.Month,
                MonthName = GetMonthName(g.Key.Month, calendar),
                RecordCount = g.Count(),
                TotalPetitionCount = g.Sum(x => x.PetitionCount),
                TotalAmount = g.Sum(x => x.Amount),
                AverageAmount = g.Average(x => x.Amount)
            })
            .OrderBy(x => x.YearGroup)
            .ThenBy(x => x.MonthGroup)
            .ToList();
    }

    private List<PetitionWriterReportRow> GroupByYear(List<PetitionWriterSecurities> records)
    {
        return records
            .GroupBy(x => x.DistributionDate.Year)
            .Select(g => new PetitionWriterReportRow
            {
                GroupKey = g.Key.ToString(),
                YearGroup = g.Key,
                RecordCount = g.Count(),
                TotalPetitionCount = g.Sum(x => x.PetitionCount),
                TotalAmount = g.Sum(x => x.Amount),
                AverageAmount = g.Average(x => x.Amount)
            })
            .OrderBy(x => x.YearGroup)
            .ToList();
    }

    private PetitionWriterReportRow MapToReportRow(PetitionWriterSecurities x, CalendarType calendar)
    {
        return new PetitionWriterReportRow
        {
            RegistrationNumber = x.RegistrationNumber,
            PetitionWriterName = x.PetitionWriterName,
            PetitionWriterFatherName = x.PetitionWriterFatherName,
            LicenseNumber = x.LicenseNumber,
            RecordCount = 1,
            TotalPetitionCount = x.PetitionCount,
            TotalAmount = x.Amount,
            AverageAmount = x.Amount,
            SerialNumberStart = x.SerialNumberStart,
            SerialNumberEnd = x.SerialNumberEnd,
            SerialRange = FormatSerialRange(x.SerialNumberStart, x.SerialNumberEnd),
            BankReceiptNumber = x.BankReceiptNumber,
            DistributionDate = DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar)
        };
    }

    private PetitionWriterReportMetadata GenerateMetadata(PetitionWriterReportRequest request)
    {
        var filters = new List<string>();
        if (!string.IsNullOrEmpty(request.StartDate)) filters.Add($"از تاریخ: {request.StartDate}");
        if (!string.IsNullOrEmpty(request.EndDate)) filters.Add($"الی تاریخ: {request.EndDate}");
        if (!string.IsNullOrEmpty(request.PetitionWriterName)) filters.Add($"اسم عریضه‌نویس: {request.PetitionWriterName}");
        if (!string.IsNullOrEmpty(request.LicenseNumber)) filters.Add($"نمبر جواز: {request.LicenseNumber}");
        if (!string.IsNullOrEmpty(request.RegistrationNumber)) filters.Add($"نمبر ثبت: {request.RegistrationNumber}");
        if (request.MinAmount.HasValue) filters.Add($"حداقل مبلغ: {request.MinAmount}");
        if (request.MaxAmount.HasValue) filters.Add($"حداکثر مبلغ: {request.MaxAmount}");

        return new PetitionWriterReportMetadata
        {
            GeneratedAt = DateTime.UtcNow,
            ReportTitle = "گزارش سند بهادار عریضه‌نویسان",
            AppliedFilters = filters,
            GroupedBy = request.GroupBy ?? new List<string>(),
            IncludedMetrics = request.Metrics ?? new List<string>()
        };
    }

    private static string FormatSerialRange(string? start, string? end)
    {
        if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end)) return "-";
        if (string.IsNullOrEmpty(end)) return start ?? "-";
        if (string.IsNullOrEmpty(start)) return end;
        return $"{start} - {end}";
    }

    private static string GetMonthName(int month, CalendarType calendar)
    {
        var shamsiMonths = new[] { "حمل", "ثور", "جوزا", "سرطان", "اسد", "سنبله", "میزان", "عقرب", "قوس", "جدی", "دلو", "حوت" };
        var gregorianMonths = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        
        if (month < 1 || month > 12) return month.ToString();
        return calendar == CalendarType.Gregorian ? gregorianMonths[month - 1] : shamsiMonths[month - 1];
    }

    private static List<object> GetAvailableMetrics()
    {
        return new List<object>
        {
            new { id = "total_petition_count", name = "تعداد عریضه مطبوع", category = "quantitative" },
            new { id = "record_count", name = "تعداد رکوردها", category = "quantitative" },
            new { id = "unique_petition_writers", name = "تعداد عریضه‌نویسان یکتا", category = "quantitative" },
            new { id = "unique_licenses", name = "تعداد جوازهای یکتا", category = "quantitative" },
            new { id = "total_amount", name = "مبلغ پول عریضه", category = "financial" },
            new { id = "average_amount", name = "میانگین مبلغ", category = "financial" },
            new { id = "registration_number", name = "نمبر ثبت تعرفه", category = "identity" },
            new { id = "license_number", name = "نمبر جواز عریضه‌نویس / وکیل", category = "identity" },
            new { id = "petition_writer_name", name = "اسم عریضه‌نویس / وکیل", category = "identity" },
            new { id = "serial_numbers", name = "سریال نمبر عریضه", category = "identity" }
        };
    }

    private static List<object> GetGroupByOptions()
    {
        return new List<object>
        {
            new { id = "petition_writer", name = "عریضه‌نویس" },
            new { id = "license_number", name = "نمبر جواز" },
            new { id = "month", name = "ماه" },
            new { id = "year", name = "سال" }
        };
    }

    #endregion
}
