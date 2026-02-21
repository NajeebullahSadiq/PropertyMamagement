using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class Guarantor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string? GrandFatherName { get; set; }

    public int? CompanyId { get; set; }

    // Electronic National ID - الیکټرونیکی تذکره
    public string? ElectronicNationalIdNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public int? PaddressProvinceId { get; set; }

    public int? PaddressDistrictId { get; set; }

    public string? PaddressVillage { get; set; }

    public int? TaddressProvinceId { get; set; }

    public int? TaddressDistrictId { get; set; }

    public string? TaddressVillage { get; set; }

    // Guarantee Information (merged from Gaurantee entity)
    public int? GuaranteeTypeId { get; set; }

    public long? PropertyDocumentNumber { get; set; }

    public DateOnly? PropertyDocumentDate { get; set; }

    public string? SenderMaktobNumber { get; set; }

    public DateOnly? SenderMaktobDate { get; set; }

    public long? AnswerdMaktobNumber { get; set; }

    public DateOnly? AnswerdMaktobDate { get; set; }

    public DateOnly? DateofGuarantee { get; set; }

    public long? GuaranteeDocNumber { get; set; }

    public DateOnly? GuaranteeDate { get; set; }

    public string? GuaranteeDocPath { get; set; }

    // Conditional fields for Sharia Deed (قباله شرعی)
    public string? CourtName { get; set; }           // محکمه نوم
    public string? CollateralNumber { get; set; }    // نمبر وثیقه

    // Conditional fields for Customary Deed (قباله عرفی)
    public string? SetSerialNumber { get; set; }     // نمبر سریال سټه
    public int? GuaranteeDistrictId { get; set; }    // ناحیه

    // Conditional fields for Cash (پول نقد)
    public string? BankName { get; set; }            // بانک
    public string? DepositNumber { get; set; }       // نمبر اویز
    public DateOnly? DepositDate { get; set; }       // تاریخ اویز

    // Witness History Tracking
    /// <summary>
    /// Indicates if this is the currently active witness for the company
    /// Only one witness can be active per company at a time
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time when this witness was replaced/expired
    /// </summary>
    public DateTime? ExpiredAt { get; set; }

    /// <summary>
    /// User ID who replaced/expired this witness
    /// </summary>
    public string? ExpiredBy { get; set; }

    /// <summary>
    /// Reference to the new witness that replaced this one
    /// </summary>
    public int? ReplacedByGuarantorId { get; set; }

    /// <summary>
    /// Navigation property to the witness that replaced this one
    /// </summary>
    public virtual Guarantor? ReplacedByGuarantor { get; set; }

    /// <summary>
    /// Navigation property to witnesses that this one replaced
    /// </summary>
    public virtual ICollection<Guarantor> ReplacedGuarantors { get; } = new List<Guarantor>();

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<Guarantorsaudit> Guarantorsaudits { get; } = new List<Guarantorsaudit>();

    public virtual GuaranteeType? GuaranteeType { get; set; }

    public virtual Location? PaddressDistrict { get; set; }

    public virtual Location? PaddressProvince { get; set; }

    public virtual Location? TaddressDistrict { get; set; }

    public virtual Location? TaddressProvince { get; set; }

    // Navigation property for Guarantee District
    public virtual Location? GuaranteeDistrict { get; set; }
}
