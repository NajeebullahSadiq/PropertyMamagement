using WebAPIBackend.Application.Reports.DTOs;

namespace WebAPIBackend.Application.Reports.Interfaces
{
    /// <summary>
    /// Generic report service interface for dynamic reporting
    /// </summary>
    public interface IReportService
    {
        Task<ReportResult<T>> GenerateReportAsync<T>(ReportRequest request) where T : class;
        Task<byte[]> ExportToExcelAsync<T>(ReportRequest request) where T : class;
        Task<byte[]> ExportToPdfAsync<T>(ReportRequest request) where T : class;
    }

    /// <summary>
    /// Securities-specific report service
    /// </summary>
    public interface ISecuritiesReportService
    {
        Task<SecuritiesReportResult> GetSecuritiesReportAsync(SecuritiesReportRequest request);
        Task<PetitionWriterReportResult> GetPetitionWriterReportAsync(PetitionWriterReportRequest request);
    }
}
