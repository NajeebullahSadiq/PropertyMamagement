namespace WebAPIBackend.Application.UserManagement.DTOs
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserRole { get; set; }
        public string? LicenseType { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UserListItemDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserRole { get; set; }
        public string? UserRoleDari { get; set; }
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserRole { get; set; }
        public string? LicenseType { get; set; }
        public int? CompanyId { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserRole { get; set; }
        public string? LicenseType { get; set; }
        public int? CompanyId { get; set; }
    }

    public class LoginRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? LicenseType { get; set; }
        public int? CompanyId { get; set; }
        public string[] Permissions { get; set; } = Array.Empty<string>();
    }

    public class ChangePasswordRequest
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string? UserId { get; set; }
        public string? NewPassword { get; set; }
    }

    public class UserProfileDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserRole { get; set; }
        public string? UserRoleDari { get; set; }
        public string? LicenseType { get; set; }
        public string? CompanyName { get; set; }
        public string[] Permissions { get; set; } = Array.Empty<string>();
    }
}
