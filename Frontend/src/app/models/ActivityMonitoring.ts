/**
 * نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان
 * Monitoring of Real Estate Offices & Petition Writers Activities
 */
export interface ActivityMonitoringRecord {
    id?: number;
    
    // 1️⃣ Financial Clearance
    licenseHolderName: string;
    taxClearanceStatus?: string;
    taxClearanceLetterNumber?: string;
    taxClearanceDate?: Date | string;
    taxClearanceDateFormatted?: string;
    paidTaxAmount?: number;
    
    // 2️⃣ Annual Activity Report
    reportRegistrationDate?: Date | string;
    reportRegistrationDateFormatted?: string;
    saleDeedsCount?: number;
    rentalDeedsCount?: number;
    baiUlWafaDeedsCount?: number;
    vehicleTransactionDeedsCount?: number;
    cancelledMixedTransactions?: number;
    lostDeedsCount?: number;
    annualReportRemarks?: string;
    
    // 6️⃣ Inspection & Supervision
    inspectionDate?: Date | string;
    inspectionDateFormatted?: string;
    inspectedRealEstateOfficesCount?: number;
    sealedOfficesCount?: number;
    inspectedPetitionWritersCount?: number;
    violatingPetitionWritersCount?: number;
    
    // Audit
    status?: boolean;
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
}

/**
 * 3️⃣ Complaints Registration
 */
export interface Complaint {
    id?: number;
    activityMonitoringRecordId?: number;
    complaintSerialNumber: string;
    complainantName: string;
    complaintSubject: string;
    complaintRegistrationDate?: Date | string;
    complaintRegistrationDateFormatted?: string;
    accusedPartyName: string;
    actionsTaken?: string;
    remarks?: string;
    createdAt?: Date;
    createdBy?: string;
}

/**
 * 4️⃣ Violations – Real Estate Offices
 */
export interface RealEstateViolation {
    id?: number;
    activityMonitoringRecordId?: number;
    violationSerialNumber: string;
    licenseHolderName: string;
    violationType: string;
    violationDate?: Date | string;
    violationDateFormatted?: string;
    actionsTaken?: string;
    remarks?: string;
    createdAt?: Date;
    createdBy?: string;
}

/**
 * 5️⃣ Violations – Petition Writers
 */
export interface PetitionWriterViolation {
    id?: number;
    activityMonitoringRecordId?: number;
    violationSerialNumber: string;
    petitionWriterName: string;
    violationType: string;
    violationDate?: Date | string;
    violationDateFormatted?: string;
    actionsTaken?: string;
    remarks?: string;
    createdAt?: Date;
    createdBy?: string;
}

/**
 * DTO for creating/updating activity monitoring record
 */
export interface ActivityMonitoringData {
    id?: number;
    licenseHolderName: string;
    taxClearanceStatus?: string;
    taxClearanceLetterNumber?: string;
    taxClearanceDate?: string;
    paidTaxAmount?: number;
    reportRegistrationDate?: string;
    saleDeedsCount?: number;
    rentalDeedsCount?: number;
    baiUlWafaDeedsCount?: number;
    vehicleTransactionDeedsCount?: number;
    cancelledMixedTransactions?: number;
    lostDeedsCount?: number;
    annualReportRemarks?: string;
    inspectionDate?: string;
    inspectedRealEstateOfficesCount?: number;
    sealedOfficesCount?: number;
    inspectedPetitionWritersCount?: number;
    violatingPetitionWritersCount?: number;
    calendarType?: string;
}

/**
 * DTO for complaint
 */
export interface ComplaintData {
    id?: number;
    activityMonitoringRecordId?: number;
    complaintSerialNumber: string;
    complainantName: string;
    complaintSubject: string;
    complaintRegistrationDate?: string;
    accusedPartyName: string;
    actionsTaken?: string;
    remarks?: string;
    calendarType?: string;
}

/**
 * DTO for real estate violation
 */
export interface RealEstateViolationData {
    id?: number;
    activityMonitoringRecordId?: number;
    violationSerialNumber: string;
    licenseHolderName: string;
    violationType: string;
    violationDate?: string;
    actionsTaken?: string;
    remarks?: string;
    calendarType?: string;
}

/**
 * DTO for petition writer violation
 */
export interface PetitionWriterViolationData {
    id?: number;
    activityMonitoringRecordId?: number;
    violationSerialNumber: string;
    petitionWriterName: string;
    violationType: string;
    violationDate?: string;
    actionsTaken?: string;
    remarks?: string;
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
