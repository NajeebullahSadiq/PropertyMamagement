using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers.Securities;

/// <summary>
/// Dynamic Report Controller for Securities Distribution
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
    /// Get available report configuration options
    /// </summary>
    [HttpGet("config")]
    public IActionResult GetReportConfig()
    {
        var config = new
        {
            metrics = GetAvailableMetrics(),
            groupByOptions = GetGroupByOptions(),
            documentTypes = GetDocumentTypes(),
            propertySubTypes = GetPropertySubTypes(),
            vehicleSubTypes = GetVehicleSubTypes(),
            bookTypes = GetBookTypes()
        };
        return Ok(config);
    }

    /// <summary>
    /// Generate dynamic report based on request parameters
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport([FromBody] SecuritiesReportRequest request)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);
            var query = _context.SecuritiesDistributions
                .Where(x => x.Status == true)
                .AsQueryable();

            // Apply filters
            query = ApplyFilters(query, request);

            // Get all matching records
            var records = await query.ToListAsync();

            // Generate response based on grouping
            var response = new SecuritiesReportResponse
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
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.SecuritiesDistributions
            .Where(x => x.Status == true)
            .AsQueryable();

        // Apply date filters
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

        var records = await query.ToListAsync();
        var summary = CalculateSummary(records);

        return Ok(summary);
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
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.SecuritiesDistributions
            .Where(x => x.Status == true)
            .AsQueryable();

        query = ApplyDateFilter(query, startDate, endDate, calendar);

        var grouped = await query
            .GroupBy(x => new { x.TransactionGuideName, x.LicenseNumber })
            .Select(g => new SecuritiesReportRow
            {
                TransactionGuideName = g.Key.TransactionGuideName,
                LicenseNumber = g.Key.LicenseNumber,
                RecordCount = g.Count(),
                PropertySaleCount = g.Sum(x => x.PropertySaleCount ?? 0),
                BayWafaCount = g.Sum(x => x.BayWafaCount ?? 0),
                RentCount = g.Sum(x => x.RentCount ?? 0),
                VehicleSaleCount = g.Sum(x => x.VehicleSaleCount ?? 0),
                VehicleExchangeCount = g.Sum(x => x.VehicleExchangeCount ?? 0),
                RegistrationBookCount = g.Sum(x => x.RegistrationBookCount ?? 0),
                DuplicateBookCount = g.Sum(x => x.DuplicateBookCount ?? 0),
                TotalDocumentsPrice = g.Sum(x => x.TotalDocumentsPrice ?? 0),
                RegistrationBookPrice = g.Sum(x => x.RegistrationBookPrice ?? 0),
                TotalSecuritiesPrice = g.Sum(x => x.TotalSecuritiesPrice ?? 0)
            })
            .OrderByDescending(x => x.TotalSecuritiesPrice)
            .ToListAsync();

        // Calculate totals
        foreach (var row in grouped)
        {
            row.TotalDocuments = row.PropertySaleCount + row.BayWafaCount + row.RentCount +
                                 row.VehicleSaleCount + row.VehicleExchangeCount;
            row.TotalBooks = row.RegistrationBookCount + row.DuplicateBookCount;
        }

        return Ok(new { data = grouped, summary = CalculateSummaryFromRows(grouped) });
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
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.SecuritiesDistributions
            .Where(x => x.Status == true)
            .AsQueryable();

        query = ApplyDateFilter(query, startDate, endDate, calendar);
        var records = await query.ToListAsync();

        var result = new List<object>
        {
            new {
                documentType = 1,
                documentTypeName = "سته‌های معاملات جایداد",
                subTypes = new[] {
                    new { subType = 1, name = "سته خرید و فروش جایداد", count = records.Sum(x => x.PropertySaleCount ?? 0), amount = records.Sum(x => (x.PropertySaleCount ?? 0) * (x.PricePerDocument ?? 0)) },
                    new { subType = 2, name = "سته بیع وفا", count = records.Sum(x => x.BayWafaCount ?? 0), amount = records.Sum(x => (x.BayWafaCount ?? 0) * (x.PricePerDocument ?? 0)) },
                    new { subType = 3, name = "سته کرایی", count = records.Sum(x => x.RentCount ?? 0), amount = records.Sum(x => (x.RentCount ?? 0) * (x.PricePerDocument ?? 0)) }
                }
            },
            new {
                documentType = 2,
                documentTypeName = "سته‌های معاملات وسایط نقلیه",
                subTypes = new[] {
                    new { subType = 1, name = "سته خرید و فروش وسایط نقلیه", count = records.Sum(x => x.VehicleSaleCount ?? 0), amount = records.Sum(x => (x.VehicleSaleCount ?? 0) * (x.PricePerDocument ?? 0)) },
                    new { subType = 2, name = "سته تبادله", count = records.Sum(x => x.VehicleExchangeCount ?? 0), amount = records.Sum(x => (x.VehicleExchangeCount ?? 0) * (x.PricePerDocument ?? 0)) }
                }
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Get report grouped by registration book type
    /// </summary>
    [HttpGet("by-book-type")]
    public async Task<IActionResult> GetByBookType(
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.SecuritiesDistributions
            .Where(x => x.Status == true)
            .AsQueryable();

        query = ApplyDateFilter(query, startDate, endDate, calendar);
        var records = await query.ToListAsync();

        var result = new
        {
            newBooks = new
            {
                type = 1,
                name = "کتاب ثبت",
                count = records.Sum(x => x.RegistrationBookCount ?? 0),
                amount = records.Where(x => x.RegistrationBookType == 1).Sum(x => x.RegistrationBookPrice ?? 0)
            },
            duplicateBooks = new
            {
                type = 2,
                name = "کتاب ثبت مثنی",
                count = records.Sum(x => x.DuplicateBookCount ?? 0),
                amount = records.Where(x => x.RegistrationBookType == 2).Sum(x => x.RegistrationBookPrice ?? 0)
            },
            totalBooks = new
            {
                count = records.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
                amount = records.Sum(x => x.RegistrationBookPrice ?? 0)
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// Get monthly trend report
    /// </summary>
    [HttpGet("monthly-trend")]
    public async Task<IActionResult> GetMonthlyTrend(
        [FromQuery] int? year = null,
        [FromQuery] string? calendarType = null)
    {
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.SecuritiesDistributions
            .Where(x => x.Status == true && x.DistributionDate.HasValue)
            .AsQueryable();

        if (year.HasValue)
        {
            query = query.Where(x => x.DistributionDate!.Value.Year == year.Value);
        }

        var records = await query.ToListAsync();

        var monthlyData = records
            .GroupBy(x => new { x.DistributionDate!.Value.Year, x.DistributionDate!.Value.Month })
            .Select(g => new
            {
                year = g.Key.Year,
                month = g.Key.Month,
                monthName = GetMonthName(g.Key.Month, calendar),
                recordCount = g.Count(),
                totalDocuments = g.Sum(x => (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                                           (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0)),
                totalBooks = g.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
                totalAmount = g.Sum(x => x.TotalSecuritiesPrice ?? 0)
            })
            .OrderBy(x => x.year)
            .ThenBy(x => x.month)
            .ToList();

        return Ok(monthlyData);
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
        var calendar = DateConversionHelper.ParseCalendarType(calendarType);
        var query = _context.SecuritiesDistributions
            .Where(x => x.Status == true)
            .AsQueryable();

        if (!string.IsNullOrEmpty(licenseNumber))
            query = query.Where(x => x.LicenseNumber.Contains(licenseNumber));
        if (!string.IsNullOrEmpty(transactionGuideName))
            query = query.Where(x => x.TransactionGuideName.Contains(transactionGuideName));

        var records = await query
            .OrderBy(x => x.TransactionGuideName)
            .ThenBy(x => x.DistributionDate)
            .Select(x => new
            {
                x.Id,
                x.RegistrationNumber,
                x.TransactionGuideName,
                x.LicenseNumber,
                distributionDate = x.DistributionDate.HasValue
                    ? DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar) : "",
                propertySale = new { count = x.PropertySaleCount, start = x.PropertySaleSerialStart, end = x.PropertySaleSerialEnd },
                bayWafa = new { count = x.BayWafaCount, start = x.BayWafaSerialStart, end = x.BayWafaSerialEnd },
                rent = new { count = x.RentCount, start = x.RentSerialStart, end = x.RentSerialEnd },
                vehicleSale = new { count = x.VehicleSaleCount, start = x.VehicleSaleSerialStart, end = x.VehicleSaleSerialEnd },
                vehicleExchange = new { count = x.VehicleExchangeCount, start = x.VehicleExchangeSerialStart, end = x.VehicleExchangeSerialEnd }
            })
            .ToListAsync();

        return Ok(records);
    }

    /// <summary>
    /// Export report data for PDF/Excel generation
    /// </summary>
    [HttpPost("export")]
    public async Task<IActionResult> ExportReport([FromBody] SecuritiesReportRequest request)
    {
        try
        {
            var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);
            var query = _context.SecuritiesDistributions
                .Where(x => x.Status == true)
                .AsQueryable();

            query = ApplyFilters(query, request);

            var records = await query
                .OrderBy(x => x.TransactionGuideName)
                .ThenByDescending(x => x.DistributionDate)
                .Select(x => new
                {
                    x.RegistrationNumber,
                    x.LicenseOwnerName,
                    x.LicenseOwnerFatherName,
                    x.TransactionGuideName,
                    x.LicenseNumber,
                    documentType = x.DocumentType == 1 ? "جایداد" : x.DocumentType == 2 ? "وسایط نقلیه" : "-",
                    x.PropertySaleCount,
                    propertySaleSerial = FormatSerialRange(x.PropertySaleSerialStart, x.PropertySaleSerialEnd),
                    x.BayWafaCount,
                    bayWafaSerial = FormatSerialRange(x.BayWafaSerialStart, x.BayWafaSerialEnd),
                    x.RentCount,
                    rentSerial = FormatSerialRange(x.RentSerialStart, x.RentSerialEnd),
                    x.VehicleSaleCount,
                    vehicleSaleSerial = FormatSerialRange(x.VehicleSaleSerialStart, x.VehicleSaleSerialEnd),
                    x.VehicleExchangeCount,
                    vehicleExchangeSerial = FormatSerialRange(x.VehicleExchangeSerialStart, x.VehicleExchangeSerialEnd),
                    x.RegistrationBookCount,
                    x.DuplicateBookCount,
                    x.PricePerDocument,
                    x.TotalDocumentsPrice,
                    x.RegistrationBookPrice,
                    x.TotalSecuritiesPrice,
                    x.BankReceiptNumber,
                    deliveryDate = x.DeliveryDate.HasValue ? DateConversionHelper.FormatDateOnly(x.DeliveryDate, calendar) : "",
                    distributionDate = x.DistributionDate.HasValue ? DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar) : ""
                })
                .ToListAsync();

            return Ok(new
            {
                data = records,
                summary = CalculateSummary(await query.ToListAsync()),
                metadata = GenerateMetadata(request)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "خطا در صادرات گزارش", error = ex.Message });
        }
    }

    #region Private Helper Methods

    private IQueryable<SecuritiesDistribution> ApplyFilters(IQueryable<SecuritiesDistribution> query, SecuritiesReportRequest request)
    {
        var calendar = DateConversionHelper.ParseCalendarType(request.CalendarType);

        // Date filters
        if (!string.IsNullOrEmpty(request.StartDate))
        {
            var start = DateConversionHelper.ParseDateOnly(request.StartDate, calendar);
            if (start.HasValue)
                query = query.Where(x => x.DistributionDate >= start.Value);
        }
        if (!string.IsNullOrEmpty(request.EndDate))
        {
            var end = DateConversionHelper.ParseDateOnly(request.EndDate, calendar);
            if (end.HasValue)
                query = query.Where(x => x.DistributionDate <= end.Value);
        }

        // Entity filters
        if (!string.IsNullOrEmpty(request.TransactionGuideName))
            query = query.Where(x => x.TransactionGuideName.Contains(request.TransactionGuideName));
        if (!string.IsNullOrEmpty(request.LicenseNumber))
            query = query.Where(x => x.LicenseNumber.Contains(request.LicenseNumber));
        if (!string.IsNullOrEmpty(request.RegistrationNumber))
            query = query.Where(x => x.RegistrationNumber.Contains(request.RegistrationNumber));
        if (request.DocumentType.HasValue)
            query = query.Where(x => x.DocumentType == request.DocumentType.Value);
        if (request.RegistrationBookType.HasValue)
            query = query.Where(x => x.RegistrationBookType == request.RegistrationBookType.Value);

        // Value filters
        if (request.MinAmount.HasValue)
            query = query.Where(x => x.TotalSecuritiesPrice >= request.MinAmount.Value);
        if (request.MaxAmount.HasValue)
            query = query.Where(x => x.TotalSecuritiesPrice <= request.MaxAmount.Value);

        return query;
    }

    private IQueryable<SecuritiesDistribution> ApplyDateFilter(
        IQueryable<SecuritiesDistribution> query,
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

    private SecuritiesReportSummary CalculateSummary(List<SecuritiesDistribution> records)
    {
        return new SecuritiesReportSummary
        {
            TotalRecords = records.Count,
            PropertySaleCount = records.Sum(x => x.PropertySaleCount ?? 0),
            BayWafaCount = records.Sum(x => x.BayWafaCount ?? 0),
            RentCount = records.Sum(x => x.RentCount ?? 0),
            VehicleSaleCount = records.Sum(x => x.VehicleSaleCount ?? 0),
            VehicleExchangeCount = records.Sum(x => x.VehicleExchangeCount ?? 0),
            RegistrationBookCount = records.Sum(x => x.RegistrationBookCount ?? 0),
            DuplicateBookCount = records.Sum(x => x.DuplicateBookCount ?? 0),
            TotalPropertyDocuments = records.Sum(x => (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0)),
            TotalVehicleDocuments = records.Sum(x => (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0)),
            TotalDocuments = records.Sum(x => (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                                              (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0)),
            TotalBookCount = records.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
            TotalDocumentsPrice = records.Sum(x => x.TotalDocumentsPrice ?? 0),
            TotalRegistrationBookPrice = records.Sum(x => x.RegistrationBookPrice ?? 0),
            TotalSecuritiesPrice = records.Sum(x => x.TotalSecuritiesPrice ?? 0)
        };
    }

    private SecuritiesReportSummary CalculateSummaryFromRows(List<SecuritiesReportRow> rows)
    {
        return new SecuritiesReportSummary
        {
            TotalRecords = rows.Sum(x => x.RecordCount),
            PropertySaleCount = rows.Sum(x => x.PropertySaleCount),
            BayWafaCount = rows.Sum(x => x.BayWafaCount),
            RentCount = rows.Sum(x => x.RentCount),
            VehicleSaleCount = rows.Sum(x => x.VehicleSaleCount),
            VehicleExchangeCount = rows.Sum(x => x.VehicleExchangeCount),
            RegistrationBookCount = rows.Sum(x => x.RegistrationBookCount),
            DuplicateBookCount = rows.Sum(x => x.DuplicateBookCount),
            TotalPropertyDocuments = rows.Sum(x => x.PropertySaleCount + x.BayWafaCount + x.RentCount),
            TotalVehicleDocuments = rows.Sum(x => x.VehicleSaleCount + x.VehicleExchangeCount),
            TotalDocuments = rows.Sum(x => x.TotalDocuments),
            TotalBookCount = rows.Sum(x => x.TotalBooks),
            TotalDocumentsPrice = rows.Sum(x => x.TotalDocumentsPrice),
            TotalRegistrationBookPrice = rows.Sum(x => x.RegistrationBookPrice),
            TotalSecuritiesPrice = rows.Sum(x => x.TotalSecuritiesPrice)
        };
    }

    private List<SecuritiesReportRow> GenerateReportData(
        List<SecuritiesDistribution> records,
        SecuritiesReportRequest request,
        CalendarType calendar)
    {
        if (request.GroupBy == null || request.GroupBy.Count == 0)
        {
            // No grouping - return individual records
            return records.Select(x => new SecuritiesReportRow
            {
                RegistrationNumber = x.RegistrationNumber,
                TransactionGuideName = x.TransactionGuideName,
                LicenseNumber = x.LicenseNumber,
                DocumentType = x.DocumentType,
                DocumentTypeName = x.DocumentType == 1 ? "جایداد" : x.DocumentType == 2 ? "وسایط نقلیه" : "-",
                RecordCount = 1,
                PropertySaleCount = x.PropertySaleCount ?? 0,
                BayWafaCount = x.BayWafaCount ?? 0,
                RentCount = x.RentCount ?? 0,
                VehicleSaleCount = x.VehicleSaleCount ?? 0,
                VehicleExchangeCount = x.VehicleExchangeCount ?? 0,
                RegistrationBookCount = x.RegistrationBookCount ?? 0,
                DuplicateBookCount = x.DuplicateBookCount ?? 0,
                TotalDocuments = (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                                 (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0),
                TotalBooks = (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0),
                TotalDocumentsPrice = x.TotalDocumentsPrice ?? 0,
                RegistrationBookPrice = x.RegistrationBookPrice ?? 0,
                TotalSecuritiesPrice = x.TotalSecuritiesPrice ?? 0,
                PropertySaleSerialRange = FormatSerialRange(x.PropertySaleSerialStart, x.PropertySaleSerialEnd),
                BayWafaSerialRange = FormatSerialRange(x.BayWafaSerialStart, x.BayWafaSerialEnd),
                RentSerialRange = FormatSerialRange(x.RentSerialStart, x.RentSerialEnd),
                VehicleSaleSerialRange = FormatSerialRange(x.VehicleSaleSerialStart, x.VehicleSaleSerialEnd),
                VehicleExchangeSerialRange = FormatSerialRange(x.VehicleExchangeSerialStart, x.VehicleExchangeSerialEnd),
                DistributionDate = x.DistributionDate.HasValue ? DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar) : "",
                DeliveryDate = x.DeliveryDate.HasValue ? DateConversionHelper.FormatDateOnly(x.DeliveryDate, calendar) : ""
            }).ToList();
        }

        // Group by first grouping option
        var groupBy = request.GroupBy[0];
        return groupBy switch
        {
            ReportGroupBy.TransactionGuide => GroupByTransactionGuide(records),
            ReportGroupBy.LicenseNumber => GroupByLicenseNumber(records),
            ReportGroupBy.DocumentType => GroupByDocumentType(records),
            ReportGroupBy.RegistrationBookType => GroupByBookType(records),
            _ => records.Select(x => MapToReportRow(x, calendar)).ToList()
        };
    }

    private List<SecuritiesReportRow> GroupByTransactionGuide(List<SecuritiesDistribution> records)
    {
        return records
            .GroupBy(x => new { x.TransactionGuideName, x.LicenseNumber })
            .Select(g => new SecuritiesReportRow
            {
                GroupKey = g.Key.TransactionGuideName,
                TransactionGuideName = g.Key.TransactionGuideName,
                LicenseNumber = g.Key.LicenseNumber,
                RecordCount = g.Count(),
                PropertySaleCount = g.Sum(x => x.PropertySaleCount ?? 0),
                BayWafaCount = g.Sum(x => x.BayWafaCount ?? 0),
                RentCount = g.Sum(x => x.RentCount ?? 0),
                VehicleSaleCount = g.Sum(x => x.VehicleSaleCount ?? 0),
                VehicleExchangeCount = g.Sum(x => x.VehicleExchangeCount ?? 0),
                RegistrationBookCount = g.Sum(x => x.RegistrationBookCount ?? 0),
                DuplicateBookCount = g.Sum(x => x.DuplicateBookCount ?? 0),
                TotalDocuments = g.Sum(x => (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                                           (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0)),
                TotalBooks = g.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
                TotalDocumentsPrice = g.Sum(x => x.TotalDocumentsPrice ?? 0),
                RegistrationBookPrice = g.Sum(x => x.RegistrationBookPrice ?? 0),
                TotalSecuritiesPrice = g.Sum(x => x.TotalSecuritiesPrice ?? 0)
            })
            .OrderByDescending(x => x.TotalSecuritiesPrice)
            .ToList();
    }

    private List<SecuritiesReportRow> GroupByLicenseNumber(List<SecuritiesDistribution> records)
    {
        return records
            .GroupBy(x => x.LicenseNumber)
            .Select(g => new SecuritiesReportRow
            {
                GroupKey = g.Key,
                LicenseNumber = g.Key,
                TransactionGuideName = g.First().TransactionGuideName,
                RecordCount = g.Count(),
                PropertySaleCount = g.Sum(x => x.PropertySaleCount ?? 0),
                BayWafaCount = g.Sum(x => x.BayWafaCount ?? 0),
                RentCount = g.Sum(x => x.RentCount ?? 0),
                VehicleSaleCount = g.Sum(x => x.VehicleSaleCount ?? 0),
                VehicleExchangeCount = g.Sum(x => x.VehicleExchangeCount ?? 0),
                RegistrationBookCount = g.Sum(x => x.RegistrationBookCount ?? 0),
                DuplicateBookCount = g.Sum(x => x.DuplicateBookCount ?? 0),
                TotalDocuments = g.Sum(x => (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                                           (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0)),
                TotalBooks = g.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
                TotalDocumentsPrice = g.Sum(x => x.TotalDocumentsPrice ?? 0),
                RegistrationBookPrice = g.Sum(x => x.RegistrationBookPrice ?? 0),
                TotalSecuritiesPrice = g.Sum(x => x.TotalSecuritiesPrice ?? 0)
            })
            .OrderByDescending(x => x.TotalSecuritiesPrice)
            .ToList();
    }

    private List<SecuritiesReportRow> GroupByDocumentType(List<SecuritiesDistribution> records)
    {
        return records
            .GroupBy(x => x.DocumentType)
            .Select(g => new SecuritiesReportRow
            {
                GroupKey = g.Key?.ToString() ?? "0",
                DocumentType = g.Key,
                DocumentTypeName = g.Key == 1 ? "سته‌های معاملات جایداد" : g.Key == 2 ? "سته‌های معاملات وسایط نقلیه" : "نامشخص",
                RecordCount = g.Count(),
                PropertySaleCount = g.Sum(x => x.PropertySaleCount ?? 0),
                BayWafaCount = g.Sum(x => x.BayWafaCount ?? 0),
                RentCount = g.Sum(x => x.RentCount ?? 0),
                VehicleSaleCount = g.Sum(x => x.VehicleSaleCount ?? 0),
                VehicleExchangeCount = g.Sum(x => x.VehicleExchangeCount ?? 0),
                RegistrationBookCount = g.Sum(x => x.RegistrationBookCount ?? 0),
                DuplicateBookCount = g.Sum(x => x.DuplicateBookCount ?? 0),
                TotalDocuments = g.Sum(x => (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                                           (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0)),
                TotalBooks = g.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
                TotalDocumentsPrice = g.Sum(x => x.TotalDocumentsPrice ?? 0),
                RegistrationBookPrice = g.Sum(x => x.RegistrationBookPrice ?? 0),
                TotalSecuritiesPrice = g.Sum(x => x.TotalSecuritiesPrice ?? 0)
            })
            .OrderByDescending(x => x.TotalSecuritiesPrice)
            .ToList();
    }

    private List<SecuritiesReportRow> GroupByBookType(List<SecuritiesDistribution> records)
    {
        return records
            .GroupBy(x => x.RegistrationBookType)
            .Select(g => new SecuritiesReportRow
            {
                GroupKey = g.Key?.ToString() ?? "0",
                RegistrationBookType = g.Key,
                RegistrationBookTypeName = g.Key == 1 ? "کتاب ثبت" : g.Key == 2 ? "کتاب ثبت مثنی" : "نامشخص",
                RecordCount = g.Count(),
                RegistrationBookCount = g.Sum(x => x.RegistrationBookCount ?? 0),
                DuplicateBookCount = g.Sum(x => x.DuplicateBookCount ?? 0),
                TotalBooks = g.Sum(x => (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0)),
                RegistrationBookPrice = g.Sum(x => x.RegistrationBookPrice ?? 0),
                TotalSecuritiesPrice = g.Sum(x => x.TotalSecuritiesPrice ?? 0)
            })
            .OrderByDescending(x => x.TotalSecuritiesPrice)
            .ToList();
    }

    private SecuritiesReportRow MapToReportRow(SecuritiesDistribution x, CalendarType calendar)
    {
        return new SecuritiesReportRow
        {
            RegistrationNumber = x.RegistrationNumber,
            TransactionGuideName = x.TransactionGuideName,
            LicenseNumber = x.LicenseNumber,
            DocumentType = x.DocumentType,
            DocumentTypeName = x.DocumentType == 1 ? "جایداد" : x.DocumentType == 2 ? "وسایط نقلیه" : "-",
            RecordCount = 1,
            PropertySaleCount = x.PropertySaleCount ?? 0,
            BayWafaCount = x.BayWafaCount ?? 0,
            RentCount = x.RentCount ?? 0,
            VehicleSaleCount = x.VehicleSaleCount ?? 0,
            VehicleExchangeCount = x.VehicleExchangeCount ?? 0,
            RegistrationBookCount = x.RegistrationBookCount ?? 0,
            DuplicateBookCount = x.DuplicateBookCount ?? 0,
            TotalDocuments = (x.PropertySaleCount ?? 0) + (x.BayWafaCount ?? 0) + (x.RentCount ?? 0) +
                             (x.VehicleSaleCount ?? 0) + (x.VehicleExchangeCount ?? 0),
            TotalBooks = (x.RegistrationBookCount ?? 0) + (x.DuplicateBookCount ?? 0),
            TotalDocumentsPrice = x.TotalDocumentsPrice ?? 0,
            RegistrationBookPrice = x.RegistrationBookPrice ?? 0,
            TotalSecuritiesPrice = x.TotalSecuritiesPrice ?? 0,
            DistributionDate = x.DistributionDate.HasValue ? DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar) : "",
            DeliveryDate = x.DeliveryDate.HasValue ? DateConversionHelper.FormatDateOnly(x.DeliveryDate, calendar) : ""
        };
    }

    private SecuritiesReportMetadata GenerateMetadata(SecuritiesReportRequest request)
    {
        var filters = new List<string>();
        if (!string.IsNullOrEmpty(request.StartDate)) filters.Add($"از تاریخ: {request.StartDate}");
        if (!string.IsNullOrEmpty(request.EndDate)) filters.Add($"تا تاریخ: {request.EndDate}");
        if (!string.IsNullOrEmpty(request.TransactionGuideName)) filters.Add($"رهنمای معاملات: {request.TransactionGuideName}");
        if (!string.IsNullOrEmpty(request.LicenseNumber)) filters.Add($"نمبر جواز: {request.LicenseNumber}");
        if (request.DocumentType.HasValue) filters.Add($"نوع سند: {(request.DocumentType == 1 ? "جایداد" : "وسایط نقلیه")}");

        return new SecuritiesReportMetadata
        {
            GeneratedAt = DateTime.UtcNow,
            ReportTitle = "گزارش اسناد بهادار رهنمای معاملات",
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
            new { id = "total_documents", name = "تعداد مجموعی سته‌ها", category = "quantitative" },
            new { id = "property_sale_count", name = "تعداد سته خرید و فروش جایداد", category = "quantitative" },
            new { id = "bay_wafa_count", name = "تعداد سته بیع وفا", category = "quantitative" },
            new { id = "rent_count", name = "تعداد سته کرایی", category = "quantitative" },
            new { id = "vehicle_sale_count", name = "تعداد سته خرید و فروش وسایط نقلیه", category = "quantitative" },
            new { id = "vehicle_exchange_count", name = "تعداد سته تبادله", category = "quantitative" },
            new { id = "registration_book_count", name = "تعداد کتاب ثبت", category = "quantitative" },
            new { id = "duplicate_book_count", name = "تعداد کتاب ثبت مثنی", category = "quantitative" },
            new { id = "total_book_count", name = "تعداد مجموعی کتاب ثبت", category = "quantitative" },
            new { id = "total_documents_price", name = "مبلغ پول مجموعه سته‌ها", category = "financial" },
            new { id = "registration_book_price", name = "مبلغ پول کتاب ثبت", category = "financial" },
            new { id = "total_securities_price", name = "مبلغ پول مجموع اسناد بهادار", category = "financial" },
            new { id = "serial_numbers", name = "سریال نمبرها", category = "identity" }
        };
    }

    private static List<object> GetGroupByOptions()
    {
        return new List<object>
        {
            new { id = "transaction_guide", name = "رهنمای معاملات" },
            new { id = "license_number", name = "نمبر جواز" },
            new { id = "document_type", name = "نوع سته" },
            new { id = "book_type", name = "نوع کتاب ثبت" },
            new { id = "month", name = "ماه" },
            new { id = "year", name = "سال" }
        };
    }

    private static List<object> GetDocumentTypes()
    {
        return new List<object>
        {
            new { id = 1, name = "سته‌های معاملات جایداد" },
            new { id = 2, name = "سته‌های معاملات وسایط نقلیه" }
        };
    }

    private static List<object> GetPropertySubTypes()
    {
        return new List<object>
        {
            new { id = 1, name = "سته خرید و فروش جایداد" },
            new { id = 2, name = "سته بیع وفا" },
            new { id = 3, name = "سته کرایی" }
        };
    }

    private static List<object> GetVehicleSubTypes()
    {
        return new List<object>
        {
            new { id = 1, name = "سته خرید و فروش وسایط نقلیه" },
            new { id = 2, name = "سته تبادله" }
        };
    }

    private static List<object> GetBookTypes()
    {
        return new List<object>
        {
            new { id = 1, name = "کتاب ثبت" },
            new { id = 2, name = "کتاب ثبت مثنی" }
        };
    }

    #endregion
}
