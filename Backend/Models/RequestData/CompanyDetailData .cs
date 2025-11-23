namespace WebAPIBackend.Models.RequestData
{
    public class CompanyDetailData
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? LicenseNumber { get; set; }

        public string? PetitionDate { get; set; }

        public string? PetitionNumber { get; set; }

        public bool? Status { get; set; }

        public string? DocPath { get; set; }

        public double? Tin { get; set; }
    }
}
