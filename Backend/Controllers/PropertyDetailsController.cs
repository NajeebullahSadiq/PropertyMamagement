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
                    CreatedAt = DateTime.Now,
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
            else
            {
                // Filter the data based on the current user's ID
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
                             SellerIndentityCardNumber = (p.SellerDetails != null && p.SellerDetails.Any()) ? p.SellerDetails.First().IndentityCardNumber : null,
                             BuyerIndentityCardNumber = (p.BuyerDetails != null && p.BuyerDetails.Any()) ? p.BuyerDetails.First().IndentityCardNumber : null,
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
                var Pro = await _context.PropertyDetails.Where(x => x.Id.Equals(id)).ToListAsync();

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
            else
            {
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
                            s.IndentityCardNumber,
                            s.TazkiraType,
                            s.TazkiraVolume,
                            s.TazkiraPage,
                            s.TazkiraNumber,
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
                            b.IndentityCardNumber,
                            b.TazkiraType,
                            b.TazkiraVolume,
                            b.TazkiraPage,
                            b.TazkiraNumber,
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
                            w.IndentityCardNumber,
                            w.TazkiraType,
                            w.TazkiraVolume,
                            w.TazkiraPage,
                            w.TazkiraNumber,
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

            var property = new PropertyDetail
            {
                Pnumber = request.Pnumber == 0 ? 0 : request.Pnumber,
                Parea = request.Parea,
                PunitTypeId = request.PunitTypeId,
                NumofFloor=request.NumofFloor,
                NumofRooms=request.NumofRooms,
                PropertyTypeId=request.PropertyTypeId,
                CustomPropertyType = normalizedCustomPropertyType,
                Price=request.Price,
                PriceText=request.PriceText,
                RoyaltyAmount= (request.Price ?? 0) * 0.01,
                TransactionTypeId=request.TransactionTypeId,
                Des=request.Des,
                CreatedAt=DateTime.Now,
                CreatedBy= userId,
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

            // Update the entity with the new values
            _context.Entry(existingProperty).CurrentValues.SetValues(request);

            // Restore the original values of the CreatedBy and CreatedOn properties
            existingProperty.CreatedBy = createdBy;
            existingProperty.CreatedAt = createdAt;

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
                        UpdatedAt = DateTime.Now,
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
        [HttpGet("GetPrintRecord/{id}")]
        public async Task<IActionResult> GetPrintRecordById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                // Call the DbContext to retrieve the data by ID
                var data = await _context.GetPrintType
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (data == null)
                {
                    return NotFound(); // Return 404 if the data with the given ID is not found
                }

                // Convert the 'CreatedAt' property to the requested calendar format
                var calendar = Helpers.DateConversionHelper.ParseCalendarType(calendarType);
                string shamsiDate = string.Empty;
                if (data.CreatedAt.HasValue)
                {
                    shamsiDate = Helpers.DateConversionHelper.FormatDate(data.CreatedAt.Value, calendar);
                }

                // Create a custom result object with the desired properties
                var result = new
                {
                    Id = data.Id,
                    DocumentType = data.DocumentType,
                    IssuanceNumber = data.IssuanceNumber,
                    IssuanceDate = data.IssuanceDate,
                    SerialNumber = data.SerialNumber,
                    TransactionDate = data.TransactionDate,
                    PNumber = data.PNumber,
                    PArea = data.PArea,
                    NumofRooms = data.NumofRooms,
                    North = data.North,
                    South = data.South,
                    West = data.West,
                    East = data.East,
                    Price = data.Price,
                    PriceText = data.PriceText ?? string.Empty,
                    RoyaltyAmount = data.RoyaltyAmount,
                    PropertypeType = data.PropertypeType,
                    CreatedAt = data.CreatedAt,
                    // Location - Province and District
                    Province = data.Province,
                    District = data.District,
                    ProvinceDari = data.ProvinceDari,
                    DistrictDari = data.DistrictDari,
                    Village = data.Village,
                    // SellerDetails
                    SellerFirstName = data.SellerFirstName,
                    SellerFatherName = data.SellerFatherName,
                    SellerIndentityCardNumber = data.SellerIndentityCardNumber,
                    SellerVillage = data.SellerVillage,
                    tSellerVillage = data.tSellerVillage,
                    SellerPhoto = data.SellerPhoto,

                    // Location - SellerProvince and SellerDistrict
                    SellerProvince = data.SellerProvince,
                    SellerDistrict = data.SellerDistrict,
                    SellerProvinceDari = data.SellerProvinceDari,
                    SellerDistrictDari = data.SellerDistrictDari,
                    tSellerProvince = data.tSellerProvince,
                    tSellerDistrict = data.tSellerDistrict,
                    tSellerProvinceDari = data.tSellerProvinceDari,
                    tSellerDistrictDari = data.tSellerDistrictDari,

                    // BuyerDetails
                    BuyerFirstName = data.BuyerFirstName,
                    BuyerFatherName = data.BuyerFatherName,
                    BuyerIndentityCardNumber = data.BuyerIndentityCardNumber,
                    BuyerVillage = data.BuyerVillage,
                    tBuyerVillage = data.tBuyerVillage,
                    BuyerPhoto = data.BuyerPhoto,

                    // Location - BuyerProvince and BuyerDistrict
                    BuyerProvince = data.BuyerProvince,
                    BuyerDistrict = data.BuyerDistrict,
                    BuyerProvinceDari = data.BuyerProvinceDari,
                    BuyerDistrictDari = data.BuyerDistrictDari,
                    tBuyerProvince = data.tBuyerProvince,
                    tBuyerDistrict = data.tBuyerDistrict,
                    tBuyerProvinceDari = data.tBuyerProvinceDari,
                    tBuyerDistrictDari = data.tBuyerDistrictDari,

                    // WitnessDetails
                    WitnessOneFirstName = data.WitnessOneFirstName,
                    WitnessOneFatherName = data.WitnessOneFatherName,
                    WitnessOneIndentityCardNumber = data.WitnessOneIndentityCardNumber,

                    WitnessTwoFirstName = data.WitnessTwoFirstName,
                    WitnessTwoFatherName = data.WitnessTwoFatherName,
                    WitnessTwoIndentityCardNumber = data.WitnessTwoIndentityCardNumber,

                    // PropertyUnitType and TransactionType
                    UnitType = data.UnitType,
                    TransactionType = data.TransactionType,
                    CreatedAtFormatted = shamsiDate,

                    // Property Documents and Images
                    FilePath = data.FilePath,
                    PreviousDocumentsPath = data.PreviousDocumentsPath,
                    ExistingDocumentsPath = data.ExistingDocumentsPath
                };

                return Ok(result); // Return the data as JSON if found
            }
            catch (Exception ex)
            {
                var hint = ex.Message != null && ex.Message.Contains("GetPrintType")
                    ? "Database view 'GetPrintType' is missing. Apply the latest migrations / SQL view scripts."
                    : string.Empty;

                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving print record",
                    error = ex.Message,
                    hint
                });
            }
        }

    }
}
