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
    /// Controller for Securities Control (کنټرول ورودی و خروجی اسناد بهادار)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecuritiesControlController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SecuritiesControlController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all securities controls with pagination and search
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
                var query = _context.SecuritiesControls
                    .Where(x => x.Status == true)
                    .AsQueryable();

                // Search by serial number, proposal number, or distribution ticket number
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.SerialNumber.Contains(search) ||
                        (x.ProposalNumber != null && x.ProposalNumber.Contains(search)) ||
                        (x.DistributionTicketNumber != null && x.DistributionTicketNumber.Contains(search)) ||
                        (x.DistributionStartNumber != null && x.DistributionStartNumber.Contains(search)));
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
                        x.SerialNumber,
                        x.SecurityDocumentType,
                        SecurityDocumentTypeName = GetSecurityDocumentTypeName(x.SecurityDocumentType),
                        x.ProposalNumber,
                        x.ProposalDate,
                        ProposalDateFormatted = x.ProposalDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ProposalDate, calendar)
                            : "",
                        x.DistributionTicketNumber,
                        x.DeliveryDate,
                        DeliveryDateFormatted = x.DeliveryDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.DeliveryDate, calendar)
                            : "",
                        x.SecuritiesType,
                        SecuritiesTypeName = GetSecuritiesTypeName(x.SecuritiesType),
                        TotalDocumentsCount = CalculateTotalDocuments(x),
                        SerialRangeStart = GetSerialRangeStart(x),
                        SerialRangeEnd = GetSerialRangeEnd(x),
                        x.DistributionStartNumber,
                        x.DistributionEndNumber,
                        x.DistributedPersonsCount,
                        x.Remarks,
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
        /// Get securities control by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var item = await _context.SecuritiesControls
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                return Ok(new
                {
                    item.Id,
                    item.SerialNumber,
                    item.SecurityDocumentType,
                    SecurityDocumentTypeName = GetSecurityDocumentTypeName(item.SecurityDocumentType),
                    item.ProposalNumber,
                    item.ProposalDate,
                    ProposalDateFormatted = item.ProposalDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(item.ProposalDate, calendar)
                        : "",
                    item.DistributionTicketNumber,
                    item.DeliveryDate,
                    DeliveryDateFormatted = item.DeliveryDate.HasValue
                        ? DateConversionHelper.FormatDateOnly(item.DeliveryDate, calendar)
                        : "",
                    item.SecuritiesType,
                    SecuritiesTypeName = GetSecuritiesTypeName(item.SecuritiesType),
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
                    item.ExchangeCount,
                    item.ExchangeSerialStart,
                    item.ExchangeSerialEnd,
                    item.RegistrationBookCount,
                    item.RegistrationBookSerialStart,
                    item.RegistrationBookSerialEnd,
                    item.PrintedPetitionCount,
                    item.PrintedPetitionSerialStart,
                    item.PrintedPetitionSerialEnd,
                    item.DistributionStartNumber,
                    item.DistributionEndNumber,
                    item.DistributedPersonsCount,
                    item.Remarks,
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
        /// Create new securities control
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR")]
        public async Task<IActionResult> Create([FromBody] SecuritiesControlData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for unique serial number
                var exists = await _context.SecuritiesControls
                    .AnyAsync(x => x.SerialNumber == data.SerialNumber && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "شماره مسلسل قبلاً استفاده شده است" });
                }

                // Validate serial ranges
                var validationResult = ValidateSerialRanges(data);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var userName = User.Identity?.Name ?? "System";

                var entity = new SecuritiesControl
                {
                    SerialNumber = data.SerialNumber,
                    SecurityDocumentType = data.SecurityDocumentType,
                    ProposalNumber = data.ProposalNumber,
                    ProposalDate = data.ProposalDate,
                    DistributionTicketNumber = data.DistributionTicketNumber,
                    DeliveryDate = data.DeliveryDate,
                    SecuritiesType = data.SecuritiesType,
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
                    ExchangeCount = data.ExchangeCount,
                    ExchangeSerialStart = data.ExchangeSerialStart,
                    ExchangeSerialEnd = data.ExchangeSerialEnd,
                    RegistrationBookCount = data.RegistrationBookCount,
                    RegistrationBookSerialStart = data.RegistrationBookSerialStart,
                    RegistrationBookSerialEnd = data.RegistrationBookSerialEnd,
                    PrintedPetitionCount = data.PrintedPetitionCount,
                    PrintedPetitionSerialStart = data.PrintedPetitionSerialStart,
                    PrintedPetitionSerialEnd = data.PrintedPetitionSerialEnd,
                    DistributionStartNumber = data.DistributionStartNumber,
                    DistributionEndNumber = data.DistributionEndNumber,
                    DistributedPersonsCount = data.DistributedPersonsCount,
                    Remarks = data.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    Status = true
                };

                _context.SecuritiesControls.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "رکورد با موفقیت ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت اطلاعات", error = ex.Message });
            }
        }

        /// <summary>
        /// Update securities control
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,COMPANY_REGISTRAR,PROPERTY_OPERATOR")]
        public async Task<IActionResult> Update(int id, [FromBody] SecuritiesControlData data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = await _context.SecuritiesControls.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { message = "رکورد یافت نشد" });
                }

                // Check for unique serial number (excluding current record)
                var exists = await _context.SecuritiesControls
                    .AnyAsync(x => x.SerialNumber == data.SerialNumber && x.Id != id && x.Status == true);

                if (exists)
                {
                    return BadRequest(new { message = "شماره مسلسل قبلاً استفاده شده است" });
                }

                // Validate serial ranges
                var validationResult = ValidateSerialRanges(data);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var userName = User.Identity?.Name ?? "System";

                entity.SerialNumber = data.SerialNumber;
                entity.SecurityDocumentType = data.SecurityDocumentType;
                entity.ProposalNumber = data.ProposalNumber;
                entity.ProposalDate = data.ProposalDate;
                entity.DistributionTicketNumber = data.DistributionTicketNumber;
                entity.DeliveryDate = data.DeliveryDate;
                entity.SecuritiesType = data.SecuritiesType;
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
                entity.ExchangeCount = data.ExchangeCount;
                entity.ExchangeSerialStart = data.ExchangeSerialStart;
                entity.ExchangeSerialEnd = data.ExchangeSerialEnd;
                entity.RegistrationBookCount = data.RegistrationBookCount;
                entity.RegistrationBookSerialStart = data.RegistrationBookSerialStart;
                entity.RegistrationBookSerialEnd = data.RegistrationBookSerialEnd;
                entity.PrintedPetitionCount = data.PrintedPetitionCount;
                entity.PrintedPetitionSerialStart = data.PrintedPetitionSerialStart;
                entity.PrintedPetitionSerialEnd = data.PrintedPetitionSerialEnd;
                entity.DistributionStartNumber = data.DistributionStartNumber;
                entity.DistributionEndNumber = data.DistributionEndNumber;
                entity.DistributedPersonsCount = data.DistributedPersonsCount;
                entity.Remarks = data.Remarks;
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
        /// Delete (soft delete) securities control
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.SecuritiesControls.FindAsync(id);
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
        /// Generate next serial number
        /// </summary>
        [HttpGet("next-serial")]
        public async Task<IActionResult> GetNextSerialNumber()
        {
            try
            {
                var lastRecord = await _context.SecuritiesControls
                    .Where(x => x.Status == true)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;
                if (lastRecord != null && int.TryParse(lastRecord.SerialNumber, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }

                return Ok(new { serialNumber = nextNumber.ToString().PadLeft(6, '0') });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در تولید شماره مسلسل", error = ex.Message });
            }
        }

        #region Helper Methods

        private static string GetSecurityDocumentTypeName(int type)
        {
            return type switch
            {
                1 => "ستههای رهنمای معاملات",
                2 => "کتاب ثبت معاملات",
                3 => "عرایض مطبوع",
                _ => "نامشخص"
            };
        }

        private static string GetSecuritiesTypeName(int? type)
        {
            return type switch
            {
                1 => "ستههای خرید و فروش جایداد",
                2 => "ستههای بیع وفا",
                3 => "ستههای کرایی",
                4 => "ستههای خرید و فروش وسایط نقلیه",
                5 => "ستههای تبادله",
                6 => "کتاب ثبت معاملات",
                7 => "عرایض مطبوع",
                8 => "ستههای خرید و فروش جایداد و بیع وفا",
                9 => "ستههای خرید و فروش جایداد و کرایی",
                10 => "ستههای بیع وفا و کرایی",
                11 => "تمام انواع ستههای جایداد",
                _ => "-"
            };
        }

        private static int CalculateTotalDocuments(SecuritiesControl x)
        {
            int total = 0;
            total += x.PropertySaleCount ?? 0;
            total += x.BayWafaCount ?? 0;
            total += x.RentCount ?? 0;
            total += x.VehicleSaleCount ?? 0;
            total += x.ExchangeCount ?? 0;
            total += x.RegistrationBookCount ?? 0;
            total += x.PrintedPetitionCount ?? 0;
            return total;
        }

        private static string GetSerialRangeStart(SecuritiesControl x)
        {
            if (!string.IsNullOrEmpty(x.PropertySaleSerialStart)) return x.PropertySaleSerialStart;
            if (!string.IsNullOrEmpty(x.BayWafaSerialStart)) return x.BayWafaSerialStart;
            if (!string.IsNullOrEmpty(x.RentSerialStart)) return x.RentSerialStart;
            if (!string.IsNullOrEmpty(x.VehicleSaleSerialStart)) return x.VehicleSaleSerialStart;
            if (!string.IsNullOrEmpty(x.ExchangeSerialStart)) return x.ExchangeSerialStart;
            if (!string.IsNullOrEmpty(x.RegistrationBookSerialStart)) return x.RegistrationBookSerialStart;
            if (!string.IsNullOrEmpty(x.PrintedPetitionSerialStart)) return x.PrintedPetitionSerialStart;
            return "-";
        }

        private static string GetSerialRangeEnd(SecuritiesControl x)
        {
            if (!string.IsNullOrEmpty(x.PropertySaleSerialEnd)) return x.PropertySaleSerialEnd;
            if (!string.IsNullOrEmpty(x.BayWafaSerialEnd)) return x.BayWafaSerialEnd;
            if (!string.IsNullOrEmpty(x.RentSerialEnd)) return x.RentSerialEnd;
            if (!string.IsNullOrEmpty(x.VehicleSaleSerialEnd)) return x.VehicleSaleSerialEnd;
            if (!string.IsNullOrEmpty(x.ExchangeSerialEnd)) return x.ExchangeSerialEnd;
            if (!string.IsNullOrEmpty(x.RegistrationBookSerialEnd)) return x.RegistrationBookSerialEnd;
            if (!string.IsNullOrEmpty(x.PrintedPetitionSerialEnd)) return x.PrintedPetitionSerialEnd;
            return "-";
        }

        private static (bool IsValid, string? ErrorMessage) ValidateSerialRanges(SecuritiesControlData data)
        {
            // Validate that quantities match serial ranges where applicable
            if (data.PropertySaleCount.HasValue && data.PropertySaleCount > 0)
            {
                if (string.IsNullOrEmpty(data.PropertySaleSerialStart) || string.IsNullOrEmpty(data.PropertySaleSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای ستههای خرید و فروش جایداد را وارد کنید");
                }
            }

            if (data.BayWafaCount.HasValue && data.BayWafaCount > 0)
            {
                if (string.IsNullOrEmpty(data.BayWafaSerialStart) || string.IsNullOrEmpty(data.BayWafaSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای ستههای بیع وفا را وارد کنید");
                }
            }

            if (data.RentCount.HasValue && data.RentCount > 0)
            {
                if (string.IsNullOrEmpty(data.RentSerialStart) || string.IsNullOrEmpty(data.RentSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای ستههای کرایی را وارد کنید");
                }
            }

            if (data.VehicleSaleCount.HasValue && data.VehicleSaleCount > 0)
            {
                if (string.IsNullOrEmpty(data.VehicleSaleSerialStart) || string.IsNullOrEmpty(data.VehicleSaleSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای ستههای خرید و فروش وسایط نقلیه را وارد کنید");
                }
            }

            if (data.ExchangeCount.HasValue && data.ExchangeCount > 0)
            {
                if (string.IsNullOrEmpty(data.ExchangeSerialStart) || string.IsNullOrEmpty(data.ExchangeSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای ستههای تبادله را وارد کنید");
                }
            }

            if (data.RegistrationBookCount.HasValue && data.RegistrationBookCount > 0)
            {
                if (string.IsNullOrEmpty(data.RegistrationBookSerialStart) || string.IsNullOrEmpty(data.RegistrationBookSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای کتاب ثبت معاملات را وارد کنید");
                }
            }

            if (data.PrintedPetitionCount.HasValue && data.PrintedPetitionCount > 0)
            {
                if (string.IsNullOrEmpty(data.PrintedPetitionSerialStart) || string.IsNullOrEmpty(data.PrintedPetitionSerialEnd))
                {
                    return (false, "لطفاً سریال نمبر آغاز و ختم برای عرایض مطبوع را وارد کنید");
                }
            }

            return (true, null);
        }

        #endregion
    }
}
