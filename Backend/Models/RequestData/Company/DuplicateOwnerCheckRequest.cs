namespace WebAPIBackend.Models.RequestData;

public class DuplicateOwnerCheckRequest
{
    public string FirstName { get; set; }
    public string FatherName { get; set; }
    public string GrandFather { get; set; }
    public int PropertyDetailsId { get; set; }
    public int SellerId { get; set; } = 0; // 0 for new, otherwise for editing
}
