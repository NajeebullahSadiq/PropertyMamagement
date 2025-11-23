using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Propertybuyeraudit
{
    public int Id { get; set; }

    public int BuyerId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ColumnName { get; set; }

    public virtual BuyerDetail Buyer { get; set; } = null!;
}
