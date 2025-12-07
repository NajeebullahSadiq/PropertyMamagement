using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models
{
    public class UserProfileWithCompany
    {

            [Key]
            public string? UserId { get; set; }
            public string? Email { get; set; }
            public string? UserName { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? PhotoPath { get; set; }
            public string? CompanyName { get; set; }
            public string? PhoneNumber { get; set; }
        
    }
}
