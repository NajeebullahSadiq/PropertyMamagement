using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class GuaranteeType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    public virtual ICollection<Gaurantee> Gaurantees { get; } = new List<Gaurantee>();
}
