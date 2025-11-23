using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class TransactionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    public virtual ICollection<PropertyDetail> PropertyDetails { get; } = new List<PropertyDetail>();

    public virtual ICollection<VehiclesPropertyDetail> VehiclesPropertyDetails { get; } = new List<VehiclesPropertyDetail>();
}
