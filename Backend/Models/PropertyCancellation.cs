using System;

namespace WebAPIBackend.Models;

public partial class PropertyCancellation
{
    public int Id { get; set; }

    public int PropertyDetailsId { get; set; }

    public DateTime CancellationDate { get; set; }

    public string? CancellationReason { get; set; }

    public string? CancelledBy { get; set; }

    public string Status { get; set; } = "Cancelled";

    public DateTime CreatedAt { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }
}
