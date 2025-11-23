using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class ViolationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    public virtual ICollection<Violation> Violations { get; } = new List<Violation>();
}
