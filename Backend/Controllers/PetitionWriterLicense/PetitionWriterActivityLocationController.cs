using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models.PetitionWriterLicense;
using WebAPIBackend.Services;

namespace WebAPIBackend.Controllers.PetitionWriterLicense
{
    /// <summary>
    /// Controller for managing Petition Writer Activity Locations (محل فعالیت عریضه‌نویس)
    /// Admin can add/edit/delete activity locations that appear as dropdown options
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PetitionWriterActivityLocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILookupCacheService _cache;

        public PetitionWriterActivityLocationController(AppDbContext context, ILookupCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Get all active activity locations for dropdown
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cachedItems = await _cache.GetActiveActivityLocationsAsync();
                var items = cachedItems.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.DariName,
                    x.IsActive
                }).ToList();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری محل فعالیت", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all activity locations (including inactive) for admin management
        /// </summary>
        [HttpGet("manage")]
        [Authorize(Roles = "ADMIN,AUTHORITY")]
        public async Task<IActionResult> GetAllForManagement()
        {
            try
            {
                var items = await _context.PetitionWriterActivityLocations
                    .AsNoTracking()
                    .OrderByDescending(x => x.IsActive)
                    .ThenBy(x => x.DariName)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.DariName,
                        x.IsActive,
                        x.CreatedAt,
                        x.CreatedBy,
                        x.UpdatedAt,
                        x.UpdatedBy
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در بارگذاری محل فعالیت", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new activity location
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,AUTHORITY")]
        public async Task<IActionResult> Create([FromBody] ActivityLocationCreateRequest request)
        {
            try
            {
                var existing = await _context.PetitionWriterActivityLocations
                    .AsNoTracking().AnyAsync(x => x.DariName == request.DariName);

                if (existing)
                {
                    return BadRequest(new { message = "محل فعالیت با این نام قبلاً ثبت شده است" });
                }

                var username = User.Identity?.Name ?? "system";
                var entity = new PetitionWriterActivityLocationEntity
                {
                    Name = request.Name ?? request.DariName,
                    DariName = request.DariName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = username
                };

                _context.PetitionWriterActivityLocations.Add(entity);
                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.ActivityLocationsKey);

                return Ok(new
                {
                    id = entity.Id,
                    message = "محل فعالیت با موفقیت ثبت شد"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در ثبت محل فعالیت", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing activity location
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,AUTHORITY")]
        public async Task<IActionResult> Update(int id, [FromBody] ActivityLocationUpdateRequest request)
        {
            try
            {
                var entity = await _context.PetitionWriterActivityLocations
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return NotFound(new { message = "محل فعالیت یافت نشد" });
                }

                var duplicate = await _context.PetitionWriterActivityLocations
                    .AsNoTracking().AnyAsync(x => x.DariName == request.DariName && x.Id != id);

                if (duplicate)
                {
                    return BadRequest(new { message = "محل فعالیت با این نام قبلاً ثبت شده است" });
                }

                var username = User.Identity?.Name ?? "system";
                entity.Name = request.Name ?? request.DariName;
                entity.DariName = request.DariName;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = username;

                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.ActivityLocationsKey);

                return Ok(new { id = entity.Id, message = "محل فعالیت با موفقیت تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در تغییر محل فعالیت", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete (deactivate) an activity location
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,AUTHORITY")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.PetitionWriterActivityLocations
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return NotFound(new { message = "محل فعالیت یافت نشد" });
                }

                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = User.Identity?.Name ?? "system";

                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.ActivityLocationsKey);

                return Ok(new { message = "محل فعالیت با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در حذف محل فعالیت", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate an activity location
        /// </summary>
        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "ADMIN,AUTHORITY")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var entity = await _context.PetitionWriterActivityLocations
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return NotFound(new { message = "محل فعالیت یافت نشد" });
                }

                entity.IsActive = true;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = User.Identity?.Name ?? "system";

                await _context.SaveChangesAsync();

                _cache.InvalidateCache(LookupCacheService.ActivityLocationsKey);

                return Ok(new { message = "محل فعالیت با موفقیت فعال شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطا در فعال سازی محل فعالیت", error = ex.Message });
            }
        }
    }

    public class ActivityLocationCreateRequest
    {
        public string DariName { get; set; } = null!;
        public string? Name { get; set; }
    }

    public class ActivityLocationUpdateRequest
    {
        public string DariName { get; set; } = null!;
        public string? Name { get; set; }
    }
}
