using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Area
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    public virtual ICollection<LicenseDetail> LicenseDetails { get; } = new List<LicenseDetail>();
}
