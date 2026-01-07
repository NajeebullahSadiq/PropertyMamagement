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

        public int? IdentityCardTypeId { get; set; }

        public string? IndentityCardNumber { get; set; }

        public string? Jild { get; set; }

        public string? Safha { get; set; }

        public int? CompanyId { get; set; }

        public string? SabtNumber { get; set; }
        public string? PothoPath { get; set; }

        // Contact Information
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// </summary>
        public string? CalendarType { get; set; }

        // Permanent Address Fields (آدرس دایمی)
        public int? PermanentProvinceId { get; set; }
        public int? PermanentDistrictId { get; set; }
        public string? PermanentVillage { get; set; }

        // Temporary Address Fields (آدرس موقت)
        public int? TemporaryProvinceId { get; set; }
        public int? TemporaryDistrictId { get; set; }
        public string? TemporaryVillage { get; set; }

        /// <summary>
        /// Flag to indicate if this is an address change operation.
        /// When true, the existing address will be archived to history.
        /// </summary>
        public bool IsAddressChange { get; set; } = false;
    }
}
