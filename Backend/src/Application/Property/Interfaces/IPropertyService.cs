using WebAPIBackend.Application.Property.DTOs;
using WebAPIBackend.Application.Common.Models;

namespace WebAPIBackend.Application.Property.Interfaces
{
    /// <summary>
    /// Service interface for Property operations
    /// </summary>
    public interface IPropertyService
    {
        Task<List<PropertyListItemDto>> GetAllAsync(string userId, IList<string> roles, bool canViewAll);
        Task<PropertyViewDto?> GetViewByIdAsync(int id, string userId, IList<string> roles, bool canViewAll);
        Task<Result<int>> CreateAsync(object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<PropertyPrintDto?> GetPrintDataAsync(int id, string? calendarType);
    }

    /// <summary>
    /// Service interface for Seller operations
    /// </summary>
    public interface ISellerService
    {
        Task<List<SellerDetailDto>> GetByPropertyIdAsync(int propertyId);
        Task<Result<int>> CreateAsync(int propertyId, object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<Result> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface for Buyer operations
    /// </summary>
    public interface IBuyerService
    {
        Task<List<BuyerDetailDto>> GetByPropertyIdAsync(int propertyId);
        Task<Result<int>> CreateAsync(int propertyId, object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<Result> DeleteAsync(int id);
    }
}
