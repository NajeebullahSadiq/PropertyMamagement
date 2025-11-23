using System;
using System.Collections.Generic;

namespace WebAPIBackend.sModels;

public partial class Gaurantee
{
    public int Id { get; set; }

    public int? GuaranteeTypeId { get; set; }

    public int? PropertyDocumentNumber { get; set; }

    public DateOnly? PropertyDocumentDate { get; set; }

    public string? SenderMaktobNumber { get; set; }

    public DateOnly? SenderMaktobDate { get; set; }

    public int? AnswerdMaktobNumber { get; set; }

    public DateOnly? AnswerdMaktobDate { get; set; }

    public DateOnly? DateofGuarantee { get; set; }

    public int? GuaranteeDocNumber { get; set; }

    public int? CompanyId { get; set; }

    public string? DocPath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public DateOnly? GuaranteeDate { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<Graunteeaudit> Graunteeaudits { get; } = new List<Graunteeaudit>();

    public virtual GuaranteeType? GuaranteeType { get; set; }
}
