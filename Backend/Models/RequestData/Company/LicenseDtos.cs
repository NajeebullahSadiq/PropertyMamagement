using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData.Company
{
    /// <summary>
    /// DTO for creating a new license
    /// </summary>
    public class LicenseCreateDto
    {
        [Required]
        public int CompanyId { get; set; }

        public string? LicenseNumber { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpireDate { get; set; }
        public string? TransferLocation { get; set; }
        public string? OfficeAddress { get; set; }
        public string? DocPath { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseCategory { get; set; }
        public int? RenewalRound { get; set; }
        public decimal? RoyaltyAmount { get; set; }
        public DateOnly? RoyaltyDate { get; set; }
        public string? TariffNumber { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public DateOnly? PenaltyDate { get; set; }
        public string? HrLetter { get; set; }
        public DateOnly? HrLetterDate { get; set; }

        // ProvinceId intentionally excluded - inherited from company
    }

    /// <summary>
    /// DTO for updating an existing license
    /// </summary>
    public class LicenseUpdateDto
    {
        public string? LicenseNumber { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpireDate { get; set; }
        public string? TransferLocation { get; set; }
        public string? OfficeAddress { get; set; }
        public string? DocPath { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseCategory { get; set; }
        public int? RenewalRound { get; set; }
        public decimal? RoyaltyAmount { get; set; }
        public DateOnly? RoyaltyDate { get; set; }
        public string? TariffNumber { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public DateOnly? PenaltyDate { get; set; }
        public string? HrLetter { get; set; }
        public DateOnly? HrLetterDate { get; set; }
        public bool? Status { get; set; }

        // ProvinceId intentionally excluded - cannot be modified
    }
}
