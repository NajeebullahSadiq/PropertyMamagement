using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class AddressType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Des { get; set; }

    public virtual ICollection<CompanyOwnerAddress> CompanyOwnerAddresses { get; } = new List<CompanyOwnerAddress>();

}
