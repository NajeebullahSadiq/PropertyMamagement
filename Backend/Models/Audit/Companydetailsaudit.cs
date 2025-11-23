namespace WebAPIBackend.Models.Audit
{
    public class Companydetailsaudit
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? PropertyName { get; set; }

        public virtual CompanyDetail Company { get; set; } = null!;
    }
}
