using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class Graunteeaudit
{
    public int Id { get; set; }

    public int GauranteeId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? PropertyName { get; set; }

    public virtual Gaurantee Gaurantee { get; set; } = null!;
}
