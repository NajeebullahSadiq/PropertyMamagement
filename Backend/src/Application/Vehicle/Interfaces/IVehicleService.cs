using WebAPIBackend.Application.Vehicle.DTOs;
using WebAPIBackend.Application.Common.Models;

namespace WebAPIBackend.Application.Vehicle.Interfaces
{
    /// <summary>
    /// Service interface for Vehicle operations
    /// </summary>
    public interface IVehicleService
    {
        Task<List<VehicleListItemDto>> GetAllAsync(string userId, IList<string> roles, bool canViewAll);
        Task<VehicleViewDto?> GetViewByIdAsync(int id, string userId, IList<string> roles, bool canViewAll);
        Task<Result<int>> CreateAsync(object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<VehiclePrintDto?> GetPrintDataAsync(int id, string? calendarType);
    }

    /// <summary>
    /// Service interface for Vehicle Seller operations
    /// </summary>
    public interface IVehicleSellerService
    {
        Task<List<VehicleSellerDto>> GetByVehicleIdAsync(int vehicleId);
        Task<Result<int>> CreateAsync(int vehicleId, object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<Result> DeleteAsync(int id);
    }

    /// <summary>
    /// Service interface for Vehicle Buyer operations
    /// </summary>
    public interface IVehicleBuyerService
    {
        Task<List<VehicleBuyerDto>> GetByVehicleIdAsync(int vehicleId);
        Task<Result<int>> CreateAsync(int vehicleId, object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<Result> DeleteAsync(int id);
    }
}
