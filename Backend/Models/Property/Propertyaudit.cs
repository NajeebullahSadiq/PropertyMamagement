using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Propertyaudit
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? PropertyName { get; set; }

    public virtual PropertyDetail Property { get; set; } = null!;
}
