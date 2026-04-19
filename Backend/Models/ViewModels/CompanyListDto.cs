using System;

namespace WebAPIBackend.Models.ViewModels
{
    public class CompanyListDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerFatherName { get; set; }
        public string? OwnerElectronicNationalIdNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public DateOnly? LicenseIssueDate { get; set; }
        public DateOnly? LicenseExpiryDate { get; set; }
        public string? Granator { get; set; }
        public bool? IsComplete { get; set; }
    }
}
