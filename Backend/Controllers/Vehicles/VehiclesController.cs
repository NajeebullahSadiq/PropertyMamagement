using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Helpers;

namespace WebAPIBackend.Controllers.Vehicles
{
   
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public VehiclesController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return Unauthorized();
            }

            string userId = userIdClaim.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Check if user can access vehicle module
            if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "vehicle"))
            {
                return StatusCode(403, new { message = "شما اجازه دسترسی به ماژول وسایط نقلیه را ندارید" });
            }

            IQueryable<VehiclesPropertyDetail> propertyQuery;

            // Check if user can view all records
            if (RbacHelper.CanViewAllRecords(roles, "vehicle"))
            {
                propertyQuery = _context.VehiclesPropertyDetails;
            }
            else
            {
                // Filter the data based on the current user's ID
                propertyQuery = _context.VehiclesPropertyDetails.Where(p => p.CreatedBy == userId);
            }
            try
            {
                var result = await propertyQuery
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        p.PermitNo,
                        p.PilateNo,
                        p.TypeOfVehicle,
                        p.Model,
                        p.EnginNo,
                        p.ShasiNo,
                        p.Color,
                        p.Price,
                        p.PriceText,
                        p.Des,
                        p.RoyaltyAmount,
                        p.FilePath,
                        p.VehicleHand,
                        p.iscomplete,
                        SellerName = p.VehiclesSellerDetails.Select(s => s.FirstName).FirstOrDefault(),
                        BuyerName = p.VehiclesBuyerDetails.Select(b => b.FirstName).FirstOrDefault(),
                    })
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            try
            {
                var Pro = await _context.VehiclesPropertyDetails.Where(x => x.Id.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// Get complete vehicle details for view page (read-only)
        /// </summary>
        [HttpGet("View/{id}")]
        public async Task<IActionResult> GetVehicleViewById(int id)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
                {
                    return Unauthorized();
                }

                string userId = userIdClaim.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Check if user can access vehicle module
                if (!RbacHelper.CanAccessModule(roles, user.LicenseType, "vehicle"))
                {
                    return StatusCode(403, new { message = "شما اجازه دسترسی به ماژول وسایط نقلیه را ندارید" });
                }

                // Get vehicle details with all related data
                var vehicle = await _context.VehiclesPropertyDetails
                    .AsNoTracking()
                    .Include(v => v.TransactionType)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (vehicle == null)
                {
                    return NotFound(new { message = "وسیله نقلیه یافت نشد" });
                }

                // Check if user can view this record
                if (!RbacHelper.CanViewAllRecords(roles, "vehicle") && vehicle.CreatedBy != userId)
                {
                    return StatusCode(403, new { message = "شما اجازه مشاهده این رکورد را ندارید" });
                }

                // Get sellers with location names
                // Note: Some columns (NationalIdCardPath, RoleType, AuthorizationLetter, HeirsLetter) may not exist in DB
                var sellers = await _context.VehiclesSellerDetails
                    .AsNoTracking()
                    .Where(s => s.PropertyDetailsId == id)
                    .Select(s => new
                    {
                        s.Id,
                        s.FirstName,
                        s.FatherName,
                        s.GrandFather,
                        s.ElectronicNationalIdNumber,
                        s.PhoneNumber,
                        s.Photo,
                        PermanentProvinceName = s.PaddressProvince != null ? s.PaddressProvince.Dari : null,
                        PermanentDistrictName = s.PaddressDistrict != null ? s.PaddressDistrict.Dari : null,
                        PaddressVillage = s.PaddressVillage,
                        TemporaryProvinceName = s.TaddressProvince != null ? s.TaddressProvince.Dari : null,
                        TemporaryDistrictName = s.TaddressDistrict != null ? s.TaddressDistrict.Dari : null,
                        TaddressVillage = s.TaddressVillage,
                        s.CreatedAt
                    })
                    .ToListAsync();

                // Get buyers with location names
                // Note: Some columns (NationalIdCardPath, RoleType, AuthorizationLetter, RentStartDate, RentEndDate) may not exist in DB
                var buyers = await _context.VehiclesBuyerDetails
                    .AsNoTracking()
                    .Where(b => b.PropertyDetailsId == id)
                    .Select(b => new
                    {
                        b.Id,
                        b.FirstName,
                        b.FatherName,
                        b.GrandFather,
                        b.ElectronicNationalIdNumber,
                        b.PhoneNumber,
                        b.Photo,
                        PermanentProvinceName = b.PaddressProvince != null ? b.PaddressProvince.Dari : null,
                        PermanentDistrictName = b.PaddressDistrict != null ? b.PaddressDistrict.Dari : null,
                        PaddressVillage = b.PaddressVillage,
                        TemporaryProvinceName = b.TaddressProvince != null ? b.TaddressProvince.Dari : null,
                        TemporaryDistrictName = b.TaddressDistrict != null ? b.TaddressDistrict.Dari : null,
                        TaddressVillage = b.TaddressVillage,
                        b.CreatedAt
                    })
                    .ToListAsync();

                // Get witnesses
                var witnesses = await _context.VehiclesWitnessDetails
                    .AsNoTracking()
                    .Where(w => w.PropertyDetailsId == id)
                    .Select(w => new
                    {
                        w.Id,
                        w.FirstName,
                        w.FatherName,
                        w.ElectronicNationalIdNumber,
                        w.PhoneNumber,
                        w.CreatedAt
                    })
                    .ToListAsync();

                // Build response
                var result = new
                {
                    // Vehicle Details
                    vehicle.Id,
                    vehicle.PermitNo,
                    vehicle.PilateNo,
                    vehicle.TypeOfVehicle,
                    vehicle.Model,
                    vehicle.EnginNo,
                    vehicle.ShasiNo,
                    vehicle.Color,
                    vehicle.VehicleHand,
                    
                    // Transaction Info
                    TransactionTypeName = vehicle.TransactionType?.Name,
                    vehicle.Price,
                    vehicle.PriceText,
                    vehicle.RoyaltyAmount,
                    vehicle.Des,
                    
                    // Documents
                    vehicle.FilePath,
                    
                    // Status
                    vehicle.iscomplete,
                    vehicle.iseditable,
                    vehicle.CreatedAt,
                    
                    // Related entities
                    Sellers = sellers,
                    Buyers = buyers,
                    Witnesses = witnesses
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] VehiclesPropertyDetail request)
        {

            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            var property = new VehiclesPropertyDetail
            {
                PermitNo = request.PermitNo,
                PilateNo = request.PilateNo,
                TypeOfVehicle = request.TypeOfVehicle,
                Model = request.Model,
                EnginNo = request.EnginNo,
                ShasiNo = request.ShasiNo,
                Color = request.Color,
                PropertyTypeId = request.PropertyTypeId,
                Price = request.Price,
                PriceText = request.PriceText,
                RoyaltyAmount = !string.IsNullOrWhiteSpace(request.Price) ? (decimal.Parse(request.Price) * 0.01m).ToString() : null,
                TransactionTypeId = request.TransactionTypeId,
                Des = request.Des,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                FilePath = request.FilePath,
                VehicleHand = request.VehicleHand,
                iscomplete=false,
                iseditable=false
            };
            _context.Add(property);
            await _context.SaveChangesAsync();
            var result = new { Id = property.Id, PropertyTypeId = property.PropertyTypeId };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicleDetails(int id, [FromBody] VehiclesPropertyDetail request)
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

            var existingProperty = await _context.VehiclesPropertyDetails.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
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
                    _context.Vehicleaudits.Add(new Vehicleaudit
                    {
                        VehicleId = existingProperty.Id,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.Now,
                        ColumnName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            var result = new { Id = request.Id, PropertyTypeId = request.PropertyTypeId };
            return Ok(result);
        }

        [HttpGet("GetPrintRecord/{id}")]
        public async Task<IActionResult> GetPrintRecordById(int id, [FromQuery] string? calendarType = null)
        {
            // Call the DbContext to retrieve the data by ID
            var data = await _context.getVehiclePrintData
                .FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return NotFound(); // Return 404 if the data with the given ID is not found
            }

            // Convert the 'CreatedAt' property to the requested calendar format
            var calendar = Helpers.DateConversionHelper.ParseCalendarType(calendarType);
            string shamsiDate = Helpers.DateConversionHelper.FormatDate(data.CreatedAt, calendar);

            // Create a custom result object with the desired properties
            var result = new
            {
                data.Id,
                data.PermitNo,
                data.PilateNo,
                data.TypeOfVehicle,
                data.Model,
                data.EnginNo,
                data.ShasiNo,
                data.Color,
                data.Price,
                data.PriceText,
                data.RoyaltyAmount,
                data.Description,
                data.SellerFirstName,
                data.SellerFatherName,
                data.SellerIndentityCardNumber,
                data.SellerVillage,
                data.tSellerVillage,
                data.SellerPhoto,
                SellerProvince = data.SellerProvince,
                SellerDistrict = data.SellerDistrict,
                TSellerProvince = data.tSellerProvince,
                TSellerDistrict = data.tSellerDistrict,
                data.BuyerFirstName,
                data.BuyerFatherName,
                data.BuyerIndentityCardNumber,
                data.BuyerVillage,
                data.tBuyerVillage,
                data.BuyerPhoto,
                BuyerProvince = data.BuyerProvince,
                BuyerDistrict = data.BuyerDistrict,
                TBuyerProvince = data.tBuyerProvince,
                TBuyerDistrict = data.tBuyerDistrict,
                data.WitnessOneFirstName,
                data.WitnessOneFatherName,
                data.WitnessOneIndentityCardNumber,
                data.WitnessTwoFirstName,
                data.WitnessTwoFatherName,
                data.WitnessTwoIndentityCardNumber,
                data.CreatedAt,
                CreatedAtFormatted = shamsiDate // Add the converted date to the result object
            };

            return Ok(result); // Return the data as JSON if found
        }
    }
}
