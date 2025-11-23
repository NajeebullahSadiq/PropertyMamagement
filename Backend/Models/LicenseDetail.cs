using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class LicenseDetail
{
    public int Id { get; set; }

    public double LicenseNumber { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public int? AreaId { get; set; }

    public string? OfficeAddress { get; set; }

    public int? CompanyId { get; set; }

    public string? DocPath { get; set; }

    public string? LicenseType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public virtual Area? Area { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<Licenseaudit> Licenseaudits { get; } = new List<Licenseaudit>();
}
