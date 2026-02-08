using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;

namespace WebAPIBackend.Controllers.Securities;

/// <summary>
/// Report Controller for Securities Distribution
/// گزارش‌گیری اسناد بهادار رهنمای معاملات
/// 
/// NOTE: This controller is temporarily disabled pending restructure to work with new Items-based data model.
/// The Securities module has been restructured to use dynamic Items collection instead of fixed fields.
/// Reports need to be redesigned to aggregate data from the SecuritiesDistributionItem table.
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
    /// Get basic statistics (temporary implementation)
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var totalRecords = await _context.SecuritiesDistributions
                .Where(x => x.Status == true)
                .CountAsync();

            var totalWithItems = await _context.SecuritiesDistributions
                .Include(x => x.Items)
                .Where(x => x.Status == true && x.Items.Any())
                .CountAsync();

            var totalItems = await _context.SecuritiesDistributionItems
                .CountAsync();

            var totalPrice = await _context.SecuritiesDistributions
                .Where(x => x.Status == true)
                .SumAsync(x => x.TotalSecuritiesPrice ?? 0);

            // Get item counts by document type
            var itemsByType = await _context.SecuritiesDistributionItems
                .GroupBy(x => x.DocumentType)
                .Select(g => new
                {
                    DocumentType = g.Key,
                    Count = g.Sum(x => x.Count),
                    TotalPrice = g.Sum(x => x.Price)
                })
                .ToListAsync();

            return Ok(new
            {
                totalDistributions = totalRecords,
                totalWithItems = totalWithItems,
                totalItems = totalItems,
                totalPrice = totalPrice,
                itemsByType = itemsByType.Select(x => new
                {
                    documentType = x.DocumentType,
                    documentTypeName = GetDocumentTypeName(x.DocumentType),
                    count = x.Count,
                    totalPrice = x.TotalPrice
                }),
                message = "گزارش‌گیری پیشرفته در حال توسعه است. فعلاً فقط آمار کلی در دسترس است."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "خطا در دریافت آمار", error = ex.Message });
        }
    }

    /// <summary>
    /// Get report configuration (placeholder)
    /// </summary>
    [HttpGet("config")]
    public IActionResult GetReportConfig()
    {
        return Ok(new
        {
            message = "گزارش‌گیری پیشرفته در حال توسعه است",
            availableReports = new[]
            {
                new { id = "summary", name = "آمار کلی", endpoint = "/api/SecuritiesReport/summary" }
            },
            documentTypes = new[]
            {
                new { id = 1, name = "سټه یی خرید و فروش", nameEn = "Property Sale" },
                new { id = 2, name = "سټه یی بیع وفا", nameEn = "Bay Wafa" },
                new { id = 3, name = "سټه یی کرایی", nameEn = "Rent" },
                new { id = 4, name = "سټه وسایط نقلیه", nameEn = "Vehicle" },
                new { id = 5, name = "کتاب ثبت", nameEn = "Registration Book" },
                new { id = 6, name = "کتاب ثبت مثنی", nameEn = "Duplicate Book" }
            }
        });
    }

    /// <summary>
    /// Generate report (placeholder)
    /// </summary>
    [HttpPost("generate")]
    public IActionResult GenerateReport([FromBody] object request)
    {
        return Ok(new
        {
            message = "گزارش‌گیری پیشرفته در حال توسعه است. لطفاً از /api/SecuritiesReport/summary استفاده کنید.",
            redirectTo = "/api/SecuritiesReport/summary"
        });
    }

    private string GetDocumentTypeName(int documentType)
    {
        return documentType switch
        {
            1 => "سټه یی خرید و فروش",
            2 => "سټه یی بیع وفا",
            3 => "سټه یی کرایی",
            4 => "سټه وسایط نقلیه",
            5 => "کتاب ثبت",
            6 => "کتاب ثبت مثنی",
            _ => "نامشخص"
        };
    }
}
