using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.sModels;

public partial class PropertyDetail
{
    public int Id { get; set; }

    [Column("PNumber")]
    public int Pnumber { get; set; }

    public int Parea { get; set; }

    public int? PunitTypeId { get; set; }

    public int? NumofFloor { get; set; }

    public int? NumofRooms { get; set; }

    public int? PropertyTypeId { get; set; }

    public double? Price { get; set; }

    public string? PriceText { get; set; }

    public double? RoyaltyAmount { get; set; }

    public int? TransactionTypeId { get; set; }

    public string? Des { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? FilePath { get; set; }

    public bool? Iseditable { get; set; }

    public bool? Iscomplete { get; set; }

    public string? West { get; set; }

    public string? South { get; set; }

    public string? East { get; set; }

    public string? North { get; set; }

    public string? DocumentType { get; set; }

    public string? IssuanceNumber { get; set; }

    public DateTime? IssuanceDate { get; set; }

    public string? SerialNumber { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual BuyerDetail? BuyerDetail { get; set; }

    public virtual ICollection<PropertyAddress> PropertyAddresses { get; } = new List<PropertyAddress>();

    public virtual PropertyType? PropertyType { get; set; }

    public virtual ICollection<Propertyaudit> Propertyaudits { get; } = new List<Propertyaudit>();

    public virtual PunitType? PunitType { get; set; }

    public virtual SellerDetail? SellerDetail { get; set; }

    public virtual TransactionType? TransactionType { get; set; }

    public virtual ICollection<WitnessDetail> WitnessDetails { get; } = new List<WitnessDetail>();
}
