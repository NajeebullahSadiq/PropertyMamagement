namespace WebAPIBackend.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for authorization operations
    /// </summary>
    public interface IAppAuthorizationService
    {
        Task<bool> HasPermissionAsync(string permission);
        Task<bool> CanAccessModuleAsync(string module);
        Task<bool> CanEditRecordAsync(string module, string recordCreatedBy);
        Task<bool> CanCreateRecordAsync(string module);
        Task<bool> CanDeleteRecordAsync(string module);
        Task<bool> CanViewAllRecordsAsync(string module);
        bool IsViewOnlyRole(string role);
        string[] GetUserPermissions(string role);
    }
}
