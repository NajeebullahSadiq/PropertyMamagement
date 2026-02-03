using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers.Vehicles
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesSubController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        
        // Allowed seller role types for Vehicle module
        private static readonly string[] AllowedSellerRoles = { "Seller", "Sellers", "Sales Agent", "Heirs" };
        
        // Allowed buyer role types for Vehicle module (restricted to 3 options)
        private static readonly string[] AllowedBuyerRoles = { "Buyer", "Buyers", "Purchase Agent" };
        
        // Single-buyer roles (only one buyer allowed)
        private static readonly string[] SingleBuyerRoles = { "Buyer", "Purchase Agent" };
        
        public VehiclesSubController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get allowed seller role types for Vehicle module
        /// </summary>
        [HttpGet("AllowedSellerRoles")]
        public IActionResult GetAllowedSellerRoles()
        {
            var roles = new[]
            {
                new { value = "Seller", label = "فروشنده", allowMultiple = false },
                new { value = "Sellers", label = "فروشندگان", allowMultiple = true },
                new { value = "Sales Agent", label = "وکیل فروش", allowMultiple = false },
                new { value = "Heirs", label = "ورثه", allowMultiple = true }
            };
            return Ok(roles);
        }

        /// <summary>
        /// Get allowed buyer role types for Vehicle module (restricted to 3 options)
        /// </summary>
        [HttpGet("AllowedBuyerRoles")]
        public IActionResult GetAllowedBuyerRoles()
        {
            var roles = new[]
            {
                new { value = "Buyer", label = "مشتری", allowMultiple = false },
                new { value = "Buyers", label = "مشتریان", allowMultiple = true },
                new { value = "Purchase Agent", label = "وکیل خرید", allowMultiple = false }
            };
            return Ok(roles);
        }

        [HttpPost("CheckDuplicateSeller")]
        public async Task<IActionResult> CheckDuplicateSeller([FromBody] DuplicateOwnerCheckRequest request)
        {
            try
            {
                var firstName = request.FirstName?.Trim() ?? "";
                var fatherName = request.FatherName?.Trim() ?? "";
                var grandFather = request.GrandFather?.Trim() ?? "";
                var propertyDetailsId = request.PropertyDetailsId;

                // Get the property to find its transaction type
                var property = await _context.VehiclesPropertyDetails.FindAsync(propertyDetailsId);
                if (property == null)
                {
                    return Ok(new { isDuplicate = false, message = "" });
                }

                // Transaction types that trigger duplicate check: Sale, Rent, Revocable Sale
                var restrictedTransactionTypeIds = new[] { 1, 2, 3 }; // Adjust based on your DB
                
                // Check if current property has a restricted transaction type
                if (property.TransactionTypeId.HasValue && !restrictedTransactionTypeIds.Contains(property.TransactionTypeId.Value))
                {
                    return Ok(new { isDuplicate = false, message = "" });
                }

                // Find all active registrations with same seller identity (excluding cancelled ones)
                var duplicates = await _context.VehiclesSellerDetails
                    .Where(s => 
                        (s.FirstName ?? "").Trim() == firstName &&
                        (s.FatherName ?? "").Trim() == fatherName &&
                        (s.GrandFather ?? "").Trim() == grandFather &&
                        s.PropertyDetails != null &&
                        s.PropertyDetails.TransactionTypeId.HasValue &&
                        restrictedTransactionTypeIds.Contains(s.PropertyDetails.TransactionTypeId.Value) &&
                        !_context.PropertyCancellations.Any(c => c.PropertyDetailsId == s.PropertyDetailsId) &&
                        s.Id != request.SellerId) // Exclude current seller if editing
                    .Include(s => s.PropertyDetails)
                    .ThenInclude(p => p!.TransactionType)
                    .ToListAsync();

                if (duplicates.Count > 0)
                {
                    var existingTransaction = duplicates.First();
                    var transactionTypeName = existingTransaction.PropertyDetails?.TransactionType?.Name ?? "Unknown";
                    
                    var message = $"این ملک قبلاً توسط همین فروشنده برای {GetDariTransactionType(transactionTypeName)} ثبت شده است. تا زمان ختم یا ابطال معامله قبلی، ثبت دوباره اجازه نیست.";
                    
                    return Ok(new { isDuplicate = true, message = message, transactionType = transactionTypeName });
                }

                return Ok(new { isDuplicate = false, message = "" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("CheckDuplicateBuyer")]
        public async Task<IActionResult> CheckDuplicateBuyer([FromBody] DuplicateOwnerCheckRequest request)
        {
            try
            {
                var firstName = request.FirstName?.Trim() ?? "";
                var fatherName = request.FatherName?.Trim() ?? "";
                var grandFather = request.GrandFather?.Trim() ?? "";
                var propertyDetailsId = request.PropertyDetailsId;

                // Get the property to find its transaction type
                var property = await _context.VehiclesPropertyDetails.FindAsync(propertyDetailsId);
                if (property == null)
                {
                    return Ok(new { isDuplicate = false, message = "" });
                }

                // Transaction types that trigger duplicate check: Sale, Rent, Revocable Sale
                var restrictedTransactionTypeIds = new[] { 1, 2, 3 }; // Adjust based on your DB
                
                // Check if current property has a restricted transaction type
                if (property.TransactionTypeId.HasValue && !restrictedTransactionTypeIds.Contains(property.TransactionTypeId.Value))
                {
                    return Ok(new { isDuplicate = false, message = "" });
                }

                // Find all active registrations with same buyer identity (excluding cancelled ones)
                var duplicates = await _context.VehiclesBuyerDetails
                    .Where(b => 
                        (b.FirstName ?? "").Trim() == firstName &&
                        (b.FatherName ?? "").Trim() == fatherName &&
                        (b.GrandFather ?? "").Trim() == grandFather &&
                        b.PropertyDetails != null &&
                        b.PropertyDetails.TransactionTypeId.HasValue &&
                        restrictedTransactionTypeIds.Contains(b.PropertyDetails.TransactionTypeId.Value) &&
                        !_context.PropertyCancellations.Any(c => c.PropertyDetailsId == b.PropertyDetailsId) &&
                        b.Id != request.SellerId) // Exclude current buyer if editing
                    .Include(b => b.PropertyDetails)
                    .ThenInclude(p => p!.TransactionType)
                    .ToListAsync();

                if (duplicates.Count > 0)
                {
                    var existingTransaction = duplicates.First();
                    var transactionTypeName = existingTransaction.PropertyDetails?.TransactionType?.Name ?? "Unknown";
                    
                    var message = $"این ملک قبلاً توسط همین مشتری برای {GetDariTransactionType(transactionTypeName)} ثبت شده است. تا زمان ختم یا ابطال معامله قبلی، ثبت دوباره اجازه نیست.";
                    
                    return Ok(new { isDuplicate = true, message = message, transactionType = transactionTypeName });
                }

                return Ok(new { isDuplicate = false, message = "" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private string GetDariTransactionType(string transactionTypeName)
        {
            return transactionTypeName switch
            {
                "Sale" => "فروش",
                "Rent" => "کرایه",
                "Revocable Sale" => "بیع جایزی",
                _ => transactionTypeName
            };
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
            
            if (request.PropertyDetailsId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            // Validate role type - only 3 approved options allowed for Vehicle buyers
            var roleType = request.RoleType ?? "Buyer";
            
            if (!AllowedBuyerRoles.Contains(roleType))
            {
                return StatusCode(400, "Invalid buyer role type. Allowed values: Buyer, Buyers, Purchase Agent (مشتری، مشتریان، وکیل خرید)");
            }

            // Single buyer roles (Buyer, Purchase Agent) - check if already has a buyer
            if (SingleBuyerRoles.Contains(roleType))
            {
                var existingBuyerCount = await _context.VehiclesBuyerDetails
                    .CountAsync(b => b.PropertyDetailsId == request.PropertyDetailsId);
                if (existingBuyerCount > 0)
                {
                    return StatusCode(400, "برای نوعیت «مشتری» یا «وکیل خرید» فقط یک مشتری مجاز است. برای ثبت چندین مشتری، نوعیت «مشتریان» را انتخاب کنید.");
                }
            }
            
            // If agent role (Purchase Agent), authorization letter is required
            if (roleType == "Purchase Agent" && string.IsNullOrWhiteSpace(request.AuthorizationLetter))
            {
                return StatusCode(400, "وکالت نامه برای وکیل خرید الزامی است");
            }

            var seller = new VehiclesBuyerDetail
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFather = request.GrandFather,
                ElectronicNationalIdNumber = request.ElectronicNationalIdNumber,
                PhoneNumber = request.PhoneNumber,
                PaddressProvinceId = request.PaddressProvinceId,
                PaddressDistrictId = request.PaddressDistrictId,
                PaddressVillage = request.PaddressVillage,
                TaddressProvinceId = request.TaddressProvinceId,
                TaddressDistrictId = request.TaddressDistrictId,
                TaddressVillage = request.TaddressVillage,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                Photo=request.Photo,
                NationalIdCardPath = request.NationalIdCardPath,
                RoleType=roleType,
                AuthorizationLetter=request.AuthorizationLetter,
                RentStartDate = request.RentStartDate,
                RentEndDate = request.RentEndDate,
            };
            try
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
                
                // Update completion status
                if (request.PropertyDetailsId.HasValue && request.PropertyDetailsId.Value > 0)
                {
                    await UpdateVehicleCompletionStatus(request.PropertyDetailsId.Value);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving buyer: {ex.Message}");
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

            // Validate role type - only 3 approved options allowed for Vehicle buyers
            var roleType = request.RoleType ?? "Buyer";
            
            if (!AllowedBuyerRoles.Contains(roleType))
            {
                return StatusCode(400, "Invalid buyer role type. Allowed values: Buyer, Buyers, Purchase Agent (مشتری، مشتریان، وکیل خرید)");
            }
            
            // If agent role (Purchase Agent), authorization letter is required
            if (roleType == "Purchase Agent" && string.IsNullOrWhiteSpace(request.AuthorizationLetter))
            {
                return StatusCode(400, "وکالت نامه برای وکیل خرید الزامی است");
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
                        UpdatedAt = DateTime.UtcNow,
                        ColumnName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Update completion status
            if (existingProperty.PropertyDetailsId.HasValue && existingProperty.PropertyDetailsId.Value > 0)
            {
                await UpdateVehicleCompletionStatus(existingProperty.PropertyDetailsId.Value);
            }

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
            
            if (request.PropertyDetailsId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            // Validate role type - only 4 approved options allowed
            var roleType = request.RoleType ?? "Seller";
            var allowedRoles = new[] { "Seller", "Sellers", "Sales Agent", "Heirs" };
            
            if (!allowedRoles.Contains(roleType))
            {
                return StatusCode(400, "Invalid seller role type. Allowed values: Seller, Sellers, Sales Agent, Heirs");
            }

            // Single seller roles (Seller, Sales Agent) - check if already has a seller
            var singleSellerRoles = new[] { "Seller", "Sales Agent" };
            if (singleSellerRoles.Contains(roleType))
            {
                var existingSellerCount = await _context.VehiclesSellerDetails
                    .CountAsync(s => s.PropertyDetailsId == request.PropertyDetailsId);
                if (existingSellerCount > 0)
                {
                    return StatusCode(400, "برای نوعیت «فروشنده» یا «وکیل فروش» فقط یک فروشنده مجاز است");
                }
            }
            
            // If agent role (Sales Agent), authorization letter is required
            if (roleType == "Sales Agent" && string.IsNullOrWhiteSpace(request.AuthorizationLetter))
            {
                return StatusCode(400, "Authorization letter is required for agent roles");
            }
            
            // If heirs role, heirs letter is required
            if (roleType == "Heirs" && string.IsNullOrWhiteSpace(request.HeirsLetter))
            {
                return StatusCode(400, "Heirs letter is required for heirs role");
            }

            var seller = new VehiclesSellerDetail
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFather = request.GrandFather,
                ElectronicNationalIdNumber = request.ElectronicNationalIdNumber,
                PhoneNumber = request.PhoneNumber,
                PaddressProvinceId = request.PaddressProvinceId,
                PaddressDistrictId = request.PaddressDistrictId,
                PaddressVillage = request.PaddressVillage,
                TaddressProvinceId = request.TaddressProvinceId,
                TaddressDistrictId = request.TaddressDistrictId,
                TaddressVillage = request.TaddressVillage,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                Photo = request.Photo,
                NationalIdCardPath = request.NationalIdCardPath,
                RoleType=roleType,
                AuthorizationLetter=request.AuthorizationLetter,
                HeirsLetter=request.HeirsLetter,
            };
            try
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
                
                // Update completion status
                if (request.PropertyDetailsId.HasValue && request.PropertyDetailsId.Value > 0)
                {
                    await UpdateVehicleCompletionStatus(request.PropertyDetailsId.Value);
                }
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

            // Validate role type - only 4 approved options allowed
            var roleType = request.RoleType ?? "Seller";
            var allowedRoles = new[] { "Seller", "Sellers", "Sales Agent", "Heirs" };
            
            if (!allowedRoles.Contains(roleType))
            {
                return StatusCode(400, "Invalid seller role type. Allowed values: Seller, Sellers, Sales Agent, Heirs");
            }
            
            // If agent role (Sales Agent), authorization letter is required
            if (roleType == "Sales Agent" && string.IsNullOrWhiteSpace(request.AuthorizationLetter))
            {
                return StatusCode(400, "Authorization letter is required for agent roles");
            }
            
            // If heirs role, heirs letter is required
            if (roleType == "Heirs" && string.IsNullOrWhiteSpace(request.HeirsLetter))
            {
                return StatusCode(400, "Heirs letter is required for heirs role");
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
                        UpdatedAt = DateTime.UtcNow,
                        ColumnName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Update completion status
            if (existingProperty.PropertyDetailsId.HasValue && existingProperty.PropertyDetailsId.Value > 0)
            {
                await UpdateVehicleCompletionStatus(existingProperty.PropertyDetailsId.Value);
            }

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
                GrandFatherName = request.GrandFatherName,
                ElectronicNationalIdNumber = request.ElectronicNationalIdNumber,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                NationalIdCard = request.NationalIdCard,
                WitnessSide = request.WitnessSide,
                Des = request.Des,
            };
            try
            {
                _context.Add(witness);
                await _context.SaveChangesAsync();

                // Update the IsComplete status based on validation
                if (request.PropertyDetailsId.HasValue)
                {
                    await UpdateVehicleCompletionStatus(request.PropertyDetailsId.Value);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving witness: {ex.Message}");
            }
            var result = new { Id = witness.Id };
            return Ok(result);
        }

        private async Task UpdateVehicleCompletionStatus(int vehicleDetailsId)
        {
            var vehicleDetails = await _context.VehiclesPropertyDetails
                .Include(v => v.VehiclesSellerDetails)
                .Include(v => v.VehiclesBuyerDetails)
                .Include(v => v.VehiclesWitnessDetails)
                .FirstOrDefaultAsync(v => v.Id == vehicleDetailsId);

            if (vehicleDetails == null)
            {
                return;
            }

            // Check if all required fields are filled
            bool isComplete = true;

            // 1. Check if vehicle details has required fields (using correct field names)
            if (string.IsNullOrWhiteSpace(vehicleDetails.TypeOfVehicle) ||
                string.IsNullOrWhiteSpace(vehicleDetails.PilateNo))
            {
                isComplete = false;
            }

            // 2. Check if at least one seller exists
            if (vehicleDetails.VehiclesSellerDetails == null || !vehicleDetails.VehiclesSellerDetails.Any())
            {
                isComplete = false;
            }

            // 3. Check if at least one buyer exists
            if (vehicleDetails.VehiclesBuyerDetails == null || !vehicleDetails.VehiclesBuyerDetails.Any())
            {
                isComplete = false;
            }

            // 4. Check if exactly two witnesses exist (one from each side)
            if (vehicleDetails.VehiclesWitnessDetails == null || vehicleDetails.VehiclesWitnessDetails.Count != 2)
            {
                isComplete = false;
            }
            else
            {
                // Check if witnesses are from different sides
                var witnessSides = vehicleDetails.VehiclesWitnessDetails
                    .Select(w => w.WitnessSide)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToList();
                
                if (witnessSides.Count != 2)
                {
                    isComplete = false;
                }
            }

            vehicleDetails.iscomplete = isComplete;
            await _context.SaveChangesAsync();
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

                // Update the IsComplete status based on validation
                if (request.PropertyDetailsId.HasValue)
                {
                    await UpdateVehicleCompletionStatus(request.PropertyDetailsId.Value);
                }
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
