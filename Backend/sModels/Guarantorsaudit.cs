using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class Guarantorsaudit
{
    public int Id { get; set; }

    public int GuarantorsId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? PropertyName { get; set; }

    public virtual Guarantor Guarantors { get; set; } = null!;
}
