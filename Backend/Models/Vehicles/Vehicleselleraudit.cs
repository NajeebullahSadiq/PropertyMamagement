using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Vehicleselleraudit
{
    public int Id { get; set; }

    public int VehicleSellerId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ColumnName { get; set; }

    public virtual VehiclesSellerDetail VehicleSeller { get; set; } = null!;
}
