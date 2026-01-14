namespace WebAPIBackend.Models.Audit
{
    public partial class Licenseaudit
    {
        public int Id { get; set; }

        public int LicenseId { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? PropertyName { get; set; }

        public virtual LicenseDetail License { get; set; } = null!;
    }
}
