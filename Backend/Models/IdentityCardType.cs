using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class IdentityCardType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    public virtual ICollection<CompanyOwner> CompanyOwners { get; } = new List<CompanyOwner>();

    public virtual ICollection<Guarantor> Guarantors { get; } = new List<Guarantor>();
}
