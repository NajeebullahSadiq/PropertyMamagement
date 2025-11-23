using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Controllers.Companies
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranatorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public GuaranatorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getGuaranatorById(int id)
        {
            try
            {
                var Pro = await _context.Guarantors.Where(x => x.CompanyId.Equals(id)).ToListAsync();

                return Ok(Pro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> SaveProperty([FromBody] Guarantor request)
        {
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }


            var userId = userIdClaim.Value;
            int propertyCount = _context.Guarantors.Count(wd => wd.CompanyId == request.CompanyId && wd.CompanyId != null);
            if (propertyCount >= 2)
            {
                return StatusCode(400, "You cannot add more than Two");
            }
            if (request.CompanyId.Equals(0))
            {
                return StatusCode(312, "Main Table is Empty");
            }

            var property = new Guarantor
            {
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                IdentityCardTypeId = request.IdentityCardTypeId,
                CompanyId = request.CompanyId,
                IndentityCardNumber = request.IndentityCardNumber,
                Jild = request.Jild,
                Safha = request.Safha,
                SabtNumber = request.SabtNumber,
                PhoneNumber = request.PhoneNumber,
                PothoPath = request.PothoPath,
                PaddressProvinceId = request.PaddressProvinceId,
                PaddressDistrictId = request.PaddressDistrictId,
                PaddressVillage = request.PaddressVillage,
                TaddressProvinceId = request.TaddressProvinceId,
                TaddressDistrictId = request.TaddressDistrictId,
                TaddressVillage = request.TaddressVillage,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,

            };

            _context.Add(property);
            await _context.SaveChangesAsync();
            var result = new { Id = property.Id };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] Guarantor request)
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

            var existingProperty = await _context.Guarantors.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Store the original values of the CreatedBy and CreatedAt properties
            var createdBy = existingProperty.CreatedBy;
            var createdAt = existingProperty.CreatedAt;
            var companyId = existingProperty.CompanyId;
            // Update the entity with the new values
            existingProperty.FirstName = request.FirstName;
            existingProperty.FatherName = request.FatherName;
            existingProperty.IdentityCardTypeId = request.IdentityCardTypeId;
            existingProperty.IndentityCardNumber = request.IndentityCardNumber;
            existingProperty.Jild = request.Jild;
            existingProperty.Safha = request.Safha;
            existingProperty.SabtNumber = request.SabtNumber;
            existingProperty.CompanyId = companyId;
            existingProperty.PothoPath = request.PothoPath;
            existingProperty.PhoneNumber = request.PhoneNumber;
            existingProperty.PaddressProvinceId = request.PaddressProvinceId;
            existingProperty.PaddressDistrictId = request.PaddressDistrictId;
            existingProperty.PaddressVillage = request.PaddressVillage;
            existingProperty.TaddressProvinceId = request.TaddressProvinceId;
            existingProperty.TaddressDistrictId = request.TaddressDistrictId;
            existingProperty.TaddressVillage = request.TaddressVillage;

            // Restore the original values of the CreatedBy and CreatedAt properties
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

            foreach (var change in changes)
            {
                // Only add an entry to the vehicleaudit table if the property has been modified
                if (change.Value.OldValue != null && !change.Value.OldValue.Equals(change.Value.NewValue))
                {
                    _context.Guarantorsaudits.Add(new Guarantorsaudit
                    {
                        GuarantorsId = existingProperty.Id,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.Now,
                        PropertyName = change.Key,
                        OldValue = change.Value.OldValue?.ToString(),
                        NewValue = change.Value.NewValue?.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();

            var result = new { Id = request.Id };
            return Ok(result);
        }
    }
}
