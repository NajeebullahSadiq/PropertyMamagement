using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models.RequestData;

/// <summary>
/// Request model for dynamic petition writer securities report generation
/// گزارش‌گیری سند بهادار عریضه‌نویسان
/// </summary>
public class PetitionWriterReportRequest
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
    
    // Identity filters
    public string? PetitionWriterName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    
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
/// Response model for petition writer securities report
/// </summary>
public class PetitionWriterReportResponse
{
    public PetitionWriterReportSummary Summary { get; set; } = new();
    public List<PetitionWriterReportRow> Data { get; set; } = new();
    public PetitionWriterReportMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Summary totals for the petition writer report
/// خلاصه آماری گزارش عریضه‌نویسان
/// </summary>
public class PetitionWriterReportSummary
{
    // Quantitative totals
    /// <summary>
    /// تعداد مجموعی عریضه‌ها
    /// </summary>
    public int TotalPetitionCount { get; set; }
    
    /// <summary>
    /// تعداد رکوردها / توزیعات
    /// </summary>
    public int TotalRecords { get; set; }
    
    /// <summary>
    /// تعداد عریضه‌نویسان یکتا
    /// </summary>
    public int UniquePetitionWriters { get; set; }
    
    /// <summary>
    /// تعداد جوازهای یکتا
    /// </summary>
    public int UniqueLicenses { get; set; }
    
    // Financial totals
    /// <summary>
    /// مبلغ مجموع پول عریضه‌ها
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// میانگین مبلغ هر توزیع
    /// </summary>
    public decimal AverageAmountPerDistribution { get; set; }
    
    /// <summary>
    /// میانگین تعداد عریضه در هر توزیع
    /// </summary>
    public decimal AveragePetitionsPerDistribution { get; set; }
    
    // Serial tracking
    /// <summary>
    /// کمترین سریال نمبر
    /// </summary>
    public string? MinSerialNumber { get; set; }
    
    /// <summary>
    /// بیشترین سریال نمبر
    /// </summary>
    public string? MaxSerialNumber { get; set; }
}

/// <summary>
/// Individual row in the petition writer report (grouped or ungrouped)
/// </summary>
public class PetitionWriterReportRow
{
    // Grouping keys
    public string? GroupKey { get; set; }
    public string? PetitionWriterName { get; set; }
    public string? PetitionWriterFatherName { get; set; }
    public string? LicenseNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? DateGroup { get; set; }
    public int? YearGroup { get; set; }
    public int? MonthGroup { get; set; }
    public string? MonthName { get; set; }
    
    // Counts
    public int RecordCount { get; set; }
    public int TotalPetitionCount { get; set; }
    
    // Financial
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    
    // Serial ranges
    public string? SerialNumberStart { get; set; }
    public string? SerialNumberEnd { get; set; }
    public string? SerialRange { get; set; }
    
    // Bank receipt
    public string? BankReceiptNumber { get; set; }
    
    // Dates
    public string? DistributionDate { get; set; }
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Metadata about the petition writer report
/// </summary>
public class PetitionWriterReportMetadata
{
    public DateTime GeneratedAt { get; set; }
    public string ReportTitle { get; set; } = "گزارش سند بهادار عریضه‌نویسان";
    public List<string> AppliedFilters { get; set; } = new();
    public List<string> GroupedBy { get; set; } = new();
    public List<string> IncludedMetrics { get; set; } = new();
}

/// <summary>
/// Available metrics for petition writer report builder
/// </summary>
public static class PetitionWriterReportMetrics
{
    // Quantitative metrics
    public const string TotalPetitionCount = "total_petition_count";
    public const string RecordCount = "record_count";
    public const string UniquePetitionWriters = "unique_petition_writers";
    public const string UniqueLicenses = "unique_licenses";
    
    // Financial metrics
    public const string TotalAmount = "total_amount";
    public const string AverageAmount = "average_amount";
    
    // Identity metrics
    public const string RegistrationNumber = "registration_number";
    public const string LicenseNumber = "license_number";
    public const string SerialNumbers = "serial_numbers";
    public const string PetitionWriterName = "petition_writer_name";
}

/// <summary>
/// Available grouping options for petition writer report
/// </summary>
public static class PetitionWriterReportGroupBy
{
    public const string PetitionWriter = "petition_writer";
    public const string LicenseNumber = "license_number";
    public const string Month = "month";
    public const string Year = "year";
    public const string Date = "date";
}
