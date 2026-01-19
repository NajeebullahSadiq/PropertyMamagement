using System;
using System.Collections.Generic;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Models;

public partial class CompanyOwner
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string? GrandFatherName { get; set; }

    public short? EducationLevelId { get; set; }

    public DateOnly? DateofBirth { get; set; }

    // Electronic National ID - الیکټرونیکی تذکره
    public string? ElectronicNationalIdNumber { get; set; }

    public int? CompanyId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public bool? Status { get; set; }

    public string? PothoPath { get; set; }

    // Contact Information
    public string? PhoneNumber { get; set; }
    public string? WhatsAppNumber { get; set; }

    // Owner's Own Address Fields (آدرس اصلی مالک)
    public int? OwnerProvinceId { get; set; }
    public int? OwnerDistrictId { get; set; }
    public string? OwnerVillage { get; set; }

    // Permanent Address Fields (آدرس دایمی) - Current Residence
    public int? PermanentProvinceId { get; set; }
    public int? PermanentDistrictId { get; set; }
    public string? PermanentVillage { get; set; }

    public virtual CompanyDetail? Company { get; set; }

    public virtual ICollection<CompanyOwnerAddress> CompanyOwnerAddresses { get; } = new List<CompanyOwnerAddress>();
    public virtual ICollection<CompanyOwnerAddressHistory> AddressHistory { get; } = new List<CompanyOwnerAddressHistory>();
    public virtual ICollection<Companyowneraudit> Companyowneraudits { get; } = new List<Companyowneraudit>();

    public virtual EducationLevel? EducationLevel { get; set; }

    // Navigation properties for address locations
    public virtual Location? OwnerProvince { get; set; }
    public virtual Location? OwnerDistrict { get; set; }
    public virtual Location? PermanentProvince { get; set; }
    public virtual Location? PermanentDistrict { get; set; }
}
