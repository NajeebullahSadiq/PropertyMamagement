using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models;
using WebAPIBackend.Models.PetitionWriterLicense;

namespace WebAPIBackend.Services;

/// <summary>
/// Centralized cache service for lookup/static data that rarely changes.
/// Reduces database hits for reference tables like Locations, PropertyTypes, etc.
/// </summary>
public interface ILookupCacheService
{
    Task<List<Location>> GetProvincesAsync();
    Task<List<Location>> GetDistrictsAsync(int provinceId);
    Task<List<Location>> GetAllDistrictsAsync();
    Task<List<PropertyType>> GetPropertyTypesAsync();
    Task<List<TransactionType>> GetTransactionTypesAsync();
    Task<List<PunitType>> GetPunitTypesAsync();
    Task<List<EducationLevel>> GetEducationLevelsAsync();
    Task<List<AddressType>> GetAddressTypesAsync();
    Task<List<Area>> GetAreasAsync();
    Task<List<GuaranteeType>> GetGuaranteeTypesAsync();
    Task<List<PetitionWriterActivityLocationEntity>> GetActiveActivityLocationsAsync();
    void InvalidateCache(string key);
    void InvalidateAllCache();
}

public class LookupCacheService : ILookupCacheService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    // Cache duration for lookup data - these tables rarely change
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(6);
    private static readonly TimeSpan ShortCacheDuration = TimeSpan.FromMinutes(30);

    // Cache key constants
    public static readonly string ProvincesKey = "lookup_provinces";
    public static readonly string AllDistrictsKey = "lookup_all_districts";
    public static readonly string DistrictsByProvinceKeyPrefix = "lookup_districts_province_";
    public static readonly string PropertyTypesKey = "lookup_property_types";
    public static readonly string TransactionTypesKey = "lookup_transaction_types";
    public static readonly string PunitTypesKey = "lookup_punit_types";
    public static readonly string EducationLevelsKey = "lookup_education_levels";
    public static readonly string AddressTypesKey = "lookup_address_types";
    public static readonly string AreasKey = "lookup_areas";
    public static readonly string GuaranteeTypesKey = "lookup_guarantee_types";
    public static readonly string ActivityLocationsKey = "lookup_activity_locations";

    public LookupCacheService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<Location>> GetProvincesAsync()
    {
        if (!_cache.TryGetValue(ProvincesKey, out List<Location>? provinces))
        {
            provinces = await _context.Locations
                .AsNoTracking()
                .Where(x => x.TypeId == 2 && x.IsActive == 1)
                .OrderBy(x => x.Dari)
                .ToListAsync();

            _cache.Set(ProvincesKey, provinces, CacheDuration);
        }
        return provinces!;
    }

    public async Task<List<Location>> GetDistrictsAsync(int provinceId)
    {
        var cacheKey = $"{DistrictsByProvinceKeyPrefix}{provinceId}";
        if (!_cache.TryGetValue(cacheKey, out List<Location>? districts))
        {
            districts = await _context.Locations
                .AsNoTracking()
                .Where(x => x.ParentId == provinceId && x.TypeId == 3 && x.IsActive == 1)
                .OrderBy(x => x.Dari)
                .ToListAsync();

            _cache.Set(cacheKey, districts, CacheDuration);
        }
        return districts!;
    }

    public async Task<List<Location>> GetAllDistrictsAsync()
    {
        if (!_cache.TryGetValue(AllDistrictsKey, out List<Location>? districts))
        {
            districts = await _context.Locations
                .AsNoTracking()
                .Where(x => x.TypeId == 3 && x.IsActive == 1)
                .OrderBy(x => x.Dari)
                .ToListAsync();

            _cache.Set(AllDistrictsKey, districts, CacheDuration);
        }
        return districts!;
    }

    public async Task<List<PropertyType>> GetPropertyTypesAsync()
    {
        if (!_cache.TryGetValue(PropertyTypesKey, out List<PropertyType>? types))
        {
            types = await _context.PropertyTypes
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(PropertyTypesKey, types, CacheDuration);
        }
        return types!;
    }

    public async Task<List<TransactionType>> GetTransactionTypesAsync()
    {
        if (!_cache.TryGetValue(TransactionTypesKey, out List<TransactionType>? types))
        {
            types = await _context.TransactionTypes
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(TransactionTypesKey, types, CacheDuration);
        }
        return types!;
    }

    public async Task<List<PunitType>> GetPunitTypesAsync()
    {
        if (!_cache.TryGetValue(PunitTypesKey, out List<PunitType>? types))
        {
            types = await _context.PunitTypes
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(PunitTypesKey, types, CacheDuration);
        }
        return types!;
    }

    public async Task<List<EducationLevel>> GetEducationLevelsAsync()
    {
        if (!_cache.TryGetValue(EducationLevelsKey, out List<EducationLevel>? levels))
        {
            levels = await _context.EducationLevels
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(EducationLevelsKey, levels, CacheDuration);
        }
        return levels!;
    }

    public async Task<List<AddressType>> GetAddressTypesAsync()
    {
        if (!_cache.TryGetValue(AddressTypesKey, out List<AddressType>? types))
        {
            types = await _context.AddressTypes
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(AddressTypesKey, types, CacheDuration);
        }
        return types!;
    }

    public async Task<List<Area>> GetAreasAsync()
    {
        if (!_cache.TryGetValue(AreasKey, out List<Area>? areas))
        {
            areas = await _context.Areas
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(AreasKey, areas, CacheDuration);
        }
        return areas!;
    }

    public async Task<List<GuaranteeType>> GetGuaranteeTypesAsync()
    {
        if (!_cache.TryGetValue(GuaranteeTypesKey, out List<GuaranteeType>? types))
        {
            types = await _context.GuaranteeTypes
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(GuaranteeTypesKey, types, CacheDuration);
        }
        return types!;
    }

    public async Task<List<PetitionWriterActivityLocationEntity>> GetActiveActivityLocationsAsync()
    {
        if (!_cache.TryGetValue(ActivityLocationsKey, out List<PetitionWriterActivityLocationEntity>? locations))
        {
            locations = await _context.PetitionWriterActivityLocations
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DariName)
                .ToListAsync();

            _cache.Set(ActivityLocationsKey, locations, ShortCacheDuration);
        }
        return locations!;
    }

    public void InvalidateCache(string key)
    {
        _cache.Remove(key);
    }

    public void InvalidateAllCache()
    {
        InvalidateCache(ProvincesKey);
        InvalidateCache(AllDistrictsKey);
        InvalidateCache(PropertyTypesKey);
        InvalidateCache(TransactionTypesKey);
        InvalidateCache(PunitTypesKey);
        InvalidateCache(EducationLevelsKey);
        InvalidateCache(AddressTypesKey);
        InvalidateCache(AreasKey);
        InvalidateCache(GuaranteeTypesKey);
        InvalidateCache(ActivityLocationsKey);

        // Invalidate all district-by-province entries
        // Since we can't enumerate cache keys, we rely on expiration
    }
}
