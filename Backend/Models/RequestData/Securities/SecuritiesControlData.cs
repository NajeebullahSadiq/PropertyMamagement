using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// DTO for Securities Control API requests
/// کنټرول ورودی و خروجی اسناد بهادار
/// </summary>
public class SecuritiesControlData
{
    public int? Id { get; set; }

    // Tab 1: معلومات عمومی و ثبت پیشنهاد
    [Required(ErrorMessage = "شماره مسلسل الزامی است")]
    [MaxLength(50, ErrorMessage = "شماره مسلسل نباید بیشتر از ۵۰ حرف باشد")]
    public string SerialNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع سند بهادار الزامی است")]
    public int SecurityDocumentType { get; set; }

    [MaxLength(100)]
    public string? ProposalNumber { get; set; }

    public DateOnly? ProposalDate { get; set; }

    [MaxLength(100)]
    public string? DistributionTicketNumber { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    // Tab 2: مشخصات اسناد بهادار و تعداد آنها
    public int? SecuritiesType { get; set; }

    // Property Sale Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? PropertySaleCount { get; set; }

    [MaxLength(100)]
    public string? PropertySaleSerialStart { get; set; }

    [MaxLength(100)]
    public string? PropertySaleSerialEnd { get; set; }

    // Bay Wafa Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? BayWafaCount { get; set; }

    [MaxLength(100)]
    public string? BayWafaSerialStart { get; set; }

    [MaxLength(100)]
    public string? BayWafaSerialEnd { get; set; }

    // Rent Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? RentCount { get; set; }

    [MaxLength(100)]
    public string? RentSerialStart { get; set; }

    [MaxLength(100)]
    public string? RentSerialEnd { get; set; }

    // Vehicle Sale Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? VehicleSaleCount { get; set; }

    [MaxLength(100)]
    public string? VehicleSaleSerialStart { get; set; }

    [MaxLength(100)]
    public string? VehicleSaleSerialEnd { get; set; }

    // Exchange Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? ExchangeCount { get; set; }

    [MaxLength(100)]
    public string? ExchangeSerialStart { get; set; }

    [MaxLength(100)]
    public string? ExchangeSerialEnd { get; set; }

    // Registration Book Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? RegistrationBookCount { get; set; }

    [MaxLength(100)]
    public string? RegistrationBookSerialStart { get; set; }

    [MaxLength(100)]
    public string? RegistrationBookSerialEnd { get; set; }

    // Printed Petition Fields
    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? PrintedPetitionCount { get; set; }

    [MaxLength(100)]
    public string? PrintedPetitionSerialStart { get; set; }

    [MaxLength(100)]
    public string? PrintedPetitionSerialEnd { get; set; }

    // Tab 3: ثبت و کنترول توزیع اسناد
    [MaxLength(100)]
    public string? DistributionStartNumber { get; set; }

    [MaxLength(100)]
    public string? DistributionEndNumber { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "تعداد باید مثبت باشد")]
    public int? DistributedPersonsCount { get; set; }

    // Tab 4: ملاحظات و توضیحات
    [MaxLength(2000)]
    public string? Remarks { get; set; }
}
