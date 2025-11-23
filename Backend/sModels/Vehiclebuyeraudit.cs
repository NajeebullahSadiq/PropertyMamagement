using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class Vehiclebuyeraudit
{
    public int Id { get; set; }

    public int VehicleBuyerId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ColumnName { get; set; }

    public virtual VehiclesBuyerDetail VehicleBuyer { get; set; } = null!;
}
