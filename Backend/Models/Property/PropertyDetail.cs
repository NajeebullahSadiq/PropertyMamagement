using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models;

public partial class PropertyDetail
{
    public int Id { get; set; }

    [Column("PNumber")]
    public string? Pnumber { get; set; }

    public string? Parea { get; set; }

    public int? PunitTypeId { get; set; }

    public int? NumofFloor { get; set; }

    public int? NumofRooms { get; set; }

    public int? PropertyTypeId { get; set; }

    public string? CustomPropertyType { get; set; }

    public string? Price { get; set; }

    public string? PriceText { get; set; }

    public string? RoyaltyAmount { get; set; }

    public int? TransactionTypeId { get; set; }

    public string Status { get; set; } = "Draft";

    public string? VerifiedBy { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? Des { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? FilePath { get; set; }
    public string? PreviousDocumentsPath { get; set; }
    public string? ExistingDocumentsPath { get; set; }
    public bool? iscomplete { get; set; }
    public bool? iseditable { get; set; }

    public string? West { get; set; }

    public string? South { get; set; }

    public string? East { get; set; }

    public string? North { get; set; }

    public string? DocumentType { get; set; }

    public string? IssuanceNumber { get; set; }

    public DateTime? IssuanceDate { get; set; }

    public string? SerialNumber { get; set; }

    public DateTime? TransactionDate { get; set; }

    /// <summary>
    /// Calendar type for date conversion (not mapped to database)
    /// </summary>
    [NotMapped]
    public string? CalendarType { get; set; }

    /// <summary>
    /// Issuance date as string from frontend (not mapped to database)
    /// </summary>
    [NotMapped]
    public string? IssuanceDateStr { get; set; }

    /// <summary>
    /// Transaction date as string from frontend (not mapped to database)
    /// </summary>
    [NotMapped]
    public string? TransactionDateStr { get; set; }

    public virtual ICollection<BuyerDetail> BuyerDetails { get; } = new List<BuyerDetail>();

    public virtual ICollection<PropertyAddress> PropertyAddresses { get; } = new List<PropertyAddress>();

    public virtual PropertyType? PropertyType { get; set; }

    public virtual PunitType? PunitType { get; set; }

    public virtual ICollection<SellerDetail> SellerDetails { get; } = new List<SellerDetail>();

    public virtual TransactionType? TransactionType { get; set; }

    public virtual ICollection<WitnessDetail> WitnessDetails { get; } = new List<WitnessDetail>();
    public virtual ICollection<Propertyaudit> Propertyaudits { get; } = new List<Propertyaudit>();
    public virtual ICollection<PropertyOwnershipHistory> PropertyOwnershipHistories { get; } = new List<PropertyOwnershipHistory>();
    public virtual ICollection<PropertyPayment> PropertyPayments { get; } = new List<PropertyPayment>();
    public virtual ICollection<PropertyValuation> PropertyValuations { get; } = new List<PropertyValuation>();
    public virtual ICollection<PropertyDocument> PropertyDocuments { get; } = new List<PropertyDocument>();
}