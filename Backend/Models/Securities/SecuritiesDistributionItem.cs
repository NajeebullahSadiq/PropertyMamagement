using System;

namespace WebAPIBackend.Models.Securities;

public class SecuritiesDistributionItem
{
    public int Id { get; set; }
    
    public int SecuritiesDistributionId { get; set; }
    
    /// <summary>
    /// Document Type:
    /// 1 = سټه یی خرید و فروش (Property Sale Document)
    /// 2 = سټه یی بیع وفا (Bay Wafa Document)
    /// 3 = سټه یی کرایی (Rent Document)
    /// 4 = سټه وسایط نقلیه (Vehicle Document)
    /// 5 = کتاب ثبت (Registration Book)
    /// 6 = کتاب ثبت مثنی (Duplicate Registration Book)
    /// </summary>
    public int DocumentType { get; set; }
    
    /// <summary>
    /// Serial number start (for document types 1-4)
    /// </summary>
    public string? SerialStart { get; set; }
    
    /// <summary>
    /// Serial number end (for document types 1-4)
    /// </summary>
    public string? SerialEnd { get; set; }
    
    /// <summary>
    /// Count of documents
    /// Auto-calculated for types 1-4: (SerialEnd - SerialStart + 1)
    /// Manual input for types 5-6
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Price for this item
    /// Types 1-4: Count × 4000
    /// Type 5: Count × 1000
    /// Type 6: Count × 20000
    /// </summary>
    public decimal Price { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    // Navigation property
    public virtual SecuritiesDistribution? SecuritiesDistribution { get; set; }
}
