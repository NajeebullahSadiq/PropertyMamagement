using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;

namespace WebAPIBackend.Controllers.Companies
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyOwnerAddressController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CompanyOwnerAddressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ownerAddress/{id}")]
        public async Task<IActionResult> GetOwnerAdressById(int id)
        {
            try
            {
                var getOwnerId = (from p in _context.CompanyOwners
                                  where p.CompanyId == id
                                  select p.Id).SingleOrDefault();
                var query = from p in _context.CompanyOwnerAddresses where p.CompanyOwnerId == getOwnerId
                            select new
                            {
                                p.Id,
                                p.CompanyOwnerId,
                                p.AddressTypeId,
                                p.ProvinceId,
                                p.DistrictId,
                                p.Village,
                              
                                province = p.Province.Dari,
                                district = p.District.Dari,
                                AddressType = p.AddressType.Name,
                               
                            };
                return Ok(query);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] CompanyOwnerAddress request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

         
            var userId = userIdClaim.Value;
            int propertyCount = _context.CompanyOwnerAddresses.Count(wd => wd.CompanyOwnerId == request.CompanyOwnerId && wd.CompanyOwnerId != null &&  wd.AddressTypeId==request.AddressTypeId);
            if (propertyCount >= 1)
            {
                return StatusCode(400, "You cannot add more than one");
            }
            if (request.CompanyOwnerId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            var property = new CompanyOwnerAddress
            {
                AddressTypeId = request.AddressTypeId,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                CompanyOwnerId = request.CompanyOwnerId,
                Village=request.Village,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,

            };

            _context.Add(property);
            await _context.SaveChangesAsync();
            var result = new { Id = property.Id, companyOwnerId = property.CompanyOwnerId };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyDetails(int id, [FromBody] CompanyOwnerAddress request)
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

            var existingProperty = await _context.CompanyOwnerAddresses.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyOwnerId = existingProperty.CompanyOwnerId;

            // Update the entity with the new values
       
            existingProperty.AddressTypeId = request.AddressTypeId;
            existingProperty.ProvinceId = request.ProvinceId;
            existingProperty.DistrictId = request.DistrictId;
            existingProperty.Village = request.Village;
            // Restore the original values of the CreatedBy and CreatedAt properties
            existingProperty.CreatedBy = createdBy;
            existingProperty.CreatedAt = createdAt;

            await _context.SaveChangesAsync();

            var result = new { Id = request.Id };
            return Ok(result);
        }
    }
}
