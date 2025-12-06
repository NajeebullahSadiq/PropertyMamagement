using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;

namespace WebAPIBackend.Controllers.Vehicles
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesSubController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public VehiclesSubController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("addBuyerDetails")]
        public async Task<ActionResult<int>> SaveBuyer([FromBody] VehiclesBuyerDetail request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            // Allow multiple buyers - removed restriction
            if (request.PropertyDetailsId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }
            //var user = await _userManager.GetUserAsync(User);

            var seller = new VehiclesBuyerDetail
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFather = request.GrandFather,
                IndentityCardNumber = request.IndentityCardNumber,
                PhoneNumber = request.PhoneNumber,
                PaddressProvinceId = request.PaddressProvinceId,
                PaddressDistrictId = request.PaddressDistrictId,
                PaddressVillage = request.PaddressVillage,
                TaddressProvinceId = request.TaddressProvinceId,
                TaddressDistrictId = request.TaddressDistrictId,
                TaddressVillage = request.TaddressVillage,
                CreatedAt = DateTime.Now,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                Photo=request.Photo,
                NationalIdCardPath = request.NationalIdCardPath,
                RoleType=request.RoleType ?? "Buyer",
                AuthorizationLetter=request.AuthorizationLetter,
            };
            try
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            var result = new { Id = seller.Id };
            return Ok(result);
        }

        [HttpPut("UpdateBuyer/{id}")]
        public async Task<IActionResult> UpdateBuyer(int id, [FromBody] VehiclesBuyerDetail request)
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

            var existingProperty = await _context.VehiclesBuyerDetails.FindAsync(id);
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
                    _context.Vehiclebuyeraudit.Add(new Vehiclebuyeraudit
                    {
                        VehicleBuyerId = existingProperty.Id,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.Now,
                        ColumnName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            var result = new { Id = request.Id};
            return Ok(result);
        }

        [HttpPost("addSellerDetails")]
        public async Task<ActionResult<int>> SaveSeller([FromBody] VehiclesSellerDetail request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            // Allow multiple sellers - removed restriction
            if (request.PropertyDetailsId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }
            //var user = await _userManager.GetUserAsync(User);

            var seller = new VehiclesSellerDetail
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFather = request.GrandFather,
                IndentityCardNumber = request.IndentityCardNumber,
                PhoneNumber = request.PhoneNumber,
                PaddressProvinceId = request.PaddressProvinceId,
                PaddressDistrictId = request.PaddressDistrictId,
                PaddressVillage = request.PaddressVillage,
                TaddressProvinceId = request.TaddressProvinceId,
                TaddressDistrictId = request.TaddressDistrictId,
                TaddressVillage = request.TaddressVillage,
                CreatedAt = DateTime.Now,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                Photo = request.Photo,
                NationalIdCardPath = request.NationalIdCardPath,
                RoleType=request.RoleType ?? "Seller",
                AuthorizationLetter=request.AuthorizationLetter,
            };
            try
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error saving seller: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Error saving seller: {ex.Message}");
            }
            var result = new { Id = seller.Id };
            return Ok(result);
        }

        [HttpPut("UpdateSeller/{id}")]
        public async Task<IActionResult> UpdateSeller(int id, [FromBody] VehiclesSellerDetail request)
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

            var existingProperty = await _context.VehiclesSellerDetails.FindAsync(id);
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
                    _context.Vehicleselleraudits.Add(new Vehicleselleraudit
                    {
                        VehicleSellerId = existingProperty.Id,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.Now,
                        ColumnName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            var result = new { Id = request.Id };
            return Ok(result);
        }

        [HttpPost("addWitnessdetails")]
        public async Task<ActionResult<int>> Savewithness([FromBody] VehiclesWitnessDetail request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            // Check if there are already 2 PropertyDetails associated with the current WitnessDetail
            int propertyCount = _context.VehiclesWitnessDetails.Count(wd => wd.PropertyDetailsId == request.PropertyDetailsId && wd.PropertyDetailsId != null);
            if (propertyCount >= 2)
            {
                return StatusCode(400, "You cannot add more than two");
            }
            if (request.PropertyDetailsId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }
            //var user = await _userManager.GetUserAsync(User);

            var witness = new VehiclesWitnessDetail
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                IndentityCardNumber = request.IndentityCardNumber,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.Now,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                NationalIdCard = request.NationalIdCard,
            };
            try
            {
                _context.Add(witness);
                await _context.SaveChangesAsync();

                // Update the IsComplete column of the PropertyDetails entity to true
                var propertyDetails = await _context.VehiclesPropertyDetails.FindAsync(request.PropertyDetailsId.Value);
                if (propertyDetails == null)
                {
                    return NotFound("PropertyDetails not found");
                }
                propertyDetails.iscomplete = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            var result = new { Id = witness.Id };
            return Ok(result);
        }

        [HttpPut("Updatewitness/{id}")]
        public async Task<IActionResult> UpdateWitness(int id, [FromBody] VehiclesWitnessDetail request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }


            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.VehiclesWitnessDetails.Any(i => i.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var result = new { Id = request.Id };
            return Ok(result);
        }

        [HttpGet("Witness/{id}")]
        public async Task<IActionResult> GetWitnessById(int id)
        {
            try
            {
                var Pro = await _context.VehiclesWitnessDetails.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("Buyer/{id}")]
        public async Task<IActionResult> GetByerById(int id)
        {
            try
            {
                var Pro = await _context.VehiclesBuyerDetails.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("Seller/{id}")]
        public async Task<IActionResult> GetSellerById(int id)
        {
            try
            {
                var Pro = await _context.VehiclesSellerDetails.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpDelete("Seller/{id}")]
        public async Task<IActionResult> DeleteSeller(int id)
        {
            try
            {
                var seller = await _context.VehiclesSellerDetails.FindAsync(id);
                if (seller == null)
                {
                    return NotFound();
                }

                _context.VehiclesSellerDetails.Remove(seller);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Seller deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Buyer/{id}")]
        public async Task<IActionResult> DeleteBuyer(int id)
        {
            try
            {
                var buyer = await _context.VehiclesBuyerDetails.FindAsync(id);
                if (buyer == null)
                {
                    return NotFound();
                }

                _context.VehiclesBuyerDetails.Remove(buyer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Buyer deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
