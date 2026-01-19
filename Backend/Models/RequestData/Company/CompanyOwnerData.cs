namespace WebAPIBackend.Models.RequestData
{
    public class CompanyOwnerData
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string FatherName { get; set; } = null!;

        public string? GrandFatherName { get; set; }

        public short? EducationLevelId { get; set; }

        public string? DateofBirth { get; set; }

        // Electronic National ID - الیکټرونیکی تذکره
        public string? ElectronicNationalIdNumber { get; set; }

        public int? CompanyId { get; set; }

        public string? PothoPath { get; set; }

        // Contact Information
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// </summary>
        public string? CalendarType { get; set; }

        // Owner's Own Address Fields (آدرس اصلی مالک)
        public int? OwnerProvinceId { get; set; }
        public int? OwnerDistrictId { get; set; }
        public string? OwnerVillage { get; set; }

        // Permanent Address Fields (آدرس دایمی) - Current Residence
        public int? PermanentProvinceId { get; set; }
        public int? PermanentDistrictId { get; set; }
        public string? PermanentVillage { get; set; }

        /// <summary>
        /// Flag to indicate if this is an address change operation.
        /// When true, the existing address will be archived to history.
        /// </summary>
        public bool IsAddressChange { get; set; } = false;
    }
}
