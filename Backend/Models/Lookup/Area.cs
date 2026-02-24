using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Area
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    // Removed LicenseDetails navigation property - no longer used after migration to TransferLocation
}
