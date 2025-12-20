using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Models.RequestData;

namespace WebAPIBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SellerDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public SellerDetailsController(AppDbContext context, UserManager<ApplicationUser> userManager)
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var sellers = await _context.SellerDetails.ToListAsync();

                return Ok(sellers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("Action/{id}")]
        public async Task<IActionResult> GetSellerById(int id)
        {
            try
            {
                var Pro = await _context.SellerDetails.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

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
                var Pro = await _context.BuyerDetails.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("Witness/{id}")]
        public async Task<IActionResult> GetWitnessById(int id)
        {
            try
            {
                var Pro = await _context.WitnessDetails.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("paddress/{id}")]
        public async Task<IActionResult> GetPaddressById(int id)
        {
            try
            {
                var Pro = await _context.PropertyAddresses.Where(x => x.PropertyDetailsId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }



        [HttpPost("CheckDuplicateOwner")]
        public async Task<IActionResult> CheckDuplicateOwner([FromBody] DuplicateOwnerCheckRequest request)
        {
            try
            {
                var firstName = request.FirstName?.Trim() ?? "";
                var fatherName = request.FatherName?.Trim() ?? "";
                var grandFather = request.GrandFather?.Trim() ?? "";
                var propertyDetailsId = request.PropertyDetailsId;

                // Get the property to find its transaction type
                var property = await _context.PropertyDetails.FindAsync(propertyDetailsId);
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

                // Find all active registrations with same owner identity (excluding cancelled ones)
                var duplicates = await _context.SellerDetails
                    .Where(s => 
                        s.FirstName.Trim() == firstName &&
                        s.FatherName.Trim() == fatherName &&
                        s.GrandFather.Trim() == grandFather &&
                        s.PropertyDetails != null &&
                        s.PropertyDetails.TransactionTypeId.HasValue &&
                        restrictedTransactionTypeIds.Contains(s.PropertyDetails.TransactionTypeId.Value) &&
                        !_context.PropertyCancellations.Any(c => c.PropertyDetailsId == s.PropertyDetailsId) &&
                        s.Id != request.SellerId) // Exclude current seller if editing
                    .Include(s => s.PropertyDetails)
                    .ThenInclude(p => p.TransactionType)
                    .ToListAsync();

                if (duplicates.Count > 0)
                {
                    var existingTransaction = duplicates.First();
                    var transactionTypeName = existingTransaction.PropertyDetails?.TransactionType?.Name ?? "Unknown";
                    
                    var message = $"این ملک قبلاً توسط همین مالک برای {GetDariTransactionType(transactionTypeName)} ثبت شده است. تا زمان ختم یا ابطال معامله قبلی، ثبت دوباره اجازه نیست.";
                    
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

        [HttpPost]
        public async Task<ActionResult<int>> SaveSeller([FromBody] SellerDetail request)
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

            // Validate conditional document requirements based on role type
            var roleType = request.RoleType ?? "Seller";
            var agentRoles = new[] { "Sales Agent", "Lease Agent", "Agent for a revocable sale" };
            
            // If agent role, authorization letter is required
            if (agentRoles.Contains(roleType) && string.IsNullOrWhiteSpace(request.AuthorizationLetter))
            {
                return StatusCode(400, "Authorization letter is required for agent roles");
            }
            
            // If heirs role, heirs letter is required
            if (roleType == "Heirs" && string.IsNullOrWhiteSpace(request.HeirsLetter))
            {
                return StatusCode(400, "Heirs letter is required for heirs role");
            }

            //var user = await _userManager.GetUserAsync(User);
           
            var seller = new SellerDetail
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
                NationalIdCard=request.NationalIdCard,
                RoleType=roleType,
                AuthorizationLetter=request.AuthorizationLetter,
                HeirsLetter=request.HeirsLetter,
            };
            _context.Add(seller);
            await _context.SaveChangesAsync();
            var result = new { Id = seller.Id};
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeller(int id, [FromBody] SellerDetail request)
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

            var existingProperty = await _context.SellerDetails.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Validate conditional document requirements based on role type
            var roleType = request.RoleType ?? "Seller";
            var agentRoles = new[] { "Sales Agent", "Lease Agent", "Agent for a revocable sale" };
            
            // If agent role, authorization letter is required
            if (agentRoles.Contains(roleType) && string.IsNullOrWhiteSpace(request.AuthorizationLetter))
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
                    _context.Propertyselleraudits.Add(new Propertyselleraudit
                    {
                        SellerId = existingProperty.Id,
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
                var property = await _context.PropertyDetails.FindAsync(propertyDetailsId);
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

                // Find all active registrations with same buyer identity
                var duplicates = await _context.BuyerDetails
                    .Where(b => 
                        b.FirstName.Trim() == firstName &&
                        b.FatherName.Trim() == fatherName &&
                        b.GrandFather.Trim() == grandFather &&
                        b.PropertyDetails != null &&
                        b.PropertyDetails.TransactionTypeId.HasValue &&
                        restrictedTransactionTypeIds.Contains(b.PropertyDetails.TransactionTypeId.Value) &&
                        b.Id != request.SellerId) // Exclude current buyer if editing
                    .Include(b => b.PropertyDetails)
                    .ThenInclude(p => p.TransactionType)
                    .ToListAsync();

                if (duplicates.Count > 0)
                {
                    var existingTransaction = duplicates.First();
                    var transactionTypeName = existingTransaction.PropertyDetails?.TransactionType?.Name ?? "Unknown";
                    
                    var message = $"این ملک قبلاً توسط همین خریدار برای {GetDariTransactionType(transactionTypeName)} ثبت شده است. تا زمان ختم یا ابطال معامله قبلی، ثبت دوباره اجازه نیست.";
                    
                    return Ok(new { isDuplicate = true, message = message, transactionType = transactionTypeName });
                }

                return Ok(new { isDuplicate = false, message = "" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("addBuyerDetails")]
        public async Task<ActionResult<int>> SaveBuyer([FromBody] BuyerDetail request)
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

            if (!request.PropertyTypeId.HasValue)
            {
                return StatusCode(400, "انتخاب نوعیت ملکیت الزامی است");
            }

            var propertyType = await _context.PropertyTypes.FindAsync(request.PropertyTypeId.Value);
            if (propertyType == null || string.IsNullOrWhiteSpace(propertyType.Name) || !AllowedPropertyTypeNames.Contains(propertyType.Name))
            {
                return StatusCode(400, "نوعیت ملکیت انتخاب‌شده درست نیست");
            }

            var isOtherPropertyType = propertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase);
            if (isOtherPropertyType)
            {
                if (string.IsNullOrWhiteSpace(request.CustomPropertyType))
                {
                    return StatusCode(400, "نوشتن نوع ملکیت (سایر) الزامی است");
                }
                request.CustomPropertyType = request.CustomPropertyType.Trim();
            }
            else
            {
                request.CustomPropertyType = null;
            }
            //var user = await _userManager.GetUserAsync(User);
           
            // Convert rental dates to UTC if they exist
            var rentStartDate = request.RentStartDate;
            var rentEndDate = request.RentEndDate;
            if (rentStartDate.HasValue)
            {
                rentStartDate = rentStartDate.Value.ToUniversalTime();
            }
            if (rentEndDate.HasValue)
            {
                rentEndDate = rentEndDate.Value.ToUniversalTime();
            }

            var buyer = new BuyerDetail
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
                RoleType=request.RoleType ?? "Buyer",
                AuthorizationLetter=request.AuthorizationLetter,
                PropertyTypeId = request.PropertyTypeId,
                CustomPropertyType = request.CustomPropertyType,
                Price = request.Price,
                PriceText = request.PriceText,
                RoyaltyAmount = request.RoyaltyAmount,
                HalfPrice = request.HalfPrice,
                TransactionType = request.TransactionType,
                TransactionTypeDescription = request.TransactionTypeDescription,
                RentStartDate = rentStartDate,
                RentEndDate = rentEndDate,
            };
            try
            {
                _context.Add(buyer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving buyer: {ex.Message}");
            }
            var result = new { Id = buyer.Id};
            return Ok(result);
        }

        [HttpPut("UpdateBuyer/{id}")]
        public async Task<IActionResult> UpdateBuyer(int id, [FromBody] BuyerDetail request)
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

            var existingProperty = await _context.BuyerDetails.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            if (!request.PropertyTypeId.HasValue)
            {
                return StatusCode(400, "انتخاب نوعیت ملکیت الزامی است");
            }

            var propertyType = await _context.PropertyTypes.FindAsync(request.PropertyTypeId.Value);
            if (propertyType == null || string.IsNullOrWhiteSpace(propertyType.Name) || !AllowedPropertyTypeNames.Contains(propertyType.Name))
            {
                return StatusCode(400, "نوعیت ملکیت انتخاب‌شده درست نیست");
            }

            var isOtherPropertyType = propertyType.Name.Equals("Other", StringComparison.OrdinalIgnoreCase);
            if (isOtherPropertyType)
            {
                if (string.IsNullOrWhiteSpace(request.CustomPropertyType))
                {
                    return StatusCode(400, "نوشتن نوع ملکیت (سایر) الزامی است");
                }
                request.CustomPropertyType = request.CustomPropertyType.Trim();
            }
            else
            {
                request.CustomPropertyType = null;
            }

            // Store the original values of the CreatedBy and CreatedOn properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;

            // Convert rental dates to UTC if they exist
            if (request.RentStartDate.HasValue)
            {
                request.RentStartDate = request.RentStartDate.Value.ToUniversalTime();
            }
            if (request.RentEndDate.HasValue)
            {
                request.RentEndDate = request.RentEndDate.Value.ToUniversalTime();
            }

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
                    _context.Propertybuyeraudits.Add(new Propertybuyeraudit
                    {
                        BuyerId = existingProperty.Id,
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
        public async Task<ActionResult<int>> Savewithness([FromBody] WitnessDetailRequest request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            // Check if there are already 2 PropertyDetails associated with the current WitnessDetail
            int propertyCount = _context.WitnessDetails.Count(wd => wd.PropertyDetailsId == request.PropertyDetailsId && wd.PropertyDetailsId != null);
            if (propertyCount >= 2)
            {
                return StatusCode(400, "You cannot add more than two");
            }
            if (request.PropertyDetailsId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }
            //var user = await _userManager.GetUserAsync(User);
          
            // Parse identity card number safely from string input
            double? identityCardNumber = null;
            if (!string.IsNullOrEmpty(request.IndentityCardNumber))
            {
                string idString = request.IndentityCardNumber.Trim();
                // Take first 15 digits to fit in double precision (double can handle up to ~15-17 significant digits)
                if (idString.Length > 15)
                {
                    idString = idString.Substring(0, 15);
                }
                if (double.TryParse(idString, out double parsedId))
                {
                    identityCardNumber = parsedId;
                }
                else
                {
                    return StatusCode(400, "Invalid identity card number format");
                }
            }

            var witness = new WitnessDetail
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                IndentityCardNumber = identityCardNumber,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.Now,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId,
                NationalIdCard = request.NationalIdCard,
            };
            try
            {
                _context.WitnessDetails.Add(witness);
                await _context.SaveChangesAsync();
                
                // Verify the witness was saved and has an ID
                if (witness.Id <= 0)
                {
                    return StatusCode(500, "Failed to save witness details - ID not generated");
                }
                
                var result = new { Id = witness.Id };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving witness details: {ex.Message}");
            }
        }

        [HttpPut("Updatewitness/{id}")]
        public async Task<IActionResult> UpdateWitness(int id, [FromBody] WitnessDetail request)
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
                if (!_context.WitnessDetails.Any(i => i.Id == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(int id)
        {
            try
            {
                var seller = await _context.SellerDetails.FindAsync(id);
                if (seller == null)
                {
                    return NotFound();
                }

                _context.SellerDetails.Remove(seller);
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
                var buyer = await _context.BuyerDetails.FindAsync(id);
                if (buyer == null)
                {
                    return NotFound();
                }

                _context.BuyerDetails.Remove(buyer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Buyer deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("addPaddress")]
        public async Task<ActionResult<int>> SavePaddress([FromBody] PropertyAddress request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;
            if (request == null)
            {
                return BadRequest("Request is null");
            }
            if (request.PropertyDetailsId == null)
            {
                return StatusCode(400, "MainTable is empty");
            }
            // Check if there are already 2 PropertyDetails associated with the current WitnessDetail
            int propertyCount = _context.PropertyAddresses.Count(ad => ad.PropertyDetailsId == request.PropertyDetailsId && ad.PropertyDetailsId != null);
            if (propertyCount >= 1)
            {
                return StatusCode(400, "You cannot add more than one");
            }
          
            var seller = new PropertyAddress
            {
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                Village = request.Village,
                CreatedAt = DateTime.Now,
                CreatedBy = userId.ToString(),
                PropertyDetailsId = request.PropertyDetailsId.Value,
            };
            try
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();

                // Update the IsComplete column of the PropertyDetails entity to true
                var propertyDetails = await _context.PropertyDetails.FindAsync(request.PropertyDetailsId.Value);
                if (propertyDetails == null)
                {
                    return NotFound("PropertyDetails not found");
                }
                propertyDetails.iscomplete = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception
            }
            var result = new { Id = seller.Id };
            return Ok(result);
        }

        [HttpPut("updatePaddress/{id}")]
        public async Task<IActionResult> UpdatePaddress(int id, [FromBody] PropertyAddress request)
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
                if (!_context.PropertyAddresses.Any(i => i.Id == id))
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
        [HttpGet("getProvince")]
        public async Task<IActionResult> GetAlllocation()
        {
            try
            {
                var locations = await _context.Locations.Where(x=>x.TypeId.Equals(2)).ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("getDistrict")]
        public async Task<IActionResult> GetAllDistrict()
        {
            try
            {
                var locations = await _context.Locations.Where(x => x.TypeId.Equals(3)).ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrict(int id)
        {
            try
            {
                var locations = await _context.Locations.Where(x => x.ParentId.Equals(id)).ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("getIdentityCardType")]
        public async Task<IActionResult> GetIdCardTypes()
        {
            try
            {
                var locations = await _context.IdentityCardTypes.ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("getEducationLevel")]
        public async Task<IActionResult> GetEducationLevel()
        {
            try
            {
                var locations = await _context.EducationLevels.ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("getAddressType")]
        public async Task<IActionResult> GetAddressType()
        {
            try
            {
                var locations = await _context.AddressTypes.ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("getArea")]
        public async Task<IActionResult> GetArea()
        {
            try
            {
                var locations = await _context.Areas.ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("getGuaranteeType")]
        public async Task<IActionResult> GetGuaranteeType()
        {
            try
            {
                var locations = await _context.GuaranteeTypes.ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


    }
}
