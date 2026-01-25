using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class CompanyDetail
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public string? DocPath { get; set; }
    public string? Tin { get; set; }

    public virtual ICollection<CompanyOwner> CompanyOwners { get; } = new List<CompanyOwner>();

    public virtual ICollection<Gaurantee> Gaurantees { get; } = new List<Gaurantee>();

    public virtual ICollection<Guarantor> Guarantors { get; } = new List<Guarantor>();

    public virtual ICollection<Haqulemtyaz> Haqulemtyazs { get; } = new List<Haqulemtyaz>();

    public virtual ICollection<LicenseDetail> LicenseDetails { get; } = new List<LicenseDetail>();

    public virtual ICollection<PeriodicForm> PeriodicForms { get; } = new List<PeriodicForm>();
    public virtual ICollection<Companydetailsaudit> Companydetailsaudits { get; } = new List<Companydetailsaudit>();
    public virtual ICollection<CompanyAccountInfo> CompanyAccountInfos { get; } = new List<CompanyAccountInfo>();
    public virtual ICollection<CompanyCancellationInfo> CompanyCancellationInfos { get; } = new List<CompanyCancellationInfo>();
}
