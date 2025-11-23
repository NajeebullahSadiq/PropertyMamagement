using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class PeriodicForm
{
    public int Id { get; set; }

    public int? ReferenceId { get; set; }

    public int? FormNumber { get; set; }

    public DateOnly? FormDate { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public string? MaktobNumber { get; set; }

    public DateOnly? MaktobDate { get; set; }

    public int? DiagnosisNumber { get; set; }

    public string? Details { get; set; }

    public string? DocPath { get; set; }

    public int? CompanyId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual FormsReference? Reference { get; set; }
}
