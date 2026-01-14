using WebAPIBackend.Application.UserManagement.DTOs;
using WebAPIBackend.Application.Common.Models;

namespace WebAPIBackend.Application.UserManagement.Interfaces
{
    /// <summary>
    /// Service interface for User Management operations
    /// </summary>
    public interface IUserService
    {
        Task<List<UserListItemDto>> GetAllUsersAsync();
        Task<UserDto?> GetByIdAsync(string id);
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<Result<string>> CreateUserAsync(CreateUserRequest request);
        Task<Result> UpdateUserAsync(UpdateUserRequest request);
        Task<Result> DeleteUserAsync(string id);
        Task<Result> LockUserAsync(string id, bool lockout);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }

    /// <summary>
    /// Service interface for Authentication operations
    /// </summary>
    public interface IAuthService
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result> LogoutAsync(string userId);
        Task<bool> ValidateTokenAsync(string token);
    }
}
