/**
 * Company License Cancellation/Revocation Information (فسخ / لغوه)
 * Interface for cancellation info data
 */
export interface CancellationInfo {
    id?: number;
    companyId: number;
    
    /** Cancellation Type / نوعیت فسخ یا لغوه (فسخ or لغوه) */
    cancellationType?: string;
    
    /** License Cancellation Letter Number / نمبر مکتوب فسخ جواز */
    licenseCancellationLetterNumber?: string;
    
    /** Revenue Cancellation Letter Number / نمبر مکتوب فسخ عواید */
    revenueCancellationLetterNumber?: string;
    
    /** License Cancellation Letter Date / تاریخ مکتوب فسخ جواز */
    licenseCancellationLetterDate?: Date | string;
    
    /** Formatted license cancellation letter date for display */
    licenseCancellationLetterDateFormatted?: string;
    
    /** Revocation Letter Number / نمبر مکتوب لغوه جواز */
    revocationLetterNumber?: string;
    
    /** Revocation Revenue Letter Number / نمبر مکتوب لغوه عواید */
    revocationRevenueLetterNumber?: string;
    
    /** Revocation Letter Date / تاریخ مکتوب لغوه جواز */
    revocationLetterDate?: Date | string;
    
    /** Formatted revocation letter date for display */
    revocationLetterDateFormatted?: string;
    
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
    cancellationType?: string;
    licenseCancellationLetterNumber?: string;
    revenueCancellationLetterNumber?: string;
    licenseCancellationLetterDate?: string;
    revocationLetterNumber?: string;
    revocationRevenueLetterNumber?: string;
    revocationLetterDate?: string;
    remarks?: string;
}
