export interface TransactionDataItem {
  name: string;
  amount: number;
}

export interface RecordSummary {
  totalAmount: number;
  totalAmountNotCompleted: number;
  totalAmountCompleted: number;
  totalTransaction: number;
  totalTransactionNotCompleted: number;
  totalTransactionCompleted: number;
  totalRoyaltyAmount: number;
  totalRoyaltyAmountNotCompleted: number;
  totalRoyaltyAmountCompleted: number;
}

export interface EstateDashboardData {
  totalRecord: RecordSummary | null;
  transactionDataByTypeTotal: TransactionDataItem[];
  transactionDataByTypeCompleted: TransactionDataItem[];
  transactionDataByTypeNotCompleted: TransactionDataItem[];
  transactionDataByTransactionTypeTotal: TransactionDataItem[];
  transactionDataByTransactionTypeCompleted: TransactionDataItem[];
  transactionDataByTransactionTypeNotCompleted: TransactionDataItem[];
}

export interface VehicleDashboardData {
  totalRecord: RecordSummary | null;
}

export interface CompanyDashboardData {
  totalCompanyRegisterd: RecordSummary | null;
}

export interface ExpiredLicenseDashboardData {
  totalLicenseExpired: RecordSummary | null;
}

export interface PropertyTypeMonthlyData {
  month: string;
  totalPriceOfProperties: number;
}

export interface PropertyTypeByMonthData {
  propertyType: string;
  data: PropertyTypeMonthlyData[];
}

export interface VehicleReportData {
  month: string;
  totalPriceOfProperties: number;
}
