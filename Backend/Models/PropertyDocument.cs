using System;

namespace WebAPIBackend.Models;

public partial class PropertyDocument
{
    public int Id { get; set; }

    public int PropertyDetailsId { get; set; }

    public string DocumentCategory { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string? OriginalFileName { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }
}
