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
                RoyaltyAmount = request.Price*0.01,
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
