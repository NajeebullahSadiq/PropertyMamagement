using System;

namespace WebAPIBackend.Models;

/// <summary>
/// Stores historical address records for company owners.
/// When an owner changes their address, the previous address is moved here.
/// </summary>
public class CompanyOwnerAddressHistory
{
    public int Id { get; set; }

    public int CompanyOwnerId { get; set; }

    // Address Fields
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public string? Village { get; set; }

    // Address Type: "Permanent" or "Current"
    public string AddressType { get; set; } = "Permanent";

    // Tracking Fields
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation Properties
    public virtual CompanyOwner? CompanyOwner { get; set; }
    public virtual Location? Province { get; set; }
    public virtual Location? District { get; set; }
}
