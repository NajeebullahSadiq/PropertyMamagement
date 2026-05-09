namespace WebAPIBackend.Models.RequestData;

public class DuplicateGuarantorFieldCheckRequest
{
    public string? SetSerialNumber { get; set; }
    public long? PropertyDocumentNumber { get; set; }
    public int GuarantorId { get; set; } = 0; // 0 for new, otherwise for editing
}
