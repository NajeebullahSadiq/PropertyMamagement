using System;

namespace WebAPIBackend.Models;

public partial class PropertyCancellationDocument
{
    public int Id { get; set; }

    public int PropertyCancellationId { get; set; }

    public string FilePath { get; set; } = null!;

    public string? OriginalFileName { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PropertyCancellation? PropertyCancellation { get; set; }
}
