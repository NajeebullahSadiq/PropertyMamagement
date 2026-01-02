using System;

namespace WebAPIBackend.Models;

public partial class PropertyPayment
{
    public int Id { get; set; }

    public int PropertyDetailsId { get; set; }

    public DateTime PaymentDate { get; set; }

    public double AmountPaid { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ReceiptNumber { get; set; }

    public double? BalanceRemaining { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual PropertyDetail? PropertyDetails { get; set; }
}
