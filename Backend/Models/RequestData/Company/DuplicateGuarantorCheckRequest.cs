namespace WebAPIBackend.Models.RequestData;

public class DuplicateGuarantorCheckRequest
{
    public string? ElectronicNationalIdNumber { get; set; }
    public int GuarantorId { get; set; } = 0; // 0 for new, otherwise for editing
}
