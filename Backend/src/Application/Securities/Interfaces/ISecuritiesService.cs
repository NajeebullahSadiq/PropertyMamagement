using WebAPIBackend.Application.Securities.DTOs;
using WebAPIBackend.Shared.Extensions;

namespace WebAPIBackend.Application.Securities.Interfaces
{
    /// <summary>
    /// Service interface for Securities Distribution operations
    /// </summary>
    public interface ISecuritiesDistributionService
    {
        Task<PaginatedResult<SecuritiesDistributionListItemDto>> GetAllAsync(int page, int pageSize, string? search, string? calendarType);
        Task<SecuritiesDistributionDto?> GetByIdAsync(int id, string? calendarType);
        Task<int> CreateAsync(CreateSecuritiesDistributionRequest request, string userName);
        Task<bool> UpdateAsync(int id, CreateSecuritiesDistributionRequest request, string userName);
        Task<bool> DeleteAsync(int id, string userName);
        Task<bool> ExistsRegistrationNumberAsync(string registrationNumber, int? excludeId = null);
    }

    /// <summary>
    /// Service interface for Petition Writer Securities operations
    /// </summary>
    public interface IPetitionWriterSecuritiesService
    {
        Task<PaginatedResult<PetitionWriterSecuritiesDto>> GetAllAsync(int page, int pageSize, string? search, string? calendarType);
        Task<PetitionWriterSecuritiesDto?> GetByIdAsync(int id, string? calendarType);
        Task<int> CreateAsync(PetitionWriterSecuritiesDto request, string userName);
        Task<bool> UpdateAsync(int id, PetitionWriterSecuritiesDto request, string userName);
        Task<bool> DeleteAsync(int id, string userName);
    }
}
