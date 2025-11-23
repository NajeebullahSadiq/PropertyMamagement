using System;

namespace WebAPIBackend.Models.RequestData
{
    public class WitnessDetailRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string FatherName { get; set; } = null!;
        public string? IndentityCardNumber { get; set; } // String to handle large numbers from frontend
        public string? PhoneNumber { get; set; }
        public int? PropertyDetailsId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? NationalIdCard { get; set; }
    }
}
