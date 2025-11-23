using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Haqulemtyaz
{
    public int Id { get; set; }

    public int? FormNumber { get; set; }

    public DateOnly? FormDate { get; set; }

    public int? SubmissionFormNumber { get; set; }

    public DateOnly? SubmissionFormDate { get; set; }

    public int? CompanyId { get; set; }

    public string? DocPath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public virtual CompanyDetail? Company { get; set; }
}
