using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class VehiclesPropertyDetail
{
    public int Id { get; set; }

    public int PermitNo { get; set; }

    public int PilateNo { get; set; }

    public string? TypeOfVehicle { get; set; }

    public string? Model { get; set; }

    public int? EnginNo { get; set; }

    public int? ShasiNo { get; set; }

    public string? Color { get; set; }

    public double? Price { get; set; }

    public string? PriceText { get; set; }

    public double? RoyaltyAmount { get; set; }

    public int? PropertyTypeId { get; set; }

    public int? TransactionTypeId { get; set; }

    public string? Des { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Iscomplete { get; set; }

    public bool? Iseditable { get; set; }

    public virtual PropertyType? PropertyType { get; set; }

    public virtual TransactionType? TransactionType { get; set; }

    public virtual ICollection<Vehicleaudit> Vehicleaudits { get; } = new List<Vehicleaudit>();

    public virtual ICollection<VehiclesBuyerDetail> VehiclesBuyerDetails { get; } = new List<VehiclesBuyerDetail>();

    public virtual ICollection<VehiclesSellerDetail> VehiclesSellerDetails { get; } = new List<VehiclesSellerDetail>();

    public virtual ICollection<VehiclesWitnessDetail> VehiclesWitnessDetails { get; } = new List<VehiclesWitnessDetail>();
}
