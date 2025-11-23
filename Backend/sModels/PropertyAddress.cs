using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class PropertyAddress
{
    public int Id { get; set; }

    public int? ProvinceId { get; set; }

    public int? DistrictId { get; set; }

    public int? PropertyDetailsId { get; set; }

    public string? Village { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }
}
