using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Propertyselleraudit
{
    public int Id { get; set; }

    public int SellerId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
    public string? ColumnName { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SellerDetail Seller { get; set; } = null!;
}
