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
    companyTitle?: string;
    district?: string;
    sectionType?: string;  // annualReport, complaints, violations, inspection
    reportRegistrationDate?: Date | string;
    reportRegistrationDateFormatted?: string;
    
    // ============ Deed Counts ============
    saleDeedsCount?: number;
    rentalDeedsCount?: number;
    baiUlWafaDeedsCount?: number;
    vehicleTransactionDeedsCount?: number;
    deedItems?: string;  // JSON string
    
    // ============ Section 2: Complaints ============
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
    year?: string;
    month?: string;
    monitoringCount?: number;
    monitoringRemarks?: string;
    
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
    companyTitle?: string;
    district?: string;
    sectionType?: string;
    reportRegistrationDate?: string;
    
    // Deed counts
    saleDeedsCount?: number;
    rentalDeedsCount?: number;
    baiUlWafaDeedsCount?: number;
    vehicleTransactionDeedsCount?: number;
    deedItems?: DeedItem[];
    
    // Complaints
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
    year?: string;
    month?: string;
    monitoringCount?: number;
    monitoringRemarks?: string;
    
    // Calendar type for date conversion
    calendarType?: string;
}

/**
 * Section types for dropdown
 */
export const ActivityMonitoringSectionTypes = [
    { value: 'annualReport', label: 'گزارش سالانه', labelEn: 'Annual Report' },
    { value: 'complaints', label: 'ثبت شکایات', labelEn: 'Complaints Registration' },
    { value: 'violations', label: 'تخلفات دفاتر رهنمای معاملات', labelEn: 'Real Estate Violations' },
    { value: 'inspection', label: 'نظارت وبررسی فعالیت دفاتر رهنمای معاملات', labelEn: 'Inspection & Supervision' }
];

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
    remarks?: string;
}
