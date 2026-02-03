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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPIBackend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public PropertyDetailsController ( AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> GetAll()
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

            // Check if user can view all records
            if (RbacHelper.CanViewAllRecords(roles, "property"))
            {
                propertyQuery = _context.PropertyDetails;
            }
            else if (RbacHelper.ShouldFilterByCompany(roles, "property"))
            {
                // Filter by company ID for PropertyOperator
                if (user.CompanyId == 0)
                {
                    return StatusCode(403, new { message = "شما به هیچ رهنمای متصل نیستید" });
                }
                propertyQuery = _context.PropertyDetails.Where(p => p.CompanyId == user.CompanyId);
            }
            else
            {
                // Fallback: Filter by user ID
                propertyQuery = _context.PropertyDetails.Where(p => p.CreatedBy == userId);
            }

            var query = (from p in propertyQuery
                        
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
                             TransactionTypeText = p.TransactionType != null ? p.TransactionType.Name : null,
                             p.iscomplete,
                             SellerName = (p.SellerDetails != null && p.SellerDetails.Any()) ? p.SellerDetails.First().FirstName : null,
                             BuyerName = (p.BuyerDetails != null && p.BuyerDetails.Any()) ? p.BuyerDetails.First().FirstName : null,
                             SellerElectronicNationalIdNumber = (p.SellerDetails != null && p.SellerDetails.Any()) ? p.SellerDetails.First().ElectronicNationalIdNumber : null,
                             BuyerElectronicNationalIdNumber = (p.BuyerDetails != null && p.BuyerDetails.Any()) ? p.BuyerDetails.First().ElectronicNationalIdNumber : null,
                             p.CreatedBy
                         }).ToList();

            return Ok(query);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            try
            {
                var Pro = await _context.PropertyDetails
                    .Include(p => p.PropertyAddresses)
                    .Where(x => x.Id.Equals(id))
                    .ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
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
                propertyQuery = _context.PropertyDetails;
            }
            else if (RbacHelper.ShouldFilterByCompany(roles, "property"))
            {
                // Filter by company ID for PropertyOperator
                if (user.CompanyId == 0)
                {
                    return StatusCode(403, new { message = "شما به هیچ رهنمای متصل نیستید" });
                }
                propertyQuery = _context.PropertyDetails.Where(p => p.CompanyId == user.CompanyId);
            }
            else
            {
                // Fallback: Filter by user ID
                propertyQuery = _context.PropertyDetails.Where(p => p.CreatedBy == userId);
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

            var result = new { Id = property.Id, PropertyTypeId = property.PropertyTypeId };
            return Ok(result);
        }

        [Authorize]
        [HttpGet("getpropertyType")]
        public async Task<IActionResult> GetAllPropertyType()
        {
            try
            {
                var users = await _context.PropertyTypes.ToListAsync();

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
                var users = await _context.TransactionTypes.ToListAsync();

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
                var users = await _context.PunitTypes.ToListAsync();

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
                // Get property details with all sellers and buyers
                var property = await _context.PropertyDetails
                    .Include(p => p.SellerDetails)
                    .Include(p => p.BuyerDetails)
                    .Include(p => p.WitnessDetails)
                    .Include(p => p.PropertyAddresses)
                    .Include(p => p.PropertyType)
                    .Include(p => p.PunitType)
                    .Include(p => p.TransactionType)
                    .FirstOrDefaultAsync(x => x.Id == id);

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

                // Get property address
                var address = property.PropertyAddresses.FirstOrDefault();
                var provinceData = address?.ProvinceId.HasValue == true 
                    ? await _context.Locations.FindAsync(address.ProvinceId.Value) 
                    : null;
                var districtData = address?.DistrictId.HasValue == true 
                    ? await _context.Locations.FindAsync(address.DistrictId.Value) 
                    : null;

                // Get all sellers with their location data
                var sellers = new List<object>();
                foreach (var seller in property.SellerDetails)
                {
                    var sellerPProvince = seller.PaddressProvinceId.HasValue 
                        ? await _context.Locations.FindAsync(seller.PaddressProvinceId.Value) 
                        : null;
                    var sellerPDistrict = seller.PaddressDistrictId.HasValue 
                        ? await _context.Locations.FindAsync(seller.PaddressDistrictId.Value) 
                        : null;
                    var sellerTProvince = seller.TaddressProvinceId.HasValue 
                        ? await _context.Locations.FindAsync(seller.TaddressProvinceId.Value) 
                        : null;
                    var sellerTDistrict = seller.TaddressDistrictId.HasValue 
                        ? await _context.Locations.FindAsync(seller.TaddressDistrictId.Value) 
                        : null;

                    sellers.Add(new
                    {
                        FirstName = seller.FirstName,
                        FatherName = seller.FatherName,
                        GrandFather = seller.GrandFather,
                        ElectronicNationalIdNumber = seller.ElectronicNationalIdNumber,
                        Photo = seller.Photo,
                        PaddressProvince = sellerPProvince?.Name,
                        PaddressProvinceDari = sellerPProvince?.Dari,
                        PaddressDistrict = sellerPDistrict?.Name,
                        PaddressDistrictDari = sellerPDistrict?.Dari,
                        PaddressVillage = seller.PaddressVillage,
                        TaddressProvince = sellerTProvince?.Name,
                        TaddressProvinceDari = sellerTProvince?.Dari,
                        TaddressDistrict = sellerTDistrict?.Name,
                        TaddressDistrictDari = sellerTDistrict?.Dari,
                        TaddressVillage = seller.TaddressVillage
                    });
                }

                // Get all buyers with their location data
                var buyers = new List<object>();
                foreach (var buyer in property.BuyerDetails)
                {
                    var buyerPProvince = buyer.PaddressProvinceId.HasValue 
                        ? await _context.Locations.FindAsync(buyer.PaddressProvinceId.Value) 
                        : null;
                    var buyerPDistrict = buyer.PaddressDistrictId.HasValue 
                        ? await _context.Locations.FindAsync(buyer.PaddressDistrictId.Value) 
                        : null;
                    var buyerTProvince = buyer.TaddressProvinceId.HasValue 
                        ? await _context.Locations.FindAsync(buyer.TaddressProvinceId.Value) 
                        : null;
                    var buyerTDistrict = buyer.TaddressDistrictId.HasValue 
                        ? await _context.Locations.FindAsync(buyer.TaddressDistrictId.Value) 
                        : null;

                    buyers.Add(new
                    {
                        FirstName = buyer.FirstName,
                        FatherName = buyer.FatherName,
                        GrandFather = buyer.GrandFather,
                        ElectronicNationalIdNumber = buyer.ElectronicNationalIdNumber,
                        Photo = buyer.Photo,
                        PaddressProvince = buyerPProvince?.Name,
                        PaddressProvinceDari = buyerPProvince?.Dari,
                        PaddressDistrict = buyerPDistrict?.Name,
                        PaddressDistrictDari = buyerPDistrict?.Dari,
                        PaddressVillage = buyer.PaddressVillage,
                        TaddressProvince = buyerTProvince?.Name,
                        TaddressProvinceDari = buyerTProvince?.Dari,
                        TaddressDistrict = buyerTDistrict?.Name,
                        TaddressDistrictDari = buyerTDistrict?.Dari,
                        TaddressVillage = buyer.TaddressVillage
                    });
                }

                // Get witnesses
                var witnesses = property.WitnessDetails.Select(w => new
                {
                    FirstName = w.FirstName,
                    FatherName = w.FatherName,
                    ElectronicNationalIdNumber = w.ElectronicNationalIdNumber
                }).ToList();

                // Create result with all sellers and buyers
                var result = new
                {
                    Id = property.Id,
                    DocumentType = property.DocumentType,
                    CustomDocumentType = property.CustomDocumentType,
                    IssuanceNumber = property.IssuanceNumber,
                    IssuanceDate = property.IssuanceDate,
                    SerialNumber = property.SerialNumber,
                    TransactionDate = property.TransactionDate,
                    PNumber = property.Pnumber,
                    PArea = property.Parea,
                    NumofRooms = property.NumofRooms,
                    North = property.North,
                    South = property.South,
                    West = property.West,
                    East = property.East,
                    Price = property.Price,
                    PriceText = property.PriceText ?? string.Empty,
                    RoyaltyAmount = property.RoyaltyAmount,
                    PropertypeType = property.PropertyType?.Name,
                    CustomPropertyType = property.CustomPropertyType,
                    CreatedAt = property.CreatedAt,
                    CreatedAtFormatted = shamsiDate,
                    
                    // Location
                    Province = provinceData?.Name,
                    ProvinceDari = provinceData?.Dari,
                    District = districtData?.Name,
                    DistrictDari = districtData?.Dari,
                    Village = address?.Village,
                    
                    // All Sellers
                    Sellers = sellers,
                    
                    // All Buyers
                    Buyers = buyers,
                    
                    // Witnesses
                    Witnesses = witnesses,
                    
                    // Backward compatibility - first seller/buyer
                    SellerFirstName = sellers.FirstOrDefault()?.GetType().GetProperty("FirstName")?.GetValue(sellers.FirstOrDefault()),
                    SellerFatherName = sellers.FirstOrDefault()?.GetType().GetProperty("FatherName")?.GetValue(sellers.FirstOrDefault()),
                    SellerIndentityCardNumber = sellers.FirstOrDefault()?.GetType().GetProperty("ElectronicNationalIdNumber")?.GetValue(sellers.FirstOrDefault()),
                    SellerPhoto = sellers.FirstOrDefault()?.GetType().GetProperty("Photo")?.GetValue(sellers.FirstOrDefault()),
                    SellerProvince = sellers.FirstOrDefault()?.GetType().GetProperty("PaddressProvince")?.GetValue(sellers.FirstOrDefault()),
                    SellerProvinceDari = sellers.FirstOrDefault()?.GetType().GetProperty("PaddressProvinceDari")?.GetValue(sellers.FirstOrDefault()),
                    SellerDistrict = sellers.FirstOrDefault()?.GetType().GetProperty("PaddressDistrict")?.GetValue(sellers.FirstOrDefault()),
                    SellerDistrictDari = sellers.FirstOrDefault()?.GetType().GetProperty("PaddressDistrictDari")?.GetValue(sellers.FirstOrDefault()),
                    SellerVillage = sellers.FirstOrDefault()?.GetType().GetProperty("PaddressVillage")?.GetValue(sellers.FirstOrDefault()),
                    
                    BuyerFirstName = buyers.FirstOrDefault()?.GetType().GetProperty("FirstName")?.GetValue(buyers.FirstOrDefault()),
                    BuyerFatherName = buyers.FirstOrDefault()?.GetType().GetProperty("FatherName")?.GetValue(buyers.FirstOrDefault()),
                    BuyerIndentityCardNumber = buyers.FirstOrDefault()?.GetType().GetProperty("ElectronicNationalIdNumber")?.GetValue(buyers.FirstOrDefault()),
                    BuyerPhoto = buyers.FirstOrDefault()?.GetType().GetProperty("Photo")?.GetValue(buyers.FirstOrDefault()),
                    BuyerProvince = buyers.FirstOrDefault()?.GetType().GetProperty("PaddressProvince")?.GetValue(buyers.FirstOrDefault()),
                    BuyerProvinceDari = buyers.FirstOrDefault()?.GetType().GetProperty("PaddressProvinceDari")?.GetValue(buyers.FirstOrDefault()),
                    BuyerDistrict = buyers.FirstOrDefault()?.GetType().GetProperty("PaddressDistrict")?.GetValue(buyers.FirstOrDefault()),
                    BuyerDistrictDari = buyers.FirstOrDefault()?.GetType().GetProperty("PaddressDistrictDari")?.GetValue(buyers.FirstOrDefault()),
                    BuyerVillage = buyers.FirstOrDefault()?.GetType().GetProperty("PaddressVillage")?.GetValue(buyers.FirstOrDefault()),
                    
                    WitnessOneFirstName = witnesses.ElementAtOrDefault(0)?.FirstName,
                    WitnessOneFatherName = witnesses.ElementAtOrDefault(0)?.FatherName,
                    WitnessOneIndentityCardNumber = witnesses.ElementAtOrDefault(0)?.ElectronicNationalIdNumber,
                    WitnessTwoFirstName = witnesses.ElementAtOrDefault(1)?.FirstName,
                    WitnessTwoFatherName = witnesses.ElementAtOrDefault(1)?.FatherName,
                    WitnessTwoIndentityCardNumber = witnesses.ElementAtOrDefault(1)?.ElectronicNationalIdNumber,
                    
                    // Property Unit Type and Transaction Type
                    UnitType = property.PunitType?.Name,
                    TransactionType = property.TransactionType?.Name,
                    
                    // Property Documents and Images
                    FilePath = property.FilePath,
                    PreviousDocumentsPath = property.PreviousDocumentsPath,
                    ExistingDocumentsPath = property.ExistingDocumentsPath
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
