using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Setum
{
    public int Id { get; set; }

    public int? TransactionTypeId { get; set; }

    public int? InquiryNumber { get; set; }

    public DateOnly? InquiryDate { get; set; }

    public int? SetaSerialNumber { get; set; }

    public DateOnly? SetaStampedDate { get; set; }

    public int? CompanyId { get; set; }

    public string? DocPath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }
}
