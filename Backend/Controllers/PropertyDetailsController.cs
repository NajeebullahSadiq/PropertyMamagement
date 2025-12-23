using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;

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

            IQueryable<PropertyDetail> propertyQuery;

            // Check if the user is an admin
            if (roles.Contains("ADMIN"))
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
                             PropertyTypeText = p.PropertyType.Name,
                             UnitTypeText = p.PunitType.Name,
                             TransactionTypeText = p.TransactionType.Name,
                             p.iscomplete,
                             SellerName = (p.SellerDetails != null && p.SellerDetails.Any()) ? p.SellerDetails.First().FirstName : null,
                             BuyerName = (p.BuyerDetails != null && p.BuyerDetails.Any()) ? p.BuyerDetails.First().FirstName : null,
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
        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] PropertyDetail request)
        {

            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            //  var userId = 25;
            var property = new PropertyDetail
            {
                Pnumber = request.Pnumber == 0 ? 0 : request.Pnumber,
                Parea = request.Parea,
                PunitTypeId = request.PunitTypeId,
                NumofFloor=request.NumofFloor,
                NumofRooms=request.NumofRooms,
                PropertyTypeId=request.PropertyTypeId,
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
                IssuanceDate=request.IssuanceDate,
                SerialNumber=request.SerialNumber,
                TransactionDate=request.TransactionDate,
                iscomplete = false,
                iseditable = false
                
            };
            _context.Add(property);
            await _context.SaveChangesAsync();
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

            var result = new { Id = request.Id, PropertyTypeId = request.PropertyTypeId };
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetPrintRecord/{id}")]
        public async Task<IActionResult> GetPrintRecordById(int id)
        {
            // Call the DbContext to retrieve the data by ID
            var data = await _context.GetPrintType
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return NotFound(); // Return 404 if the data with the given ID is not found
            }

            // Convert the 'CreatedAt' property to Shamsi (Persian) date format
            var persianCalendar = new PersianCalendar();
            string shamsiDate = $"{persianCalendar.GetYear(data.CreatedAt)}/{persianCalendar.GetMonth(data.CreatedAt)}/{persianCalendar.GetDayOfMonth(data.CreatedAt)}";

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
                PriceText = data.PriceText,
                RoyaltyAmount = data.RoyaltyAmount,
                PropertypeType = data.PropertypeType,
                CreatedAt = data.CreatedAt,
                // Location - Province and District
                Province = data.Province,
                District = data.District,
                Village=data.Village,
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
                tSellerProvince = data.tSellerProvince,
                tSellerDistrict = data.tSellerDistrict,

                // BuyerDetails
                BuyerFirstName = data.BuyerFirstName,
                BuyerFatherName = data.BuyerFatherName,
                BuyerIndentityCardNumber = data.BuyerIndentityCardNumber,
                BuyerVillage = data.BuyerVillage,
                tBuyerVillage=data.tBuyerVillage,
                BuyerPhoto = data.BuyerPhoto,

                // Location - BuyerProvince and BuyerDistrict
                BuyerProvince = data.BuyerProvince,
                BuyerDistrict = data.BuyerDistrict,
                tBuyerProvince = data.tBuyerProvince,
                tBuyerDistrict = data.tBuyerDistrict,

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
                CreatedAtShamsi = shamsiDate // Add the converted Shamsi date to the result object
            };

            return Ok(result); // Return the data as JSON if found
        }

    }
}
