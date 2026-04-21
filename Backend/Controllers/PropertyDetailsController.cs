using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Helpers;
using WebAPIBackend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPIBackend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly WebAPIBackend.Services.IComprehensiveAuditService _auditService;
        private readonly ILookupCacheService _cache;

        public PropertyDetailsController ( AppDbContext context, UserManager<ApplicationUser> userManager, WebAPIBackend.Services.IComprehensiveAuditService auditService, ILookupCacheService cache)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
            _cache = cache;
        }

        private static readonly HashSet<string> AllowedPropertyTypeNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "House",
            "Apartment",
            "Shop",
            "Block",
            "Land",
            "Garden",
            "Hill",
            "Other"
        };

        private async Task<(bool IsValid, string? ErrorMessage, string? NormalizedCustomPropertyType)> ValidateAndNormalizePropertyTypeAsync(int? propertyTypeId, string? customPropertyType)
        {
            if (!propertyTypeId.HasValue)
            {
                return (false, "انتخاب نوعیت ملکیت الزامی است", null);
            }

            var propertyType = await _context.PropertyTypes.FindAsync(propertyTypeId.Value);
            if (propertyType == null || string.IsNullOrWhiteSpace(propertyType.Name) || !AllowedPropertyTypeNames.Contains(propertyType.Name))
            {
                return (false, "نوعیت ملکیت انتخاب‌شده درست نیست", null);
            }

            var isOtherPropertyType = propertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase);
            if (isOtherPropertyType)
            {
                if (string.IsNullOrWhiteSpace(customPropertyType))
                {
                    return (false, "نوشتن نوع ملکیت (سایر) الزامی است", null);
                }

                return (true, null, customPropertyType.Trim());
            }

            return (true, null, null);
        }

        private async Task UpsertPropertyAddressAsync(int propertyDetailsId, string userId, PropertyDetail request)
        {
            if (request?.PropertyAddresses == null)
            {
                return;
            }

            var incoming = request.PropertyAddresses.FirstOrDefault();
            if (incoming == null)
            {
                return;
            }


            var existing = await _context.PropertyAddresses
                .FirstOrDefaultAsync(x => x.PropertyDetailsId == propertyDetailsId);

            if (existing == null)
            {
                var address = new PropertyAddress
                {
                    ProvinceId = incoming.ProvinceId,
                    DistrictId = incoming.DistrictId,
                    Village = incoming.Village,
                    PropertyDetailsId = propertyDetailsId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };
                _context.PropertyAddresses.Add(address);
            }
            else
            {
                existing.ProvinceId = incoming.ProvinceId;
                existing.DistrictId = incoming.DistrictId;
                existing.Village = incoming.Village;
            }

            await _context.SaveChangesAsync();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Check if user can access property module
            if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "property"))
            {
                return StatusCode(403, new { message = "شما اجازه دسترسی به ماژول ملکیت را ندارید" });
            }

            IQueryable<PropertyDetail> propertyQuery;

            // Check if user can view all records (Admin, Authority, etc.)
            if (RbacHelper.CanViewAllRecords(roles, "property"))
            {
                // Admin and Authority can see ALL records (including those with NULL CompanyId)
                propertyQuery = _context.PropertyDetails
                    .AsNoTracking();
            }
            else if (RbacHelper.ShouldFilterByCompany(roles, "property"))
            {
                // Filter by company ID for PropertyOperator
                if (user.CompanyId == 0)
                {
                    return StatusCode(403, new { message = "شما به هیچ رهنمای متصل نیستید" });
                }
                // Only show properties that belong to this company OR have NULL CompanyId (legacy records)
                propertyQuery = _context.PropertyDetails
                    .AsNoTracking()
                    .Where(p => p.CompanyId == user.CompanyId || p.CompanyId == null);
            }
            else
            {
                // Fallback: Filter by user ID (show user's own records regardless of CompanyId)
                propertyQuery = _context.PropertyDetails
                    .AsNoTracking()
                    .Where(p => p.CreatedBy == userId);
            }

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower().Trim();
                propertyQuery = propertyQuery.Where(p =>
                    (p.Pnumber != null && p.Pnumber.ToLower().Contains(searchLower)) ||
                    (p.SellerDetails.Any(s => s.FirstName.ToLower().Contains(searchLower) || s.ElectronicNationalIdNumber.Contains(searchLower))) ||
                    (p.BuyerDetails.Any(b => b.FirstName.ToLower().Contains(searchLower) || b.ElectronicNationalIdNumber.Contains(searchLower)))
                );
            }

            var totalCount = await propertyQuery.CountAsync();

            var query = await (from p in propertyQuery
                         orderby p.Id descending
                         select new
                         {
                             p.Id,
                             p.Pnumber,
                             p.Parea,
                             p.PunitTypeId,
                             p.NumofFloor,
                             p.NumofRooms,
                             p.PropertyTypeId,
                             p.TransactionTypeId,
                             p.Price,
                             p.PriceText,
                             p.Des,
                             p.RoyaltyAmount,
                             p.FilePath,
                             PropertyTypeText = p.PropertyType != null && p.PropertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase) && p.CustomPropertyType != null
                                ? p.CustomPropertyType
                                : p.PropertyType != null ? p.PropertyType.Name : null,
                             UnitTypeText = p.PunitType != null ? p.PunitType.Name : null,
                             TransactionTypeText = p.TransactionType != null 
                                ? p.TransactionType.Name 
                                : (p.BuyerDetails != null && p.BuyerDetails.Any() && p.BuyerDetails.First().TransactionType != null)
                                    ? p.BuyerDetails.First().TransactionType
                                    : null,
                             p.iscomplete,
                             SellerName = (p.SellerDetails != null && p.SellerDetails.Any()) ? p.SellerDetails.First().FirstName : null,
                             BuyerName = (p.BuyerDetails != null && p.BuyerDetails.Any()) ? p.BuyerDetails.First().FirstName : null,
                             SellerElectronicNationalIdNumber = (p.SellerDetails != null && p.SellerDetails.Any()) ? p.SellerDetails.First().ElectronicNationalIdNumber : null,
                             BuyerElectronicNationalIdNumber = (p.BuyerDetails != null && p.BuyerDetails.Any()) ? p.BuyerDetails.First().ElectronicNationalIdNumber : null,
                             p.CreatedBy,
                             p.CompanyId,
                             CompanyTitle = p.Company != null ? p.Company.Title : null
                         })
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();

            return Ok(new
            {
                items = query,
                totalCount,
                page,
                pageSize
            });
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                IQueryable<PropertyDetail> propertyQuery;

                // Apply RBAC filtering to ensure users only see authorized records
                if (RbacHelper.CanViewAllRecords(roles, "property"))
                {
                    // Admin and Authority can see ALL records (including those with NULL CompanyId)
                    propertyQuery = _context.PropertyDetails.AsNoTracking();
                }
                else if (RbacHelper.ShouldFilterByCompany(roles, "property"))
                {
                    // Filter by company ID for PropertyOperator
                    if (user.CompanyId == 0)
                    {
                        return StatusCode(403, new { message = "شما به هیچ رهنمای متصل نیستید" });
                    }
                    // Show properties that belong to this company OR have NULL CompanyId (legacy records)
                    propertyQuery = _context.PropertyDetails
                        .AsNoTracking()
                        .Where(p => p.CompanyId == user.CompanyId || p.CompanyId == null);
                }
                else
                {
                    // Filter by user ID for regular users (show user's own records regardless of CompanyId)
                    propertyQuery = _context.PropertyDetails
                        .AsNoTracking()
                        .Where(p => p.CreatedBy == userId);
                }

                var property = await propertyQuery
                    .Where(x => x.Id == id)
                    .Select(p => new
                    {
                        p.Id,
                        p.Pnumber,
                        p.Parea,
                        p.PunitTypeId,
                        p.NumofFloor,
                        p.NumofRooms,
                        p.PropertyTypeId,
                        p.CustomPropertyType,
                        p.TransactionTypeId,
                        p.Price,
                        p.PriceText,
                        p.RoyaltyAmount,
                        p.Des,
                        p.iscomplete,
                        p.West,
                        p.East,
                        p.North,
                        p.South,
                        p.DocumentType,
                        p.IssuanceNumber,
                        p.IssuanceDate,
                        p.SerialNumber,
                        p.TransactionDate,
                        p.FilePath,
                        p.PreviousDocumentsPath,
                        p.ExistingDocumentsPath,
                        p.CompanyId,
                        PropertyAddresses = p.PropertyAddresses.Select(a => new
                        {
                            a.Id,
                            a.PropertyDetailsId,
                            a.ProvinceId,
                            a.DistrictId,
                            a.Village
                        })
                    })
                    .ToListAsync();

                if (property == null || !property.Any())
                {
                    return NotFound(new { message = "سند ملکیت یافت نشد یا شما اجازه دسترسی ندارید" });
                }

                return Ok(property);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in GetPropertyById: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "خطای داخلی سرور", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetView/{id}")]
        public async Task<IActionResult> GetPropertyViewById(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            IQueryable<PropertyDetail> propertyQuery;
            // ADMIN and AUTHORITY can view all records
            if (RbacHelper.CanViewAllRecords(roles, "property"))
            {
                // Admin and Authority can see ALL records (including those with NULL CompanyId)
                propertyQuery = _context.PropertyDetails;
            }
            else if (RbacHelper.ShouldFilterByCompany(roles, "property"))
            {
                // Filter by company ID for PropertyOperator
                if (user.CompanyId == 0)
                {
                    return StatusCode(403, new { message = "شما به هیچ رهنمای متصل نیستید" });
                }
                // Only show properties that belong to this company OR have NULL CompanyId (legacy records)
                propertyQuery = _context.PropertyDetails
                    .Where(p => p.CompanyId == user.CompanyId || p.CompanyId == null);
            }
            else
            {
                // Fallback: Filter by user ID (show user's own records regardless of CompanyId)
                propertyQuery = _context.PropertyDetails
                    .Where(p => p.CreatedBy == userId);
            }

            var data = await propertyQuery
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Pnumber,
                    p.Parea,
                    p.PunitTypeId,
                    UnitTypeName = p.PunitType != null ? p.PunitType.Name : null,
                    p.NumofFloor,
                    p.NumofRooms,
                    p.PropertyTypeId,
                    p.CustomPropertyType,
                    PropertyTypeName = p.PropertyType != null ? p.PropertyType.Name : null,
                    PropertyTypeText = p.PropertyType != null && p.PropertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase) && p.CustomPropertyType != null
                        ? p.CustomPropertyType
                        : p.PropertyType != null ? p.PropertyType.Name : null,
                    p.TransactionTypeId,
                    TransactionTypeName = p.TransactionType != null ? p.TransactionType.Name : null,
                    p.Price,
                    p.PriceText,
                    p.RoyaltyAmount,
                    p.Des,
                    p.iscomplete,
                    p.iseditable,
                    p.West,
                    p.East,
                    p.North,
                    p.South,
                    p.DocumentType,
                    p.IssuanceNumber,
                    p.IssuanceDate,
                    p.SerialNumber,
                    p.TransactionDate,
                    p.FilePath,
                    p.PreviousDocumentsPath,
                    p.ExistingDocumentsPath,
                    p.CompanyId,
                    CompanyTitle = p.Company != null ? p.Company.Title : null,
                    Address = p.PropertyAddresses
                        .Select(a => new
                        {
                            a.Id,
                            a.ProvinceId,
                            a.DistrictId,
                            a.Village,
                            ProvinceDari = _context.Locations
                                .Where(l => a.ProvinceId.HasValue && l.Id == a.ProvinceId.Value)
                                .Select(l => l.Dari)
                                .FirstOrDefault(),
                            DistrictDari = _context.Locations
                                .Where(l => a.DistrictId.HasValue && l.Id == a.DistrictId.Value)
                                .Select(l => l.Dari)
                                .FirstOrDefault(),
                        })
                        .FirstOrDefault(),
                    Sellers = p.SellerDetails
                        .Select(s => new
                        {
                            s.Id,
                            s.FirstName,
                            s.FatherName,
                            s.GrandFather,
                            s.PhoneNumber,
                            s.ElectronicNationalIdNumber,
                            s.RoleType,
                            s.TaxIdentificationNumber,
                            s.AdditionalDetails,
                            s.Photo,
                            s.NationalIdCard,
                            s.AuthorizationLetter,
                            s.HeirsLetter,
                            s.PaddressProvinceId,
                            s.PaddressDistrictId,
                            s.PaddressVillage,
                            PaddressProvinceDari = s.PaddressProvince != null ? s.PaddressProvince.Dari : null,
                            PaddressDistrictDari = s.PaddressDistrict != null ? s.PaddressDistrict.Dari : null,
                            s.TaddressProvinceId,
                            s.TaddressDistrictId,
                            s.TaddressVillage,
                            TaddressProvinceDari = s.TaddressProvince != null ? s.TaddressProvince.Dari : null,
                            TaddressDistrictDari = s.TaddressDistrict != null ? s.TaddressDistrict.Dari : null,
                        })
                        .ToList(),
                    Buyers = p.BuyerDetails
                        .Select(b => new
                        {
                            b.Id,
                            b.FirstName,
                            b.FatherName,
                            b.GrandFather,
                            b.PhoneNumber,
                            b.ElectronicNationalIdNumber,
                            b.RoleType,
                            b.TaxIdentificationNumber,
                            b.AdditionalDetails,
                            b.Photo,
                            b.NationalIdCard,
                            b.AuthorizationLetter,
                            b.Price,
                            b.PriceText,
                            b.RoyaltyAmount,
                            b.HalfPrice,
                            b.TransactionType,
                            b.TransactionTypeDescription,
                            b.RentStartDate,
                            b.RentEndDate,
                            b.ContractStartDate,
                            b.ContractEndDate,
                            b.PaddressProvinceId,
                            b.PaddressDistrictId,
                            b.PaddressVillage,
                            PaddressProvinceDari = b.PaddressProvince != null ? b.PaddressProvince.Dari : null,
                            PaddressDistrictDari = b.PaddressDistrict != null ? b.PaddressDistrict.Dari : null,
                            b.TaddressProvinceId,
                            b.TaddressDistrictId,
                            b.TaddressVillage,
                            TaddressProvinceDari = b.TaddressProvince != null ? b.TaddressProvince.Dari : null,
                            TaddressDistrictDari = b.TaddressDistrict != null ? b.TaddressDistrict.Dari : null,
                        })
                        .ToList(),
                    Witnesses = p.WitnessDetails
                        .Select(w => new
                        {
                            w.Id,
                            w.FirstName,
                            w.FatherName,
                            w.PhoneNumber,
                            w.ElectronicNationalIdNumber,
                            w.NationalIdCard
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] PropertyDetail request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Check if user can create property records
            if (!RbacHelper.CanCreateRecords(roles, "property"))
            {
                return StatusCode(403, new { message = "شما اجازه ایجاد سند ملکیت را ندارید" });
            }

            var (isValid, errorMessage, normalizedCustomPropertyType) = await ValidateAndNormalizePropertyTypeAsync(request.PropertyTypeId, request.CustomPropertyType);
            if (!isValid)
            {
                return StatusCode(400, errorMessage);
            }

            // Parse calendar type and convert dates
            var calendarType = DateConversionHelper.ParseCalendarType(request.CalendarType);
            
            DateTime? issuanceDate = null;
            if (!string.IsNullOrWhiteSpace(request.IssuanceDateStr))
            {
                issuanceDate = DateConversionHelper.ParseDateString(request.IssuanceDateStr, calendarType);
            }
            else if (request.IssuanceDate.HasValue)
            {
                issuanceDate = request.IssuanceDate;
            }

            DateTime? transactionDate = null;
            if (!string.IsNullOrWhiteSpace(request.TransactionDateStr))
            {
                transactionDate = DateConversionHelper.ParseDateString(request.TransactionDateStr, calendarType);
            }
            else if (request.TransactionDate.HasValue)
            {
                transactionDate = request.TransactionDate;
            }

            // Generate property number if not provided
            string propertyNumber;
            if (string.IsNullOrWhiteSpace(request.Pnumber) || request.Pnumber == "0")
            {
                // Generate a unique property number based on timestamp
                propertyNumber = $"PROP-{DateTime.UtcNow:yyyyMMddHHmmss}";
            }
            else
            {
                propertyNumber = request.Pnumber;
            }

            var property = new PropertyDetail
            {
                Pnumber = propertyNumber,
                Parea = request.Parea,
                PunitTypeId = request.PunitTypeId,
                NumofFloor=request.NumofFloor,
                NumofRooms=request.NumofRooms,
                PropertyTypeId=request.PropertyTypeId,
                CustomPropertyType = normalizedCustomPropertyType,
                Price=request.Price,
                PriceText=request.PriceText,
                RoyaltyAmount= (double.TryParse(request.Price, out var priceVal) ? priceVal * 0.01 : 0).ToString(),
                TransactionTypeId=request.TransactionTypeId,
                Des=request.Des,
                CreatedAt=DateTime.UtcNow,
                CreatedBy= userId,
                CompanyId=user.CompanyId, // Set company ID for data isolation
                FilePath=request.FilePath,
                PreviousDocumentsPath = request.PreviousDocumentsPath,
                ExistingDocumentsPath = request.ExistingDocumentsPath,
                West=request.West,
                East=request.East,
                North=request.North,
                South=request.South,
                DocumentType=request.DocumentType,
                IssuanceNumber=request.IssuanceNumber,
                IssuanceDate=issuanceDate,
                SerialNumber=request.SerialNumber,
                TransactionDate=transactionDate,
                iscomplete = false,
                iseditable = false
                
            };
            _context.Add(property);
            await _context.SaveChangesAsync();

            await UpsertPropertyAddressAsync(property.Id, userId, request);

            // Log the creation to comprehensive audit
            await _auditService.LogCreateAsync(
                WebAPIBackend.Models.Audit.AuditModules.Property,
                "Property",
                property.Id.ToString(),
                newValues: new { property.Id, property.Pnumber, property.IssuanceNumber, property.SerialNumber, property.PropertyTypeId },
                descriptionDari: $"ثبت سند ملکیت با شماره {property.Pnumber}");

            var result = new { Id = property.Id, PropertyTypeId = property.PropertyTypeId };
            return Ok(result);
        }

        [Authorize]
        [HttpGet("getpropertyType")]
        public async Task<IActionResult> GetAllPropertyType()
        {
            try
            {
                var users = await _cache.GetPropertyTypesAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Authorize]
        [HttpGet("gettransactionType")]
        public async Task<IActionResult> GetAlltransactionType()
        {
            try
            {
                var users = await _cache.GetTransactionTypesAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Authorize]
        [HttpGet("getunitType")]
        public async Task<IActionResult> GetAllUnitType()
        {
            try
            {
                var users = await _cache.GetPunitTypesAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePropertyDetails(int id, [FromBody] PropertyDetail request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            if (id != request.Id)
            {
                return BadRequest();
            }
            var existingProperty = await _context.PropertyDetails.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            var (isValid, errorMessage, normalizedCustomPropertyType) = await ValidateAndNormalizePropertyTypeAsync(request.PropertyTypeId, request.CustomPropertyType);
            if (!isValid)
            {
                return StatusCode(400, errorMessage);
            }

            request.CustomPropertyType = normalizedCustomPropertyType;

            // Parse calendar type and convert dates
            var calendarType = DateConversionHelper.ParseCalendarType(request.CalendarType);
            
            if (!string.IsNullOrWhiteSpace(request.IssuanceDateStr))
            {
                request.IssuanceDate = DateConversionHelper.ParseDateString(request.IssuanceDateStr, calendarType);
            }

            if (!string.IsNullOrWhiteSpace(request.TransactionDateStr))
            {
                request.TransactionDate = DateConversionHelper.ParseDateString(request.TransactionDateStr, calendarType);
            }

            // Store the original values of the CreatedBy and CreatedOn properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var pNumber = existingProperty.Pnumber; // Preserve PNumber

            // Update the entity with the new values
            _context.Entry(existingProperty).CurrentValues.SetValues(request);

            // Restore the original values of the CreatedBy and CreatedOn properties
            existingProperty.CreatedBy = createdBy;
            existingProperty.CreatedAt = createdAt;
            existingProperty.Pnumber = pNumber; // Restore PNumber

            var entry = _context.Entry(existingProperty);
            entry.State = EntityState.Modified;

            var changes = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .SelectMany(e => e.Properties)
                .Where(p => p.IsModified)
                .ToDictionary(p => p.Metadata.Name, p => new
                {
                    OldValue = p.OriginalValue,
                    NewValue = p.CurrentValue
                });

            await _context.SaveChangesAsync();

            foreach (var change in changes)
            {
                // Only add an entry to the vehicleaudit table if the property has been modified
                if (change.Value.OldValue != null && !change.Value.OldValue.Equals(change.Value.NewValue))
                {
                    _context.Propertyaudits.Add(new Propertyaudit
                    {
                        PropertyId = existingProperty.Id,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.UtcNow,
                        PropertyName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            await UpsertPropertyAddressAsync(existingProperty.Id, userId, request);

            // Log the update to comprehensive audit
            var modifiedChanges = changes.Where(c => c.Value.OldValue != null && !c.Value.OldValue.Equals(c.Value.NewValue))
                .ToDictionary(c => c.Key, c => new { OldValue = c.Value.OldValue, NewValue = c.Value.NewValue });
            if (modifiedChanges.Any())
            {
                await _auditService.LogUpdateAsync(
                    WebAPIBackend.Models.Audit.AuditModules.Property,
                    "Property",
                    id.ToString(),
                    oldValues: modifiedChanges.ToDictionary(c => c.Key, c => c.Value.OldValue),
                    newValues: modifiedChanges.ToDictionary(c => c.Key, c => c.Value.NewValue),
                    descriptionDari: $"تغییر سند ملکیت با شماره {existingProperty.Pnumber}");
            }

            var result = new { Id = request.Id, PropertyTypeId = request.PropertyTypeId };
            return Ok(result);
        }

        [Authorize]
        [Authorize]
        [HttpGet("GetPrintRecord/{id}")]
        public async Task<IActionResult> GetPrintRecordById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                // Get property details with all related data in a single server-side projection
                var property = await _context.PropertyDetails
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(p => new
                    {
                        p.Id,
                        p.DocumentType,
                        p.CustomDocumentType,
                        p.IssuanceNumber,
                        p.IssuanceDate,
                        p.SerialNumber,
                        p.TransactionDate,
                        p.Pnumber,
                        p.Parea,
                        p.NumofRooms,
                        p.NumofFloor,
                        p.North,
                        p.South,
                        p.West,
                        p.East,
                        p.Price,
                        p.PriceText,
                        p.RoyaltyAmount,
                        p.CustomPropertyType,
                        p.CreatedAt,
                        PropertyTypeName = p.PropertyType != null ? p.PropertyType.Name : null,
                        UnitTypeName = p.PunitType != null ? p.PunitType.Name : null,
                        UnitTypeDari = p.PunitType != null ? p.PunitType.Dari : null,
                        TransactionTypeName = p.TransactionType != null ? p.TransactionType.Name : null,
                        p.FilePath,
                        p.PreviousDocumentsPath,
                        p.ExistingDocumentsPath,
                        // Address
                        Address = p.PropertyAddresses.Select(a => new
                        {
                            a.Village,
                            ProvinceName = a.ProvinceId.HasValue ? _context.Locations.Where(l => l.Id == a.ProvinceId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            ProvinceDari = a.ProvinceId.HasValue ? _context.Locations.Where(l => l.Id == a.ProvinceId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            DistrictName = a.DistrictId.HasValue ? _context.Locations.Where(l => l.Id == a.DistrictId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            DistrictDari = a.DistrictId.HasValue ? _context.Locations.Where(l => l.Id == a.DistrictId.Value).Select(l => l.Dari).FirstOrDefault() : null
                        }).FirstOrDefault(),
                        // Sellers with location data
                        Sellers = p.SellerDetails.Select(s => new
                        {
                            s.FirstName,
                            s.FatherName,
                            s.GrandFather,
                            s.PhoneNumber,
                            s.ElectronicNationalIdNumber,
                            s.Photo,
                            PaddressProvince = s.PaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == s.PaddressProvinceId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            PaddressProvinceDari = s.PaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == s.PaddressProvinceId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            PaddressDistrict = s.PaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == s.PaddressDistrictId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            PaddressDistrictDari = s.PaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == s.PaddressDistrictId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            s.PaddressVillage,
                            TaddressProvince = s.TaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == s.TaddressProvinceId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            TaddressProvinceDari = s.TaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == s.TaddressProvinceId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            TaddressDistrict = s.TaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == s.TaddressDistrictId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            TaddressDistrictDari = s.TaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == s.TaddressDistrictId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            s.TaddressVillage
                        }).ToList(),
                        // Buyers with location data
                        Buyers = p.BuyerDetails.Select(b => new
                        {
                            b.FirstName,
                            b.FatherName,
                            b.GrandFather,
                            b.PhoneNumber,
                            b.ElectronicNationalIdNumber,
                            b.Photo,
                            b.TransactionType,
                            b.Price,
                            b.PriceText,
                            b.RoyaltyAmount,
                            b.HalfPrice,
                            PaddressProvince = b.PaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == b.PaddressProvinceId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            PaddressProvinceDari = b.PaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == b.PaddressProvinceId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            PaddressDistrict = b.PaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == b.PaddressDistrictId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            PaddressDistrictDari = b.PaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == b.PaddressDistrictId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            b.PaddressVillage,
                            TaddressProvince = b.TaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == b.TaddressProvinceId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            TaddressProvinceDari = b.TaddressProvinceId.HasValue ? _context.Locations.Where(l => l.Id == b.TaddressProvinceId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            TaddressDistrict = b.TaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == b.TaddressDistrictId.Value).Select(l => l.Name).FirstOrDefault() : null,
                            TaddressDistrictDari = b.TaddressDistrictId.HasValue ? _context.Locations.Where(l => l.Id == b.TaddressDistrictId.Value).Select(l => l.Dari).FirstOrDefault() : null,
                            b.TaddressVillage
                        }).ToList(),
                        // Witnesses
                        Witnesses = p.WitnessDetails.Select(w => new
                        {
                            w.FirstName,
                            w.FatherName,
                            w.PhoneNumber,
                            w.ElectronicNationalIdNumber
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (property == null)
                {
                    return NotFound();
                }

                // Convert the 'CreatedAt' property to the requested calendar format
                var calendar = Helpers.DateConversionHelper.ParseCalendarType(calendarType);
                string shamsiDate = string.Empty;
                if (property.CreatedAt.HasValue)
                {
                    shamsiDate = Helpers.DateConversionHelper.FormatDate(property.CreatedAt.Value, calendar);
                }

                var firstBuyer = property.Buyers.FirstOrDefault();
                var firstSeller = property.Sellers.FirstOrDefault();
                var witnesses = property.Witnesses;

                var result = new
                {
                    property.Id,
                    property.DocumentType,
                    property.CustomDocumentType,
                    property.IssuanceNumber,
                    property.IssuanceDate,
                    property.SerialNumber,
                    property.TransactionDate,
                    PNumber = property.Pnumber,
                    PArea = property.Parea,
                    property.NumofRooms,
                    NumofHouses = property.NumofFloor,
                    property.North,
                    property.South,
                    property.West,
                    property.East,
                    Price = firstBuyer?.Price ?? property.Price,
                    PriceText = firstBuyer?.PriceText ?? property.PriceText ?? string.Empty,
                    RoyaltyAmount = firstBuyer?.RoyaltyAmount ?? property.RoyaltyAmount,
                    HalfPrice = firstBuyer?.HalfPrice,
                    PropertypeType = property.PropertyTypeName,
                    property.CustomPropertyType,
                    property.CreatedAt,
                    CreatedAtFormatted = shamsiDate,
                    
                    // Location
                    Province = property.Address?.ProvinceName,
                    ProvinceDari = property.Address?.ProvinceDari,
                    District = property.Address?.DistrictName,
                    DistrictDari = property.Address?.DistrictDari,
                    Village = property.Address?.Village,
                    
                    // All Sellers
                    Sellers = property.Sellers,
                    
                    // All Buyers
                    Buyers = property.Buyers,
                    
                    // Witnesses
                    Witnesses = property.Witnesses,
                    
                    // Backward compatibility - first seller
                    SellerFirstName = firstSeller?.FirstName,
                    SellerFatherName = firstSeller?.FatherName,
                    SellerGrandFather = firstSeller?.GrandFather,
                    SellerPhoneNumber = firstSeller?.PhoneNumber,
                    SellerIndentityCardNumber = firstSeller?.ElectronicNationalIdNumber,
                    SellerPhoto = firstSeller?.Photo,
                    SellerProvince = firstSeller?.PaddressProvince,
                    SellerProvinceDari = firstSeller?.PaddressProvinceDari,
                    SellerDistrict = firstSeller?.PaddressDistrict,
                    SellerDistrictDari = firstSeller?.PaddressDistrictDari,
                    SellerVillage = firstSeller?.PaddressVillage,
                    TSellerProvince = firstSeller?.TaddressProvince,
                    TSellerProvinceDari = firstSeller?.TaddressProvinceDari,
                    TSellerDistrict = firstSeller?.TaddressDistrict,
                    TSellerDistrictDari = firstSeller?.TaddressDistrictDari,
                    TSellerVillage = firstSeller?.TaddressVillage,
                    
                    // Backward compatibility - first buyer
                    BuyerFirstName = firstBuyer?.FirstName,
                    BuyerFatherName = firstBuyer?.FatherName,
                    BuyerGrandFather = firstBuyer?.GrandFather,
                    BuyerPhoneNumber = firstBuyer?.PhoneNumber,
                    BuyerIndentityCardNumber = firstBuyer?.ElectronicNationalIdNumber,
                    BuyerPhoto = firstBuyer?.Photo,
                    BuyerProvince = firstBuyer?.PaddressProvince,
                    BuyerProvinceDari = firstBuyer?.PaddressProvinceDari,
                    BuyerDistrict = firstBuyer?.PaddressDistrict,
                    BuyerDistrictDari = firstBuyer?.PaddressDistrictDari,
                    BuyerVillage = firstBuyer?.PaddressVillage,
                    TBuyerProvince = firstBuyer?.TaddressProvince,
                    TBuyerProvinceDari = firstBuyer?.TaddressProvinceDari,
                    TBuyerDistrict = firstBuyer?.TaddressDistrict,
                    TBuyerDistrictDari = firstBuyer?.TaddressDistrictDari,
                    TBuyerVillage = firstBuyer?.TaddressVillage,
                    
                    WitnessOneFirstName = witnesses.ElementAtOrDefault(0)?.FirstName,
                    WitnessOneFatherName = witnesses.ElementAtOrDefault(0)?.FatherName,
                    WitnessOnePhoneNumber = witnesses.ElementAtOrDefault(0)?.PhoneNumber,
                    WitnessOneIndentityCardNumber = witnesses.ElementAtOrDefault(0)?.ElectronicNationalIdNumber,
                    WitnessTwoFirstName = witnesses.ElementAtOrDefault(1)?.FirstName,
                    WitnessTwoFatherName = witnesses.ElementAtOrDefault(1)?.FatherName,
                    WitnessTwoPhoneNumber = witnesses.ElementAtOrDefault(1)?.PhoneNumber,
                    WitnessTwoIndentityCardNumber = witnesses.ElementAtOrDefault(1)?.ElectronicNationalIdNumber,
                    
                    // Property Unit Type and Transaction Type
                    UnitType = property.UnitTypeName,
                    UnitTypeDari = property.UnitTypeDari,
                    TransactionType = firstBuyer?.TransactionType ?? property.TransactionTypeName,
                    
                    // Property Documents and Images
                    property.FilePath,
                    property.PreviousDocumentsPath,
                    property.ExistingDocumentsPath
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving print record",
                    error = ex.Message
                });
            }
        }

        private async Task<string?> GetLocationNameAsync(int? locationId)
        {
            if (!locationId.HasValue) return null;
            var location = await _context.Locations.FindAsync(locationId.Value);
            return location?.Name;
        }

        private async Task<string?> GetLocationDariAsync(int? locationId)
        {
            if (!locationId.HasValue) return null;
            var location = await _context.Locations.FindAsync(locationId.Value);
            return location?.Dari;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var property = await _context.PropertyDetails
                    .Include(p => p.SellerDetails)
                    .Include(p => p.BuyerDetails)
                    .Include(p => p.WitnessDetails)
                    .Include(p => p.PropertyAddresses)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (property == null)
                {
                    return NotFound(new { message = "سند ملکیت یافت نشد" });
                }

                // Delete audit records first
                var auditRecords = await _context.Propertyaudits
                    .Where(a => a.PropertyId == id)
                    .ToListAsync();
                if (auditRecords.Any())
                {
                    _context.Propertyaudits.RemoveRange(auditRecords);
                }

                // Delete seller audit records
                if (property.SellerDetails != null && property.SellerDetails.Any())
                {
                    var sellerIds = property.SellerDetails.Select(s => s.Id).ToList();
                    var sellerAuditRecords = await _context.Propertyselleraudits
                        .Where(a => sellerIds.Contains(a.SellerId))
                        .ToListAsync();
                    if (sellerAuditRecords.Any())
                    {
                        _context.Propertyselleraudits.RemoveRange(sellerAuditRecords);
                    }
                }

                // Delete buyer audit records
                if (property.BuyerDetails != null && property.BuyerDetails.Any())
                {
                    var buyerIds = property.BuyerDetails.Select(b => b.Id).ToList();
                    var buyerAuditRecords = await _context.Propertybuyeraudits
                        .Where(a => buyerIds.Contains(a.BuyerId))
                        .ToListAsync();
                    if (buyerAuditRecords.Any())
                    {
                        _context.Propertybuyeraudits.RemoveRange(buyerAuditRecords);
                    }
                }

                // Note: Witness audit records are not tracked in a separate audit table

                // Remove property addresses
                if (property.PropertyAddresses != null && property.PropertyAddresses.Any())
                {
                    _context.PropertyAddresses.RemoveRange(property.PropertyAddresses);
                }

                // Remove witnesses
                if (property.WitnessDetails != null && property.WitnessDetails.Any())
                {
                    _context.WitnessDetails.RemoveRange(property.WitnessDetails);
                }

                // Remove buyers
                if (property.BuyerDetails != null && property.BuyerDetails.Any())
                {
                    _context.BuyerDetails.RemoveRange(property.BuyerDetails);
                }

                // Remove sellers
                if (property.SellerDetails != null && property.SellerDetails.Any())
                {
                    _context.SellerDetails.RemoveRange(property.SellerDetails);
                }

                // Remove the property itself
                _context.PropertyDetails.Remove(property);

                await _context.SaveChangesAsync();

                // Log the deletion to comprehensive audit
                await _auditService.LogDeleteAsync(
                    WebAPIBackend.Models.Audit.AuditModules.Property,
                    "Property",
                    id.ToString(),
                    oldValues: new { property.Id, property.Pnumber, property.SerialNumber },
                    descriptionDari: $"حذف سند ملکیت با شماره {property.Pnumber}");

                return Ok(new { message = "سند ملکیت با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                // Log the detailed error for debugging
                Console.WriteLine($"Error deleting property: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return StatusCode(500, new { 
                    message = $"خطا در حذف سند: {ex.Message}",
                    details = ex.InnerException?.Message 
                });
            }
        }

    }
}
