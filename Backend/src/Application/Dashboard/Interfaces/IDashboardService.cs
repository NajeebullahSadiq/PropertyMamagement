using WebAPIBackend.Application.Dashboard.DTOs;

namespace WebAPIBackend.Application.Dashboard.Interfaces
{
    /// <summary>
    /// Service interface for Dashboard operations
    /// </summary>
    public interface IDashboardService
    {
        Task<CompanyDashboardDto> GetCompanyDashboardAsync();
        Task<LicenseExpiredDashboardDto> GetExpiredLicenseDashboardAsync();
        Task<DashboardSummaryDto> GetVehicleDashboardAsync();
        Task<EstateDashboardDto> GetEstateDashboardAsync();
        Task<CombinedDashboardDto> GetDashboardByDateAsync(DateTime startDate, DateTime endDate);
        Task<TopUsersSummaryResultDto> GetTopUsersSummaryAsync();
        Task<TopUsersSummaryResultDto> GetVehicleTopUsersSummaryAsync();
        Task<TopUsersSummaryResultDto> GetTopUsersSummaryByDateAsync(DateTime startDate, DateTime endDate);
        Task<TopUsersSummaryResultDto> GetVehicleTopUsersSummaryByDateAsync(DateTime startDate, DateTime endDate);
        Task<List<PropertyTypeMonthlyDto>> GetPropertyTypesByMonthAsync();
        Task<List<PropertyTypeMonthlyDto>> GetTransactionTypesByMonthAsync();
        Task<List<MonthlyDataDto>> GetVehicleReportByMonthAsync();
    }
}
