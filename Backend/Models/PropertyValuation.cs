using System;

namespace WebAPIBackend.Models;

public partial class PropertyValuation
{
    public int Id { get; set; }

    public int PropertyDetailsId { get; set; }

    public DateTime ValuationDate { get; set; }

    public double ValuationAmount { get; set; }

    public string? ValuatorName { get; set; }

    public string? ValuatorOrganization { get; set; }

    public string? ValuationDocumentPath { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }
}
