using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    /// <summary>
    /// DTO for creating a new user
    /// </summary>
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int CompanyId { get; set; }
        public string? LicenseType { get; set; }

        /// <summary>
        /// Province assignment (required for COMPANY_REGISTRAR role)
        /// </summary>
        public int? ProvinceId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing user
    /// </summary>
    public class UserUpdateDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? CompanyId { get; set; }
        public string? LicenseType { get; set; }

        /// <summary>
        /// Province assignment (required for COMPANY_REGISTRAR role)
        /// </summary>
        public int? ProvinceId { get; set; }
    }
}
