using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebAPIBackend.Models.RequestData.Securities;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// DTO for Securities Distribution API requests
/// </summary>
public class SecuritiesDistributionData
{
    public int? Id { get; set; }

    // Tab 1: مشخصات رهنمای معاملات
    [Required(ErrorMessage = "نمبر ثبت الزامی است")]
    [MaxLength(50, ErrorMessage = "نمبر ثبت نباید بیشتر از ۵۰ حرف باشد")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم صاحب امتیاز جواز الزامی است")]
    [MaxLength(200, ErrorMessage = "اسم صاحب امتیاز جواز نباید بیشتر از ۲۰۰ حرف باشد")]
    public string LicenseOwnerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم پدر صاحب امتیاز جواز الزامی است")]
    [MaxLength(200, ErrorMessage = "اسم پدر صاحب امتیاز جواز نباید بیشتر از ۲۰۰ حرف باشد")]
    public string LicenseOwnerFatherName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام رهنمای معاملات الزامی است")]
    [MaxLength(200, ErrorMessage = "نام رهنمای معاملات نباید بیشتر از ۲۰۰ حرف باشد")]
    public string TransactionGuideName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نمبر جواز الزامی است")]
    [MaxLength(50, ErrorMessage = "نمبر جواز نباید بیشتر از ۵۰ حرف باشد")]
    public string LicenseNumber { get; set; } = string.Empty;

    // Tab 2: مشخصات اسناد توزیعی - Now using Items collection
    public List<SecuritiesDistributionItemData> Items { get; set; } = new List<SecuritiesDistributionItemData>();

    // Tab 3: قیمت اسناد بهادار
    [Range(0, double.MaxValue, ErrorMessage = "قیمت باید مثبت باشد")]
    public decimal? PricePerDocument { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "قیمت باید مثبت باشد")]
    public decimal? TotalDocumentsPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "قیمت باید مثبت باشد")]
    public decimal? TotalSecuritiesPrice { get; set; }

    // Tab 4: مشخصات آویز تحویلی و تاریخ توزیع
    [MaxLength(100)]
    public string? BankReceiptNumber { get; set; }

    public DateOnly? DeliveryDate { get; set; }
    public DateOnly? DistributionDate { get; set; }
}
