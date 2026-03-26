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
 * Afghan calendar months (Hijri Shamsi)
 */
export const ShamsiMonths = [
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

/**
 * Hijri Qamari months (Islamic Lunar)
 */
export const QamariMonths = [
    { value: 'محرم', label: 'محرم (Muharram)' },
    { value: 'صفر', label: 'صفر (Safar)' },
    { value: 'ربیع الاول', label: 'ربیع الاول (Rabi al-Awwal)' },
    { value: 'ربیع الثانی', label: 'ربیع الثانی (Rabi al-Thani)' },
    { value: 'جمادی الاول', label: 'جمادی الاول (Jumada al-Awwal)' },
    { value: 'جمادی الثانی', label: 'جمادی الثانی (Jumada al-Thani)' },
    { value: 'رجب', label: 'رجب (Rajab)' },
    { value: 'شعبان', label: 'شعبان (Shaban)' },
    { value: 'رمضان', label: 'رمضان (Ramadan)' },
    { value: 'شوال', label: 'شوال (Shawwal)' },
    { value: 'ذی القعده', label: 'ذی القعده (Dhu al-Qadah)' },
    { value: 'ذی الحجه', label: 'ذی الحجه (Dhu al-Hijjah)' }
];

/**
 * Gregorian months (Miladi)
 */
export const GregorianMonths = [
    { value: 'January', label: 'جنوری (January)' },
    { value: 'February', label: 'فبروری (February)' },
    { value: 'March', label: 'مارچ (March)' },
    { value: 'April', label: 'اپریل (April)' },
    { value: 'May', label: 'می (May)' },
    { value: 'June', label: 'جون (June)' },
    { value: 'July', label: 'جولای (July)' },
    { value: 'August', label: 'اگست (August)' },
    { value: 'September', label: 'سپتمبر (September)' },
    { value: 'October', label: 'اکتوبر (October)' },
    { value: 'November', label: 'نومبر (November)' },
    { value: 'December', label: 'دسمبر (December)' }
];

/**
 * Legacy export for backward compatibility
 */
export const AfghanMonths = ShamsiMonths;
