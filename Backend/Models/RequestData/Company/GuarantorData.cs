namespace WebAPIBackend.Models.RequestData
{
    public class GuarantorData
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string FatherName { get; set; } = null!;

        public string? GrandFatherName { get; set; }

        public int? CompanyId { get; set; }

        // Electronic National ID - الیکټرونیکی تذکره
        public string? ElectronicNationalIdNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public int? PaddressProvinceId { get; set; }

        public int? PaddressDistrictId { get; set; }

        public string? PaddressVillage { get; set; }

        public int? TaddressProvinceId { get; set; }

        public int? TaddressDistrictId { get; set; }

        public string? TaddressVillage { get; set; }

        // Guarantee Information
        public int? GuaranteeTypeId { get; set; }

        public long? PropertyDocumentNumber { get; set; }

        public string? PropertyDocumentDate { get; set; }

        public string? SenderMaktobNumber { get; set; }

        public string? SenderMaktobDate { get; set; }

        public long? AnswerdMaktobNumber { get; set; }

        public string? AnswerdMaktobDate { get; set; }

        public string? DateofGuarantee { get; set; }

        public long? GuaranteeDocNumber { get; set; }

        public string? GuaranteeDate { get; set; }

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
        public string? DepositDate { get; set; }         // تاریخ اویز (string for calendar conversion)

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// </summary>
        public string? CalendarType { get; set; }
    }
}
