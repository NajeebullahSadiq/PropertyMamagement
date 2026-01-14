namespace WebAPIBackend.Domain.Common
{
    /// <summary>
    /// Base entity with common audit fields
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Base entity with update tracking
    /// </summary>
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// Base entity with soft delete support
    /// </summary>
    public abstract class BaseSoftDeleteEntity : BaseAuditableEntity
    {
        public bool Status { get; set; } = true;
    }
}
