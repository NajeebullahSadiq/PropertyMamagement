/**
 * Company Account/Financial Information (مالیه)
 * Interface for account info data
 */
export interface AccountInfo {
    id?: number;
    companyId: number;
    
    /** Account settlement information / نمرمكتوب تصفيه معلومات */
    settlementInfo?: string;
    
    /** Tax payment amount / تحويل ماليات (مبلغ) */
    taxPaymentAmount: number;
    
    /** Settlement year / سال تصفيه مالية */
    settlementYear?: number;
    
    /** Tax payment date / تاريخ تحويل ماليات */
    taxPaymentDate?: Date | string;
    
    /** Formatted tax payment date for display */
    taxPaymentDateFormatted?: string;
    
    /** Transaction count / تعدادی معامله */
    transactionCount?: number;
    
    /** Company commission / كمیشن رهنما */
    companyCommission?: number;
    
    /** Record creation timestamp */
    createdAt?: Date;
    
    /** User who created the record */
    createdBy?: string;
    
    /** Record status */
    status?: boolean;
}

/**
 * DTO for creating/updating account info
 */
export interface AccountInfoData {
    id?: number;
    companyId: number;
    settlementInfo?: string;
    taxPaymentAmount: number;
    settlementYear?: number;
    taxPaymentDate?: string;
    transactionCount?: number;
    companyCommission?: number;
}
