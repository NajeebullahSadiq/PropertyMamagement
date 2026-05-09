namespace WebAPIBackend.Models.RequestData;

public class DuplicateCompanyOwnerCheckRequest
{
    public string? FirstName { get; set; }
    public string? FatherName { get; set; }
    public string? GrandFatherName { get; set; }
    public string? ElectronicNationalIdNumber { get; set; }
    public int OwnerId { get; set; } = 0; // 0 for new, otherwise for editing
}
