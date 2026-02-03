using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class VehiclesPropertyDetail
{
    public int Id { get; set; }

    public string? PermitNo { get; set; }

    public string? PilateNo { get; set; }

    public string? TypeOfVehicle { get; set; }

    public string? Model { get; set; }

    public string? EnginNo { get; set; }

    public string? ShasiNo { get; set; }

    public string? Color { get; set; }

    public string? Price { get; set; }

    public string? PriceText { get; set; }

    public string? HalfPrice { get; set; }

    public string? RoyaltyAmount { get; set; }

    public int? PropertyTypeId { get; set; }

    public int? TransactionTypeId { get; set; }

    public string? Des { get; set; }

    public string? FilePath { get; set; }

    public string? VehicleHand { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    /// <summary>
    /// Company ID that created this vehicle record (for data isolation)
    /// </summary>
    public int? CompanyId { get; set; }

    public bool? iscomplete { get; set; }
    public bool? iseditable { get; set; }

    public virtual PropertyType? PropertyType { get; set; }

    public virtual TransactionType? TransactionType { get; set; }
    public virtual ICollection<Vehicleaudit> Vehicleaudits { get; } = new List<Vehicleaudit>();

    public virtual ICollection<VehiclesBuyerDetail> VehiclesBuyerDetails { get; } = new List<VehiclesBuyerDetail>();

    public virtual ICollection<VehiclesSellerDetail> VehiclesSellerDetails { get; } = new List<VehiclesSellerDetail>();

    public virtual ICollection<VehiclesWitnessDetail> VehiclesWitnessDetails { get; } = new List<VehiclesWitnessDetail>();
}
