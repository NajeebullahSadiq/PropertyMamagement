using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class LostdocumentsType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }
}
