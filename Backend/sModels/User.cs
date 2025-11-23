using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? ImgPath { get; set; }
}
