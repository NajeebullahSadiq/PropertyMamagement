/**
 * Company License Cancellation/Revocation Information (فسخ / لغوه)
 * Interface for cancellation info data
 */
export interface CancellationInfo {
    id?: number;
    companyId: number;
    
    /** License Cancellation Letter Number / نمبر مکتوب فسخ جواز */
    licenseCancellationLetterNumber?: string;
    
    /** Revenue Cancellation Letter Number / نمبر مکتوب فسخ عواید */
    revenueCancellationLetterNumber?: string;
    
    /** License Cancellation Letter Date / تاریخ مکتوب فسخ جواز */
    licenseCancellationLetterDate?: Date | string;
    
    /** Formatted license cancellation letter date for display */
    licenseCancellationLetterDateFormatted?: string;
    
    /** Remarks/Notes / ملاحظات */
    remarks?: string;
    
    /** Record creation timestamp */
    createdAt?: Date;
    
    /** User who created the record */
    createdBy?: string;
    
    /** Record status */
    status?: boolean;
}

/**
 * DTO for creating/updating cancellation info
 */
export interface CancellationInfoData {
    id?: number;
    companyId: number;
    licenseCancellationLetterNumber?: string;
    revenueCancellationLetterNumber?: string;
    licenseCancellationLetterDate?: string;
    remarks?: string;
}
