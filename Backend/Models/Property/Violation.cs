using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Violation
{
    public int Id { get; set; }

    public int ViolationTypeId { get; set; }

    public DateOnly? ViolationDate { get; set; }

    public DateOnly? NotifyDate { get; set; }

    public int? NumberOfViolation { get; set; }

    public DateOnly? DateOfsummons { get; set; }

    public DateOnly? PresentedDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ViolationType ViolationType { get; set; } = null!;
}
