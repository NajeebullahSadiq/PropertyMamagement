using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Services;

namespace WebAPIBackend.Controllers.Lookup
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictManagementController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILookupCacheService _cache;

        public DistrictManagementController(AppDbContext context, ILookupCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Get all districts for a specific province (including inactive ones)
        /// </summary>
        [HttpGet("province/{provinceId}")]
        public async Task<IActionResult> GetDistrictsByProvince(int provinceId)
        {
            try
            {
                var districts = await _context.Locations
                    .AsNoTracking()
                    .Where(x => x.ParentId == provinceId && x.TypeId == 3)
                    .OrderByDescending(x => x.IsActive)
                    .ThenBy(x => x.Dari)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Dari,
                        x.ParentId,
                        x.IsActive,
                        x.PathDari
                    })
                    .ToListAsync();

                return Ok(districts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری ولسوالی ها", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all provinces
        /// </summary>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provinces = await _cache.GetProvincesAsync();
                var result = provinces.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Dari
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری ولایات", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a single district by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrict(int id)
        {
            try
            {
                var district = await _context.Locations
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.TypeId == 3)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Dari,
                        x.ParentId,
                        x.IsActive,
                        x.PathDari
                    })
                    .FirstOrDefaultAsync();

                if (district == null)
                {
                    return NotFound(new { message = "ولسوالی یافت نشد" });
                }

                return Ok(district);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری ولسوالی", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new district
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDistrict([FromBody] DistrictCreateRequest request)
        {
            try
            {
                // Validate province exists
                var province = await _context.Locations
                    .FirstOrDefaultAsync(x => x.Id == request.ProvinceId && x.TypeId == 2);

                if (province == null)
                {
                    return BadRequest(new { message = "ولایت یافت نشد" });
                }

                // Check if district with same name already exists for this province
                var existingDistrict = await _context.Locations
                    .AsNoTracking().AnyAsync(x => x.ParentId == request.ProvinceId && 
                                   x.TypeId == 3 && 
                                   x.Dari == request.Dari);

                if (existingDistrict)
                {
                    return BadRequest(new { message = "ولسوالی با این نام قبلاً ثبت شده است" });
                }

                var district = new Location
                {
                    Dari = request.Dari,
                    Name = request.Name ?? request.Dari,
                    ParentId = request.ProvinceId,
                    TypeId = 3,
                    IsActive = 1,
                    PathDari = $"{province.Dari}/{request.Dari}"
                };

                _context.Locations.Add(district);
                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.ProvincesKey);
                _cache.InvalidateCache(LookupCacheService.AllDistrictsKey);
                _cache.InvalidateCache($"{LookupCacheService.DistrictsByProvinceKeyPrefix}{request.ProvinceId}");

                return Ok(new
                {
                    message = "ولسوالی با موفقیت ثبت شد",
                    district = new
                    {
                        district.Id,
                        district.Name,
                        district.Dari,
                        district.ParentId,
                        district.IsActive,
                        district.PathDari
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت ولسوالی", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing district
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistrict(int id, [FromBody] DistrictUpdateRequest request)
        {
            try
            {
                var district = await _context.Locations
                    .FirstOrDefaultAsync(x => x.Id == id && x.TypeId == 3);

                if (district == null)
                {
                    return NotFound(new { message = "ولسوالی یافت نشد" });
                }

                // Get province for path update
                var province = await _context.Locations
                    .FirstOrDefaultAsync(x => x.Id == district.ParentId && x.TypeId == 2);

                if (province == null)
                {
                    return BadRequest(new { message = "ولایت یافت نشد" });
                }

                // Check if another district with same name exists for this province
                var duplicateDistrict = await _context.Locations
                    .AsNoTracking().AnyAsync(x => x.ParentId == district.ParentId && 
                                   x.TypeId == 3 && 
                                   x.Dari == request.Dari &&
                                   x.Id != id);

                if (duplicateDistrict)
                {
                    return BadRequest(new { message = "ولسوالی با این نام قبلاً ثبت شده است" });
                }

                district.Dari = request.Dari;
                district.Name = request.Name ?? request.Dari;
                district.PathDari = $"{province.Dari}/{request.Dari}";

                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.AllDistrictsKey);
                _cache.InvalidateCache($"{LookupCacheService.DistrictsByProvinceKeyPrefix}{district.ParentId}");

                return Ok(new
                {
                    message = "ولسوالی با موفقیت ویرایش شد",
                    district = new
                    {
                        district.Id,
                        district.Name,
                        district.Dari,
                        district.ParentId,
                        district.IsActive,
                        district.PathDari
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ویرایش ولسوالی", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a district (soft delete by setting IsActive = 0)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            try
            {
                var district = await _context.Locations
                    .FirstOrDefaultAsync(x => x.Id == id && x.TypeId == 3);

                if (district == null)
                {
                    return NotFound(new { message = "ولسوالی یافت نشد" });
                }

                // Check if district is being used in any records
                var isUsed = await IsDistrictInUse(id);
                if (isUsed)
                {
                    return BadRequest(new { message = "این ولسوالی در سیستم استفاده شده و نمی توان آن را حذف کرد" });
                }

                // Soft delete
                district.IsActive = 0;
                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.AllDistrictsKey);
                _cache.InvalidateCache($"{LookupCacheService.DistrictsByProvinceKeyPrefix}{district.ParentId}");

                return Ok(new { message = "ولسوالی با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف ولسوالی", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate a district (set IsActive = 1)
        /// </summary>
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateDistrict(int id)
        {
            try
            {
                var district = await _context.Locations
                    .FirstOrDefaultAsync(x => x.Id == id && x.TypeId == 3);

                if (district == null)
                {
                    return NotFound(new { message = "ولسوالی یافت نشد" });
                }

                // Activate the district
                district.IsActive = 1;
                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.AllDistrictsKey);
                _cache.InvalidateCache($"{LookupCacheService.DistrictsByProvinceKeyPrefix}{district.ParentId}");

                return Ok(new { message = "ولسوالی با موفقیت فعال شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در فعال سازی ولسوالی", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if a district is being used in any records
        /// </summary>
        private async Task<bool> IsDistrictInUse(int districtId)
        {
            // Check in various tables that reference districts
            var usedInCompanyOwner = await _context.CompanyOwners
                .AsNoTracking().AnyAsync(x => x.OwnerDistrictId == districtId || x.PermanentDistrictId == districtId);

            var usedInGuarantor = await _context.Guarantors
                .AsNoTracking().AnyAsync(x => x.PaddressDistrictId == districtId || x.TaddressDistrictId == districtId);

            var usedInSeller = await _context.SellerDetails
                .AsNoTracking().AnyAsync(x => x.PaddressDistrictId == districtId || x.TaddressDistrictId == districtId);

            var usedInBuyer = await _context.BuyerDetails
                .AsNoTracking().AnyAsync(x => x.PaddressDistrictId == districtId || x.TaddressDistrictId == districtId);

            return usedInCompanyOwner || usedInGuarantor || usedInSeller || usedInBuyer;
        }
    }

    public class DistrictCreateRequest
    {
        public string Dari { get; set; } = null!;
        public string? Name { get; set; }
        public int ProvinceId { get; set; }
    }

    public class DistrictUpdateRequest
    {
        public string Dari { get; set; } = null!;
        public string? Name { get; set; }
    }
}
