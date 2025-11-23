using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class EducationLevel
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public short? Parentid { get; set; }

    public string? Sorter { get; set; }

    public virtual ICollection<CompanyOwner> CompanyOwners { get; } = new List<CompanyOwner>();
}
