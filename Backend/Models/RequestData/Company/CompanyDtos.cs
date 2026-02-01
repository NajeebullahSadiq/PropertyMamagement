using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData.Company
{
    /// <summary>
    /// DTO for creating a new company
    /// </summary>
    public class CompanyCreateDto
    {
        [Required]
        public string Title { get; set; } = null!;

        public string? DocPath { get; set; }
        public string? Tin { get; set; }

        /// <summary>
        /// Province ID (optional for administrators, auto-populated for COMPANY_REGISTRAR)
        /// </summary>
        public int? ProvinceId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing company
    /// </summary>
    public class CompanyUpdateDto
    {
        [Required]
        public string Title { get; set; } = null!;

        public string? DocPath { get; set; }
        public string? Tin { get; set; }
        public bool? Status { get; set; }

        // ProvinceId intentionally excluded - cannot be modified
    }
}
