using System;

namespace WebAPIBackend.Models;

public partial class PropertyOwnershipHistory
{
    public int Id { get; set; }

    public int PropertyDetailsId { get; set; }

    public string OwnerName { get; set; } = null!;

    public string? OwnerFatherName { get; set; }

    public DateTime? OwnershipStartDate { get; set; }

    public DateTime? OwnershipEndDate { get; set; }

    public string? TransferDocumentPath { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }
}
