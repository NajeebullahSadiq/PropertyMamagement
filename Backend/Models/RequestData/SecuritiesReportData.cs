using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// Request model for dynamic securities report generation
/// </summary>
public class SecuritiesReportRequest
{
    // Metrics to include
    public List<string> Metrics { get; set; } = new();
    
    // Grouping options
    public List<string> GroupBy { get; set; } = new();
    
    // Time filters
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    
    // Entity filters
    public string? TransactionGuideName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public int? DocumentType { get; set; } // 1=Property, 2=Vehicle
    public int? PropertySubType { get; set; }
    public int? VehicleSubType { get; set; }
    public int? RegistrationBookType { get; set; } // 1=New, 2=Duplicate
    
    // Value filters
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int? MinCount { get; set; }
    public int? MaxCount { get; set; }
    
    // Serial number filters
    public string? SerialNumberStart { get; set; }
    public string? SerialNumberEnd { get; set; }
    
    // Calendar type for date formatting
    public string? CalendarType { get; set; }
}

/// <summary>
/// Response model for securities report
/// </summary>
public class SecuritiesReportResponse
{
    public SecuritiesReportSummary Summary { get; set; } = new();
    public List<SecuritiesReportRow> Data { get; set; } = new();
    public SecuritiesReportMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Summary totals for the report
/// </summary>
public class SecuritiesReportSummary
{
    // Quantitative totals
    public int TotalDocuments { get; set; }
    public int TotalPropertyDocuments { get; set; }
    public int TotalVehicleDocuments { get; set; }
    public int PropertySaleCount { get; set; }
    public int BayWafaCount { get; set; }
    public int RentCount { get; set; }
    public int VehicleSaleCount { get; set; }
    public int VehicleExchangeCount { get; set; }
    public int RegistrationBookCount { get; set; }
    public int DuplicateBookCount { get; set; }
    public int TotalBookCount { get; set; }
    
    // Financial totals
    public decimal TotalDocumentsPrice { get; set; }
    public decimal TotalRegistrationBookPrice { get; set; }
    public decimal TotalSecuritiesPrice { get; set; }
    
    // Record count
    public int TotalRecords { get; set; }
}

/// <summary>
/// Individual row in the report (grouped or ungrouped)
/// </summary>
public class SecuritiesReportRow
{
    // Grouping keys
    public string? GroupKey { get; set; }
    public string? TransactionGuideName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? DateGroup { get; set; }
    public int? DocumentType { get; set; }
    public string? DocumentTypeName { get; set; }
    public int? RegistrationBookType { get; set; }
    public string? RegistrationBookTypeName { get; set; }
    
    // Counts
    public int RecordCount { get; set; }
    public int PropertySaleCount { get; set; }
    public int BayWafaCount { get; set; }
    public int RentCount { get; set; }
    public int VehicleSaleCount { get; set; }
    public int VehicleExchangeCount { get; set; }
    public int RegistrationBookCount { get; set; }
    public int DuplicateBookCount { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalBooks { get; set; }
    
    // Financial
    public decimal TotalDocumentsPrice { get; set; }
    public decimal RegistrationBookPrice { get; set; }
    public decimal TotalSecuritiesPrice { get; set; }
    
    // Serial ranges (for detail view)
    public string? PropertySaleSerialRange { get; set; }
    public string? BayWafaSerialRange { get; set; }
    public string? RentSerialRange { get; set; }
    public string? VehicleSaleSerialRange { get; set; }
    public string? VehicleExchangeSerialRange { get; set; }
    
    // Dates
    public string? DistributionDate { get; set; }
    public string? DeliveryDate { get; set; }
}

/// <summary>
/// Metadata about the report
/// </summary>
public class SecuritiesReportMetadata
{
    public DateTime GeneratedAt { get; set; }
    public string ReportTitle { get; set; } = string.Empty;
    public List<string> AppliedFilters { get; set; } = new();
    public List<string> GroupedBy { get; set; } = new();
    public List<string> IncludedMetrics { get; set; } = new();
}

/// <summary>
/// Available metrics for report builder
/// </summary>
public static class ReportMetrics
{
    // Quantitative metrics
    public const string TotalDocuments = "total_documents";
    public const string PropertySaleCount = "property_sale_count";
    public const string BayWafaCount = "bay_wafa_count";
    public const string RentCount = "rent_count";
    public const string VehicleSaleCount = "vehicle_sale_count";
    public const string VehicleExchangeCount = "vehicle_exchange_count";
    public const string RegistrationBookCount = "registration_book_count";
    public const string DuplicateBookCount = "duplicate_book_count";
    public const string TotalBookCount = "total_book_count";
    
    // Financial metrics
    public const string TotalDocumentsPrice = "total_documents_price";
    public const string RegistrationBookPrice = "registration_book_price";
    public const string TotalSecuritiesPrice = "total_securities_price";
    
    // Identity metrics
    public const string RegistrationNumber = "registration_number";
    public const string LicenseNumber = "license_number";
    public const string SerialNumbers = "serial_numbers";
}

/// <summary>
/// Available grouping options
/// </summary>
public static class ReportGroupBy
{
    public const string TransactionGuide = "transaction_guide";
    public const string LicenseNumber = "license_number";
    public const string DocumentType = "document_type";
    public const string RegistrationBookType = "book_type";
    public const string Month = "month";
    public const string Year = "year";
    public const string Date = "date";
}
