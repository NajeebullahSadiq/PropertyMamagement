/**
 * نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان
 * Monitoring of Real Estate Offices & Petition Writers Activities
 * Single Table Design - All sections in one entity
 */
export interface ActivityMonitoringRecord {
    id?: number;
    
    // ============ Common Fields ============
    serialNumber?: string;
    licenseNumber?: string;
    licenseHolderName?: string;
    district?: string;
    sectionType?: string;  // complaints, violations, inspection
    reportRegistrationDate?: Date | string;
    reportRegistrationDateFormatted?: string;
    
    // ============ Deed Counts ============
    saleDeedsCount?: number;
    rentalDeedsCount?: number;
    baiUlWafaDeedsCount?: number;
    vehicleTransactionDeedsCount?: number;
    annualReportRemarks?: string;
    deedItems?: string;  // JSON string
    
    // ============ Section 2: Complaints ============
    complaintRegistrationDate?: Date | string;
    complaintRegistrationDateFormatted?: string;
    complaintSubject?: string;
    complainantName?: string;
    complaintActionsTaken?: string;
    complaintRemarks?: string;
    
    // ============ Section 3: Violations ============
    violationStatus?: string;
    violationType?: string;
    violationDate?: Date | string;
    violationDateFormatted?: string;
    closureDate?: Date | string;
    closureDateFormatted?: string;
    closureReason?: string;
    violationActionsTaken?: string;
    violationRemarks?: string;
    
    // ============ Section 4: Inspections ============
    monitoringType?: string;
    month?: string;
    monitoringCount?: number;
    
    // ============ Audit ============
    status?: boolean;
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
}

/**
 * DTO for creating/updating activity monitoring record (Single Table Design)
 */
export interface ActivityMonitoringData {
    id?: number;
    
    // Common fields
    serialNumber?: string;
    licenseNumber?: string;
    licenseHolderName?: string;
    district?: string;
    sectionType?: string;
    reportRegistrationDate?: string;
    
    // Deed counts
    saleDeedsCount?: number;
    rentalDeedsCount?: number;
    baiUlWafaDeedsCount?: number;
    vehicleTransactionDeedsCount?: number;
    annualReportRemarks?: string;
    deedItems?: DeedItem[];
    
    // Complaints
    complaintRegistrationDate?: string;
    complaintSubject?: string;
    complainantName?: string;
    complaintActionsTaken?: string;
    complaintRemarks?: string;
    
    // Violations
    violationStatus?: string;
    violationType?: string;
    violationDate?: string;
    closureDate?: string;
    closureReason?: string;
    violationActionsTaken?: string;
    violationRemarks?: string;
    
    // Inspections
    monitoringType?: string;
    month?: string;
    monitoringCount?: number;
    
    // Calendar type for date conversion
    calendarType?: string;
}

/**
 * List response
 */
export interface ActivityMonitoringListResponse {
    items: ActivityMonitoringRecord[];
    totalCount: number;
    page: number;
    pageSize: number;
}

/**
 * Deed Document Type for Annual Report
 */
export interface DeedDocumentTypeInfo {
    id: number;
    name: string;
    nameEn: string;
    hasSerial: boolean;
}

export const DeedDocumentTypes: DeedDocumentTypeInfo[] = [
    { id: 1, name: 'سته‌های وسایط نقلیه', nameEn: 'Vehicle Transaction Deeds', hasSerial: true },
    { id: 2, name: 'سته‌های کرایی', nameEn: 'Rental Deeds', hasSerial: true },
    { id: 3, name: 'سته‌های فروش', nameEn: 'Sale Deeds', hasSerial: true },
    { id: 4, name: 'سته‌های بیع الوفا', nameEn: 'Bai Ul Wafa Deeds', hasSerial: true }
];

/**
 * Deed Item for tracking serial numbers
 */
export interface DeedItem {
    id?: number;
    deedType: number;
    serialStart?: string;
    serialEnd?: string;
    count: number;
}
