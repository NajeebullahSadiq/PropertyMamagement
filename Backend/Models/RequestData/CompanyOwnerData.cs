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

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// </summary>
        public string? CalendarType { get; set; }
    }
}
