namespace WebAPIBackend.Models.RequestData.Securities;

public class SecuritiesDistributionItemData
{
    public int? Id { get; set; }
    public int DocumentType { get; set; }
    public string? SerialStart { get; set; }
    public string? SerialEnd { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}
