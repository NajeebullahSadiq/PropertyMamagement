using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class CompanyOwnerAddress
{
    public int Id { get; set; }

    public int? AddressTypeId { get; set; }

    public int? ProvinceId { get; set; }

    public int? DistrictId { get; set; }

    public int? CompanyOwnerId { get; set; }

    public string? Village { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public virtual AddressType? AddressType { get; set; }

    public virtual CompanyOwner? CompanyOwner { get; set; }

    public virtual Location? District { get; set; }

    public virtual Location? Province { get; set; }
}
