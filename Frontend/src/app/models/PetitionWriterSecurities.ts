/**
 * سند بهادار عریضه‌ نویسان - Securities for Petition Writers
 */
export interface PetitionWriterSecurities {
    id?: number;

    // Tab 1: مشخصات عریضه‌نویس
    registrationNumber: string;
    petitionWriterName: string;
    petitionWriterFatherName: string;
    licenseNumber: string;

    // Tab 2: مشخصات سند بهادار عریضه
    petitionCount: number;
    amount: number;
    bankReceiptNumber: string;
    serialNumberStart: string;
    serialNumberEnd: string;
    distributionDate?: Date | string;
    distributionDateFormatted?: string;

    // Audit Fields
    createdAt?: Date;
    createdBy?: string;
    updatedAt?: Date;
    updatedBy?: string;
    status?: boolean;
}

/**
 * DTO for creating/updating petition writer securities
 */
export interface PetitionWriterSecuritiesData {
    id?: number;
    registrationNumber: string;
    petitionWriterName: string;
    petitionWriterFatherName: string;
    licenseNumber: string;
    petitionCount: number;
    amount: number;
    bankReceiptNumber: string;
    serialNumberStart: string;
    serialNumberEnd: string;
    distributionDate?: string;
}

/**
 * Paginated response for petition writer securities list
 */
export interface PetitionWriterSecuritiesListResponse {
    items: PetitionWriterSecurities[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}
