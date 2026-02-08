using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;
using WebAPIBackend.Models.RequestData.Securities;

namespace WebAPIBackend.Controllers.Securities
{
    /// <summary>
    /// Controller for Securities Distribution (اسناد بهادار رهنمای معاملات)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecuritiesDistributionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SecuritiesDistributionController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all securities distributions with pagination and search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var query = _context.SecuritiesDistributions
                    .Include(x => x.Items)
                    .Where(x => x.Status == true)
                    .AsQueryable();

                // Search by registration number, license number, or transaction guide name
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.RegistrationNumber.Contains(search) ||
                        x.LicenseNumber.Contains(search) ||
                        x.TransactionGuideName.Contains(search) ||
                        (x.BankReceiptNumber != null && x.BankReceiptNumber.Contains(search)));
                }

                var totalCount = await query.CountAsync();
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.RegistrationNumber,
                        x.LicenseOwnerName,
                        x.LicenseOwnerFatherName,
                        x.TransactionGuideName,
                        x.LicenseNumber,
                        Items = x.Items.Select(i => new
                        {
                            i.Id,
                            i.DocumentType,
                            i.SerialStart,
                            i.SerialEnd,
                            i.Count,
                            i.Price
                        }).ToList(),
                        x.PricePerDocument,
                        x.TotalDocumentsPrice,
                        x.TotalSecuritiesPrice,
                        x.BankReceiptNumber,
                        x.DeliveryDate,
                        DeliveryDateFormatted = x.DeliveryDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.DeliveryDate, calendar)
                            : "",
                        x.DistributionDate,
                        DistributionDateFormatted = x.DistributionDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.DistributionDate, calendar)
                            : "",
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Get securities distribution by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var item = await _context.SecuritiesDistributions
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                return Ok(new
                {
                    item.Id,
                    item.RegistrationNumber,
                    item.LicenseOwnerName,
                    item.LicenseOwnerFatherName,
                    item.TransactionGuideName,
                    item.LicenseNumber,
                    Items = item.Items.Select(i => new
                    {
                        i.Id,
                        i.DocumentType,
                        i.SerialStart,
                        i.SerialEnd,
                        i.Count,
                        i.Price
                    }).ToList(),
                    item.PricePerDocument,
                    item.TotalDocumentsPrice,
                    item.TotalSecuritiesPrice,
                    item.BankReceiptNumber,
                    item.DeliveryDate,
                    DeliveryDateFormatted = item.DeliveryDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(item.DeliveryDate, calendar)
                        : "",
                    item.DistributionDate,
                    DistributionDateFormatted = item.DistributionDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(item.DistributionDate, calendar)
                        : "",
                    item.CreatedAt,
                    item.CreatedBy,
                    item.UpdatedAt,
                    item.UpdatedBy,
                    item.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new securities distribution
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR")]
        public async Task<IActionResult> Create([FromBody] SecuritiesDistributionData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for unique registration number
                var exists = await _context.SecuritiesDistributions
                    .AnyAsync(x => x.RegistrationNumber == data.RegistrationNumber && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر ثبت قبلاً استفاده شده است" });
                }

                // Validate items
                var validationResult = ValidateItems(data.Items);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var userName = User.Identity?.Name ?? "System";

                var entity = new SecuritiesDistribution
                {
                    RegistrationNumber = data.RegistrationNumber,
                    LicenseOwnerName = data.LicenseOwnerName,
                    LicenseOwnerFatherName = data.LicenseOwnerFatherName,
                    TransactionGuideName = data.TransactionGuideName,
                    LicenseNumber = data.LicenseNumber,
                    PricePerDocument = data.PricePerDocument,
                    TotalDocumentsPrice = data.TotalDocumentsPrice,
                    TotalSecuritiesPrice = data.TotalSecuritiesPrice,
                    BankReceiptNumber = data.BankReceiptNumber,
                    DeliveryDate = data.DeliveryDate,
                    DistributionDate = data.DistributionDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    Status = true
                };

                // Add items
                foreach (var itemData in data.Items)
                {
                    var item = new WebAPIBackend.Models.Securities.SecuritiesDistributionItem
                    {
                        DocumentType = itemData.DocumentType,
                        SerialStart = itemData.SerialStart,
                        SerialEnd = itemData.SerialEnd,
                        Count = itemData.Count,
                        Price = itemData.Price,
                        CreatedAt = DateTime.UtcNow
                    };
                    entity.Items.Add(item);
                }

                _context.SecuritiesDistributions.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت اطلاعات", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Update securities distribution
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR")]
        public async Task<IActionResult> Update(int id, [FromBody] SecuritiesDistributionData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = await _context.SecuritiesDistributions
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                // Check for unique registration number (excluding current record)
                var exists = await _context.SecuritiesDistributions
                    .AnyAsync(x => x.RegistrationNumber == data.RegistrationNumber && x.Id != id && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "نمبر ثبت قبلاً استفاده شده است" });
                }

                // Validate items
                var validationResult = ValidateItems(data.Items);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var userName = User.Identity?.Name ?? "System";

                entity.RegistrationNumber = data.RegistrationNumber;
                entity.LicenseOwnerName = data.LicenseOwnerName;
                entity.LicenseOwnerFatherName = data.LicenseOwnerFatherName;
                entity.TransactionGuideName = data.TransactionGuideName;
                entity.LicenseNumber = data.LicenseNumber;
                entity.PricePerDocument = data.PricePerDocument;
                entity.TotalDocumentsPrice = data.TotalDocumentsPrice;
                entity.TotalSecuritiesPrice = data.TotalSecuritiesPrice;
                entity.BankReceiptNumber = data.BankReceiptNumber;
                entity.DeliveryDate = data.DeliveryDate;
                entity.DistributionDate = data.DistributionDate;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userName;

                // Update items - remove old ones and add new ones
                _context.SecuritiesDistributionItems.RemoveRange(entity.Items);
                entity.Items.Clear();

                foreach (var itemData in data.Items)
                {
                    var item = new WebAPIBackend.Models.Securities.SecuritiesDistributionItem
                    {
                        DocumentType = itemData.DocumentType,
                        SerialStart = itemData.SerialStart,
                        SerialEnd = itemData.SerialEnd,
                        Count = itemData.Count,
                        Price = itemData.Price,
                        CreatedAt = DateTime.UtcNow
                    };
                    entity.Items.Add(item);
                }

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت بروزرسانی شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بروزرسانی اطلاعات", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Delete (soft delete) securities distribution
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.SecuritiesDistributions.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                entity.Status = false;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = User.Identity?.Name ?? "System";

                await _context.SaveChangesAsync();

                return Ok(new { message = "رکورد با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate items collection
        /// </summary>
        private (bool IsValid, string? ErrorMessage) ValidateItems(List<SecuritiesDistributionItemData> items)
        {
            if (items == null || items.Count == 0)
            {
                return (false, "لطفاً حداقل یک سند اضافه کنید");
            }

            foreach (var item in items)
            {
                // Validate document type
                if (item.DocumentType < 1 || item.DocumentType > 6)
                {
                    return (false, "نوعیت سند نامعتبر است");
                }

                // For types 1-4, serial numbers are required
                if (item.DocumentType >= 1 && item.DocumentType <= 4)
                {
                    if (string.IsNullOrWhiteSpace(item.SerialStart) || string.IsNullOrWhiteSpace(item.SerialEnd))
                    {
                        return (false, "سریال نمبر آغاز و ختم برای این نوع سند الزامی است");
                    }

                    // Validate count calculation
                    if (item.Count <= 0)
                    {
                        return (false, "تعداد باید بیشتر از صفر باشد");
                    }
                }

                // For types 5-6, count is required
                if (item.DocumentType == 5 || item.DocumentType == 6)
                {
                    if (item.Count <= 0)
                    {
                        return (false, "تعداد برای کتاب ثبت الزامی است");
                    }
                }

                // Validate price
                if (item.Price < 0)
                {
                    return (false, "قیمت نمی‌تواند منفی باشد");
                }
            }

            return (true, null);
        }
    }
}
