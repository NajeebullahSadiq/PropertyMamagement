using WebAPIBackend.Application.Company.DTOs;
using WebAPIBackend.Application.Common.Models;

namespace WebAPIBackend.Application.Company.Interfaces
{
    /// <summary>
    /// Service interface for Company operations
    /// </summary>
    public interface ICompanyService
    {
        Task<List<CompanyListItemDto>> GetAllAsync();
        Task<List<CompanyListItemDto>> GetExpiredLicensesAsync();
        Task<CompanyDetailDto?> GetByIdAsync(int id, string? calendarType);
        Task<CompanyViewDto?> GetViewByIdAsync(int id);
        Task<Result<int>> CreateAsync(CreateCompanyRequest request, string userId);
        Task<Result> UpdateAsync(int id, CreateCompanyRequest request, string userId);
    }

    /// <summary>
    /// Service interface for Company Owner operations
    /// </summary>
    public interface ICompanyOwnerService
    {
        Task<object?> GetByCompanyIdAsync(int companyId);
        Task<Result<int>> CreateOrUpdateAsync(int companyId, object request, string userId);
    }

    /// <summary>
    /// Service interface for License Detail operations
    /// </summary>
    public interface ILicenseDetailService
    {
        Task<object?> GetByCompanyIdAsync(int companyId);
        Task<Result<int>> CreateOrUpdateAsync(int companyId, object request, string userId);
    }

    /// <summary>
    /// Service interface for Guarantor operations
    /// </summary>
    public interface IGuarantorService
    {
        Task<List<GuarantorViewDto>> GetByCompanyIdAsync(int companyId);
        Task<Result<int>> CreateAsync(int companyId, object request, string userId);
        Task<Result> UpdateAsync(int id, object request, string userId);
        Task<Result> DeleteAsync(int id);
    }
}
