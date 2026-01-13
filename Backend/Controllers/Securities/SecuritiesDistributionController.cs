using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

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
                        x.DocumentType,
                        x.PropertySubType,
                        x.VehicleSubType,
                        x.PropertySaleCount,
                        x.PropertySaleSerialStart,
                        x.PropertySaleSerialEnd,
                        x.BayWafaCount,
                        x.BayWafaSerialStart,
                        x.BayWafaSerialEnd,
                        x.RentCount,
                        x.RentSerialStart,
                        x.RentSerialEnd,
                        x.VehicleSaleCount,
                        x.VehicleSaleSerialStart,
                        x.VehicleSaleSerialEnd,
                        x.VehicleExchangeCount,
                        x.VehicleExchangeSerialStart,
                        x.VehicleExchangeSerialEnd,
                        x.RegistrationBookType,
                        x.RegistrationBookCount,
                        x.DuplicateBookCount,
                        x.PricePerDocument,
                        x.TotalDocumentsPrice,
                        x.RegistrationBookPrice,
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
                return StatusCode(500, new { message = "خطا در دریافت اطلاعات", error = ex.Message });
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
                    item.DocumentType,
                    item.PropertySubType,
                    item.VehicleSubType,
                    item.PropertySaleCount,
                    item.PropertySaleSerialStart,
                    item.PropertySaleSerialEnd,
                    item.BayWafaCount,
                    item.BayWafaSerialStart,
                    item.BayWafaSerialEnd,
                    item.RentCount,
                    item.RentSerialStart,
                    item.RentSerialEnd,
                    item.VehicleSaleCount,
                    item.VehicleSaleSerialStart,
                    item.VehicleSaleSerialEnd,
                    item.VehicleExchangeCount,
                    item.VehicleExchangeSerialStart,
                    item.VehicleExchangeSerialEnd,
                    item.RegistrationBookType,
                    item.RegistrationBookCount,
                    item.DuplicateBookCount,
                    item.PricePerDocument,
                    item.TotalDocumentsPrice,
                    item.RegistrationBookPrice,
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

                // Validate conditional fields
                var validationResult = ValidateConditionalFields(data);
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
                    DocumentType = data.DocumentType,
                    PropertySubType = data.PropertySubType,
                    VehicleSubType = data.VehicleSubType,
                    PropertySaleCount = data.PropertySaleCount,
                    PropertySaleSerialStart = data.PropertySaleSerialStart,
                    PropertySaleSerialEnd = data.PropertySaleSerialEnd,
                    BayWafaCount = data.BayWafaCount,
                    BayWafaSerialStart = data.BayWafaSerialStart,
                    BayWafaSerialEnd = data.BayWafaSerialEnd,
                    RentCount = data.RentCount,
                    RentSerialStart = data.RentSerialStart,
                    RentSerialEnd = data.RentSerialEnd,
                    VehicleSaleCount = data.VehicleSaleCount,
                    VehicleSaleSerialStart = data.VehicleSaleSerialStart,
                    VehicleSaleSerialEnd = data.VehicleSaleSerialEnd,
                    VehicleExchangeCount = data.VehicleExchangeCount,
                    VehicleExchangeSerialStart = data.VehicleExchangeSerialStart,
                    VehicleExchangeSerialEnd = data.VehicleExchangeSerialEnd,
                    RegistrationBookType = data.RegistrationBookType,
                    RegistrationBookCount = data.RegistrationBookCount,
                    DuplicateBookCount = data.DuplicateBookCount,
                    PricePerDocument = data.PricePerDocument,
                    TotalDocumentsPrice = data.TotalDocumentsPrice,
                    RegistrationBookPrice = data.RegistrationBookPrice,
                    TotalSecuritiesPrice = data.TotalSecuritiesPrice,
                    BankReceiptNumber = data.BankReceiptNumber,
                    DeliveryDate = data.DeliveryDate,
                    DistributionDate = data.DistributionDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    Status = true
                };

                _context.SecuritiesDistributions.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت اطلاعات", error = ex.Message });
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

                var entity = await _context.SecuritiesDistributions.FindAsync(id);
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

                // Validate conditional fields
                var validationResult = ValidateConditionalFields(data);
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
                entity.DocumentType = data.DocumentType;
                entity.PropertySubType = data.PropertySubType;
                entity.VehicleSubType = data.VehicleSubType;
                entity.PropertySaleCount = data.PropertySaleCount;
                entity.PropertySaleSerialStart = data.PropertySaleSerialStart;
                entity.PropertySaleSerialEnd = data.PropertySaleSerialEnd;
                entity.BayWafaCount = data.BayWafaCount;
                entity.BayWafaSerialStart = data.BayWafaSerialStart;
                entity.BayWafaSerialEnd = data.BayWafaSerialEnd;
                entity.RentCount = data.RentCount;
                entity.RentSerialStart = data.RentSerialStart;
                entity.RentSerialEnd = data.RentSerialEnd;
                entity.VehicleSaleCount = data.VehicleSaleCount;
                entity.VehicleSaleSerialStart = data.VehicleSaleSerialStart;
                entity.VehicleSaleSerialEnd = data.VehicleSaleSerialEnd;
                entity.VehicleExchangeCount = data.VehicleExchangeCount;
                entity.VehicleExchangeSerialStart = data.VehicleExchangeSerialStart;
                entity.VehicleExchangeSerialEnd = data.VehicleExchangeSerialEnd;
                entity.RegistrationBookType = data.RegistrationBookType;
                entity.RegistrationBookCount = data.RegistrationBookCount;
                entity.DuplicateBookCount = data.DuplicateBookCount;
                entity.PricePerDocument = data.PricePerDocument;
                entity.TotalDocumentsPrice = data.TotalDocumentsPrice;
                entity.RegistrationBookPrice = data.RegistrationBookPrice;
                entity.TotalSecuritiesPrice = data.TotalSecuritiesPrice;
                entity.BankReceiptNumber = data.BankReceiptNumber;
                entity.DeliveryDate = data.DeliveryDate;
                entity.DistributionDate = data.DistributionDate;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userName;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت بروزرسانی شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بروزرسانی اطلاعات", error = ex.Message });
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
        /// Validate conditional fields based on document type selection
        /// </summary>
        private (bool IsValid, string? ErrorMessage) ValidateConditionalFields(SecuritiesDistributionData data)
        {
            // If document type is Property (1)
            if (data.DocumentType == 1)
            {
                if (!data.PropertySubType.HasValue)
                {
                    return (false, "لطفاً نوع سته جایداد را انتخاب کنید");
                }

                // Clear vehicle fields
                data.VehicleSubType = null;
                data.VehicleSaleCount = null;
                data.VehicleSaleSerialStart = null;
                data.VehicleSaleSerialEnd = null;
                data.VehicleExchangeCount = null;
                data.VehicleExchangeSerialStart = null;
                data.VehicleExchangeSerialEnd = null;
            }
            // If document type is Vehicle (2)
            else if (data.DocumentType == 2)
            {
                if (!data.VehicleSubType.HasValue)
                {
                    return (false, "لطفاً نوع سته وسایط نقلیه را انتخاب کنید");
                }

                // Clear property fields
                data.PropertySubType = null;
                data.PropertySaleCount = null;
                data.PropertySaleSerialStart = null;
                data.PropertySaleSerialEnd = null;
                data.BayWafaCount = null;
                data.BayWafaSerialStart = null;
                data.BayWafaSerialEnd = null;
                data.RentCount = null;
                data.RentSerialStart = null;
                data.RentSerialEnd = null;
            }

            // Validate registration book type
            if (data.RegistrationBookType == 1 && !data.RegistrationBookCount.HasValue)
            {
                return (false, "لطفاً تعداد کتاب ثبت را وارد کنید");
            }
            if (data.RegistrationBookType == 2 && !data.DuplicateBookCount.HasValue)
            {
                return (false, "لطفاً تعداد کتاب ثبت مثنی را وارد کنید");
            }

            return (true, null);
        }
    }
}
