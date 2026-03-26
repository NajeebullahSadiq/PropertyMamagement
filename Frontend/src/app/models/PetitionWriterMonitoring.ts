/**
 * ثبت نظارت بر فعالیت عریضه نویسان
 * Registration of Monitoring the Activities of Petition Writers
 * Single Table Design - All sections in one entity
 */

/**
 * Main record interface
 */
export interface PetitionWriterMonitoringRecord {
    id?: number;
    
    // ============ Common Fields ============
    serialNumber?: string;
    sectionType?: string;  // complaints, violations, monitoring
    registrationDate?: Date | string;
    registrationDateFormatted?: string;
    
    // ============ Section 1: Complaints Registration ============
    complainantName?: string;
    complaintSubject?: string;
    complaintActionsTaken?: string;
    complaintRemarks?: string;
    
    // ============ Section 2: Violations ============
    petitionWriterName?: string;
    petitionWriterLicenseNumber?: string;
    petitionWriterDistrict?: string;
    violationType?: string;
    violationActionsTaken?: string;
    violationRemarks?: string;
    
    // ============ Section 3: Monitoring Activities ============
    monitoringYear?: string;
    monitoringMonth?: string;
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
 * DTO for creating/updating petition writer monitoring record
 */
export interface PetitionWriterMonitoringData {
    id?: number;
    
    // Common fields
    serialNumber?: string;
    sectionType?: string;
    registrationDate?: string;
    
    // Complaints
    complainantName?: string;
    complaintSubject?: string;
    complaintActionsTaken?: string;
    complaintRemarks?: string;
    
    // Violations
    petitionWriterName?: string;
    petitionWriterLicenseNumber?: string;
    petitionWriterDistrict?: string;
    violationType?: string;
    violationActionsTaken?: string;
    violationRemarks?: string;
    
    // Monitoring
    monitoringYear?: string;
    monitoringMonth?: string;
    monitoringCount?: number;
    monitoringRemarks?: string;
    
    // Calendar type for date conversion
    calendarType?: string;
}

/**
 * List response
 */
export interface PetitionWriterMonitoringListResponse {
    items: PetitionWriterMonitoringRecord[];
    totalCount: number;
    page: number;
    pageSize: number;
}

/**
 * Section types for dropdown
 */
export const PetitionWriterMonitoringSectionTypes = [
    { value: 'complaints', label: 'ثبت شکایات', labelEn: 'Complaints Registration' },
    { value: 'violations', label: 'تخلفات عریضه نویسان', labelEn: 'Petition Writer Violations' },
    { value: 'monitoring', label: 'نظارت فعالیت عریضه نویسان', labelEn: 'Monitoring Activities' }
];

/**
 * Afghan calendar months
 */
export const AfghanMonths = [
    { value: 'حمل', label: 'حمل (Hamal)' },
    { value: 'ثور', label: 'ثور (Saur)' },
    { value: 'جوزا', label: 'جوزا (Jawza)' },
    { value: 'سرطان', label: 'سرطان (Saratan)' },
    { value: 'اسد', label: 'اسد (Asad)' },
    { value: 'سنبله', label: 'سنبله (Sonbola)' },
    { value: 'میزان', label: 'میزان (Mizan)' },
    { value: 'عقرب', label: 'عقرب (Aqrab)' },
    { value: 'قوس', label: 'قوس (Qaws)' },
    { value: 'جدی', label: 'جدی (Jadi)' },
    { value: 'دلو', label: 'دلو (Dalw)' },
    { value: 'حوت', label: 'حوت (Hoot)' }
];
