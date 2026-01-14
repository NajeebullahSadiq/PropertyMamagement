namespace WebAPIBackend.Application.Dashboard.DTOs
{
    public class DashboardSummaryDto
    {
        public double? TotalAmount { get; set; }
        public double? TotalAmountNotCompleted { get; set; }
        public double? TotalAmountCompleted { get; set; }
        public int TotalTransactionCompleted { get; set; }
        public int TotalTransactionNotCompleted { get; set; }
        public int TotalTransaction { get; set; }
        public double? TotalRoyaltyAmount { get; set; }
        public double? TotalRoyaltyAmountNotCompleted { get; set; }
        public double? TotalRoyaltyAmountCompleted { get; set; }
    }

    public class CompanyDashboardDto
    {
        public int TotalCompanyRegistered { get; set; }
    }

    public class LicenseExpiredDashboardDto
    {
        public int TotalLicenseExpired { get; set; }
    }

    public class CombinedDashboardDto
    {
        public DashboardSummaryDto? VehicleSummary { get; set; }
        public DashboardSummaryDto? PropertySummary { get; set; }
    }

    public class TransactionByTypeDto
    {
        public string? Name { get; set; }
        public double Amount { get; set; }
    }

    public class EstateDashboardDto
    {
        public List<TransactionByTypeDto> TransactionDataByTypeTotal { get; set; } = new();
        public List<TransactionByTypeDto> TransactionDataByTypeCompleted { get; set; } = new();
        public List<TransactionByTypeDto> TransactionDataByTypeNotCompleted { get; set; } = new();
        public List<TransactionByTypeDto> TransactionDataByTransactionTypeTotal { get; set; } = new();
        public List<TransactionByTypeDto> TransactionDataByTransactionTypeNotCompleted { get; set; } = new();
        public List<TransactionByTypeDto> TransactionDataByTransactionTypeCompleted { get; set; } = new();
        public DashboardSummaryDto? TotalRecord { get; set; }
    }

    public class TopUserSummaryDto
    {
        public string? CreatedBy { get; set; }
        public int TotalPropertiesCreated { get; set; }
        public double TotalPriceOfProperties { get; set; }
        public string? CompanyTitle { get; set; }
    }

    public class TopUsersSummaryResultDto
    {
        public int TotalProperties { get; set; }
        public double TotalPrice { get; set; }
        public List<TopUserSummaryDto> TopUsers { get; set; } = new();
    }

    public class MonthlyDataDto
    {
        public string? Month { get; set; }
        public double TotalPriceOfProperties { get; set; }
    }

    public class PropertyTypeMonthlyDto
    {
        public string? PropertyType { get; set; }
        public List<MonthlyDataDto> Data { get; set; } = new();
    }
}
